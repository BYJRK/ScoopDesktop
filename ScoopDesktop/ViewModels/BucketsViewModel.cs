using ScoopDesktop.Helpers;
using ScoopDesktop.Messages;
using ScoopDesktop.Models;
using ScoopDesktop.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Data;

namespace ScoopDesktop.ViewModels;

public partial class BucketsViewModel : PageViewModelBase
{
    #region Properties

    [ObservableProperty]
    ObservableCollection<BucketInfo> buckets = new();

    [ObservableProperty]
    ListCollectionView bucketAppsView;

    [ObservableProperty]
    BucketInfo? selectedBucket;

    [ObservableProperty]
    string userInputQueryText;

    [ObservableProperty]
    bool isBusy;

    partial void OnUserInputQueryTextChanged(string value)
    {
        Suggestions = bucketApps
            .Select(app => app.AppName)
            .Where(app => app.Contains(UserInputQueryText))
            .ToList();
    }

    [ObservableProperty]
    List<string> suggestions;

    partial void OnSelectedBucketChanged(BucketInfo? value)
    {
        if (value is null)
            return;

        bucketApps = new(
            SelectedBucket!.AppList
            .Select(app => new AppInfo(Path.GetFileNameWithoutExtension(app), "", value.BucketName)));

        Task.Run(() =>
        {
            Parallel.For(0, bucketApps.Count, (i) =>
            {
                var jsonFile = SelectedBucket.AppList[i];
                var info = ScoopHelper.GetExtraInfoFromJson(jsonFile);
                bucketApps[i].Version = info.Version;
                bucketApps[i].Desc = info.Desc;
                bucketApps[i].HomePage = info.HomePage;
                bucketApps[i].IsInstalled = installedApps.Contains(bucketApps[i].AppName);
            });
        });

        InitBucketAppsView();
    }

    #endregion

    List<AppInfo> bucketApps;

    HashSet<string> installedApps;

    public BucketsViewModel()
    {

    }

    #region Relay Commands

    [RelayCommand]
    async Task Loaded()
    {
        var bucketDirs = ScoopHelper.GetBucketDirs();
        Buckets.Clear();
        foreach (var bucketDir in bucketDirs)
        {
            var name = Path.GetFileNameWithoutExtension(bucketDir);
            var apps = ScoopHelper.GetBucketAppList(bucketDir).ToArray();
            Buckets.Add(new BucketInfo(name, bucketDir, apps));
        }

        installedApps = WeakReferenceMessenger.Default.Send<RequestInstalledAppsMessage>();

        SelectedBucket = null;
        SelectedBucket = Buckets.First(b => b.BucketName == "main");
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
    private void OpenHomePage(AppInfo app)
    {
        Process.Start("explorer", app.HomePage);
    }

    void InitBucketAppsView()
    {
        BucketAppsView = (ListCollectionView)CollectionViewSource.GetDefaultView(bucketApps);
        BucketAppsView.Filter = app =>
        {
            if (string.IsNullOrEmpty(UserInputQueryText))
                return true;
            return ((AppInfo)app).AppName.Contains(UserInputQueryText);
        };
    }

    [RelayCommand]
    void QueryAppName()
    {
        BucketAppsView.Refresh();
    }

    [RelayCommand]
    void Filter(FilterEventArgs e)
    {
        if (string.IsNullOrEmpty(UserInputQueryText))
        {
            e.Accepted = true;
            return;
        }
        var app = e.Item as AppInfo;
        if (app != null)
        {
            e.Accepted = app.AppName.Contains(UserInputQueryText);
        }
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
    private async Task InstallApp(AppInfo app)
    {
        await DialogHelper.Progressive(
            $"scoop install {app.AppName}",
            $"Installing {app.AppName}",
            rule: s =>
            {
                return s.StartsWith("Installing")
                || s.StartsWith("Starting")
                || s.StartsWith("Checking")
                || s.StartsWith("Extracting")
                || s.StartsWith("Linking")
                || s.StartsWith("Creating")
                || s.EndsWith("completed.")
                || s.EndsWith("successfully!");
            });

        await Loaded();
    }

    #endregion
}
