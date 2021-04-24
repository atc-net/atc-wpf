using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Atc.Wpf.ValueConverters
{
    /// <summary>
    /// MultiValueConverter: Multi Bool To Visibility-Visible.
    /// </summary>
    [ValueConversion(typeof(List<bool>), typeof(Visibility))]
    public class MultiBoolToVisibilityVisibleValueConverter : IMultiValueConverter
    {
        /// <inheritdoc />
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return Visibility.Visible;
            }

            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            bool visible = true;
            foreach (object value in values)
            {
                if (value is not bool boolValue)
                {
                    throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(bool)");
                }

                if (boolValue)
                {
                    continue;
                }

                visible = false;
                break;
            }

            return visible
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This is a OneWay converter.");
        }
    }
}