namespace Atc.Wpf.Hardware.Tests.Models;

public sealed class BluetoothDeviceInfoTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        var info = new BluetoothDeviceInfo(
            deviceId: "Bluetooth#Bluetooth00:11:22:33:44:55-66:77:88:99:aa:bb",
            friendlyName: "Headphones",
            isPaired: true,
            isConnected: false);

        Assert.Equal("Bluetooth#Bluetooth00:11:22:33:44:55-66:77:88:99:aa:bb", info.DeviceId);
        Assert.Equal("Headphones", info.FriendlyName);
        Assert.True(info.IsPaired);
        Assert.False(info.IsConnected);
        Assert.Equal(DeviceState.Unknown, info.State);
    }

    [Theory]
    [InlineData(false, "Headphones", "Headphones")]
    [InlineData(true, "Headphones", "Headphones ●")]
    public void ToString_AppendsDotForConnected(
        bool isConnected,
        string name,
        string expected)
    {
        var info = new BluetoothDeviceInfo(
            deviceId: "id",
            friendlyName: name,
            isPaired: true,
            isConnected: isConnected);

        Assert.Equal(expected, info.ToString());
    }

    [Fact]
    public void IsConnected_RaisesPropertyChanged()
    {
        var info = new BluetoothDeviceInfo(
            deviceId: "id",
            friendlyName: "Speaker",
            isPaired: true,
            isConnected: false);

        var raised = false;
        info.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(BluetoothDeviceInfo.IsConnected))
            {
                raised = true;
            }
        };

        info.IsConnected = true;

        Assert.True(raised);
    }

    [Fact]
    public void State_RaisesPropertyChanged()
    {
        var info = new BluetoothDeviceInfo(
            deviceId: "id",
            friendlyName: "Mouse",
            isPaired: true,
            isConnected: false);

        var raised = false;
        info.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(BluetoothDeviceInfo.State))
            {
                raised = true;
            }
        };

        info.State = DeviceState.Available;

        Assert.True(raised);
    }
}