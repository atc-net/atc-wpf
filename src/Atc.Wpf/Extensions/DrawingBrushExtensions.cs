namespace Atc.Wpf.Extensions;

/// <summary>
/// Extension methods for <see cref="DrawingBrush"/>.
/// </summary>
public static class DrawingBrushExtensions
{
    /// <summary>
    /// Renders the drawing brush to a square bitmap at 96 DPI.
    /// </summary>
    public static BitmapSource ToBitmapSource(
        this DrawingBrush brush,
        int pixelWidthAndHeight = 32)
        => brush.ToBitmapSource(
            pixelWidthAndHeight,
            pixelWidthAndHeight,
            dpiX: 96,
            dpiY: 96);
}