using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DailyKanji.Helper
{
    internal sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool booleanValue && booleanValue
                ? Visibility.Visible
                : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
