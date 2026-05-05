namespace Atc.Wpf.Hardware.Tests.Services;

public sealed class SerialPortServiceTests
{
    [Fact]
    public void StartWatching_IsIdempotent()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new SerialPortService(host);

        service.StartWatching();
        service.StartWatching();

        Assert.Equal(1, host.StartWatchingCallCount);
    }

    [Fact]
    public void StopWatching_OnlyTriggersHostWhenStarted()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new SerialPortService(host);

        service.StopWatching();
        service.StartWatching();
        service.StopWatching();
        service.StopWatching();

        Assert.Equal(1, host.StopWatchingCallCount);
    }

    [Fact]
    public void Added_BeforeEnumerationCompleted_IsAvailable()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new SerialPortService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = @"\\?\USB#VID_2341&PID_0043#abc",
            Name = "Arduino Uno",
            PortName = "COM3",
        });

        Assert.Single(service.Ports);
        var port = service.Ports[0];
        Assert.Equal(@"\\?\USB#VID_2341&PID_0043#abc", port.DeviceId);
        Assert.Equal("COM3", port.PortName);
        Assert.Equal("2341", port.VendorId);
        Assert.Equal("0043", port.ProductId);
        Assert.Equal(DeviceState.Available, port.State);
    }

    [Fact]
    public void Added_AfterEnumerationCompleted_IsJustConnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new SerialPortService(host);

        host.RaiseEnumerationCompleted();
        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "id-1",
            Name = "Late arrival",
            PortName = "COM7",
        });

        Assert.Equal(DeviceState.JustConnected, service.Ports[0].State);
    }

    [Fact]
    public void Added_FallsBackToNameWhenPortNameMissing()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new SerialPortService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "id-1",
            Name = "Generic Serial",
            PortName = null,
        });

        Assert.Equal("Generic Serial", service.Ports[0].PortName);
    }

    [Fact]
    public void Removed_MarksPortDisconnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new SerialPortService(host);

        host.RaiseAdded(new DeviceSnapshot { Id = "id-1", Name = "Port", PortName = "COM3" });
        host.RaiseRemoved("id-1");

        Assert.Equal(DeviceState.Disconnected, service.Ports[0].State);
    }

    [Fact]
    public void Added_WithExistingDisconnectedPort_FlipsBackToAvailable()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new SerialPortService(host);

        host.RaiseAdded(new DeviceSnapshot { Id = "id-1", Name = "Port", PortName = "COM3" });
        host.RaiseRemoved("id-1");
        host.RaiseAdded(new DeviceSnapshot { Id = "id-1", Name = "Port", PortName = "COM3" });

        Assert.Single(service.Ports);
        Assert.Equal(DeviceState.Available, service.Ports[0].State);
    }

    [Fact]
    public async Task RefreshAsync_AddsSeededDevicesAndDisconnectsMissing()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new SerialPortService(host);

        host.RaiseAdded(new DeviceSnapshot { Id = "stale-id", Name = "Stale", PortName = "COM9" });

        host.SeedSnapshots.Add(new DeviceSnapshot
        {
            Id = "fresh-id",
            Name = "Fresh",
            PortName = "COM4",
        });

        await service.RefreshAsync();

        Assert.Equal(2, service.Ports.Count);
        Assert.Equal(DeviceState.Available, service.Ports.Single(p => p.DeviceId == "fresh-id").State);
        Assert.Equal(DeviceState.Disconnected, service.Ports.Single(p => p.DeviceId == "stale-id").State);
        Assert.Equal(1, host.FindAllCallCount);
    }

    [Fact]
    public void Dispose_DetachesEventsAndDisposesHost()
    {
        using var host = new FakeDeviceWatcherHost();
        var service = new SerialPortService(host);

        service.Dispose();

        host.RaiseAdded(new DeviceSnapshot { Id = "id-1", Name = "Port", PortName = "COM3" });

        Assert.Empty(service.Ports);
        Assert.True(host.IsDisposed);
    }
}