namespace Atc.Wpf.Hardware.Services;

public sealed class BluetoothDeviceService : IBluetoothDeviceService
{
    private static readonly TimeSpan JustConnectedDuration = TimeSpan.FromSeconds(3);

    private readonly IDeviceWatcherHost watcher;
    private bool started;
    private bool initialEnumerationCompleted;
    private bool disposed;

    public BluetoothDeviceService()
        : this(new DeviceWatcherHost(
            Windows.Devices.Bluetooth.BluetoothDevice.GetDeviceSelectorFromPairingState(true)))
    {
    }

    internal BluetoothDeviceService(IDeviceWatcherHost watcher)
    {
        this.watcher = watcher ?? throw new ArgumentNullException(nameof(watcher));
        Devices = new ObservableCollection<BluetoothDeviceInfo>();

        this.watcher.Added += OnAdded;
        this.watcher.Removed += OnRemoved;
        this.watcher.Updated += OnUpdated;
        this.watcher.EnumerationCompleted += OnEnumerationCompleted;
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
        var found = await watcher.FindAllAsync();

        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var snapshot in found)
        {
            foundIds.Add(snapshot.Id);
            UpsertFromSnapshot(snapshot, isInitialEnumeration: true);
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
        DeviceSnapshotEventArgs e)
        => UpsertFromSnapshot(e.Snapshot, isInitialEnumeration: !initialEnumerationCompleted);

    private void OnRemoved(
        object? sender,
        DeviceRemovedEventArgs e)
    {
        var existing = FindByDeviceId(e.DeviceId);
        if (existing is not null)
        {
            existing.State = DeviceState.Disconnected;
        }
    }

    private static void OnUpdated(
        object? sender,
        DeviceSnapshotEventArgs e)
    {
        // No-op for Bluetooth; rely on Added/Removed for v1 connection signalling.
    }

    private void OnEnumerationCompleted(
        object? sender,
        EventArgs e)
    {
        initialEnumerationCompleted = true;
    }

    private void UpsertFromSnapshot(
        DeviceSnapshot snapshot,
        bool isInitialEnumeration)
    {
        var existing = FindByDeviceId(snapshot.Id);

        if (existing is not null)
        {
            if (existing.State is DeviceState.Disconnected)
            {
                existing.State = DeviceState.Available;
            }

            existing.IsConnected = snapshot.IsEnabled;
            return;
        }

        var newInfo = new BluetoothDeviceInfo(
            deviceId: snapshot.Id,
            friendlyName: snapshot.Name,
            isPaired: snapshot.IsPaired ?? true,
            isConnected: snapshot.IsEnabled)
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