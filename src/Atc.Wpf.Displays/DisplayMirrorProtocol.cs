namespace Atc.Wpf.Displays;

/// <summary>
/// The wire codec of the display mirroring protocol (version 1), shared by the receiver and by tests.
/// </summary>
/// <remarks>
/// <para>
/// A packet is a 10-byte header, the payload and a 2-byte CRC. All multi-byte fields are little-endian.
/// The header is the magic <c>0xA7 0xC0</c>, the version, the display id, the type, the flags and the
/// payload length (u32). The CRC is CRC-16/CCITT-FALSE over header bytes 2..9 and the payload.
/// </para>
/// <para>
/// <see cref="TryDecode"/> checks, in this order: at least 12 bytes, magic, version, known type, the payload cap of the type,
/// the payload length against the bytes present, and last the CRC. The cap check comes before the length check so a header
/// that declares an oversized payload is recognised as noise without waiting for a body.
/// </para>
/// </remarks>
public static class DisplayMirrorProtocol
{
    /// <summary>
    /// The first magic byte.
    /// </summary>
    public const byte Magic0 = 0xA7;

    /// <summary>
    /// The second magic byte.
    /// </summary>
    public const byte Magic1 = 0xC0;

    /// <summary>
    /// The protocol version this codec reads and writes.
    /// </summary>
    public const byte Version = 1;

    /// <summary>
    /// The length of the header in bytes.
    /// </summary>
    public const int HeaderLength = 10;

    /// <summary>
    /// The length of the trailing CRC in bytes.
    /// </summary>
    public const int CrcLength = 2;

    /// <summary>
    /// The default cap for the payload of a frame message (4 MiB).
    /// </summary>
    public const int DefaultMaxFramePayload = 4_194_304;

    /// <summary>
    /// The default cap for the payload of every other message (64 KiB).
    /// </summary>
    public const int DefaultMaxTrafficPayload = 65_536;

    private const ushort CrcInitial = 0xFFFF;
    private const ushort CrcPolynomial = 0x1021;
    private const int MinimumPacketLength = HeaderLength + CrcLength;
    private const int MaxDimension = ushort.MaxValue;

    private static readonly ushort[] CrcTable = BuildCrcTable();

    /// <summary>
    /// Computes the CRC-16/CCITT-FALSE (polynomial 0x1021, initial value 0xFFFF, no reflection, no final xor).
    /// The check value of the ASCII text "123456789" is 0x29B1.
    /// </summary>
    /// <param name="data">The bytes to checksum.</param>
    /// <returns>The CRC.</returns>
    public static ushort ComputeCrc(ReadOnlySpan<byte> data)
        => UpdateCrc(CrcInitial, data);

    /// <summary>
    /// Encodes a packet.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="type">The message type.</param>
    /// <param name="payload">The payload.</param>
    /// <param name="flags">The flags byte.</param>
    /// <returns>The packet bytes.</returns>
    /// <exception cref="ArgumentException">
    /// The payload is over the cap of its type: <see cref="DefaultMaxFramePayload"/> for a frame message (<see cref="IsFrameType"/>),
    /// <see cref="DefaultMaxTrafficPayload"/> for any other; a receiver with the default caps would reject it as too large.
    /// </exception>
    public static byte[] Encode(
        byte displayId,
        DisplayMirrorMessageType type,
        ReadOnlySpan<byte> payload,
        byte flags = 0)
    {
        ThrowIfOverTheCap(type, payload);

        var result = new byte[MinimumPacketLength + payload.Length];
        Encode(displayId, type, payload, result, flags);
        return result;
    }

    /// <summary>
    /// Encodes a packet into a caller-supplied buffer.
    /// </summary>
    /// <param name="displayId">The display id.</param>
    /// <param name="type">The message type.</param>
    /// <param name="payload">The payload.</param>
    /// <param name="destination">The buffer to write to; it must hold at least <c>12 + payload.Length</c> bytes.</param>
    /// <param name="flags">The flags byte.</param>
    /// <returns>The number of bytes written.</returns>
    /// <exception cref="ArgumentException">
    /// The payload is over the cap of its type (<see cref="DefaultMaxFramePayload"/> for a frame message, <see cref="DefaultMaxTrafficPayload"/>
    /// for any other), or the destination is too small.
    /// </exception>
    public static int Encode(
        byte displayId,
        DisplayMirrorMessageType type,
        ReadOnlySpan<byte> payload,
        Span<byte> destination,
        byte flags = 0)
    {
        ThrowIfOverTheCap(type, payload);

        var total = MinimumPacketLength + payload.Length;
        if (destination.Length < total)
        {
            throw new ArgumentException("The destination is too small for the packet.", nameof(destination));
        }

        destination[0] = Magic0;
        destination[1] = Magic1;
        destination[2] = Version;
        destination[3] = displayId;
        destination[4] = (byte)type;
        destination[5] = flags;
        BinaryPrimitives.WriteUInt32LittleEndian(destination.Slice(6, 4), (uint)payload.Length);
        payload.CopyTo(destination[HeaderLength..]);

        var crc = ComputeCrc(destination.Slice(2, HeaderLength - 2 + payload.Length));
        BinaryPrimitives.WriteUInt16LittleEndian(destination.Slice(HeaderLength + payload.Length, CrcLength), crc);
        return total;
    }

