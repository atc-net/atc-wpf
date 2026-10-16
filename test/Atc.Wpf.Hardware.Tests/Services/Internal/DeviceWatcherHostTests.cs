namespace Atc.Wpf.Hardware.Tests.Services.Internal;

public sealed class DeviceWatcherHostTests
{
    private const string IsConnectedKey = "System.Devices.Aep.IsConnected";
    private const string PortNameKey = "System.DeviceInterface.Serial.PortName";

    [Fact]
    public void ToUpdatedSnapshot_IsConnectedPresent_IsMapped()
    {
        var properties = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [IsConnectedKey] = true,
        };

        var snapshot = DeviceWatcherHost.ToUpdatedSnapshot("bt-1", properties);

        Assert.Equal("bt-1", snapshot.Id);
        Assert.Equal(string.Empty, snapshot.Name);
        Assert.True(snapshot.IsConnected);
    }

    [Fact]
    public void ToUpdatedSnapshot_IsConnectedAbsent_IsNull()
    {
        var properties = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            ["System.ItemNameDisplay"] = "Headphones",
        };

        var snapshot = DeviceWatcherHost.ToUpdatedSnapshot("bt-1", properties);

        Assert.Null(snapshot.IsConnected);
    }

    [Fact]
    public void ToUpdatedSnapshot_IsConnectedNotABoolean_IsNull()
    {
        var properties = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [IsConnectedKey] = "true",
        };

        var snapshot = DeviceWatcherHost.ToUpdatedSnapshot("bt-1", properties);

        Assert.Null(snapshot.IsConnected);
    }

    [Fact]
    public void ToUpdatedSnapshot_PortNamePresent_IsMapped()
    {
        var properties = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [PortNameKey] = "COM7",
        };

        var snapshot = DeviceWatcherHost.ToUpdatedSnapshot("usb-1", properties);

        Assert.Equal("COM7", snapshot.PortName);
        Assert.Null(snapshot.IsConnected);
    }
}