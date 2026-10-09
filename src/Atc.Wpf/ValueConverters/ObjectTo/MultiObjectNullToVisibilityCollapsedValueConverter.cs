// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// MultiValueConverter: Multi Object Null To Visibility-Collapsed.
/// </summary>
[ValueConversion(typeof(List<object>), typeof(Visibility))]
public sealed class MultiObjectNullToVisibilityCollapsedValueConverter : IMultiValueConverter
{
    /// <summary>
    /// Gets a static default instance of <see cref="MultiObjectNullToVisibilityCollapsedValueConverter"/>.
    /// </summary>
    public static readonly MultiObjectNullToVisibilityCollapsedValueConverter Instance = new();

    /// <inheritdoc />
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

    /// <inheritdoc />
    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}