namespace Atc.Wpf.Hardware.Abstractions;

/// <summary>
/// Defines a labeled running-process picker with validation.
/// </summary>
public interface ILabelProcessPicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected process.
    /// </summary>
    RunningProcessInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the inline refresh button is shown.
    /// </summary>
    bool ShowRefreshButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the process list refreshes automatically when processes start or exit.
    /// </summary>
    bool AutoRefreshOnDeviceChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the selection is cleared when the selected process exits.
    /// </summary>
    bool ClearValueOnDisconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a lost selection is re-attached by process ID when the process reappears.
    /// </summary>
    bool AutoRebindOnReconnect { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the first available process is selected automatically when none is selected.
    /// </summary>
    bool AutoSelectFirstAvailable { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text shown when no process is selected.
    /// </summary>
    string WatermarkText { get; set; }
}