using System;
using System.Globalization;
using System.Windows.Data;
using Atc.Helpers;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: Enum-Description To String.
    /// </summary>
    public class EnumDescriptionToStringValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(value is Enum))
            {
                throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not an enum type");
            }

            return EnumHelper.GetDescription(value as Enum);
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}