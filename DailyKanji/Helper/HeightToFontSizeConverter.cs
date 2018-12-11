using System;
using System.Globalization;
using System.Windows.Data;

namespace DailyKanji.Helper
{
    public sealed class HeightToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!double.TryParse(value.ToString(), out var actualHigh))
            {
                return 1;
            }

            var newFontSize = actualHigh * 0.65;

            return newFontSize < 1 ? 1 : newFontSize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
