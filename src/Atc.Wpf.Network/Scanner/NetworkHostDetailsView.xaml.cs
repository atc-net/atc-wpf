namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// Shows what service discovery and NetBIOS found for one host: mDNS services, SSDP devices,
/// ONVIF devices and NetBIOS names.
/// </summary>
/// <remarks>
/// This control expects a <see cref="NetworkHostViewModel"/> as its DataContext, such as
/// <see cref="NetworkScannerViewModel.SelectedEntry"/>.
/// </remarks>
public partial class NetworkHostDetailsView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NetworkHostDetailsView"/> class.
    /// </summary>
    public NetworkHostDetailsView()
        => InitializeComponent();
}