﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace DailyKanji.Converter
{
    /// <summary>
    /// Compare two given <see cref="int"/> values and return the result as a <see cref="bool"/>
    /// </summary>
    internal sealed class ValueToBooleanConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => System.Convert.ToInt32(value) == System.Convert.ToInt32(parameter);

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
