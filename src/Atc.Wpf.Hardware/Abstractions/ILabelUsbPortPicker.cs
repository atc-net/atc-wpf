namespace Atc.Wpf.Hardware.Abstractions;

/// <summary>
/// Defines a labeled USB device picker with validation.
/// </summary>
public interface ILabelUsbPortPicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected USB device.
    /// </summary>
    UsbDeviceInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets the device class used to filter the listed USB devices.
    /// </summary>
    UsbDeviceClassFilter ClassFilter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the inline refresh button is shown.
    /// </summary>
    bool ShowRefreshButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the device list refreshes automatically when devices are added or removed.
    /// </summary>
    bool AutoRefreshOnDeviceChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether devices are actively probed to detect whether they are in use.
    /// </summary>
    bool DetectInUseState { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection is cleared when the selected device disconnects.
    /// </summary>
    bool ClearValueOnDisconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a disconnected selection is re-attached by device ID when the device reconnects.
    /// </summary>
    bool AutoRebindOnReconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the first available device is selected automatically when none is selected.
    /// </summary>
    bool AutoSelectFirstAvailable { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text shown when no device is selected.
    /// </summary>
    string WatermarkText { get; set; }
}