// ReSharper disable InvertIf
namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// ViewModel for the network scanner control, providing IP range scanning functionality.
/// </summary>
public partial class NetworkScannerViewModel : ViewModelBase, IDisposable
{
    private const string ColumnPropertyPrefix = "Show";

    private static readonly PropertyInfo[] ColumnVisibilityProperties = typeof(NetworkScannerColumnsViewModel)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(x => x.PropertyType == typeof(bool) &&
                    x.CanWrite &&
                    x.Name.StartsWith(ColumnPropertyPrefix, StringComparison.Ordinal))
        .ToArray();

    private readonly ICollectionView view;
    private readonly Dispatcher dispatcher;
    private readonly ConcurrentQueue<PendingProgress> pendingProgress = new();
    private readonly Dictionary<IPAddress, NetworkHostViewModel> entriesByIp = [];
    private readonly Dictionary<IPAddress, NetworkKnownHost> knownHosts = [];
    private GridViewColumnHeader? lastHeaderClicked;
    private ListSortDirection lastDirection = ListSortDirection.Ascending;
    private DispatcherTimer? elapsedTimer;
    private Stopwatch? stopwatch;
    private int totalIpCount;
    private int progressDrainScheduled;
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Owned and disposed by the running Scan; Dispose only cancels it.")]
    private CancellationTokenSource? scanCancellation;
    private bool disposed;
    private Func<NetworkHostViewModel, bool>? hostFilter;
    private string? rangeCidr;
    private List<(IPAddress Start, IPAddress End)> cidrRanges = [];
    private bool defaultToFirstLocalNetwork;
    private IReadOnlyList<(IPAddress Start, IPAddress End)> scannedRanges = [];
    private bool isApplyingPendingProgress;
    private string? selectedLocalNetworkKey;

    /// <summary>
    /// Gets or sets the currently selected network host entry.
    /// </summary>
    [ObservableProperty(AfterChangedCallback = $"{nameof(EntrySelected)}?.Invoke(this, new {nameof(NetworkHostSelectedEventArgs)}({nameof(selectedEntry)}));")]
    private NetworkHostViewModel? selectedEntry;

    /// <summary>
    /// Gets or sets the column visibility settings.
    /// </summary>
    [ObservableProperty]
    private NetworkScannerColumnsViewModel columns;

    /// <summary>
    /// Gets or sets the filter settings.
    /// </summary>
    [ObservableProperty]
    private NetworkScannerFilterViewModel filter;

    /// <summary>
    /// Gets or sets what the scanner checks on each address, and how long it waits.
    /// </summary>
    [ObservableProperty]
    private NetworkScannerOptionsViewModel options = new();

    /// <summary>
    /// Gets or sets the maximum value for the busy indicator progress bar.
    /// </summary>
    [ObservableProperty]
    private int busyIndicatorMaximumValue;

    /// <summary>
    /// Gets or sets the current value for the busy indicator progress bar.
    /// </summary>
    [ObservableProperty]
    private int busyIndicatorCurrentValue;

    /// <summary>
    /// Gets or sets the percentage value text for the busy indicator.
    /// </summary>
    [ObservableProperty]
    private string busyIndicatorPercentageValue = string.Empty;

    /// <summary>
    /// Gets or sets the completion time display string.
    /// </summary>
    [ObservableProperty]
    private string? completionTime;

    /// <summary>
    /// Gets or sets the starting IP address for the scan range.
    /// </summary>
    [ObservableProperty]
    private string? startIpAddress;

    /// <summary>
    /// Gets or sets the ending IP address for the scan range.
    /// </summary>
    [ObservableProperty]
    private string? endIpAddress;

    /// <summary>
    /// Gets or sets the list of port numbers to scan.
    /// </summary>
    [ObservableProperty(DependentPropertyNames = [nameof(PortsNumbersText)])]
    private List<ushort> portsNumbers = [];

    /// <summary>
    /// Gets or sets the selected port preset; selecting one sets <see cref="PortsNumbers"/>.
    /// </summary>
    [ObservableProperty(
        DependentPropertyNames = [nameof(SelectedPortPresetKey)],
        AfterChangedCallback = nameof(ApplySelectedPortPreset))]
    private NetworkPortPreset? selectedPortPreset;

    /// <summary>
    /// Gets or sets the entry count information display string.
    /// </summary>
    [ObservableProperty]
    private string entryCountInfo = "0 / 0";

    /// <summary>
    /// Gets or sets the error message from the last scan operation.
    /// </summary>
    /// <value>The error message, or <c>null</c> if no error occurred.</value>
    [ObservableProperty(DependentPropertyNames = [nameof(HasError)])]
    private string? errorMessage;

