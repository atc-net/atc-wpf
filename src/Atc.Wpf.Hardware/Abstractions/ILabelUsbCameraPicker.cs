namespace Atc.Wpf.Hardware.Abstractions;

/// <summary>
/// Defines a labeled USB camera picker with validation.
/// </summary>
public interface ILabelUsbCameraPicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected camera.
    /// </summary>
    UsbCameraInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the inline refresh button is shown.
    /// </summary>
    bool ShowRefreshButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the camera list refreshes automatically when cameras are added or removed.
    /// </summary>
    bool AutoRefreshOnDeviceChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection is cleared when the selected camera disconnects.
    /// </summary>
    bool ClearValueOnDisconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a disconnected selection is re-attached by device ID when the camera reconnects.
    /// </summary>
    bool AutoRebindOnReconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the first available camera is selected automatically when none is selected.
    /// </summary>
    bool AutoSelectFirstAvailable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a live preview of the selected camera is shown.
    /// </summary>
    bool ShowLivePreview { get; set; }

    /// <summary>
    /// Gets or sets the height, in device-independent pixels, of the live preview pane.
    /// </summary>
    double PreviewHeight { get; set; }

    /// <summary>
    /// Gets or sets the preferred resolution and frame rate for the live preview.
    /// </summary>
    UsbCameraFormat? PreferredFormat { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text shown when no camera is selected.
    /// </summary>
    string WatermarkText { get; set; }
}