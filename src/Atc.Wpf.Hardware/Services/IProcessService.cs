namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the running processes and polls for changes.
/// </summary>
public interface IProcessService : IDisposable
{
    /// <summary>
    /// Gets the known processes; processes that exit are marked as <see cref="DeviceState.Disconnected"/>.
    /// </summary>
    ObservableCollection<RunningProcessInfo> Processes { get; }

    /// <summary>
    /// Gets or sets the interval between polls while watching.
    /// </summary>
    TimeSpan PollingInterval { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether only processes that have a main window are listed.
    /// </summary>
    bool OnlyWithMainWindow { get; set; }

    /// <summary>
    /// Starts polling for process changes at <see cref="PollingInterval"/>.
    /// </summary>
    void StartWatching();

    /// <summary>
    /// Stops polling for process changes.
    /// </summary>
    void StopWatching();

    /// <summary>
    /// Enumerates the processes once and updates <see cref="Processes"/>.
    /// </summary>
    /// <remarks>Safe to call from any thread: the collection is only changed on the UI thread.</remarks>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    Task RefreshAsync();
}