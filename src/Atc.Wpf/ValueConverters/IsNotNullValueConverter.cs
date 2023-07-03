namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object To Bool.
/// </summary>
[ValueConversion(typeof(object), typeof(bool))]
public sealed class IsNotNullValueConverter : IValueConverter
{
    public static readonly IsNotNullValueConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not null;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}