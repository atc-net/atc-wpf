namespace Atc.Wpf.Components.Tests.Viewers;

public sealed class TerminalTermMatcherTests
{
    [Theory]
    [InlineData("error", "error")]
    [InlineData("an error occurred", "error")]
    [InlineData("error: failed", "error")]
    [InlineData("[error] failed", "error")]
    [InlineData("Done.", "Done")]
    [InlineData("build (error)", "error")]
    [InlineData("ERROR: failed", "error")]
    [InlineData("at 100% done", "100%")]
    [InlineData("at 100%, done", "100%")]
    public void ContainsTermAsWord_TermDelimitedByNonWordCharacters_Matches(
        string line,
        string term)
        => Assert.True(TerminalTermMatcher.ContainsTermAsWord(line, term, StringComparison.OrdinalIgnoreCase));

    [Theory]
    [InlineData("errors", "error")]
    [InlineData("my_error", "error")]
    [InlineData("error2", "error")]
    [InlineData("terror", "error")]
    [InlineData("Done", "on")]
    [InlineData("", "error")]
    public void ContainsTermAsWord_TermInsideAWord_DoesNotMatch(
        string line,
        string term)
        => Assert.False(TerminalTermMatcher.ContainsTermAsWord(line, term, StringComparison.OrdinalIgnoreCase));

    [Fact]
    public void ContainsTermAsWord_EmptyTerm_DoesNotMatch()
        => Assert.False(TerminalTermMatcher.ContainsTermAsWord("anything", string.Empty, StringComparison.OrdinalIgnoreCase));

    [Fact]
    public void ContainsTermAsWord_LaterOccurrenceIsAWholeWord_Matches()
        => Assert.True(TerminalTermMatcher.ContainsTermAsWord("errors then error", "error", StringComparison.OrdinalIgnoreCase));

    [Fact]
    public void ContainsTermAsWord_CaseSensitiveComparison_RespectsCase()
        => Assert.False(TerminalTermMatcher.ContainsTermAsWord("ERROR", "error", StringComparison.Ordinal));
}