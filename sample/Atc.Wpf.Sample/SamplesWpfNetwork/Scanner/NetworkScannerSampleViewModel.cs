namespace Atc.Wpf.Sample.SamplesWpfNetwork.Scanner;

[SuppressMessage("Security", "S1313:Do not hardcode IP addresses", Justification = "OK - Sample data only.")]
public partial class NetworkScannerSampleViewModel : ViewModelBase
{
    private static readonly ushort[] RtspPorts = [554, 8554];

    [ObservableProperty(AfterChangedCallback = nameof(ApplyCameraFilter))]
    private bool showOnlyCameras;

    public NetworkScannerSampleViewModel()
    {
        NetworkScanner.ScanStarted += (_, _) =>
        {
            EventLog.Clear();
            AddToEventLog("Scan started");
        };
        NetworkScanner.HostFound += (_, e) => AddToEventLog($"Found {e.ScanResult.IPAddress}");
        NetworkScanner.VncRequested += (_, e) => AddToEventLog($"VNC requested: {e.Host.IpAddress}:{e.Port}");
        NetworkScanner.TracerouteRequested += (_, e) => ShowTraceroute(e.SelectedHost);
        NetworkScanner.ScanCompleted += (_, e) => AddToEventLog(
            e.IsCancelled
                ? "Scan cancelled"
                : $"Scan completed: {e.ScanResults?.CollectedResults.Count(x => x.HasConnection) ?? 0} reachable hosts");
    }

    public NetworkScannerViewModel NetworkScanner { get; } = new()
    {
        StartIpAddress = "192.168.1.1",
        EndIpAddress = "192.168.1.254",
        PortsNumbers = [22, 80, 443, 554, 8554],
        AllowRemoteDesktop = true,
    };

    public ObservableCollection<string> EventLog { get; } = [];

    /// <summary>
    /// Looks up MAC vendors offline from the IEEE registry files (oui.csv, mam.csv, oui36.csv) instead of macvendors.com.
    /// </summary>
    [RelayCommand]
    private void LoadVendorFiles()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "IEEE registry files (oui.csv, mam.csv, oui36.csv)",
            Filter = "CSV (*.csv)|*.csv",
            Multiselect = true,
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        NetworkScanner.MacAddressVendorLookup = Atc.Network.Helpers.IeeeOuiMacAddressVendorLookup.FromFiles(dialog.FileNames);
        AddToEventLog($"Offline vendor lookup: {string.Join(", ", dialog.SafeFileNames)}");
    }

    private static void ShowTraceroute(NetworkHostViewModel? host)
    {
        if (host is null)
        {
            return;
        }

        var traceroute = new TracerouteViewModel { Target = host.IpAddress.ToString() };
        var window = new Window
        {
            Title = $"Traceroute - {host.IpAddress}",
            Width = 800,
            Height = 450,
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new TracerouteView { DataContext = traceroute, Margin = new Thickness(10) },
        };
        window.Show();
        _ = traceroute.TraceCommand.ExecuteAsync(null);
    }

    private void ApplyCameraFilter()
        => NetworkScanner.HostFilter = ShowOnlyCameras
            ? IsCamera
            : null;

    /// <summary>
    /// A sample HostFilter: the control has no camera logic; the app decides what a camera is.
    /// </summary>
    private static bool IsCamera(NetworkHostViewModel host)
        => host.ScanResult is { } result &&
           (result.OnvifDevices.Count > 0 ||
            result.OpenPortNumbers.Any(RtspPorts.Contains) ||
            result.MdnsServices.Any(x => x.ServiceType.StartsWith("_rtsp.", StringComparison.OrdinalIgnoreCase)));

    private void AddToEventLog(string text)
        => EventLog.Insert(0, $"{DateTime.Now:HH:mm:ss}  {text}");
}