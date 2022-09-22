namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: ToLower.
/// </summary>
[MarkupExtensionReturnType(typeof(ToLowerValueConverter))]
[ValueConversion(typeof(object), typeof(object))]
[ValueConversion(typeof(string), typeof(string))]
public class ToLowerValueConverter : MarkupValueConverter
{
    /// <inheritdoc />
    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string s
            ? s.ToLower(culture)
            : value;
    }

    /// <inheritdoc />
    protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}