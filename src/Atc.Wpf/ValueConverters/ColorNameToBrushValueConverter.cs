using System;
using System.Globalization;
using System.Linq;
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

            if (value is not string stringValue)
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(string)");
            }

            if (!ColorUtil.KnownColors.TryGetValue(stringValue, out var color))
            {
                throw new InvalidCastException($"{stringValue} is not a valid brush");
            }

            return new SolidColorBrush(color);
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return "DeepPink";
            }

            if (value is not Brush)
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Brush)");
            }

            var knownColor = ColorUtil.KnownColors.FirstOrDefault(x => x.Value.ToString(GlobalizationConstants.EnglishCultureInfo) == value.ToString());
            return string.IsNullOrEmpty(knownColor.Key)
                ? value.ToString()!
                : knownColor.Key;
        }
    }
}