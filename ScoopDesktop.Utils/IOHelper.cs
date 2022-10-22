using System.Text;

namespace ScoopDesktop.Utils
{
    public interface IOHelper
    {
        public static string ReadTextFromFile(string filename, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;

            return File.ReadAllText(filename, encoding);
        }
    }
}
