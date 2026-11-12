namespace Atc.Wpf.Network.Clients;

/// <summary>
/// A TCP client test tool: connect, send text or hex, and see the traffic in a <c>TerminalViewer</c>.
/// </summary>
/// <remarks>
/// This control expects a <see cref="TcpClientViewModel"/> as its DataContext.
/// </remarks>
public partial class TcpClientView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TcpClientView"/> class.
    /// </summary>
    public TcpClientView()
        => InitializeComponent();
}