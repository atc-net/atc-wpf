namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// Running state for an ANSI SGR parse — the terminal "cursor attributes"
/// that persist across escape sequences (and across lines, in real terminals).
/// </summary>
public sealed record AnsiSgrState(
    Brush? Foreground,
    Brush? Background,
    bool Bold,
    bool Italic,
    bool Underline)
{
    public static AnsiSgrState Default { get; } = new(
        Foreground: null,
        Background: null,
        Bold: false,
        Italic: false,
        Underline: false);
}
