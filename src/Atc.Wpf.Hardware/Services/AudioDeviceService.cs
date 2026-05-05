namespace Atc.Wpf.Hardware.Services;

public sealed class AudioDeviceService : IAudioDeviceService
{
    private static readonly TimeSpan JustConnectedDuration = TimeSpan.FromSeconds(3);

    private readonly IDeviceWatcherHost watcher;
    private bool started;
    private bool initialEnumerationCompleted;
    private bool disposed;

    public AudioDeviceService(AudioDeviceKind kind)
        : this(kind, new DeviceWatcherHost(DeviceInformation.GetAqsFilterFromDeviceClass(
            kind is AudioDeviceKind.Input ? DeviceClass.AudioCapture : DeviceClass.AudioRender)))
    {
    }

    internal AudioDeviceService(
        AudioDeviceKind kind,
        IDeviceWatcherHost watcher)
    {
        this.watcher = watcher ?? throw new ArgumentNullException(nameof(watcher));
        Kind = kind;
        Devices = new ObservableCollection<AudioDeviceInfo>();

        this.watcher.Added += OnAdded;
        this.watcher.Removed += OnRemoved;
        this.watcher.Updated += OnUpdated;
        this.watcher.EnumerationCompleted += OnEnumerationCompleted;
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
        // No-op for audio enumeration changes today.
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

        var newInfo = new AudioDeviceInfo(
            deviceId: snapshot.Id,
            friendlyName: snapshot.Name,
            kind: Kind,
            isEnabled: snapshot.IsEnabled,
            isDefault: snapshot.IsDefault)
        {
            State = !snapshot.IsEnabled
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