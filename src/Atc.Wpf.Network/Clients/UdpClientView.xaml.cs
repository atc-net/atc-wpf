namespace Atc.Wpf.Network.Clients;

/// <summary>
/// A UDP client test tool: connect, send text or hex, and see the traffic in a <c>TerminalViewer</c>.
/// </summary>
/// <remarks>
/// This control expects a <see cref="UdpClientViewModel"/> as its DataContext.
/// </remarks>
public partial class UdpClientView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UdpClientView"/> class.
    /// </summary>
    public UdpClientView()
        => InitializeComponent();
}