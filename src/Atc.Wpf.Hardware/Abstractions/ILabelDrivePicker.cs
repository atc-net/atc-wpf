namespace Atc.Wpf.Hardware.Abstractions;

/// <summary>
/// Defines a labeled drive picker with validation.
/// </summary>
public interface ILabelDrivePicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected drive.
    /// </summary>
    DiskDriveInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the inline refresh button is shown.
    /// </summary>
    bool ShowRefreshButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the drive list refreshes automatically when drives are added or removed.
    /// </summary>
    bool AutoRefreshOnDeviceChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection is cleared when the selected drive disconnects.
    /// </summary>
    bool ClearValueOnDisconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a disconnected selection is re-attached by device ID when the drive reconnects.
    /// </summary>
    bool AutoRebindOnReconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the first available drive is selected automatically when none is selected.
    /// </summary>
    bool AutoSelectFirstAvailable { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text shown when no drive is selected.
    /// </summary>
    string WatermarkText { get; set; }
}