using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: Color To (SolidColor)Brush.
    /// </summary>
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class ColorToBrushValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Brushes.DeepPink;
            }

            if (value is not Color color)
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Color)");
            }

            return new SolidColorBrush(color);
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Colors.DeepPink;
            }

            if (value is not SolidColorBrush brush)
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(SolidColorBrush)");
            }

            return brush.Color;
        }
    }
}
