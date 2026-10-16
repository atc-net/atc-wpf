namespace Atc.Wpf.Components.Tests.Viewers;

public sealed class TerminalLineTrimmerTests
{
    [Fact]
    public void TrimToCap_AboveTheCap_RemovesTheOldestLinesFirst()
    {
        var items = Lines(5);

        TerminalLineTrimmer.TrimToCap(items, maxLines: 3);

        Assert.Equal(["3", "4", "5"], items.Cast<TerminalLineItem>().Select(l => l.Text), StringComparer.Ordinal);
    }

    [Fact]
    public void TrimToCap_PinnedLines_SurviveTheTrim()
    {
        var items = Lines(5);
        items[0] = ((TerminalLineItem)items[0]!) with { IsPinned = true };

        TerminalLineTrimmer.TrimToCap(items, maxLines: 3);

        Assert.Equal(["1", "4", "5"], items.Cast<TerminalLineItem>().Select(l => l.Text), StringComparer.Ordinal);
    }

    [Fact]
    public void TrimToCap_OnlyPinnedLinesLeft_StopsAboveTheCap()
    {
        var items = new List<object>
        {
            new TerminalLineItem("a", Brushes.White) { IsPinned = true },
            new TerminalLineItem("b", Brushes.White) { IsPinned = true },
            new TerminalLineItem("c", Brushes.White),
        };

        TerminalLineTrimmer.TrimToCap(items, maxLines: 1);

        Assert.Equal(["a", "b"], items.Cast<TerminalLineItem>().Select(l => l.Text), StringComparer.Ordinal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void TrimToCap_NoCap_LeavesTheListAlone(int maxLines)
    {
        var items = Lines(5);

        TerminalLineTrimmer.TrimToCap(items, maxLines);

        Assert.Equal(5, items.Count);
    }

    private static List<object> Lines(int count)
        => Enumerable
            .Range(1, count)
            .Select(i => (object)new TerminalLineItem(i.ToString(CultureInfo.InvariantCulture), Brushes.White))
            .ToList();
}