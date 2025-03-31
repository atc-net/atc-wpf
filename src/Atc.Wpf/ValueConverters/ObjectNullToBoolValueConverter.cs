namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Object Null To Bool.
/// </summary>
[ValueConversion(typeof(object), typeof(bool))]
public sealed class ObjectNullToBoolValueConverter : IValueConverter
{
    public static readonly ObjectNullToBoolValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is null;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}