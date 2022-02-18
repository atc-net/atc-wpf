namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object NotNull To Visibility-Visible.
/// </summary>
[ValueConversion(typeof(object), typeof(Visibility))]
public class ObjectNotNullToVisibilityVisibleValueConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}