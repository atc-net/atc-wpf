namespace Atc.Wpf.Network;

/// <summary>
/// Provides a settings panel for the network scanner, allowing configuration of
/// IP range, ports, and filter options.
/// </summary>
/// <remarks>
/// This control expects a <see cref="NetworkScannerViewModel"/> as its DataContext.
/// The consumer can configure the initial values (StartIpAddress, EndIpAddress, PortsNumbers)
/// through the ViewModel before displaying this control.
/// </remarks>
public partial class NetworkScannerSettingsView
{
    public NetworkScannerSettingsView()
        => InitializeComponent();
}