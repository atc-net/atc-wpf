namespace Atc.Wpf.Network.Tests.Scanner;

public sealed class NetworkScannerRowActionTests
{
    private static readonly IPAddress Address = IPAddress.Parse("10.0.0.7");

    [Fact]
    public void WebUrls_UseTheWebPortsTheScannerConfirmed()
    {
        var result = new IPScanResult(Address);
        result.Ports.Add(Port(8081, ServiceProtocolType.Http));
        result.Ports.Add(Port(8444, ServiceProtocolType.Https));
        result.Ports.Add(Port(22, ServiceProtocolType.Unknown));

        var sut = new NetworkHostViewModel(result);

        Assert.Equal([new Uri("http://10.0.0.7:8081/"), new Uri("https://10.0.0.7:8444/")], sut.WebUrls);
    }

    [Fact]
    public void WebUrls_FallBackToTheUsualWebPorts()
    {
        var result = new IPScanResult(Address);
        result.Ports.Add(Port(443, ServiceProtocolType.Unknown));
        result.Ports.Add(Port(80, ServiceProtocolType.Unknown));
        result.Ports.Add(Port(554, ServiceProtocolType.Unknown));

        var sut = new NetworkHostViewModel(result);

        Assert.Equal([new Uri("http://10.0.0.7/"), new Uri("https://10.0.0.7/")], sut.WebUrls);
    }

    [StaFact]
    public void OpenInBrowser_OpensTheFirstWebUrl()
    {
        var opened = new List<Uri>();
        using var sut = CreateSutWithSelectedHost(Port(80, ServiceProtocolType.Http));
        sut.OpenUrl = opened.Add;

        Assert.True(sut.OpenInBrowserCommand.CanExecute(null));
        sut.OpenInBrowserCommand.Execute(null);

        Assert.Equal([new Uri("http://10.0.0.7/")], opened);
    }

    [StaFact]
    public void OpenInBrowser_WithoutWebPorts_IsDisabled()
    {
        using var sut = CreateSutWithSelectedHost(Port(22, ServiceProtocolType.Unknown));

        Assert.False(sut.OpenInBrowserCommand.CanExecute(null));
    }

    [StaFact]
    public async Task WakeOnLan_SendsToTheMacAddressOfTheHost()
    {
        var woken = new List<string>();
        using var sut = CreateSutWithSelectedHost();
        sut.SelectedEntry!.MacAddress = "00-11-22-33-44-55";
        sut.WakeOnLanSender = (mac, _) =>
        {
            woken.Add(mac);
            return Task.CompletedTask;
        };

        Assert.True(sut.WakeOnLanCommand.CanExecute(null));
        await sut.WakeOnLanCommand.ExecuteAsync(null);

        Assert.Equal(["00-11-22-33-44-55"], woken);
    }

    [StaFact]
    public void WakeOnLan_WithoutMacAddress_IsDisabled()
    {
        using var sut = CreateSutWithSelectedHost();
        sut.SelectedEntry!.MacAddress = null;

        Assert.False(sut.WakeOnLanCommand.CanExecute(null));
    }

    [StaFact]
    public void ConnectWithVnc_RaisesVncRequestedWithTheOpenVncPort()
    {
        using var sut = CreateSutWithSelectedHost(Port(5901, ServiceProtocolType.Unknown));
        NetworkHostConnectEventArgs? args = null;
        sut.VncRequested += (_, e) => args = e;

        Assert.True(sut.ConnectWithVncCommand.CanExecute(null));
        sut.ConnectWithVncCommand.Execute(null);

        Assert.Same(sut.SelectedEntry, args!.Host);
        Assert.Equal(5901, args.Port);
    }

    [StaFact]
    public void ConnectWithVnc_WithoutVncPort_IsDisabled()
    {
        using var sut = CreateSutWithSelectedHost(Port(80, ServiceProtocolType.Http));

        Assert.False(sut.ConnectWithVncCommand.CanExecute(null));
    }

    [StaFact]
    public async Task RescanHost_ScansTheAddressAgainAndUpdatesTheRow()
    {
        using var sut = CreateSutWithSelectedHost();
        var rescanned = new IPScanResult(Address)
        {
            Hostname = "found-now",
            End = DateTime.Now,
        };
        FakeIpScanner? scanner = null;
        sut.ScannerFactory = config =>
        {
            scanner = new FakeIpScanner(config);
            scanner.ResultsToReport.Add(rescanned);
            return scanner;
        };

        await sut.RescanHostCommand.ExecuteAsync(null);

        Assert.Equal(Address, scanner!.ScannedSingle);
        Assert.Equal("found-now", Assert.Single(sut.Entries).Hostname);
    }

    private static NetworkScannerViewModel CreateSutWithSelectedHost(
        params IPScanPortResult[] ports)
    {
        var sut = new NetworkScannerViewModel();
        sut.Filter.ShowOnlySuccess = false;
        sut.Filter.ShowOnlyWithOpenPorts = false;
        var result = new IPScanResult(Address) { MacAddress = "00-11-22-33-44-55" };
        foreach (var port in ports)
        {
            result.Ports.Add(port);
        }

        sut.ApplyProgress(result, 1, 1, 100);
        sut.SelectedEntry = sut.Entries[0];
        return sut;
    }

    private static IPScanPortResult Port(
        ushort port,
        ServiceProtocolType serviceProtocol)
        => new()
        {
            IPAddress = Address,
            Port = port,
            TransportProtocol = TransportProtocolType.Tcp,
            ServiceProtocol = serviceProtocol,
            CanConnect = true,
        };
}