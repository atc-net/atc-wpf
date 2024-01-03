namespace Atc.Wpf.ValueConverters;

/// <summary>
/// MultiValueConverter: Multi Object Null To Visibility-Collapsed.
/// </summary>
[ValueConversion(typeof(List<object>), typeof(Visibility))]
public class MultiObjectNullToVisibilityCollapsedValueConverter : IMultiValueConverter
{
    public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (values is null)
        {
            return Visibility.Collapsed;
        }

        foreach (var value in values)
        {
            if (value is null)
            {
                return Visibility.Collapsed;
            }
        }

        return Visibility.Visible;
    }

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}