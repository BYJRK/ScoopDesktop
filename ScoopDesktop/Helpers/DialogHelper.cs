using ModernWpf.Controls;
using ScoopDesktop.Utils;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScoopDesktop.Helpers;

public static class DialogHelper
{
    static ContentDialog GetDialog(string? info = null, string? title = null, bool monospace = false, bool preventEsc = false)
    {
        var text = new TextBlock { Text = info };
        if (monospace)
            text.FontFamily = new FontFamily("Cascadia Mono");

        var dialog = new ContentDialog
        {
            Title = title,
            Content = text
        };

        if (preventEsc)
        {
            // Prevent closing dialog by pressing ESC
            dialog.PreviewKeyDown += (_, e) =>
            {
                if (e.Key == Key.Escape)
                    e.Handled = true;
            };
        }

        return dialog;
    }

    public static async Task Info(string info, string? title = null, bool monospace = false, bool preventEsc = false, string closeText = "Close")
    {
        var dialog = GetDialog(info, title, monospace, preventEsc);

        dialog.CloseButtonText = closeText;
        dialog.DefaultButton = ContentDialogButton.Close;

        await dialog.ShowAsync();
    }

    public static async Task Progressive(string command, string? title, string closeText = "Done", bool monospace = false, Predicate<string>? rule = null)
    {
        var dialog = GetDialog(null, title, monospace, true);

        dialog.Loaded += async (_, _) =>
        {
            await PwshHelper.RunCommandAsync(command, (_, e) =>
            {
                if (e.Data is null)
                    return;

                if (rule == null || rule.Invoke(e.Data))
                {
                    dialog.Dispatcher.Invoke(() =>
                    {
                        var box = (TextBlock)dialog.Content;
                        if (!string.IsNullOrWhiteSpace(box.Text))
                            box.Text += Environment.NewLine;
                        box.Text += e.Data;
                    });
                }
            });

            dialog.CloseButtonText = closeText;
            dialog.DefaultButton = ContentDialogButton.Close;
        };

        await dialog.ShowAsync();
    }

    public static async Task<ContentDialogResult> YesNo(string info, string? title = null, string yes = "Yes", string no = "No")
    {
        var dialog = GetDialog(info, title);

        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.PrimaryButtonText = yes;
        dialog.CloseButtonText = no;

        return await dialog.ShowAsync();
    }
}