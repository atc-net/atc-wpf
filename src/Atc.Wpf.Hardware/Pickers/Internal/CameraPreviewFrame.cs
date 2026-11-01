namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// A BGRA8 camera frame on its way from the capture thread to the UI thread. It keeps the reader's frame
/// alive (or owns a converted copy) until the UI thread has copied its pixels into the preview bitmap.
/// </summary>
[SuppressMessage("Security", "S6640:Make sure that using \"unsafe\" is safe here", Justification = "OK - By design, required for WinRT buffer interop.")]
internal sealed class CameraPreviewFrame : IDisposable
{
    private readonly IDisposable? owner;
    private readonly bool ownsBitmap;

    private CameraPreviewFrame(
        Windows.Graphics.Imaging.SoftwareBitmap bitmap,
        IDisposable? owner,
        bool ownsBitmap)
    {
        Bitmap = bitmap;
        this.owner = owner;
        this.ownsBitmap = ownsBitmap;
    }

    public Windows.Graphics.Imaging.SoftwareBitmap Bitmap { get; }

    /// <summary>
    /// Takes over <paramref name="frame"/>. A frame the reader already delivers as BGRA8 is used as is; any other
    /// format (a camera that refused the BGRA8 reader) is converted once and the reader's frame released.
    /// </summary>
    /// <returns>The frame, or <see langword="null"/> when it carries no bitmap (then it has been disposed).</returns>
    public static CameraPreviewFrame? From(
        Windows.Media.Capture.Frames.MediaFrameReference frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        var bitmap = frame.VideoMediaFrame?.SoftwareBitmap;
        if (bitmap is null)
        {
            frame.Dispose();
            return null;
        }

        if (bitmap.BitmapPixelFormat is Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8)
        {
            return new CameraPreviewFrame(bitmap, frame, ownsBitmap: false);
        }

        try
        {
            var converted = Windows.Graphics.Imaging.SoftwareBitmap.Convert(
                bitmap,
                Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8,
                Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied);
            return new CameraPreviewFrame(converted, owner: null, ownsBitmap: true);
        }
        finally
        {
            frame.Dispose();
        }
    }

    /// <summary>
    /// Copies the pixels straight from the frame's buffer into <paramref name="target"/>, which must be locked
    /// and have the frame's size.
    /// </summary>
    /// <returns><see langword="false"/> when the buffer could not be read.</returns>
    public unsafe bool CopyTo(WriteableBitmap target)
    {
        ArgumentNullException.ThrowIfNull(target);

        using var buffer = Bitmap.LockBuffer(Windows.Graphics.Imaging.BitmapBufferAccessMode.Read);
        var plane = buffer.GetPlaneDescription(0);
        using var reference = buffer.CreateReference();
        if (!AudioBufferAccess.TryGetBufferPointer(reference, out var data, out var capacity) ||
            plane.StartIndex < 0 ||
            plane.StartIndex > capacity)
        {
            return false;
        }

        var height = Bitmap.PixelHeight;
        var source = new ReadOnlySpan<byte>(data + plane.StartIndex, (int)capacity - plane.StartIndex);
        var destination = new Span<byte>(
            (void*)target.BackBuffer,
            target.BackBufferStride * target.PixelHeight);

        return CameraPreviewFormat.CopyRows(
            source,
            plane.Stride,
            destination,
            target.BackBufferStride,
            Bitmap.PixelWidth * 4,
            height);
    }

    public void Dispose()
    {
        if (ownsBitmap)
        {
            Bitmap.Dispose();
        }

        owner?.Dispose();
    }
}