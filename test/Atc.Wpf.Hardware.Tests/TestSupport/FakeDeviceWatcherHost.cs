namespace Atc.Wpf.Hardware.Tests.TestSupport;

internal sealed class FakeDeviceWatcherHost : IDeviceWatcherHost
{
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
        return Task.FromResult<IReadOnlyList<DeviceSnapshot>>(SeedSnapshots.ToArray());
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