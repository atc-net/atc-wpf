namespace Atc.Wpf.Network.Tests.Scanner;

public sealed class NetworkScannerRangeTests
{
    [StaTheory]
    [InlineData("192.168.1.0/24", "192.168.1.0", "192.168.1.255")]
    [InlineData("10.1.2.3/16", "10.1.0.0", "10.1.255.255")]
    [InlineData("10.0.0.5/32", "10.0.0.5", "10.0.0.5")]
    public void RangeCidr_SetsTheStartAndEndAddress(
        string cidr,
        string expectedStart,
        string expectedEnd)
    {
        using var sut = new NetworkScannerViewModel();

        sut.RangeCidr = cidr;

        Assert.Equal(expectedStart, sut.StartIpAddress);
        Assert.Equal(expectedEnd, sut.EndIpAddress);
    }

    [StaTheory]
    [InlineData("not a range")]
    [InlineData("10.0.0.0/33")]
    [InlineData("10.0.0.0")]
    [InlineData("::1/64")]
    public void RangeCidr_Invalid_KeepsTheRange(string cidr)
    {
        using var sut = new NetworkScannerViewModel
        {
            StartIpAddress = "10.0.0.1",
            EndIpAddress = "10.0.0.9",
        };

        sut.RangeCidr = cidr;

        Assert.Equal("10.0.0.1", sut.StartIpAddress);
        Assert.Equal("10.0.0.9", sut.EndIpAddress);
    }

    [StaTheory]
    [InlineData("fe80::1", null, true)]
    [InlineData("::1", "", true)]
    [InlineData("::1", "::1", true)]
    [InlineData("::1", "::2", false)]
    [InlineData("10.0.0.1", null, false)]
    public void CanScan_AcceptsASingleIpv6Address(
        string start,
        string? end,
        bool expected)
    {
        using var sut = new NetworkScannerViewModel
        {
            StartIpAddress = start,
            EndIpAddress = end,
        };

        Assert.Equal(expected, sut.ScanCommand.CanExecute(null));
    }

    [StaFact]
    public async Task Scan_SingleIpv6Address_ScansOnlyThatAddress()
    {
        FakeIpScanner? scanner = null;
        using var sut = new NetworkScannerViewModel
        {
            StartIpAddress = "::1",
            ScannerFactory = config => scanner = new FakeIpScanner(config),
        };

        await sut.ScanAsync(CancellationToken.None);

        Assert.Equal(IPAddress.IPv6Loopback, scanner!.ScannedSingle);
        Assert.Null(scanner.ScannedStart);
        Assert.Null(sut.ErrorMessage);
    }

    [Fact]
    public void LocalNetwork_FromAddressAndMask_CoversTheHostsOfTheSubnet()
    {
        var sut = LocalNetwork.FromAddressAndMask(
            "Ethernet",
            IPAddress.Parse("192.168.1.23"),
            IPAddress.Parse("255.255.255.0"));

        Assert.Equal("Ethernet", sut.InterfaceName);
        Assert.Equal("192.168.1.0/24", sut.Cidr);
        Assert.Equal(IPAddress.Parse("192.168.1.1"), sut.StartIpAddress);
        Assert.Equal(IPAddress.Parse("192.168.1.254"), sut.EndIpAddress);
    }

    [Fact]
    public void LocalNetwork_FromAddressAndMask_Slash16()
    {
        var sut = LocalNetwork.FromAddressAndMask(
            "Wi-Fi",
            IPAddress.Parse("172.16.4.9"),
            IPAddress.Parse("255.255.0.0"));

        Assert.Equal("172.16.0.0/16", sut.Cidr);
        Assert.Equal(IPAddress.Parse("172.16.0.1"), sut.StartIpAddress);
        Assert.Equal(IPAddress.Parse("172.16.255.254"), sut.EndIpAddress);
    }

    [Theory]
    [InlineData("192.168.1.5", true)]
    [InlineData("10.0.0.1", true)]
    [InlineData("127.0.0.1", false)]
    [InlineData("169.254.10.20", false)]
    [InlineData("::1", false)]
    public void LocalNetwork_IsScannable_SkipsLoopbackLinkLocalAndIpv6(
        string address,
        bool expected)
        => Assert.Equal(expected, LocalNetwork.IsScannable(IPAddress.Parse(address)));

    [StaFact]
    public void SelectedLocalNetworkKey_SetsTheRange()
    {
        using var sut = new NetworkScannerViewModel();
        sut.LocalNetworks.Clear();
        sut.LocalNetworks.Add(LocalNetwork.FromAddressAndMask("Ethernet", IPAddress.Parse("192.168.7.3"), IPAddress.Parse("255.255.255.0")));

        sut.SelectedLocalNetworkKey = "192.168.7.0/24";

        Assert.Equal("192.168.7.1", sut.StartIpAddress);
        Assert.Equal("192.168.7.254", sut.EndIpAddress);
        Assert.Equal("Ethernet - 192.168.7.0/24", sut.LocalNetworkItems["192.168.7.0/24"]);
    }
}