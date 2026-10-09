namespace Atc.Wpf.Hardware.Abstractions;

/// <summary>
/// Defines a labeled serial port picker with validation.
/// </summary>
public interface ILabelSerialPortPicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected serial port.
    /// </summary>
    SerialPortInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the inline refresh button is shown.
    /// </summary>
    bool ShowRefreshButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the port list refreshes automatically when ports are added or removed.
    /// </summary>
    bool AutoRefreshOnDeviceChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether ports are actively probed to detect whether they are in use.
    /// </summary>
    bool DetectInUseState { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection is cleared when the selected port disconnects.
    /// </summary>
    bool ClearValueOnDisconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a disconnected selection is re-attached by device ID when the port reconnects.
    /// </summary>
    bool AutoRebindOnReconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the first available port is selected automatically when none is selected.
    /// </summary>
    bool AutoSelectFirstAvailable { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text shown when no port is selected.
    /// </summary>
    string WatermarkText { get; set; }
}