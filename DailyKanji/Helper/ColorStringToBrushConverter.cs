using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DailyKanji.Helper
{
    public class ColorStringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is string colorValue && !string.IsNullOrWhiteSpace(colorValue)
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorValue))
                : new SolidColorBrush(Colors.Transparent);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}
