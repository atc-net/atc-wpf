namespace Atc.Wpf.Displays;

/// <summary>
/// The MAX7219 seven-segment rules: the Code B font and the segment bit order.
/// </summary>
internal static class Max7219Segments
{
    /// <summary>The decimal point bit of a digit register (D7).</summary>
    public const byte DecimalPoint = 0x80;

    private const byte CodeMask = 0x0F;

    // §MAX7219.4: the segment bytes of the Code B characters 0-9, '-', E, H, L, P and blank (DP off).
    private static ReadOnlySpan<byte> CodeB
        => [0x7E, 0x30, 0x6D, 0x79, 0x33, 0x5B, 0x5F, 0x70, 0x7F, 0x7B, 0x01, 0x4F, 0x37, 0x0E, 0x67, 0x00];

    /// <summary>
    /// Gets the segment byte a digit register drives (§MAX7219.3 and §MAX7219.4).
    /// </summary>
    /// <param name="value">The digit register value.</param>
    /// <param name="isDecoded">Whether the digit's decode-mode bit is set.</param>
    /// <returns>The segment byte, bits D7..D0 = DP A B C D E F G.</returns>
    public static byte GetSegmentByte(
        byte value,
        bool isDecoded)
        => isDecoded
            ? (byte)(CodeB[value & CodeMask] | (value & DecimalPoint))
            : value;

    /// <summary>
    /// Converts a segment byte to the parts of a <see cref="SevenSegment"/> (the decimal point is not a part).
    /// </summary>
    /// <param name="segmentByte">The segment byte, bits D6..D0 = A B C D E F G (§MAX7219.3).</param>
    /// <returns>The lit parts.</returns>
    public static SevenSegmentParts ToParts(byte segmentByte)
    {
        // Bit 6 - k drives segment k (A = Top ... G = Middle), which is bit k of SevenSegmentParts.
        var parts = 0;
        for (var segment = 0; segment < 7; segment++)
        {
            if ((segmentByte & (1 << (6 - segment))) != 0)
            {
                parts |= 1 << segment;
            }
        }

        return (SevenSegmentParts)parts;
    }
}