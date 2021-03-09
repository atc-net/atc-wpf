using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: BrushToColor.
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

            if (!(value is SolidColorBrush))
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(SolidColorBrush)");
            }

            var brush = (SolidColorBrush)value;
            return brush.Color;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Brushes.DeepPink;
            }

            if (!(value is Color))
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Color)");
            }

            var color = (Color)value;
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
}