    /// <summary>
    /// Decodes exactly one packet using the default payload caps.
    /// </summary>
    /// <param name="packet">The packet bytes; the span must hold exactly one packet.</param>
    /// <param name="result">The decoded packet (its payload is a copy), or the default value when rejected.</param>
    /// <returns><see cref="DisplayMirrorRejectReason.None"/> on success; otherwise the reason the packet was rejected.</returns>
    public static DisplayMirrorRejectReason TryDecode(
        ReadOnlySpan<byte> packet,
        out DisplayMirrorPacket result)
    {
        result = default;

        if (packet.Length < MinimumPacketLength)
        {
            return DisplayMirrorRejectReason.Truncated;
        }

        if (packet[0] != Magic0 ||
            packet[1] != Magic1)
        {
            return DisplayMirrorRejectReason.Malformed;
        }

        if (packet[2] != Version)
        {
            return DisplayMirrorRejectReason.UnsupportedVersion;
        }

        var type = (DisplayMirrorMessageType)packet[4];
        if (!IsKnownType(type))
        {
            return DisplayMirrorRejectReason.UnknownType;
        }

        var length = BinaryPrimitives.ReadUInt32LittleEndian(packet.Slice(6, 4));
        var cap = IsFrameType(type)
            ? DefaultMaxFramePayload
            : DefaultMaxTrafficPayload;
        if (length > (uint)cap)
        {
            return DisplayMirrorRejectReason.TooLarge;
        }

        var present = packet.Length - MinimumPacketLength;
        if (length > (uint)present)
        {
            return DisplayMirrorRejectReason.Truncated;
        }

        if (length < (uint)present)
        {
            return DisplayMirrorRejectReason.Malformed;
        }

        var crc = ComputeCrc(packet.Slice(2, HeaderLength - 2 + present));
        if (crc != BinaryPrimitives.ReadUInt16LittleEndian(packet.Slice(HeaderLength + present, CrcLength)))
        {
            return DisplayMirrorRejectReason.BadCrc;
        }

        result = new DisplayMirrorPacket(
            packet[3],
            type,
            packet[5],
            packet.Slice(HeaderLength, present).ToArray());
        return DisplayMirrorRejectReason.None;
    }

    /// <summary>
    /// Creates a MonoFrame payload: u16 width, u16 height, then the controller pages
    /// (<c>width</c> bytes per page of 8 rows, bit 0 = top row).
    /// </summary>
    /// <param name="width">The width in pixels (1 to 65535).</param>
    /// <param name="height">The height in pixels (1 to 65535).</param>
    /// <param name="pages">The page bytes: exactly <c>width x ceil(height / 8)</c>.</param>
    /// <returns>The payload.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A dimension is outside 1 to 65535.</exception>
    /// <exception cref="ArgumentException">The page length does not match the dimensions.</exception>
    public static byte[] CreateMonoFramePayload(
        int width,
        int height,
        ReadOnlySpan<byte> pages)
    {
        ValidateDimensions(width, height);
        RequireLength(pages.Length, (long)width * ((height + 7) / 8), nameof(pages));

        var result = new byte[4 + pages.Length];
        WriteDimensions(result, width, height);
        pages.CopyTo(result.AsSpan(4));
        return result;
    }

    /// <summary>
    /// Creates a ColorFrame payload: u16 width, u16 height, then RGB565 big-endian pixels row by row.
    /// </summary>
    /// <param name="width">The width in pixels (1 to 65535).</param>
    /// <param name="height">The height in pixels (1 to 65535).</param>
    /// <param name="rgb565BigEndian">The pixel bytes: exactly <c>width x height x 2</c>.</param>
    /// <returns>The payload.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A dimension is outside 1 to 65535.</exception>
    /// <exception cref="ArgumentException">The pixel length does not match the dimensions.</exception>
    public static byte[] CreateColorFramePayload(
        int width,
        int height,
        ReadOnlySpan<byte> rgb565BigEndian)
    {
        ValidateDimensions(width, height);
        RequireLength(rgb565BigEndian.Length, (long)width * height * 2, nameof(rgb565BigEndian));

        var result = new byte[4 + rgb565BigEndian.Length];
        WriteDimensions(result, width, height);
        rgb565BigEndian.CopyTo(result.AsSpan(4));
        return result;
    }

