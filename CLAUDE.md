# Weather Taskbar App — Project Notes

A lightweight Windows 11 weather app that shows live conditions on the taskbar button or
as a system-tray icon.

**Version:** 2.3.0
**Tech stack:** C# / .NET 8.0 / Windows Forms (net8.0-windows, x64)
**License:** Apache 2.0

## API Provider
- **Weather & Geocoding:** Open-Meteo (free, open-source, no API key required)
  - Weather: `api.open-meteo.com/v1/forecast`
  - Geocoding: `geocoding-api.open-meteo.com/v1/search`
  - Uses WMO weather interpretation codes mapped to condition strings

## Build Commands

Windows only — WinForms does not build or run on macOS/Linux, so this repo cannot be
compiled on the Mac.

```bash
# Default build (lightweight, requires .NET 8.0 on target)
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true

# Self-contained build (larger, no .NET runtime required on target)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Output: `WeatherTaskbarApp\bin\Release\net8.0-windows\win-x64\publish\`

## Key Files
- `WeatherTaskbarApp/Services/WeatherService.cs` — Weather API integration
- `WeatherTaskbarApp/Services/SettingsManager.cs` — Settings persistence (`%APPDATA%\WeatherTaskbarApp\settings.json`)
- `WeatherTaskbarApp/UI/WeatherForm.cs` — Main UI and forecast display
- `WeatherTaskbarApp/UI/AboutForm.cs` — About dialog (version, attribution, privacy link)
- `WeatherTaskbarApp/UI/PrivacyPolicyForm.cs` — Privacy policy text
- `WeatherTaskbarApp/Models/WeatherData.cs` — Data models
- `PROJECT_STATUS.md` — Detailed development history

## Privacy Position
No accounts, no analytics, no telemetry, no crash reporting, no servers. The app does not
read device location — the user types a city or ZIP. The only outbound traffic is to
Open-Meteo. Keep it that way: any change that adds a network call or an identifier needs
the in-app privacy policy (`PrivacyPolicyForm.cs`) and the README Privacy section updated
in the same commit.

## Current Issues / TODO
1. ~~Migrate from Open-Meteo to Visual Crossing API~~ ✓
2. ~~Migrate back to Open-Meteo (free, no API key)~~ ✓
3. ~~Fix right edge cutoff on final day forecast card~~ ✓
4. ~~Only show precipitation % if >= 10%~~ ✓
5. ~~Add About screen~~ ✓
6. ~~Add Privacy Policy~~ ✓
7. ~~Improve system tray icon~~ ✓
8. ~~Center main window buttons~~ ✓
9. ~~Add API rate limiting~~ ✓
10. No app icon resource — the window/exe uses the default WinForms icon
11. No automated tests

## Attribution Requirements
- Open-Meteo: attribution shown in the About screen ✓

## Session Notes
- **2026-09-14:** Moved to a personal GitHub account and relicensed under Apache 2.0.
  Privacy policy rewritten to match what the code actually does (the old text claimed the
  app requested device location — it never did). Prebuilt `releases/*.exe` dropped from the
  repo; use GitHub Releases if binaries are published later.
- **2026-02-13 (v2.3.0):** Migrated back from Visual Crossing to Open-Meteo. Open-Meteo is
  free with no API key needed. Attempted a UI overhaul (gradients, glassmorphic cards,
  emoji icons) but reverted — WinForms emoji rendering was monochrome and the gradients
  were too vivid. Kept the original UI, only changed the API provider.
