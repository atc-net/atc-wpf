namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates and watches the video capture devices (cameras) of the system.
/// </summary>
public interface IUsbCameraService : IDisposable
{
    /// <summary>
    /// Gets the known cameras; cameras that disappear stay in the list with the <see cref="DeviceState.Disconnected"/> state.
    /// </summary>
    ObservableCollection<UsbCameraInfo> Cameras { get; }

    /// <summary>
    /// Starts the device watcher so that cameras added or removed are reflected in <see cref="Cameras"/>.
    /// </summary>
    void StartWatching();

    /// <summary>
    /// Stops the device watcher.
    /// </summary>
    /// <remarks>Start and stop calls are counted, so a service shared by several pickers keeps watching until each
    /// <see cref="StartWatching"/> has been matched by a call to this method.</remarks>
    void StopWatching();

    /// <summary>
    /// Enumerates the cameras once, adding new ones and marking missing ones as <see cref="DeviceState.Disconnected"/>.
    /// </summary>
    /// <remarks>Safe to call from any thread: the collection is only changed on the UI thread.</remarks>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    Task RefreshAsync();
}