namespace Atc.Wpf.Displays;

/// <summary>
/// Something a <see cref="DisplayMirrorReceiver"/> applies the packets of one display id to: a frame buffer, a chip emulator or a bus decoder.
/// <see cref="DisplayMirrorTargets"/> creates the built-in targets.
/// </summary>
/// <remarks>
/// <para>
/// <b>Payload lifetime:</b> the <see cref="DisplayMirrorPacket.Payload"/> a <see cref="DisplayMirrorReceiver"/> passes in is a slice of a pooled buffer
/// that is reused as soon as <see cref="Apply"/> returns. Use it only during the call; to use it later (for example on the UI thread), copy it first:
/// </para>
/// <code>
/// public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
/// {
///     var bytes = packet.Payload.ToArray(); // a copy that outlives the call
///     dispatcher.BeginInvoke(() => Show(bytes));
///     return DisplayMirrorRejectReason.None;
/// }
/// </code>
/// <para>
/// <see cref="Apply"/> runs under the receiver's lock and must not feed the same receiver (see <see cref="DisplayMirrorReceiver"/>).
/// </para>
/// </remarks>
public interface IDisplayMirrorTarget
{
    /// <summary>
    /// Applies a packet.
    /// </summary>
    /// <param name="packet">
    /// The packet. When it comes from a <see cref="DisplayMirrorReceiver"/>, its payload is a pooled buffer that is valid only during the call:
    /// copy what must be kept.
    /// </param>
    /// <returns>
    /// <see cref="DisplayMirrorRejectReason.None"/> when applied; <see cref="DisplayMirrorRejectReason.Unsupported"/> when the target does not take
    /// the message type (or, for an I2C target with an address filter, the address); <see cref="DisplayMirrorRejectReason.SizeMismatch"/> when a frame
    /// has another size than the target; <see cref="DisplayMirrorRejectReason.Malformed"/> when the payload does not match its type.
    /// </returns>
    DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet);
}