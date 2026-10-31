namespace Atc.Wpf.Hardware.Services.Internal;

internal sealed class DeviceWatcherHost : IDeviceWatcherHost
{
    private const string SerialPortNameKey = "System.DeviceInterface.Serial.PortName";
    private const string AepIsConnectedKey = "System.Devices.Aep.IsConnected";
    private const string ItemNameDisplayKey = "System.ItemNameDisplay";
    private const string InterfaceEnabledKey = "System.Devices.InterfaceEnabled";

    private readonly DeviceWatcher watcher;
    private readonly string aqsFilter;
    private readonly IEnumerable<string>? requestedProperties;
    private readonly Dispatcher uiDispatcher;
    private readonly WatcherEventSink sink;
    private readonly WatcherStartGate startGate = new();
    private volatile bool disposed;

    public DeviceWatcherHost(
        string aqsFilter,
        IEnumerable<string>? requestedProperties = null)
    {
        this.aqsFilter = aqsFilter;
        this.requestedProperties = requestedProperties;

        watcher = requestedProperties is null
            ? DeviceInformation.CreateWatcher(aqsFilter)
            : DeviceInformation.CreateWatcher(aqsFilter, requestedProperties);

        uiDispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

        // The watcher holds its handlers through COM, which the GC cannot see through: handlers on this host
        // would form a cycle (host -> watcher -> handler -> host) that keeps an undisposed service alive forever.
        // The sink only holds the host weakly.
        sink = new WatcherEventSink(this);
        sink.Attach(watcher);
    }

    public event EventHandler<DeviceSnapshotEventArgs>? Added;

    public event EventHandler<DeviceSnapshotEventArgs>? Updated;

    public event EventHandler<DeviceRemovedEventArgs>? Removed;

    public event EventHandler? EnumerationCompleted;

    public void StartWatching()
    {
        // Start is only legal from Created, Stopped or Aborted. Stop is asynchronous, so a quick Unloaded then
        // Loaded finds the watcher Stopping; the gate then starts it again once the Stopped event arrives.
        if (startGate.RequestStart(() => watcher.Status))
        {
            watcher.Start();
        }
    }

    public void StopWatching()
    {
        startGate.RequestStop();

        if (watcher.Status is DeviceWatcherStatus.Stopped or DeviceWatcherStatus.Created or DeviceWatcherStatus.Aborted)
        {
            return;
        }

        watcher.Stop();
    }

    public async Task<IReadOnlyList<DeviceSnapshot>> FindAllAsync()
    {
        var found = requestedProperties is null
            ? await DeviceInformation.FindAllAsync(aqsFilter)
            : await DeviceInformation.FindAllAsync(aqsFilter, requestedProperties);

        var snapshots = new List<DeviceSnapshot>(found.Count);
        foreach (var info in found)
        {
            snapshots.Add(ToSnapshot(info));
        }

        return snapshots;
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        sink.Detach(watcher);

        StopWatching();
    }

    private void OnWatcherAdded(DeviceInformation info)
        => MarshalToUi(
            () => Added?.Invoke(this, new DeviceSnapshotEventArgs(ToSnapshot(info))));

    private void OnWatcherUpdated(DeviceInformationUpdate update)
        => MarshalToUi(
            () => Updated?.Invoke(
                this,
                new DeviceSnapshotEventArgs(ToUpdatedSnapshot(update.Id, update.Properties))));

    private void OnWatcherRemoved(DeviceInformationUpdate update)
        => MarshalToUi(() => Removed?.Invoke(this, new DeviceRemovedEventArgs(update.Id)));

    private void OnWatcherEnumerationCompleted()
        => MarshalToUi(() => EnumerationCompleted?.Invoke(this, EventArgs.Empty));

    public Task InvokeOnUiAsync(Action action)
    {
        if (uiDispatcher.CheckAccess())
        {
            action();
            return Task.CompletedTask;
        }

        return uiDispatcher.InvokeAsync(action).Task;
    }

    private void OnWatcherStopped()
    {
        // Raised on a WinRT thread.
        if (!startGate.OnStopped())
        {
            return;
        }

        MarshalToUi(() =>
        {
            // A stop may have been requested again while this was queued.
            if (!disposed && startGate.IsStartWanted)
            {
                StartWatching();
            }
        });
    }

    private void MarshalToUi(Action action)
    {
        if (uiDispatcher.CheckAccess())
        {
            action();
            return;
        }

        _ = uiDispatcher.BeginInvoke(action);
    }

