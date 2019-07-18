using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DailyKanji.Helper
{
    /// <summary>
    /// Convert a <see cref="bool"/> value into a <see cref="FontWeight"/> value
    /// </summary>
    public sealed class BooleanToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is bool booleanValue) && booleanValue
                ? FontWeights.ExtraBold
                : FontWeights.Normal;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => false;
    }
}
