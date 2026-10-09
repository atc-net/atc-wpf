namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the display monitors and polls for changes.
/// </summary>
public interface IDisplayService : IDisposable
{
    /// <summary>
    /// Gets the known display monitors; monitors that disappear are marked as <see cref="DeviceState.Disconnected"/>.
    /// </summary>
    ObservableCollection<DisplayInfo> Displays { get; }

    /// <summary>
    /// Gets or sets the interval between polls while watching.
    /// </summary>
    TimeSpan PollingInterval { get; set; }

    /// <summary>
    /// Starts polling for monitor changes at <see cref="PollingInterval"/>.
    /// </summary>
    void StartWatching();

    /// <summary>
    /// Stops polling for monitor changes.
    /// </summary>
    void StopWatching();

    /// <summary>
    /// Enumerates the monitors once and updates <see cref="Displays"/>.
    /// </summary>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    Task RefreshAsync();
}