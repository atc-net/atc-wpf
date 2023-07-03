namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object To Bool.
/// </summary>
[ValueConversion(typeof(object), typeof(bool))]
public sealed class IsNullValueConverter : IValueConverter
{
    public static readonly IsNullValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}