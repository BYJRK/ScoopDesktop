using ScoopDesktop.Utils;

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

    [ObservableProperty]
    bool isInstalled;

    [ObservableProperty]
    string? desc;

    public AppInfo(string appName, string version, string bucket)
    {
        this.appName = appName;
        this.version = version;
        this.bucket = bucket;
    }

    public static AppInfo LoadInfoFromPath(string path)
    {
        var appName = Path.GetFileName(path) ?? throw new ArgumentException(path);
        var version = ScoopHelper.GetAppVersion(path) ?? throw new ArgumentException(path);
        var bucket = ScoopHelper.GetAppBucket(path) ?? throw new ArgumentException(path);
        return new(appName, version, bucket)
        {
            HomePage = ScoopHelper.GetAppHomePage(path),
            Folder = path
        };
    }
}
