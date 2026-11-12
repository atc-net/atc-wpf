namespace Atc.Wpf.Network.Discovery;

/// <summary>
/// Finds devices that announce themselves with mDNS, SSDP or ONVIF WS-Discovery.
/// </summary>
public interface IServiceDiscoverer
{
    /// <summary>
    /// Finds every mDNS / DNS-SD service.
    /// </summary>
    /// <param name="duration">How long to listen for answers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The services, as they are found.</returns>
    IAsyncEnumerable<MdnsServiceInstance> BrowseMdns(
        TimeSpan duration,
        CancellationToken cancellationToken);

    /// <summary>
    /// Finds every SSDP (UPnP) device and service.
    /// </summary>
    /// <param name="duration">How long to listen for answers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The devices, as they are found.</returns>
    IAsyncEnumerable<SsdpDevice> SearchSsdp(
        TimeSpan duration,
        CancellationToken cancellationToken);

    /// <summary>
    /// Finds every ONVIF device.
    /// </summary>
    /// <param name="duration">How long to listen for answers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The devices, as they are found.</returns>
    IAsyncEnumerable<OnvifDevice> ProbeOnvif(
        TimeSpan duration,
        CancellationToken cancellationToken);
}