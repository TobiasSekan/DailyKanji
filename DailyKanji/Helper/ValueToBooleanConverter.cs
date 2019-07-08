using System;
using System.Globalization;
using System.Windows.Data;

#nullable enable

namespace DailyKanji.Helper
{
    /// <summary>
    /// Compare two given <see cref="int"/> values and return the result as a <see cref="bool"/>
    /// </summary>
    public class ValueToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => System.Convert.ToInt32(value) == System.Convert.ToInt32(parameter);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
