using System;
using System.Globalization;
using System.Windows.Data;

namespace DailyKanji.Helper
{
    public sealed class HeightToSmallFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!double.TryParse(value.ToString(), out var actualHigh))
            {
                return 1;
            }

            var newFontSize = actualHigh * 0.15;

            return newFontSize < 1 ? 1 : newFontSize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
