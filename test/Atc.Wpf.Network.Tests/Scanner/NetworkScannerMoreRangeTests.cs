namespace Atc.Wpf.Network.Tests.Scanner;

public sealed class NetworkScannerMoreRangeTests
{
    [Theory]
    [InlineData("vEthernet (WSL (Hyper-V firewall))", "Hyper-V Virtual Ethernet Adapter", true)]
    [InlineData("VMware Network Adapter VMnet8", "VMware Virtual Ethernet Adapter for VMnet8", true)]
    [InlineData("Ethernet 3", "VirtualBox Host-Only Ethernet Adapter", true)]
    [InlineData("OpenVPN", "TAP-Windows Adapter V9", true)]
    [InlineData("wg0", "WireGuard Tunnel", true)]
    [InlineData("Tailscale", "Tailscale Tunnel", true)]
    [InlineData("ZeroTier One", "ZeroTier Virtual Port", true)]
    [InlineData("Ethernet", "Intel(R) Ethernet Connection (17) I219-LM", false)]
    [InlineData("Wi-Fi", "Intel(R) Wi-Fi 6E AX211 160MHz", false)]
    public void IsVirtualAdapter_RecognisesVirtualAndVpnAdapters(
        string name,
        string description,
        bool expected)
        => Assert.Equal(expected, LocalNetwork.IsVirtualAdapter(name, description));

    [Fact]
    public void LocalNetwork_Virtual_SaysSoInItsName()
    {
        var sut = LocalNetwork.FromAddressAndMask(
            "vEthernet (WSL)",
            IPAddress.Parse("172.20.0.1"),
            IPAddress.Parse("255.255.240.0"),
            isVirtual: true);

        Assert.True(sut.IsVirtual);
        Assert.Equal(
            string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Network.Resources.Resources.VirtualNetworkFormat1, "vEthernet (WSL) - 172.20.0.0/20"),
            sut.ToString());
    }

    [StaFact]
    public void DefaultToFirstLocalNetwork_FillsAnEmptyRangeWithTheFirstPhysicalNetwork()
    {
        using var sut = new NetworkScannerViewModel();
        sut.LocalNetworks.Clear();
        sut.LocalNetworks.Add(LocalNetwork.FromAddressAndMask("vEthernet", IPAddress.Parse("172.20.0.1"), IPAddress.Parse("255.255.240.0"), isVirtual: true));
        sut.LocalNetworks.Add(LocalNetwork.FromAddressAndMask("Ethernet", IPAddress.Parse("192.168.1.23"), IPAddress.Parse("255.255.255.0")));

        sut.DefaultToFirstLocalNetwork = true;

        Assert.Equal("192.168.1.1", sut.StartIpAddress);
        Assert.Equal("192.168.1.254", sut.EndIpAddress);
        Assert.Equal("192.168.1.0/24", sut.SelectedLocalNetworkKey);
    }

    [StaFact]
    public void DefaultToFirstLocalNetwork_KeepsARangeTheAppSet()
    {
        using var sut = new NetworkScannerViewModel
        {
            StartIpAddress = "10.0.0.1",
            EndIpAddress = "10.0.0.9",
        };
        sut.LocalNetworks.Clear();
        sut.LocalNetworks.Add(LocalNetwork.FromAddressAndMask("Ethernet", IPAddress.Parse("192.168.1.23"), IPAddress.Parse("255.255.255.0")));

        sut.DefaultToFirstLocalNetwork = true;

        Assert.Equal("10.0.0.1", sut.StartIpAddress);
    }

    [StaFact]
    public async Task RangeCidr_WithSeveralRanges_ScansThemAll()
    {
        var scanners = new List<FakeIpScanner>();
        using var sut = new NetworkScannerViewModel
        {
            ScannerFactory = config =>
            {
                var scanner = new FakeIpScanner(config);
                scanners.Add(scanner);
                return scanner;
            },
        };

        sut.RangeCidr = "192.168.1.0/30, 10.0.0.0/30";
        await sut.ScanAsync(CancellationToken.None);

        Assert.Equal("192.168.1.0", sut.StartIpAddress);
        Assert.Equal("192.168.1.3", sut.EndIpAddress);
        Assert.Equal(
            [("192.168.1.0", "192.168.1.3"), ("10.0.0.0", "10.0.0.3")],
            scanners.Select(x => (x.ScannedStart!.ToString(), x.ScannedEnd!.ToString())));
        Assert.EndsWith("/ 8", sut.EntryCountInfo, StringComparison.Ordinal);
    }

    [StaFact]
    public async Task RangeCidr_WithSeveralRanges_ThenAnEditedStart_ScansOnlyTheEditedRange()
    {
        var scanners = new List<FakeIpScanner>();
        using var sut = new NetworkScannerViewModel
        {
            ScannerFactory = config =>
            {
                var scanner = new FakeIpScanner(config);
                scanners.Add(scanner);
                return scanner;
            },
        };
        sut.RangeCidr = "192.168.1.0/30, 10.0.0.0/30";

        sut.StartIpAddress = "192.168.1.2";
        await sut.ScanAsync(CancellationToken.None);

        Assert.Equal(IPAddress.Parse("192.168.1.2"), Assert.Single(scanners).ScannedStart);
    }

    [StaFact]
    public void GetRangesToScan_ListsEachRange()
    {
        using var sut = new NetworkScannerViewModel();

        sut.RangeCidr = "192.168.1.0/24; 10.0.0.0/30";

        Assert.Equal(
            ["192.168.1.0-192.168.1.255", "10.0.0.0-10.0.0.3"],
            sut.GetRangesToScan().Select(x => $"{x.Start}-{x.End}"),
            StringComparer.Ordinal);
    }
}