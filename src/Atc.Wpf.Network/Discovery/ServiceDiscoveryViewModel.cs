namespace Atc.Wpf.Network.Discovery;

/// <summary>
/// ViewModel that finds the devices on the local network that announce themselves with mDNS, SSDP or ONVIF,
/// without scanning an address range.
/// </summary>
public partial class ServiceDiscoveryViewModel : ViewModelBase
{
    private readonly IServiceDiscoverer discoverer;
    private readonly Dictionary<IPAddress, IPScanResult> resultsByIp = [];
    private readonly Dictionary<IPAddress, NetworkHostViewModel> devicesByIp = [];

    /// <summary>
    /// Gets or sets a value indicating whether mDNS / DNS-SD services are found.
    /// </summary>
    [ObservableProperty]
    private bool useMdns = true;

    /// <summary>
    /// Gets or sets a value indicating whether SSDP (UPnP) devices are found.
    /// </summary>
    [ObservableProperty]
    private bool useSsdp = true;

    /// <summary>
    /// Gets or sets a value indicating whether ONVIF devices (IP cameras) are found.
    /// </summary>
    [ObservableProperty]
    private bool useOnvif = true;

    /// <summary>
    /// Gets or sets how long to listen for answers, in milliseconds.
    /// </summary>
    [ObservableProperty]
    private int durationInMs = 3000;

    /// <summary>
    /// Gets or sets the selected device.
    /// </summary>
    [ObservableProperty]
    private NetworkHostViewModel? selectedDevice;

    /// <summary>
    /// Gets or sets the error message of the last discovery.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(HasError)])]
    private string? errorMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceDiscoveryViewModel"/> class that uses Atc.Network's discovery.
    /// </summary>
    public ServiceDiscoveryViewModel()
        : this(new ServiceDiscoverer())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceDiscoveryViewModel"/> class.
    /// </summary>
    /// <param name="discoverer">Finds the devices.</param>
    public ServiceDiscoveryViewModel(IServiceDiscoverer discoverer)
    {
        this.discoverer = discoverer;
        CollectionViewSource
            .GetDefaultView(Devices)
            .SortDescriptions
            .Add(new SortDescription(nameof(NetworkHostViewModel.IpAddressSortable), ListSortDirection.Ascending));
    }

    /// <summary>
    /// Occurs when the first answer from an address arrives and its device is added.
    /// </summary>
    public event EventHandler<NetworkHostEventArgs>? DeviceFound;

    /// <summary>
    /// Occurs when a discovery ends.
    /// </summary>
    public event EventHandler? DiscoveryCompleted;

    /// <summary>
    /// Gets a value indicating whether a protocol failed in the last discovery.
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Gets the devices found, one per address, with the services they announced.
    /// </summary>
    public ObservableCollection<NetworkHostViewModel> Devices { get; } = [];

    private bool CanDiscover()
        => (UseMdns || UseSsdp || UseOnvif) && !IsBusy;

    [RelayCommand(CanExecute = nameof(CanDiscover))]
    private async Task Discover(CancellationToken cancellationToken)
    {
        IsBusy = true;
        ErrorMessage = null;
        SelectedDevice = null;
        Devices.Clear();
        devicesByIp.Clear();
        resultsByIp.Clear();

        var duration = TimeSpan.FromMilliseconds(DurationInMs);
        var errors = new List<string>();
        var protocols = new List<Task>();
        if (UseMdns)
        {
            protocols.Add(Collect("mDNS", discoverer.BrowseMdns(duration, cancellationToken), AddMdns, errors));
        }

        if (UseSsdp)
        {
            protocols.Add(Collect("SSDP", discoverer.SearchSsdp(duration, cancellationToken), AddSsdp, errors));
        }

        if (UseOnvif)
        {
            protocols.Add(Collect("ONVIF", discoverer.ProbeOnvif(duration, cancellationToken), AddOnvif, errors));
        }

        try
        {
            await Task.WhenAll(protocols).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Cancelled by the user; keep what was found.
        }
        finally
        {
            ErrorMessage = errors.Count > 0
                ? string.Join(Environment.NewLine, errors)
                : null;
            IsBusy = false;
        }

        DiscoveryCompleted?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Reads one protocol's answers. The continuations return to the UI thread, so the devices are added there,
    /// while the protocols listen at the same time.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "One failing protocol must not stop the others.")]
    private static async Task Collect<T>(
        string protocol,
        IAsyncEnumerable<T> answers,
        Action<T> add,
        List<string> errors)
    {
        try
        {
            await foreach (var answer in answers.ConfigureAwait(true))
            {
                add(answer);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            errors.Add(string.Format(CultureInfo.CurrentCulture, Resources.Resources.DiscoveryErrorFormat2, protocol, ex.Message));
        }
    }

    private void AddMdns(MdnsServiceInstance service)
    {
        // One row per device: its IPv4 addresses when it has any, as the scanner shows.
        var addresses = service.IPAddresses.Any(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            ? service.IPAddresses.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            : service.IPAddresses;

        foreach (var address in addresses)
        {
            AddAnswer(address, result => result.MdnsServices.Add(service));
        }
    }

    private void AddSsdp(SsdpDevice device)
        => AddAnswer(device.IPAddress, result => result.SsdpDevices.Add(device));

    private void AddOnvif(OnvifDevice device)
        => AddAnswer(device.IPAddress, result => result.OnvifDevices.Add(device));

    private void AddAnswer(
        IPAddress address,
        Action<IPScanResult> attach)
    {
        if (!resultsByIp.TryGetValue(address, out var result))
        {
            result = new IPScanResult(address);
            resultsByIp[address] = result;
        }

        attach(result);

        if (devicesByIp.TryGetValue(address, out var device))
        {
            device.Update(result);
            return;
        }

        device = new NetworkHostViewModel(result);
        devicesByIp[address] = device;
        Devices.Add(device);
        DeviceFound?.Invoke(this, new NetworkHostEventArgs(device, result));
    }
}