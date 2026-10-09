namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates and watches the serial (COM) ports of the system.
/// </summary>
public interface ISerialPortService : IDisposable
{
    /// <summary>
    /// Gets the known serial ports; ports that disappear stay in the list with the <see cref="DeviceState.Disconnected"/> state.
    /// </summary>
    ObservableCollection<SerialPortInfo> Ports { get; }

    /// <summary>
    /// Starts the device watcher so that ports added or removed are reflected in <see cref="Ports"/>.
    /// </summary>
    void StartWatching();

    /// <summary>
    /// Stops the device watcher.
    /// </summary>
    void StopWatching();

    /// <summary>
    /// Enumerates the ports once, adding new ones and marking missing ones as <see cref="DeviceState.Disconnected"/>.
    /// </summary>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    Task RefreshAsync();

    /// <summary>
    /// Checks whether a port is held by another process by trying to open it.
    /// </summary>
    /// <param name="port">The port to probe.</param>
    /// <returns><see langword="true"/> if the port could not be opened because it is in use or access was denied; otherwise <see langword="false"/>.</returns>
    Task<bool> ProbeInUseAsync(SerialPortInfo port);
}