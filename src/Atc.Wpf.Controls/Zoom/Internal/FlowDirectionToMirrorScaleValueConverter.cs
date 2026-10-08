namespace Atc.Wpf.Controls.Zoom.Internal;

/// <summary>
/// ValueConverter: FlowDirection to a horizontal scale, -1 for RightToLeft and 1 otherwise.
/// </summary>
internal sealed class FlowDirectionToMirrorScaleValueConverter : IValueConverter
{
    public static readonly FlowDirectionToMirrorScaleValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is FlowDirection.RightToLeft ? -1d : 1d;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();
}