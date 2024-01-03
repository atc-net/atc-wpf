namespace Atc.Wpf.Extensions;

public static class DrawingBrushExtensions
{
    public static BitmapSource ToBitmapSource(
        this DrawingBrush brush,
        int pixelWidthAndHeight = 32)
        => brush.ToBitmapSource(
            pixelWidthAndHeight,
            pixelWidthAndHeight,
            dpiX: 96,
            dpiY: 96);
}