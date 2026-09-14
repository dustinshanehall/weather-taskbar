namespace WeatherTaskbarApp.Models;

public class WeatherData
{
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public int WindDirection { get; set; }
    public int UVIndex { get; set; }
    public int WeatherCode { get; set; }
    public string Condition { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public List<HourlyForecast>? HourlyForecasts { get; set; }
    public List<DailyForecast>? DailyForecasts { get; set; }
}

public class HourlyForecast
{
    public DateTime Time { get; set; }
    public double Temperature { get; set; }
    public int WeatherCode { get; set; }
    public string Condition { get; set; } = string.Empty;
    public int PrecipitationProbability { get; set; }
}

public class DailyForecast
{
    public DateTime Date { get; set; }
    public double TemperatureMax { get; set; }
    public double TemperatureMin { get; set; }
    public int WeatherCode { get; set; }
    public string Condition { get; set; } = string.Empty;
    public int PrecipitationProbability { get; set; }
}
