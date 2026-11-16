namespace Atc.Wpf.Displays;

/// <summary>
/// Why a packet was not accepted or applied by the display mirroring codec or receiver.
/// </summary>
public enum DisplayMirrorRejectReason
{
    /// <summary>
    /// Nothing was rejected.
    /// </summary>
    None,

    /// <summary>
    /// The CRC does not match.
    /// </summary>
    BadCrc,

    /// <summary>
    /// The protocol version is not supported.
    /// </summary>
    UnsupportedVersion,

    /// <summary>
    /// The message type is not known.
    /// </summary>
    UnknownType,

    /// <summary>
    /// The declared payload is over the cap for its type.
    /// </summary>
    TooLarge,

    /// <summary>
    /// The packet is shorter than its header declares.
    /// </summary>
    Truncated,

    /// <summary>
    /// The packet is malformed (wrong magic, extra bytes, or an invalid payload).
    /// </summary>
    Malformed,

    /// <summary>
    /// A frame size differs from the size of its target.
    /// </summary>
    SizeMismatch,

    /// <summary>
    /// The target does not support the message type.
    /// </summary>
    Unsupported,

    /// <summary>
    /// No target is registered for the display id.
    /// </summary>
    NoTarget,

    /// <summary>
    /// The target threw while applying the message.
    /// </summary>
    TargetFailed,
}