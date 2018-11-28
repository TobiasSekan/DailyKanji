using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DailyKanji.Helper
{
    public class ColorStringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorValue = value as string;

            return string.IsNullOrWhiteSpace(colorValue)
                ? new SolidColorBrush(Colors.Transparent)
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorValue));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}
