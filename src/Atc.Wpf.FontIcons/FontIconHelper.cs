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

    internal static ImageSource CreateImageSource(
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
            pixelsPerDip: 1)
        {
            TextAlignment = TextAlignment.Center,
        };

        var width = (int)formattedText.Width > 0
            ? (int)formattedText.Width
            : (int)(emSize * iconChar.Length);

        var widthHalf = width / 2;
        var height = (int)formattedText.Height;

        var drawingVisual = new DrawingVisual();
        using (var drawingContext = drawingVisual.RenderOpen())
        {
            drawingContext.DrawText(formattedText, new Point(widthHalf, 0));
        }

        var renderTargetBitmap = new RenderTargetBitmap(
            width,
            height,
            96,
            96,
            PixelFormats.Pbgra32);

        renderTargetBitmap.Render(drawingVisual);
        renderTargetBitmap.Freeze();
        return renderTargetBitmap;
    }

    internal static DrawingImage CreateDrawingImage(
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
            pixelsPerDip: 1)
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