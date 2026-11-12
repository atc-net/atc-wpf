namespace Atc.Wpf.Network.Diagnostics;

/// <summary>
/// Asks an NTP server for the time and shows how far the local clock is off.
/// </summary>
/// <remarks>
/// This control expects a <see cref="NtpTimeViewModel"/> as its DataContext.
/// </remarks>
public partial class NtpTimeView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NtpTimeView"/> class.
    /// </summary>
    public NtpTimeView()
        => InitializeComponent();
}