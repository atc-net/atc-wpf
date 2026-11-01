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
    /// <remarks>Start and stop calls are counted, so a service shared by several pickers keeps watching until each
    /// <see cref="StartWatching"/> has been matched by a call to this method.</remarks>
    void StopWatching();

    /// <summary>
    /// Enumerates the endpoints once, adding new ones and marking missing ones as <see cref="DeviceState.Disconnected"/>.
    /// </summary>
    /// <remarks>Safe to call from any thread: the collection is only changed on the UI thread.</remarks>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    Task RefreshAsync();
}