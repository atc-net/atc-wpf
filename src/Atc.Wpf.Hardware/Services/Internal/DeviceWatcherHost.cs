namespace Atc.Wpf.Hardware.Services.Internal;

internal sealed class DeviceWatcherHost : IDeviceWatcherHost
{
    private const string SerialPortNameKey = "System.DeviceInterface.Serial.PortName";

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
                new DeviceSnapshotEventArgs(
                    new DeviceSnapshot { Id = update.Id, Name = string.Empty })));

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
            PortName = TryGetPortName(info),
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

    private static string? TryGetPortName(DeviceInformation info)
        => info.Properties.TryGetValue(SerialPortNameKey, out var value) ? value as string : null;
}