// ReSharper disable once CheckNamespace
namespace System.Windows.Media;

public static class VisualExtensions
{
    /// <summary>
    /// Create BitmapSource from a visual.
    /// </summary>
    /// <param name="visual">The visual.</param>
    /// <param name="dpiX">The dpi x.</param>
    /// <param name="dpiY">The dpi y.</param>
    /// <returns>The BitmapSource from a visual.</returns>
    public static BitmapSource ToBitmapSource(this Visual visual, double dpiX = 96, double dpiY = 96)
    {
        if (visual is null)
        {
            throw new ArgumentNullException(nameof(visual));
        }

        if (dpiX < 1)
        {
            dpiX = 96;
        }

        if (dpiY < 1)
        {
            dpiY = 96;
        }

        var bounds = VisualTreeHelper.GetDescendantBounds(visual);
        var renderTargetBitmap = new RenderTargetBitmap(
            (int)(bounds.Width * dpiX / 96.0),
            (int)(bounds.Height * dpiY / 96.0),
            dpiX,
            dpiY,
            PixelFormats.Pbgra32);

        var drawingVisual = new DrawingVisual();
        using (DrawingContext drawingContext = drawingVisual.RenderOpen())
        {
            var visualBrush = new VisualBrush(visual);
            drawingContext.DrawRectangle(
                visualBrush,
                pen: null,
                new Rect(
                    new Point(0, 0),
                    bounds.Size));
        }

        renderTargetBitmap.Render(drawingVisual);
        return renderTargetBitmap;
    }
}