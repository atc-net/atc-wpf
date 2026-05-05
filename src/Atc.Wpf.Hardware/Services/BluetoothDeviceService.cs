namespace Atc.Wpf.Hardware.Services;

public sealed class BluetoothDeviceService : IBluetoothDeviceService
{
    private static readonly TimeSpan JustConnectedDuration = TimeSpan.FromSeconds(3);

    private readonly DeviceWatcherHost watcher;
    private readonly string aqs;
    private bool started;
    private bool initialEnumerationCompleted;
    private bool disposed;

    public BluetoothDeviceService()
    {
        Devices = new ObservableCollection<BluetoothDeviceInfo>();

        aqs = Windows.Devices.Bluetooth.BluetoothDevice.GetDeviceSelectorFromPairingState(true);

        watcher = new DeviceWatcherHost(aqs);

        watcher.Added += OnAdded;
        watcher.Removed += OnRemoved;
        watcher.Updated += OnUpdated;
        watcher.EnumerationCompleted += OnEnumerationCompleted;
    }

    public ObservableCollection<BluetoothDeviceInfo> Devices { get; }

    public void StartWatching()
    {
        if (started)
        {
            return;
        }

        started = true;
        watcher.StartWatching();
    }

    public void StopWatching()
    {
        if (!started)
        {
            return;
        }

        started = false;
        watcher.StopWatching();
    }

    public async Task RefreshAsync()
    {
        var found = await DeviceInformation.FindAllAsync(aqs);

        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var info in found)
        {
            foundIds.Add(info.Id);
            UpsertFromInfo(info, isInitialEnumeration: true);
        }

        for (var i = Devices.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Devices[i].DeviceId))
            {
                Devices[i].State = DeviceState.Disconnected;
            }
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        watcher.Added -= OnAdded;
        watcher.Removed -= OnRemoved;
        watcher.Updated -= OnUpdated;
        watcher.EnumerationCompleted -= OnEnumerationCompleted;
        watcher.Dispose();
    }

    private void OnAdded(
        object? sender,
        DeviceArrivedEventArgs e)
        => UpsertFromInfo(e.Device, isInitialEnumeration: !initialEnumerationCompleted);

    private void OnRemoved(
        object? sender,
        DeviceRemovedEventArgs e)
    {
        var existing = FindByDeviceId(e.Update.Id);
        if (existing is not null)
        {
            existing.State = DeviceState.Disconnected;
        }
    }

    private static void OnUpdated(
        object? sender,
        DeviceUpdatedEventArgs e)
    {
        // No-op for Bluetooth; rely on Added/Removed for v1 connection signalling.
    }

    private void OnEnumerationCompleted(
        object? sender,
        EventArgs e)
    {
        initialEnumerationCompleted = true;
    }

    private void UpsertFromInfo(
        DeviceInformation info,
        bool isInitialEnumeration)
    {
        var existing = FindByDeviceId(info.Id);

        if (existing is not null)
        {
            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            existing.IsConnected = info.IsEnabled;
            return;
        }

        var newInfo = new BluetoothDeviceInfo(
            deviceId: info.Id,
            friendlyName: info.Name,
            isPaired: info.Pairing?.IsPaired ?? true,
            isConnected: info.IsEnabled)
        {
            State = isInitialEnumeration
                ? DeviceState.Available
                : DeviceState.JustConnected,
        };

        Devices.Add(newInfo);

        if (newInfo.State is DeviceState.JustConnected)
        {
            JustConnectedTimer.TransitionToAvailableAfter(
                state => newInfo.State = state,
                JustConnectedDuration);
        }
    }

    private BluetoothDeviceInfo? FindByDeviceId(string deviceId)
    {
        for (var i = 0; i < Devices.Count; i++)
        {
            if (string.Equals(Devices[i].DeviceId, deviceId, StringComparison.OrdinalIgnoreCase))
            {
                return Devices[i];
            }
        }

        return null;
    }
}