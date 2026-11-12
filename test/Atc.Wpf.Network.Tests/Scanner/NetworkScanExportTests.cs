namespace Atc.Wpf.Network.Tests.Scanner;

public sealed class NetworkScanExportTests
{
    [Fact]
    public void ToCsv_WritesAHeaderAndOneLinePerHost()
    {
        var host = Host("10.0.0.1", "nas.lan", 22, 80);

        var csv = NetworkScanExport.ToCsv([host]);

        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
        Assert.StartsWith("IP address,", lines[0], StringComparison.Ordinal);
        Assert.StartsWith("10.0.0.1,", lines[1], StringComparison.Ordinal);
        Assert.Contains("nas.lan", lines[1], StringComparison.Ordinal);
    }

    [Fact]
    public void ToCsv_QuotesValuesWithCommasAndQuotes()
    {
        var host = Host("10.0.0.1", "say \"hi\"", 22, 80);

        var csv = NetworkScanExport.ToCsv([host]);

        Assert.Contains("\"say \"\"hi\"\"\"", csv, StringComparison.Ordinal);
        Assert.Contains("\"22,80\"", csv, StringComparison.Ordinal);
    }

    [Fact]
    public void ToJson_WritesTheHostsWithTheirPorts()
    {
        var host = Host("10.0.0.1", "nas.lan", 22, 80);

        var json = NetworkScanExport.ToJson([host]);

        using var document = System.Text.Json.JsonDocument.Parse(json);
        var item = Assert.Single(document.RootElement.EnumerateArray());
        Assert.Equal("10.0.0.1", item.GetProperty("ipAddress").GetString());
        Assert.Equal("nas.lan", item.GetProperty("hostname").GetString());
        Assert.Equal([22, 80], item.GetProperty("openPorts").EnumerateArray().Select(x => x.GetInt32()));
    }

    [StaFact]
    public void GetVisibleEntries_FollowsTheFilters()
    {
        using var sut = new NetworkScannerViewModel();
        sut.Filter.ShowOnlySuccess = false;
        sut.Filter.ShowOnlyWithOpenPorts = true;
        sut.ApplyProgress(Result("10.0.0.1", 80), 2, 1, 50);
        sut.ApplyProgress(Result("10.0.0.2"), 2, 2, 100);

        var visible = sut.GetVisibleEntries();

        Assert.Equal(IPAddress.Parse("10.0.0.1"), Assert.Single(visible).IpAddress);
    }

    private static NetworkHostViewModel Host(
        string ipAddress,
        string hostname,
        params ushort[] ports)
    {
        var result = Result(ipAddress, ports);
        result.Hostname = hostname;
        return new NetworkHostViewModel(result);
    }

    private static IPScanResult Result(
        string ipAddress,
        params ushort[] ports)
    {
        var address = IPAddress.Parse(ipAddress);
        var result = new IPScanResult(address)
        {
            PingStatus = new PingStatusResult(address, IPStatus.Success, pingInMs: 5),
        };

        foreach (var port in ports)
        {
            result.Ports.Add(new IPScanPortResult { Port = port, TransportProtocol = TransportProtocolType.Tcp, CanConnect = true });
        }

        return result;
    }
}