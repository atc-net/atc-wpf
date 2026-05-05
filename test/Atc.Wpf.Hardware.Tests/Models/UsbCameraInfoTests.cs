namespace Atc.Wpf.Hardware.Tests.Models;

public sealed class UsbCameraInfoTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        var info = new UsbCameraInfo(
            deviceId: @"\\?\USB#VID_046D&PID_C534#abc",
            friendlyName: "Logitech HD Webcam C270",
            panel: CameraPanel.External,
            isEnabled: true);

        Assert.Equal(@"\\?\USB#VID_046D&PID_C534#abc", info.DeviceId);
        Assert.Equal("Logitech HD Webcam C270", info.FriendlyName);
        Assert.Equal(CameraPanel.External, info.Panel);
        Assert.True(info.IsEnabled);
        Assert.Equal(DeviceState.Unknown, info.State);
    }

    [Theory]
    [InlineData(CameraPanel.Unknown, "Webcam", "Webcam")]
    [InlineData(CameraPanel.Front, "Webcam", "Webcam (Front)")]
    [InlineData(CameraPanel.External, "Webcam", "Webcam (External)")]
    public void ToString_ReturnsExpected(
        CameraPanel panel,
        string name,
        string expected)
    {
        var info = new UsbCameraInfo(
            deviceId: "id",
            friendlyName: name,
            panel: panel,
            isEnabled: true);

        Assert.Equal(expected, info.ToString());
    }
}