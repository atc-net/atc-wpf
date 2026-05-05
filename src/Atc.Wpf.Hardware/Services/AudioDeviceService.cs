namespace Atc.Wpf.Hardware.Services;

public sealed class AudioDeviceService : IAudioDeviceService
{
    private static readonly TimeSpan JustConnectedDuration = TimeSpan.FromSeconds(3);

    private readonly DeviceClass deviceClass;
    private readonly DeviceWatcherHost watcher;
    private bool started;
    private bool initialEnumerationCompleted;
    private bool disposed;

    public AudioDeviceService(AudioDeviceKind kind)
    {
        Kind = kind;
        deviceClass = kind is AudioDeviceKind.Input
            ? DeviceClass.AudioCapture
            : DeviceClass.AudioRender;

        Devices = new ObservableCollection<AudioDeviceInfo>();

        watcher = new DeviceWatcherHost(
            DeviceInformation.GetAqsFilterFromDeviceClass(deviceClass));

        watcher.Added += OnAdded;
        watcher.Removed += OnRemoved;
        watcher.Updated += OnUpdated;
        watcher.EnumerationCompleted += OnEnumerationCompleted;
    }

    public ObservableCollection<AudioDeviceInfo> Devices { get; }

    public AudioDeviceKind Kind { get; }

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
        var found = await DeviceInformation.FindAllAsync(deviceClass);

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
        // No-op for audio enumeration changes today.
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

        var newInfo = new AudioDeviceInfo(
            deviceId: info.Id,
            friendlyName: info.Name,
            kind: Kind,
            isEnabled: info.IsEnabled,
            isDefault: info.IsDefault)
        {
            State = !info.IsEnabled
                ? DeviceState.InUse
                : isInitialEnumeration
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

    private AudioDeviceInfo? FindByDeviceId(string deviceId)
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