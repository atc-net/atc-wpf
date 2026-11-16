namespace Atc.Wpf.Displays;

/// <summary>
/// The decoded payload of a <see cref="DisplayMirrorMessageType.Describe"/> message. It is advisory:
/// a receiver never creates or resizes a target from it.
/// </summary>
/// <param name="Kind">The kind of display. Undefined byte values from the wire are kept as they are.</param>
/// <param name="Chip">The controller chip. Undefined byte values from the wire are kept as they are.</param>
/// <param name="Width">The width in pixels (or columns for character displays).</param>
/// <param name="Height">The height in pixels (or rows for character displays).</param>
/// <param name="Name">The display name.</param>
public sealed record DisplayMirrorDescribe(
    DisplayMirrorDisplayKind Kind,
    DisplayMirrorChip Chip,
    int Width,
    int Height,
    string Name)
{
    private const int FixedLength = 6;

    /// <summary>
    /// Parses a Describe payload: kind, chip, u16 width, u16 height (little-endian), then the UTF-8 name.
    /// </summary>
    /// <param name="payload">The payload bytes (at least 6).</param>
    /// <param name="describe">The parsed description, or <see langword="null"/> when the payload is shorter than 6 bytes.</param>
    /// <returns><see langword="true"/> when parsed. Unknown kind and chip values still parse; invalid UTF-8 in the name becomes replacement characters.</returns>
    public static bool TryParse(
        ReadOnlySpan<byte> payload,
        out DisplayMirrorDescribe? describe)
    {
        if (payload.Length < FixedLength)
        {
            describe = null;
            return false;
        }

        describe = new DisplayMirrorDescribe(
            (DisplayMirrorDisplayKind)payload[0],
            (DisplayMirrorChip)payload[1],
            payload[2] | (payload[3] << 8),
            payload[4] | (payload[5] << 8),
            Encoding.UTF8.GetString(payload[FixedLength..]));
        return true;
    }

    /// <summary>
    /// Serializes this description to a Describe payload.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The width or height is outside 0 to 65535.</exception>
    /// <exception cref="ArgumentNullException">The name is null.</exception>
    /// <exception cref="ArgumentException">The UTF-8 form of the name does not fit the Describe cap.</exception>
    [SuppressMessage("Meziantou.Analyzer", "MA0015:Specify the parameter name in ArgumentException", Justification = "The invalid values are record properties, not parameters.")]
    public byte[] ToPayload()
    {
        RequireUShort(Width, nameof(Width));
        RequireUShort(Height, nameof(Height));
        ArgumentNullException.ThrowIfNull(Name);

        var name = Encoding.UTF8.GetBytes(Name);
        if (FixedLength + name.Length > DisplayMirrorProtocol.DefaultMaxTrafficPayload)
        {
            throw new ArgumentException("The name is too long for a Describe payload.", nameof(Name));
        }

        var result = new byte[FixedLength + name.Length];
        result[0] = (byte)Kind;
        result[1] = (byte)Chip;
        result[2] = (byte)(Width & 0xFF);
        result[3] = (byte)(Width >> 8);
        result[4] = (byte)(Height & 0xFF);
        result[5] = (byte)(Height >> 8);
        name.CopyTo(result, FixedLength);
        return result;
    }

    private static void RequireUShort(
        int value,
        string parameterName)
    {
        if (value is < 0 or > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "The value must be between 0 and 65535.");
        }
    }
}