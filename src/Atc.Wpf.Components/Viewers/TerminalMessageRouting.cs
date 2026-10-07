namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// Decides whether a <see cref="TerminalViewer"/> should handle a terminal message.
/// </summary>
internal static class TerminalMessageRouting
{
    /// <summary>
    /// A message without a terminal id is a broadcast; a message with an id only reaches the viewer with
    /// exactly that (ordinal) id.
    /// </summary>
    public static bool Accepts(
        string? messageTerminalId,
        string? viewerTerminalId)
        => messageTerminalId is null ||
           string.Equals(messageTerminalId, viewerTerminalId, StringComparison.Ordinal);
}