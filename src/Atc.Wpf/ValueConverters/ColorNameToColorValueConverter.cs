using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Atc.Wpf.Media;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: string-color-name To Color.
    /// </summary>
    [ValueConversion(typeof(string), typeof(Color))]
    public class ColorNameToColorValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Colors.DeepPink;
            }

            if (!(value is string))
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(string)");
            }

            if (!ColorUtil.KnownColors.TryGetValue((string)value, out var color))
            {
                throw new InvalidCastException($"{value} is not a valid color");
            }

            return color;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Colors.DeepPink.ToString(GlobalizationConstants.EnglishCultureInfo);
            }

            if (!(value is Color))
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Color)");
            }

            return value.ToString()!;
        }
    }
}