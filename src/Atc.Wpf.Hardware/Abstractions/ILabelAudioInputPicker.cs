namespace Atc.Wpf.Hardware.Abstractions;

/// <summary>
/// Defines a labeled audio input device picker with validation.
/// </summary>
public interface ILabelAudioInputPicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected audio input device.
    /// </summary>
    AudioDeviceInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the inline refresh button is shown.
    /// </summary>
    bool ShowRefreshButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the device list refreshes automatically when devices are added or removed.
    /// </summary>
    bool AutoRefreshOnDeviceChange { get; set; }

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
    /// Gets or sets a value indicating whether a live waveform and peak-level preview is shown for the selected device.
    /// </summary>
    bool ShowLivePreview { get; set; }

    /// <summary>
    /// Gets or sets the height, in device-independent pixels, of the live preview pane.
    /// </summary>
    double PreviewHeight { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text shown when no device is selected.
    /// </summary>
    string WatermarkText { get; set; }
}