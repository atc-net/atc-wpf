

// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// Attached properties that turn a <see cref="TextBlock"/> into a search-aware
/// renderer with optional timestamp / line-number / pin prefixes. The block's
/// <see cref="TextBlock.Inlines"/> are rebuilt whenever any input changes.
/// </summary>
public static class TerminalLineHighlight
{
    private const string TimestampFormat = "HH:mm:ss.fff";
    private const string PinGlyph = "★ ";

    public static readonly DependencyProperty SourceTextProperty =
        DependencyProperty.RegisterAttached(
            "SourceText",
            typeof(string),
            typeof(TerminalLineHighlight),
            new PropertyMetadata(defaultValue: null, OnAnyChanged));

    public static readonly DependencyProperty SearchPatternProperty =
        DependencyProperty.RegisterAttached(
            "SearchPattern",
            typeof(string),
            typeof(TerminalLineHighlight),
            new PropertyMetadata(defaultValue: null, OnAnyChanged));

    public static readonly DependencyProperty UseRegexProperty =
        DependencyProperty.RegisterAttached(
            "UseRegex",
            typeof(bool),
            typeof(TerminalLineHighlight),
            new PropertyMetadata(defaultValue: false, OnAnyChanged));

    public static readonly DependencyProperty HighlightBackgroundProperty =
        DependencyProperty.RegisterAttached(
            "HighlightBackground",
            typeof(Brush),
            typeof(TerminalLineHighlight),
            new PropertyMetadata(System.Windows.Media.Brushes.Gold, OnAnyChanged));

    public static readonly DependencyProperty ShowTimestampProperty =
        DependencyProperty.RegisterAttached(
            "ShowTimestamp",
            typeof(bool),
            typeof(TerminalLineHighlight),
            new PropertyMetadata(defaultValue: false, OnAnyChanged));

    public static readonly DependencyProperty TimestampProperty =
        DependencyProperty.RegisterAttached(
            "Timestamp",
            typeof(DateTimeOffset),
            typeof(TerminalLineHighlight),
            new PropertyMetadata(default(DateTimeOffset), OnAnyChanged));

    public static readonly DependencyProperty ShowLineNumberProperty =
        DependencyProperty.RegisterAttached(
            "ShowLineNumber",
            typeof(bool),
            typeof(TerminalLineHighlight),
            new PropertyMetadata(defaultValue: false, OnAnyChanged));

    public static readonly DependencyProperty LineNumberProperty =
        DependencyProperty.RegisterAttached(
            "LineNumber",
            typeof(int),
            typeof(TerminalLineHighlight),
            new PropertyMetadata(defaultValue: 0, OnAnyChanged));

    public static readonly DependencyProperty IsPinnedProperty =
        DependencyProperty.RegisterAttached(
            "IsPinned",
            typeof(bool),
            typeof(TerminalLineHighlight),
            new PropertyMetadata(defaultValue: false, OnAnyChanged));

    public static readonly DependencyProperty MutedBrushProperty =
        DependencyProperty.RegisterAttached(
            "MutedBrush",
            typeof(Brush),
            typeof(TerminalLineHighlight),
            new PropertyMetadata(System.Windows.Media.Brushes.Gray, OnAnyChanged));

    public static readonly DependencyProperty RunsProperty =
        DependencyProperty.RegisterAttached(
            "Runs",
            typeof(IReadOnlyList<TerminalRun>),
            typeof(TerminalLineHighlight),
            new PropertyMetadata(defaultValue: null, OnAnyChanged));

    public static string? GetSourceText(DependencyObject d) => (string?)d.GetValue(SourceTextProperty);
    public static void SetSourceText(DependencyObject d, string? value) => d.SetValue(SourceTextProperty, value);

    public static string? GetSearchPattern(DependencyObject d) => (string?)d.GetValue(SearchPatternProperty);
    public static void SetSearchPattern(DependencyObject d, string? value) => d.SetValue(SearchPatternProperty, value);

    public static bool GetUseRegex(DependencyObject d) => (bool)d.GetValue(UseRegexProperty);
    public static void SetUseRegex(DependencyObject d, bool value) => d.SetValue(UseRegexProperty, value);

    public static Brush GetHighlightBackground(DependencyObject d) => (Brush)d.GetValue(HighlightBackgroundProperty);
    public static void SetHighlightBackground(DependencyObject d, Brush value) => d.SetValue(HighlightBackgroundProperty, value);

    public static bool GetShowTimestamp(DependencyObject d) => (bool)d.GetValue(ShowTimestampProperty);
    public static void SetShowTimestamp(DependencyObject d, bool value) => d.SetValue(ShowTimestampProperty, value);

    public static DateTimeOffset GetTimestamp(DependencyObject d) => (DateTimeOffset)d.GetValue(TimestampProperty);
    public static void SetTimestamp(DependencyObject d, DateTimeOffset value) => d.SetValue(TimestampProperty, value);

    public static bool GetShowLineNumber(DependencyObject d) => (bool)d.GetValue(ShowLineNumberProperty);
    public static void SetShowLineNumber(DependencyObject d, bool value) => d.SetValue(ShowLineNumberProperty, value);

    public static int GetLineNumber(DependencyObject d) => (int)d.GetValue(LineNumberProperty);
    public static void SetLineNumber(DependencyObject d, int value) => d.SetValue(LineNumberProperty, value);

    public static bool GetIsPinned(DependencyObject d) => (bool)d.GetValue(IsPinnedProperty);
    public static void SetIsPinned(DependencyObject d, bool value) => d.SetValue(IsPinnedProperty, value);

    public static Brush GetMutedBrush(DependencyObject d) => (Brush)d.GetValue(MutedBrushProperty);
    public static void SetMutedBrush(DependencyObject d, Brush value) => d.SetValue(MutedBrushProperty, value);

    public static IReadOnlyList<TerminalRun>? GetRuns(DependencyObject d)
        => (IReadOnlyList<TerminalRun>?)d.GetValue(RunsProperty);

    public static void SetRuns(DependencyObject d, IReadOnlyList<TerminalRun>? value)
        => d.SetValue(RunsProperty, value);

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

        // ANSI-parsed multi-run rendering takes precedence — the runs already
        // encode foreground / background / bold / italic / underline. Search
        // highlight is skipped when runs are present (composing the two would
        // need a per-run search-and-split pass that's not worth the complexity
        // for v1 of ANSI support).
        var runs = GetRuns(tb);
        if (runs is { Count: > 0 })
        {
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
            // Bad regex / unexpected — render plain text body (prefixes stay as already appended).
            tb.Inlines.Add(new Run(text));
        }
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
            }

            tb.Inlines.Add(new Run(match.Value) { Background = brush });
            pos = match.Index + match.Length;

            if (match.Length == 0)
            {
                pos++;
            }
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
