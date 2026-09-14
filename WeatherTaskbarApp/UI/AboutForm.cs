namespace WeatherTaskbarApp.UI;

public class AboutForm : Form
{
    public AboutForm()
    {
        Text = "About Weather Taskbar";
        Size = new Size(450, 380);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;

        // App icon/logo area
        var logoPanel = new Panel
        {
            Location = new Point(0, 0),
            Size = new Size(450, 80),
            BackColor = Color.FromArgb(45, 85, 145)
        };

        var appNameLabel = new Label
        {
            Text = "Weather Taskbar",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(20, 15)
        };
        logoPanel.Controls.Add(appNameLabel);

        var versionLabel = new Label
        {
            Text = "Version 2.3.0",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(200, 220, 255),
            AutoSize = true,
            Location = new Point(22, 50)
        };
        logoPanel.Controls.Add(versionLabel);

        // Project home (clickable link to the source repository)
        var projectLink = new LinkLabel
        {
            Text = "github.com/dustinshanehall/weather-taskbar",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            LinkColor = Color.FromArgb(45, 85, 145),
            ActiveLinkColor = Color.FromArgb(30, 60, 110),
            VisitedLinkColor = Color.FromArgb(45, 85, 145),
            AutoSize = true,
            Location = new Point(20, 100)
        };
        projectLink.Click += (s, e) =>
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/dustinshanehall/weather-taskbar",
                    UseShellExecute = true
                });
            }
            catch { }
        };

        var copyrightLabel = new Label
        {
            Text = "Open source \u2014 Apache License 2.0",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(100, 100, 100),
            AutoSize = true,
            Location = new Point(20, 125)
        };

        // Attribution section
        var attributionTitle = new Label
        {
            Text = "Data Providers",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(50, 50, 50),
            AutoSize = true,
            Location = new Point(20, 160)
        };

        var weatherAttribution = new Label
        {
            Text = "\u2022 Weather data powered by Open-Meteo",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(80, 80, 80),
            AutoSize = true,
            Location = new Point(20, 185)
        };

        // Privacy Policy link
        var privacyLink = new LinkLabel
        {
            Text = "Privacy Policy",
            Font = new Font("Segoe UI", 10),
            AutoSize = true,
            Location = new Point(20, 220),
            LinkColor = Color.FromArgb(45, 85, 145)
        };
        privacyLink.Click += (s, e) =>
        {
            using var privacyForm = new PrivacyPolicyForm();
            privacyForm.ShowDialog(this);
        };

        // Contact info
        var contactLabel = new Label
        {
            Text = "Questions or bugs: open an issue on GitHub",
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(100, 100, 100),
            AutoSize = true,
            Location = new Point(20, 255)
        };

        // Close button
        var closeButton = new Button
        {
            Text = "Close",
            Size = new Size(80, 30),
            Location = new Point(345, 300),
            FlatStyle = FlatStyle.Flat
        };
        closeButton.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
        closeButton.Click += (s, e) => Close();

        Controls.AddRange(new Control[]
        {
            logoPanel,
            projectLink,
            copyrightLabel,
            attributionTitle,
            weatherAttribution,
            privacyLink,
            contactLabel,
            closeButton
        });
    }
}
