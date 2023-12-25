namespace Atc.Wpf.Extensions;

public static class DrawingBrushExtensions
{
    public static BitmapSource ToBitmapSource(
        this DrawingBrush brush,
        int pixelWidthAndHeight = 32,
        double dpiX = 96,
        double dpiY = 96)
        => BrushExtensions.ToBitmapSource(
            brush,
            pixelWidthAndHeight,
            pixelWidthAndHeight,
            dpiX,
            dpiY);

    public static BitmapSource ToBitmapSource(
        this DrawingBrush brush,
        int pixelWidth = 32,
        int pixelHeight = 32,
        double dpiX = 96,
        double dpiY = 96)
        => BrushExtensions.ToBitmapSource(
            brush,
            pixelWidth,
            pixelHeight,
            dpiX,
            dpiY);
}