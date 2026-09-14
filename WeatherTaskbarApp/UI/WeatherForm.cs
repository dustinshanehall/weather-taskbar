using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using WeatherTaskbarApp.Models;
using WeatherTaskbarApp.Services;

namespace WeatherTaskbarApp.UI;

// Theme color definitions
internal static class ThemeColors
{
    // Light theme colors
    public static class Light
    {
        public static readonly Color FormBackground = Color.FromArgb(250, 251, 252);
        public static readonly Color PanelBackground = Color.FromArgb(252, 253, 255);
        public static readonly Color CardBackground = Color.FromArgb(245, 247, 250);
        public static readonly Color PrimaryText = Color.FromArgb(50, 50, 50);
        public static readonly Color SecondaryText = Color.FromArgb(100, 100, 100);
        public static readonly Color TertiaryText = Color.FromArgb(120, 120, 120);
        public static readonly Color MutedText = Color.FromArgb(150, 150, 150);
        public static readonly Color ForecastText = Color.FromArgb(70, 80, 95);
        public static readonly Color ForecastSecondary = Color.FromArgb(100, 110, 125);
        public static readonly Color HighLowText = Color.FromArgb(100, 110, 120);
        public static readonly Color LocationText = Color.FromArgb(90, 100, 115);
        public static readonly Color SlashText = Color.FromArgb(120, 130, 145);
        public static readonly Color ConditionText = Color.FromArgb(80, 90, 100);
        public static readonly Color ButtonPrimary = Color.FromArgb(87, 167, 230);
        public static readonly Color ButtonPrimaryHover = Color.FromArgb(65, 145, 210);
        public static readonly Color ButtonPrimaryPressed = Color.FromArgb(45, 120, 190);
        public static readonly Color ButtonPrimaryText = Color.White;
        public static readonly Color ButtonSecondary = Color.FromArgb(236, 240, 245);
        public static readonly Color ButtonSecondaryHover = Color.FromArgb(220, 225, 235);
        public static readonly Color ButtonSecondaryPressed = Color.FromArgb(200, 210, 225);
        public static readonly Color ButtonSecondaryText = Color.FromArgb(60, 70, 85);
    }

    // Dark theme colors
    public static class Dark
    {
        public static readonly Color FormBackground = Color.FromArgb(30, 32, 38);
        public static readonly Color PanelBackground = Color.FromArgb(38, 40, 48);
        public static readonly Color CardBackground = Color.FromArgb(45, 48, 56);
        public static readonly Color PrimaryText = Color.FromArgb(240, 240, 245);
        public static readonly Color SecondaryText = Color.FromArgb(180, 185, 195);
        public static readonly Color TertiaryText = Color.FromArgb(150, 155, 165);
        public static readonly Color MutedText = Color.FromArgb(120, 125, 135);
        public static readonly Color ForecastText = Color.FromArgb(210, 215, 225);
        public static readonly Color ForecastSecondary = Color.FromArgb(160, 165, 180);
        public static readonly Color HighLowText = Color.FromArgb(170, 175, 185);
        public static readonly Color LocationText = Color.FromArgb(160, 165, 180);
        public static readonly Color SlashText = Color.FromArgb(130, 135, 150);
        public static readonly Color ConditionText = Color.FromArgb(180, 185, 195);
        public static readonly Color ButtonPrimary = Color.FromArgb(70, 140, 200);
        public static readonly Color ButtonPrimaryHover = Color.FromArgb(85, 155, 215);
        public static readonly Color ButtonPrimaryPressed = Color.FromArgb(55, 125, 185);
        public static readonly Color ButtonPrimaryText = Color.White;
        public static readonly Color ButtonSecondary = Color.FromArgb(55, 58, 68);
        public static readonly Color ButtonSecondaryHover = Color.FromArgb(70, 75, 85);
        public static readonly Color ButtonSecondaryPressed = Color.FromArgb(50, 53, 62);
        public static readonly Color ButtonSecondaryText = Color.FromArgb(200, 205, 215);
    }
}

internal static class WeatherColorHelper
{
    public static Color GetTemperatureColor(double temperature, string unit)
    {
        // Convert to Fahrenheit for consistent thresholds
        double tempF = unit.ToLower() == "celsius"
            ? (temperature * 9.0 / 5.0) + 32
            : temperature;

        return tempF switch
        {
            < 32 => Color.FromArgb(70, 140, 220),    // Freezing - cold blue
            < 50 => Color.FromArgb(85, 180, 240),    // Cold - cool blue
            < 65 => Color.FromArgb(90, 200, 180),    // Cool - teal
            < 75 => Color.FromArgb(100, 200, 100),   // Moderate - green
            < 85 => Color.FromArgb(255, 180, 80),    // Warm - orange
            < 95 => Color.FromArgb(255, 120, 80),    // Hot - red-orange
            _ => Color.FromArgb(240, 70, 70)         // Very hot - red
        };
    }

    public static Color GetConditionColor(string condition)
    {
        return condition.ToLower() switch
        {
            "sunny" => Color.FromArgb(255, 200, 60),     // Golden yellow
            "clear" => Color.FromArgb(255, 220, 100),    // Light yellow
            "cloudy" => Color.FromArgb(140, 150, 165),   // Gray
            "rainy" => Color.FromArgb(70, 140, 200),     // Rain blue
            "snowy" => Color.FromArgb(200, 220, 240),    // Snow white-blue
            "stormy" => Color.FromArgb(60, 70, 90),      // Storm dark
            "foggy" => Color.FromArgb(180, 185, 195),    // Fog gray
            _ => Color.FromArgb(87, 167, 230)            // Default sky blue
        };
    }

