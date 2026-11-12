namespace Atc.Wpf.Network.Diagnostics;

/// <summary>
/// Traces the route to a host and lists each router with its answer time and name.
/// </summary>
/// <remarks>
/// This control expects a <see cref="TracerouteViewModel"/> as its DataContext.
/// </remarks>
public partial class TracerouteView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TracerouteView"/> class.
    /// </summary>
    public TracerouteView()
        => InitializeComponent();
}