namespace Atc.Wpf.Controls.Zoom.ValueConverters;

/// <summary>
/// MultiValueConverter used by the ZoomBox mini-map to clamp the visible viewport indicator's
/// width or height so it never exceeds the actual mini-map extent.
/// </summary>
/// <remarks>
/// Expects four binding values: <c>[0] size</c>, <c>[1] offset</c>, <c>[2] zoom</c>,
/// <c>[3] ZoomBox</c>. <c>ConverterParameter</c> selects the dimension — <c>"width"</c>
/// (case-insensitive) for width, anything else for height.
/// </remarks>
public class ZoomMiniMapClampMultiValueConverter : MarkupExtension, IMultiValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;

    public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (values is null || values.Length < 3)
        {
            return DependencyProperty.UnsetValue;
        }

        if (values[0] == null ||
            values[1] == null ||
            values[2] == null ||
            values[3] is not ZoomBox zoomBox ||
            parameter is null)
        {
            return DependencyProperty.UnsetValue;
        }

        var size = (double)values[0];
        var offset = (double)values[1];
        var zoom = (double)values[2];

        var isWidth = "width".Equals(parameter.ToString(), StringComparison.OrdinalIgnoreCase);

        return System.Math.Max(
            isWidth
            ? System.Math.Min((zoomBox.ExtentWidth / zoom) - offset, size)
            : System.Math.Min((zoomBox.ExtentHeight / zoom) - offset, size),
            0);
    }

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
        => throw new NotSupportedException(
            $"{nameof(ZoomMiniMapClampMultiValueConverter)} is one-way only.");
}