namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// ViewModel for controlling the visibility of columns in the network scanner grid.
/// </summary>
public partial class NetworkScannerColumnsViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets a value indicating whether the ping quality category column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showPingQualityCategory = true;

    /// <summary>
    /// Gets or sets a value indicating whether the IP address column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showIpAddress = true;

    /// <summary>
    /// Gets or sets a value indicating whether the IP status column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showIpStatus = true;

    /// <summary>
    /// Gets or sets a value indicating whether the hostname column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showHostname = true;

    /// <summary>
    /// Gets or sets a value indicating whether the MAC address column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showMacAddress = true;

    /// <summary>
    /// Gets or sets a value indicating whether the MAC vendor column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showMacVendor = true;

    /// <summary>
    /// Gets or sets a value indicating whether the open port numbers column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showOpenPortNumbers = true;

    /// <summary>
    /// Gets or sets a value indicating whether the total time column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showTotalInMs = true;

    /// <summary>
    /// Gets or sets a value indicating whether the device name column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showDeviceName = true;

    /// <summary>
    /// Gets or sets a value indicating whether the NetBIOS workgroup column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showWorkgroup;

    /// <summary>
    /// Gets or sets a value indicating whether the model column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showModel = true;

    /// <summary>
    /// Gets or sets a value indicating whether the services column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showServices = true;

    /// <summary>
    /// Gets or sets a value indicating whether the ONVIF column is visible.
    /// </summary>
    [ObservableProperty]
    private bool showOnvif = true;
}