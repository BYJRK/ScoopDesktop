using System.Text.RegularExpressions;

namespace ScoopDesktop.Utils
{
    public static class StringHelper
    {
        /// <summary>
        /// 将控制台输出的内容转为逐行且去掉首尾空格的字符串数组
        /// </summary>
        public static string[] ToTrimmedLines(this string input)
        {
            return Regex
                .Split(input, @"\r?\n")
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .ToArray();
        }
    }
}
