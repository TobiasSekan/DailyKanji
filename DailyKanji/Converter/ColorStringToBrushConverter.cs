using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DailyKanji.Converter
{
    /// <summary>
    /// Convert a hexadecimal value of a <see cref="Color"/> into a <see cref="SolidColorBrush"/>
    /// </summary>
    internal sealed class ColorStringToBrushConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is System.Drawing.Color color)
                ? new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B))
                : new SolidColorBrush(Colors.Transparent);

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}
