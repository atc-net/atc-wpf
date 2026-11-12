namespace Atc.Wpf.Network.Discovery;

/// <summary>
/// Lists the devices on the local network that announce themselves with mDNS, SSDP or ONVIF, without scanning
/// an address range, with the details of the selected device.
/// </summary>
/// <remarks>
/// This control expects a <see cref="ServiceDiscoveryViewModel"/> as its DataContext.
/// </remarks>
public partial class ServiceDiscoveryView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceDiscoveryView"/> class.
    /// </summary>
    public ServiceDiscoveryView()
        => InitializeComponent();
}