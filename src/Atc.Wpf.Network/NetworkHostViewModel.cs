namespace Atc.Wpf.Network;

/// <summary>
/// Represents a single network host discovered during a network scan.
/// </summary>
public partial class NetworkHostViewModel : ViewModelBase
{
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
    [ObservableProperty]
    private IEnumerable<ushort> openPortNumbers = new List<ushort>();

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
        if (ipScanResult.PingStatus is not null)
        {
            PingStatus = ipScanResult.PingStatus;
            PingQualityCategoryToolTip = string.Create(GlobalizationConstants.EnglishCultureInfo, $"{ipScanResult.PingStatus.QualityCategory.GetDescription()} - {ipScanResult.PingStatus.PingInMs}ms");
        }
        else
        {
            PingStatus = new PingStatusResult(ipScanResult.IPAddress, IPStatus.Unknown, 0);
        }

        Hostname = ipScanResult.Hostname;
        MacAddress = ipScanResult.MacAddress;
        MacVendor = ipScanResult.MacVendor;
        OpenPortNumbers = ipScanResult.OpenPortNumbers;
        if (ipScanResult.IsCompleted)
        {
            TimeDiff = ipScanResult.TimeDiff;
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
    /// Gets the open port numbers formatted as a comma-separated list.
    /// </summary>
    /// <value>A comma-separated string of port numbers, or an empty string if no ports are open.</value>
    public string OpenPortNumbersAsCommaList
        => OpenPortNumbers.Any()
            ? string.Join(',', OpenPortNumbers)
            : string.Empty;
}