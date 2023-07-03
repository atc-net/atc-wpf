// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// MathMultiplyConverter provides a multi value converter as a MarkupExtension which can be used for math operations.
/// This class cannot be inherited.
/// </summary>
[MarkupExtensionReturnType(typeof(MathMultiplyValueConverter))]
public sealed class MathMultiplyValueConverter : MarkupMultiValueConverterBase
{
    private static readonly MathValueConverter MathConverter = new() { Operation = MathOperation.Multiply };

    public override object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
    {
        return MathConverter.Convert(values, targetType, parameter, culture);
    }

    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return MathConverter.Convert(value, targetType, parameter, culture);
    }

    public override object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        return MathConverter.ConvertBack(value, targetTypes, parameter, culture);
    }

    public override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return MathConverter.ConvertBack(value, targetType, parameter, culture);
    }
}