namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object Null To Visibility-Visible.
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility))]
public sealed class ObjectNullToVisibilityVisibleValueConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}