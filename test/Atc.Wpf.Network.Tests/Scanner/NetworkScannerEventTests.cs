namespace Atc.Wpf.Network.Tests.Scanner;

public sealed class NetworkScannerEventTests
{
    private static readonly IPAddress HostA = IPAddress.Parse("10.0.0.1");
    private static readonly IPAddress HostB = IPAddress.Parse("10.0.0.2");

    [StaFact]
    public async Task ScanAsync_ReturnsWithEveryReportApplied()
    {
        using var sut = CreateSut(Result(HostA, completed: true), Result(HostB, completed: true));

        await sut.ScanAsync(CancellationToken.None);

        Assert.Equal(2, sut.Entries.Count);
    }

    [StaFact]
    public async Task HostFound_IsRaisedOncePerAddress_WithTheScanResult()
    {
        var first = Result(HostA, completed: false);
        var last = Result(HostA, completed: true);
        using var sut = CreateSut(first, last);
        var found = new List<NetworkHostEventArgs>();
        sut.HostFound += (_, e) => found.Add(e);

        await sut.ScanAsync(CancellationToken.None);

        var args = Assert.Single(found);
        Assert.Same(first, args.ScanResult);
        Assert.Same(sut.Entries[0], args.Host);
    }

    [StaFact]
    public async Task HostUpdated_IsRaisedForLaterReports()
    {
        var last = Result(HostA, completed: false);
        using var sut = CreateSut(Result(HostA, completed: false), last);
        var updated = new List<NetworkHostEventArgs>();
        sut.HostUpdated += (_, e) => updated.Add(e);

        await sut.ScanAsync(CancellationToken.None);

        // The later report, and once more when the scan returns (with the discovered services).
        Assert.Equal(2, updated.Count);
        Assert.All(updated, x => Assert.Same(last, x.ScanResult));
    }

    [StaFact]
    public async Task HostCompleted_IsRaisedWhenTheAddressIsDone()
    {
        var done = Result(HostB, completed: true);
        using var sut = CreateSut(Result(HostA, completed: false), done);
        var completed = new List<NetworkHostEventArgs>();
        sut.HostCompleted += (_, e) => completed.Add(e);

        await sut.ScanAsync(CancellationToken.None);

        Assert.Same(done, Assert.Single(completed).ScanResult);
    }

    [StaFact]
    public async Task ScanStartedAndScanCompleted_AreRaisedAroundTheScan()
    {
        using var sut = CreateSut(Result(HostA, completed: true));
        var events = new List<string>();
        NetworkScanCompletedEventArgs? completedArgs = null;
        sut.ScanStarted += (_, _) => events.Add("started");
        sut.HostFound += (_, _) => events.Add("found");
        sut.ScanCompleted += (_, e) =>
        {
            events.Add("completed");
            completedArgs = e;
        };

        await sut.ScanAsync(CancellationToken.None);

        Assert.Equal(["started", "found", "completed"], events);
        Assert.NotNull(completedArgs);
        Assert.False(completedArgs.IsCancelled);
        Assert.Null(completedArgs.ErrorMessage);
        Assert.Single(completedArgs.ScanResults!.CollectedResults);
    }

    [StaFact]
    public async Task ScanCompleted_WhenCancelled_SaysSo()
    {
        using var sut = CreateSut(Result(HostA, completed: true));
        NetworkScanCompletedEventArgs? completedArgs = null;
        sut.ScanCompleted += (_, e) => completedArgs = e;
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await sut.ScanAsync(cts.Token);

        Assert.True(completedArgs!.IsCancelled);
    }

    [StaFact]
    public async Task Host_KeepsTheLatestScanResult()
    {
        var last = Result(HostA, completed: true);
        using var sut = CreateSut(Result(HostA, completed: false), last);

        await sut.ScanAsync(CancellationToken.None);

        Assert.Same(last, sut.Entries[0].ScanResult);
    }

    [StaFact]
    public async Task HostFilter_HidesTheHostsItRejects()
    {
        using var sut = CreateSut(Result(HostA, completed: true), Result(HostB, completed: true));
        sut.HostFilter = host => host.IpAddress.Equals(HostB);

        await sut.ScanAsync(CancellationToken.None);

        var visible = CollectionViewSource.GetDefaultView(sut.Entries).Cast<NetworkHostViewModel>();
        Assert.Equal(HostB, Assert.Single(visible).IpAddress);
    }

