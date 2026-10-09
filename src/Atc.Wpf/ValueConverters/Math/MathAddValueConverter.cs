// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// MathAddConverter provides a multi value converter as a MarkupExtension which can be used for math operations.
/// This class cannot be inherited.
/// </summary>
[MarkupExtensionReturnType(typeof(MathAddValueConverter))]
public sealed class MathAddValueConverter : MarkupMultiValueConverterBase
{
    /// <summary>
    /// Gets the shared <see cref="MathValueConverter"/> configured for the <see cref="MathOperation.Add"/> operation.
    /// </summary>
    public static readonly MathValueConverter Instance = new() { Operation = MathOperation.Add };

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