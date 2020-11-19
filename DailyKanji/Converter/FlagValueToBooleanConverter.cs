using System;
using System.Globalization;
using System.Windows.Data;

namespace DailyKanji.Converter
{
    /// <summary>
    /// Compare a given enumeration value with a <see cref="int"/> value and return the result as a <see cref="bool"/>
    /// </summary>
    internal sealed class FlagValueToBooleanConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (System.Convert.ToInt32(value) & System.Convert.ToInt32(parameter)) != 0;

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
