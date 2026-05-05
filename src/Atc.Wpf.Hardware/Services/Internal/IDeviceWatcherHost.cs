namespace Atc.Wpf.Hardware.Services.Internal;

internal interface IDeviceWatcherHost : IDisposable
{
    event EventHandler<DeviceSnapshotEventArgs>? Added;

    event EventHandler<DeviceSnapshotEventArgs>? Updated;

    event EventHandler<DeviceRemovedEventArgs>? Removed;

    event EventHandler? EnumerationCompleted;

    void StartWatching();

    void StopWatching();

    Task<IReadOnlyList<DeviceSnapshot>> FindAllAsync();
}