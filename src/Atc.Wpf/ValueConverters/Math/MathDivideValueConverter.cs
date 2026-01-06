// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// MathDivideValueConverter provides a multi value converter as a MarkupExtension which can be used for math operations.
/// This class cannot be inherited.
/// </summary>
[MarkupExtensionReturnType(typeof(MathDivideValueConverter))]
public sealed class MathDivideValueConverter : MarkupMultiValueConverterBase
{
    public static readonly MathValueConverter Instance = new() { Operation = MathOperation.Divide };

    public override object? Convert(
        object[]? values,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => Instance.Convert(values, targetType, parameter, culture);

    public override object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => Instance.Convert(value, targetType, parameter, culture);

    public override object[] ConvertBack(
        object? value,
        Type[] targetTypes,
        object? parameter,
        CultureInfo culture)
        => Instance.ConvertBack(value, targetTypes, parameter, culture);

    public override object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => Instance.ConvertBack(value, targetType, parameter, culture);
}