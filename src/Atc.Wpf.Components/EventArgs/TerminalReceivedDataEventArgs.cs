// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components;

/// <summary>
/// Lines of terminal output. Send it through <c>Messenger.Default</c>.
/// </summary>
public sealed class TerminalReceivedDataEventArgs(string[] lines) : EventArgs
{
    /// <summary>
    /// Gets the received lines of terminal output.
    /// </summary>
    public IReadOnlyList<string> Lines { get; } = lines;

    /// <summary>
    /// Gets the <see cref="Viewers.TerminalViewer.TerminalId"/> of the viewer that should show these lines,
    /// or <see langword="null"/> (default) to send them to every viewer.
    /// </summary>
    public string? TerminalId { get; init; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Lines)}.Count: {Lines.Count}";
}