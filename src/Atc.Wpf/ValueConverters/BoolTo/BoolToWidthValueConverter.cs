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
    /// <summary>
    /// Gets a static default instance of <see cref="BoolToWidthValueConverter"/>.
    /// </summary>
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

        var s = parameter.ToString();
        if (s is null)
        {
            return 0;
        }

        // The parameter is XAML text, so it is read the way XAML reads lengths: independent of the current culture.
        var lengthConverter = new LengthConverter();
        width = lengthConverter.ConvertFromInvariantString(s) is double length
            ? length
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