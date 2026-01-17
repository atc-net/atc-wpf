namespace Atc.Wpf.Controls.BaseControls;

/// <summary>
/// Converts track height and thumb size to a vertical offset to center the thumb on the track.
/// </summary>
public sealed class RangeSliderThumbOffsetConverter : IMultiValueConverter
{
    /// <summary>
    /// Gets the singleton instance of the converter.
    /// </summary>
    public static RangeSliderThumbOffsetConverter Instance { get; } = new();

    /// <inheritdoc />
    public object Convert(
        object[] values,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length >= 2 &&
            values[0] is double trackHeight &&
            values[1] is double thumbSize)
        {
            // Center the thumb vertically on the track
            return (trackHeight / 2) - (thumbSize / 2);
        }

        return 0.0;
    }

    /// <inheritdoc />
    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}