    private static DeviceSnapshot ToSnapshot(DeviceInformation info)
        => new()
        {
            Id = info.Id,
            Name = info.Name,
            IsEnabled = info.IsEnabled,
            IsDefault = info.IsDefault,
            Panel = MapPanel(info.EnclosureLocation),
            IsPaired = info.Pairing?.IsPaired,
            IsConnected = TryGetBoolean(info.Properties, AepIsConnectedKey),
            PortName = TryGetString(info.Properties, SerialPortNameKey),
        };

    /// <summary>
    /// Builds the snapshot for an Updated event. Only the id and the properties that changed are known,
    /// so everything else stays at its default and the optional properties are <see langword="null"/> when absent.
    /// </summary>
    internal static DeviceSnapshot ToUpdatedSnapshot(
        string id,
        IReadOnlyDictionary<string, object>? properties)
        => new()
        {
            Id = id,
            Name = string.Empty,
            IsConnected = TryGetBoolean(properties, AepIsConnectedKey),
            PortName = TryGetString(properties, SerialPortNameKey),
            UpdatedName = TryGetString(properties, ItemNameDisplayKey),
            UpdatedIsEnabled = TryGetBoolean(properties, InterfaceEnabledKey),
        };

    private static CameraPanel? MapPanel(EnclosureLocation? location)
    {
        if (location is null)
        {
            return null;
        }

        return location.Panel switch
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

    private static bool? TryGetBoolean(
        IReadOnlyDictionary<string, object>? properties,
        string key)
        => properties is not null &&
           properties.TryGetValue(key, out var value) &&
           value is bool result
            ? result
            : null;

    private static string? TryGetString(
        IReadOnlyDictionary<string, object>? properties,
        string key)
        => properties is not null &&
           properties.TryGetValue(key, out var value)
            ? value as string
            : null;

    /// <summary>
    /// Receives the watcher events and forwards them to the host while it is alive. Once the host has been
    /// collected without being disposed, the sink stops the watcher and unsubscribes.
    /// </summary>
    private sealed class WatcherEventSink
    {
        private readonly WeakReference<DeviceWatcherHost> owner;

        public WatcherEventSink(DeviceWatcherHost owner)
            => this.owner = new WeakReference<DeviceWatcherHost>(owner);

        public void Attach(DeviceWatcher watcher)
        {
            watcher.Added += OnAdded;
            watcher.Updated += OnUpdated;
            watcher.Removed += OnRemoved;
            watcher.EnumerationCompleted += OnEnumerationCompleted;
            watcher.Stopped += OnStopped;
        }

        public void Detach(DeviceWatcher watcher)
        {
            watcher.Added -= OnAdded;
            watcher.Updated -= OnUpdated;
            watcher.Removed -= OnRemoved;
            watcher.EnumerationCompleted -= OnEnumerationCompleted;
            watcher.Stopped -= OnStopped;
        }

        private void OnAdded(
            DeviceWatcher sender,
            DeviceInformation info)
        {
            if (TryGetOwner(sender, out var host))
            {
                host.OnWatcherAdded(info);
            }
        }

        private void OnUpdated(
            DeviceWatcher sender,
            DeviceInformationUpdate update)
        {
            if (TryGetOwner(sender, out var host))
            {
                host.OnWatcherUpdated(update);
            }
        }

        private void OnRemoved(
            DeviceWatcher sender,
            DeviceInformationUpdate update)
        {
            if (TryGetOwner(sender, out var host))
            {
                host.OnWatcherRemoved(update);
            }
        }

        private void OnEnumerationCompleted(
            DeviceWatcher sender,
            object args)
        {
            if (TryGetOwner(sender, out var host))
            {
                host.OnWatcherEnumerationCompleted();
            }
        }

        private void OnStopped(
            DeviceWatcher sender,
            object args)
        {
            if (TryGetOwner(sender, out var host))
            {
                host.OnWatcherStopped();
            }
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Best-effort cleanup of an orphaned watcher.")]
        private bool TryGetOwner(
            DeviceWatcher sender,
            [NotNullWhen(true)] out DeviceWatcherHost? host)
        {
            if (owner.TryGetTarget(out host))
            {
                return true;
            }

            // The host was dropped while the watcher was still running: release the watcher.
            try
            {
                Detach(sender);
                if (sender.Status is DeviceWatcherStatus.Started or DeviceWatcherStatus.EnumerationCompleted)
                {
                    sender.Stop();
                }
            }
            catch (Exception)
            {
                // The watcher is already going away.
            }

            return false;
        }
    }
}