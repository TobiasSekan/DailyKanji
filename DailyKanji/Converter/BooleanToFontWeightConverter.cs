using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable enable

namespace DailyKanji.Converter
{
    /// <summary>
    /// Convert a <see cref="bool"/> value into a <see cref="FontWeight"/> value
    /// </summary>
    internal sealed class BooleanToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is bool booleanValue) && booleanValue
                ? FontWeights.ExtraBold
                : FontWeights.Normal;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => false;
    }
}
