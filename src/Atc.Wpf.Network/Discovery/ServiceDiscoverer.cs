namespace Atc.Wpf.Network.Discovery;

/// <summary>
/// Finds devices with Atc.Network's <see cref="MdnsDiscovery"/>, <see cref="SsdpDiscovery"/> and <see cref="OnvifDiscovery"/>.
/// </summary>
public sealed class ServiceDiscoverer : IServiceDiscoverer
{
    /// <inheritdoc />
    public IAsyncEnumerable<MdnsServiceInstance> BrowseMdns(
        TimeSpan duration,
        CancellationToken cancellationToken)
        => MdnsDiscovery.BrowseAll(duration, cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<SsdpDevice> SearchSsdp(
        TimeSpan duration,
        CancellationToken cancellationToken)
        => SsdpDiscovery.Search(SsdpDiscovery.DefaultSearchTarget, duration, cancellationToken);

    /// <inheritdoc />
    public IAsyncEnumerable<OnvifDevice> ProbeOnvif(
        TimeSpan duration,
        CancellationToken cancellationToken)
        => OnvifDiscovery.Probe(duration, cancellationToken);
}