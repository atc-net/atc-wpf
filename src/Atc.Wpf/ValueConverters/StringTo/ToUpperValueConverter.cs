// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: ToUpper.
/// </summary>
[MarkupExtensionReturnType(typeof(ToUpperValueConverter))]
[ValueConversion(typeof(object), typeof(object))]
[ValueConversion(typeof(string), typeof(string))]
public sealed class ToUpperValueConverter : MarkupValueConverterBase
{
    public static readonly ToUpperValueConverter Instance = new();

    /// <inheritdoc />
    protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string s
            ? s.ToUpper(culture)
            : value;
    }

    /// <inheritdoc />
    protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}