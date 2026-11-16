namespace Atc.Wpf.Displays;

/// <summary>
/// One decoded display mirroring packet.
/// </summary>
/// <param name="DisplayId">The display id the packet is addressed to.</param>
/// <param name="Type">The message type.</param>
/// <param name="Flags">The flags byte; receivers ignore unknown bits.</param>
/// <param name="Payload">
/// The payload. From <see cref="DisplayMirrorProtocol.TryDecode"/> it is a copy that is independent of the buffer it was decoded from;
/// from a <see cref="DisplayMirrorReceiver"/> it is a pooled buffer that is valid only during <see cref="IDisplayMirrorTarget.Apply"/>.
/// </param>
public readonly record struct DisplayMirrorPacket(
    byte DisplayId,
    DisplayMirrorMessageType Type,
    byte Flags,
    ReadOnlyMemory<byte> Payload);