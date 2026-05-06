namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// A styled segment of a terminal line. Multiple <see cref="TerminalRun"/>s
/// are concatenated to render a single line with mixed colours / weights —
/// the result of parsing ANSI SGR escape sequences.
/// </summary>
public sealed record TerminalRun(
    string Text,
    Brush? Foreground = null,
    Brush? Background = null,
    bool Bold = false,
    bool Italic = false,
    bool Underline = false);