    [StaFact]
    public async Task ShowOnlyReachable_ShowsHostsFoundOnlyByDiscovery()
    {
        var discovered = Result(HostA, completed: true);
        discovered.NetBiosStatus = new NetBiosNodeStatus(HostA, [], macAddress: null);
        using var sut = CreateSut(discovered, Result(HostB, completed: true));
        sut.Filter.ShowOnlyReachable = true;

        await sut.ScanAsync(CancellationToken.None);

        var visible = CollectionViewSource.GetDefaultView(sut.Entries).Cast<NetworkHostViewModel>();
        Assert.Equal(HostA, Assert.Single(visible).IpAddress);
    }

    [StaFact]
    public async Task ScanAsync_AppliesDiscoveryAttachedAfterTheLastReport()
    {
        var result = Result(HostA, completed: true);
        using var sut = CreateSut(result);
        sut.ScannerFactory = config =>
        {
            var scanner = new FakeIpScanner(config) { AfterLastReport = () => AttachCameraAfterReportsAreApplied(result) };
            scanner.ResultsToReport.Add(result);
            return scanner;
        };

        await sut.ScanAsync(CancellationToken.None);

        var host = Assert.Single(sut.Entries);
        Assert.Equal("C210", host.DeviceName);
        Assert.True(host.IsOnvifCamera);
    }

    [StaFact]
    public async Task HostFilter_OnDiscoveryData_ShowsHostsDiscoveredAfterTheLastReport()
    {
        var result = Result(HostA, completed: true);
        using var sut = CreateSut(result);
        sut.ScannerFactory = config =>
        {
            var scanner = new FakeIpScanner(config) { AfterLastReport = () => AttachCameraAfterReportsAreApplied(result) };
            scanner.ResultsToReport.Add(result);
            return scanner;
        };
        sut.HostFilter = host => host.IsOnvifDevice;

        await sut.ScanAsync(CancellationToken.None);

        var visible = CollectionViewSource.GetDefaultView(sut.Entries).Cast<NetworkHostViewModel>();
        Assert.Single(visible);
    }

    [Fact]
    public void Update_WithTheSameResultInstance_RaisesScanResultChanged()
    {
        var result = Result(HostA, completed: false);
        var host = new NetworkHostViewModel(result);
        var changed = new List<string?>();
        host.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        host.Update(result);

        Assert.Contains(changed, x => string.Equals(x, nameof(NetworkHostViewModel.ScanResult), StringComparison.Ordinal));
    }

    /// <summary>
    /// As in a real scan: the UI has applied every progress report before IPScanner attaches the discovered services.
    /// </summary>
    private static void AttachCameraAfterReportsAreApplied(IPScanResult result)
    {
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
        result.OnvifDevices = [Camera(result.IPAddress)];
    }

    private static OnvifDevice Camera(IPAddress address)
        => new(
            address,
            "urn:uuid:1",
            ["dn:NetworkVideoTransmitter"],
            ["onvif://www.onvif.org/name/C210", "onvif://www.onvif.org/hardware/C210"],
            [new Uri($"http://{address}/onvif/device_service")]);

    private static NetworkScannerViewModel CreateSut(
        params IPScanResult[] results)
    {
        var sut = new NetworkScannerViewModel
        {
            StartIpAddress = "10.0.0.1",
            EndIpAddress = "10.0.0.2",
        };
        sut.Filter.ShowOnlySuccess = false;
        sut.Filter.ShowOnlyWithOpenPorts = false;
        sut.ScannerFactory = config =>
        {
            var scanner = new FakeIpScanner(config);
            foreach (var result in results)
            {
                scanner.ResultsToReport.Add(result);
            }

            return scanner;
        };

        return sut;
    }

    private static IPScanResult Result(
        IPAddress ipAddress,
        bool completed)
        => new(ipAddress)
        {
            PingStatus = new PingStatusResult(ipAddress, IPStatus.TimedOut, pingInMs: 0),
            End = completed ? DateTime.Now : null,
        };
}