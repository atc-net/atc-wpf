namespace Atc.Wpf.Network.Vnc;

/// <summary>
/// Provides data for the <see cref="VncConnectionService.ClipboardTextReceived"/> event.
/// </summary>
/// <param name="text">The clipboard text the server sent.</param>
public sealed class VncClipboardEventArgs(string text)
    : EventArgs
{
    /// <summary>
    /// Gets the clipboard text the server sent.
    /// </summary>
    public string Text { get; } = text;
}