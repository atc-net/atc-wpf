// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// Parses ANSI control sequences out of a single line and emits a list of
/// styled <see cref="TerminalRun"/>s. Supports SGR (Select Graphic Rendition)
/// for the standard 16-colour palette plus bold / italic / underline. Other
/// non-SGR sequences (cursor movement, screen clear, etc.) are stripped
/// silently. 256-colour and 24-bit (true colour) extensions are recognised
/// and rendered: 256-indices 0–15 map onto <see cref="AnsiPalette.Colors16"/>;
/// indices 16+ are ignored until the extended palette is added.
/// </summary>
public static class AnsiSequenceParser
{
    private const char Esc = (char)0x1B;

    private static readonly Regex EscapeSequenceRegex = new(
        Esc + @"\[([0-9;]*)([A-Za-z])",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(50));

    /// <summary>
    /// <c>true</c> if <paramref name="text"/> contains at least one ANSI
    /// CSI escape sequence. Cheap pre-check so callers can skip parsing on
    /// plain-text lines.
    /// </summary>
    public static bool ContainsEscapeSequence(string text)
        => !string.IsNullOrEmpty(text) && text.Contains(Esc, StringComparison.Ordinal);

    /// <summary>
    /// Splits <paramref name="text"/> into runs and returns the SGR state
    /// that survives at end-of-line — pass that back as <paramref name="state"/>
    /// for the next line so colours can span lines.
    /// </summary>
    public static (List<TerminalRun> Runs, AnsiSgrState NewState) Parse(
        string text,
        AnsiSgrState state)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(state);

        var runs = new List<TerminalRun>();
        var pos = 0;

        foreach (Match match in EscapeSequenceRegex.Matches(text))
        {
            if (match.Index > pos)
            {
                runs.Add(BuildRun(text[pos..match.Index], state));
            }

            if (match.Groups[2].Value == "m")
            {
                state = ApplySgr(state, match.Groups[1].Value);
            }

            // else: non-SGR sequence (cursor movement etc.) — strip silently.
            pos = match.Index + match.Length;
        }

        if (pos < text.Length)
        {
            runs.Add(BuildRun(text[pos..], state));
        }

        return (runs, state);
    }

    private static TerminalRun BuildRun(
        string text,
        AnsiSgrState state)
        => new(
            text,
            state.Foreground,
            state.Background,
            state.Bold,
            state.Italic,
            state.Underline);

    private static AnsiSgrState ApplySgr(
        AnsiSgrState state,
        string paramsCsv)
    {
        var parts = ParseParams(paramsCsv);

        for (var i = 0; i < parts.Count; i++)
        {
            var code = parts[i];
            switch (code)
            {
                case 0:
                    state = AnsiSgrState.Default;
                    break;
                case 1:
                    state = state with { Bold = true };
                    break;
                case 22:
                    state = state with { Bold = false };
                    break;
                case 3:
                    state = state with { Italic = true };
                    break;
                case 23:
                    state = state with { Italic = false };
                    break;
                case 4:
                    state = state with { Underline = true };
                    break;
                case 24:
                    state = state with { Underline = false };
                    break;
                case 39:
                    state = state with { Foreground = null };
                    break;
                case 49:
                    state = state with { Background = null };
                    break;
                case >= 30 and <= 37:
                    state = state with { Foreground = AnsiPalette.Colors16[code - 30] };
                    break;
                case >= 40 and <= 47:
                    state = state with { Background = AnsiPalette.Colors16[code - 40] };
                    break;
                case >= 90 and <= 97:
                    state = state with { Foreground = AnsiPalette.Colors16[code - 90 + 8] };
                    break;
                case >= 100 and <= 107:
                    state = state with { Background = AnsiPalette.Colors16[code - 100 + 8] };
                    break;
                case 38 or 48:
                    state = ApplyExtendedColor(state, parts, ref i, foreground: code == 38);
                    break;
            }
        }

        return state;
    }

    private static AnsiSgrState ApplyExtendedColor(
        AnsiSgrState state,
        IReadOnlyList<int> parts,
        ref int i,
        bool foreground)
    {
        if (i + 1 >= parts.Count)
        {
            return state;
        }

        var mode = parts[i + 1];

        if (mode == 5 && i + 2 < parts.Count)
        {
            var index = parts[i + 2];
            i += 2;
            if (index >= 0 && index < AnsiPalette.Colors16.Count)
            {
                var brush = AnsiPalette.Colors16[index];
                return foreground
                    ? state with { Foreground = brush }
                    : state with { Background = brush };
            }

            // 256-colour cube / grayscale fallback: leave state unchanged.
            return state;
        }

        if (mode == 2 && i + 4 < parts.Count)
        {
            var r = (byte)global::System.Math.Clamp(parts[i + 2], 0, 255);
            var g = (byte)global::System.Math.Clamp(parts[i + 3], 0, 255);
            var b = (byte)global::System.Math.Clamp(parts[i + 4], 0, 255);
            i += 4;

            var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze();
            return foreground
                ? state with { Foreground = brush }
                : state with { Background = brush };
        }

        return state;
    }

    private static IReadOnlyList<int> ParseParams(string paramsCsv)
    {
        if (string.IsNullOrEmpty(paramsCsv))
        {
            // Empty SGR (`<ESC>[m`) is an alias for reset.
            return [0];
        }

        var split = paramsCsv.Split(';');
        var result = new int[split.Length];
        for (var i = 0; i < split.Length; i++)
        {
            result[i] = int.TryParse(split[i], out var v) ? v : 0;
        }

        return result;
    }
}