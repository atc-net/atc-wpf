namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// The capture of an ESP32 sketch that drives one MAX7219 seven-segment module with LedControl: the constructor, <c>shutdown(0, false)</c>,
/// <c>setIntensity(0, 8)</c>, "HELP 1.23" with <c>setChar</c> (no decode), then the chip's own Code B decoder showing "-12.34567".
/// </summary>
/// <remarks>
/// The register values are restated from §LIB.LEDCONTROL, §MAX7219.3 and §MAX7219.4 (no library code is copied). One device, so each
/// latch frame is one address/data pair.
/// </remarks>
public static class SevenSegmentDisplayCaptures
{
    /// <summary>The no-decode segment bytes of 0 to 9 (bit 7 = DP, bit 6 = A ... bit 0 = G; §MAX7219.4).</summary>
    private static readonly byte[] DigitSegments = [0x7E, 0x30, 0x6D, 0x79, 0x33, 0x5B, 0x5F, 0x70, 0x7F, 0x7B];

    /// <summary>
    /// Gets the no-decode segment byte of a decimal digit.
    /// </summary>
    /// <param name="value">The digit, 0 to 9.</param>
    /// <returns>The segment byte, without the decimal point.</returns>
    public static byte GetDigitSegments(int value)
        => DigitSegments[value % 10];

    /// <summary>
    /// Gets the Code B code of a character (§MAX7219.4): 0-9, '-', 'E', 'H', 'L', 'P'; anything else is blank.
    /// </summary>
    /// <param name="c">The character.</param>
    /// <returns>The code, 0x0 to 0xF.</returns>
    public static byte GetCodeB(char c)
        => char.ToUpperInvariant(c) switch
        {
            >= '0' and <= '9' => (byte)(c - '0'),
            '-' => 0x0A,
            'E' => 0x0B,
            'H' => 0x0C,
            'L' => 0x0D,
            'P' => 0x0E,
            _ => 0x0F,
        };

    /// <summary>
    /// Creates the latch frames.
    /// </summary>
    /// <returns>The frames in the order they are sent.</returns>
    public static IReadOnlyList<byte[]> CreateLedControlCapture()
    {
        // Constructor: test off, scan limit 7, decode off, clear the eight digits, shutdown on (the module stays dark).
        List<byte[]> frames = [[0x0F, 0x00], [0x0B, 0x07], [0x09, 0x00]];
        for (var digit = 1; digit <= 8; digit++)
        {
            frames.Add([(byte)digit, 0x00]);
        }

        frames.Add([0x0C, 0x00]);

        // The sketch: shutdown(0, false), setIntensity(0, 8).
        frames.Add([0x0C, 0x01]);
        frames.Add([0x0A, 0x08]);

        // setChar from the leftmost digit (7) to digit 0: H E L P, blank, 1 with its decimal point, 2, 3 (LedControl charTable).
        byte[] help = [0x37, 0x4F, 0x0E, 0x67, 0x00, 0x30 | 0x80, 0x6D, 0x79];
        for (var i = 0; i < help.Length; i++)
        {
            frames.Add([(byte)(8 - i), help[i]]);
        }

        // The chip's Code B decoder for every digit, then "-12.34567" as codes (bit 7 = the decimal point).
        frames.Add([0x09, 0xFF]);
        byte[] codes = [0x0A, 0x01, 0x02 | 0x80, 0x03, 0x04, 0x05, 0x06, 0x07];
        for (var i = 0; i < codes.Length; i++)
        {
            frames.Add([(byte)(8 - i), codes[i]]);
        }

        return frames;
    }
}