    public static Color GetBackgroundGradientTop(string condition)
    {
        return condition.ToLower() switch
        {
            "sunny" or "clear" => Color.FromArgb(250, 245, 220),  // Sunny sky
            "cloudy" => Color.FromArgb(230, 235, 242),            // Overcast
            "rainy" => Color.FromArgb(220, 230, 240),             // Rainy sky
            "snowy" => Color.FromArgb(240, 245, 250),             // Snowy sky
            "stormy" => Color.FromArgb(210, 215, 225),            // Storm sky
            "foggy" => Color.FromArgb(235, 238, 242),             // Foggy
            _ => Color.FromArgb(245, 250, 255)                    // Default
        };
    }
}

public class WeatherForm : Form
{
    // Windows API for system menu
    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("user32.dll")]
    private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

    private const int MF_STRING = 0x0;
    private const int MF_SEPARATOR = 0x800;
    private const int WM_SYSCOMMAND = 0x112;
    private const int SYSMENU_MOVE_TO_TRAY = 0x1;
    private const int SYSMENU_EXIT = 0x2;

    // Extended window style for proper compositing (reduces scroll artifacts)
    private const int WS_EX_COMPOSITED = 0x02000000;

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= WS_EX_COMPOSITED;
            return cp;
        }
    }

    private readonly WeatherService _weatherService;
    private readonly SettingsManager _settingsManager;
    private AppSettings _settings;
    private WeatherData? _currentWeather;
    private System.Windows.Forms.Timer? _refreshTimer;
    private NotifyIcon? _notifyIcon;
    private bool _isTrayMode = false;
    private string _currentCondition = "Clear";
    private bool _isDarkMode = false;
    private DateTime _lastApiCallTime = DateTime.MinValue;
    private const int API_RATE_LIMIT_SECONDS = 60;

    // UI Controls
    private Label _temperatureLabel;
    private Label _highLowLabel;
    private Label _conditionLabel;
    private Label _detailsLabel;
    private Label _locationLabel;
    private Label _lastUpdatedLabel;
    private Panel _forecastPanel;
    private ModernButton _refreshButton;
    private ModernButton _settingsButton;
    private ModernButton _aboutButton;
    private ModernButton _moveToTrayButton;
    private List<Panel> _forecastRowPanels = new List<Panel>();
    private List<Label> _forecastTitleLabels = new List<Label>();

    public WeatherForm()
    {
        _weatherService = new WeatherService();
        _settingsManager = new SettingsManager();
        _settings = _settingsManager.LoadSettings();

        InitializeForm();
        InitializeControls();
        SetupSystemTray();
        SetupRefreshTimer();
        SetupPowerManagement();
        ApplyTheme();

        // Initial weather fetch
        _ = RefreshWeatherAsync();

        // Start minimized
        WindowState = FormWindowState.Minimized;
    }

    private void InitializeForm()
    {
        Text = "Weather - Loading...";
        Size = new Size(635, 570);
        MinimumSize = new Size(635, 520);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimizeBox = true;
        ShowInTaskbar = true;

        // Enable double buffering for smooth gradient rendering
        DoubleBuffered = true;
        SetStyle(ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint, true);
        Paint += OnFormPaint;

        // Handle window state changes
        Resize += OnResize;

        // Add custom system menu item
        Load += OnFormLoad;
    }

    private void OnFormLoad(object? sender, EventArgs e)
    {
        // Add custom items to the system menu
        IntPtr systemMenu = GetSystemMenu(Handle, false);
        AppendMenu(systemMenu, MF_SEPARATOR, 0, string.Empty);
        AppendMenu(systemMenu, MF_STRING, SYSMENU_MOVE_TO_TRAY, "Move to System Tray");
        AppendMenu(systemMenu, MF_STRING, SYSMENU_EXIT, "Exit");
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_SYSCOMMAND)
        {
            if (m.WParam.ToInt32() == SYSMENU_MOVE_TO_TRAY)
            {
                SwitchToTrayMode();
                return;
            }
            else if (m.WParam.ToInt32() == SYSMENU_EXIT)
            {
                Application.Exit();
                return;
            }
        }
        base.WndProc(ref m);
    }

    private void OnFormPaint(object? sender, PaintEventArgs e)
    {
        if (_isDarkMode)
        {
            // Dark mode: subtle dark gradient
            Color topColor = Color.FromArgb(35, 38, 45);
            Color bottomColor = ThemeColors.Dark.FormBackground;

            using (var brush = new LinearGradientBrush(
                ClientRectangle,
                topColor,
                bottomColor,
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
        }
        else
        {
            // Light mode: gradient based on weather condition
            Color topColor = WeatherColorHelper.GetBackgroundGradientTop(_currentCondition);
            Color bottomColor = Color.FromArgb(250, 251, 252); // Misty white

            using (var brush = new LinearGradientBrush(
                ClientRectangle,
                topColor,
                bottomColor,
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
        }
    }

    private void InitializeControls()
    {
        // Temperature display
        _temperatureLabel = new Label
        {
            Text = "--°",
            Font = new Font("Segoe UI", 48, FontStyle.Bold),
            ForeColor = Color.FromArgb(50, 50, 50),
            BackColor = Color.Transparent,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(600, 80),
            Location = new Point(10, 15),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        // Today's high/low display
        _highLowLabel = new Label
        {
            Text = "H: --° L: --°",
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 100, 100),
            BackColor = Color.Transparent,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(600, 25),
            Location = new Point(10, 90),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        // Condition display
        _conditionLabel = new Label
        {
            Text = "Loading...",
            Font = new Font("Segoe UI", 20, FontStyle.Regular),
            ForeColor = Color.FromArgb(80, 80, 80),
            BackColor = Color.Transparent,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(600, 35),
            Location = new Point(10, 115),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        // Weather details (feels like, humidity, wind, UV)
        _detailsLabel = new Label
        {
            Text = "",
            Font = new Font("Segoe UI", 9, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 100, 100),
            BackColor = Color.Transparent,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(600, 18),
            Location = new Point(10, 150),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        // Location display
        _locationLabel = new Label
        {
            Text = _settings.LocationName,
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            ForeColor = Color.FromArgb(120, 120, 120),
            BackColor = Color.Transparent,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(600, 25),
            Location = new Point(10, 170),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        // Last updated
        _lastUpdatedLabel = new Label
        {
            Text = "Last updated: --",
            Font = new Font("Segoe UI", 9, FontStyle.Italic),
            ForeColor = Color.FromArgb(150, 150, 150),
            BackColor = Color.Transparent,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(600, 20),
            Location = new Point(10, 197),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        // Forecast panel - solid background prevents scroll artifacts
        _forecastPanel = new DoubleBufferedPanel
        {
            Location = new Point(10, 222),
            Size = new Size(615, 253),
            BackColor = Color.FromArgb(252, 253, 255), // Very light solid background
            BorderStyle = BorderStyle.None,
            AutoScroll = false, // Disable scrollbars - content should fit
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        _forecastPanel.Resize += OnForecastPanelResize;

        // Refresh button (primary style)
        _refreshButton = new ModernButton(isPrimary: true)
        {
            Text = "Refresh Now",
            Size = new Size(130, 40),
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            Anchor = AnchorStyles.Bottom
        };
        _refreshButton.Click += async (s, e) => await RefreshWeatherAsync();

        // Settings button (secondary style)
        _settingsButton = new ModernButton(isPrimary: false)
        {
            Text = "Settings",
            Size = new Size(90, 40),
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            Anchor = AnchorStyles.Bottom
        };
        _settingsButton.Click += OnSettingsClick;

        // About button (secondary style)
        _aboutButton = new ModernButton(isPrimary: false)
        {
            Text = "About",
            Size = new Size(75, 40),
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            Anchor = AnchorStyles.Bottom
        };
        _aboutButton.Click += OnAboutClick;

        // Move to Tray button (secondary style)
        _moveToTrayButton = new ModernButton(isPrimary: false)
        {
            Text = "Move to Tray",
            Size = new Size(110, 40),
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            Anchor = AnchorStyles.Bottom
        };
        _moveToTrayButton.Click += (s, e) => SwitchToTrayMode();

        // Center buttons initially
        CenterButtons();

        Controls.AddRange(new Control[]
        {
            _temperatureLabel,
            _highLowLabel,
            _conditionLabel,
            _detailsLabel,
            _locationLabel,
            _lastUpdatedLabel,
            _forecastPanel,
            _refreshButton,
            _settingsButton,
            _aboutButton,
            _moveToTrayButton
        });
    }

    private void SetupSystemTray()
    {
        _notifyIcon = new NotifyIcon
        {
            Visible = false,
            Text = "Weather App"
        };
        _notifyIcon.MouseClick += OnTrayIconClick;

        var trayMenu = new ContextMenuStrip();

        var restoreItem = new ToolStripMenuItem("Restore to Taskbar");
        restoreItem.Click += (s, e) => SwitchToTaskbarMode();

        var settingsItem = new ToolStripMenuItem("Settings...");
        settingsItem.Click += OnSettingsClick;

        var exitItem = new ToolStripMenuItem("Exit");
        exitItem.Click += (s, e) => Application.Exit();

        trayMenu.Items.AddRange(new ToolStripItem[] { restoreItem, settingsItem, new ToolStripSeparator(), exitItem });
        _notifyIcon.ContextMenuStrip = trayMenu;
    }

    private void SetupRefreshTimer()
    {
        _refreshTimer?.Dispose();

        _refreshTimer = new System.Windows.Forms.Timer
        {
            Interval = _settings.RefreshIntervalMinutes * 60 * 1000
        };
        _refreshTimer.Tick += async (s, e) => await RefreshWeatherAsync();
        _refreshTimer.Start();
    }

    private void SetupPowerManagement()
    {
        SystemEvents.PowerModeChanged += OnPowerModeChanged;
    }

    private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        if (e.Mode == PowerModes.Resume)
        {
            // System woke up from sleep/hibernate - refresh immediately
            _ = RefreshWeatherAsync();
        }
    }

    private async Task RefreshWeatherAsync(bool bypassRateLimit = false)
    {
        // Rate limiting: only allow one API call per minute unless bypassed
        var timeSinceLastCall = DateTime.Now - _lastApiCallTime;
        if (!bypassRateLimit && timeSinceLastCall.TotalSeconds < API_RATE_LIMIT_SECONDS)
        {
            // Show cached data if available, skip API call
            if (_currentWeather != null)
            {
                UpdateDisplay(_currentWeather);
            }
            return;
        }

        try
        {
            _lastApiCallTime = DateTime.Now;
            var weather = await _weatherService.GetWeatherAsync(
                _settings.Latitude,
                _settings.Longitude,
                _settings.TemperatureUnit);

            if (weather != null)
            {
                _currentWeather = weather;
                UpdateDisplay(weather);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refreshing weather: {ex.Message}");
        }
    }

    private void UpdateDisplay(WeatherData weather)
    {
        int temp = (int)Math.Round(weather.Temperature);
        string unit = _settings.TemperatureUnit.ToLower() == "celsius" ? "C" : "F";

        // Get dynamic colors based on temperature and condition
        Color tempColor = WeatherColorHelper.GetTemperatureColor(weather.Temperature, _settings.TemperatureUnit);
        Color conditionColor = WeatherColorHelper.GetConditionColor(weather.Condition);

        // Update window controls with dynamic colors
        _temperatureLabel.Text = $"{temp}°{unit}";
        _temperatureLabel.ForeColor = tempColor;

        // Update today's high/low from daily forecast
        var todayForecast = weather.DailyForecasts?.FirstOrDefault(f => f.Date.Date == DateTime.Today);
        if (todayForecast != null)
        {
            int high = (int)Math.Round(todayForecast.TemperatureMax);
            int low = (int)Math.Round(todayForecast.TemperatureMin);
            _highLowLabel.Text = $"H: {high}°  L: {low}°";

            // Color the high/low based on their temperatures
            Color highColor = WeatherColorHelper.GetTemperatureColor(todayForecast.TemperatureMax, _settings.TemperatureUnit);
            Color lowColor = WeatherColorHelper.GetTemperatureColor(todayForecast.TemperatureMin, _settings.TemperatureUnit);
            // Use a blended/neutral color since we can't color parts of a label differently
            _highLowLabel.ForeColor = Color.FromArgb(100, 110, 120);
        }
        else
        {
            _highLowLabel.Text = "";
        }

        _conditionLabel.Text = weather.Condition;
        _conditionLabel.ForeColor = conditionColor;

        // Weather details line
        int feelsLike = (int)Math.Round(weather.FeelsLike);
        string windDir = GetWindDirection(weather.WindDirection);
        int windSpeed = (int)Math.Round(weather.WindSpeed);
        string windUnit = _settings.TemperatureUnit.ToLower() == "celsius" ? "km/h" : "mph";
        _detailsLabel.Text = $"Feels {feelsLike}°  •  {weather.Humidity}% humidity  •  Wind {windDir} {windSpeed} {windUnit}  •  UV {weather.UVIndex}";

        _locationLabel.Text = _settings.LocationName;
        _locationLabel.ForeColor = Color.FromArgb(90, 100, 115);

        _lastUpdatedLabel.Text = $"Last updated: {weather.LastUpdated:h:mm tt}";
        _lastUpdatedLabel.ForeColor = Color.FromArgb(140, 150, 165);

        // Update window title (shows on taskbar button) - no location name
        Text = $"{temp}°{unit} {weather.Condition}";

        // Update window icon
        Icon = WeatherIconGenerator.GenerateWeatherIcon(weather.Condition, temp, _settings.TemperatureUnit);

        // Update form background gradient based on condition
        _currentCondition = weather.Condition;
        Invalidate(); // Trigger repaint with new gradient

        // Update tray icon if in tray mode
        if (_isTrayMode && _notifyIcon != null)
        {
            _notifyIcon.Icon = WeatherIconGenerator.GenerateTrayIcon(temp, _settings.TemperatureUnit);
            _notifyIcon.Text = $"{temp}°{unit} - {weather.Condition}";
        }

        // Update forecast panel
        UpdateForecastPanel(weather);
    }

    private void OnForecastPanelResize(object? sender, EventArgs e)
    {
        // Re-center all row panels and update title widths when the forecast panel is resized
        int panelWidth = _forecastPanel.ClientSize.Width;

        // Update title label widths and keep them at x=0 for centering
        foreach (Label titleLabel in _forecastTitleLabels)
        {
            if (titleLabel != null && !titleLabel.IsDisposed)
            {
                titleLabel.Width = panelWidth;
                titleLabel.Left = 0;
            }
        }

        // Re-center all forecast row panels (card containers)
        foreach (Panel rowPanel in _forecastRowPanels)
        {
            if (rowPanel != null && !rowPanel.IsDisposed)
            {
                rowPanel.Left = (panelWidth - rowPanel.Width) / 2;
            }
        }
    }

    private Panel CreateForecastRow(string condition, Bitmap icon, string timeOrDay, double temperature, string unit, int precipProbability, double? temperatureMin = null)
    {
        // Fixed column widths for consistent alignment
        const int ROW_WIDTH = 330;
        const int INDICATOR_WIDTH = 4;
        const int ICON_WIDTH = 24;
        const int DAY_COLUMN_WIDTH = 70;  // Fixed width for day/time column
        const int TEMP_COLUMN_WIDTH = 85; // Fixed width for temperature column
        const int PRECIP_COLUMN_WIDTH = 40; // Fixed width for precipitation %

        // Create container panel with fixed width (not AutoSize)
        var rowPanel = new DoubleBufferedPanel
        {
            Height = 32,
            Width = ROW_WIDTH,
            BackColor = Color.FromArgb(250, 251, 253) // Slight background for readability
        };

        // Add colored left indicator bar
        Color indicatorColor = WeatherColorHelper.GetConditionColor(condition);
        var indicator = new Panel
        {
            Width = INDICATOR_WIDTH,
            Height = 32,
            Location = new Point(0, 0),
            BackColor = indicatorColor
        };
        rowPanel.Controls.Add(indicator);

        // Weather icon
        var iconBox = new PictureBox
        {
            Image = icon,
            Size = new Size(20, 20),
            Location = new Point(INDICATOR_WIDTH + 4, 6),
            SizeMode = PictureBoxSizeMode.StretchImage,
            BackColor = Color.Transparent
        };
        rowPanel.Controls.Add(iconBox);

        int xPos = INDICATOR_WIDTH + ICON_WIDTH + 8;

        // Time/Day label - fixed width for alignment
        var timeLabel = new Label
        {
            Text = timeOrDay,
            Font = new Font("Segoe UI", 10),
            Location = new Point(xPos, 6),
            Size = new Size(DAY_COLUMN_WIDTH, 20),
            AutoSize = false,
            ForeColor = Color.FromArgb(70, 80, 95),
            BackColor = Color.Transparent
        };
        rowPanel.Controls.Add(timeLabel);
        xPos += DAY_COLUMN_WIDTH;

        // Temperature label(s) with dynamic color - fixed width column
        Color tempColor = WeatherColorHelper.GetTemperatureColor(temperature, _settings.TemperatureUnit);

        if (temperatureMin.HasValue)
        {
            // Daily forecast - show high/low in a fixed-width container
            Color tempMinColor = WeatherColorHelper.GetTemperatureColor(temperatureMin.Value, _settings.TemperatureUnit);

            var tempMaxLabel = new Label
            {
                Text = $"{temperature:F0}{unit}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(xPos, 6),
                AutoSize = true,
                ForeColor = tempColor,
                BackColor = Color.Transparent
            };
            rowPanel.Controls.Add(tempMaxLabel);

            var slashLabel = new Label
            {
                Text = "/",
                Font = new Font("Segoe UI", 10),
                Location = new Point(xPos + tempMaxLabel.PreferredWidth, 6),
                AutoSize = true,
                ForeColor = Color.FromArgb(120, 130, 145),
                BackColor = Color.Transparent
            };
            rowPanel.Controls.Add(slashLabel);

            var tempMinLabel = new Label
            {
                Text = $"{temperatureMin:F0}{unit}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(xPos + tempMaxLabel.PreferredWidth + slashLabel.PreferredWidth, 6),
                AutoSize = true,
                ForeColor = tempMinColor,
                BackColor = Color.Transparent
            };
            rowPanel.Controls.Add(tempMinLabel);
            xPos += TEMP_COLUMN_WIDTH;
        }
        else
        {
            // Hourly forecast - single temp
            var tempLabel = new Label
            {
                Text = $"{temperature:F0}{unit}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(xPos, 6),
                AutoSize = true,
                ForeColor = tempColor,
                BackColor = Color.Transparent
            };
            rowPanel.Controls.Add(tempLabel);
            xPos += TEMP_COLUMN_WIDTH;
        }

        // Condition label
        var conditionLabel = new Label
        {
            Text = condition,
            Font = new Font("Segoe UI", 10),
            Location = new Point(xPos, 6),
            AutoSize = true,
            ForeColor = Color.FromArgb(100, 110, 125),
            BackColor = Color.Transparent
        };
        rowPanel.Controls.Add(conditionLabel);
        xPos += conditionLabel.PreferredWidth + 8;

        // Precipitation probability label (only show if >= 10%)
        if (precipProbability >= 10)
        {
            var precipLabel = new Label
            {
                Text = $"{precipProbability}%",
                Font = new Font("Segoe UI", 10),
                Location = new Point(xPos, 6),
                AutoSize = true,
                ForeColor = Color.FromArgb(70, 130, 180), // Steel blue for rain
                BackColor = Color.Transparent
            };
            rowPanel.Controls.Add(precipLabel);
        }

        return rowPanel;
    }

    private void UpdateForecastPanel(WeatherData weather)
    {
        _forecastPanel.Controls.Clear();
        _forecastRowPanels.Clear();
        _forecastTitleLabels.Clear();
        int yPos = 5;
        string unit = _settings.TemperatureUnit.ToLower() == "celsius" ? "°C" : "°F";
        int panelWidth = _forecastPanel.ClientSize.Width;
        int cardWidth = 82;
        int cardSpacing = 5;
        int cardHeight = 85;

        // Hourly Forecast Section - Horizontal layout
        if (weather.HourlyForecasts != null && weather.HourlyForecasts.Count > 0)
        {
            var hourlyTitle = new Label
            {
                Text = "Hourly",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = panelWidth,
                Height = 20,
                Location = new Point(0, yPos),
                ForeColor = _isDarkMode ? ThemeColors.Dark.ForecastText : ThemeColors.Light.ForecastText,
                BackColor = Color.Transparent
            };
            _forecastPanel.Controls.Add(hourlyTitle);
            _forecastTitleLabels.Add(hourlyTitle);
            yPos += 22;

            // Create container panel for hourly cards
            int hourlyCount = Math.Min(6, weather.HourlyForecasts.Count);
            int rowWidth = hourlyCount * cardWidth + (hourlyCount - 1) * cardSpacing;
            var hourlyRowPanel = new DoubleBufferedPanel
            {
                Size = new Size(rowWidth, cardHeight),
                Location = new Point((panelWidth - rowWidth) / 2, yPos),
                BackColor = Color.Transparent
            };

            int xPos = 0;
            foreach (var forecast in weather.HourlyForecasts.Take(6))
            {
                var card = CreateForecastCard(
                    forecast.Condition,
                    forecast.Time.ToString("h tt"),
                    forecast.Temperature,
                    unit,
                    forecast.PrecipitationProbability
                );
                card.Location = new Point(xPos, 0);
                hourlyRowPanel.Controls.Add(card);
                xPos += cardWidth + cardSpacing;
            }

            _forecastPanel.Controls.Add(hourlyRowPanel);
            _forecastRowPanels.Add(hourlyRowPanel);
            yPos += cardHeight + 7;
        }

        // 7-Day Forecast Section - Horizontal layout
        if (weather.DailyForecasts != null && weather.DailyForecasts.Count > 0)
        {
            var dailyTitle = new Label
            {
                Text = "7-Day",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = panelWidth,
                Height = 20,
                Location = new Point(0, yPos),
                ForeColor = _isDarkMode ? ThemeColors.Dark.ForecastText : ThemeColors.Light.ForecastText,
                BackColor = Color.Transparent
            };
            _forecastPanel.Controls.Add(dailyTitle);
            _forecastTitleLabels.Add(dailyTitle);
            yPos += 22;

            // Create container panel for daily cards
            var dailyForecasts = weather.DailyForecasts.Where(f => f.Date.Date > DateTime.Today).Take(7).ToList();
            int dailyCount = dailyForecasts.Count;
            int rowWidth = dailyCount * cardWidth + (dailyCount - 1) * cardSpacing;
            var dailyRowPanel = new DoubleBufferedPanel
            {
                Size = new Size(rowWidth, cardHeight),
                Location = new Point((panelWidth - rowWidth) / 2, yPos),
                BackColor = Color.Transparent
            };

            int xPos = 0;
            foreach (var forecast in dailyForecasts)
            {
                string dayName = forecast.Date.ToString("ddd");
                var card = CreateForecastCard(
                    forecast.Condition,
                    dayName,
                    forecast.TemperatureMax,
                    unit,
                    forecast.PrecipitationProbability,
                    forecast.TemperatureMin
                );
                card.Location = new Point(xPos, 0);
                dailyRowPanel.Controls.Add(card);
                xPos += cardWidth + cardSpacing;
            }

            _forecastPanel.Controls.Add(dailyRowPanel);
            _forecastRowPanels.Add(dailyRowPanel);
        }
    }

    private Panel CreateForecastCard(string condition, string timeOrDay, double temperature, string unit, int precipProbability, double? temperatureMin = null)
    {
        int cardWidth = 82;
        int cardHeight = 85; // Uniform height for all cards

        var card = new DoubleBufferedPanel
        {
            Size = new Size(cardWidth, cardHeight),
            BackColor = _isDarkMode ? ThemeColors.Dark.CardBackground : ThemeColors.Light.CardBackground
        };

        // Colored top indicator
        Color indicatorColor = WeatherColorHelper.GetConditionColor(condition);
        var indicator = new Panel
        {
            Size = new Size(cardWidth, 3),
            Location = new Point(0, 0),
            BackColor = indicatorColor
        };
        card.Controls.Add(indicator);

        // Time/Day label
        var timeLabel = new Label
        {
            Text = timeOrDay,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(cardWidth, 16),
            Location = new Point(0, 4),
            ForeColor = _isDarkMode ? ThemeColors.Dark.ForecastText : ThemeColors.Light.ForecastText,
            BackColor = Color.Transparent
        };
        card.Controls.Add(timeLabel);

        // Weather icon
        var icon = WeatherIconGenerator.GenerateSmallWeatherIcon(condition, 22);
        var iconBox = new PictureBox
        {
            Image = icon,
            Size = new Size(22, 22),
            Location = new Point((cardWidth - 22) / 2, 20),
            SizeMode = PictureBoxSizeMode.StretchImage,
            BackColor = Color.Transparent
        };
        card.Controls.Add(iconBox);

        // Temperature label
        string tempText;
        if (temperatureMin.HasValue)
        {
            tempText = $"{(int)Math.Round(temperature)}°/{(int)Math.Round(temperatureMin.Value)}°";
        }
        else
        {
            tempText = $"{(int)Math.Round(temperature)}{unit}";
        }

        Color tempColor = WeatherColorHelper.GetTemperatureColor(temperature, _settings.TemperatureUnit);
        var tempLabel = new Label
        {
            Text = tempText,
            Font = new Font("Segoe UI", 9, FontStyle.Regular),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(cardWidth, 14),
            Location = new Point(0, 43),
            ForeColor = tempColor,
            BackColor = Color.Transparent
        };
        card.Controls.Add(tempLabel);

        int nextYPos = 55;

        // Condition text for all cards
        string conditionText = char.ToUpper(condition[0]) + condition.Substring(1);
        var conditionLabel = new Label
        {
            Text = conditionText,
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(cardWidth, 14),
            Location = new Point(0, nextYPos),
            ForeColor = _isDarkMode ? ThemeColors.Dark.ConditionText : ThemeColors.Light.ConditionText,
            BackColor = Color.Transparent
        };
        card.Controls.Add(conditionLabel);
        nextYPos += 14;

        // Precipitation probability (only show if >= 10%)
        if (precipProbability >= 10)
        {
            var precipLabel = new Label
            {
                Text = $"{precipProbability}%",
                Font = new Font("Segoe UI", 8),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(cardWidth, 12),
                Location = new Point(0, nextYPos),
                ForeColor = Color.FromArgb(70, 130, 180), // Steel blue stays consistent
                BackColor = Color.Transparent
            };
            card.Controls.Add(precipLabel);
        }

        return card;
    }

    private string GetShortLocationName(string fullName)
    {
        var abbreviations = new Dictionary<string, string>
        {
            { "New York", "NYC" },
            { "Los Angeles", "LA" },
            { "San Francisco", "SF" },
            { "Washington", "DC" }
        };

        foreach (var pair in abbreviations)
        {
            if (fullName.Contains(pair.Key, StringComparison.OrdinalIgnoreCase))
                return pair.Value;
        }

        var firstWord = fullName.Split(' ')[0];
        return firstWord.Length > 10 ? firstWord.Substring(0, 10) : firstWord;
    }

    private string GetWindDirection(int degrees)
    {
        string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        int index = (int)Math.Round(degrees / 45.0) % 8;
        return directions[index];
    }

    private void OnResize(object? sender, EventArgs e)
    {
        // Don't hide when minimizing in taskbar mode
        if (WindowState == FormWindowState.Minimized && !_isTrayMode)
        {
            // Stay in taskbar, just minimized
            return;
        }

        // Keep buttons centered when form is resized
        CenterButtons();
    }

    private void CenterButtons()
    {
        // Button widths: 130 + 90 + 75 + 110 = 405, gaps: 12 × 3 = 36, total = 441
        const int buttonSpacing = 12;
        int totalWidth = _refreshButton.Width + _settingsButton.Width + _aboutButton.Width + _moveToTrayButton.Width + (buttonSpacing * 3);
        int startX = (ClientSize.Width - totalWidth) / 2;
        int buttonY = ClientSize.Height - 55; // 55px from bottom for taller buttons

        int xPos = startX;
        _refreshButton.Location = new Point(xPos, buttonY);
        xPos += _refreshButton.Width + buttonSpacing;
        _settingsButton.Location = new Point(xPos, buttonY);
        xPos += _settingsButton.Width + buttonSpacing;
        _aboutButton.Location = new Point(xPos, buttonY);
        xPos += _aboutButton.Width + buttonSpacing;
        _moveToTrayButton.Location = new Point(xPos, buttonY);
    }

    private void OnAboutClick(object? sender, EventArgs e)
    {
        using var aboutForm = new AboutForm();
        aboutForm.ShowDialog(this);
    }

    private void OnTrayIconClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            SwitchToTaskbarMode();
        }
    }

    public void SwitchToTrayMode()
    {
        _isTrayMode = true;
        Hide();
        ShowInTaskbar = false;

        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = true;
            if (_currentWeather != null)
            {
                int temp = (int)Math.Round(_currentWeather.Temperature);
                _notifyIcon.Icon = WeatherIconGenerator.GenerateTrayIcon(temp, _settings.TemperatureUnit);
                string unit = _settings.TemperatureUnit.ToLower() == "celsius" ? "C" : "F";
                _notifyIcon.Text = $"{temp}°{unit} - {_currentWeather.Condition}";
            }
        }
    }

    public void SwitchToTaskbarMode()
    {
        _isTrayMode = false;

        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = false;
        }

        ShowInTaskbar = true;
        Show();
        WindowState = FormWindowState.Normal;
        Activate();
    }

    private void OnSettingsClick(object? sender, EventArgs e)
    {
        // Make sure window is visible for settings dialog
        bool wasInTrayMode = _isTrayMode;
        if (_isTrayMode)
        {
            SwitchToTaskbarMode();
        }

        using var settingsForm = new SettingsForm(_settings, _weatherService);

        if (settingsForm.ShowDialog() == DialogResult.OK)
        {
            _settings = settingsForm.Settings;
            _settingsManager.SaveSettings(_settings);

            _locationLabel.Text = _settings.LocationName;
            SetupRefreshTimer();
            ApplyTheme();
            _ = RefreshWeatherAsync();
        }

        if (wasInTrayMode)
        {
            SwitchToTrayMode();
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Allow the app to close normally - use minimize button to minimize
        base.OnFormClosing(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshTimer?.Dispose();
            _notifyIcon?.Dispose();
            SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        }
        base.Dispose(disposing);
    }

    private bool GetSystemDarkMode()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key != null)
            {
                var value = key.GetValue("AppsUseLightTheme");
                if (value != null)
                {
                    return (int)value == 0; // 0 = dark mode, 1 = light mode
                }
            }
        }
        catch
        {
            // If registry read fails, default to light mode
        }
        return false;
    }

    private bool ShouldUseDarkMode()
    {
        return _settings.Theme switch
        {
            ThemeMode.Light => false,
            ThemeMode.Dark => true,
            _ => GetSystemDarkMode() // ThemeMode.System
        };
    }

    private void ApplyTheme()
    {
        _isDarkMode = ShouldUseDarkMode();

        // Update form background
        BackColor = _isDarkMode ? ThemeColors.Dark.FormBackground : ThemeColors.Light.FormBackground;

        // Update main labels
        _temperatureLabel.ForeColor = _isDarkMode ? ThemeColors.Dark.PrimaryText : ThemeColors.Light.PrimaryText;
        _highLowLabel.ForeColor = _isDarkMode ? ThemeColors.Dark.HighLowText : ThemeColors.Light.HighLowText;
        _detailsLabel.ForeColor = _isDarkMode ? ThemeColors.Dark.SecondaryText : ThemeColors.Light.SecondaryText;
        _locationLabel.ForeColor = _isDarkMode ? ThemeColors.Dark.LocationText : ThemeColors.Light.LocationText;
        _lastUpdatedLabel.ForeColor = _isDarkMode ? ThemeColors.Dark.MutedText : ThemeColors.Light.MutedText;

        // Update forecast panel
        _forecastPanel.BackColor = _isDarkMode ? ThemeColors.Dark.PanelBackground : ThemeColors.Light.PanelBackground;

        // Update buttons
        _refreshButton.UpdateTheme(_isDarkMode);
        _settingsButton.UpdateTheme(_isDarkMode);
        _aboutButton.UpdateTheme(_isDarkMode);
        _moveToTrayButton.UpdateTheme(_isDarkMode);

        // Refresh the display to update forecast cards and other themed elements
        if (_currentWeather != null)
        {
            UpdateForecastPanel(_currentWeather);
        }

        // Force repaint
        Invalidate();
    }

    // Modern button using Control base class (no button chrome artifacts)
    private class ModernButton : Control
    {
        private bool _isHovered = false;
        private bool _isPressed = false;
        private bool _isDarkMode = false;
        private Color _normalColor;
        private Color _hoverColor;
        private Color _pressedColor;
        private readonly bool _isPrimary;
        private const int CornerRadius = 8;

        public ModernButton(bool isPrimary = false)
        {
            _isPrimary = isPrimary;

            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            // Initialize with light theme colors
            UpdateTheme(isDarkMode: false);

            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;
        }

        public void UpdateTheme(bool isDarkMode)
        {
            _isDarkMode = isDarkMode;
            if (_isPrimary)
            {
                _normalColor = isDarkMode ? ThemeColors.Dark.ButtonPrimary : ThemeColors.Light.ButtonPrimary;
                _hoverColor = isDarkMode ? ThemeColors.Dark.ButtonPrimaryHover : ThemeColors.Light.ButtonPrimaryHover;
                _pressedColor = isDarkMode ? ThemeColors.Dark.ButtonPrimaryPressed : ThemeColors.Light.ButtonPrimaryPressed;
                ForeColor = isDarkMode ? ThemeColors.Dark.ButtonPrimaryText : ThemeColors.Light.ButtonPrimaryText;
            }
            else
            {
                _normalColor = isDarkMode ? ThemeColors.Dark.ButtonSecondary : ThemeColors.Light.ButtonSecondary;
                _hoverColor = isDarkMode ? ThemeColors.Dark.ButtonSecondaryHover : ThemeColors.Light.ButtonSecondaryHover;
                _pressedColor = isDarkMode ? ThemeColors.Dark.ButtonSecondaryPressed : ThemeColors.Light.ButtonSecondaryPressed;
                ForeColor = isDarkMode ? ThemeColors.Dark.ButtonSecondaryText : ThemeColors.Light.ButtonSecondaryText;
            }

            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color bgColor = _isPressed ? _pressedColor :
                           _isHovered ? _hoverColor :
                           _normalColor;

            // Inset bounds slightly to make room for the border on secondary buttons
            Rectangle fillBounds = _isPrimary ? ClientRectangle :
                new Rectangle(ClientRectangle.X + 1, ClientRectangle.Y + 1, ClientRectangle.Width - 2, ClientRectangle.Height - 2);

            using (var path = GetRoundedRectangle(fillBounds, CornerRadius))
            {
                using (var brush = new SolidBrush(bgColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                // Add subtle border to secondary buttons for more definition
                if (!_isPrimary)
                {
                    Color borderColor = _isDarkMode
                        ? Color.FromArgb(80, 85, 95)
                        : Color.FromArgb(200, 205, 215);
                    using (var pen = new Pen(borderColor, 1))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            }

            // Draw text
            TextRenderer.DrawText(
                e.Graphics,
                Text,
                Font,
                ClientRectangle,
                ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private GraphicsPath GetRoundedRectangle(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }
    }

    // Double-buffered panel to prevent scrolling artifacts
    private class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
            UpdateStyles();
        }
    }
}
