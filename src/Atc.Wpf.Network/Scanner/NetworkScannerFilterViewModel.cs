namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// ViewModel for controlling the filter options in the network scanner.
/// </summary>
public partial class NetworkScannerFilterViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets a value indicating whether to show only hosts with successful ping responses.
    /// </summary>
    [ObservableProperty]
    private bool showOnlySuccess = true;

    /// <summary>
    /// Gets or sets a value indicating whether to show only hosts with open ports.
    /// </summary>
    [ObservableProperty]
    private bool showOnlyWithOpenPorts = true;

    /// <summary>
    /// Gets or sets a value indicating whether to show only hosts that answered in any way:
    /// ping, host name, MAC address, an open port, NetBIOS or service discovery.
    /// </summary>
    [ObservableProperty]
    private bool showOnlyReachable;
}