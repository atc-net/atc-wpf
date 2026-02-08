namespace Atc.Wpf.Clipboard;

/// <summary>
/// Provides data for the <see cref="IClipboardService.ClipboardChanged"/> event.
/// </summary>
public sealed class ClipboardChangedEventArgs(ClipboardEntry entry) : EventArgs
{
    public ClipboardEntry Entry { get; } = entry;
}