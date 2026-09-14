using WeatherTaskbarApp.UI;

namespace WeatherTaskbarApp;

static class Program
{
    [STAThread]
    static void Main()
    {
        // Enable visual styles for modern Windows UI
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Ensure only one instance is running
        bool createdNew;
        using var mutex = new Mutex(true, "WeatherTaskbarApp_SingleInstance", out createdNew);

        if (!createdNew)
        {
            MessageBox.Show("Weather Taskbar App is already running.", "Already Running",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Run the application with the new weather form
        Application.Run(new WeatherForm());
    }
}
