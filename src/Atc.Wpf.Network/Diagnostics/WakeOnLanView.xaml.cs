namespace Atc.Wpf.Network.Diagnostics;

/// <summary>
/// Wakes a device with a Wake-on-LAN magic packet sent to its MAC address.
/// </summary>
/// <remarks>
/// This control expects a <see cref="WakeOnLanViewModel"/> as its DataContext.
/// </remarks>
public partial class WakeOnLanView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WakeOnLanView"/> class.
    /// </summary>
    public WakeOnLanView()
        => InitializeComponent();
}