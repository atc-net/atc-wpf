// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// Attached properties that turn a <see cref="TextBlock"/> into a search-aware
/// renderer with optional timestamp / line-number / pin prefixes. The block's
/// <see cref="TextBlock.Inlines"/> are rebuilt whenever any input changes.
/// </summary>
[AttachedProperty<string>("SourceText", PropertyChangedCallback = nameof(OnAnyChanged), GenerateClrAccessors = false)]
[AttachedProperty<string>("SearchPattern", PropertyChangedCallback = nameof(OnAnyChanged), GenerateClrAccessors = false)]
[AttachedProperty<bool>("UseRegex", DefaultValue = false, PropertyChangedCallback = nameof(OnAnyChanged), GenerateClrAccessors = false)]
[AttachedProperty<Brush>("HighlightBackground", DefaultValueExpression = "System.Windows.Media.Brushes.Gold", PropertyChangedCallback = nameof(OnAnyChanged), GenerateClrAccessors = false)]
[AttachedProperty<bool>("ShowTimestamp", DefaultValue = false, PropertyChangedCallback = nameof(OnAnyChanged), GenerateClrAccessors = false)]
[AttachedProperty<DateTimeOffset>("Timestamp", PropertyChangedCallback = nameof(OnAnyChanged), GenerateClrAccessors = false)]
[AttachedProperty<bool>("ShowLineNumber", DefaultValue = false, PropertyChangedCallback = nameof(OnAnyChanged), GenerateClrAccessors = false)]
[AttachedProperty<int>("LineNumber", DefaultValue = 0, PropertyChangedCallback = nameof(OnAnyChanged), GenerateClrAccessors = false)]
[AttachedProperty<bool>("IsPinned", DefaultValue = false, PropertyChangedCallback = nameof(OnAnyChanged), GenerateClrAccessors = false)]
[AttachedProperty<Brush>("MutedBrush", DefaultValueExpression = "System.Windows.Media.Brushes.Gray", PropertyChangedCallback = nameof(OnAnyChanged), GenerateClrAccessors = false)]
[AttachedProperty<IReadOnlyList<TerminalRun>>("Runs", PropertyChangedCallback = nameof(OnAnyChanged), GenerateClrAccessors = false)]
public static partial class TerminalLineHighlight
{
    // Only the ...Property fields are generated. The Get/Set methods stay hand-written: their element
    // parameter is named obj and each has its own documentation.
    private const string TimestampFormat = "HH:mm:ss.fff";
    private const string PinGlyph = "★ ";

    /// <summary>Gets the line text to render.</summary>
    public static string? GetSourceText(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (string?)obj.GetValue(SourceTextProperty);
    }

    /// <summary>Sets the line text to render.</summary>
    public static void SetSourceText(
        DependencyObject obj,
        string? value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(SourceTextProperty, value);
    }

    /// <summary>Gets the text or regular expression whose matches are highlighted.</summary>
    public static string? GetSearchPattern(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (string?)obj.GetValue(SearchPatternProperty);
    }

    /// <summary>Sets the text or regular expression whose matches are highlighted.</summary>
    public static void SetSearchPattern(
        DependencyObject obj,
        string? value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(SearchPatternProperty, value);
    }

    /// <summary>Gets a value indicating whether the search pattern is a regular expression.</summary>
    public static bool GetUseRegex(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (bool)obj.GetValue(UseRegexProperty);
    }

    /// <summary>Sets a value indicating whether the search pattern is a regular expression.</summary>
    public static void SetUseRegex(
        DependencyObject obj,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(UseRegexProperty, value);
    }

    /// <summary>Gets the background brush of search matches.</summary>
    public static Brush GetHighlightBackground(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (Brush)obj.GetValue(HighlightBackgroundProperty);
    }

    /// <summary>Sets the background brush of search matches.</summary>
    public static void SetHighlightBackground(
        DependencyObject obj,
        Brush value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(HighlightBackgroundProperty, value);
    }

    /// <summary>Gets a value indicating whether the timestamp prefix is shown.</summary>
    public static bool GetShowTimestamp(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (bool)obj.GetValue(ShowTimestampProperty);
    }

    /// <summary>Sets a value indicating whether the timestamp prefix is shown.</summary>
    public static void SetShowTimestamp(
        DependencyObject obj,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(ShowTimestampProperty, value);
    }

    /// <summary>Gets the time shown in the timestamp prefix.</summary>
    public static DateTimeOffset GetTimestamp(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (DateTimeOffset)obj.GetValue(TimestampProperty);
    }

    /// <summary>Sets the time shown in the timestamp prefix.</summary>
    public static void SetTimestamp(
        DependencyObject obj,
        DateTimeOffset value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(TimestampProperty, value);
    }

    /// <summary>Gets a value indicating whether the line-number prefix is shown.</summary>
    public static bool GetShowLineNumber(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (bool)obj.GetValue(ShowLineNumberProperty);
    }

    /// <summary>Sets a value indicating whether the line-number prefix is shown.</summary>
    public static void SetShowLineNumber(
        DependencyObject obj,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(ShowLineNumberProperty, value);
    }

    /// <summary>Gets the number shown in the line-number prefix.</summary>
    public static int GetLineNumber(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (int)obj.GetValue(LineNumberProperty);
    }

    /// <summary>Sets the number shown in the line-number prefix.</summary>
    public static void SetLineNumber(
        DependencyObject obj,
        int value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(LineNumberProperty, value);
    }

    /// <summary>Gets a value indicating whether a pin glyph prefix is shown.</summary>
    public static bool GetIsPinned(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (bool)obj.GetValue(IsPinnedProperty);
    }

