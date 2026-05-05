namespace Atc.Wpf.Hardware.Tests.Models;

public sealed class NetworkAdapterInfoTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        var info = new NetworkAdapterInfo(
            adapterId: "{abc-123}",
            name: "Ethernet",
            description: "Realtek PCIe GBE",
            adapterType: System.Net.NetworkInformation.NetworkInterfaceType.Ethernet,
            macAddress: "00:11:22:33:44:55",
            speed: 1_000_000_000L,
            isLoopback: false);

        Assert.Equal("{abc-123}", info.DeviceId);
        Assert.Equal("Ethernet", info.Name);
        Assert.Equal("Realtek PCIe GBE", info.Description);
        Assert.Equal(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet, info.AdapterType);
        Assert.Equal("00:11:22:33:44:55", info.MacAddress);
        Assert.Equal(1_000_000_000L, info.Speed);
        Assert.False(info.IsLoopback);
        Assert.Equal(DeviceState.Unknown, info.State);
    }

    [Theory]
    [InlineData("Ethernet", "", "Ethernet")]
    [InlineData("Ethernet", "Realtek PCIe", "Realtek PCIe")]
    public void FriendlyName_PrefersDescription(
        string name,
        string description,
        string expected)
    {
        var info = new NetworkAdapterInfo(
            adapterId: "id",
            name: name,
            description: description,
            adapterType: System.Net.NetworkInformation.NetworkInterfaceType.Ethernet,
            macAddress: string.Empty,
            speed: null,
            isLoopback: false);

        Assert.Equal(expected, info.FriendlyName);
    }

    [Fact]
    public void OperationalStatus_RaisesPropertyChanged()
    {
        var info = new NetworkAdapterInfo(
            adapterId: "id",
            name: "Wi-Fi",
            description: "Wireless",
            adapterType: System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211,
            macAddress: string.Empty,
            speed: null,
            isLoopback: false);

        var raised = false;
        info.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NetworkAdapterInfo.OperationalStatus))
            {
                raised = true;
            }
        };

        info.OperationalStatus = System.Net.NetworkInformation.OperationalStatus.Up;

        Assert.True(raised);
    }
}