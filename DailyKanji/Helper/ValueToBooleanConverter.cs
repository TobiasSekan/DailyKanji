using System;
using System.Globalization;
using System.Windows.Data;

namespace DailyKanji.Helper
{
    class ValueToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => System.Convert.ToByte(value) == System.Convert.ToByte(parameter);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
