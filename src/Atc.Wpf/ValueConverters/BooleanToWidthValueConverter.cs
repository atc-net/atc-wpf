using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: BooleanToWidth.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(LengthConverter))]
    public class BooleanToWidthValueConverter : IValueConverter
    {
        /// <inheritdoc />
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
        public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(value is bool))
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(bool)");
            }

            double width = double.NaN;
            if (parameter is null || "Auto".IsEqual(parameter.ToString()!, StringComparison.OrdinalIgnoreCase))
            {
                return (bool)value
                    ? width
                    : 0;
            }

            var lengthConverter = new LengthConverter();
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                width = (double)lengthConverter.ConvertFromString(parameter.ToString());
            }
            catch
            {
                // Dummy
            }

            return (bool)value
                ? width
                : 0;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}