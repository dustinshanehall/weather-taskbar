# Weather Taskbar App - Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]
### Changed
- Relicensed under the Apache License 2.0 and moved to a personal GitHub repository
- About screen now links to the project source repository instead of a company site
- Privacy Policy rewritten to describe what the app actually does: it never reads device
  location, and the only outbound traffic is the Open-Meteo request

### Removed
- Prebuilt `releases/*.exe` binaries from the repository (build from source instead)

---

## [2.3.0] - 2026-02-13
### Changed
- Migrated back from Visual Crossing to Open-Meteo API (free, open-source, no API key required)
- Weather data now uses WMO weather interpretation codes
- Geocoding now uses Open-Meteo's geocoding API
- Updated About screen attribution to Open-Meteo
- Updated Privacy Policy to reference Open-Meteo

---

## [2.2.0] - 2026-01-29
### Changed
- Improved button styling: larger corner radius (8px), taller buttons (40px), better spacing (12px)
- Added subtle border to secondary buttons for better visual definition
- About screen now links to the project home page

### Fixed
- About button now properly updates with theme changes

---

## [2.1.0] - 2026-01-25
### Changed
- Migrated weather API from Open-Meteo to Visual Crossing
- Migrated geocoding from Open-Meteo to Visual Crossing (single API provider)
- Precipitation percentage now only displays if >= 10% (was > 0%)
- Increased window width from 620px to 635px to accommodate 7-day forecast
- Centered the four main buttons (Refresh, Settings, About, Move to Tray)

### Fixed
- Right edge cutoff on final day forecast card (expanded panel from 600px to 615px)

### Added
- About screen with app info, version, attribution, and privacy policy link
- Privacy Policy screen
- API rate limiting: Refresh button limited to one API call per 60 seconds
- Redesigned system tray icon with gradient blue background, rounded corners, and degree symbol
- About button in main window

---

## [2.0.0] - 2026-01-16
### Added
- Sky-inspired gradient background that adapts to weather conditions
- Temperature-based color coding (blue for cold, red for hot)
- Weather condition colors (sunny=yellow, rainy=blue, etc.)
- Modern rounded buttons with hover effects
- Colored forecast indicators (4px left bar on cards)
- Dynamic temperature colors in forecast entries
- Rounded forecast panel border

### Changed
- Complete visual overhaul with professional weather-appropriate aesthetic

---

## [1.0.0] - 2026-01-16
### Added
- Dual mode display (taskbar button and system tray icon)
- Dynamic weather icons with condition symbols
- Sleep/wake detection for auto-refresh
- 6-hour hourly forecast display (future hours only)
- 7-day daily forecast with high/low temps
- Configurable refresh intervals (5-60 minutes)
- Auto-start with Windows option
- Single instance enforcement
- Window resizing support with anchored controls
- Centered forecast display with weather icons
- System menu with "Move to Tray" and "Exit" options
- Temperature display on system tray icon

### Technical
- Built with .NET 8.0 and Windows Forms
- Visual Crossing API for weather data
- Target platform: Windows x64
- Memory footprint: ~20-25 MB when idle
