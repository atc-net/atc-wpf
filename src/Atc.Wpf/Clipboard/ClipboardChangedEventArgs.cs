namespace Atc.Wpf.Clipboard;

/// <summary>
/// Provides data for the <see cref="IClipboardService.ClipboardChanged"/> event.
/// </summary>
public sealed class ClipboardChangedEventArgs(ClipboardEntry entry) : EventArgs
{
    /// <summary>
    /// Gets the clipboard entry that was captured.
    /// </summary>
    public ClipboardEntry Entry { get; } = entry;
}