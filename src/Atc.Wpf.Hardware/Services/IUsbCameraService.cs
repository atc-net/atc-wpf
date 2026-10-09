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
    void StopWatching();

    /// <summary>
    /// Enumerates the cameras once, adding new ones and marking missing ones as <see cref="DeviceState.Disconnected"/>.
    /// </summary>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    Task RefreshAsync();
}