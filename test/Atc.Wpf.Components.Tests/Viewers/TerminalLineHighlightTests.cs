namespace Atc.Wpf.Components.Tests.Viewers;

public sealed class TerminalLineHighlightTests
{
    [StaTheory]
    [InlineData("z*")]
    [InlineData("a*")]
    [InlineData(@"\b")]
    [InlineData(".*?")]
    public void Regex_WithZeroLengthMatches_KeepsTheWholeText(string pattern)
    {
        var tb = new TextBlock();
        TerminalLineHighlight.SetUseRegex(tb, value: true);
        TerminalLineHighlight.SetSearchPattern(tb, pattern);
        TerminalLineHighlight.SetSourceText(tb, "hello world");

        Assert.Equal("hello world", RenderedText(tb));
    }

    [StaFact]
    public void Regex_WithMatches_HighlightsOnlyTheMatches()
    {
        var tb = new TextBlock();
        TerminalLineHighlight.SetUseRegex(tb, value: true);
        TerminalLineHighlight.SetSearchPattern(tb, "o");
        TerminalLineHighlight.SetSourceText(tb, "foo bar");

        Assert.Equal("foo bar", RenderedText(tb));
        Assert.Equal(["o", "o"], HighlightedRuns(tb), StringComparer.Ordinal);
    }

    private static string RenderedText(TextBlock tb)
        => string.Concat(tb.Inlines.OfType<Run>().Select(r => r.Text));

    private static IEnumerable<string> HighlightedRuns(TextBlock tb)
        => tb.Inlines
            .OfType<Run>()
            .Where(r => r.Background is not null)
            .Select(r => r.Text);
}