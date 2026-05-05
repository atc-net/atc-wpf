namespace Atc.Wpf.Hardware.Services;

public sealed class UsbDeviceService : IUsbDeviceService
{
    private static readonly TimeSpan JustConnectedDuration = TimeSpan.FromSeconds(3);

    private readonly System.Threading.Lock sync = new();
    private readonly Func<string, IDeviceWatcherHost> watcherFactory;
    private IDeviceWatcherHost watcher;
    private string aqs;
    private UsbDeviceClassFilter classFilter = UsbDeviceClassFilter.None;
    private bool started;
    private bool initialEnumerationCompleted;
    private bool disposed;

    public UsbDeviceService()
        : this(static aqs => new DeviceWatcherHost(aqs))
    {
    }

    internal UsbDeviceService(Func<string, IDeviceWatcherHost> watcherFactory)
    {
        this.watcherFactory = watcherFactory ?? throw new ArgumentNullException(nameof(watcherFactory));
        Devices = new ObservableCollection<UsbDeviceInfo>();

        aqs = UsbDeviceClassFilterResolver.ToAqs(classFilter);
        watcher = CreateWatcher(aqs);
    }

    public ObservableCollection<UsbDeviceInfo> Devices { get; }

    public UsbDeviceClassFilter ClassFilter
    {
        get => classFilter;
        set
        {
            if (classFilter == value)
            {
                return;
            }

            classFilter = value;
            ApplyClassFilter();
        }
    }

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

        DetachWatcher(watcher);
        watcher.Dispose();
    }

    private IDeviceWatcherHost CreateWatcher(string aqsFilter)
    {
        var host = watcherFactory(aqsFilter);
        host.Added += OnAdded;
        host.Removed += OnRemoved;
        host.Updated += OnUpdated;
        host.EnumerationCompleted += OnEnumerationCompleted;
        return host;
    }

    private void DetachWatcher(IDeviceWatcherHost host)
    {
        host.Added -= OnAdded;
        host.Removed -= OnRemoved;
        host.Updated -= OnUpdated;
        host.EnumerationCompleted -= OnEnumerationCompleted;
    }

    private void ApplyClassFilter()
    {
        lock (sync)
        {
            var newAqs = UsbDeviceClassFilterResolver.ToAqs(classFilter);
            if (string.Equals(newAqs, aqs, StringComparison.Ordinal))
            {
                return;
            }

            var wasStarted = started;
            if (wasStarted)
            {
                watcher.StopWatching();
            }

            DetachWatcher(watcher);
            watcher.Dispose();

            aqs = newAqs;
            initialEnumerationCompleted = false;
            Devices.Clear();

            watcher = CreateWatcher(aqs);

            if (wasStarted)
            {
                watcher.StartWatching();
            }
        }
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
        // No-op for USB. Property updates are rare.
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

        var (vid, pid) = UsbIdParser.Parse(snapshot.Id);

        var newInfo = new UsbDeviceInfo(
            deviceId: snapshot.Id,
            friendlyName: snapshot.Name,
            vendorId: vid,
            productId: pid,
            pnpClass: null,
            interfaceEnabled: snapshot.IsEnabled)
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

    private UsbDeviceInfo? FindByDeviceId(string deviceId)
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