// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// MathMultiplyConverter provides a multi value converter as a MarkupExtension which can be used for math operations.
/// This class cannot be inherited.
/// </summary>
[MarkupExtensionReturnType(typeof(MathMultiplyValueConverter))]
public sealed class MathMultiplyValueConverter : MarkupMultiValueConverterBase
{
    /// <summary>
    /// Gets the shared <see cref="MathValueConverter"/> configured for the <see cref="MathOperation.Multiply"/> operation.
    /// </summary>
    public static readonly MathValueConverter Instance = new() { Operation = MathOperation.Multiply };

    /// <inheritdoc />
    public override object? Convert(
        object[]? values,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => Instance.Convert(values, targetType, parameter, culture);

    /// <inheritdoc />
    public override object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => Instance.Convert(value, targetType, parameter, culture);

    /// <inheritdoc />
    public override object[] ConvertBack(
        object? value,
        Type[] targetTypes,
        object? parameter,
        CultureInfo culture)
        => Instance.ConvertBack(value, targetTypes, parameter, culture);

    /// <inheritdoc />
    public override object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => Instance.ConvertBack(value, targetType, parameter, culture);
}