using System.Windows;
using ModernWpf;
using ScoopDesktop.Properties;

namespace ScoopDesktop;

public partial class App : Application
{
    public App()
    {
        if (Settings.Default.DarkMode)
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        Settings.Default.Save();
    }
}
