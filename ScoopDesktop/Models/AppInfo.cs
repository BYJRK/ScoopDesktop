using CommunityToolkit.Mvvm.ComponentModel;
using ScoopDesktop.Utils;
using System;
using System.IO;

namespace ScoopDesktop.Models;

public partial class AppInfo : ObservableObject
{
    [ObservableProperty]
    string appName;

    [ObservableProperty]
    string version;

    [ObservableProperty]
    string bucket;

    [ObservableProperty]
    string? homePage;

    [ObservableProperty]
    string? folder;

    [ObservableProperty]
    bool canUpdate;

    public static AppInfo LoadInfoFromPath(string path)
    {
        return new()
        {
            AppName = Path.GetFileName(path) ?? throw new ArgumentException(path),
            Version = ScoopHelper.GetAppVersion(path) ?? throw new ArgumentException(path),
            Bucket = ScoopHelper.GetAppBucket(path) ?? throw new ArgumentException(path),
            HomePage = ScoopHelper.GetAppHomePage(path),
            Folder = path
        };
    }
}
