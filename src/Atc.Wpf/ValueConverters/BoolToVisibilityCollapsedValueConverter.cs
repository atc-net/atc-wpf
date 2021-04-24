using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: Bool To Visibility-Collapsed.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityCollapsedValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value is not bool boolValue)
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(bool)");
            }

            return boolValue
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value is not Visibility visibility)
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(Visibility)");
            }

            return visibility == Visibility.Collapsed;
        }
    }
}