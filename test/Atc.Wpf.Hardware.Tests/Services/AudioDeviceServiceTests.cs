namespace Atc.Wpf.Hardware.Tests.Services;

public sealed class AudioDeviceServiceTests
{
    [Fact]
    public void Constructor_ExposesKind()
    {
        using var host = new FakeDeviceWatcherHost();
        using var input = new AudioDeviceService(AudioDeviceKind.Input, host);

        Assert.Equal(AudioDeviceKind.Input, input.Kind);
    }

    [Fact]
    public void Added_PopulatesKindAndIsDefault()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new AudioDeviceService(AudioDeviceKind.Output, host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "speaker-1",
            Name = "Speakers",
            IsEnabled = true,
            IsDefault = true,
        });

        Assert.Single(service.Devices);
        var device = service.Devices[0];
        Assert.Equal(AudioDeviceKind.Output, device.Kind);
        Assert.True(device.IsDefault);
        Assert.True(device.IsEnabled);
        Assert.Equal(DeviceState.Available, device.State);
    }

    [Fact]
    public void Added_DisabledInterface_MapsToInUse()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new AudioDeviceService(AudioDeviceKind.Input, host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "mic-1",
            Name = "Mic",
            IsEnabled = false,
        });

        Assert.Equal(DeviceState.InUse, service.Devices[0].State);
    }

    [Fact]
    public void Added_AfterEnumerationCompleted_IsJustConnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new AudioDeviceService(AudioDeviceKind.Output, host);

        host.RaiseEnumerationCompleted();
        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "spk-1",
            Name = "USB Headset",
            IsEnabled = true,
        });

        Assert.Equal(DeviceState.JustConnected, service.Devices[0].State);
    }

    [Fact]
    public void Removed_MarksDeviceDisconnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new AudioDeviceService(AudioDeviceKind.Input, host);

        host.RaiseAdded(new DeviceSnapshot { Id = "mic-1", Name = "Mic", IsEnabled = true });
        host.RaiseRemoved("mic-1");

        Assert.Equal(DeviceState.Disconnected, service.Devices[0].State);
    }

    [Fact]
    public async Task RefreshAsync_SyncsDevices()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new AudioDeviceService(AudioDeviceKind.Output, host);

        host.RaiseAdded(new DeviceSnapshot { Id = "stale", Name = "Stale", IsEnabled = true });

        host.SeedSnapshots.Add(new DeviceSnapshot
        {
            Id = "fresh",
            Name = "Fresh",
            IsEnabled = true,
            IsDefault = true,
        });

        await service.RefreshAsync();

        Assert.Equal(2, service.Devices.Count);
        Assert.Equal(DeviceState.Available, service.Devices.Single(d => d.DeviceId == "fresh").State);
        Assert.Equal(DeviceState.Disconnected, service.Devices.Single(d => d.DeviceId == "stale").State);
    }
}