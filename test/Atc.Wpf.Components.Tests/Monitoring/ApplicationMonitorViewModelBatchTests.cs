namespace Atc.Wpf.Components.Tests.Monitoring;

public sealed class ApplicationMonitorViewModelBatchTests
{
    [StaFact]
    public void Paused_ManyEntries_KeepsAtMostMaxEntriesAndShowsThemOnResume()
    {
        using var sut = new ApplicationMonitorViewModel
        {
            ListenOnToastNotificationMessage = false,
            MaxEntries = 1_000,
            IsPaused = true,
        };

        for (var i = 0; i < 200_000; i++)
        {
            sut.AddEntry(Entry(i));
        }

        Assert.Equal(1_000, sut.BufferedCount);

        var stopwatch = Stopwatch.StartNew();
        sut.IsPaused = false;
        DispatcherPump.Until(() => sut.Entries.Count == 1_000);

        Assert.Equal(1_000, sut.Entries.Count);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(5), stopwatch.Elapsed.ToString());
        Assert.Equal("199999", sut.Entries[^1].Message);
    }

    [StaFact]
    public void Batch_RaisesCanExecuteChangedOncePerCommand()
    {
        using var sut = new ApplicationMonitorViewModel { ListenOnToastNotificationMessage = false, IsPaused = true };
        var copyAllRaised = 0;
        var exportRaised = 0;
        sut.CopyAllCommand.CanExecuteChanged += (_, _) => copyAllRaised++;
        sut.ExportCommand.CanExecuteChanged += (_, _) => exportRaised++;

        for (var i = 0; i < 50; i++)
        {
            sut.AddEntry(Entry(i));
        }

        sut.IsPaused = false;
        DispatcherPump.Until(() => sut.Entries.Count == 50);

        Assert.Equal(50, sut.Entries.Count);
        Assert.Equal(1, copyAllRaised);
        Assert.Equal(1, exportRaised);
    }

    private static ApplicationEventEntry Entry(int i)
        => new(LogCategoryType.Information, "Test", i.ToString(CultureInfo.InvariantCulture));
}