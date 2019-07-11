using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

#nullable enable

namespace DailyKanji.Helper
{
    /// <summary>
    /// Multiply the given high value and command parameter to a new font size
    /// </summary>
    public sealed class HeightToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!float.TryParse(value.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out var high))
            {
                Debug.WriteLine($"HeightToFontSizeConverter: Can't parse [{nameof(value)}]:[{value}] to double value");
                return 10;
            }

            if(high < 1)
            {
                return 10;
            }

            if(!int.TryParse(parameter.ToString(), NumberStyles.None, CultureInfo.CurrentCulture, out var multiplicator))
            {
                Debug.WriteLine($"HeightToFontSizeConverter: Can't parse [{nameof(parameter)}]:[{parameter}] to double value");
                return 10;
            }

            var newFontSize = high * multiplicator / 100;
            if(newFontSize > 1)
            {
                return newFontSize > 500 ? 500 : newFontSize;
            }

            Debug.WriteLine($"HeightToFontSizeConverter: Calculated value [{newFontSize}] is to low, fallback to 1");
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
