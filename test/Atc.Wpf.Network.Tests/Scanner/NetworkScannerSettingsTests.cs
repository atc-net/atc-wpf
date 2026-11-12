namespace Atc.Wpf.Network.Tests.Scanner;

public sealed class NetworkScannerSettingsTests
{
    [StaFact]
    public void GetSettings_ThenApplySettings_RestoresTheScanner()
    {
        using var source = new NetworkScannerViewModel
        {
            StartIpAddress = "10.0.0.1",
            EndIpAddress = "10.0.0.50",
            PortsNumbers = [80, 554],
        };
        source.Options.ResolveHostName = false;
        source.Options.DiscoverOnvif = true;
        source.Options.TimeoutPingInMs = 321;
        source.Filter.ShowOnlySuccess = false;
        source.Filter.ShowOnlyReachable = true;
        source.Columns.ShowMacVendor = false;
        source.Columns.ShowWorkgroup = true;

        var json = System.Text.Json.JsonSerializer.Serialize(source.GetSettings());
        var settings = System.Text.Json.JsonSerializer.Deserialize<NetworkScannerSettings>(json)!;
        using var sut = new NetworkScannerViewModel();
        sut.ApplySettings(settings);

        Assert.Equal("10.0.0.1", sut.StartIpAddress);
        Assert.Equal("10.0.0.50", sut.EndIpAddress);
        Assert.Equal([80, 554], sut.PortsNumbers);
        Assert.False(sut.Options.ResolveHostName);
        Assert.True(sut.Options.DiscoverOnvif);
        Assert.Equal(321, sut.Options.TimeoutPingInMs);
        Assert.False(sut.Filter.ShowOnlySuccess);
        Assert.True(sut.Filter.ShowOnlyReachable);
        Assert.False(sut.Columns.ShowMacVendor);
        Assert.True(sut.Columns.ShowWorkgroup);
    }

    [StaFact]
    public void ApplySettings_FromDefaultSettings_KeepsTheDefaults()
    {
        using var sut = new NetworkScannerViewModel();

        sut.ApplySettings(new NetworkScannerSettings());

        Assert.True(sut.Options.IcmpPing);
        Assert.True(sut.Options.ResolveHostName);
        Assert.False(sut.Options.DiscoverServices);
        Assert.True(sut.Filter.ShowOnlySuccess);
        Assert.True(sut.Columns.ShowDeviceName);
        Assert.False(sut.Columns.ShowWorkgroup);
    }
}