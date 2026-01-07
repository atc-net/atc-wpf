namespace Atc.Wpf.NetworkControls;

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
}