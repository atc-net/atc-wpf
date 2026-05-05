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

    [Fact]
    public void SupportedFormats_DefaultsToNullAndRaisesPropertyChanged()
    {
        var info = new UsbCameraInfo(
            deviceId: "id",
            friendlyName: "Cam",
            panel: CameraPanel.Front,
            isEnabled: true);

        Assert.Null(info.SupportedFormats);

        var raised = false;
        info.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(UsbCameraInfo.SupportedFormats))
            {
                raised = true;
            }
        };

        info.SupportedFormats = new[]
        {
            new UsbCameraFormat(1920, 1080, 30, "NV12"),
        };

        Assert.True(raised);
        Assert.Single(info.SupportedFormats);
    }
}