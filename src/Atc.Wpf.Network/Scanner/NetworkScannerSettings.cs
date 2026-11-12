namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// The user's network scanner settings: range, ports, options, filters and columns.
/// </summary>
/// <remarks>
/// Get them with <see cref="NetworkScannerViewModel.GetSettings"/>, store them where the app keeps its settings
/// (for example as JSON), and restore them with <see cref="NetworkScannerViewModel.ApplySettings"/>.
/// The defaults are the defaults of a new <see cref="NetworkScannerViewModel"/>.
/// </remarks>
public sealed class NetworkScannerSettings
{
    private static readonly IPScannerConfig DefaultConfig = new();

    /// <summary>
    /// Gets or sets the first address of the range.
    /// </summary>
    public string? StartIpAddress { get; set; }

    /// <summary>
    /// Gets or sets the last address of the range.
    /// </summary>
    public string? EndIpAddress { get; set; }

    /// <summary>
    /// Gets or sets the ports to scan.
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Settings are deserialized.")]
    public IList<ushort> PortNumbers { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether each address is pinged.
    /// </summary>
    public bool IcmpPing { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether host names are looked up with reverse DNS.
    /// </summary>
    public bool ResolveHostName { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether NetBIOS names are asked for.
    /// </summary>
    public bool ResolveNetBiosName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether MAC addresses are read.
    /// </summary>
    public bool ResolveMacAddress { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether MAC vendors are looked up.
    /// </summary>
    public bool ResolveVendorFromMacAddress { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether mDNS and SSDP services are discovered.
    /// </summary>
    public bool DiscoverServices { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether ONVIF devices are discovered.
    /// </summary>
    public bool DiscoverOnvif { get; set; }

    /// <summary>
    /// Gets or sets how the open ports are checked for web services.
    /// </summary>
    public IPServicePortExaminationLevel TreatOpenPortsAsWebServices { get; set; } = IPServicePortExaminationLevel.WellKnown;

    /// <summary>
    /// Gets or sets the ping timeout in milliseconds.
    /// </summary>
    public int TimeoutPingInMs { get; set; } = (int)DefaultConfig.TimeoutPing.TotalMilliseconds;

    /// <summary>
    /// Gets or sets the TCP port timeout in milliseconds.
    /// </summary>
    public int TimeoutTcpInMs { get; set; } = (int)DefaultConfig.TimeoutTcp.TotalMilliseconds;

    /// <summary>
    /// Gets or sets the HTTP timeout in milliseconds.
    /// </summary>
    public int TimeoutHttpInMs { get; set; } = (int)DefaultConfig.TimeoutHttp.TotalMilliseconds;

    /// <summary>
    /// Gets or sets how long service discovery listens, in milliseconds.
    /// </summary>
    public int TimeoutDiscoveryInMs { get; set; } = (int)DefaultConfig.TimeoutDiscovery.TotalMilliseconds;

    /// <summary>
    /// Gets or sets the NetBIOS timeout in milliseconds.
    /// </summary>
    public int TimeoutNetBiosInMs { get; set; } = (int)DefaultConfig.TimeoutNetBios.TotalMilliseconds;

    /// <summary>
    /// Gets or sets a value indicating whether only hosts that answered ping are shown.
    /// </summary>
    public bool ShowOnlySuccess { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether only hosts with open ports are shown.
    /// </summary>
    public bool ShowOnlyWithOpenPorts { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether only hosts that answered in any way are shown.
    /// </summary>
    public bool ShowOnlyReachable { get; set; }

    /// <summary>
    /// Gets or sets the names of the hidden columns, as the <c>Show…</c> properties of
    /// <see cref="NetworkScannerColumnsViewModel"/> without the prefix (for example <c>MacVendor</c>).
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Settings are deserialized.")]
    public IList<string> HiddenColumns { get; set; } = [nameof(NetworkScannerColumnsViewModel.ShowWorkgroup)[4..]];
}