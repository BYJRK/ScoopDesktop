﻿using ModernWpf.Controls;
using ScoopDesktop.Utils;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScoopDesktop.Helpers;

public static class PopupHelper
{
    static ContentDialog GetDialog(string? info = null, string? title = null, bool isMono = false, bool preventEsc = false)
    {
        var text = new TextBlock { Text = info };
        if (isMono)
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

    public static async Task ShowInfo(string info, string? title = null, bool isMono = false, bool preventEsc = false, string closeText = "Close")
    {
        var dialog = GetDialog(info, title, isMono, preventEsc);

        dialog.CloseButtonText = closeText;
        dialog.DefaultButton = ContentDialogButton.Close;

        await dialog.ShowAsync();
    }

    public static async Task ShowScoopStatus(string? title, bool isMono = false)
    {
        var dialog = GetDialog(null, title, isMono, true);

        dialog.Loaded += async (_, _) =>
        {
            await PwshHelper.RunCommandAsync("scoop update", (_, e) =>
            {
                if (e.Data is not null
                && (e.Data.StartsWith("Updating") || e.Data.EndsWith("successfully!")))
                {
                    dialog.Dispatcher.Invoke(() =>
                    {
                        var text = (TextBlock)dialog.Content;
                        if (text.Text.Length > 0)
                            text.Text += "\n";
                        text.Text += e.Data;
                    });
                }
            });

            dialog.CloseButtonText = "Done";
            dialog.DefaultButton = ContentDialogButton.Close;
        };

        await dialog.ShowAsync();
    }
}
