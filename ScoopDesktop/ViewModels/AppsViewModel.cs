using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScoopDesktop.Helpers;
using ScoopDesktop.Models;
using ScoopDesktop.Utils;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScoopDesktop.ViewModels;

public partial class AppsViewModel : PageViewModelBase
{
    #region Observable Properties

    [ObservableProperty]
    private ObservableCollection<AppInfo> appList = new();

    [ObservableProperty]
    private bool isBusy = false;

    #endregion

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

        await PopupHelper.ShowInfo(message, app.AppName, isMono: true);
    }

    [RelayCommand]
    private async Task Update()
    {
        await PopupHelper.ShowScoopStatus("Scoop Update", true);
    }

    [RelayCommand]
    private async Task GetStatus()
    {
        IsBusy = true;

        var output = await PwshHelper.RunCommandAsync("scoop status");

        var apps = output
            .Split(Environment.NewLine)
            .SkipWhile(line => !line.StartsWith("----"))
            .Skip(1)
            .Select(line => Regex.Match(line, @"^(\w+)\s").Groups[1].Value)
            .ToHashSet();

        foreach (var app in AppList)
            app.CanUpdate = apps.Contains(app.AppName);

        IsBusy = false;
    }

    #endregion

    public AppsViewModel()
    {
        IsCommandBarVisible = true;
    }
}
