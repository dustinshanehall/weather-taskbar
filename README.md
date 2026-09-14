# Weather Taskbar App

A lightweight Windows 11 weather application with **dual display modes**: taskbar button or system tray icon.

## Features

### Taskbar Mode (Default)
- **Taskbar Button**: Shows live weather info directly on your taskbar (e.g., "72°F Sunny - NYC")
- **Weather Icons**: Dynamic icons with symbols - sun ☀️, clouds ☁️, rain 🌧️, snow ❄️, lightning ⚡
- **Minimized by Default**: Stays in taskbar without cluttering your desktop
- **Detailed Window**: Click to see full forecast, hourly data, and controls
- **Auto-updates**: Weather refreshes automatically (configurable 5-60 minutes)

### System Tray Mode (Optional)
- **Move to Tray**: Right-click taskbar button → "Move to System Tray" to free up taskbar space
- **Temperature Icon**: Shows temperature number on system tray icon
- **Quick Restore**: Click tray icon to return to taskbar mode
- **Switch Anytime**: Toggle between modes as needed

### Smart Features
- **Sleep/Wake Detection**: Automatically refreshes weather when PC wakes from sleep/hibernate
- **6-Hour Forecast**: Hourly breakdown with temps and conditions
- **Minimal Resources**: ~20-25 MB RAM, <0.1% CPU when idle
- **Free Weather Data**: Uses Open-Meteo API (no API key required)
- **Single Instance**: Prevents multiple copies from running

## Prerequisites

- Windows 10 or 11 (x64)
- .NET 8.0 SDK — https://dotnet.microsoft.com/download/dotnet/8.0

No prebuilt binaries are published; build it yourself with the steps below.

## Building the Application

### Option 1: Using Command Line

1. Open Command Prompt or PowerShell
2. Clone and enter the repository:
   ```
   git clone https://github.com/dustinshanehall/weather-taskbar.git
   cd weather-taskbar
   ```

3. Build the project:
   ```
   dotnet build
   ```

4. Run the application:
   ```
   dotnet run --project WeatherTaskbarApp
   ```

### Option 2: Using Visual Studio

1. Open `WeatherTaskbarApp.sln` in Visual Studio 2022
2. Press F5 to build and run

### Creating a Standalone Executable

To create a single .exe file you can distribute:

```bash
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

The executable will be in: `WeatherTaskbarApp\bin\Release\net8.0-windows\win-x64\publish\`

For a completely self-contained version (no .NET required on target machine):

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## First-Time Setup

1. Run the application - it will appear in the system tray
2. Right-click the tray icon → **Settings**
3. Enter your city name or ZIP code
4. Choose Fahrenheit or Celsius
5. Select refresh interval and display mode
6. Click Save

## Usage

### Taskbar Mode
- **App starts minimized** in taskbar by default
- **Taskbar button** shows: "72°F Sunny - NYC" (auto-updates)
- **Left-click** taskbar button: Opens detailed weather window
- **Right-click** taskbar button → "Move to System Tray": Switch to tray mode
- **Close window (X button)**: Choose to minimize, move to tray, or exit

### System Tray Mode
- **Right-click** tray icon:
  - Restore to Taskbar: Return to taskbar mode
  - Settings: Configure location and preferences
  - Exit: Close the application
- **Left-click** tray icon: Return to taskbar mode
- **Tray icon** shows temperature number (e.g., "72°")
- **Tooltip** shows full weather (hover over icon)

### Weather Window
- **Large temperature display** with current conditions
- **6-hour forecast** panel with hourly breakdown
- **Refresh Now** button for manual updates
- **Settings** button to configure location, units, refresh interval

## Weather Conditions & Icons

The app displays these weather conditions with matching icons:

| Condition | Icon Symbol | Description |
|-----------|-------------|-------------|
| Sunny/Clear | ☀️ Sun with rays | Clear skies |
| Cloudy | ☁️ Cloud | Overcast or partly cloudy |
| Rainy | 🌧️ Cloud with raindrops | Rain or drizzle |
| Snowy | ❄️ Cloud with snowflakes | Snow or sleet |
| Stormy | ⚡ Cloud with lightning | Thunderstorms |
| Foggy | 🌫️ Fog lines | Fog or mist |

## Icon Colors

Icon backgrounds change based on temperature:
- **Blue**: Cold (< 32°F / 0°C)
- **Steel Blue**: Cool (32-50°F / 0-10°C)
- **Green**: Mild (50-70°F / 10-21°C)
- **Orange**: Warm (70-85°F / 21-29°C)
- **Red**: Hot (> 85°F / 29°C)

## Auto-start with Windows

Enable "Start automatically with Windows" in Settings to launch the app on boot.

## Data Source

Weather data provided by [Open-Meteo](https://open-meteo.com/) - a free weather API with no registration required.

## Troubleshooting

**App doesn't appear in taskbar:**
- Look for minimized window in taskbar (may be grouped with other windows)
- If moved to system tray, check notification area near clock

**Weather not updating after sleep/hibernate:**
- The app now auto-detects wake events and refreshes immediately
- If still stale, click "Refresh Now" in the window

**"Location not found" error:**
- Try different city name format (e.g., "New York" instead of "NYC")
- Use ZIP code instead of city name
- Check internet connection

**Want to switch between taskbar and tray:**
- **Taskbar → Tray**: Right-click title bar or taskbar button → "Move to System Tray"
- **Tray → Taskbar**: Left-click tray icon or right-click → "Restore to Taskbar"

**Taskbar button text not updating:**
- Window title updates even when minimized
- Check if window is actually running (look in Task Manager)
- Try manually refreshing

## Project Structure

```
WeatherTaskbarApp/
├── Models/
│   ├── WeatherData.cs           # Weather data models
│   └── AppSettings.cs           # Application settings
├── Services/
│   ├── WeatherService.cs        # Open-Meteo API integration
│   └── SettingsManager.cs       # Settings persistence
├── UI/
│   ├── WeatherIconGenerator.cs  # Dynamic weather icon rendering
│   ├── WeatherForm.cs           # Main weather window
│   ├── SettingsForm.cs          # Settings dialog
│   ├── AboutForm.cs             # About dialog
│   └── PrivacyPolicyForm.cs     # Privacy policy dialog
└── Program.cs                   # Entry point
```

## Privacy

The app has no accounts, analytics or telemetry, and no servers behind it. Settings live
in a local JSON file (`%APPDATA%\WeatherTaskbarApp\settings.json`). The only network
traffic is to Open-Meteo: the place name you type, and the coordinates it resolves to.
The full policy is in the app under **About > Privacy Policy**.

## License

Apache License 2.0 — see [LICENSE](LICENSE).

## Credits

- Weather data: [Open-Meteo](https://open-meteo.com/) (free, no API key required)
- Built with .NET 8 and Windows Forms