    /// <summary>
    /// Gets a value indicating whether there is an error message to display.
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkScannerViewModel"/> class.
    /// </summary>
    public NetworkScannerViewModel()
    {
        dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        Entries = [];
        columns = new NetworkScannerColumnsViewModel();
        filter = new NetworkScannerFilterViewModel();
        ScannerFactory = CreateDefaultScanner;
        PortPresets.CollectionChanged += (_, _) => RaisePropertyChanged(nameof(PortPresetItems));
        LocalNetworks.CollectionChanged += (_, _) => RaisePropertyChanged(nameof(LocalNetworkItems));
        foreach (var localNetwork in GetLocalNetworksOrEmpty())
        {
            LocalNetworks.Add(localNetwork);
        }

        filter.PropertyChanged += OnFilterPropertyChanged;
        view = CollectionViewSource.GetDefaultView(Entries);
        view.Filter = IsEntryVisible;

        // Re-evaluate the filter for a single entry when the properties it depends on change,
        // instead of resetting the whole view on every scanner progress report.
        if (view is ICollectionViewLiveShaping { CanChangeLiveFiltering: true } liveShaping)
        {
            liveShaping.LiveFilteringProperties.Add(nameof(NetworkHostViewModel.PingStatus));
            liveShaping.LiveFilteringProperties.Add(nameof(NetworkHostViewModel.OpenPortNumbers));
            liveShaping.LiveFilteringProperties.Add(nameof(NetworkHostViewModel.HasConnection));
            liveShaping.LiveFilteringProperties.Add(nameof(NetworkHostViewModel.DeviceName));
            liveShaping.LiveFilteringProperties.Add(nameof(NetworkHostViewModel.Model));
            liveShaping.LiveFilteringProperties.Add(nameof(NetworkHostViewModel.Services));
            liveShaping.LiveFilteringProperties.Add(nameof(NetworkHostViewModel.IsOnvifDevice));
            liveShaping.IsLiveFiltering = true;
        }

        // Live filtering applies its changes asynchronously; keep the visible count in step with the view.
        ((INotifyCollectionChanged)view).CollectionChanged += (_, _) =>
        {
            // While a batch of reports is applied, the count is updated once at the end instead.
            if (!isApplyingPendingProgress)
            {
                UpdateEntryCountInfo();
            }
        };

        if (XamlToolkit.Helpers.DesignModeHelper.IsInDesignMode)
        {
            Entries.AddRange(DesignModeHelper.CreateNetworkHostInfoList());
        }
    }

    /// <summary>
    /// Occurs when a network host entry is selected.
    /// </summary>
    public event EventHandler<NetworkHostSelectedEventArgs>? EntrySelected;

    /// <summary>
    /// Gets or sets a callback that adjusts the scanner configuration after <see cref="Options"/> is applied,
    /// for settings the options don't cover.
    /// </summary>
    public Action<IPScannerConfig>? ConfigureScanner { get; set; }

    /// <summary>
    /// Gets or sets the factory that creates the scanner for each scan.
    /// </summary>
    /// <remarks>
    /// By default an <see cref="IPScanner"/> with <see cref="Logger"/> and <see cref="MacAddressVendorLookup"/>.
    /// The scanner is disposed after the scan when it implements <see cref="IDisposable"/>.
    /// </remarks>
    public Func<IPScannerConfig, IIPScanner> ScannerFactory { get; set; }

    /// <summary>
    /// Gets or sets the logger the default scanner writes to.
    /// </summary>
    public ILogger? Logger { get; set; }

    /// <summary>
    /// Gets or sets the MAC address vendor lookup the default scanner uses, such as
    /// <see cref="IeeeOuiMacAddressVendorLookup"/> to look up vendors offline.
    /// </summary>
    /// <remarks>
    /// When <c>null</c>, vendors are looked up online at macvendors.com.
    /// </remarks>
    public IMacAddressVendorLookup? MacAddressVendorLookup { get; set; }

    /// <summary>
    /// Creates the scanner configuration for the next scan from <see cref="Options"/>,
    /// <see cref="PortsNumbers"/> and <see cref="ConfigureScanner"/>.
    /// </summary>
    /// <returns>The configuration.</returns>
    public IPScannerConfig CreateScannerConfig()
    {
        var config = new IPScannerConfig(Options.TreatOpenPortsAsWebServices)
        {
            IcmpPing = Options.IcmpPing,
            ResolveHostName = Options.ResolveHostName,
            ResolveNetBiosName = Options.ResolveNetBiosName,
            ResolveMacAddress = Options.ResolveMacAddress,
            ResolveVendorFromMacAddress = Options.ResolveVendorFromMacAddress,
            DiscoverServices = Options.DiscoverServices,
            DiscoverOnvif = Options.DiscoverOnvif,
            TreatOpenPortsAsWebServices = Options.TreatOpenPortsAsWebServices,
            TimeoutPing = TimeSpan.FromMilliseconds(Options.TimeoutPingInMs),
            TimeoutTcp = TimeSpan.FromMilliseconds(Options.TimeoutTcpInMs),
            TimeoutHttp = TimeSpan.FromMilliseconds(Options.TimeoutHttpInMs),
            TimeoutDiscovery = TimeSpan.FromMilliseconds(Options.TimeoutDiscoveryInMs),
            TimeoutNetBios = TimeSpan.FromMilliseconds(Options.TimeoutNetBiosInMs),
        };

        if (PortsNumbers.Count > 0)
        {
            config.PortNumbers = PortsNumbers;
        }

        ConfigureScanner?.Invoke(config);
        return config;
    }

    private IIPScanner CreateDefaultScanner(IPScannerConfig config)
    {
        var logger = Logger ?? NullLogger.Instance;
        return MacAddressVendorLookup is null
            ? new IPScanner(logger, config)
            : new IPScanner(logger, config, MacAddressVendorLookup);
    }

    /// <summary>
    /// Occurs when the first report for an address arrives and its row is added.
    /// </summary>
    public event EventHandler<NetworkHostEventArgs>? HostFound;

    /// <summary>
    /// Occurs when a later report for an address arrives and its row is updated.
    /// </summary>
    public event EventHandler<NetworkHostEventArgs>? HostUpdated;

    /// <summary>
    /// Occurs when the scanner is done with an address.
    /// </summary>
    public event EventHandler<NetworkHostEventArgs>? HostCompleted;

