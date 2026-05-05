namespace Atc.Wpf.Hardware.Tests.Models;

public sealed class AudioDeviceInfoTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        var info = new AudioDeviceInfo(
            deviceId: "{0.0.1.00000000}.{abc-123}",
            friendlyName: "Microphone (USB Audio)",
            kind: AudioDeviceKind.Input,
            isEnabled: true,
            isDefault: false);

        Assert.Equal("{0.0.1.00000000}.{abc-123}", info.DeviceId);
        Assert.Equal("Microphone (USB Audio)", info.FriendlyName);
        Assert.Equal(AudioDeviceKind.Input, info.Kind);
        Assert.True(info.IsEnabled);
        Assert.False(info.IsDefault);
        Assert.Equal(DeviceState.Unknown, info.State);
    }

    [Theory]
    [InlineData(false, "Speakers", "Speakers")]
    [InlineData(true, "Speakers", "Speakers ★")]
    public void ToString_AppendsStarForDefault(
        bool isDefault,
        string name,
        string expected)
    {
        var info = new AudioDeviceInfo(
            deviceId: "id",
            friendlyName: name,
            kind: AudioDeviceKind.Output,
            isEnabled: true,
            isDefault: isDefault);

        Assert.Equal(expected, info.ToString());
    }

    [Fact]
    public void State_RaisesPropertyChanged()
    {
        var info = new AudioDeviceInfo(
            deviceId: "id",
            friendlyName: "Mic",
            kind: AudioDeviceKind.Input,
            isEnabled: true,
            isDefault: false);

        var raised = false;
        info.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(AudioDeviceInfo.State))
            {
                raised = true;
            }
        };

        info.State = DeviceState.Available;

        Assert.True(raised);
    }
}