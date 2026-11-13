namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// Represents a single network host discovered during a network scan.
/// </summary>
public partial class NetworkHostViewModel : ViewModelBase
{
    private const ushort RemoteDesktopPort = 3389;

    private int[] connectTargetsKey = [];

    /// <summary>
    /// Gets or sets the IP address of the network host.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(IpAddressSortable)])]
    private IPAddress ipAddress = IPAddress.None;

    /// <summary>
    /// Gets or sets the ping status result for this host.
    /// </summary>
    [ObservableProperty]
    private PingStatusResult? pingStatus;

    /// <summary>
    /// Gets or sets the tooltip text for the ping quality category icon.
    /// </summary>
    [ObservableProperty]
    private string? pingQualityCategoryToolTip;

    /// <summary>
    /// Gets or sets the hostname of the network host.
    /// </summary>
    [ObservableProperty]
    private string? hostname;

    /// <summary>
    /// Gets or sets the MAC address of the network host.
    /// </summary>
    [ObservableProperty]
    private string? macAddress;

    /// <summary>
    /// Gets or sets the vendor name associated with the MAC address.
    /// </summary>
    [ObservableProperty]
    private string? macVendor;

    /// <summary>
    /// Gets or sets the collection of open port numbers discovered on this host.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(OpenPortNumbersAsCommaList)])]
    private IEnumerable<ushort> openPortNumbers = new List<ushort>();

    /// <summary>
    /// Gets or sets the latest scan result for this host, or <c>null</c> when the host was not created from a scan.
    /// </summary>
    [ObservableProperty]
    private IPScanResult? scanResult;

    /// <summary>
    /// Gets or sets a value indicating whether the host answered in any way: ping, host name, MAC address,
    /// an open port, NetBIOS or service discovery.
    /// </summary>
    [ObservableProperty]
    private bool hasConnection;

    /// <summary>
    /// Gets or sets the best name for the device: the DNS host name, else the mDNS host name,
    /// else the NetBIOS computer name, else the ONVIF name.
    /// </summary>
    [ObservableProperty]
    private string? deviceName;

    /// <summary>
    /// Gets or sets the NetBIOS workgroup or domain.
    /// </summary>
    [ObservableProperty]
    private string? workgroup;

    /// <summary>
    /// Gets or sets the model or description: the ONVIF hardware, else an mDNS model property
    /// (<c>ty</c>, <c>md</c>, <c>model</c>), else the first mDNS instance name.
    /// </summary>
    [ObservableProperty]
    private string? model;

    /// <summary>
    /// Gets or sets the discovered services: the mDNS service types, and <c>UPnP</c> when the host answered SSDP.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(ServicesAsCommaList)])]
    private IReadOnlyList<string> services = [];

    /// <summary>
    /// Gets or sets a value indicating whether the host answered ONVIF discovery.
    /// </summary>
    [ObservableProperty]
    private bool isOnvifDevice;

    /// <summary>
    /// Gets or sets a value indicating whether the host is an ONVIF camera or video encoder.
    /// </summary>
    [ObservableProperty]
    private bool isOnvifCamera;

    /// <summary>
    /// Gets or sets the ONVIF device service URL, to pass to <c>OnvifClient</c>.
    /// </summary>
    [ObservableProperty]
    private Uri? onvifDeviceServiceUrl;

    /// <summary>
    /// Gets or sets the time difference string representing scan duration.
    /// </summary>
    [ObservableProperty]
    private string? timeDiff;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkHostViewModel"/> class with the specified IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address of the network host.</param>
    public NetworkHostViewModel(IPAddress ipAddress)
        => this.ipAddress = ipAddress;

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkHostViewModel"/> class from an IP scan result.
    /// </summary>
    /// <param name="ipScanResult">The IP scan result to populate this view model from.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="ipScanResult"/> is <c>null</c>.</exception>
    public NetworkHostViewModel(IPScanResult ipScanResult)
    {
        ArgumentNullException.ThrowIfNull(ipScanResult);

        IpAddress = ipScanResult.IPAddress;
        Update(ipScanResult);
    }