    /// <summary>
    /// Occurs when a scan starts.
    /// </summary>
    public event EventHandler? ScanStarted;

    /// <summary>
    /// Occurs when a scan ends, whether it completed, was cancelled or failed.
    /// </summary>
    public event EventHandler<NetworkScanCompletedEventArgs>? ScanCompleted;

    /// <summary>
    /// Occurs when the user asks to connect to the selected host with VNC; the app opens the viewer.
    /// </summary>
    public event EventHandler<NetworkHostConnectEventArgs>? VncRequested;

    /// <summary>
    /// Occurs when the user asks to trace the route to the selected host; the app opens a <c>TracerouteView</c>.
    /// </summary>
    public event EventHandler<NetworkHostSelectedEventArgs>? TracerouteRequested;

    /// <summary>
    /// Gets or sets a value indicating whether the Remote Desktop action is offered for hosts with port 3389 open.
    /// </summary>
    public bool AllowRemoteDesktop { get; set; }

    /// <summary>
    /// Gets or sets how Remote Desktop is started for an address; by default <c>mstsc /v:address</c>.
    /// </summary>
    public Action<IPAddress> RemoteDesktopLauncher { get; set; } = StartRemoteDesktop;

    /// <summary>
    /// Gets the hosts earlier scans found with a MAC address that stopped answering in a later scan of their range,
    /// so they can be woken with Wake-on-LAN.
    /// </summary>
    public ObservableCollection<NetworkKnownHost> OfflineHosts { get; } = [];

    /// <summary>
    /// Gets or sets how a web interface is opened; by default in the default browser.
    /// </summary>
    public Action<Uri> OpenUrl { get; set; } = OpenInDefaultBrowser;

    /// <summary>
    /// Gets or sets how a Wake-on-LAN packet is sent to a MAC address; by default as a broadcast on port 9.
    /// </summary>
    public Func<string, CancellationToken, Task> WakeOnLanSender { get; set; } = (macAddress, cancellationToken)
        => WakeOnLanHelper.SendMagicPacket(macAddress, cancellationToken: cancellationToken);

    /// <summary>
    /// Gets or sets an extra filter for the rows: hosts it returns <c>false</c> for are hidden.
    /// </summary>
    public Func<NetworkHostViewModel, bool>? HostFilter
    {
        get => hostFilter;
        set
        {
            hostFilter = value;
            FilterChange();
        }
    }

