namespace Atc.Wpf.Hardware.Abstractions;

/// <summary>
/// Defines a labeled printer picker with validation.
/// </summary>
public interface ILabelPrinterPicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected printer.
    /// </summary>
    PrinterInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the inline refresh button is shown.
    /// </summary>
    bool ShowRefreshButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the printer list refreshes automatically when printers are added or removed.
    /// </summary>
    bool AutoRefreshOnDeviceChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection is cleared when the selected printer disappears.
    /// </summary>
    bool ClearValueOnDisconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a disconnected selection is re-attached by device ID when the printer reappears.
    /// </summary>
    bool AutoRebindOnReconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the first available printer is selected automatically when none is selected.
    /// </summary>
    bool AutoSelectFirstAvailable { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text shown when no printer is selected.
    /// </summary>
    string WatermarkText { get; set; }
}