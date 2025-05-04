namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Integer > 0 To Visibility-Visible.
/// </summary>
[ValueConversion(typeof(int), typeof(Visibility))]
public sealed class IntegerGreaterThenZeroToVisibilityVisibleValueConverter : IValueConverter
{
    public static readonly IntegerGreaterThenZeroToVisibilityVisibleValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return Visibility.Collapsed;
        }

        if (value is not int intValue)
        {
            throw new UnexpectedTypeException($"Type {value.GetType().FullName} is not typeof(int)");
        }

        return intValue > 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}