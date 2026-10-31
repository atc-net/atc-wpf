namespace Atc.Wpf.Components.Tests.Viewers;

public sealed class TerminalSearchMatcherTests
{
    private const string CatastrophicPattern = "(?:a|aa)+$";

    [Fact]
    public void IsMatch_RegexTimesOut_ReturnsFalseAndFlagsThePattern()
    {
        var line = CatastrophicLine();
        Assert.Throws<RegexMatchTimeoutException>(
            () => new Regex(CatastrophicPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TerminalSearchMatcher.MatchTimeout).IsMatch(line));

        var sut = new TerminalSearchMatcher();
        sut.Update(CatastrophicPattern, useRegex: true);

        var exception = Record.Exception(() => sut.IsMatch(line));

        Assert.Null(exception);
        Assert.False(sut.IsMatch(line));
        Assert.True(sut.HasTimedOut);
    }

    [Fact]
    public void IsMatch_AfterATimeout_SkipsThePatternUntilItChanges()
    {
        var sut = new TerminalSearchMatcher();
        sut.Update(CatastrophicPattern, useRegex: true);
        _ = sut.IsMatch(CatastrophicLine());

        var stopwatch = Stopwatch.StartNew();
        for (var i = 0; i < 100; i++)
        {
            Assert.False(sut.IsMatch(CatastrophicLine()));
        }

        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(1));

        Assert.True(sut.Update("a+", useRegex: true));
        Assert.False(sut.HasTimedOut);
        Assert.True(sut.IsMatch("aaa"));
    }

    [Theory]
    [InlineData("", false, "anything", true)]
    [InlineData("ERR", false, "an error", true)]
    [InlineData("err", false, "warning", false)]
    [InlineData("^warn", true, "WARNING: x", true)]
    [InlineData("^warn", true, "a warning", false)]
    [InlineData("[", true, "[", false)]
    public void IsMatch_TextAndRegexPatterns(
        string pattern,
        bool useRegex,
        string line,
        bool expected)
    {
        var sut = new TerminalSearchMatcher();
        sut.Update(pattern, useRegex);

        Assert.Equal(expected, sut.IsMatch(line));
    }

    [Fact]
    public void Update_SamePattern_ReportsNoChange()
    {
        var sut = new TerminalSearchMatcher();

        Assert.True(sut.Update("x", useRegex: false));
        Assert.False(sut.Update("x", useRegex: false));
        Assert.True(sut.Update("x", useRegex: true));
        Assert.True(sut.Update(null, useRegex: true));
        Assert.False(sut.IsActive);
    }

    private static string CatastrophicLine()
        => new string('a', 10_000) + "!";
}