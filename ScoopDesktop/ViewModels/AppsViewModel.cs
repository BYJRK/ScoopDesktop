using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScoopDesktop.Helpers;
using ScoopDesktop.Models;
using ScoopDesktop.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ScoopDesktop.ViewModels;

public partial class AppsViewModel : PageViewModelBase
{
    #region Observable Properties

    [ObservableProperty]
    private ObservableCollection<AppInfo> appList = new();

    [ObservableProperty]
    private bool isProgressRingActive = false;

    #endregion

    #region Commands

    [RelayCommand]
    private void Loaded()
    {
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
        IsProgressRingActive = true;

        var message = await PwshHelper.RunPowerShellCommandAsync($"scoop info {app.AppName}");

        IsProgressRingActive = false;

        await PopupHelper.ShowInfo(message, app.AppName, isMono: true);
    }

    #endregion

    public AppsViewModel()
    {
        IsCommandBarVisible = true;
    }
}
