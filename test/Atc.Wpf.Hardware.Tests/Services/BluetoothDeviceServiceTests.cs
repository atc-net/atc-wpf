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

    // A paired device that is switched off is enabled in Windows but not connected.
    [Fact]
    public void Added_MapsIsConnectedFromTheSnapshot()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new BluetoothDeviceService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = "Headphones",
            IsEnabled = true,
            IsConnected = false,
            IsPaired = true,
        });

        Assert.False(service.Devices[0].IsConnected);
    }

    [Fact]
    public void Added_WithoutIsConnected_FallsBackToIsEnabled()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new BluetoothDeviceService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = "Headphones",
            IsEnabled = false,
            IsConnected = null,
            IsPaired = true,
        });

        Assert.False(service.Devices[0].IsConnected);
    }

    [Fact]
    public void Added_ExistingDevice_RefreshesIsConnected()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new BluetoothDeviceService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = "Speaker",
            IsConnected = false,
            IsPaired = true,
        });

        Assert.False(service.Devices[0].IsConnected);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = "Speaker",
            IsConnected = true,
            IsPaired = true,
        });

        Assert.True(service.Devices[0].IsConnected);
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
            IsConnected = false,
            IsPaired = true,
        });

        var device = service.Devices[0];
        var changed = new List<string>();
        device.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        host.RaiseUpdated(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = string.Empty,
            IsConnected = true,
        });

        Assert.True(device.IsConnected);
        Assert.Contains(nameof(BluetoothDeviceInfo.IsConnected), changed, StringComparer.Ordinal);
    }

    [Fact]
    public void Updated_WithoutIsConnected_KeepsTheValue()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new BluetoothDeviceService(host);

        host.RaiseAdded(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = "Speaker",
            IsConnected = true,
            IsPaired = true,
        });

        host.RaiseUpdated(new DeviceSnapshot
        {
            Id = "bt-1",
            Name = string.Empty,
            IsConnected = null,
        });

        Assert.True(service.Devices[0].IsConnected);
    }

    [Fact]
    public void Updated_UnknownDevice_IsIgnored()
    {
        using var host = new FakeDeviceWatcherHost();
        using var service = new BluetoothDeviceService(host);

        host.RaiseUpdated(new DeviceSnapshot
        {
            Id = "bt-unknown",
            Name = string.Empty,
            IsConnected = true,
        });

        Assert.Empty(service.Devices);
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