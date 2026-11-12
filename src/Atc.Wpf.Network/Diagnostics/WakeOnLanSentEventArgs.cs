namespace Atc.Wpf.Network.Diagnostics;

/// <summary>
/// Provides data for the <see cref="WakeOnLanButton.PacketSent"/> event.
/// </summary>
/// <param name="macAddress">The MAC address the packet was sent to.</param>
/// <param name="error">The error when sending failed; otherwise <c>null</c>.</param>
public sealed class WakeOnLanSentEventArgs(
    string macAddress,
    Exception? error)
    : EventArgs
{
    /// <summary>
    /// Gets the MAC address the packet was sent to.
    /// </summary>
    public string MacAddress { get; } = macAddress;

    /// <summary>
    /// Gets the error when sending failed; otherwise <c>null</c>.
    /// </summary>
    public Exception? Error { get; } = error;
}