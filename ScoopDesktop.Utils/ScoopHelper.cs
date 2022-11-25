using Newtonsoft.Json.Linq;

namespace ScoopDesktop.Utils
{
    public class ScoopHelper
    {
        private ScoopHelper() { }

        /// <summary>
        /// Root directory of scoop
        /// </summary>
        public static readonly string ScoopRootDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "scoop");

        /// <summary>
        /// 获取所有 ~/scoop/apps 下的文件夹的路径
        /// </summary>
        /// <returns></returns>
        public static string[] GetAppDirs()
        {
            return Directory.GetDirectories(Path.Combine(ScoopRootDir, "apps"));
        }

        /// <summary>
        /// 根据 app 文件夹中的 current 文件指向的文件夹名来判断当前 app 的版本号
        /// </summary>
        public static string? GetAppVersion(string appDir)
        {
            var current = new DirectoryInfo(Path.Combine(appDir, "current")).LinkTarget;

            if (current == null)
                return null;

            return Path.GetFileName(current);
        }

        /// <summary>
        /// 从 app 文件夹中的 install.json 中获取对应的 bucket
        /// </summary>
        public static string? GetAppBucket(string appDir)
        {
            var jsonFile = Path.Combine(appDir, "current", "install.json");
            if (!File.Exists(jsonFile))
                return null;
            var jsonText = File.ReadAllText(jsonFile);
            return JObject.Parse(jsonText)["bucket"]?.ToString();
        }

        /// <summary>
        /// 从 app 文件夹中的 manifest.json 中获取 app 的网站链接
        /// </summary>
        public static string? GetAppHomePage(string appDir)
        {
            var jsonFile = Path.Combine(appDir, "current", "manifest.json");
            if (!File.Exists(jsonFile))
                return null;
            var jsonText = File.ReadAllText(jsonFile);
            return JObject.Parse(jsonText)["homepage"]?.ToString();
        }
    }
}
