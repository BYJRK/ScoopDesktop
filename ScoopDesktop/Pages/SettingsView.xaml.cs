using ModernWpf;
using System.Windows;
using System.Windows.Controls;

namespace ScoopDesktop.Pages
{
    /// <summary>
    /// SettingsView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsView : Page
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var btn = sender as ModernWpf.Controls.ToggleSwitch;

            ThemeManager.Current.ApplicationTheme = btn!.IsOn ? ApplicationTheme.Dark : ApplicationTheme.Light;
        }
    }
}