    /// <summary>
    /// Copies the values of a scan result into this host.
    /// </summary>
    /// <param name="ipScanResult">The latest scan result for this host.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="ipScanResult"/> is <c>null</c>.</exception>
    public void Update(IPScanResult ipScanResult)
    {
        ArgumentNullException.ThrowIfNull(ipScanResult);

        // The scanner reports the same ping result again; only format the tooltip when it is a new one.
        if (ipScanResult.PingStatus is not null)
        {
            if (!ReferenceEquals(PingStatus, ipScanResult.PingStatus))
            {
                PingStatus = ipScanResult.PingStatus;
                PingQualityCategoryToolTip = GetPingQualityCategoryToolTip(ipScanResult.PingStatus);
            }
        }
        else
        {
            PingStatus = new PingStatusResult(ipScanResult.IPAddress, IPStatus.Unknown, 0);
            PingQualityCategoryToolTip = null;
        }

        Hostname = ipScanResult.Hostname;
        MacAddress = ipScanResult.MacAddress;
        MacVendor = ipScanResult.MacVendor;
        HasConnection = ipScanResult.HasConnection;
        UpdateDiscovery(ipScanResult);
        UpdateConnectTargets(ipScanResult);

        // The scanner reports the same result object again as it learns more, so an unchanged
        // reference still means new data for bindings to ScanResult.
        if (ReferenceEquals(ScanResult, ipScanResult))
        {
            RaisePropertyChanged(nameof(ScanResult));
        }
        else
        {
            ScanResult = ipScanResult;
        }

        // A snapshot: IPScanResult.OpenPortNumbers is a query over a bag the scanner keeps filling.
        var ports = ipScanResult.OpenPortNumbers.ToArray();
        if (!ports.SequenceEqual(OpenPortNumbers))
        {
            OpenPortNumbers = ports;
        }

        if (ipScanResult.IsCompleted)
        {
            TimeDiff = ipScanResult.TimeDiff;
        }
    }

    /// <summary>
    /// Gets the web interfaces of the host: the ports the scanner confirmed as HTTP or HTTPS,
    /// else the open ports 80, 443, 8080 and 8443.
    /// </summary>
    public IReadOnlyList<Uri> WebUrls { get; private set; } = [];

    /// <summary>
    /// Gets the first open VNC port (5900 to 5903), or <c>null</c>.
    /// </summary>
    public int? VncPort { get; private set; }

    /// <summary>
    /// Gets a value indicating whether Remote Desktop (port 3389) is open.
    /// </summary>
    public bool IsRemoteDesktopOpen { get; private set; }

    /// <summary>
    /// Gets everything known about the host as text, one value per line, as "Copy all info" copies it.
    /// </summary>
    /// <returns>The text.</returns>
    public string ToAllInfoText()
    {
        var sb = new StringBuilder();
        AppendLine(sb, Resources.Resources.CopyInfoIpAddressFormat1, IpAddress.ToString());
        AppendLine(sb, Resources.Resources.CopyInfoDeviceNameFormat1, DeviceName);
        AppendLine(sb, Resources.Resources.CopyInfoHostnameFormat1, Hostname);
        AppendLine(sb, Resources.Resources.CopyInfoWorkgroupFormat1, Workgroup);
        AppendLine(sb, Resources.Resources.CopyInfoModelFormat1, Model);
        AppendLine(sb, Resources.Resources.CopyInfoMacAddressFormat1, MacAddress);
        AppendLine(sb, Resources.Resources.CopyInfoMacVendorFormat1, MacVendor);
        AppendLine(sb, Resources.Resources.CopyInfoStatusFormat1, PingStatus?.Status.ToString());
        AppendLine(sb, Resources.Resources.CopyInfoOpenPortsFormat1, OpenPortNumbersAsCommaList);
        AppendLine(sb, Resources.Resources.CopyInfoServicesFormat1, ServicesAsCommaList);
        AppendLine(sb, Resources.Resources.CopyInfoOnvifFormat1, OnvifDeviceServiceUrl?.ToString());
        return sb.ToString();
    }

