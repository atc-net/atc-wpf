namespace Atc.Wpf.Components.Capture;

/// <summary>
/// Default implementation of <see cref="ICaptureService"/> that provides
/// visual element, region, and window capture with file and clipboard output.
/// </summary>
[SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Capture operations should not throw.")]
public class CaptureService : ICaptureService
{
    /// <inheritdoc />
    public BitmapSource CaptureVisual(
        Visual visual,
        CaptureSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(visual);

        settings ??= new CaptureSettings();

        return RenderVisual(visual, settings);
    }

    /// <inheritdoc />
    public BitmapSource CaptureRegion(
        Visual visual,
        Int32Rect region,
        CaptureSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(visual);

        settings ??= new CaptureSettings();

        var fullBitmap = RenderVisual(visual, settings);

        return new CroppedBitmap(fullBitmap, region);
    }

    /// <inheritdoc />
    public BitmapSource CaptureWindow(
        Window window,
        CaptureSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(window);

        settings ??= new CaptureSettings();

        return RenderVisual(window, settings);
    }

    /// <inheritdoc />
    public CaptureResult SaveToFile(
        BitmapSource bitmap,
        string filePath,
        ImageFormatType format = ImageFormatType.Png)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        ArgumentNullException.ThrowIfNull(filePath);

        try
        {
            var fileInfo = new FileInfo(filePath);

            var directory = fileInfo.DirectoryName;
            if (directory is not null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var stream = new FileStream(fileInfo.FullName, FileMode.Create);
            var encoder = BitmapEncoderFactory.Create(format);
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);

            return CaptureResult.FileSuccess(
                bitmap.PixelWidth,
                bitmap.PixelHeight,
                fileInfo.FullName,
                stream.Length);
        }
        catch (Exception ex)
        {
            return CaptureResult.Failed(ex.Message);
        }
    }

    /// <inheritdoc />
    public CaptureResult CopyToClipboard(BitmapSource bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        try
        {
            System.Windows.Clipboard.SetImage(bitmap);

            return CaptureResult.ClipboardSuccess(
                bitmap.PixelWidth,
                bitmap.PixelHeight);
        }
        catch (Exception ex)
        {
            return CaptureResult.Failed(ex.Message);
        }
    }

    private static BitmapSource RenderVisual(
        Visual visual,
        CaptureSettings settings)
    {
        var dpiX = settings.DpiX < 1 ? 96 : settings.DpiX;
        var dpiY = settings.DpiY < 1 ? 96 : settings.DpiY;

        var bounds = VisualTreeHelper.GetDescendantBounds(visual);

        var renderTargetBitmap = new RenderTargetBitmap(
            (int)(bounds.Width * dpiX / 96.0),
            (int)(bounds.Height * dpiY / 96.0),
            dpiX,
            dpiY,
            PixelFormats.Pbgra32);

        var drawingVisual = new DrawingVisual();
        using (var drawingContext = drawingVisual.RenderOpen())
        {
            var visualBrush = new VisualBrush(visual);
            drawingContext.DrawRectangle(
                visualBrush,
                pen: null,
                new Rect(new Point(0, 0), bounds.Size));
        }

        renderTargetBitmap.Render(drawingVisual);

        return renderTargetBitmap;
    }
}