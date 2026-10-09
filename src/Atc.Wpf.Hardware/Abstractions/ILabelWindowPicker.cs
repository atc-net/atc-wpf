namespace Atc.Wpf.Hardware.Abstractions;

/// <summary>
/// Defines a labeled top-level window picker with validation.
/// </summary>
public interface ILabelWindowPicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected window.
    /// </summary>
    TopLevelWindowInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the inline refresh button is shown.
    /// </summary>
    bool ShowRefreshButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the window list refreshes automatically when windows open or close.
    /// </summary>
    bool AutoRefreshOnDeviceChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection is cleared when the selected window closes.
    /// </summary>
    bool ClearValueOnDisconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a lost selection is re-attached by window handle when the window reappears.
    /// </summary>
    bool AutoRebindOnReconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the first available window is selected automatically when none is selected.
    /// </summary>
    bool AutoSelectFirstAvailable { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text shown when no window is selected.
    /// </summary>
    string WatermarkText { get; set; }
}