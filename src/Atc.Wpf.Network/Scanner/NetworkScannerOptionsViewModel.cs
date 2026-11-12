namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// ViewModel for what the network scanner checks on each address, and how long it waits.
/// </summary>
/// <remarks>
/// The defaults keep the behaviour of earlier versions: ping, host name, MAC address and vendor on;
/// NetBIOS and service discovery off.
/// </remarks>
public partial class NetworkScannerOptionsViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets a value indicating whether each address is pinged.
    /// </summary>
    [ObservableProperty]
    private bool icmpPing = true;

    /// <summary>
    /// Gets or sets a value indicating whether host names are looked up with reverse DNS.
    /// </summary>
    /// <remarks>
    /// Reverse DNS has no timeout, so on networks without it a scan can take minutes. Turn it off,
    /// and turn on <see cref="ResolveNetBiosName"/> or <see cref="DiscoverServices"/>, for fast device names.
    /// </remarks>
    [ObservableProperty]
    private bool resolveHostName = true;

    /// <summary>
    /// Gets or sets a value indicating whether NetBIOS computer names are asked for.
    /// </summary>
    [ObservableProperty]
    private bool resolveNetBiosName;

    /// <summary>
    /// Gets or sets a value indicating whether MAC addresses are read from the ARP table.
    /// </summary>
    [ObservableProperty]
    private bool resolveMacAddress = true;

    /// <summary>
    /// Gets or sets a value indicating whether the vendor of each MAC address is looked up.
    /// </summary>
    /// <remarks>
    /// Without a vendor lookup set on the scanner, MAC addresses are sent to an online service (macvendors.com).
    /// </remarks>
    [ObservableProperty]
    private bool resolveVendorFromMacAddress = true;

    /// <summary>
    /// Gets or sets a value indicating whether mDNS and SSDP services are discovered.
    /// </summary>
    [ObservableProperty]
    private bool discoverServices;

    /// <summary>
    /// Gets or sets a value indicating whether ONVIF devices (IP cameras) are discovered.
    /// </summary>
    [ObservableProperty]
    private bool discoverOnvif;

    /// <summary>
    /// Gets or sets how the open ports are checked for web services.
    /// </summary>
    [ObservableProperty]
    private IPServicePortExaminationLevel treatOpenPortsAsWebServices = IPServicePortExaminationLevel.WellKnown;

    /// <summary>
    /// Gets or sets the ping timeout in milliseconds.
    /// </summary>
    [ObservableProperty]
    private int timeoutPingInMs = (int)new IPScannerConfig().TimeoutPing.TotalMilliseconds;

    /// <summary>
    /// Gets or sets the TCP port timeout in milliseconds.
    /// </summary>
    [ObservableProperty]
    private int timeoutTcpInMs = (int)new IPScannerConfig().TimeoutTcp.TotalMilliseconds;

    /// <summary>
    /// Gets or sets the HTTP timeout in milliseconds.
    /// </summary>
    [ObservableProperty]
    private int timeoutHttpInMs = (int)new IPScannerConfig().TimeoutHttp.TotalMilliseconds;

    /// <summary>
    /// Gets or sets how long service discovery listens for answers, in milliseconds.
    /// </summary>
    [ObservableProperty]
    private int timeoutDiscoveryInMs = (int)new IPScannerConfig().TimeoutDiscovery.TotalMilliseconds;

    /// <summary>
    /// Gets or sets the NetBIOS timeout in milliseconds.
    /// </summary>
    [ObservableProperty]
    private int timeoutNetBiosInMs = (int)new IPScannerConfig().TimeoutNetBios.TotalMilliseconds;
}