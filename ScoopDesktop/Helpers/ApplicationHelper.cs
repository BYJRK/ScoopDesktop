using System.Windows;

namespace ScoopDesktop.Helpers
{
    public class ApplicationHelper
    {
        public static void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
