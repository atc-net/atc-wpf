namespace Atc.Wpf.Capture;

/// <summary>
/// Service interface for capturing visual elements as images.
/// Provides methods for capturing controls, regions, and windows
/// to bitmap, file, or clipboard.
/// </summary>
public interface ICaptureService
{
    /// <summary>
    /// Captures a visual element as a bitmap.
    /// </summary>
    /// <param name="visual">The visual element to capture.</param>
    /// <param name="settings">Optional capture settings. Uses defaults if null.</param>
    /// <returns>A <see cref="BitmapSource"/> containing the rendered visual.</returns>
    BitmapSource CaptureVisual(
        Visual visual,
        CaptureSettings? settings = null);

    /// <summary>
    /// Captures a rectangular region of a visual element as a bitmap.
    /// </summary>
    /// <param name="visual">The visual element to capture from.</param>
    /// <param name="region">The region to capture, in pixels relative to the visual.</param>
    /// <param name="settings">Optional capture settings. Uses defaults if null.</param>
    /// <returns>A <see cref="BitmapSource"/> containing the captured region.</returns>
    BitmapSource CaptureRegion(
        Visual visual,
        Int32Rect region,
        CaptureSettings? settings = null);

    /// <summary>
    /// Captures a window's content as a bitmap.
    /// </summary>
    /// <param name="window">The window to capture.</param>
    /// <param name="settings">Optional capture settings. Uses defaults if null.</param>
    /// <returns>A <see cref="BitmapSource"/> containing the rendered window content.</returns>
    BitmapSource CaptureWindow(
        Window window,
        CaptureSettings? settings = null);

    /// <summary>
    /// Saves a bitmap to a file.
    /// </summary>
    /// <param name="bitmap">The bitmap to save.</param>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="format">The image format to use. Default is PNG.</param>
    /// <returns>A <see cref="CaptureResult"/> indicating the outcome.</returns>
    CaptureResult SaveToFile(
        BitmapSource bitmap,
        string filePath,
        ImageFormatType format = ImageFormatType.Png);

    /// <summary>
    /// Copies a bitmap to the system clipboard.
    /// </summary>
    /// <param name="bitmap">The bitmap to copy.</param>
    /// <returns>A <see cref="CaptureResult"/> indicating the outcome.</returns>
    CaptureResult CopyToClipboard(BitmapSource bitmap);
}