namespace Atc.Wpf.Components.Viewers;

public record TerminalLineItem(
    string Text,
    Brush Foreground)
{
    /// <summary>Wall-clock instant the line was received. Defaults to <see cref="DateTimeOffset.Now"/>.</summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;

    /// <summary>Sequential ordinal of the line within the viewer (1-based).</summary>
    public int LineNumber { get; init; }

    /// <summary>Whether the user has pinned this line. Pinned lines survive Clear.</summary>
    public bool IsPinned { get; init; }

    /// <summary>
    /// Optional pre-parsed styled runs (the result of running the line through
    /// <see cref="AnsiSequenceParser"/>). When non-null, the renderer uses
    /// these instead of the single <see cref="Text"/>+<see cref="Foreground"/>
    /// pair, enabling mixed-colour rendering of ANSI-coded output.
    /// </summary>
    public IReadOnlyList<TerminalRun>? Runs { get; init; }
}
