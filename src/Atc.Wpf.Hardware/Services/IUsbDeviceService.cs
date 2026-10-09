namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates and watches the USB device interfaces of the system.
/// </summary>
public interface IUsbDeviceService : IDisposable
{
    /// <summary>
    /// Gets the known USB devices; devices that disappear stay in the list with the <see cref="DeviceState.Disconnected"/> state.
    /// </summary>
    ObservableCollection<UsbDeviceInfo> Devices { get; }

    /// <summary>
    /// Gets or sets the device interface classes to list; <see cref="UsbDeviceClassFilter.None"/> lists all USB devices.
    /// </summary>
    UsbDeviceClassFilter ClassFilter { get; set; }

    /// <summary>
    /// Starts the device watcher so that devices added or removed are reflected in <see cref="Devices"/>.
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