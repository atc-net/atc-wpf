namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// Renders an <see cref="ImageSource"/> to the 32-bit BGRA pixels of a square icon.
/// </summary>
internal static class TrayIconImageRenderer
{
    /// <summary>
    /// Renders the image scaled to <paramref name="size"/> x <paramref name="size"/> pixels.
    /// </summary>
    /// <returns>Top-down rows of BGRA pixels with straight (not premultiplied) alpha, as icons expect.</returns>
    public static byte[] RenderBgra(
        ImageSource source,
        int size)
    {
        ArgumentNullException.ThrowIfNull(source);

        var visual = new DrawingVisual();
        using (var context = visual.RenderOpen())
        {
            context.DrawImage(source, new Rect(0, 0, size, size));
        }

        var rendered = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
        rendered.Render(visual);

        var straight = new FormatConvertedBitmap(rendered, PixelFormats.Bgra32, destinationPalette: null, alphaThreshold: 0);
        var pixels = new byte[size * size * 4];
        straight.CopyPixels(pixels, size * 4, 0);
        return pixels;
    }
}