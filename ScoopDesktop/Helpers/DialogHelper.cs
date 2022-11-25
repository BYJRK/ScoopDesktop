using ModernWpf.Controls;
using ScoopDesktop.Utils;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScoopDesktop.Helpers;

public static class DialogHelper
{
    /// <summary>
    /// Create a new ContentDialog
    /// </summary>
    /// <param name="info">The information shown in the dialog (default is empty)</param>
    /// <param name="title">The Title of the dialog (default is empty)</param>
    /// <param name="monospace">Whether to use monospace font to display the info</param>
    /// <param name="preventEsc">Whether to prevent the dialog from being closed by pressing ESC</param>
    /// <returns></returns>
    static ContentDialog GetDialog(
        string? info = null,
        string? title = null,
        bool monospace = false,
        bool preventEsc = false)
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

    /// <summary>
    /// Show a simple message in a ContentDialog
    /// </summary>
    /// <param name="info">The information to be shown in the dialog</param>
    /// <param name="title">The title of the dialog</param>
    /// <param name="monospace">Whether to use monospace font to display the information</param>
    /// <param name="closeText">The text on close button</param>
    /// <returns></returns>
    public static async Task Info(
        string info,
        string? title = null,
        bool monospace = false,
        string closeText = "Close")
    {
        var dialog = GetDialog(info, title, monospace);

        dialog.CloseButtonText = closeText;
        dialog.DefaultButton = ContentDialogButton.Close;

        await dialog.ShowAsync();
    }

    /// <summary>
    /// Progressively show powershell outputs inside the ContentDialog
    /// </summary>
    /// <param name="command">The command to run in the powershell</param>
    /// <param name="title">The title of the dialog</param>
    /// <param name="closeText">The text shown on the close button</param>
    /// <param name="monospace">Whether to use monospace font to display information</param>
    /// <param name="rule">The rule to decide whether a line is printed</param>
    /// <returns></returns>
    public static async Task Progressive(
        string command,
        string? title,
        string closeText = "Done",
        bool monospace = false,
        Predicate<string>? rule = null)
    {
        var dialog = GetDialog(null, title, monospace, true);

        dialog.Loaded += async (_, _) =>
        {
            await PwshHelper.RunCommandAsync(command, line =>
            {
                if (rule == null || rule.Invoke(line))
                {
                    dialog.Dispatcher.Invoke(() =>
                    {
                        var box = (TextBlock)dialog.Content;
                        if (!string.IsNullOrWhiteSpace(box.Text))
                            box.Text += Environment.NewLine;
                        box.Text += line;
                    });
                }
            });

            dialog.CloseButtonText = closeText;
            dialog.DefaultButton = ContentDialogButton.Close;
        };

        await dialog.ShowAsync();
    }

    /// <summary>
    /// Show a simple ContentDialog to ask user to choose yes or no
    /// </summary>
    /// <param name="info">The message shown in the dialog</param>
    /// <param name="title">The title of the dialog</param>
    /// <param name="yes">The text on the primary button</param>
    /// <param name="no">The text on the close button</param>
    /// <returns></returns>
    public static async Task<ContentDialogResult> YesNo(
        string info,
        string? title = null,
        string yes = "Yes",
        string no = "No")
    {
        var dialog = GetDialog(info, title);

        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.PrimaryButtonText = yes;
        dialog.CloseButtonText = no;

        return await dialog.ShowAsync();
    }
}