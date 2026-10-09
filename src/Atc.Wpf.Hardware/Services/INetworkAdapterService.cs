namespace Atc.Wpf.Hardware.Services;

/// <summary>
/// Enumerates the network adapters and polls for changes.
/// </summary>
public interface INetworkAdapterService : IDisposable
{
    /// <summary>
    /// Gets the known network adapters; adapters that disappear are marked as <see cref="DeviceState.Disconnected"/>.
    /// </summary>
    ObservableCollection<NetworkAdapterInfo> Adapters { get; }

    /// <summary>
    /// Gets or sets the interval between polls while watching.
    /// </summary>
    TimeSpan PollingInterval { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether loopback adapters are included.
    /// </summary>
    bool IncludeLoopback { get; set; }

    /// <summary>
    /// Starts polling for adapter changes at <see cref="PollingInterval"/>.
    /// </summary>
    void StartWatching();

    /// <summary>
    /// Stops polling for adapter changes.
    /// </summary>
    void StopWatching();

    /// <summary>
    /// Enumerates the adapters once and updates <see cref="Adapters"/>.
    /// </summary>
    /// <returns>A task that completes when the enumeration has been applied.</returns>
    Task RefreshAsync();
}