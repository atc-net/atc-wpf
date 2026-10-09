namespace Atc.Wpf.Hardware.Abstractions;

/// <summary>
/// Defines a labeled network adapter picker with validation.
/// </summary>
public interface ILabelNetworkAdapterPicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected network adapter.
    /// </summary>
    NetworkAdapterInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the inline refresh button is shown.
    /// </summary>
    bool ShowRefreshButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the adapter list refreshes automatically when adapters are added or removed.
    /// </summary>
    bool AutoRefreshOnDeviceChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection is cleared when the selected adapter disappears.
    /// </summary>
    bool ClearValueOnDisconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a disconnected selection is re-attached by device ID when the adapter reappears.
    /// </summary>
    bool AutoRebindOnReconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the first available adapter is selected automatically when none is selected.
    /// </summary>
    bool AutoSelectFirstAvailable { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text shown when no adapter is selected.
    /// </summary>
    string WatermarkText { get; set; }
}