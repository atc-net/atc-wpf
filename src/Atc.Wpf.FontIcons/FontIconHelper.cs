namespace Atc.Wpf.FontIcons;

internal static class FontIconHelper
{
    internal static object SpinDurationCoerceValue(
        DependencyObject d,
        object value)
    {
        var val = (double)value;
        return val < 0 ? 0d : value;
    }

    [SuppressMessage("Major Code Smell", "S3358:Ternary operators should not be nested", Justification = "OK.")]
    internal static object RotationCoerceValue(
        DependencyObject d,
        object value)
    {
        var val = (double)value;
        return val < 0
            ? 0d
            : val > 360
                ? 360d
                : value;
    }

    internal static DrawingImage CreateImageSource(
        Typeface typeface,
        string iconChar,
        Brush foregroundBrush,
        double emSize = 100)
    {
        var formattedText = new FormattedText(
            iconChar,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            typeface,
            emSize,
            foregroundBrush,
            1)
        {
            TextAlignment = TextAlignment.Center,
        };

        var visual = new DrawingVisual();
        using (var drawingContext = visual.RenderOpen())
        {
            drawingContext.DrawText(formattedText, new Point(0, 0));
        }

        var drawingImage = new DrawingImage(visual.Drawing);
        drawingImage.Freeze();
        return drawingImage;
    }
}