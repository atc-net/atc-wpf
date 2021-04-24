using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: ICollection Null Or Empty To Bool.
    /// </summary>
    [ValueConversion(typeof(ICollection), typeof(bool))]
    public class CollectionNullOrEmptyToBoolValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not ICollection collectionValue || collectionValue.Count == 0;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This is a OneWay converter.");
        }
    }
}