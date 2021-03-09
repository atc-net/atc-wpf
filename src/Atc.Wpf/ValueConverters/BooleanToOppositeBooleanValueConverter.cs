using System;
using System.Globalization;
using System.Windows.Data;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: BooleanToOppositeBoolean.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanToOppositeBooleanValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(value is bool))
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(bool)");
            }

            return !(bool)value;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}