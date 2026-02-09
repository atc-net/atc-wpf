namespace Atc.Wpf.Capture;

/// <summary>
/// Represents the result of a capture operation.
/// </summary>
public sealed class CaptureResult
{
    private CaptureResult()
    {
    }

    /// <summary>
    /// Gets a value indicating whether the capture operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; private init; }

    /// <summary>
    /// Gets the file path where the image was saved, if applicable.
    /// </summary>
    public string? FilePath { get; private init; }

    /// <summary>
    /// Gets the width of the captured image in pixels.
    /// </summary>
    public int Width { get; private init; }

    /// <summary>
    /// Gets the height of the captured image in pixels.
    /// </summary>
    public int Height { get; private init; }

    /// <summary>
    /// Gets the file size in bytes, if saved to a file.
    /// </summary>
    public long FileSizeBytes { get; private init; }

    /// <summary>
    /// Gets the error message if the capture operation failed.
    /// </summary>
    public string? ErrorMessage { get; private init; }

    /// <summary>
    /// Creates a successful capture result for a file save operation.
    /// </summary>
    /// <param name="width">The width of the captured image.</param>
    /// <param name="height">The height of the captured image.</param>
    /// <param name="filePath">The file path where the image was saved.</param>
    /// <param name="fileSizeBytes">The file size in bytes.</param>
    /// <returns>A new <see cref="CaptureResult"/> indicating success.</returns>
    public static CaptureResult FileSuccess(
        int width,
        int height,
        string filePath,
        long fileSizeBytes)
        => new()
        {
            IsSuccess = true,
            Width = width,
            Height = height,
            FilePath = filePath,
            FileSizeBytes = fileSizeBytes,
        };

    /// <summary>
    /// Creates a successful capture result for a clipboard operation.
    /// </summary>
    /// <param name="width">The width of the captured image.</param>
    /// <param name="height">The height of the captured image.</param>
    /// <returns>A new <see cref="CaptureResult"/> indicating success.</returns>
    public static CaptureResult ClipboardSuccess(
        int width,
        int height)
        => new()
        {
            IsSuccess = true,
            Width = width,
            Height = height,
        };

    /// <summary>
    /// Creates a failed capture result.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A new <see cref="CaptureResult"/> indicating failure.</returns>
    public static CaptureResult Failed(string errorMessage)
        => new()
        {
            ErrorMessage = errorMessage,
        };

    /// <inheritdoc />
    public override string ToString()
    {
        if (!IsSuccess)
        {
            return $"Failed: {ErrorMessage}";
        }

        if (FilePath is not null)
        {
            return $"Saved {Width}x{Height} ({FileSizeBytes:N0} bytes) to {FilePath}";
        }

        return $"Copied {Width}x{Height} to clipboard";
    }
}