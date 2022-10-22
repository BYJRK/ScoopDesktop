using System;
using System.Globalization;
using System.Windows;

namespace ScoopDesktop.Converters;

public class BoolToVisibilityConverter : BaseConverter
{
    public bool UseHidden { get; set; }
    public bool Reverse { get; set; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool v)
            throw new ArgumentException();

        if (Reverse) v = !v;

        if (v)
            return Visibility.Visible;
        else
            return UseHidden ? Visibility.Hidden : Visibility.Collapsed;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
