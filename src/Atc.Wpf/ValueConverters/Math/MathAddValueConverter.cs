// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// MathAddConverter provides a multi value converter as a MarkupExtension which can be used for math operations.
/// This class cannot be inherited.
/// </summary>
[MarkupExtensionReturnType(typeof(MathAddValueConverter))]
public sealed class MathAddValueConverter : MarkupMultiValueConverterBase
{
    public static readonly MathValueConverter Instance = new() { Operation = MathOperation.Add };

    public override object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        return Instance.Convert(values, targetType, parameter, culture);
    }

    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Instance.Convert(value, targetType, parameter, culture);
    }

    public override object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        return Instance.ConvertBack(value, targetTypes, parameter, culture);
    }

    public override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Instance.ConvertBack(value, targetType, parameter, culture);
    }
}