namespace Atc.Wpf.Network.Tests.Scanner;

public sealed class NetworkScannerPortPresetTests
{
    [StaFact]
    public void PortPresets_IncludeIpCameras()
    {
        using var sut = new NetworkScannerViewModel();

        var cameras = Assert.Single(sut.PortPresets, x => x.Key == NetworkPortPreset.IpCamerasKey);

        Assert.Contains((ushort)554, cameras.Ports);
        Assert.Contains((ushort)8000, cameras.Ports);
        Assert.Contains((ushort)2020, cameras.Ports);
        Assert.Contains((ushort)37777, cameras.Ports);
    }

    [StaFact]
    public void PortPresets_IncludeTheWellKnownPortsOfAtcNetwork()
    {
        using var sut = new NetworkScannerViewModel();

        var wellKnown = Assert.Single(sut.PortPresets, x => x.Key == NetworkPortPreset.WellKnownKey);

        Assert.Equal(IPServicePortLists.GetWellKnown().Order(), wellKnown.Ports.Order());
    }

    [StaFact]
    public void SelectedPortPreset_SetsThePortNumbers()
    {
        using var sut = new NetworkScannerViewModel();
        var remote = sut.PortPresets.Single(x => x.Key == NetworkPortPreset.RemoteKey);

        sut.SelectedPortPreset = remote;

        Assert.Equal(remote.Ports, sut.PortsNumbers);
        Assert.Equal(string.Join(", ", remote.Ports), sut.PortsNumbersText);
    }

    [StaFact]
    public void SelectedPortPreset_AppPreset_SetsThePortNumbers()
    {
        using var sut = new NetworkScannerViewModel();
        var preset = new NetworkPortPreset("modbus", "Modbus", [502]);
        sut.PortPresets.Add(preset);

        sut.SelectedPortPreset = preset;

        Assert.Equal([502], sut.PortsNumbers);
    }

    [StaFact]
    public void SelectedPortPresetKey_SelectsThePreset()
    {
        using var sut = new NetworkScannerViewModel();

        sut.SelectedPortPresetKey = NetworkPortPreset.IpCamerasKey;

        Assert.Equal(NetworkPortPreset.IpCamerasKey, sut.SelectedPortPreset?.Key);
        Assert.Contains((ushort)554, sut.PortsNumbers);
    }

    [StaFact]
    public void PortPresetItems_FollowThePresets()
    {
        using var sut = new NetworkScannerViewModel();
        var changed = new List<string?>();
        sut.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        sut.PortPresets.Add(new NetworkPortPreset("modbus", "Modbus", [502]));

        Assert.Equal("Modbus", sut.PortPresetItems["modbus"]);
        Assert.Contains(changed, x => string.Equals(x, nameof(NetworkScannerViewModel.PortPresetItems), StringComparison.Ordinal));
    }
}