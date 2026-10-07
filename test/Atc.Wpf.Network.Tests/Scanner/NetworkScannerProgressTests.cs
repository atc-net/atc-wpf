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

    private static IPScanResult Result(
        IPAddress ipAddress,
        IPStatus status)
        => new(ipAddress)
        {
            PingStatus = new PingStatusResult(ipAddress, status, pingInMs: 5),
        };
}