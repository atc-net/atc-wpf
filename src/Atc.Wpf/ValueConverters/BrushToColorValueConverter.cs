using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: (SolidColor)Brush To Color.
    /// </summary>
    [ValueConversion(typeof(SolidColorBrush), typeof(Color))]
    public class BrushToColorValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
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

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Brushes.DeepPink;
            }

            if (value is not Color color)
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Color)");
            }

            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
}
