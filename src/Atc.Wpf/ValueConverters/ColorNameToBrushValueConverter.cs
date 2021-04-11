using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Atc.Wpf.Media;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: string-color-name To SolidColorBrush.
    /// </summary>
    [ValueConversion(typeof(string), typeof(Brush))]
    public class ColorNameToBrushValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Brushes.DeepPink;
            }

            if (!(value is string))
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(string)");
            }

            if (!ColorUtil.KnownColors.TryGetValue((string)value, out var color))
            {
                throw new InvalidCastException($"{value} is not a valid brush");
            }

            return new SolidColorBrush(color);
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Brushes.DeepPink.ToString(GlobalizationConstants.EnglishCultureInfo);
            }

            if (!(value is Brush))
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Brush)");
            }

            return value.ToString()!;
        }
    }
}