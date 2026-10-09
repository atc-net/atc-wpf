namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the printers (print queues) and polls for changes.
/// </summary>
public interface IPrinterService : IDisposable
{
    /// <summary>
    /// Gets the known printers; printers that disappear are marked as <see cref="DeviceState.Disconnected"/>.
    /// </summary>
    ObservableCollection<PrinterInfo> Printers { get; }

    /// <summary>
    /// Gets or sets the interval between polls while watching.
    /// </summary>
    TimeSpan PollingInterval { get; set; }

    /// <summary>
    /// Starts polling for printer changes at <see cref="PollingInterval"/>.
    /// </summary>
    void StartWatching();

    /// <summary>
    /// Stops polling for printer changes.
    /// </summary>
    void StopWatching();

    /// <summary>
    /// Enumerates the printers once and updates <see cref="Printers"/>.
    /// </summary>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    Task RefreshAsync();
}