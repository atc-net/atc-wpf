// ReSharper disable once CheckNamespace
namespace System.Windows.Media;

public static class BrushExtensions
{
    /// <summary>
    /// Create BitmapSource from a brush.
    /// </summary>
    /// <param name="brush">The brush.</param>
    /// <param name="pixelWidthAndHeight">The pixel width and height.</param>
    /// <param name="dpiX">The dpi x.</param>
    /// <param name="dpiY">The dpi y.</param>
    /// <returns>The BitmapSource from a brush.</returns>
    public static BitmapSource ToBitmapSource(this Brush brush, int pixelWidthAndHeight = 32, double dpiX = 96, double dpiY = 96)
    {
        return brush.ToBitmapSource(
            pixelWidthAndHeight,
            pixelWidthAndHeight,
            dpiX,
            dpiY);
    }

    /// <summary>
    /// Create BitmapSource from a brush.
    /// </summary>
    /// <param name="brush">The brush.</param>
    /// <param name="pixelWidth">The pixel width.</param>
    /// <param name="pixelHeight">The pixel height.</param>
    /// <param name="dpiX">The dpi x.</param>
    /// <param name="dpiY">The dpi y.</param>
    /// <returns>The BitmapSource from a brush.</returns>
    public static BitmapSource ToBitmapSource(this Brush brush, int pixelWidth = 32, int pixelHeight = 32, double dpiX = 96, double dpiY = 96)
    {
        ArgumentNullException.ThrowIfNull(brush);

        if (pixelWidth < 1)
        {
            pixelWidth = 1;
        }

        if (pixelHeight < 1)
        {
            pixelHeight = 1;
        }

        if (dpiX < 1)
        {
            dpiX = 96;
        }

        if (dpiY < 1)
        {
            dpiY = 96;
        }

        var renderTargetBitmap = new RenderTargetBitmap(
            pixelWidth,
            pixelHeight,
            dpiX,
            dpiY,
            PixelFormats.Pbgra32);

        var drawingVisual = new DrawingVisual();
        using (var drawingContext = drawingVisual.RenderOpen())
        {
            drawingContext.DrawRectangle(
                brush,
                pen: null,
                new Rect(
                    0,
                    0,
                    pixelWidth,
                    pixelHeight));
        }

        renderTargetBitmap.Render(drawingVisual);
        return renderTargetBitmap;
    }
}