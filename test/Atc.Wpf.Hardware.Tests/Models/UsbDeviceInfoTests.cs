namespace Atc.Wpf.Hardware.Tests.Models;

public sealed class UsbDeviceInfoTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        var info = new UsbDeviceInfo(
            deviceId: @"\\?\USB#VID_2341&PID_0043#abc",
            friendlyName: "Arduino Uno",
            vendorId: "2341",
            productId: "0043",
            pnpClass: "USB",
            interfaceEnabled: true);

        Assert.Equal(@"\\?\USB#VID_2341&PID_0043#abc", info.DeviceId);
        Assert.Equal("Arduino Uno", info.FriendlyName);
        Assert.Equal("2341", info.VendorId);
        Assert.Equal("0043", info.ProductId);
        Assert.Equal("USB", info.PnpClass);
        Assert.True(info.InterfaceEnabled);
        Assert.Equal(DeviceState.Unknown, info.State);
    }

    [Fact]
    public void State_RaisesPropertyChanged_WhenChanged()
    {
        var info = new UsbDeviceInfo(
            deviceId: "id",
            friendlyName: "Generic",
            vendorId: null,
            productId: null,
            pnpClass: null,
            interfaceEnabled: true);

        var raised = 0;
        info.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(UsbDeviceInfo.State))
            {
                raised++;
            }
        };

        info.State = DeviceState.Available;
        info.State = DeviceState.InUse;

        Assert.Equal(2, raised);
    }

    [Theory]
    [InlineData("Generic Device", null, null, "Generic Device")]
    [InlineData("Arduino", "2341", "0043", "Arduino (VID:2341 PID:0043)")]
    public void ToString_ReturnsExpected(
        string friendly,
        string? vid,
        string? pid,
        string expected)
    {
        var info = new UsbDeviceInfo(
            deviceId: "id",
            friendlyName: friendly,
            vendorId: vid,
            productId: pid,
            pnpClass: null,
            interfaceEnabled: true);

        Assert.Equal(expected, info.ToString());
    }
}