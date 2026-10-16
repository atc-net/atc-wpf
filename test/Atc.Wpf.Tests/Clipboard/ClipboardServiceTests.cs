namespace Atc.Wpf.Tests.Clipboard;

[Collection("Clipboard")]
public sealed class ClipboardServiceTests
{
    [Fact]
    public void MaxHistorySize_Negative_Throws()
    {
        using var sut = new ClipboardService(Dispatcher.CurrentDispatcher);

        Assert.Throws<ArgumentOutOfRangeException>(() => sut.MaxHistorySize = -1);
    }

    [Fact]
    public void MaxHistorySize_Zero_IsAllowed()
    {
        using var sut = new ClipboardService(Dispatcher.CurrentDispatcher);

        sut.MaxHistorySize = 0;

        Assert.Equal(0, sut.MaxHistorySize);
    }

    [StaFact]
    public void MaxHistorySize_WhenReduced_TrimsTheHistoryImmediately()
    {
        using var sut = new ClipboardService(Dispatcher.CurrentDispatcher);
        var newest = UniqueText();
        sut.SetText(UniqueText());
        sut.SetText(UniqueText());
        sut.SetText(newest);
        Assert.Equal(3, sut.History.Count);

        sut.MaxHistorySize = 1;

        Assert.Single(sut.History);
        Assert.Equal(newest, sut.History[0].Data);
    }

    [WpfFact]
    public async Task SetText_WhileMonitoring_RecordsTheEntryOnce()
    {
        using var sut = new ClipboardService(Dispatcher.CurrentDispatcher);
        var text = UniqueText();
        var changedCount = 0;
        sut.ClipboardChanged += (_, e) =>
        {
            if (string.Equals(e.Entry.Data as string, text, StringComparison.Ordinal))
            {
                changedCount++;
            }
        };

        sut.StartMonitoring();
        sut.SetText(text);
        await PumpUntilAsync(() => CountEntries(sut, text) >= 1);

        // Give a possible duplicate notification time to arrive before counting.
        await Task.Delay(300);
        sut.StopMonitoring();

        Assert.Equal(1, CountEntries(sut, text));
        Assert.Equal(1, changedCount);
    }

    [WpfFact]
    public async Task SetText_FromAnotherThread_IsMarshalledToTheDispatcher()
    {
        using var sut = new ClipboardService(Dispatcher.CurrentDispatcher);
        var text = UniqueText();

        await Task.Run(() => sut.SetText(text));

        Assert.Equal(text, sut.GetText());
        Assert.Single(sut.History);
    }

    [WpfFact]
    public async Task GetText_FromAnotherThread_IsMarshalledToTheDispatcher()
    {
        using var sut = new ClipboardService(Dispatcher.CurrentDispatcher);
        var text = UniqueText();
        sut.SetText(text);

        var read = await Task.Run(sut.GetText);

        Assert.Equal(text, read);
    }

    private static string UniqueText()
        => $"ClipboardServiceTests-{Guid.NewGuid():N}";

    private static int CountEntries(
        ClipboardService service,
        string text)
        => service.History.Count(e => string.Equals(e.Data as string, text, StringComparison.Ordinal));

    private static async Task PumpUntilAsync(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (!condition() && DateTime.UtcNow < deadline)
        {
            await Task.Delay(25);
        }
    }
}