namespace Atc.Wpf.Hardware.Tests.TestSupport;

internal sealed class FakeDeviceWatcherHost : IDeviceWatcherHost
{
    private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
    private TaskCompletionSource<IReadOnlyList<DeviceSnapshot>>? pendingFindAll;

    public event EventHandler<DeviceSnapshotEventArgs>? Added;

    public event EventHandler<DeviceSnapshotEventArgs>? Updated;

    public event EventHandler<DeviceRemovedEventArgs>? Removed;

    public event EventHandler? EnumerationCompleted;

    public bool IsStarted { get; private set; }

    public int StartWatchingCallCount { get; private set; }

    public int StopWatchingCallCount { get; private set; }

    public int FindAllCallCount { get; private set; }

    public bool IsDisposed { get; private set; }

    public List<DeviceSnapshot> SeedSnapshots { get; } = new();

    /// <summary>
    /// When set, <see cref="FindAllAsync"/> returns a task that stays pending until
    /// <see cref="CompleteFindAll"/> is called, so a test can complete it from another thread.
    /// </summary>
    public bool DeferFindAll { get; set; }

    public void StartWatching()
    {
        IsStarted = true;
        StartWatchingCallCount++;
    }

    public void StopWatching()
    {
        IsStarted = false;
        StopWatchingCallCount++;
    }

    public Task<IReadOnlyList<DeviceSnapshot>> FindAllAsync()
    {
        FindAllCallCount++;

        if (!DeferFindAll)
        {
            return Task.FromResult<IReadOnlyList<DeviceSnapshot>>(SeedSnapshots.ToArray());
        }

        pendingFindAll = new TaskCompletionSource<IReadOnlyList<DeviceSnapshot>>();
        return pendingFindAll.Task;
    }

    /// <summary>
    /// Completes the deferred <see cref="FindAllAsync"/> on the calling thread, so the awaiting
    /// service continues on that thread.
    /// </summary>
    public void CompleteFindAll()
    {
        var pending = pendingFindAll
                      ?? throw new InvalidOperationException($"{nameof(FindAllAsync)} has not been called with {nameof(DeferFindAll)} set.");

        pendingFindAll = null;
        pending.SetResult(SeedSnapshots.ToArray());
    }

    public Task InvokeOnUiAsync(Action action)
    {
        if (dispatcher.CheckAccess())
        {
            action();
            return Task.CompletedTask;
        }

        return dispatcher.InvokeAsync(action).Task;
    }

    public void Dispose()
    {
        IsDisposed = true;
    }

    public void RaiseAdded(DeviceSnapshot snapshot)
        => Added?.Invoke(this, new DeviceSnapshotEventArgs(snapshot));

    public void RaiseUpdated(DeviceSnapshot snapshot)
        => Updated?.Invoke(this, new DeviceSnapshotEventArgs(snapshot));

    public void RaiseRemoved(string deviceId)
        => Removed?.Invoke(this, new DeviceRemovedEventArgs(deviceId));

    public void RaiseEnumerationCompleted()
        => EnumerationCompleted?.Invoke(this, EventArgs.Empty);
}