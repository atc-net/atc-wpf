namespace Atc.Wpf.Controls.Tests.Displays;

public sealed class Ssd1331ControllerTests
{
    private const int Black = 0x000000;
    private const int Red = 0xFF0000;
    private const int Green = 0x00FF00;
    private const int Blue = 0x0000FF;
    private const int White = 0xFFFFFF;

    // ref: §LIB.ASSD1331.2: rotation 0 (upright) and rotation 2 (180 degrees), 65k, COM split, order CBA (A[2] = 1).
    private const byte UprightRemap = 0x76;
    private const byte RotatedRemap = 0x64;

    // ref: §SSD1331.4 65k format: C4..C0 B5..B3, B2..B0 A4..A0 (RGB565 big-endian); with A[2] = 1, C is red and A blue.
    private static readonly byte[] Red565 = [0xF8, 0x00];
    private static readonly byte[] Green565 = [0x07, 0xE0];
    private static readonly byte[] Blue565 = [0x00, 0x1F];

    // ref: §LIB.ASSD1331.1 begin(), byte for byte (0xA0 without its parameter), then setRotation(0): A0 76.
    private static readonly byte[] AdafruitInit =
    [
        0xAE, 0xA0, 0xA1, 0x00, 0xA2, 0x00, 0xA4, 0xA8, 0x3F, 0xAD, 0x8E, 0xB0, 0x0B, 0xB1, 0x31, 0xB3, 0xF0,
        0x8A, 0x64, 0x8B, 0x78, 0x8C, 0x64, 0xBB, 0x3A, 0xBE, 0x3E, 0x87, 0x06, 0x81, 0x91, 0x82, 0x50, 0x83, 0x7D,
        0xAF, 0xA0, 0x76,
    ];

    [Fact]
    public void Constructor_NullBuffer_Throws()
        => Assert.Throws<ArgumentNullException>(() => new Ssd1331Controller(null!));

    [Fact]
    public void Constructor_AppliesThePowerOnState()
    {
        // ref: §SSD1331.1: display off, remap 0x40, contrast 0x80, master current 0x0F, MUX 64, window 0..95 / 0..63, normal mode.
        var buffer = new ColorFrameBuffer(96, 64);

        using var controller = new Ssd1331Controller(buffer);

        Assert.False(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);
        Assert.Equal(byte.MaxValue, buffer.Brightness);
        Assert.Equal(0x40, controller.Remap);
        Assert.Equal(0x80, controller.ContrastA);
        Assert.Equal(0x80, controller.ContrastB);
        Assert.Equal(0x80, controller.ContrastC);
        Assert.Equal(0x0F, controller.MasterCurrent);
        Assert.Equal(63, controller.MultiplexRatio);
        Assert.Equal(0, controller.StartLine);
        Assert.Equal(0, controller.DisplayOffset);
        Assert.Equal(95, controller.ColumnEnd);
        Assert.Equal(63, controller.RowEnd);
        Assert.False(controller.IsFillEnabled);
        Assert.All(controller.GetRam(), word => Assert.Equal(0, word));
    }