    /// <summary>Sets a value indicating whether a pin glyph prefix is shown.</summary>
    public static void SetIsPinned(
        DependencyObject obj,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(IsPinnedProperty, value);
    }

    /// <summary>Gets the foreground brush of the timestamp and line-number prefixes.</summary>
    public static Brush GetMutedBrush(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (Brush)obj.GetValue(MutedBrushProperty);
    }

    /// <summary>Sets the foreground brush of the timestamp and line-number prefixes.</summary>
    public static void SetMutedBrush(
        DependencyObject obj,
        Brush value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(MutedBrushProperty, value);
    }

    /// <summary>Gets the optional pre-parsed ANSI styled runs rendered instead of the source text.</summary>
    public static IReadOnlyList<TerminalRun>? GetRuns(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (IReadOnlyList<TerminalRun>?)obj.GetValue(RunsProperty);
    }

    /// <summary>Sets the optional pre-parsed ANSI styled runs rendered instead of the source text.</summary>
    public static void SetRuns(
        DependencyObject obj,
        IReadOnlyList<TerminalRun>? value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(RunsProperty, value);
    }

    private static void OnAnyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBlock tb)
        {
            Refresh(tb);
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Bad regex must fall back to plain text rather than throw out of a binding callback.")]
    private static void Refresh(TextBlock tb)
    {
        var text = GetSourceText(tb) ?? string.Empty;

        tb.Inlines.Clear();

        AppendPrefixes(tb);

        if (TryAppendAnsiRuns(tb))
        {
            return;
        }

        var pattern = GetSearchPattern(tb);
        if (string.IsNullOrEmpty(pattern))
        {
            if (text.Length > 0)
            {
                tb.Inlines.Add(new Run(text));
            }

            return;
        }

        var brush = GetHighlightBackground(tb);
        var useRegex = GetUseRegex(tb);

        try
        {
            if (useRegex)
            {
                AppendRegex(tb, text, pattern, brush);
            }
            else
            {
                AppendSubstring(tb, text, pattern, brush);
            }
        }
        catch (Exception)
        {
            // Bad regex / unexpected — render plain text body.
            tb.Inlines.Add(new Run(text));
        }
    }

    /// <summary>
    /// ANSI-parsed multi-run rendering takes precedence — the runs already
    /// encode foreground / background / bold / italic / underline. Search
    /// highlight is skipped when runs are present (composing the two would
    /// need a per-run search-and-split pass that's not worth the complexity
    /// for v1 of ANSI support).
    /// </summary>
    private static bool TryAppendAnsiRuns(TextBlock tb)
    {
        var runs = GetRuns(tb);
        if (runs is not { Count: > 0 })
        {
            return false;
        }

        foreach (var r in runs)
        {
            if (r.Text.Length == 0)
            {
                continue;
            }

            var inline = new Run(r.Text);
            if (r.Foreground is not null)
            {
                inline.Foreground = r.Foreground;
            }

            if (r.Background is not null)
            {
                inline.Background = r.Background;
            }

            if (r.Bold)
            {
                inline.FontWeight = System.Windows.FontWeights.Bold;
            }

            if (r.Italic)
            {
                inline.FontStyle = System.Windows.FontStyles.Italic;
            }

            if (r.Underline)
            {
                inline.TextDecorations = System.Windows.TextDecorations.Underline;
            }

            tb.Inlines.Add(inline);
        }

        return true;
    }

    private static void AppendPrefixes(TextBlock tb)
    {
        var muted = GetMutedBrush(tb);

        if (GetIsPinned(tb))
        {
            tb.Inlines.Add(new Run(PinGlyph)
            {
                Foreground = System.Windows.Media.Brushes.Gold,
                FontWeight = System.Windows.FontWeights.Bold,
            });
        }

        if (GetShowTimestamp(tb))
        {
            var ts = GetTimestamp(tb);
            tb.Inlines.Add(new Run(ts.LocalDateTime.ToString(TimestampFormat, GlobalizationConstants.EnglishCultureInfo) + "  ")
            {
                Foreground = muted,
            });
        }

        if (GetShowLineNumber(tb))
        {
            var n = GetLineNumber(tb);
            tb.Inlines.Add(new Run(n.ToString("D5", GlobalizationConstants.EnglishCultureInfo) + "  ")
            {
                Foreground = muted,
            });
        }
    }

    private static void AppendRegex(
        TextBlock tb,
        string text,
        string pattern,
        Brush brush)
    {
        var regex = new Regex(
            pattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled,
            TimeSpan.FromMilliseconds(50));

        var pos = 0;
        foreach (Match match in regex.Matches(text))
        {
            if (match.Index > pos)
            {
                tb.Inlines.Add(new Run(text[pos..match.Index]));
                pos = match.Index;
            }

            if (match.Length == 0)
            {
                // Nothing to highlight, and the text here belongs to the next run or the tail.
                continue;
            }

            tb.Inlines.Add(new Run(match.Value) { Background = brush });
            pos = match.Index + match.Length;
        }

        if (pos < text.Length)
        {
            tb.Inlines.Add(new Run(text[pos..]));
        }
    }

    private static void AppendSubstring(
        TextBlock tb,
        string text,
        string pattern,
        Brush brush)
    {
        var pos = 0;

        while (pos < text.Length)
        {
            var hit = text.IndexOf(pattern, pos, StringComparison.OrdinalIgnoreCase);
            if (hit < 0)
            {
                tb.Inlines.Add(new Run(text[pos..]));
                return;
            }

            if (hit > pos)
            {
                tb.Inlines.Add(new Run(text[pos..hit]));
            }

            tb.Inlines.Add(new Run(text.Substring(hit, pattern.Length)) { Background = brush });
            pos = hit + pattern.Length;
        }
    }
}