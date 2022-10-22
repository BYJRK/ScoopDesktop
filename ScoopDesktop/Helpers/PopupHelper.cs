using ModernWpf.Controls;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScoopDesktop.Helpers;

public static class PopupHelper
{
    public static async Task ShowInfo(string info, string title, bool isMono = false, bool preventEsc = false)
    {
        var text = new TextBlock { Text = info };
        if (isMono) text.FontFamily = new FontFamily("Cascadia Mono");

        var dialog = new ContentDialog
        {
            Title = title,
            Content = text,
            PrimaryButtonText = "Close",
            DefaultButton = ContentDialogButton.Primary,
        };

        if (preventEsc)
        {
            dialog.KeyDown += (_, e) =>
            {
                if (e.Key == Key.Escape)
                    e.Handled = true;
            };
        }

        await dialog.ShowAsync();
    }
}