    /// <summary>
    /// Creates an EPaperFrame payload: u16 width, u16 height, plane count (1 or 2), the black plane,
    /// the red plane when there are two, then the refresh kind (0 full, 1 partial).
    /// Planes use the native orientation of the panel: rows are padded to whole bytes, the most significant bit is the leftmost pixel.
    /// </summary>
    /// <param name="width">The width in pixels (1 to 65535).</param>
    /// <param name="height">The height in pixels (1 to 65535).</param>
    /// <param name="black">The black plane: exactly <c>ceil(width / 8) x height</c> bytes.</param>
    /// <param name="red">The red plane (the same length as <paramref name="black"/>), or empty for a one-plane frame.</param>
    /// <param name="isPartial">Whether the refresh is partial rather than full.</param>
    /// <returns>The payload.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A dimension is outside 1 to 65535.</exception>
    /// <exception cref="ArgumentException">A plane length does not match the dimensions.</exception>
    public static byte[] CreateEPaperFramePayload(
        int width,
        int height,
        ReadOnlySpan<byte> black,
        ReadOnlySpan<byte> red,
        bool isPartial)
    {
        ValidateDimensions(width, height);
        var planeLength = (long)((width + 7) / 8) * height;
        RequireLength(black.Length, planeLength, nameof(black));
        if (!red.IsEmpty)
        {
            RequireLength(red.Length, planeLength, nameof(red));
        }

        var result = new byte[4 + 1 + black.Length + red.Length + 1];
        WriteDimensions(result, width, height);
        result[4] = red.IsEmpty ? (byte)1 : (byte)2;
        black.CopyTo(result.AsSpan(5));
        red.CopyTo(result.AsSpan(5 + black.Length));
        result[^1] = isPartial ? (byte)1 : (byte)0;
        return result;
    }

    /// <summary>
    /// Tells whether a message type is a whole-frame message (subject to the larger frame payload cap).
    /// </summary>
    /// <param name="type">The message type.</param>
    /// <returns><see langword="true"/> for MonoFrame, ColorFrame and EPaperFrame.</returns>
    public static bool IsFrameType(DisplayMirrorMessageType type)
        => type is DisplayMirrorMessageType.MonoFrame
            or DisplayMirrorMessageType.ColorFrame
            or DisplayMirrorMessageType.EPaperFrame;

    internal static bool IsKnownType(DisplayMirrorMessageType type)
        => type is DisplayMirrorMessageType.Describe
            or DisplayMirrorMessageType.MonoFrame
            or DisplayMirrorMessageType.ColorFrame
            or DisplayMirrorMessageType.EPaperFrame
            or DisplayMirrorMessageType.Commands
            or DisplayMirrorMessageType.Data
            or DisplayMirrorMessageType.I2cWrite
            or DisplayMirrorMessageType.Reset
            or DisplayMirrorMessageType.Latch;

    private static void ThrowIfOverTheCap(
        DisplayMirrorMessageType type,
        ReadOnlySpan<byte> payload)
    {
        var length = payload.Length;
        var cap = IsFrameType(type)
            ? DefaultMaxFramePayload
            : DefaultMaxTrafficPayload;
        if (length > cap)
        {
            throw new ArgumentException(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"The {type} payload of {length} bytes is over the cap of {cap} bytes for its message type."),
                nameof(payload));
        }
    }

    private static void ValidateDimensions(
        int width,
        int height)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(width, MaxDimension);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(height, MaxDimension);
    }

    private static void RequireLength(
        int actual,
        long expected,
        string parameterName)
    {
        if (actual != expected)
        {
            throw new ArgumentException($"Expected {expected} bytes for the given dimensions but got {actual}.", parameterName);
        }
    }

    private static void WriteDimensions(
        byte[] payload,
        int width,
        int height)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(0, 2), (ushort)width);
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(2, 2), (ushort)height);
    }

    private static ushort UpdateCrc(
        ushort crc,
        ReadOnlySpan<byte> data)
    {
        foreach (var b in data)
        {
            crc = (ushort)((crc << 8) ^ CrcTable[(crc >> 8) ^ b]);
        }

        return crc;
    }

    private static ushort[] BuildCrcTable()
    {
        var table = new ushort[256];
        for (var i = 0; i < table.Length; i++)
        {
            var value = (ushort)(i << 8);
            for (var bit = 0; bit < 8; bit++)
            {
                value = (value & 0x8000) != 0
                    ? (ushort)((value << 1) ^ CrcPolynomial)
                    : (ushort)(value << 1);
            }

            table[i] = value;
        }

        return table;
    }
}