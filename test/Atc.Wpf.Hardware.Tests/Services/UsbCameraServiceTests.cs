namespace Atc.Wpf.Hardware.Tests.Services;

public sealed class UsbCameraServiceTests
{
    [Fact]
    public void Added_PopulatesPanelAndIsEnabled()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbCameraService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "cam-1",
            Name = "HD Webcam",
            IsEnabled = true,
            Panel = CameraPanel.Front,
        });

        Assert.Single(service.Cameras);
        var camera = service.Cameras[0];
        Assert.Equal("cam-1", camera.DeviceId);
        Assert.Equal(CameraPanel.Front, camera.Panel);
        Assert.True(camera.IsEnabled);
        Assert.Equal(DeviceState.Available, camera.State);
    }

    [Fact]
    public void Added_PanelDefaultsToUnknownWhenNotProvided()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbCameraService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "cam-1",
            Name = "External camera",
            IsEnabled = true,
            Panel = null,
        });

        Assert.Equal(CameraPanel.Unknown, service.Cameras[0].Panel);
    }

    [Fact]
    public void Added_DisabledInterface_MapsToInUse()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbCameraService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "cam-1",
            Name = "Camera",
            IsEnabled = false,
        });

        Assert.Equal(DeviceState.InUse, service.Cameras[0].State);
    }

    [Fact]
    public void Added_AfterEnumerationCompleted_IsJustConnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbCameraService(host);

        host.RaiseEnumerationCompleted();
        host.RaiseAdded(new DeviceSnapshot { Id = "cam-1", Name = "New", IsEnabled = true });

        Assert.Equal(DeviceState.JustConnected, service.Cameras[0].State);
    }

    [Fact]
    public void Removed_MarksCameraDisconnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbCameraService(host);

        host.RaiseAdded(new DeviceSnapshot { Id = "cam-1", Name = "Camera", IsEnabled = true });
        host.RaiseRemoved("cam-1");

        Assert.Equal(DeviceState.Disconnected, service.Cameras[0].State);
    }

    [Fact]
    public async Task RefreshAsync_QueriesHostAndSyncsCameras()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbCameraService(host);

        host.SeedSnapshots.Add(new DeviceSnapshot
        {
            Id = "cam-1",
            Name = "From refresh",
            IsEnabled = true,
            Panel = CameraPanel.Back,
        });

        await service.RefreshAsync();

        Assert.Single(service.Cameras);
        Assert.Equal(CameraPanel.Back, service.Cameras[0].Panel);
        Assert.Equal(1, host.FindAllCallCount);
    }
}