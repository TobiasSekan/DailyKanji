using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

#nullable enable

namespace DailyKanji.Helper
{
    /// <summary>
    /// Convert a hexadecimal value of a <see cref="Color"/> into a <see cref="SolidColorBrush"/>
    /// </summary>
    internal sealed class ColorStringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is string colorValue && !string.IsNullOrWhiteSpace(colorValue)
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorValue))
                : new SolidColorBrush(Colors.Transparent);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
