using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// ValueConverter: NotNullToVisibilityVisible.
    /// </summary>
    public class NotNullToVisibilityVisibleValueConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}