    [Fact]
    public void ParametersInCommandMode_SetTheWindow()
    {
        // ref: §SSD1331.2, §SSD1331.5: 0x15 / 0x75 take their start and end as command bytes and move the pointer to the start.
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, UprightRemap, 0x15, 0x0A, 0x0C, 0x75, 0x05, 0x06]);

        Assert.Equal(10, controller.ColumnStart);
        Assert.Equal(12, controller.ColumnEnd);
        Assert.Equal(5, controller.RowStart);
        Assert.Equal(6, controller.RowEnd);

        // ref: §SSD1331.5 horizontal increment: column + 1, at the column end back to the start and row + 1.
        controller.WriteData(Repeat(Red565, 6));
        for (var y = 5; y <= 6; y++)
        {
            for (var x = 10; x <= 12; x++)
            {
                Assert.Equal(Red, Rgb(buffer, x, y));
            }
        }

        Assert.Equal(Black, Rgb(buffer, 9, 5));
        Assert.Equal(Black, Rgb(buffer, 13, 5));
        Assert.Equal(Black, Rgb(buffer, 10, 4));
        Assert.Equal(Black, Rgb(buffer, 10, 7));
        Assert.Equal(10, controller.ColumnPointer);
        Assert.Equal(5, controller.RowPointer);

        // The full window, sent one command byte at a time.
        foreach (var value in new byte[] { 0x15, 0x00, 0x5F, 0x75, 0x00, 0x3F })
        {
            controller.WriteCommand(value);
        }

        Assert.Equal(0, controller.ColumnStart);
        Assert.Equal(95, controller.ColumnEnd);
        Assert.Equal(0, controller.RowStart);
        Assert.Equal(63, controller.RowEnd);
        controller.WriteData(Green565);
        Assert.Equal(Green, Rgb(buffer, 0, 0));
        Assert.Equal(1, controller.ColumnPointer);
    }

    [Fact]
    public void ParameterBytes_AreTakenWhateverTheirValue()
    {
        // ref: §SSD1331.2: parameter consumption is count-based; 0xAF and 0xA7 here are the column start and end, not commands.
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);

        controller.WriteCommands([0x15, 0xAF, 0xA7]);

        Assert.False(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);
        Assert.Equal(0x2F, controller.ColumnStart);
        Assert.Equal(0x27, controller.ColumnEnd);
    }

    [Fact]
    public void VerticalIncrement_WalksDownTheColumnFirst()
    {
        // ref: §SSD1331.5, §SSD1331.3 A[0] = 1: row + 1, at the row end back to the row start and column + 1.
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, UprightRemap | 0x01, 0x15, 0x00, 0x01, 0x75, 0x00, 0x01]);

        controller.WriteData([.. Red565, .. Green565, .. Blue565]);

        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(Green, Rgb(buffer, 0, 1));
        Assert.Equal(Blue, Rgb(buffer, 1, 0));
        Assert.Equal(Black, Rgb(buffer, 1, 1));
    }

    [Fact]
    public void AdafruitInit_ShowsRamUpright()
    {
        // ref: §LIB.ASSD1331.1: after begin() + setRotation(0): remap 0x76, start line 0, offset 0, MUX 63, display on;
        // the bare 0xA0 takes 0xA1 as its parameter and the following 0x00 is an unknown command (ignored).
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);

        controller.WriteCommands(AdafruitInit);

        Assert.Equal(UprightRemap, controller.Remap);
        Assert.Equal(0, controller.StartLine);
        Assert.Equal(0, controller.DisplayOffset);
        Assert.Equal(63, controller.MultiplexRatio);
        Assert.True(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);
        Assert.Equal(0x06, controller.MasterCurrent);
        Assert.Equal(0x91, controller.ContrastA);
        Assert.Equal(0x50, controller.ContrastB);
        Assert.Equal(0x7D, controller.ContrastC);

        // ref: §SSD1331.3 upright reference (⚠️ derived, global constraints): panel (x, y) shows RAM (x, y); §SSD1331.4: A[2] = 1, C red.
        DrawMarker(controller, 0, 0);
        AssertMarkerAtTopLeft(buffer);
    }

    [Fact]
    public void Remap_Rotation2_ShowsTheRamRotated180Degrees()
    {
        // ref: §SSD1331.3, §LIB.ASSD1331.2: rotation 2 (0x64, A[1] = A[4] = 0) is rotation 0 turned by 180 degrees.
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, RotatedRemap]);

        DrawMarker(controller, 0, 0);

        Assert.Equal(Red, Rgb(buffer, 95, 63));
        Assert.Equal(Green, Rgb(buffer, 94, 63));
        Assert.Equal(Blue, Rgb(buffer, 95, 62));
        Assert.Equal(Black, Rgb(buffer, 0, 0));
    }

    [Fact]
    public void Remap_ColourOrderBitClear_SwapsRedAndBlue()
    {
        // ref: §SSD1331.4: with A[2] = 0, A drives red and C blue, so RGB565 host data shows red and blue swapped (0x72, the RGB build).
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, 0x72, 0x15, 0x00, 0x02, 0x75, 0x00, 0x00]);

        controller.WriteData([.. Red565, .. Green565, .. Blue565]);

        Assert.Equal(Blue, Rgb(buffer, 0, 0));
        Assert.Equal(Green, Rgb(buffer, 1, 0));
        Assert.Equal(Red, Rgb(buffer, 2, 0));
    }

    [Fact]
    public void Remap_256Colour_Rgb332()
    {
        // ref: §SSD1331.4 256-colour format (A[7:6] = 00): one byte C2 C1 C0 B2 B1 B0 A1 A0 per pixel (C red with A[2] = 1).
        // Only saturated values are asserted: the bit replication into the 65k RAM is ⚠️ unverified.
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, UprightRemap & 0x3F, 0x15, 0x00, 0x04, 0x75, 0x00, 0x00]);

        controller.WriteData([0xE0, 0x1C, 0x03, 0xFF, 0x00]);

        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(Green, Rgb(buffer, 1, 0));
        Assert.Equal(Blue, Rgb(buffer, 2, 0));
        Assert.Equal(White, Rgb(buffer, 3, 0));
        Assert.Equal(Black, Rgb(buffer, 4, 0));
        Assert.Equal(0, controller.ColumnPointer);
    }

    [Fact]
    public void Remap_65kFormat2_TakesThreeBytesPerPixel()
    {
        // ref: §SSD1331.4 65k format 2 (A[7:6] = 10): X X C4..C0 X, X X B5..B0, X X A4..A0 X.
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, (UprightRemap & 0x3F) | 0x80, 0x15, 0x00, 0x02, 0x75, 0x00, 0x00]);

        controller.WriteData([0x3E, 0x00, 0x00, 0x00, 0x3F]);
        controller.WriteData([0x00, 0xC0, 0xC0, 0xFE]);

        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(Green, Rgb(buffer, 1, 0));
        Assert.Equal(Blue, Rgb(buffer, 2, 0));
    }

    [Fact]
    public void WriteData_PixelSplitAcrossCalls_IsJoined()
    {
        // ref: §SSD1331.4: two bytes per 65k pixel; the bytes may arrive in separate transfers.
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, UprightRemap]);

        controller.WriteData([Red565[0]]);
        Assert.Equal(Black, Rgb(buffer, 0, 0));
        controller.WriteData([Red565[1], Blue565[0]]);
        controller.WriteData([Blue565[1]]);

        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(Blue, Rgb(buffer, 1, 0));
    }

    [Fact]
    public void DrawLine_DrawsInRam()
    {
        // ref: §SSD1331.6 0x21: col1, row1, col2, row2, C, B, A; C and A in bits 5:1, B in bits 5:0 (0x3E 0x00 0x00 = C 31 = red with A[2] = 1).
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, UprightRemap]);

        controller.WriteCommands([0x21, 0x02, 0x01, 0x05, 0x04, 0x3E, 0x00, 0x00]);
        controller.WriteCommands([0x21, 0x00, 0x0A, 0x03, 0x0A, 0x00, 0x00, 0x3E]);

        for (var i = 0; i <= 3; i++)
        {
            Assert.Equal(Red, Rgb(buffer, 2 + i, 1 + i));
            Assert.Equal(Blue, Rgb(buffer, i, 10));
        }

        Assert.Equal(Black, Rgb(buffer, 3, 1));
        Assert.Equal(Black, Rgb(buffer, 6, 5));
        Assert.Equal(Black, Rgb(buffer, 4, 10));
        Assert.Equal((ushort)0xF800, controller.GetRam()[(1 * 96) + 2]);
    }

    [Fact]
    public void DrawRectangle_WithFill_FillsInside()
    {
        // ref: §SSD1331.6 0x22: col1, row1, col2, row2, outline C B A, fill C B A; the fill only when 0x26 A[0] = 1 (reset: off).
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, UprightRemap]);

        controller.WriteCommands([0x22, 0x00, 0x00, 0x03, 0x03, 0x3E, 0x00, 0x00, 0x00, 0x3F, 0x00]);

        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(Red, Rgb(buffer, 3, 0));
        Assert.Equal(Red, Rgb(buffer, 0, 3));
        Assert.Equal(Red, Rgb(buffer, 3, 3));
        Assert.Equal(Red, Rgb(buffer, 2, 3));
        Assert.Equal(Black, Rgb(buffer, 1, 1));
        Assert.Equal(Black, Rgb(buffer, 2, 2));

        controller.WriteCommands([0x26, 0x01]);
        Assert.True(controller.IsFillEnabled);
        controller.WriteCommands([0x22, 0x10, 0x10, 0x13, 0x13, 0x3E, 0x00, 0x00, 0x00, 0x3F, 0x00]);

        Assert.Equal(Red, Rgb(buffer, 0x10, 0x10));
        Assert.Equal(Red, Rgb(buffer, 0x13, 0x12));
        Assert.Equal(Green, Rgb(buffer, 0x11, 0x11));
        Assert.Equal(Green, Rgb(buffer, 0x12, 0x12));
        Assert.Equal(Black, Rgb(buffer, 0x14, 0x11));
    }

    [Fact]
    public void Copy_Clear_Dim_ChangeTheRegion()
    {
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, UprightRemap, 0x15, 0x00, 0x01, 0x75, 0x00, 0x00]);
        controller.WriteData([.. Red565, .. Green565]);

        // ref: §SSD1331.6 0x23: copies (col1, row1)-(col2, row2) to the top-left (col3, row3).
        controller.WriteCommands([0x23, 0x00, 0x00, 0x01, 0x00, 0x10, 0x08]);

        Assert.Equal(Red, Rgb(buffer, 0x10, 0x08));
        Assert.Equal(Green, Rgb(buffer, 0x11, 0x08));
        Assert.Equal(Red, Rgb(buffer, 0, 0));

        // ref: §SSD1331.6 0x26 A[4] reverse copy: GS0 <-> GS63 per colour (31 - v / 63 - v).
        controller.WriteCommands([0x26, 0x10, 0x23, 0x00, 0x00, 0x01, 0x00, 0x20, 0x08]);

        Assert.Equal(0x00FFFF, Rgb(buffer, 0x20, 0x08));
        Assert.Equal(0xFF00FF, Rgb(buffer, 0x21, 0x08));

        // ref: §SSD1331.6 0x25: the RAM in the window is set to zero.
        controller.WriteCommands([0x25, 0x10, 0x08, 0x10, 0x08]);

        Assert.Equal(Black, Rgb(buffer, 0x10, 0x08));
        Assert.Equal(Green, Rgb(buffer, 0x11, 0x08));

        // ref: §SSD1331.6 0x24, Table 15: GS0..GS15 unchanged, GS60..GS63 -> GS15; a second dim does not change the data.
        // Asserted on the 6-bit B (green) value only; the mapping for the 5-bit A and C is ⚠️ unverified.
        controller.WriteCommands([0x24, 0x11, 0x08, 0x11, 0x08]);

        Assert.Equal(15 << 5, controller.GetRam()[(8 * 96) + 0x11]);
        controller.WriteCommands([0x24, 0x11, 0x08, 0x11, 0x08]);
        Assert.Equal(15 << 5, controller.GetRam()[(8 * 96) + 0x11]);
        Assert.Equal(Green, Rgb(buffer, 1, 0));
    }

    [Fact]
    public void ScrollCommands_AreConsumedAndIgnored()
    {
        // ref: §SSD1331.2: 0x27 takes 5 parameters (consumed even when they look like commands); 0x2E / 0x2F are ignored (design spec).
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, UprightRemap]);
        controller.WriteData(Red565);
        var ram = controller.GetRam();
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        controller.WriteCommands([0x27, 0xA5, 0xA7, 0xAF, 0x25, 0xA6, 0x2F, 0x2E]);

        Assert.Equal(0, changes);
        Assert.False(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);
        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(ram, controller.GetRam());
    }

    [Fact]
    public void DisplayModes_SetTheBufferState()
    {
        // ref: §SSD1331.7: A5 all on (white), A6 all off, A7 inverse, A4 normal; AE / AF display off / on. RAM is not touched.
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, UprightRemap, 0xAF]);
        controller.WriteData(Red565);
        var ram = controller.GetRam();

        Assert.True(buffer.IsDisplayOn);

        controller.WriteCommand(0xA5);
        Assert.Equal(White, Rgb(buffer, 0, 0));
        Assert.Equal(White, Rgb(buffer, 95, 63));

        controller.WriteCommand(0xA6);
        Assert.Equal(Black, Rgb(buffer, 0, 0));

        controller.WriteCommand(0xA7);
        Assert.True(buffer.IsInverted);
        Assert.Equal(Red, Rgb(buffer, 0, 0));

        controller.WriteCommand(0xA4);
        Assert.False(buffer.IsInverted);
        Assert.Equal(Red, Rgb(buffer, 0, 0));

        controller.WriteCommand(0xAE);
        Assert.False(buffer.IsDisplayOn);
        Assert.Equal(ram, controller.GetRam());
    }

    [Fact]
    public void WriteCommands_RaisesChangedOnceAndOnlyForVisibleChanges()
    {
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        controller.WriteCommands([0x81, 0x10, 0xB1, 0x31, 0x15, 0x00, 0x5F]);
        Assert.Equal(0, changes);

        controller.WriteCommands([0xAF, 0xA7, 0x25, 0x00, 0x00, 0x5F, 0x3F]);
        Assert.Equal(1, changes);

        controller.WriteData(Repeat(Red565, 4));
        Assert.Equal(2, changes);
    }

    [Fact]
    public void Resize_DrawsTheRamAgain()
    {
        var buffer = new ColorFrameBuffer(10, 10);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, UprightRemap, 0x15, 0x50, 0x50, 0x75, 0x3F, 0x3F]);
        controller.WriteData(Red565);

        buffer.Resize(96, 64);

        Assert.Equal(Red, Rgb(buffer, 0x50, 0x3F));
    }

    [Fact]
    public void Reset_AppliesThePowerOnStateAndKeepsTheRam()
    {
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands(AdafruitInit);
        controller.WriteData(Red565);

        controller.Reset();

        Assert.Equal(0x40, controller.Remap);
        Assert.False(buffer.IsDisplayOn);
        Assert.Equal(0xF800, controller.GetRam()[0]);
    }

    [Fact]
    public void UnknownBytes_AreIgnored()
    {
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        Assert.False(buffer.IsDisplayOn);

        controller.WriteCommands([0x00, 0x01, 0xFF, 0x00, 0xAF]);

        Assert.True(buffer.IsDisplayOn);
    }

    [Fact]
    public void IsPanelBgr_False_ShowsTheRgbBuildCorrectly()
    {
        // ref: §SSD1331.4, §LIB.ASSD1331.1-2: the SSD1331_COLORORDER_RGB build sends remap 0x72 (A[2] = 0) for modules wired the other way.
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        Assert.True(controller.IsPanelBgr);
        controller.IsPanelBgr = false;
        byte[] rgbInit = [.. AdafruitInit[..^1], 0x72];

        controller.WriteCommands(rgbInit);
        DrawMarker(controller, 0, 0);

        Assert.False(controller.IsPanelBgr);
        Assert.Equal(0x72, controller.Remap);
        AssertMarkerAtTopLeft(buffer);

        // Changing the panel order redraws the stored picture with red and blue swapped.
        controller.IsPanelBgr = true;
        Assert.Equal(Blue, Rgb(buffer, 0, 0));
        Assert.Equal(Red, Rgb(buffer, 0, 1));
    }

    [Fact]
    public void CommandLock_IgnoresCommandsAndDataUntilUnlocked()
    {
        // ref: §SSD1331.2 0xFD: 0x16 locks ("prohibits all commands and memory access except the FDh command"), 0x12 unlocks.
        // ⚠️ derived: the bytes of a command ignored while locked are not counted as its parameters.
        var buffer = new ColorFrameBuffer(96, 64);
        using var controller = new Ssd1331Controller(buffer);
        controller.WriteCommands([0xA0, UprightRemap, 0xFD, 0x16]);

        controller.WriteCommands([0xAF, 0xA7]);
        controller.WriteData(Red565);

        Assert.False(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);
        Assert.Equal(Black, Rgb(buffer, 0, 0));

        controller.WriteCommands([0xFD, 0x12, 0xAF]);
        controller.WriteData(Red565);

        Assert.True(buffer.IsDisplayOn);
        Assert.Equal(Red, Rgb(buffer, 0, 0));
    }

    [Fact]
    public void Dispose_LaterWritesAreIgnored()
    {
        // Disposing makes the controller inert: nothing reaches the RAM or the buffer afterwards.
        var buffer = new ColorFrameBuffer(96, 64);
        var controller = new Ssd1331Controller(buffer);
        controller.Dispose();
        var ram = controller.GetRam();
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        controller.WriteCommand(0xAF);
        controller.WriteCommands([0xA7, 0xA0, 0x76]);
        controller.WriteData(Repeat(Red565, 4));
        controller.Reset();
        controller.IsPanelBgr = false;

        Assert.Equal(0, changes);
        Assert.Equal(ram, controller.GetRam());
        Assert.False(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);
        Assert.True(controller.IsPanelBgr);
        Assert.Equal(0x40, controller.Remap);
    }

    private static void DrawMarker(
        Ssd1331Controller controller,
        int xStart,
        int yStart)
    {
        (int X, int Y, byte[] Color)[] marker = [(0, 0, Red565), (1, 0, Green565), (0, 1, Blue565)];
        foreach (var (x, y, color) in marker)
        {
            controller.WriteCommands([0x15, (byte)(x + xStart), (byte)(x + xStart), 0x75, (byte)(y + yStart), (byte)(y + yStart)]);
            controller.WriteData(color);
        }
    }

    private static void AssertMarkerAtTopLeft(ColorFrameBuffer buffer)
    {
        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(Green, Rgb(buffer, 1, 0));
        Assert.Equal(Blue, Rgb(buffer, 0, 1));
        Assert.Equal(Black, Rgb(buffer, 1, 1));
        Assert.Equal(Black, Rgb(buffer, 95, 0));
        Assert.Equal(Black, Rgb(buffer, 0, 63));
    }

    private static byte[] Repeat(
        byte[] pixel,
        int count)
    {
        var data = new byte[pixel.Length * count];
        for (var i = 0; i < count; i++)
        {
            pixel.CopyTo(data, i * pixel.Length);
        }

        return data;
    }

    private static int Rgb(
        ColorFrameBuffer buffer,
        int x,
        int y)
    {
        var color = buffer.GetPixel(x, y);
        return (color.R << 16) | (color.G << 8) | color.B;
    }
}