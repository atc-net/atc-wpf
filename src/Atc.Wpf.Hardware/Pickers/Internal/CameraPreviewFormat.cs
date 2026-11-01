namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// The size and pixel format decisions of the live camera preview.
/// </summary>
internal static class CameraPreviewFormat
{
    /// <summary>
    /// The preview height, in pixels, assumed while the preview has not been laid out yet.
    /// </summary>
    public const double FallbackPreviewPixelHeight = 480;

    /// <summary>
    /// Gets the frame size to ask the frame reader for: the camera's aspect ratio at the height the preview is
    /// shown at, so the capture pipeline scales the frames down instead of the CPU copying full-resolution frames
    /// that WPF then shrinks. Dimensions are even, as video processors require.
    /// </summary>
    /// <param name="sourceWidth">The width of the camera format.</param>
    /// <param name="sourceHeight">The height of the camera format.</param>
    /// <param name="previewPixelHeight">The height the preview is shown at, in device pixels.</param>
    /// <returns>The reduced size, or <see langword="null"/> to keep the camera size (never upscaled).</returns>
    public static (uint Width, uint Height)? GetReaderSize(
        uint sourceWidth,
        uint sourceHeight,
        double previewPixelHeight)
    {
        if (sourceWidth == 0 || sourceHeight == 0 || double.IsNaN(previewPixelHeight) || previewPixelHeight <= 0)
        {
            return null;
        }

        var height = RoundToEven(System.Math.Ceiling(previewPixelHeight));
        if (height >= sourceHeight)
        {
            return null;
        }

        var width = RoundToEven((double)sourceWidth * height / sourceHeight);
        return (width, height);
    }

    /// <summary>
    /// Gets the WPF pixel format matching a BGRA8 frame's alpha mode. Camera frames are opaque, and with
    /// <see cref="Windows.Graphics.Imaging.BitmapAlphaMode.Ignore"/> the alpha bytes can be 0, which a
    /// <see cref="PixelFormats.Bgra32"/> bitmap would render invisible.
    /// </summary>
    public static PixelFormat ToWpfPixelFormat(
        Windows.Graphics.Imaging.BitmapAlphaMode alphaMode)
        => alphaMode switch
        {
            Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied => PixelFormats.Pbgra32,
            Windows.Graphics.Imaging.BitmapAlphaMode.Straight => PixelFormats.Bgra32,
            _ => PixelFormats.Bgr32,
        };

    /// <summary>
    /// Copies <paramref name="rows"/> rows of <paramref name="rowBytes"/> bytes between buffers whose strides may
    /// differ (a WinRT plane and a WPF back buffer can both pad their rows).
    /// </summary>
    /// <returns><see langword="false"/> when either buffer is too small; nothing is copied then.</returns>
    public static bool CopyRows(
        ReadOnlySpan<byte> source,
        int sourceStride,
        Span<byte> destination,
        int destinationStride,
        int rowBytes,
        int rows)
    {
        if (rows <= 0 || rowBytes <= 0)
        {
            return true;
        }

        if (sourceStride < rowBytes ||
            destinationStride < rowBytes ||
            source.Length < ((long)sourceStride * (rows - 1)) + rowBytes ||
            destination.Length < ((long)destinationStride * (rows - 1)) + rowBytes)
        {
            return false;
        }

        if (sourceStride == rowBytes && destinationStride == rowBytes)
        {
            source[..(rowBytes * rows)].CopyTo(destination);
            return true;
        }

        for (var row = 0; row < rows; row++)
        {
            source.Slice(row * sourceStride, rowBytes).CopyTo(destination.Slice(row * destinationStride, rowBytes));
        }

        return true;
    }

    private static uint RoundToEven(double value)
    {
        var rounded = (uint)System.Math.Round(value / 2, MidpointRounding.AwayFromZero) * 2;
        return System.Math.Max(2u, rounded);
    }
}