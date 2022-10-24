using ModernWpf;
using ScoopDesktop.Utils;

namespace ScoopDesktop.ViewModels;

public partial class SettingsViewModel : PageViewModelBase
{
    [ObservableProperty]
    bool darkMode;

    [ObservableProperty]
    string proxy;

    [ObservableProperty]
    bool isBusy;

    partial void OnDarkModeChanged(bool value)
    {
        Properties.Settings.Default.DarkMode = value;
        ThemeManager.Current.ApplicationTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
    }

    public SettingsViewModel()
    {
        IsCommandBarVisible = false;

        DarkMode = Properties.Settings.Default.DarkMode;
    }

    [RelayCommand]
    async Task Loaded()
    {
        IsBusy = true;

        Proxy = await PwshHelper.RunCommandAsync("scoop config proxy");

        IsBusy = false;
    }

    [RelayCommand]
    async Task SetProxy()
    {
        IsBusy = true;

        await PwshHelper.RunCommandAsync($"scoop config proxy {Proxy}");

        IsBusy = false;
    }
}
