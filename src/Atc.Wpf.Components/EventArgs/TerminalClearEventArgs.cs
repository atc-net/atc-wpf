// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components;

/// <summary>
/// Clears terminal output. Send it through <c>Messenger.Default</c>.
/// </summary>
public sealed class TerminalClearEventArgs : EventArgs
{
    /// <summary>
    /// Gets the <see cref="Viewers.TerminalViewer.TerminalId"/> of the viewer to clear,
    /// or <see langword="null"/> (default) to clear every viewer.
    /// </summary>
    public string? TerminalId { get; init; }
}