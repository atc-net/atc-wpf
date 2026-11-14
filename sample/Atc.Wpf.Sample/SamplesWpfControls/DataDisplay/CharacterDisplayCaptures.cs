namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

/// <summary>
/// Two captures of the same 20 x 4 "clock + temperature" screen, as an ESP32 sketch sends it:
/// the logical HD44780 instructions and data, and the raw bytes a PCF8574 I2C backpack receives (LiquidCrystal_I2C).
/// </summary>
public static class CharacterDisplayCaptures
{
    private const byte ClearDisplay = 0x01;
    private const byte EntryModeIncrement = 0x06;
    private const byte DisplayOnCursorOff = 0x0C;
    private const byte FunctionSet8BitTwoLines = 0x38;
    private const byte FunctionSet4BitTwoLines = 0x28;
    private const byte SetCgramAddress = 0x40;
    private const byte SetDdramAddress = 0x80;
    private const byte PinRs = 0x01;
    private const byte PinEnable = 0x04;
    private const byte PinBacklight = 0x08;
    private const int NibbleShift = 4;
    private const byte NibbleMask = 0x0F;

    // The DDRAM addresses of the rows of a 20 x 4 module: rows 3 and 4 continue rows 1 and 2.
    private static readonly byte[] RowAddresses = [0x00, 0x40, 0x14, 0x54];

    // The nibbles that put the chip into 4-bit mode, whatever state it is in.
    private static readonly byte[] FourBitInitNibbles = [0x3, 0x3, 0x3, 0x2];

    // \u0000 is the custom degree glyph.
    private static readonly string[] ScreenLines =
    [
        "12:34:56  14-11-2026",
        "Inside        21.5\u0000C",
        "Outside       -3.2\u0000C",
        "Humidity        48 %",
    ];

    /// <summary>
    /// Gets the rows of the custom degree glyph (slot 0), row 0 at the top, the low 5 bits of each row.
    /// </summary>
    public static IReadOnlyList<byte> DegreeGlyph { get; } = [0x0C, 0x12, 0x12, 0x0C, 0x00, 0x00, 0x00, 0x00];

    /// <summary>
    /// Creates the logical capture: the instructions and data bytes in 8-bit mode, one step each.
    /// </summary>
    /// <returns>The steps.</returns>
    public static IReadOnlyList<Hd44780CaptureStep> CreateLogicalCapture()
    {
        var steps = new List<Hd44780CaptureStep>();
        AddSetup(steps, FunctionSet8BitTwoLines);
        AddContent(steps);
        return steps;
    }

    /// <summary>
    /// Creates the raw PCF8574 capture: the expander bytes, grouped as one chunk per nibble of the initialization,
    /// one for the backlight and one per instruction or data byte afterwards (each nibble is sent with EN high, then EN low).
    /// </summary>
    /// <returns>The chunks of expander bytes.</returns>
    public static IReadOnlyList<byte[]> CreateI2cCapture()
    {
        var chunks = new List<byte[]>
        {
            // LiquidCrystal_I2C begin(): all pins low, backlight off.
            new byte[] { 0x00 },
        };

        foreach (var nibble in FourBitInitNibbles)
        {
            chunks.Add(CreateNibble(nibble, isData: false, isBacklightOn: false));
        }

        var setup = new List<Hd44780CaptureStep>();
        AddSetup(setup, FunctionSet4BitTwoLines);
        chunks.AddRange(setup.Select(step => CreateByte(step, isBacklightOn: false)));

        // lcd.backlight(): one byte with the backlight bit.
        chunks.Add([PinBacklight]);

        var content = new List<Hd44780CaptureStep>();
        AddContent(content);
        chunks.AddRange(content.Select(step => CreateByte(step, isBacklightOn: true)));

        return chunks;
    }

    private static void AddSetup(
        List<Hd44780CaptureStep> steps,
        byte functionSet)
    {
        steps.Add(new Hd44780CaptureStep(IsData: false, functionSet));
        steps.Add(new Hd44780CaptureStep(IsData: false, DisplayOnCursorOff));
        steps.Add(new Hd44780CaptureStep(IsData: false, ClearDisplay));
        steps.Add(new Hd44780CaptureStep(IsData: false, EntryModeIncrement));
    }

    private static void AddContent(List<Hd44780CaptureStep> steps)
    {
        // lcd.createChar(0, degree): the CGRAM address of slot 0, then its 8 rows.
        steps.Add(new Hd44780CaptureStep(IsData: false, SetCgramAddress));
        steps.AddRange(DegreeGlyph.Select(row => new Hd44780CaptureStep(IsData: true, row)));

        // lcd.setCursor(0, row) and lcd.print(line) for each row.
        for (var row = 0; row < ScreenLines.Length; row++)
        {
            steps.Add(new Hd44780CaptureStep(IsData: false, (byte)(SetDdramAddress | RowAddresses[row])));
            steps.AddRange(ScreenLines[row].Select(c => new Hd44780CaptureStep(IsData: true, (byte)c)));
        }
    }

    private static byte[] CreateByte(
        Hd44780CaptureStep step,
        bool isBacklightOn)
        =>
        [
            .. CreateNibble((byte)(step.Value >> NibbleShift), step.IsData, isBacklightOn),
            .. CreateNibble((byte)(step.Value & NibbleMask), step.IsData, isBacklightOn),
        ];

    // As LiquidCrystal_I2C's write4bits: the pins, then EN high and EN low (the falling edge latches the nibble).
    private static byte[] CreateNibble(
        byte nibble,
        bool isData,
        bool isBacklightOn)
    {
        var pins = (byte)(nibble << NibbleShift);
        if (isData)
        {
            pins |= PinRs;
        }

        if (isBacklightOn)
        {
            pins |= PinBacklight;
        }

        return [pins, (byte)(pins | PinEnable), pins];
    }
}