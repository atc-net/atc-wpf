namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates and watches the audio input or output endpoints of the system.
/// </summary>
public interface IAudioDeviceService : IDisposable
{
    /// <summary>
    /// Gets the known audio endpoints; endpoints that disappear stay in the list with the <see cref="DeviceState.Disconnected"/> state.
    /// </summary>
    ObservableCollection<AudioDeviceInfo> Devices { get; }

    /// <summary>
    /// Gets the kind of endpoints this service enumerates.
    /// </summary>
    AudioDeviceKind Kind { get; }

    /// <summary>
    /// Starts the device watcher so that endpoints added or removed are reflected in <see cref="Devices"/>.
    /// </summary>
    void StartWatching();

    /// <summary>
    /// Stops the device watcher.
    /// </summary>
    void StopWatching();

    /// <summary>
    /// Enumerates the endpoints once, adding new ones and marking missing ones as <see cref="DeviceState.Disconnected"/>.
    /// </summary>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    Task RefreshAsync();
}