    private static void AppendLine(
        StringBuilder sb,
        string format,
        string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            sb.AppendLine(string.Format(CultureInfo.CurrentCulture, format, value));
        }
    }

    /// <summary>
    /// Gets a sortable string representation of the IP address.
    /// </summary>
    /// <remarks>
    /// For IPv4 addresses, returns zero-padded octets (e.g., "192.168.001.001") for proper lexicographic sorting.
    /// For IPv6 addresses, returns the standard string representation.
    /// </remarks>
    public string IpAddressSortable
    {
        get
        {
            var bytes = IpAddress.GetAddressBytes();
            if (bytes.Length == 4)
            {
                // IPv4: format as zero-padded octets for proper sorting
                return $"{bytes[0]:000}.{bytes[1]:000}.{bytes[2]:000}.{bytes[3]:000}";
            }

            // IPv6: use string representation
            return IpAddress.ToString();
        }
    }

    /// <summary>
    /// Gets the discovered services as a comma-separated list.
    /// </summary>
    public string ServicesAsCommaList
        => string.Join(", ", Services);

    /// <summary>
    /// Gets the open port numbers formatted as a comma-separated list.
    /// </summary>
    /// <value>A comma-separated string of port numbers, or an empty string if no ports are open.</value>
    public string OpenPortNumbersAsCommaList
        => OpenPortNumbers.Any()
            ? string.Join(',', OpenPortNumbers)
            : string.Empty;

    private static string GetPingQualityCategoryToolTip(
        PingStatusResult pingStatus)
    {
        var category = pingStatus.QualityCategory;
        var description = Resources.EnumResources.ResourceManager.GetString(
                              $"{nameof(NetworkQualityCategoryType)}{category}",
                              CultureInfo.CurrentUICulture) ??
                          category.GetDescription();

        return string.Create(CultureInfo.CurrentCulture, $"{description} - {pingStatus.PingInMs}ms");
    }

    private static readonly string[] MdnsModelPropertyNames = ["ty", "md", "model"];

    private static readonly Dictionary<ushort, bool> UsualWebPorts = new()
    {
        [80] = false,
        [443] = true,
        [8080] = false,
        [8443] = true,
    };

    private void UpdateDiscovery(IPScanResult ipScanResult)
    {
        var onvifDevices = ipScanResult.OnvifDevices;
        var mdnsServices = ipScanResult.MdnsServices;

        // Most hosts have no discovery data, and the scanner reports each host several times.
        if (onvifDevices.Count == 0 && mdnsServices.Count == 0 && ipScanResult.SsdpDevices.Count == 0)
        {
            DeviceName = FirstNonEmpty(ipScanResult.Hostname, ipScanResult.MdnsHostName, ipScanResult.NetBiosStatus?.ComputerName);
            Workgroup = ipScanResult.NetBiosStatus?.Workgroup;
            Model = null;
            if (Services.Count > 0)
            {
                Services = [];
            }

            IsOnvifDevice = false;
            IsOnvifCamera = false;
            OnvifDeviceServiceUrl = null;
            return;
        }

        DeviceName = FirstNonEmpty(
            ipScanResult.Hostname,
            ipScanResult.MdnsHostName,
            ipScanResult.NetBiosStatus?.ComputerName,
            onvifDevices.Select(x => x.Name).FirstOrDefault(x => !string.IsNullOrEmpty(x)));
        Workgroup = ipScanResult.NetBiosStatus?.Workgroup;
        Model = FirstNonEmpty(
            onvifDevices.Select(x => x.Hardware).FirstOrDefault(x => !string.IsNullOrEmpty(x)),
            mdnsServices
                .SelectMany(x => MdnsModelPropertyNames.Select(name => x.Properties.GetValueOrDefault(name)))
                .FirstOrDefault(x => !string.IsNullOrEmpty(x)),
            mdnsServices.Select(x => x.InstanceName).FirstOrDefault(x => !string.IsNullOrEmpty(x)));

        var serviceNames = mdnsServices
            .Select(x => TrimLocalSuffix(x.ServiceType))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (ipScanResult.SsdpDevices.Count > 0)
        {
            serviceNames.Add("UPnP");
        }

        if (!serviceNames.SequenceEqual(Services, StringComparer.Ordinal))
        {
            Services = serviceNames;
        }

        IsOnvifDevice = onvifDevices.Count > 0;
        IsOnvifCamera = onvifDevices.Any(x => x.IsNetworkVideoTransmitter);
        OnvifDeviceServiceUrl = onvifDevices.Select(x => x.DeviceServiceUrl).FirstOrDefault(x => x is not null);
    }

    private void UpdateConnectTargets(IPScanResult ipScanResult)
    {
        var openPorts = ipScanResult.Ports
            .Where(x => x.CanConnect && x.TransportProtocol != TransportProtocolType.None)
            .OrderBy(x => x.Port)
            .ToList();

        // The scanner reports the same host many times; only rebuild when its open ports changed.
        var openPortsKey = new int[openPorts.Count];
        for (var i = 0; i < openPorts.Count; i++)
        {
            openPortsKey[i] = (openPorts[i].Port << 8) | (int)openPorts[i].ServiceProtocol;
        }

        if (openPortsKey.AsSpan().SequenceEqual(connectTargetsKey))
        {
            return;
        }

        connectTargetsKey = openPortsKey;

        var webPorts = openPorts
            .Where(x => x.ServiceProtocol is ServiceProtocolType.Http or ServiceProtocolType.Https)
            .Select(x => (x.Port, IsHttps: x.ServiceProtocol == ServiceProtocolType.Https))
            .ToList();
        if (webPorts.Count == 0)
        {
            webPorts = openPorts
                .Where(x => UsualWebPorts.ContainsKey(x.Port))
                .Select(x => (x.Port, IsHttps: UsualWebPorts[x.Port]))
                .ToList();
        }

        WebUrls = webPorts
            .DistinctBy(x => x.Port)
            .OrderBy(x => x.Port)
            .Select(x => new UriBuilder(x.IsHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, IpAddress.ToString(), x.Port).Uri)
            .ToList();
        RaisePropertyChanged(nameof(WebUrls));

        VncPort = openPorts
            .Select(x => (int)x.Port)
            .Where(x => x is >= 5900 and <= 5903)
            .Order()
            .Select(x => (int?)x)
            .FirstOrDefault();
        RaisePropertyChanged(nameof(VncPort));

        IsRemoteDesktopOpen = openPorts.Any(x => x.Port == RemoteDesktopPort);
        RaisePropertyChanged(nameof(IsRemoteDesktopOpen));
    }

    private static string? FirstNonEmpty(params string?[] values)
        => values.FirstOrDefault(x => !string.IsNullOrEmpty(x));

    private static string TrimLocalSuffix(string serviceType)
        => serviceType.EndsWith(".local", StringComparison.OrdinalIgnoreCase)
            ? serviceType[..^".local".Length]
            : serviceType;
}