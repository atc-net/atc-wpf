namespace Atc.Wpf.Hardware.Tests.Services;

public sealed class BluetoothDeviceServiceTests
{
    [Fact]
    public void Added_PopulatesIsPairedAndIsConnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new BluetoothDeviceService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = "Headphones",
            IsEnabled = true,
            IsPaired = true,
        });

        Assert.Single(service.Devices);
        var device = service.Devices[0];
        Assert.True(device.IsPaired);
        Assert.True(device.IsConnected);
        Assert.Equal(DeviceState.Available, device.State);
    }

    [Fact]
    public void Added_WithNullIsPaired_DefaultsToTrue()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new BluetoothDeviceService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = "Headphones",
            IsEnabled = true,
            IsPaired = null,
        });

        Assert.True(service.Devices[0].IsPaired);
    }

    [Fact]
    public void Updated_ExistingDevice_RefreshesIsConnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new BluetoothDeviceService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = "Speaker",
            IsEnabled = false,
            IsPaired = true,
        });

        Assert.False(service.Devices[0].IsConnected);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = "Speaker",
            IsEnabled = true,
            IsPaired = true,
        });

        Assert.True(service.Devices[0].IsConnected);
    }

    [Fact]
    public void Added_AfterEnumerationCompleted_IsJustConnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new BluetoothDeviceService(host);

        host.RaiseEnumerationCompleted();
        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = "New",
            IsEnabled = true,
            IsPaired = true,
        });

        Assert.Equal(DeviceState.JustConnected, service.Devices[0].State);
    }

    [Fact]
    public void Removed_MarksDeviceDisconnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new BluetoothDeviceService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = "Mouse",
            IsEnabled = true,
            IsPaired = true,
        });

        host.RaiseRemoved("bt-1");

        Assert.Equal(DeviceState.Disconnected, service.Devices[0].State);
    }

    [Fact]
    public async Task RefreshAsync_SyncsDevices()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new BluetoothDeviceService(host);

        host.SeedSnapshots.Add(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = "Headset",
            IsEnabled = true,
            IsPaired = true,
        });

        await service.RefreshAsync();

        Assert.Single(service.Devices);
        Assert.Equal(1, host.FindAllCallCount);
    }
}