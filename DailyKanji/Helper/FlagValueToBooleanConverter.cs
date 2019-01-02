using System;
using System.Globalization;
using System.Windows.Data;

namespace DailyKanji.Helper
{
    public class FlagValueToBooleanConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (System.Convert.ToInt32(value) & System.Convert.ToInt32(parameter)) != 0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
