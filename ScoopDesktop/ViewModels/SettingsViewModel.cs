using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModernWpf;
using ScoopDesktop.Utils;
using System.Threading.Tasks;

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

        Proxy = await PwshHelper.RunPowerShellCommandAsync("scoop config proxy");

        IsBusy = false;
    }

    [RelayCommand]
    async Task SetProxy()
    {
        IsBusy = true;

        await PwshHelper.RunPowerShellCommandAsync($"scoop config proxy {Proxy}");

        IsBusy = false;
    }
}
