using System;
using System.Globalization;
using System.Windows.Data;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: Object Null To Bool.
    /// </summary>
    [ValueConversion(typeof(object), typeof(bool))]
    public class ObjectNullToBoolValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This is a OneWay converter.");
        }
    }
}
