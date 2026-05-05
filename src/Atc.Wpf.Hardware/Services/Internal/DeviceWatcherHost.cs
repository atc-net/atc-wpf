namespace Atc.Wpf.Hardware.Services.Internal;

internal sealed class DeviceWatcherHost : IDisposable
{
    private readonly DeviceWatcher watcher;
    private readonly Dispatcher uiDispatcher;
    private bool disposed;

    public DeviceWatcherHost(
        string aqsFilter,
        IEnumerable<string>? requestedProperties = null)
    {
        watcher = requestedProperties is null
            ? DeviceInformation.CreateWatcher(aqsFilter)
            : DeviceInformation.CreateWatcher(aqsFilter, requestedProperties);

        uiDispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

        watcher.Added += OnWatcherAdded;
        watcher.Updated += OnWatcherUpdated;
        watcher.Removed += OnWatcherRemoved;
        watcher.EnumerationCompleted += OnWatcherEnumerationCompleted;
    }

    public event EventHandler<DeviceArrivedEventArgs>? Added;

    public event EventHandler<DeviceUpdatedEventArgs>? Updated;

    public event EventHandler<DeviceRemovedEventArgs>? Removed;

    public event EventHandler? EnumerationCompleted;

    public DeviceWatcherStatus Status => watcher.Status;

    public void StartWatching()
    {
        if (watcher.Status is DeviceWatcherStatus.Started or DeviceWatcherStatus.EnumerationCompleted)
        {
            return;
        }

        watcher.Start();
    }

    public void StopWatching()
    {
        if (watcher.Status is DeviceWatcherStatus.Stopped or DeviceWatcherStatus.Created)
        {
            return;
        }

        watcher.Stop();
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

        StopWatching();
    }

    private void OnWatcherAdded(
        DeviceWatcher sender,
        DeviceInformation info)
        => MarshalToUi(() => Added?.Invoke(this, new DeviceArrivedEventArgs(info)));

    private void OnWatcherUpdated(
        DeviceWatcher sender,
        DeviceInformationUpdate update)
        => MarshalToUi(
            () => Updated?.Invoke(this, new DeviceUpdatedEventArgs(update)));

    private void OnWatcherRemoved(
        DeviceWatcher sender,
        DeviceInformationUpdate update)
        => MarshalToUi(
            () => Removed?.Invoke(this, new DeviceRemovedEventArgs(update)));

    private void OnWatcherEnumerationCompleted(
        DeviceWatcher sender,
        object args)
        => MarshalToUi(
            () => EnumerationCompleted?.Invoke(this, EventArgs.Empty));

    private void MarshalToUi(Action action)
    {
        if (uiDispatcher.CheckAccess())
        {
            action();
            return;
        }

        _ = uiDispatcher.BeginInvoke(action);
    }
}