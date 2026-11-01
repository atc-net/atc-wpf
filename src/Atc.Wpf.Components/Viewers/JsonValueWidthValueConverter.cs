// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// The width of a JSON value text: the viewer width minus the room for the key column (the second value).
/// Before the viewer has a width (a row realized early) the text sizes itself instead of getting a negative width.
/// </summary>
internal sealed class JsonValueWidthValueConverter : IMultiValueConverter
{
    /// <summary>
    /// Gets the shared instance of the converter.
    /// </summary>
    public static readonly JsonValueWidthValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object[]? values,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (values is not { Length: 2 } ||
            values[0] is not double width ||
            !double.TryParse(System.Convert.ToString(values[1], CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out var reserved))
        {
            return double.NaN;
        }

        var result = width - reserved;
        return result > 0 ? result : double.NaN;
    }

    /// <inheritdoc />
    public object[] ConvertBack(
        object? value,
        Type[] targetTypes,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();
}