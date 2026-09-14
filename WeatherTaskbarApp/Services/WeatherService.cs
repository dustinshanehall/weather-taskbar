using System.Text.Json;
using WeatherTaskbarApp.Models;

namespace WeatherTaskbarApp.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private WeatherData? _cachedWeather;

    private const string WeatherBaseUrl = "https://api.open-meteo.com/v1/forecast";
    private const string GeocodingBaseUrl = "https://geocoding-api.open-meteo.com/v1/search";

    public WeatherService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    public async Task<WeatherData?> GetWeatherAsync(double latitude, double longitude, string unit)
    {
        try
        {
            string tempUnit = unit.ToLower() == "celsius" ? "celsius" : "fahrenheit";
            string windUnit = unit.ToLower() == "celsius" ? "kmh" : "mph";

            string url = $"{WeatherBaseUrl}?latitude={latitude}&longitude={longitude}" +
                        $"&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code,wind_speed_10m,wind_direction_10m,uv_index" +
                        $"&hourly=temperature_2m,weather_code,precipitation_probability" +
                        $"&daily=temperature_2m_max,temperature_2m_min,weather_code,precipitation_probability_max" +
                        $"&temperature_unit={tempUnit}&wind_speed_unit={windUnit}" +
                        $"&timezone=auto&forecast_days=7";

            var response = await _httpClient.GetStringAsync(url);
            var jsonDoc = JsonDocument.Parse(response);
            var root = jsonDoc.RootElement;

            // Parse current conditions
            var current = root.GetProperty("current");
            double temperature = current.GetProperty("temperature_2m").GetDouble();
            double feelsLike = current.GetProperty("apparent_temperature").GetDouble();
            int humidity = current.GetProperty("relative_humidity_2m").GetInt32();
            double windSpeed = current.GetProperty("wind_speed_10m").GetDouble();
            int windDirection = current.GetProperty("wind_direction_10m").GetInt32();
            int uvIndex = (int)Math.Round(current.TryGetProperty("uv_index", out var uv) ? uv.GetDouble() : 0);
            int weatherCode = current.GetProperty("weather_code").GetInt32();

            var weatherData = new WeatherData
            {
                Temperature = temperature,
                FeelsLike = feelsLike,
                Humidity = humidity,
                WindSpeed = windSpeed,
                WindDirection = windDirection,
                UVIndex = uvIndex,
                WeatherCode = weatherCode,
                Condition = MapWmoCode(weatherCode),
                LastUpdated = DateTime.Now,
                HourlyForecasts = new List<HourlyForecast>(),
                DailyForecasts = new List<DailyForecast>()
            };

            // Parse hourly forecast - next 6 hours from current time
            if (root.TryGetProperty("hourly", out var hourly))
            {
                var times = hourly.GetProperty("time");
                var temps = hourly.GetProperty("temperature_2m");
                var codes = hourly.GetProperty("weather_code");
                var precipProbs = hourly.GetProperty("precipitation_probability");

                DateTime now = DateTime.Now;
                int futureHoursFound = 0;

                for (int i = 0; i < times.GetArrayLength() && futureHoursFound < 6; i++)
                {
                    DateTime forecastTime = DateTime.Parse(times[i].GetString() ?? "");
                    if (forecastTime > now)
                    {
                        int code = codes[i].GetInt32();
                        int precip = precipProbs[i].ValueKind == JsonValueKind.Null ? 0 : precipProbs[i].GetInt32();

                        weatherData.HourlyForecasts.Add(new HourlyForecast
                        {
                            Time = forecastTime,
                            Temperature = temps[i].GetDouble(),
                            WeatherCode = code,
                            Condition = MapWmoCode(code),
                            PrecipitationProbability = precip
                        });
                        futureHoursFound++;
                    }
                }
            }

            // Parse daily forecast for 7 days
            if (root.TryGetProperty("daily", out var daily))
            {
                var times = daily.GetProperty("time");
                var maxTemps = daily.GetProperty("temperature_2m_max");
                var minTemps = daily.GetProperty("temperature_2m_min");
                var codes = daily.GetProperty("weather_code");
                var precipProbs = daily.GetProperty("precipitation_probability_max");

                int count = Math.Min(7, times.GetArrayLength());
                for (int i = 0; i < count; i++)
                {
                    int code = codes[i].GetInt32();
                    int precip = precipProbs[i].ValueKind == JsonValueKind.Null ? 0 : precipProbs[i].GetInt32();

                    weatherData.DailyForecasts.Add(new DailyForecast
                    {
                        Date = DateTime.Parse(times[i].GetString() ?? DateTime.Today.ToString("yyyy-MM-dd")),
                        TemperatureMax = maxTemps[i].GetDouble(),
                        TemperatureMin = minTemps[i].GetDouble(),
                        WeatherCode = code,
                        Condition = MapWmoCode(code),
                        PrecipitationProbability = precip
                    });
                }
            }

            _cachedWeather = weatherData;
            return weatherData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching weather: {ex.Message}");
            return _cachedWeather; // Return cached data on error
        }
    }

    public async Task<(double latitude, double longitude, string name)?> GeocodeLocationAsync(string locationQuery)
    {
        try
        {
            string url = $"{GeocodingBaseUrl}?name={Uri.EscapeDataString(locationQuery)}&count=1&language=en&format=json";

            var response = await _httpClient.GetStringAsync(url);
            var jsonDoc = JsonDocument.Parse(response);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
            {
                Console.WriteLine($"No geocoding results for: {locationQuery}");
                return null;
            }

            var first = results[0];
            double lat = first.GetProperty("latitude").GetDouble();
            double lon = first.GetProperty("longitude").GetDouble();
            string name = first.GetProperty("name").GetString() ?? locationQuery;

            // Build a friendly display name (e.g. "New York, New York, United States")
            string? admin1 = first.TryGetProperty("admin1", out var a1) ? a1.GetString() : null;
            string? country = first.TryGetProperty("country", out var c) ? c.GetString() : null;

            string displayName = name;
            if (!string.IsNullOrEmpty(admin1) && admin1 != name)
                displayName += $", {admin1}";
            if (!string.IsNullOrEmpty(country))
                displayName += $", {country}";

            return (lat, lon, displayName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error geocoding location: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Maps WMO weather interpretation codes to standard condition strings.
    /// </summary>
    private static string MapWmoCode(int code)
    {
        return code switch
        {
            0 => "Sunny",      // Clear sky
            1 => "Sunny",      // Mainly clear
            2 => "Cloudy",     // Partly cloudy
            3 => "Cloudy",     // Overcast
            45 or 48 => "Foggy",  // Fog / depositing rime fog
            51 or 53 or 55 => "Rainy",  // Drizzle (light/moderate/dense)
            56 or 57 => "Rainy",        // Freezing drizzle
            61 or 63 or 65 => "Rainy",  // Rain (slight/moderate/heavy)
            66 or 67 => "Rainy",        // Freezing rain
            71 or 73 or 75 => "Snowy",  // Snow fall (slight/moderate/heavy)
            77 => "Snowy",              // Snow grains
            80 or 81 => "Rainy",        // Rain showers (slight/moderate)
            82 => "Stormy",             // Rain showers (violent)
            85 or 86 => "Snowy",        // Snow showers (slight/heavy)
            95 => "Stormy",             // Thunderstorm
            96 or 99 => "Stormy",       // Thunderstorm with hail
            _ => "Cloudy"               // Unknown - safe fallback
        };
    }
}
