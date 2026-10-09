namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the logical drives and polls for changes.
/// </summary>
public interface IDriveService : IDisposable
{
    /// <summary>
    /// Gets the known logical drives.
    /// </summary>
    ObservableCollection<DiskDriveInfo> Drives { get; }

    /// <summary>
    /// Gets or sets the interval between polls while watching.
    /// </summary>
    TimeSpan PollingInterval { get; set; }

    /// <summary>
    /// Starts polling for drive changes at <see cref="PollingInterval"/>.
    /// </summary>
    void StartWatching();

    /// <summary>
    /// Stops polling for drive changes.
    /// </summary>
    void StopWatching();

    /// <summary>
    /// Enumerates the drives once and updates <see cref="Drives"/>.
    /// </summary>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    Task RefreshAsync();
}