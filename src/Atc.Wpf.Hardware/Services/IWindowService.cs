namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the top-level windows and polls for changes.
/// </summary>
public interface IWindowService : IDisposable
{
    /// <summary>
    /// Gets the known top-level windows; windows that close are marked as <see cref="DeviceState.Disconnected"/>.
    /// </summary>
    ObservableCollection<TopLevelWindowInfo> Windows { get; }

    /// <summary>
    /// Gets or sets the interval between polls while watching.
    /// </summary>
    TimeSpan PollingInterval { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether only visible windows that have a title are listed.
    /// </summary>
    bool OnlyVisibleWithTitle { get; set; }

    /// <summary>
    /// Starts polling for window changes at <see cref="PollingInterval"/>.
    /// </summary>
    void StartWatching();

    /// <summary>
    /// Stops polling for window changes.
    /// </summary>
    void StopWatching();

    /// <summary>
    /// Enumerates the windows once and updates <see cref="Windows"/>.
    /// </summary>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    Task RefreshAsync();
}