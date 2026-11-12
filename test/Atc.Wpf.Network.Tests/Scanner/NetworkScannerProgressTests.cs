namespace Atc.Wpf.Network.Tests.Scanner;

public sealed class NetworkScannerProgressTests
{
    private static readonly IPAddress HostA = IPAddress.Parse("10.0.0.1");
    private static readonly IPAddress HostB = IPAddress.Parse("10.0.0.2");

    [StaFact]
    public void ApplyProgress_ForAKnownHost_UpdatesItWithoutResettingTheView()
    {
        using var sut = CreateSutShowingEverything();
        var view = CollectionViewSource.GetDefaultView(sut.Entries);
        sut.ApplyProgress(Result(HostA, IPStatus.TimedOut), 10, 1, 10);
        var resets = 0;
        ((INotifyCollectionChanged)view).CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                resets++;
            }
        };

        for (var i = 0; i < 5; i++)
        {
            sut.ApplyProgress(Result(HostA, IPStatus.Success), 10, 2 + i, 20 + i);
        }

        Assert.Equal(0, resets);
        Assert.Equal(IPStatus.Success, Assert.Single(sut.Entries).PingStatus?.Status);
    }

    [StaFact]
    public void ApplyProgress_RoutesEachUpdateToItsHost()
    {
        using var sut = CreateSutShowingEverything();
        sut.ApplyProgress(Result(HostA, IPStatus.TimedOut), 10, 1, 10);
        sut.ApplyProgress(Result(HostB, IPStatus.TimedOut), 10, 2, 20);

        sut.ApplyProgress(Result(HostB, IPStatus.Success), 10, 3, 30);

        Assert.Equal(2, sut.Entries.Count);
        Assert.Equal(IPStatus.TimedOut, sut.Entries.Single(e => e.IpAddress.Equals(HostA)).PingStatus?.Status);
        Assert.Equal(IPStatus.Success, sut.Entries.Single(e => e.IpAddress.Equals(HostB)).PingStatus?.Status);
        Assert.StartsWith("2 /", sut.EntryCountInfo, StringComparison.Ordinal);
    }

    [StaFact]
    public void ShowOnlySuccess_HostThatStartsAnsweringWhileScanning_BecomesVisible()
    {
        using var sut = new NetworkScannerViewModel();
        var view = CollectionViewSource.GetDefaultView(sut.Entries);
        sut.Filter.ShowOnlySuccess = true;
        sut.Filter.ShowOnlyWithOpenPorts = false;

        sut.ApplyProgress(Result(HostA, IPStatus.TimedOut), 10, 1, 10);
        Assert.Empty(view.Cast<object>());
        Assert.StartsWith("0 /", sut.EntryCountInfo, StringComparison.Ordinal);

        sut.ApplyProgress(Result(HostA, IPStatus.Success), 10, 2, 20);
        DrainDispatcher(); // live filtering re-evaluates on the dispatcher, as in the running UI

        Assert.Single(view.Cast<object>());
        Assert.StartsWith("1 /", sut.EntryCountInfo, StringComparison.Ordinal);
    }

    [StaFact]
    public void Sorted_HostWhoseSortValueChanges_MovesToItsPlace()
    {
        using var sut = CreateSutShowingEverything();
        var view = CollectionViewSource.GetDefaultView(sut.Entries);
        sut.ApplyProgress(Named(HostA, "b-host"), 10, 1, 10);
        sut.ApplyProgress(Named(HostB, "c-host"), 10, 2, 20);
        sut.ApplySort(nameof(NetworkHostViewModel.Hostname), System.ComponentModel.ListSortDirection.Ascending);

        sut.ApplyProgress(Named(HostB, "a-host"), 10, 3, 30);
        DrainDispatcher(); // live sorting re-positions on the dispatcher, as in the running UI

        Assert.Equal(HostB, view.Cast<NetworkHostViewModel>().First().IpAddress);
    }

    [StaFact]
    public void ProgressReports_FromAWorkerThread_AreAppliedInOneDispatcherCallback()
    {
        using var sut = CreateSutShowingEverything();
        var results = Enumerable.Range(0, 1000)
            .Select(i => Result(new IPAddress([10, 1, (byte)(i / 256), (byte)(i % 256)]), IPStatus.TimedOut))
            .ToList();

        // The scanner reports from worker threads while the UI thread is busy.
        Task.Run(() =>
        {
            for (var i = 0; i < results.Count; i++)
            {
                sut.EnqueueProgress(results[i], results.Count, i + 1, (i + 1) / 10.0);
            }
        }).GetAwaiter().GetResult();

        DrainDispatcher();

        Assert.Equal(1000, sut.Entries.Count);
        Assert.Equal(1000, sut.BusyIndicatorCurrentValue);
        Assert.Equal(1, sut.ProgressDispatchCount);
    }

    [StaFact]
    public void ProgressReports_UpdateTheEntryCountOncePerBatch()
    {
        using var sut = CreateSutShowingEverything();
        var countChanges = 0;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NetworkScannerViewModel.EntryCountInfo))
            {
                countChanges++;
            }
        };

        for (var i = 0; i < 200; i++)
        {
            sut.EnqueueProgress(Result(new IPAddress([10, 2, 0, (byte)i]), IPStatus.TimedOut), 200, i + 1, (i + 1) / 2.0);
        }

        DrainDispatcher();

        Assert.Equal(1, countChanges);
        Assert.StartsWith("200 /", sut.EntryCountInfo, StringComparison.Ordinal);
    }

    [StaFact]
    public void ProgressReports_AfterDispose_AreDropped()
    {
        var sut = CreateSutShowingEverything();
        sut.EnqueueProgress(Result(HostA, IPStatus.Success), 10, 1, 10);

        sut.Dispose();
        DrainDispatcher();

        Assert.Empty(sut.Entries);
    }

    [StaFact]
    public async Task Dispose_WhileScanning_CancelsTheScan()
    {
        // TEST-NET-1 (RFC 5737): never routed, so the scan waits on ping time-outs until cancelled.
        var sut = new NetworkScannerViewModel
        {
            StartIpAddress = "192.0.2.1",
            EndIpAddress = "192.0.2.254",
        };

        var scan = sut.ScanCommand.ExecuteAsync(null);
        sut.Dispose();

        await scan.WaitAsync(TimeSpan.FromSeconds(30));

        Assert.False(sut.IsBusy);
    }

    private static void DrainDispatcher()
        => System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.ApplicationIdle,
            new Action(() => { }));

    private static NetworkScannerViewModel CreateSutShowingEverything()
    {
        var sut = new NetworkScannerViewModel();
        sut.Filter.ShowOnlySuccess = false;
        sut.Filter.ShowOnlyWithOpenPorts = false;
        return sut;
    }

    private static IPScanResult Named(
        IPAddress ipAddress,
        string hostname)
        => new(ipAddress)
        {
            PingStatus = new PingStatusResult(ipAddress, IPStatus.Success, pingInMs: 5),
            Hostname = hostname,
        };

    private static IPScanResult Result(
        IPAddress ipAddress,
        IPStatus status)
        => new(ipAddress)
        {
            PingStatus = new PingStatusResult(ipAddress, status, pingInMs: 5),
        };
}