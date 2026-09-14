namespace WeatherTaskbarApp.Models;

public class AppSettings
{
    public string LocationName { get; set; } = "New York";
    public double Latitude { get; set; } = 40.7128;
    public double Longitude { get; set; } = -74.0060;
    public string TemperatureUnit { get; set; } = "fahrenheit"; // "fahrenheit" or "celsius"
    public int RefreshIntervalMinutes { get; set; } = 15;
    public TextDisplayMode DisplayMode { get; set; } = TextDisplayMode.WordOnly;
    public ThemeMode Theme { get; set; } = ThemeMode.System;
    public bool AutoStart { get; set; } = false;
}

public enum TextDisplayMode
{
    WordOnly,           // "Sunny"
    WithLocation,       // "NYC Sunny"
    Extended            // "NYC Sunny     " (padded)
}

public enum ThemeMode
{
    Light,
    Dark,
    System              // Match OS theme
}
