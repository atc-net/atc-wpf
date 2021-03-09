using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: MultiBooleanToBoolean.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class MultiBooleanToBooleanValueConverter : IMultiValueConverter
    {
        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.All(value => !(value is bool) || (bool)value);
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("BooleanAndConverter is a OneWay converter.");
        }
    }
}