namespace Atc.Wpf.Clipboard;

/// <summary>
/// Best-effort access to the Windows clipboard.
/// </summary>
/// <remarks>
/// Another process can hold the clipboard open, in which case <c>Clipboard.SetText</c> throws a
/// <c>COMException</c>; copying is a convenience, so the failure is reported as a result instead.
/// </remarks>
public static class ClipboardHelper
{
    /// <summary>
    /// Puts the text on the clipboard.
    /// </summary>
    /// <param name="text">The text to copy; <see langword="null"/> copies nothing.</param>
    /// <returns><see langword="true"/> when the text is on the clipboard.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Another process can hold the clipboard; copying is best effort.")]
    public static bool TrySetText(string? text)
    {
        if (text is null)
        {
            return false;
        }

        try
        {
            System.Windows.Clipboard.SetText(text);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}