    /// <summary>
    /// Scans the range from <see cref="StartIpAddress"/> to <see cref="EndIpAddress"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when the scan has ended and every report is applied.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-facing error handling requires catching all exceptions.")]
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "Scan operation requires sequential setup, execution, and cleanup.")]
    public async Task ScanAsync(CancellationToken cancellationToken)
    {
        IsBusy = true;
        ErrorMessage = null;
        SelectedEntry = null;
        Entries.Clear();
        entriesByIp.Clear();

        BusyIndicatorMaximumValue = 0;
        BusyIndicatorCurrentValue = 0;
        BusyIndicatorPercentageValue = string.Empty;
        CompletionTime = null;

        IIPScanner? ipScanner = null;
        IPScanResults? ipScanResults = null;
        var isCancelled = false;

        // Linked to the command's token, so Dispose can stop a scan that is still running.
        using var scanCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        scanCancellation = scanCts;

        try
        {
            // Inside the try, so an invalid range ends up in ErrorMessage and releases IsBusy like every
            // other failure instead of escaping the command.
            var ranges = GetRangesToScan();
            scannedRanges = ranges;
            if (ranges.Count == 0)
            {
                // Reports the invalid address the same way as a failing scan.
                _ = IPAddress.Parse(StartIpAddress!);
                _ = IPAddress.Parse(EndIpAddress!);
                throw new FormatException($"{StartIpAddress} - {EndIpAddress}");
            }

            totalIpCount = ranges.Sum(x => x.Start.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
                ? 1
                : IPv4AddressHelper.GetAddressesInRange(x.Start, x.End).Count);
            UpdateEntryCountInfo();

            StartElapsedTimer();
            ScanStarted?.Invoke(this, EventArgs.Empty);

            // Several ranges are scanned one after another into the same list.
            var combinedResults = ranges.Count > 1 ? new IPScanResults() : null;
            foreach (var (startIp, endIp) in ranges)
            {
                ipScanner = ScannerFactory(CreateScannerConfig());
                ipScanner.ProgressReporting += IpScannerOnProgressReporting;

                ipScanResults = startIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
                    ? await ipScanner.Scan(startIp, scanCts.Token).ConfigureAwait(true)
                    : await ipScanner.ScanRange(startIp, endIp, scanCts.Token).ConfigureAwait(true);

                ipScanner.ProgressReporting -= IpScannerOnProgressReporting;
                (ipScanner as IDisposable)?.Dispose();
                ipScanner = null;

                if (combinedResults is not null)
                {
                    foreach (var result in ipScanResults.CollectedResults)
                    {
                        combinedResults.CollectedResults.Add(result);
                    }
                }
            }

            if (combinedResults is not null)
            {
                combinedResults.End = DateTime.Now;
                ipScanResults = combinedResults;
            }

            if (ipScanResults!.End.HasValue)
            {
                CompletionTime = ipScanResults.TimeDiff;
            }
        }
        catch (OperationCanceledException)
        {
            isCancelled = true;
            CompletionTime = null;
        }
        catch (ArgumentException ex)
        {
            ErrorMessage = string.Format(
                CultureInfo.CurrentCulture,
                Resources.Resources.InvalidIpRangeFormat1,
                ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            ErrorMessage = Resources.Resources.InsufficientPermissions;
        }
        catch (System.Net.Sockets.SocketException ex)
        {
            ErrorMessage = string.Format(
                CultureInfo.CurrentCulture,
                Resources.Resources.NetworkErrorFormat1,
                ex.Message);
        }
        catch (Exception ex)
        {
            ErrorMessage = string.Format(
                CultureInfo.CurrentCulture,
                Resources.Resources.ScanErrorFormat1,
                ex.Message);
        }
        finally
        {
            StopElapsedTimer();
            if (ipScanner is not null)
            {
                ipScanner.ProgressReporting -= IpScannerOnProgressReporting;
            }

            (ipScanner as IDisposable)?.Dispose();
            scanCancellation = null;

            // Reports still queued for the dispatcher are applied now, so the rows are complete when the scan ends.
            if (!pendingProgress.IsEmpty)
            {
                ApplyPendingProgress();
            }

            if (ipScanResults is not null)
            {
                ApplyFinalResults(ipScanResults);
                if (!isCancelled)
                {
                    UpdateOfflineHosts(scannedRanges);
                }
            }

            IsBusy = false;
        }

        ScanCompleted?.Invoke(this, new NetworkScanCompletedEventArgs(ipScanResults, isCancelled, ErrorMessage));
    }

    /// <summary>
    /// Gets or sets the scan range in CIDR notation, such as <c>192.168.1.0/24</c>; a valid value sets
    /// <see cref="StartIpAddress"/> and <see cref="EndIpAddress"/>.
    /// </summary>
    public string? RangeCidr
    {
        get => rangeCidr;
        set
        {
            rangeCidr = value;
            RaisePropertyChanged();

            var parts = (value ?? string.Empty).Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var ranges = new List<(IPAddress Start, IPAddress End)>();
            foreach (var part in parts)
            {
                if (!TryParseIpv4Cidr(part, out var address, out var prefixLength))
                {
                    return;
                }

                ranges.Add(IPv4AddressHelper.GetFirstAndLastAddressInRange(address, prefixLength));
            }

            if (ranges.Count == 0)
            {
                return;
            }

            cidrRanges = ranges;
            StartIpAddress = ranges[0].Start.ToString();
            EndIpAddress = ranges[0].End.ToString();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether an empty range is filled with the first physical local network.
    /// </summary>
    public bool DefaultToFirstLocalNetwork
    {
        get => defaultToFirstLocalNetwork;
        set
        {
            defaultToFirstLocalNetwork = value;
            if (value &&
                string.IsNullOrWhiteSpace(StartIpAddress) &&
                string.IsNullOrWhiteSpace(EndIpAddress))
            {
                var network = LocalNetworks.FirstOrDefault(x => !x.IsVirtual) ?? LocalNetworks.FirstOrDefault();
                if (network is not null)
                {
                    SelectedLocalNetworkKey = network.Cidr;
                }
            }
        }
    }

    /// <summary>
    /// Gets the ranges the next scan covers: every range of a <see cref="RangeCidr"/> with several ranges
    /// (while the start and end address are still its first range), else the start to the end address.
    /// </summary>
    /// <returns>The ranges.</returns>
    public IReadOnlyList<(IPAddress Start, IPAddress End)> GetRangesToScan()
    {
        if (cidrRanges.Count > 1 &&
            string.Equals(StartIpAddress, cidrRanges[0].Start.ToString(), StringComparison.Ordinal) &&
            string.Equals(EndIpAddress, cidrRanges[0].End.ToString(), StringComparison.Ordinal))
        {
            return cidrRanges;
        }

        if (IsValidIpv4Range(StartIpAddress, EndIpAddress))
        {
            return [(IPAddress.Parse(StartIpAddress!), IPAddress.Parse(EndIpAddress!))];
        }

        if (IsSingleIpv6Address(StartIpAddress, EndIpAddress))
        {
            var address = IPAddress.Parse(StartIpAddress!);
            return [(address, address)];
        }

        return [];
    }

    /// <summary>
    /// Gets the networks of this machine's network adapters.
    /// </summary>
    public ObservableCollection<LocalNetwork> LocalNetworks { get; } = [];

    /// <summary>
    /// Gets the local networks as key (CIDR) and name, for <c>LabelComboBox</c>.
    /// </summary>
    public Dictionary<string, string> LocalNetworkItems
        => LocalNetworks.ToDictionary(x => x.Cidr, x => x.ToString(), StringComparer.Ordinal);

    /// <summary>
    /// Gets or sets the CIDR of the selected local network; selecting one sets the range.
    /// </summary>
    public string? SelectedLocalNetworkKey
    {
        get => selectedLocalNetworkKey;
        set
        {
            selectedLocalNetworkKey = value;
            RaisePropertyChanged();
            var network = LocalNetworks.FirstOrDefault(x => string.Equals(x.Cidr, value, StringComparison.Ordinal));
            if (network is not null)
            {
                StartIpAddress = network.StartIpAddress.ToString();
                EndIpAddress = network.EndIpAddress.ToString();
            }
        }
    }

    /// <summary>
    /// Gets the rows the filters show, in the shown order.
    /// </summary>
    /// <returns>The visible rows.</returns>
    public IReadOnlyList<NetworkHostViewModel> GetVisibleEntries()
        => view.Cast<NetworkHostViewModel>().ToList();

    /// <summary>
    /// Gets the current settings, to store them.
    /// </summary>
    /// <returns>The settings.</returns>
    public NetworkScannerSettings GetSettings()
        => new()
        {
            StartIpAddress = StartIpAddress,
            EndIpAddress = EndIpAddress,
            PortNumbers = [.. PortsNumbers],
            IcmpPing = Options.IcmpPing,
            ResolveHostName = Options.ResolveHostName,
            ResolveNetBiosName = Options.ResolveNetBiosName,
            ResolveMacAddress = Options.ResolveMacAddress,
            ResolveVendorFromMacAddress = Options.ResolveVendorFromMacAddress,
            DiscoverServices = Options.DiscoverServices,
            DiscoverOnvif = Options.DiscoverOnvif,
            TreatOpenPortsAsWebServices = Options.TreatOpenPortsAsWebServices,
            TimeoutPingInMs = Options.TimeoutPingInMs,
            TimeoutTcpInMs = Options.TimeoutTcpInMs,
            TimeoutHttpInMs = Options.TimeoutHttpInMs,
            TimeoutDiscoveryInMs = Options.TimeoutDiscoveryInMs,
            TimeoutNetBiosInMs = Options.TimeoutNetBiosInMs,
            ShowOnlySuccess = Filter.ShowOnlySuccess,
            ShowOnlyWithOpenPorts = Filter.ShowOnlyWithOpenPorts,
            ShowOnlyReachable = Filter.ShowOnlyReachable,
            HiddenColumns = ColumnVisibilityProperties
                .Where(x => !(bool)x.GetValue(Columns)!)
                .Select(x => x.Name[ColumnPropertyPrefix.Length..])
                .ToList(),
        };

    /// <summary>
    /// Restores stored settings.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public void ApplySettings(NetworkScannerSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        StartIpAddress = settings.StartIpAddress;
        EndIpAddress = settings.EndIpAddress;
        PortsNumbers = [.. settings.PortNumbers];
        Options.IcmpPing = settings.IcmpPing;
        Options.ResolveHostName = settings.ResolveHostName;
        Options.ResolveNetBiosName = settings.ResolveNetBiosName;
        Options.ResolveMacAddress = settings.ResolveMacAddress;
        Options.ResolveVendorFromMacAddress = settings.ResolveVendorFromMacAddress;
        Options.DiscoverServices = settings.DiscoverServices;
        Options.DiscoverOnvif = settings.DiscoverOnvif;
        Options.TreatOpenPortsAsWebServices = settings.TreatOpenPortsAsWebServices;
        Options.TimeoutPingInMs = settings.TimeoutPingInMs;
        Options.TimeoutTcpInMs = settings.TimeoutTcpInMs;
        Options.TimeoutHttpInMs = settings.TimeoutHttpInMs;
        Options.TimeoutDiscoveryInMs = settings.TimeoutDiscoveryInMs;
        Options.TimeoutNetBiosInMs = settings.TimeoutNetBiosInMs;
        Filter.ShowOnlySuccess = settings.ShowOnlySuccess;
        Filter.ShowOnlyWithOpenPorts = settings.ShowOnlyWithOpenPorts;
        Filter.ShowOnlyReachable = settings.ShowOnlyReachable;

        var hiddenColumns = new HashSet<string>(settings.HiddenColumns, StringComparer.Ordinal);
        foreach (var property in ColumnVisibilityProperties)
        {
            property.SetValue(Columns, !hiddenColumns.Contains(property.Name[ColumnPropertyPrefix.Length..]));
        }
    }

    /// <summary>
    /// Gets the port presets; add your own to offer them in the settings view.
    /// </summary>
    public ObservableCollection<NetworkPortPreset> PortPresets { get; } = [.. NetworkPortPreset.CreateDefaults()];

    /// <summary>
    /// Gets the port presets as key and name, for <c>LabelComboBox</c>.
    /// </summary>
    public Dictionary<string, string> PortPresetItems
        => PortPresets.ToDictionary(x => x.Key, x => x.Name, StringComparer.Ordinal);

    /// <summary>
    /// Gets or sets the key of the selected port preset.
    /// </summary>
    public string? SelectedPortPresetKey
    {
        get => SelectedPortPreset?.Key;
        set => SelectedPortPreset = PortPresets.FirstOrDefault(x => string.Equals(x.Key, value, StringComparison.Ordinal));
    }

    /// <summary>
    /// Gets the collection of discovered network host entries.
    /// </summary>
    public ObservableCollectionEx<NetworkHostViewModel> Entries { get; }

    /// <summary>
    /// Gets or sets the port numbers as a comma-separated text string.
    /// </summary>
    /// <remarks>
    /// Accepts comma, semicolon, or space-separated port numbers.
    /// Invalid values are silently ignored during parsing.
    /// </remarks>
    public string PortsNumbersText
    {
        get => string.Join(", ", PortsNumbers);
        set
        {
            var ports = new List<ushort>();
            if (!string.IsNullOrWhiteSpace(value))
            {
                var parts = value.Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    if (ushort.TryParse(part.Trim(), out var port))
                    {
                        ports.Add(port);
                    }
                }
            }

            PortsNumbers = ports;
        }
    }

    private void ApplySelectedPortPreset()
    {
        if (SelectedPortPreset is not null)
        {
            PortsNumbers = [.. SelectedPortPreset.Ports];
        }
    }

    [RelayCommand]
    private void FilterChange()
    {
        view.Refresh();
        UpdateEntryCountInfo();
    }

    private bool IsEntryVisible(object item)
    {
        if (item is not NetworkHostViewModel entry)
        {
            return true;
        }

        if (filter.ShowOnlySuccess &&
            entry.PingStatus?.Status != IPStatus.Success)
        {
            return false;
        }

        if (filter.ShowOnlyWithOpenPorts &&
            !entry.OpenPortNumbers.Any())
        {
            return false;
        }

        if (filter.ShowOnlyReachable &&
            !entry.HasConnection)
        {
            return false;
        }

        return hostFilter?.Invoke(entry) ?? true;
    }

    [RelayCommand]
    private void Clear()
    {
        SelectedEntry = null;
        Entries.Clear();
        entriesByIp.Clear();
        totalIpCount = 0;
        UpdateEntryCountInfo();
    }

    private bool CanCopy()
        => SelectedEntry is not null;

    [RelayCommand(CanExecute = nameof(CanCopy))]
    private void CopyIpAddress()
    {
        if (SelectedEntry is not null)
        {
            System.Windows.Clipboard.SetText(SelectedEntry.IpAddress.ToString());
        }
    }

    [RelayCommand(CanExecute = nameof(CanCopy))]
    private void CopyHostname()
    {
        if (SelectedEntry?.Hostname is not null)
        {
            System.Windows.Clipboard.SetText(SelectedEntry.Hostname);
        }
    }

    [RelayCommand(CanExecute = nameof(CanCopy))]
    private void CopyMacAddress()
    {
        if (SelectedEntry?.MacAddress is not null)
        {
            System.Windows.Clipboard.SetText(SelectedEntry.MacAddress);
        }
    }

    [RelayCommand(CanExecute = nameof(CanCopy))]
    private void CopyAllInfo()
    {
        if (SelectedEntry is not null)
        {
            System.Windows.Clipboard.SetText(SelectedEntry.ToAllInfoText());
        }
    }

    private bool CanOpenInBrowser()
        => SelectedEntry?.WebUrls.Count > 0;

    [RelayCommand(CanExecute = nameof(CanOpenInBrowser))]
    private void OpenInBrowser()
        => OpenUrl(SelectedEntry!.WebUrls[0]);

    private bool CanWakeOnLan()
        => !string.IsNullOrEmpty(SelectedEntry?.MacAddress);

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-facing error handling requires catching all exceptions.")]
    [RelayCommand(CanExecute = nameof(CanWakeOnLan))]
    private async Task WakeOnLan(CancellationToken cancellationToken)
    {
        try
        {
            await WakeOnLanSender(SelectedEntry!.MacAddress!, cancellationToken).ConfigureAwait(true);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            ErrorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Resources.NetworkErrorFormat1, ex.Message);
        }
    }

    private bool CanConnectWithVnc()
        => SelectedEntry?.VncPort is not null;

    [RelayCommand(CanExecute = nameof(CanConnectWithVnc))]
    private void ConnectWithVnc()
        => VncRequested?.Invoke(this, new NetworkHostConnectEventArgs(SelectedEntry!, SelectedEntry!.VncPort!.Value));

    private bool CanRescanHost()
        => SelectedEntry is not null && !IsBusy;

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-facing error handling requires catching all exceptions.")]
    [RelayCommand(CanExecute = nameof(CanRescanHost))]
    private async Task RescanHost(CancellationToken cancellationToken)
    {
        var host = SelectedEntry!;
        var scanner = ScannerFactory(CreateScannerConfig());
        try
        {
            var results = await scanner.Scan(host.IpAddress, cancellationToken).ConfigureAwait(true);
            var result = results.CollectedResults.FirstOrDefault(x => x.IPAddress.Equals(host.IpAddress));
            if (result is not null)
            {
                host.Update(result);
                HostUpdated?.Invoke(this, new NetworkHostEventArgs(host, result));
                HostCompleted?.Invoke(this, new NetworkHostEventArgs(host, result));
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            ErrorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Resources.ScanErrorFormat1, ex.Message);
        }
        finally
        {
            (scanner as IDisposable)?.Dispose();
        }
    }

    private static void OpenInDefaultBrowser(Uri uri)
        => Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true })?.Dispose();

    [RelayCommand(CanExecute = nameof(CanCopy))]
    private void Traceroute()
        => TracerouteRequested?.Invoke(this, new NetworkHostSelectedEventArgs(SelectedEntry));

    private bool CanConnectWithRemoteDesktop()
        => AllowRemoteDesktop && SelectedEntry?.IsRemoteDesktopOpen == true;

    [RelayCommand(CanExecute = nameof(CanConnectWithRemoteDesktop))]
    private void ConnectWithRemoteDesktop()
        => RemoteDesktopLauncher(SelectedEntry!.IpAddress);

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-facing error handling requires catching all exceptions.")]
    [RelayCommand]
    private async Task WakeOfflineHost(
        NetworkKnownHost? host,
        CancellationToken cancellationToken)
    {
        if (host is null)
        {
            return;
        }

        try
        {
            await WakeOnLanSender(host.MacAddress, cancellationToken).ConfigureAwait(true);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            ErrorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Resources.NetworkErrorFormat1, ex.Message);
        }
    }

    /// <summary>
    /// Remembers the hosts that answered with a MAC address, and lists the remembered hosts in the scanned ranges
    /// that didn't answer this time.
    /// </summary>
    private void UpdateOfflineHosts(
        IReadOnlyList<(IPAddress Start, IPAddress End)> ranges)
    {
        var now = DateTime.Now;
        foreach (var host in Entries)
        {
            if (host.HasConnection && !string.IsNullOrEmpty(host.MacAddress))
            {
                knownHosts[host.IpAddress] = new NetworkKnownHost(host.IpAddress, host.MacAddress, host.DeviceName, now);
            }
        }

        foreach (var known in knownHosts.Values)
        {
            var isAnswering = entriesByIp.TryGetValue(known.IpAddress, out var host) && host.HasConnection;
            var offline = OfflineHosts.FirstOrDefault(x => x.IpAddress.Equals(known.IpAddress));
            if (isAnswering)
            {
                if (offline is not null)
                {
                    OfflineHosts.Remove(offline);
                }
            }
            else if (offline is null && ranges.Any(x => IsInRange(known.IpAddress, x.Start, x.End)))
            {
                OfflineHosts.Add(known);
            }
        }
    }

    private static bool IsInRange(
        IPAddress address,
        IPAddress start,
        IPAddress end)
        => address.AddressFamily == start.AddressFamily &&
           (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
               ? ToUInt32(address) >= ToUInt32(start) && ToUInt32(address) <= ToUInt32(end)
               : address.Equals(start));

    private static void StartRemoteDesktop(IPAddress address)
        => Process.Start(new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "mstsc.exe"), $"/v:{address}"))?.Dispose();

    private bool CanExport()
        => Entries.Count > 0;

    [RelayCommand(CanExecute = nameof(CanExport))]
    private void CopyAllEntries()
        => System.Windows.Clipboard.SetText(NetworkScanExport.ToCsv(GetVisibleEntries()));

    [RelayCommand(CanExecute = nameof(CanExport))]
    private void Export()
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = "network-scan",
            DefaultExt = ".csv",
            Filter = "CSV (*.csv)|*.csv|JSON (*.json)|*.json",
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var content = dialog.FilterIndex == 2
            ? NetworkScanExport.ToJson(GetVisibleEntries())
            : NetworkScanExport.ToCsv(GetVisibleEntries());
        File.WriteAllText(dialog.FileName, content, Encoding.UTF8);
    }

    private bool CanScan()
        => IsValidIpv4Range(StartIpAddress, EndIpAddress) ||
           IsSingleIpv6Address(StartIpAddress, EndIpAddress);

    /// <summary>
    /// A single IPv6 address can be scanned (ranges are IPv4 only): the start address, with no end address
    /// or the same one.
    /// </summary>
    internal static bool IsSingleIpv6Address(
        string? start,
        string? end)
        => IPAddress.TryParse(start, out var startAddress) &&
           startAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 &&
           (string.IsNullOrWhiteSpace(end) ||
            (IPAddress.TryParse(end, out var endAddress) && endAddress.Equals(startAddress)));

    private static bool TryParseIpv4Cidr(
        string? text,
        out IPAddress address,
        out int prefixLength)
    {
        address = IPAddress.None;
        prefixLength = 0;
        var parts = text?.Split('/', StringSplitOptions.TrimEntries);
        return parts is [var addressText, var prefixText] &&
               TryParseIpv4(addressText, out address) &&
               int.TryParse(prefixText, NumberStyles.None, CultureInfo.InvariantCulture, out prefixLength) &&
               prefixLength <= 32;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Without readable adapters the picker is just empty.")]
    private static IReadOnlyList<LocalNetwork> GetLocalNetworksOrEmpty()
    {
        try
        {
            return LocalNetwork.GetAll();
        }
        catch (Exception)
        {
            return [];
        }
    }

    /// <summary>
    /// The scanner only handles IPv4 ranges whose start is not above the end; anything else makes
    /// <c>GetAddressesInRange</c> throw, so it must never reach the scan.
    /// </summary>
    internal static bool IsValidIpv4Range(
        string? start,
        string? end)
        => TryParseIpv4(start, out var startAddress) &&
           TryParseIpv4(end, out var endAddress) &&
           ToUInt32(startAddress) <= ToUInt32(endAddress);

    private static bool TryParseIpv4(
        string? text,
        out IPAddress address)
    {
        if (IPAddress.TryParse(text, out var parsed) &&
            parsed.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            address = parsed;
            return true;
        }

        address = IPAddress.None;
        return false;
    }

    private static uint ToUInt32(IPAddress address)
    {
        var bytes = address.GetAddressBytes();
        return ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16) | ((uint)bytes[2] << 8) | bytes[3];
    }

    [RelayCommand(CanExecute = nameof(CanScan), SupportsCancellation = true)]
    private Task Scan(CancellationToken cancellationToken)
        => ScanAsync(cancellationToken);

    [RelayCommand]
    private void Sort(object parameter)
    {
        if (parameter is not RoutedEventArgs { OriginalSource: GridViewColumnHeader headerClicked })
        {
            return;
        }

        if (headerClicked.Role == GridViewColumnHeaderRole.Padding)
        {
            return;
        }

        var direction = headerClicked != lastHeaderClicked
            ? ListSortDirection.Ascending
            : (lastDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending);

        var columnBinding = headerClicked.Tag?.ToString();
        if (string.IsNullOrEmpty(columnBinding))
        {
            return;
        }

        ApplySort(columnBinding, direction);

        lastHeaderClicked = headerClicked;
        lastDirection = direction;
    }

    private void OnFilterPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
        => FilterChange();

    private void IpScannerOnProgressReporting(
        object? sender,
        IPScannerProgressReport e)
    {
        if (e.LatestUpdate is null)
        {
            return;
        }

        // Capture values before queuing to avoid race conditions
        EnqueueProgress(
            e.LatestUpdate,
            e.TasksToProcessCount,
            e.TasksProcessedCount,
            e.PercentageCompleted);
    }

    /// <summary>
    /// Gets the number of dispatcher callbacks that applied queued progress reports.
    /// </summary>
    internal int ProgressDispatchCount { get; private set; }

    /// <summary>
    /// Queues one scanner progress report. Safe to call from any thread; one dispatcher callback
    /// applies every report queued until it runs, so a large range doesn't flood the dispatcher.
    /// </summary>
    internal void EnqueueProgress(
        IPScanResult latestUpdate,
        int tasksToProcess,
        int tasksProcessed,
        double percentageCompleted)
    {
        if (disposed)
        {
            return;
        }

        pendingProgress.Enqueue(new PendingProgress(latestUpdate, tasksToProcess, tasksProcessed, percentageCompleted));

        if (Interlocked.Exchange(ref progressDrainScheduled, 1) == 0)
        {
            _ = dispatcher.BeginInvoke(DispatcherPriority.Background, ApplyPendingProgress);
        }
    }

    private void ApplyPendingProgress()
    {
        // Cleared before draining: a report queued while draining schedules the next callback.
        Interlocked.Exchange(ref progressDrainScheduled, 0);

        if (disposed)
        {
            pendingProgress.Clear();
            return;
        }

        ProgressDispatchCount++;
        isApplyingPendingProgress = true;
        try
        {
            while (pendingProgress.TryDequeue(out var progress))
            {
                ApplyProgress(
                    progress.LatestUpdate,
                    progress.TasksToProcess,
                    progress.TasksProcessed,
                    progress.PercentageCompleted);
            }
        }
        finally
        {
            isApplyingPendingProgress = false;
        }

        UpdateEntryCountInfo();
    }

    /// <summary>
    /// Applies one scanner progress report. Runs on the UI thread.
    /// </summary>
    internal void ApplyProgress(
        IPScanResult latestUpdate,
        int tasksToProcess,
        int tasksProcessed,
        double percentageCompleted)
    {
        BusyIndicatorMaximumValue = tasksToProcess;
        BusyIndicatorCurrentValue = tasksProcessed;
        BusyIndicatorPercentageValue = $"{percentageCompleted:0.00} %";

        if (entriesByIp.TryGetValue(latestUpdate.IPAddress, out var vm))
        {
            vm.Update(latestUpdate);
            HostUpdated?.Invoke(this, new NetworkHostEventArgs(vm, latestUpdate));
        }
        else
        {
            vm = new NetworkHostViewModel(latestUpdate);
            entriesByIp[latestUpdate.IPAddress] = vm;
            Entries.Add(vm);
            HostFound?.Invoke(this, new NetworkHostEventArgs(vm, latestUpdate));
        }

        if (latestUpdate.IsCompleted)
        {
            HostCompleted?.Invoke(this, new NetworkHostEventArgs(vm, latestUpdate));
        }
    }

    /// <summary>
    /// The scanner attaches the services it discovered (mDNS, SSDP, ONVIF) to the results after its last
    /// progress report, so every row is updated once more from the returned results.
    /// </summary>
    private void ApplyFinalResults(IPScanResults ipScanResults)
    {
        foreach (var result in ipScanResults.CollectedResults)
        {
            if (entriesByIp.TryGetValue(result.IPAddress, out var host))
            {
                host.Update(result);
                HostUpdated?.Invoke(this, new NetworkHostEventArgs(host, result));
            }
        }

        view.Refresh();
        UpdateEntryCountInfo();
    }

    private void StartElapsedTimer()
    {
        stopwatch = Stopwatch.StartNew();
        elapsedTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100),
        };
        elapsedTimer.Tick += (_, _) =>
        {
            CompletionTime = stopwatch.Elapsed.GetPrettyTime();
        };
        elapsedTimer.Start();
    }

    private void StopElapsedTimer()
    {
        stopwatch?.Stop();
        elapsedTimer?.Stop();
        elapsedTimer = null;
    }

    private void UpdateEntryCountInfo()
    {
        var filteredCount = view is CollectionView collectionView
            ? collectionView.Count
            : view.Cast<object>().Count();

        EntryCountInfo = $"{filteredCount} / {totalIpCount}";
    }

    internal void ApplySort(
        string sortBy,
        ListSortDirection direction)
    {
        // Changing the sort descriptions re-sorts the view; live sorting then keeps a row in place
        // when the scanner updates its sort value, instead of resetting the whole view.
        view.SortDescriptions.Clear();
        view.SortDescriptions.Add(new SortDescription(sortBy, direction));

        if (view is ICollectionViewLiveShaping { CanChangeLiveSorting: true } liveShaping)
        {
            liveShaping.LiveSortingProperties.Clear();
            liveShaping.LiveSortingProperties.Add(sortBy.Split('.')[0]);
            liveShaping.IsLiveSorting = true;
        }
    }

    /// <summary>
    /// Releases all resources used by this instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        if (disposing)
        {
            filter.PropertyChanged -= OnFilterPropertyChanged;
            scanCancellation?.Cancel();
            StopElapsedTimer();
            pendingProgress.Clear();
        }
    }

    private sealed record PendingProgress(
        IPScanResult LatestUpdate,
        int TasksToProcess,
        int TasksProcessed,
        double PercentageCompleted);
}