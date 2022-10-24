using ScoopDesktop.Helpers;
using ScoopDesktop.Models;
using ScoopDesktop.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ScoopDesktop.ViewModels;

public partial class AppsViewModel : PageViewModelBase
{
    #region Observable Properties

    [ObservableProperty]
    private ObservableCollection<AppInfo> appList = new();

    [ObservableProperty]
    private bool isBusy = false;

    #endregion

    private HashSet<string> updateApps;

    #region Commands

    [RelayCommand]
    private void Loaded()
    {
        AppList.Clear();
        foreach (var app in ScoopHelper.GetAppDirs())
        {
            var appName = Path.GetFileName(app);
            if (appName == "scoop")
                continue;
            AppList.Add(AppInfo.LoadInfoFromPath(app));
        }

        foreach (var app in AppList)
            app.CanUpdate = updateApps.Contains(app.AppName);
    }

    [RelayCommand]
    private void OpenHomePage(AppInfo app)
    {
        Process.Start("explorer", app.HomePage);
    }

    [RelayCommand]
    private void OpenFolder(AppInfo app)
    {
        Process.Start("explorer", app.Folder);
    }

    [RelayCommand]
    private async Task ShowInfo(AppInfo app)
    {
        IsBusy = true;

        var message = await PwshHelper.RunCommandAsync($"scoop info {app.AppName}");

        IsBusy = false;

        await PopupHelper.Info(message, app.AppName, isMono: true);
    }

    [RelayCommand]
    private async Task Update()
    {
        await PopupHelper.ScoopStatus("Scoop Update", true);
    }

    [RelayCommand]
    private async Task GetStatus()
    {
        IsBusy = true;

        var output = await PwshHelper.RunCommandAsync("scoop status");

        updateApps = output
            .Split(Environment.NewLine)
            .SkipWhile(line => !line.StartsWith("----"))
            .Skip(1)
            .Select(line => Regex.Match(line, @"^(\w+)\s").Groups[1].Value)
            .ToHashSet();

        foreach (var app in AppList)
            app.CanUpdate = updateApps.Contains(app.AppName);

        IsBusy = false;
    }

    [RelayCommand]
    private async Task Uninstall(AppInfo app)
    {
        if (await PopupHelper.YesNo($"Are you sure you want to uninstall {app.AppName}?", "Scoop Uninstall") == ModernWpf.Controls.ContentDialogResult.Primary)
        {
            IsBusy = true;

            await PwshHelper.RunCommandAsync($"scoop uninstall {app.AppName}");

            await PopupHelper.Info($"{app.AppName} is uninstalled successfully.");

            Loaded();

            IsBusy = false;
        }
    }

    #endregion

    public AppsViewModel()
    {
        IsCommandBarVisible = true;

        updateApps = new HashSet<string>();
    }
}
