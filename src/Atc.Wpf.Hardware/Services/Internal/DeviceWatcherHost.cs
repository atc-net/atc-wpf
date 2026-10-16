namespace Atc.Wpf.Hardware.Services.Internal;

internal sealed class DeviceWatcherHost : IDeviceWatcherHost
{
    private const string SerialPortNameKey = "System.DeviceInterface.Serial.PortName";
    private const string AepIsConnectedKey = "System.Devices.Aep.IsConnected";

    private readonly DeviceWatcher watcher;
    private readonly string aqsFilter;
    private readonly IEnumerable<string>? requestedProperties;
    private readonly Dispatcher uiDispatcher;
    private bool disposed;
    private bool startRequested;

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

        watcher.Added += OnWatcherAdded;
        watcher.Updated += OnWatcherUpdated;
        watcher.Removed += OnWatcherRemoved;
        watcher.EnumerationCompleted += OnWatcherEnumerationCompleted;
        watcher.Stopped += OnWatcherStopped;
    }

    public event EventHandler<DeviceSnapshotEventArgs>? Added;

    public event EventHandler<DeviceSnapshotEventArgs>? Updated;

    public event EventHandler<DeviceRemovedEventArgs>? Removed;

    public event EventHandler? EnumerationCompleted;

    public void StartWatching()
    {
        switch (watcher.Status)
        {
            case DeviceWatcherStatus.Started:
            case DeviceWatcherStatus.EnumerationCompleted:
                return;
            case DeviceWatcherStatus.Stopping:
                // Start is only legal from Created, Stopped or Aborted. Stop is asynchronous, so a quick
                // Unloaded then Loaded lands here; start again once the Stopped event arrives.
                startRequested = true;
                return;
        }

        watcher.Start();
    }

    public void StopWatching()
    {
        startRequested = false;

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

        watcher.Added -= OnWatcherAdded;
        watcher.Updated -= OnWatcherUpdated;
        watcher.Removed -= OnWatcherRemoved;
        watcher.EnumerationCompleted -= OnWatcherEnumerationCompleted;
        watcher.Stopped -= OnWatcherStopped;

        StopWatching();
    }

    private void OnWatcherAdded(
        DeviceWatcher sender,
        DeviceInformation info)
        => MarshalToUi(
            () => Added?.Invoke(this, new DeviceSnapshotEventArgs(ToSnapshot(info))));

    private void OnWatcherUpdated(
        DeviceWatcher sender,
        DeviceInformationUpdate update)
        => MarshalToUi(
            () => Updated?.Invoke(
                this,
                new DeviceSnapshotEventArgs(ToUpdatedSnapshot(update.Id, update.Properties))));

    private void OnWatcherRemoved(
        DeviceWatcher sender,
        DeviceInformationUpdate update)
        => MarshalToUi(() => Removed?.Invoke(this, new DeviceRemovedEventArgs(update.Id)));

    private void OnWatcherEnumerationCompleted(
        DeviceWatcher sender,
        object args)
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

    private void OnWatcherStopped(
        DeviceWatcher sender,
        object args)
    {
        if (!startRequested)
        {
            return;
        }

        startRequested = false;
        MarshalToUi(() =>
        {
            if (!disposed &&
                watcher.Status is DeviceWatcherStatus.Stopped or DeviceWatcherStatus.Aborted or DeviceWatcherStatus.Created)
            {
                watcher.Start();
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
}