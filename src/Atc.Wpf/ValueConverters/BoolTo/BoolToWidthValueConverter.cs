// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: Bool To Width — returns the specified width (default <c>double.NaN</c>, i.e.
/// <c>Auto</c>) when the bound bool is <see langword="true"/>, otherwise <c>0</c>.
/// </summary>
/// <remarks>
/// <c>ConverterParameter</c> accepts <c>"Auto"</c> (case-insensitive) for <c>double.NaN</c> or
/// any WPF length string parsed by <see cref="LengthConverter"/>. Null/non-bool values return
/// <c>0</c> (collapsed-width fallback) rather than throwing.
/// </remarks>
[ValueConversion(typeof(bool), typeof(LengthConverter))]
public sealed class BoolToWidthValueConverter : IValueConverter
{
    public static readonly BoolToWidthValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            return 0;
        }

        var width = double.NaN;
        if (parameter is null || "Auto".IsEqual(parameter.ToString()!, StringComparison.OrdinalIgnoreCase))
        {
            return boolValue
                ? width
                : 0;
        }

        var lengthConverter = new LengthConverter();
        var s = parameter.ToString();
        if (s is null)
        {
            return 0;
        }

        var convertFromString = lengthConverter.ConvertFromString(s);
        width = convertFromString is not null &&
                double.TryParse(convertFromString.ToString(), NumberStyles.Any, GlobalizationConstants.EnglishCultureInfo, out var result)
            ? result
            : 0;

        return boolValue
            ? width
            : 0;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}