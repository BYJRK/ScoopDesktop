using ScoopDesktop.Helpers;
using ScoopDesktop.Messages;
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
            try
            {
                AppList.Add(AppInfo.LoadInfoFromPath(app));
            }
            catch { }
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

        await DialogHelper.Info(message, app.AppName, monospace: true);
    }

    [RelayCommand]
    private async Task Update()
    {
        await DialogHelper.Progressive(
            "scoop update",
            "Scoop Update",
            rule: s => s.StartsWith("Updating") || s.EndsWith("successfully!")
        );
    }

    [RelayCommand]
    private async Task UpdateApp(AppInfo app)
    {
        await DialogHelper.Progressive(
            $"scoop update {app.AppName}",
            $"Updating {app.AppName}",
            rule: s =>
            {
                return s.Contains("->")
                    || s.StartsWith("Updating")
                    || s.StartsWith("Downloading")
                    || s.StartsWith("Uninstalling")
                    || s.StartsWith("Installing")
                    || s.StartsWith("Linking")
                    || s.StartsWith("Running")
                    || s.EndsWith("completed.")
                    || s.EndsWith("ok.")
                    || s.EndsWith("done.")
                    || s.EndsWith("successfully!");
            }
        );
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
        if (
            await DialogHelper.YesNo(
                $"Are you sure you want to uninstall {app.AppName}?",
                "Scoop Uninstall"
            ) == ModernWpf.Controls.ContentDialogResult.Primary
        )
        {
            await DialogHelper.Progressive(
                $"scoop uninstall {app.AppName}",
                $"Uninstalling {app.AppName}",
                rule: s =>
                    s.StartsWith("Uninstalling")
                    || s.StartsWith("Removing")
                    || s.StartsWith("Unlinking")
                    || s.EndsWith("uninstalled.")
            );

            Loaded();
        }
    }

    [RelayCommand]
    private async Task Cleanup()
    {
        var res = await DialogHelper.YesNo(
            "Are you sure you want to remove all download caches and outdated apps?"
        );
        if (res != ModernWpf.Controls.ContentDialogResult.Primary)
            return;

        await DialogHelper.Progressive(
            "scoop cache rm * && scoop cleanup *",
            "Scoop Cache & Scoop Cleanup",
            rule: s => s.StartsWith("Removing") || s.StartsWith("Deleted:") || s.EndsWith("now!")
        );
    }

    #endregion

    public AppsViewModel()
    {
        IsCommandBarVisible = true;

        updateApps = new HashSet<string>();

        WeakReferenceMessenger.Default.Register<RequestInstalledAppsMessage>(
            this,
            (_, m) =>
            {
                m.Reply(AppList.Select(app => app.AppName).ToHashSet());
            }
        );
    }
}
