namespace Atc.Wpf.Hardware.Tests.Services;

public sealed class UsbCameraServiceTests
{
    [WpfFact]
    public async Task TwoPickersOnOneService_UnloadingOne_KeepsTheServiceWatching()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbCameraService(host);
        var first = new UsbCameraPicker(service);
        var second = new UsbCameraPicker(service);

        first.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        second.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        await Dispatcher.Yield();
        first.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));

        Assert.True(host.IsStarted);

        second.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));

        Assert.False(host.IsStarted);
    }

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

    [Fact]
    public void Updated_RenamedCamera_UpdatesTheNameInPlace()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbCameraService(host);
        host.RaiseAdded(new DeviceSnapshot { Id = "cam-1", Name = "HD Webcam", IsEnabled = true, Panel = CameraPanel.Front });
        var camera = service.Cameras[0];
        var changed = new List<string>();
        camera.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        host.RaiseUpdated(DeviceWatcherHost.ToUpdatedSnapshot(
            "cam-1",
            new Dictionary<string, object>(StringComparer.Ordinal) { ["System.ItemNameDisplay"] = "Desk camera" }));

        Assert.Same(camera, Assert.Single(service.Cameras));
        Assert.Equal("Desk camera", camera.FriendlyName);
        Assert.Equal("Desk camera (Front)", camera.DisplayName);
        Assert.True(camera.IsEnabled);
        Assert.Equal(DeviceState.Available, camera.State);
        Assert.Contains(nameof(UsbCameraInfo.FriendlyName), changed, StringComparer.Ordinal);
        Assert.Contains(nameof(UsbCameraInfo.DisplayName), changed, StringComparer.Ordinal);
    }

    [Fact]
    public void Updated_CameraDisabledAndEnabled_MovesItsState()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbCameraService(host);
        host.RaiseAdded(new DeviceSnapshot { Id = "cam-1", Name = "HD Webcam", IsEnabled = true });
        var camera = service.Cameras[0];

        host.RaiseUpdated(DeviceWatcherHost.ToUpdatedSnapshot(
            "cam-1",
            new Dictionary<string, object>(StringComparer.Ordinal) { ["System.Devices.InterfaceEnabled"] = false }));

        Assert.False(camera.IsEnabled);
        Assert.Equal(DeviceState.InUse, camera.State);
        Assert.Equal("HD Webcam", camera.FriendlyName);

        host.RaiseUpdated(DeviceWatcherHost.ToUpdatedSnapshot(
            "cam-1",
            new Dictionary<string, object>(StringComparer.Ordinal) { ["System.Devices.InterfaceEnabled"] = true }));

        Assert.True(camera.IsEnabled);
        Assert.Equal(DeviceState.Available, camera.State);
    }

    [Fact]
    public void Updated_WithoutNameOrEnabledChange_LeavesTheCameraAlone()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbCameraService(host);
        host.RaiseAdded(new DeviceSnapshot { Id = "cam-1", Name = "HD Webcam", IsEnabled = true });

        host.RaiseUpdated(DeviceWatcherHost.ToUpdatedSnapshot("cam-1", properties: null));

        var camera = Assert.Single(service.Cameras);
        Assert.Equal("HD Webcam", camera.FriendlyName);
        Assert.True(camera.IsEnabled);
        Assert.Equal(DeviceState.Available, camera.State);
    }
}