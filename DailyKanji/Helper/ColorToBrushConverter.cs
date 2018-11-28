using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DailyKanji.Helper
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => new SolidColorBrush(value is Color color ? color : Colors.Transparent);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => null;
    }
}
