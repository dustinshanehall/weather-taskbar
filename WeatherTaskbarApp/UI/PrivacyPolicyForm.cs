namespace WeatherTaskbarApp.UI;

public class PrivacyPolicyForm : Form
{
    public PrivacyPolicyForm()
    {
        Text = "Privacy Policy";
        Size = new Size(550, 600);
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(450, 400);
        MaximizeBox = true;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;

        // Title
        var titleLabel = new Label
        {
            Text = "Privacy Policy",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(45, 85, 145),
            AutoSize = true,
            Location = new Point(20, 15)
        };

        var lastUpdatedLabel = new Label
        {
            Text = "Last updated: September 14, 2026",
            Font = new Font("Segoe UI", 9, FontStyle.Italic),
            ForeColor = Color.FromArgb(120, 120, 120),
            AutoSize = true,
            Location = new Point(20, 45)
        };

        // Scrollable content panel
        var contentPanel = new Panel
        {
            Location = new Point(20, 75),
            Size = new Size(490, 430),
            AutoScroll = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        var contentLabel = new Label
        {
            Text = GetPrivacyPolicyText(),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(50, 50, 50),
            AutoSize = true,
            MaximumSize = new Size(460, 0),
            Location = new Point(0, 0)
        };
        contentPanel.Controls.Add(contentLabel);

        // Close button
        var closeButton = new Button
        {
            Text = "Close",
            Size = new Size(80, 30),
            Location = new Point(430, 520),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            FlatStyle = FlatStyle.Flat
        };
        closeButton.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
        closeButton.Click += (s, e) => Close();

        Controls.AddRange(new Control[]
        {
            titleLabel,
            lastUpdatedLabel,
            contentPanel,
            closeButton
        });
    }

    private string GetPrivacyPolicyText()
    {
        return @"INTRODUCTION

Weather Taskbar is a free, open-source Windows application. This policy explains what
the app does with your information. The short version: everything stays on your PC, and
there are no servers behind this app.


INFORMATION WE COLLECT

None. The app has no accounts, no analytics, no telemetry and no crash reporting, and it
never transmits anything to the developer.

The app does not read your device's location. The location used for the forecast is the
city name or ZIP code you type into Settings yourself.


WHAT IS STORED ON YOUR PC

Your preferences are saved in a plain JSON file under your Windows user profile
(%APPDATA%\WeatherTaskbarApp\settings.json):
  - The location name and its coordinates
  - Theme preference (light/dark/system)
  - Temperature unit
  - Refresh interval
  - Taskbar or system-tray display mode
  - Auto-start preference

This file never leaves your machine. Deleting the folder, or uninstalling the app,
removes it.


THIRD-PARTY SERVICES

The app calls one external service: Open-Meteo (open-meteo.com), a free, open-source
weather API that requires no API key or account.

Two kinds of request are sent:
  - Geocoding: the place name you typed, to convert it into coordinates
  - Forecast: those coordinates, to fetch current conditions and the forecast

No name, email, account, device identifier or advertising ID is sent, because the app
holds none. As with any internet request, Open-Meteo will see the originating IP address
and can apply its own policy to it, so their privacy terms apply to that leg of the
request.


CHILDREN'S PRIVACY

The app collects no personal information from anyone, including children.


YOUR RIGHTS

Because no personal data is collected or held, there is nothing to request, correct or
delete on our side. All app data is local and under your control: edit it in Settings, or
delete the settings folder above.


CHANGES TO THIS POLICY

Changes are published with each release; the ""Last updated"" date at the top of this
policy reflects the most recent one.


CONTACT

Questions, bugs and policy issues: open an issue at
https://github.com/dustinshanehall/weather-taskbar";
    }
}
