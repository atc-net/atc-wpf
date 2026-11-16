namespace Atc.Wpf.Displays;

/// <summary>
/// A snapshot of the counters a <see cref="DisplayMirrorReceiver"/> keeps for one display id, or for the stream as a whole.
/// </summary>
/// <param name="Packets">The packets applied to a target, plus every valid Describe packet (whether or not a target took it).</param>
/// <param name="Bytes">The whole-packet bytes (header, payload and CRC) of the packets counted in <paramref name="Packets"/>.</param>
/// <param name="CrcErrors">Packets with a bad CRC. Kept in the stream statistics only, because the display id of such a packet cannot be trusted.</param>
/// <param name="Resyncs">The times the stream parser lost sync and had to discard bytes. Kept in the stream statistics only.</param>
/// <param name="Unsupported">Packets the target did not take (<see cref="DisplayMirrorRejectReason.Unsupported"/>); not counted in <paramref name="Rejected"/>.</param>
/// <param name="Rejected">
/// Per display id: packets with a valid CRC that were rejected for another reason (no target, size mismatch, malformed payload, target failed).
/// In the stream statistics: headers and messages rejected before their display id could be trusted (unsupported version, unknown type, over the cap,
/// truncated or malformed messages, dropped partial packets); bad CRCs are counted in <paramref name="CrcErrors"/> instead.
/// </param>
/// <param name="LastPacket">When the latest packet with a valid CRC arrived for the display id; <see langword="null"/> for none (and always in the stream statistics).</param>
public sealed record DisplayMirrorStatistics(
    long Packets,
    long Bytes,
    long CrcErrors,
    long Resyncs,
    long Unsupported,
    long Rejected,
    DateTimeOffset? LastPacket);