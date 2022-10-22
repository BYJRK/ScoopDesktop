using ScoopDesktop.Utils;
using System;
using System.IO;

namespace ScoopDesktop.Models
{
    public partial class AppInfo
    {
        private AppInfo() { }

        public string AppName { get; init; }
        public string Version { get; init; }
        public string Bucket { get; init; }
        public string? HomePage { get; init; }
        public string? Folder { get; init; }

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
}
