namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates and watches the paired Bluetooth devices.
/// </summary>
public interface IBluetoothDeviceService : IDisposable
{
    /// <summary>
    /// Gets the known Bluetooth devices; devices that disappear stay in the list with the <see cref="DeviceState.Disconnected"/> state.
    /// </summary>
    ObservableCollection<BluetoothDeviceInfo> Devices { get; }

    /// <summary>
    /// Starts the device watcher so that devices added, removed or updated are reflected in <see cref="Devices"/>.
    /// </summary>
    void StartWatching();

    /// <summary>
    /// Stops the device watcher.
    /// </summary>
    void StopWatching();

    /// <summary>
    /// Enumerates the devices once, adding new ones and marking missing ones as <see cref="DeviceState.Disconnected"/>.
    /// </summary>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    Task RefreshAsync();
}