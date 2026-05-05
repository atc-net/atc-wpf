namespace Atc.Wpf.Hardware.Services;

public sealed class UsbCameraService : IUsbCameraService
{
    private static readonly TimeSpan JustConnectedDuration = TimeSpan.FromSeconds(3);

    private readonly IDeviceWatcherHost watcher;
    private bool started;
    private bool initialEnumerationCompleted;
    private bool disposed;

    public UsbCameraService()
        : this(new DeviceWatcherHost(
            DeviceInformation.GetAqsFilterFromDeviceClass(DeviceClass.VideoCapture)))
    {
    }

    internal UsbCameraService(IDeviceWatcherHost watcher)
    {
        this.watcher = watcher ?? throw new ArgumentNullException(nameof(watcher));
        Cameras = new ObservableCollection<UsbCameraInfo>();

        this.watcher.Added += OnAdded;
        this.watcher.Removed += OnRemoved;
        this.watcher.Updated += OnUpdated;
        this.watcher.EnumerationCompleted += OnEnumerationCompleted;
    }

    public ObservableCollection<UsbCameraInfo> Cameras { get; }

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

        for (var i = Cameras.Count - 1; i >= 0; i--)
        {
            if (!foundIds.Contains(Cameras[i].DeviceId))
            {
                Cameras[i].State = DeviceState.Disconnected;
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
        // No-op for camera enumeration changes today.
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

            return;
        }

        var newInfo = new UsbCameraInfo(
            deviceId: snapshot.Id,
            friendlyName: snapshot.Name,
            panel: snapshot.Panel ?? CameraPanel.Unknown,
            isEnabled: snapshot.IsEnabled)
        {
            State = !snapshot.IsEnabled
                ? DeviceState.InUse
                : isInitialEnumeration
                    ? DeviceState.Available
                    : DeviceState.JustConnected,
        };

        Cameras.Add(newInfo);

        if (newInfo.State is DeviceState.JustConnected)
        {
            JustConnectedTimer.TransitionToAvailableAfter(
                state => newInfo.State = state,
                JustConnectedDuration);
        }
    }

    private UsbCameraInfo? FindByDeviceId(string deviceId)
    {
        for (var i = 0; i < Cameras.Count; i++)
        {
            if (string.Equals(Cameras[i].DeviceId, deviceId, StringComparison.OrdinalIgnoreCase))
            {
                return Cameras[i];
            }
        }

        return null;
    }
}