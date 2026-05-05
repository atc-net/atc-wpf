namespace Atc.Wpf.Hardware.Tests.Services;

public sealed class UsbDeviceServiceTests
{
    [Fact]
    public void Added_PopulatesParsedVidPidAndIsEnabled()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbDeviceService(_ => host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = @"\\?\USB#VID_046D&PID_C534#abc",
            Name = "Logitech Unifying Receiver",
            IsEnabled = true,
        });

        Assert.Single(service.Devices);
        var device = service.Devices[0];
        Assert.Equal("046D", device.VendorId);
        Assert.Equal("C534", device.ProductId);
        Assert.True(device.InterfaceEnabled);
        Assert.Equal(DeviceState.Available, device.State);
    }

    [Fact]
    public void Added_DisabledInterface_MapsToInUse()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbDeviceService(_ => host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "id-1",
            Name = "Locked device",
            IsEnabled = false,
        });

        Assert.Equal(DeviceState.InUse, service.Devices[0].State);
    }

    [Fact]
    public void ClassFilterChange_ClearsDevicesAndRecreatesWatcher()
    {
        using var first = new FakeDeviceWatcherHost();
        using var second = new FakeDeviceWatcherHost();
        var queue = new Queue<FakeDeviceWatcherHost>(new[] { first, second });

        using var service = new UsbDeviceService(_ => queue.Dequeue());

        first.RaiseAdded(new DeviceSnapshot { Id = "id-1", Name = "Old" });
        Assert.Single(service.Devices);

        service.ClassFilter = UsbDeviceClassFilter.Hid;

        Assert.Empty(service.Devices);
        Assert.True(first.IsDisposed);
        Assert.False(second.IsDisposed);
    }

    [Fact]
    public void ClassFilterChange_PreservesStartedState()
    {
        using var first = new FakeDeviceWatcherHost();
        using var second = new FakeDeviceWatcherHost();
        var queue = new Queue<FakeDeviceWatcherHost>(new[] { first, second });

        using var service = new UsbDeviceService(_ => queue.Dequeue());

        service.StartWatching();
        Assert.True(first.IsStarted);

        service.ClassFilter = UsbDeviceClassFilter.Audio;

        Assert.True(second.IsStarted);
    }

    [Fact]
    public void Removed_MarksDeviceDisconnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbDeviceService(_ => host);

        host.RaiseAdded(new DeviceSnapshot { Id = "id-1", Name = "Device", IsEnabled = true });
        host.RaiseRemoved("id-1");

        Assert.Equal(DeviceState.Disconnected, service.Devices[0].State);
    }

    [Fact]
    public void Added_AfterEnumerationCompleted_IsJustConnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbDeviceService(_ => host);

        host.RaiseEnumerationCompleted();
        host.RaiseAdded(new DeviceSnapshot { Id = "id-1", Name = "Hot-plugged", IsEnabled = true });

        Assert.Equal(DeviceState.JustConnected, service.Devices[0].State);
    }

    [Fact]
    public async Task RefreshAsync_QueriesHostAndSyncsDevices()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new UsbDeviceService(_ => host);

        host.SeedSnapshots.Add(new DeviceSnapshot
        {
            Id = "id-1",
            Name = "From refresh",
            IsEnabled = true,
        });

        await service.RefreshAsync();

        Assert.Single(service.Devices);
        Assert.Equal(1, host.FindAllCallCount);
    }
}