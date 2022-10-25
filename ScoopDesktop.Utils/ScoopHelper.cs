using Newtonsoft.Json.Linq;

namespace ScoopDesktop.Utils
{
    public class ScoopHelper
    {
        /// <summary>
        /// Root directory of scoop
        /// </summary>
        public static readonly string ScoopRootDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "scoop");

        public static string[] GetAppDirs()
        {
            return Directory.GetDirectories(Path.Combine(ScoopRootDir, "apps"));
        }

        public static string? GetAppVersion(string appDir)
        {
            var current = new DirectoryInfo(Path.Combine(appDir, "current")).LinkTarget;

            if (current == null)
                return null;

            return Path.GetFileName(current);
        }

        public static string? GetAppBucket(string appDir)
        {
            var jsonFile = Path.Combine(appDir, "current", "install.json");
            var jsonText = File.ReadAllText(jsonFile);
            return JObject.Parse(jsonText)["bucket"]?.ToString();
        }

        public static string? GetAppHomePage(string appDir)
        {
            var jsonFile = Path.Combine(appDir, "current", "manifest.json");
            var jsonText = File.ReadAllText(jsonFile);
            return JObject.Parse(jsonText)["homepage"]?.ToString();
        }
    }
}
