namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// The link the display mirror sample receives packets on.
/// </summary>
public enum DisplayMirrorTransport
{
    /// <summary>A TCP listen port; one client at a time, read as a byte stream.</summary>
    Tcp,

    /// <summary>A UDP port; one packet per datagram.</summary>
    Udp,

    /// <summary>A serial (COM) port, read as a byte stream.</summary>
    Serial,
}