namespace Atc.Wpf.Network.Tests.Scanner;

public sealed class NetworkScannerMoreRowActionTests
{
    private static readonly IPAddress HostA = IPAddress.Parse("10.0.0.1");
    private static readonly IPAddress HostB = IPAddress.Parse("10.0.0.2");

    [StaFact]
    public void Traceroute_RaisesTracerouteRequestedForTheSelectedHost()
    {
        using var sut = CreateSutWithSelectedHost();
        NetworkHostSelectedEventArgs? args = null;
        sut.TracerouteRequested += (_, e) => args = e;

        Assert.True(sut.TracerouteCommand.CanExecute(null));
        sut.TracerouteCommand.Execute(null);

        Assert.Same(sut.SelectedEntry, args!.SelectedHost);
    }

    [StaFact]
    public void RemoteDesktop_IsOffUnlessTheAppAllowsIt()
    {
        using var sut = CreateSutWithSelectedHost(3389);

        Assert.False(sut.ConnectWithRemoteDesktopCommand.CanExecute(null));
    }

    [StaFact]
    public void RemoteDesktop_Allowed_StartsItForAHostWithPort3389()
    {
        var started = new List<IPAddress>();
        using var sut = CreateSutWithSelectedHost(3389);
        sut.AllowRemoteDesktop = true;
        sut.RemoteDesktopLauncher = started.Add;

        Assert.True(sut.ConnectWithRemoteDesktopCommand.CanExecute(null));
        sut.ConnectWithRemoteDesktopCommand.Execute(null);

        Assert.Equal([HostA], started);
    }

    [StaFact]
    public void RemoteDesktop_Allowed_WithoutPort3389_IsDisabled()
    {
        using var sut = CreateSutWithSelectedHost(80);
        sut.AllowRemoteDesktop = true;

        Assert.False(sut.ConnectWithRemoteDesktopCommand.CanExecute(null));
    }

    [StaFact]
    public async Task OfflineHosts_ListsHostsWithAMacAddressThatStoppedAnswering()
    {
        using var sut = new NetworkScannerViewModel { StartIpAddress = "10.0.0.1", EndIpAddress = "10.0.0.2" };
        sut.ScannerFactory = Reporting(Answering(HostA, "00-11-22-33-44-01"), Answering(HostB, "00-11-22-33-44-02"));
        await sut.ScanAsync(CancellationToken.None);
        Assert.Empty(sut.OfflineHosts);

        sut.ScannerFactory = Reporting(Answering(HostA, "00-11-22-33-44-01"), Silent(HostB));
        await sut.ScanAsync(CancellationToken.None);

        var offline = Assert.Single(sut.OfflineHosts);
        Assert.Equal(HostB, offline.IpAddress);
        Assert.Equal("00-11-22-33-44-02", offline.MacAddress);
    }

    [StaFact]
    public async Task OfflineHosts_HostAnsweringAgain_LeavesTheList()
    {
        using var sut = new NetworkScannerViewModel { StartIpAddress = "10.0.0.1", EndIpAddress = "10.0.0.2" };
        sut.ScannerFactory = Reporting(Answering(HostB, "00-11-22-33-44-02"));
        await sut.ScanAsync(CancellationToken.None);
        sut.ScannerFactory = Reporting(Silent(HostB));
        await sut.ScanAsync(CancellationToken.None);

        sut.ScannerFactory = Reporting(Answering(HostB, "00-11-22-33-44-02"));
        await sut.ScanAsync(CancellationToken.None);

        Assert.Empty(sut.OfflineHosts);
    }

    [StaFact]
    public async Task OfflineHosts_OutsideTheScannedRange_AreNotListed()
    {
        using var sut = new NetworkScannerViewModel { StartIpAddress = "10.0.0.1", EndIpAddress = "10.0.0.2" };
        sut.ScannerFactory = Reporting(Answering(HostB, "00-11-22-33-44-02"));
        await sut.ScanAsync(CancellationToken.None);

        sut.StartIpAddress = "10.0.0.1";
        sut.EndIpAddress = "10.0.0.1";
        sut.ScannerFactory = Reporting(Silent(HostA));
        await sut.ScanAsync(CancellationToken.None);

        Assert.Empty(sut.OfflineHosts);
    }

    [StaFact]
    public async Task WakeOfflineHost_SendsToItsMacAddress()
    {
        var woken = new List<string>();
        using var sut = new NetworkScannerViewModel
        {
            WakeOnLanSender = (mac, _) =>
            {
                woken.Add(mac);
                return Task.CompletedTask;
            },
        };
        var host = new NetworkKnownHost(HostB, "00-11-22-33-44-02", "nas", DateTime.Now);

        await sut.WakeOfflineHostCommand.ExecuteAsync(host);

        Assert.Equal(["00-11-22-33-44-02"], woken);
    }

    private static NetworkScannerViewModel CreateSutWithSelectedHost(
        params ushort[] ports)
    {
        var sut = new NetworkScannerViewModel();
        sut.Filter.ShowOnlySuccess = false;
        sut.Filter.ShowOnlyWithOpenPorts = false;
        var result = new IPScanResult(HostA);
        foreach (var port in ports)
        {
            result.Ports.Add(new IPScanPortResult { Port = port, TransportProtocol = TransportProtocolType.Tcp, CanConnect = true });
        }

        sut.ApplyProgress(result, 1, 1, 100);
        sut.SelectedEntry = sut.Entries[0];
        return sut;
    }

    private static IPScanResult Answering(
        IPAddress address,
        string mac)
        => new(address)
        {
            PingStatus = new PingStatusResult(address, IPStatus.Success, pingInMs: 3),
            MacAddress = mac,
            Hostname = "nas",
            End = DateTime.Now,
        };

    private static IPScanResult Silent(IPAddress address)
        => new(address)
        {
            PingStatus = new PingStatusResult(address, IPStatus.TimedOut, pingInMs: 0),
            End = DateTime.Now,
        };

    private static Func<IPScannerConfig, IIPScanner> Reporting(
        params IPScanResult[] results)
        => config =>
        {
            var scanner = new FakeIpScanner(config);
            foreach (var result in results)
            {
                scanner.ResultsToReport.Add(result);
            }

            return scanner;
        };
}