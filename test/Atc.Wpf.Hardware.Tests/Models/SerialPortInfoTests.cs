namespace Atc.Wpf.Hardware.Tests.Models;

public sealed class SerialPortInfoTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        var info = new SerialPortInfo(
            deviceId: @"\\?\USB#VID_2341&PID_0043#abc",
            portName: "COM3",
            friendlyName: "Arduino Uno",
            vendorId: "2341",
            productId: "0043");

        Assert.Equal(@"\\?\USB#VID_2341&PID_0043#abc", info.DeviceId);
        Assert.Equal("COM3", info.PortName);
        Assert.Equal("Arduino Uno", info.FriendlyName);
        Assert.Equal("2341", info.VendorId);
        Assert.Equal("0043", info.ProductId);
        Assert.Equal(DeviceState.Unknown, info.State);
    }

    [Fact]
    public void State_RaisesPropertyChanged_WhenChanged()
    {
        var info = new SerialPortInfo(
            deviceId: "id",
            portName: "COM1",
            friendlyName: "Generic",
            vendorId: null,
            productId: null);

        var raised = 0;
        info.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SerialPortInfo.State))
            {
                raised++;
            }
        };

        info.State = DeviceState.Available;
        info.State = DeviceState.InUse;

        Assert.Equal(2, raised);
    }

    [Theory]
    [InlineData("", "COM3", "COM3")]
    [InlineData("Arduino Uno", "COM3", "COM3 — Arduino Uno")]
    public void ToString_ReturnsExpected(
        string friendly,
        string port,
        string expected)
    {
        var info = new SerialPortInfo(
            deviceId: "id",
            portName: port,
            friendlyName: friendly,
            vendorId: null,
            productId: null);

        Assert.Equal(expected, info.ToString());
    }
}