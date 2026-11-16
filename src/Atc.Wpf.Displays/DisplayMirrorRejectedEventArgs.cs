namespace Atc.Wpf.Displays;

/// <summary>
/// The data of <see cref="DisplayMirrorReceiver.PacketRejected"/>.
/// </summary>
public sealed class DisplayMirrorRejectedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorRejectedEventArgs"/> class.
    /// </summary>
    /// <param name="displayId">The display id of the packet, or <see langword="null"/> when it cannot be trusted.</param>
    /// <param name="reason">Why the packet was rejected.</param>
    public DisplayMirrorRejectedEventArgs(
        byte? displayId,
        DisplayMirrorRejectReason reason)
    {
        DisplayId = displayId;
        Reason = reason;
    }

    /// <summary>
    /// Gets the display id of the packet, or <see langword="null"/> for a rejection before the display id could be trusted
    /// (a bad header, a bad CRC, a truncated or malformed message, or a dropped partial packet).
    /// </summary>
    public byte? DisplayId { get; }

    /// <summary>
    /// Gets why the packet was rejected.
    /// </summary>
    public DisplayMirrorRejectReason Reason { get; }
}