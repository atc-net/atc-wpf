using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: ICollection Null Or Empty To Visibility-Collapsed.
    /// </summary>
    [ValueConversion(typeof(ICollection), typeof(Visibility))]
    public class CollectionNullOrEmptyToVisibilityCollapsedValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not ICollection collectionValue || collectionValue.Count == 0
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <inheritdoc />
        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This is a OneWay converter.");
        }
    }
}