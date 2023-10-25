namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object Null To Visibility-Collapsed.
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility))]
public sealed class ObjectNullToVisibilityCollapsedValueConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}