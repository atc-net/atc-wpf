namespace Atc.Wpf.Hardware.Services;

public sealed class UsbCameraService : IUsbCameraService
{
    private static readonly TimeSpan JustConnectedDuration = TimeSpan.FromSeconds(3);

    private readonly DeviceWatcherHost watcher;
    private bool started;
    private bool initialEnumerationCompleted;
    private bool disposed;

    public UsbCameraService()
    {
        Cameras = new ObservableCollection<UsbCameraInfo>();

        watcher = new DeviceWatcherHost(
            DeviceInformation.GetAqsFilterFromDeviceClass(DeviceClass.VideoCapture));

        watcher.Added += OnAdded;
        watcher.Removed += OnRemoved;
        watcher.Updated += OnUpdated;
        watcher.EnumerationCompleted += OnEnumerationCompleted;
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
        var found = await DeviceInformation.FindAllAsync(
            DeviceClass.VideoCapture);

        var foundIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var info in found)
        {
            foundIds.Add(info.Id);
            UpsertFromInfo(info, isInitialEnumeration: true);
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
        // No-op for camera enumeration changes today.
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

            return;
        }

        var panel = TryReadPanel(info);

        var newInfo = new UsbCameraInfo(
            deviceId: info.Id,
            friendlyName: info.Name,
            panel: panel,
            isEnabled: info.IsEnabled)
        {
            State = !info.IsEnabled
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

    private static CameraPanel TryReadPanel(DeviceInformation info)
    {
        if (info.EnclosureLocation is null)
        {
            return CameraPanel.Unknown;
        }

        return info.EnclosureLocation.Panel switch
        {
            Windows.Devices.Enumeration.Panel.Front => CameraPanel.Front,
            Windows.Devices.Enumeration.Panel.Back => CameraPanel.Back,
            Windows.Devices.Enumeration.Panel.Top => CameraPanel.Top,
            Windows.Devices.Enumeration.Panel.Bottom => CameraPanel.Bottom,
            Windows.Devices.Enumeration.Panel.Left => CameraPanel.Left,
            Windows.Devices.Enumeration.Panel.Right => CameraPanel.Right,
            _ => CameraPanel.External,
        };
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