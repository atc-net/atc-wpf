// ReSharper disable InvertIf
namespace Atc.Wpf.Network;

/// <summary>
/// ViewModel for the network scanner control, providing IP range scanning functionality.
/// </summary>
public partial class NetworkScannerViewModel : ViewModelBase, IDisposable
{
    private readonly ICollectionView view;
    private GridViewColumnHeader? lastHeaderClicked;
    private ListSortDirection lastDirection = ListSortDirection.Ascending;
    private DispatcherTimer? elapsedTimer;
    private Stopwatch? stopwatch;
    private int totalIpCount;
    private bool disposed;

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
        Entries = [];
        columns = new NetworkScannerColumnsViewModel();
        filter = new NetworkScannerFilterViewModel();
        filter.PropertyChanged += OnFilterPropertyChanged;
        view = CollectionViewSource.GetDefaultView(Entries);

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

    [RelayCommand]
    private void FilterChange()
    {
        view.Filter = o =>
        {
            if (o is NetworkHostViewModel entry)
            {
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
            }

            return true;
        };

        view.Refresh();
        UpdateEntryCountInfo();
    }

    [RelayCommand]
    private void Clear()
    {
        SelectedEntry = null;
        Entries.Clear();
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
            Clipboard.SetText(SelectedEntry.IpAddress.ToString());
        }
    }

    [RelayCommand(CanExecute = nameof(CanCopy))]
    private void CopyHostname()
    {
        if (SelectedEntry?.Hostname is not null)
        {
            Clipboard.SetText(SelectedEntry.Hostname);
        }
    }

    [RelayCommand(CanExecute = nameof(CanCopy))]
    private void CopyMacAddress()
    {
        if (SelectedEntry?.MacAddress is not null)
        {
            Clipboard.SetText(SelectedEntry.MacAddress);
        }
    }

    [RelayCommand(CanExecute = nameof(CanCopy))]
    private void CopyAllInfo()
    {
        if (SelectedEntry is null)
        {
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CopyInfoIpAddressFormat1, SelectedEntry.IpAddress));

        if (!string.IsNullOrEmpty(SelectedEntry.Hostname))
        {
            sb.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CopyInfoHostnameFormat1, SelectedEntry.Hostname));
        }

        if (!string.IsNullOrEmpty(SelectedEntry.MacAddress))
        {
            sb.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CopyInfoMacAddressFormat1, SelectedEntry.MacAddress));
        }

        if (!string.IsNullOrEmpty(SelectedEntry.MacVendor))
        {
            sb.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CopyInfoMacVendorFormat1, SelectedEntry.MacVendor));
        }

        if (SelectedEntry.PingStatus is not null)
        {
            sb.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CopyInfoStatusFormat1, SelectedEntry.PingStatus.Status));
        }

        if (SelectedEntry.OpenPortNumbers.Any())
        {
            sb.AppendLine(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CopyInfoOpenPortsFormat1, SelectedEntry.OpenPortNumbersAsCommaList));
        }

        Clipboard.SetText(sb.ToString());
    }

    private bool CanScan()
        => StartIpAddress is not null &&
           IPAddress.TryParse((string?)StartIpAddress, out _) &&
           EndIpAddress is not null &&
           IPAddress.TryParse((string?)EndIpAddress, out _);

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "User-facing error handling requires catching all exceptions.")]
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "Scan operation requires sequential setup, execution, and cleanup.")]
    [RelayCommand(CanExecute = nameof(CanScan), SupportsCancellation = true)]
    private async Task Scan(CancellationToken cancellationToken)
    {
        IsBusy = true;
        ErrorMessage = null;
        SelectedEntry = null;
        Entries.Clear();

        var startIp = IPAddress.Parse(StartIpAddress!);
        var endIp = IPAddress.Parse(EndIpAddress!);
        totalIpCount = IPv4AddressHelper.GetAddressesInRange(startIp, endIp).Count;
        UpdateEntryCountInfo();

        BusyIndicatorMaximumValue = 0;
        BusyIndicatorCurrentValue = 0;
        BusyIndicatorPercentageValue = string.Empty;
        CompletionTime = null;

        StartElapsedTimer();

        var ipScannerConfig = new IPScannerConfig(IPServicePortExaminationLevel.WellKnown);
        using var ipScanner = new IPScanner(ipScannerConfig);

        if (PortsNumbers.Count > 0)
        {
            ipScanner.Configuration.PortNumbers = PortsNumbers;
        }

        ipScanner.ProgressReporting += IpScannerOnProgressReporting;

        try
        {
            var ipScanResults = await ipScanner
                .ScanRange(startIp, endIp, cancellationToken)
                .ConfigureAwait(true);

            if (ipScanResults.End.HasValue)
            {
                CompletionTime = ipScanResults.TimeDiff;
            }
        }
        catch (OperationCanceledException)
        {
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
            ipScanner.ProgressReporting -= IpScannerOnProgressReporting;
            IsBusy = false;
        }
    }

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

        // Capture values before dispatch to avoid race conditions
        var latestUpdate = e.LatestUpdate;
        var tasksToProcess = e.TasksToProcessCount;
        var tasksProcessed = e.TasksProcessedCount;
        var percentageCompleted = e.PercentageCompleted;
        var isCompleted = e.LatestUpdate.IsCompleted;

        _ = Application.Current.Dispatcher.BeginInvoke(() =>
        {
            BusyIndicatorMaximumValue = tasksToProcess;
            BusyIndicatorCurrentValue = tasksProcessed;
            BusyIndicatorPercentageValue = $"{percentageCompleted:0.00} %";

            var vm = Entries.FirstOrDefault(x => x.IpAddress.Equals(latestUpdate.IPAddress));

            if (vm is null)
            {
                Entries.Add(new NetworkHostViewModel(latestUpdate));
                FilterChange();
                return;
            }

            if (latestUpdate.PingStatus is not null)
            {
                vm.PingStatus = latestUpdate.PingStatus;
                vm.PingQualityCategoryToolTip = string.Create(
                    GlobalizationConstants.EnglishCultureInfo,
                    $"{latestUpdate.PingStatus.QualityCategory.GetDescription()} - {latestUpdate.PingStatus.PingInMs}ms");
            }
            else
            {
                vm.PingStatus = new PingStatusResult(latestUpdate.IPAddress, IPStatus.Unknown, 0);
            }

            vm.Hostname = latestUpdate.Hostname;
            vm.MacAddress = latestUpdate.MacAddress;
            vm.MacVendor = latestUpdate.MacVendor;
            vm.OpenPortNumbers = latestUpdate.OpenPortNumbers;
            if (isCompleted)
            {
                vm.TimeDiff = latestUpdate.TimeDiff;
            }

            FilterChange();
        });
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
        var filteredCount = view
            .Cast<object>()
            .Count();

        EntryCountInfo = $"{filteredCount} / {totalIpCount}";
    }

    private void ApplySort(
        string sortBy,
        ListSortDirection direction)
    {
        view.SortDescriptions.Clear();
        view.SortDescriptions.Add(new SortDescription(sortBy, direction));
        view.Refresh();
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

        if (disposing)
        {
            filter.PropertyChanged -= OnFilterPropertyChanged;
            StopElapsedTimer();
        }

        disposed = true;
    }
}