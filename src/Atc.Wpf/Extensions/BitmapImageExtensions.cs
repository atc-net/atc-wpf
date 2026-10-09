namespace Atc.Wpf.Extensions;

/// <summary>
/// Extension methods for <see cref="BitmapImage"/>.
/// </summary>
public static class BitmapImageExtensions
{
    /// <summary>
    /// Returns the image unchanged when enabled; otherwise returns a grayscale copy of it that keeps its transparency.
    /// </summary>
    public static BitmapImage AutoGrey(
        this BitmapImage image,
        bool isEnabled)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (isEnabled)
        {
            return image;
        }

        // Converted per pixel instead of with a grayscale pixel format, which would drop the transparency.
        var bgra = new FormatConvertedBitmap(image, PixelFormats.Bgra32, destinationPalette: null, alphaThreshold: 0);
        var stride = bgra.PixelWidth * 4;
        var pixels = new byte[stride * bgra.PixelHeight];
        bgra.CopyPixels(pixels, stride, 0);

        for (var i = 0; i < pixels.Length; i += 4)
        {
            var grey = (byte)System.Math.Round(
                (0.0722 * pixels[i]) + (0.7152 * pixels[i + 1]) + (0.2126 * pixels[i + 2]),
                MidpointRounding.AwayFromZero);
            pixels[i] = grey;
            pixels[i + 1] = grey;
            pixels[i + 2] = grey;
        }

        return BitmapSource
            .Create(bgra.PixelWidth, bgra.PixelHeight, image.DpiX, image.DpiY, PixelFormats.Bgra32, palette: null, pixels, stride)
            .ToBitmapImage();
    }
}