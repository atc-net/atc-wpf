namespace Atc.Wpf.Components.Tests.Viewers;

public sealed class TerminalLineBufferTests
{
    [StaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void Add_WhileASearchIsActive_UpdatesTheListWithoutAReset(
        bool hideNonMatching)
    {
        var sut = new TerminalLineBuffer();
        var listView = new ListView { ItemsSource = sut.Lines };
        var actions = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)listView.Items).CollectionChanged += (_, e) => actions.Add(e.Action);

        sut.ApplySearch("error", useRegex: false, hideNonMatching);
        actions.Clear();

        for (var batch = 0; batch < 100; batch++)
        {
            sut.Add([Line($"error {batch}"), Line($"info {batch}")], maxLines: 10_000);
        }

        Assert.DoesNotContain(NotifyCollectionChangedAction.Reset, actions);
        Assert.Equal(100, sut.MatchCount);
        Assert.Equal(200, sut.Lines.Count);
        Assert.Equal(hideNonMatching ? 100 : 200, listView.Items.Count);
    }

    [StaFact]
    public void Add_LargeBatchesAtTheCap_RaiseNoResetAndStayFast()
    {
        var sut = new TerminalLineBuffer();
        var listView = new ListView { ItemsSource = sut.Lines };
        sut.Add(Enumerable.Range(0, 10_000).Select(i => Line($"warmup {i}")).ToList(), maxLines: 10_000);
        sut.ApplySearch("^error", useRegex: true, hideNonMatching: true);
        var actions = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)listView.Items).CollectionChanged += (_, e) => actions.Add(e.Action);
        var stopwatch = Stopwatch.StartNew();

        for (var batch = 0; batch < 50; batch++)
        {
            sut.Add(
                Enumerable.Range(0, 1_000).Select(i => Line(i % 2 == 0 ? $"error {batch}.{i}" : $"info {batch}.{i}")).ToList(),
                maxLines: 10_000);
        }

        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), stopwatch.Elapsed.ToString());
        Assert.DoesNotContain(NotifyCollectionChangedAction.Reset, actions);
        Assert.Equal(10_000, sut.Lines.Count);
        Assert.Equal(5_000, sut.MatchCount);
        Assert.Equal(5_000, listView.Items.Count);
    }

    [StaFact]
    public void Add_HideNonMatching_TrimsHiddenLinesAndKeepsTheMatchCount()
    {
        var sut = new TerminalLineBuffer();
        var listView = new ListView { ItemsSource = sut.Lines };
        sut.ApplySearch("error", useRegex: false, hideNonMatching: true);

        for (var i = 0; i < 200; i++)
        {
            sut.Add([Line(i % 4 == 0 ? $"error {i}" : $"info {i}")], maxLines: 50);
        }

        Assert.Equal(50, sut.Lines.Count);
        Assert.Equal("info 199", sut.Lines[^1].Text);
        Assert.Equal(sut.Lines.Count(l => l.Text.StartsWith("error", StringComparison.Ordinal)), sut.MatchCount);
        Assert.Equal(sut.MatchCount, listView.Items.Count);
    }

    [StaFact]
    public void Add_MoreLinesThanTheCap_KeepsTheNewestQuickly()
    {
        var sut = new TerminalLineBuffer();
        _ = new ListView { ItemsSource = sut.Lines };
        sut.ApplySearch("7", useRegex: false, hideNonMatching: false);
        var stopwatch = Stopwatch.StartNew();

        for (var batch = 0; batch < 20; batch++)
        {
            sut.Add(
                Enumerable.Range(batch * 10_000, 10_000).Select(i => Line(i.ToString(CultureInfo.InvariantCulture))).ToList(),
                maxLines: 10_000);
        }

        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), stopwatch.Elapsed.ToString());
        Assert.Equal(10_000, sut.Lines.Count);
        Assert.Equal("190000", sut.Lines[0].Text);
        Assert.Equal(sut.Lines.Count(l => l.Text.Contains('7', StringComparison.Ordinal)), sut.MatchCount);
    }

    [StaFact]
    public void Add_PinnedLines_SurviveTheBulkTrim()
    {
        var sut = new TerminalLineBuffer();
        sut.Add([Line("pinned") with { IsPinned = true }], maxLines: 10);

        sut.Add(Enumerable.Range(0, 1_000).Select(i => Line(i.ToString(CultureInfo.InvariantCulture))).ToList(), maxLines: 10);

        Assert.Equal(10, sut.Lines.Count);
        Assert.Equal("pinned", sut.Lines[0].Text);
        Assert.Equal("999", sut.Lines[^1].Text);
    }

    [StaFact]
    public void Add_RegexTimesOut_KeepsAddingLines()
    {
        var sut = new TerminalLineBuffer();
        var listView = new ListView { ItemsSource = sut.Lines };
        sut.ApplySearch("(?:a|aa)+$", useRegex: true, hideNonMatching: true);

        var exception = Record.Exception(() => sut.Add([Line(new string('a', 10_000) + "!")], maxLines: 100));
        sut.Add([Line("normal line")], maxLines: 100);

        Assert.Null(exception);
        Assert.True(sut.Matcher.HasTimedOut);
        Assert.Equal("normal line", sut.Lines[^1].Text);
        Assert.Equal(0, sut.MatchCount);
        Assert.Empty(listView.Items);
    }

    [StaFact]
    public void ApplySearch_SameSearchAgain_DoesNotResetTheList()
    {
        var sut = new TerminalLineBuffer();
        var listView = new ListView { ItemsSource = sut.Lines };
        sut.Add([Line("error"), Line("info")], maxLines: 100);
        sut.ApplySearch("error", useRegex: false, hideNonMatching: true);
        var actions = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)listView.Items).CollectionChanged += (_, e) => actions.Add(e.Action);

        sut.ApplySearch("error", useRegex: false, hideNonMatching: true);

        Assert.Empty(actions);
        Assert.Equal(1, sut.MatchCount);
    }

    [StaFact]
    public void ClearUnpinned_KeepsPinnedLinesAndRecounts()
    {
        var sut = new TerminalLineBuffer();
        sut.ApplySearch("error", useRegex: false, hideNonMatching: false);
        sut.Add([Line("error a") with { IsPinned = true }, Line("error b"), Line("info")], maxLines: 100);

        sut.ClearUnpinned();

        Assert.Equal(["error a"], sut.Lines.Select(l => l.Text), StringComparer.Ordinal);
        Assert.Equal(1, sut.MatchCount);
    }

    private static TerminalLineItem Line(string text)
        => new(text, Brushes.White);
}