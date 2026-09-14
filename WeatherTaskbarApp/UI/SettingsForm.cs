using WeatherTaskbarApp.Models;
using WeatherTaskbarApp.Services;

namespace WeatherTaskbarApp.UI;

public class SettingsForm : Form
{
    private readonly TextBox _locationTextBox;
    private readonly ComboBox _unitComboBox;
    private readonly ComboBox _refreshComboBox;
    private readonly ComboBox _themeComboBox;
    private readonly CheckBox _autoStartCheckBox;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;
    private readonly WeatherService _weatherService;

    public AppSettings Settings { get; private set; }

    public SettingsForm(AppSettings currentSettings, WeatherService weatherService)
    {
        Settings = currentSettings;
        _weatherService = weatherService;

        // Form settings
        Text = "Weather App Settings";
        Size = new Size(400, 340);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        // Location
        var locationLabel = new Label
        {
            Text = "Location (City or ZIP):",
            Location = new Point(20, 20),
            AutoSize = true
        };

        _locationTextBox = new TextBox
        {
            Location = new Point(20, 45),
            Size = new Size(340, 25),
            Text = currentSettings.LocationName
        };

        // Temperature Unit
        var unitLabel = new Label
        {
            Text = "Temperature Unit:",
            Location = new Point(20, 80),
            AutoSize = true
        };

        _unitComboBox = new ComboBox
        {
            Location = new Point(20, 105),
            Size = new Size(160, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _unitComboBox.Items.AddRange(new[] { "Fahrenheit", "Celsius" });
        _unitComboBox.SelectedItem = currentSettings.TemperatureUnit.ToLower() == "celsius" ? "Celsius" : "Fahrenheit";

        // Theme
        var themeLabel = new Label
        {
            Text = "Theme:",
            Location = new Point(200, 80),
            AutoSize = true
        };

        _themeComboBox = new ComboBox
        {
            Location = new Point(200, 105),
            Size = new Size(160, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _themeComboBox.Items.AddRange(new[] { "Light", "Dark", "Match System" });
        _themeComboBox.SelectedIndex = currentSettings.Theme switch
        {
            ThemeMode.Light => 0,
            ThemeMode.Dark => 1,
            _ => 2
        };

        // Refresh Interval
        var refreshLabel = new Label
        {
            Text = "Refresh Interval:",
            Location = new Point(20, 140),
            AutoSize = true
        };

        _refreshComboBox = new ComboBox
        {
            Location = new Point(20, 165),
            Size = new Size(340, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _refreshComboBox.Items.AddRange(new[] { "5 minutes", "10 minutes", "15 minutes", "30 minutes", "60 minutes" });
        _refreshComboBox.SelectedItem = $"{currentSettings.RefreshIntervalMinutes} minutes";

        // Auto-start
        _autoStartCheckBox = new CheckBox
        {
            Text = "Start automatically with Windows",
            Location = new Point(20, 205),
            AutoSize = true,
            Checked = currentSettings.AutoStart
        };

        // Buttons
        _saveButton = new Button
        {
            Text = "Save",
            Location = new Point(200, 260),
            Size = new Size(80, 30)
        };
        _saveButton.Click += OnSaveClick;

        _cancelButton = new Button
        {
            Text = "Cancel",
            Location = new Point(290, 260),
            Size = new Size(80, 30)
        };
        _cancelButton.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        // Add controls
        Controls.AddRange(new Control[]
        {
            locationLabel, _locationTextBox,
            unitLabel, _unitComboBox,
            themeLabel, _themeComboBox,
            refreshLabel, _refreshComboBox,
            _autoStartCheckBox,
            _saveButton, _cancelButton
        });
    }

    private async void OnSaveClick(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_locationTextBox.Text))
        {
            MessageBox.Show("Please enter a location.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Disable save button during geocoding
        _saveButton.Enabled = false;
        _saveButton.Text = "Saving...";

        try
        {
            // Geocode the location
            var result = await _weatherService.GeocodeLocationAsync(_locationTextBox.Text);

            if (result == null)
            {
                MessageBox.Show("Could not find the specified location. Please try a different city name or ZIP code.",
                    "Location Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _saveButton.Enabled = true;
                _saveButton.Text = "Save";
                return;
            }

            // Update settings
            Settings.LocationName = result.Value.name;
            Settings.Latitude = result.Value.latitude;
            Settings.Longitude = result.Value.longitude;
            Settings.TemperatureUnit = _unitComboBox.SelectedItem?.ToString()?.ToLower() ?? "fahrenheit";

            string refreshText = _refreshComboBox.SelectedItem?.ToString() ?? "15 minutes";
            Settings.RefreshIntervalMinutes = int.Parse(refreshText.Split(' ')[0]);

            Settings.Theme = _themeComboBox.SelectedIndex switch
            {
                0 => ThemeMode.Light,
                1 => ThemeMode.Dark,
                _ => ThemeMode.System
            };

            Settings.AutoStart = _autoStartCheckBox.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            _saveButton.Enabled = true;
            _saveButton.Text = "Save";
        }
    }
}
