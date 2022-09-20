namespace Atc.Wpf.ValueConverters;

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
        return Binding.DoNothing;
    }
}