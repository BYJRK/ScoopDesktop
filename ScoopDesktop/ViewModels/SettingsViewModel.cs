using ModernWpf;
using ScoopDesktop.Utils;
using System.Text.RegularExpressions;

namespace ScoopDesktop.ViewModels;

public partial class SettingsViewModel : PageViewModelBase
{
    [ObservableProperty]
    bool darkMode;

    [ObservableProperty]
    bool useAria2;

    [ObservableProperty]
    string proxy;

    [ObservableProperty]
    string lastUpdate;

    [ObservableProperty]
    bool isBusy;

    partial void OnDarkModeChanged(bool value)
    {
        Properties.Settings.Default.DarkMode = value;
        ThemeManager.Current.ApplicationTheme = value ? ApplicationTheme.Dark : ApplicationTheme.Light;
    }

    partial void OnUseAria2Changed(bool value)
    {
        Task.Run(async () => await PwshHelper.RunCommandAsync($"scoop config aria2-enabled {value}"));
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

        await Task.WhenAll(
            Task.Run(async () => Proxy = await PwshHelper.RunCommandAsync("scoop config proxy")),
            Task.Run(async () => LastUpdate = Regex.Match(await PwshHelper.RunCommandAsync("scoop config"), @"(?m)last_update\s+:\s+(.*)$").Groups[1].Value.TrimEnd()),
            Task.Run(async () => UseAria2 = (await PwshHelper.RunCommandAsync("scoop config aria2-enabled")) == "True")
            );
        ;

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
