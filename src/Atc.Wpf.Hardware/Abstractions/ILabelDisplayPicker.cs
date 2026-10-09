namespace Atc.Wpf.Hardware.Abstractions;

/// <summary>
/// Defines a labeled display (monitor) picker with validation.
/// </summary>
public interface ILabelDisplayPicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected display.
    /// </summary>
    DisplayInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the inline refresh button is shown.
    /// </summary>
    bool ShowRefreshButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the display list refreshes automatically when displays are added or removed.
    /// </summary>
    bool AutoRefreshOnDeviceChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection is cleared when the selected display disconnects.
    /// </summary>
    bool ClearValueOnDisconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a disconnected selection is re-attached by device ID when the display reconnects.
    /// </summary>
    bool AutoRebindOnReconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the first available display is selected automatically when none is selected.
    /// </summary>
    bool AutoSelectFirstAvailable { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text shown when no display is selected.
    /// </summary>
    string WatermarkText { get; set; }
}