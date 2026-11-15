namespace Atc.Wpf.Controls.Tests.Displays;

public sealed class Ssd1351ControllerTests
{
    private const int Black = 0x000000;
    private const int Red = 0xFF0000;
    private const int Green = 0x00FF00;
    private const int Blue = 0x0000FF;
    private const int White = 0xFFFFFF;

    // ref: §LIB.ASSD1351.2: rotation 0 (upright, start line HEIGHT) and rotation 2 (180 degrees, start line 0).
    private const byte UprightRemap = 0x74;
    private const byte RotatedRemap = 0x66;

    // ref: §SSD1351.4 65k format: C4..C0 B5..B3, B2..B0 A4..A0 (RGB565 big-endian); with A[2] = 1, C is red and A blue.
    private static readonly byte[] Red565 = [0xF8, 0x00];
    private static readonly byte[] Green565 = [0x07, 0xE0];
    private static readonly byte[] Blue565 = [0x00, 0x1F];

    [Fact]
    public void Constructor_NullBuffer_Throws()
        => Assert.Throws<ArgumentNullException>(() => new Ssd1351Controller(null!));

    [Fact]
    public void Constructor_AppliesThePowerOnState()
    {
        // ref: §SSD1351.1: display off, MUX 128, start line 0, window 0..127 / 0..127, normal mode,
        // contrast 0x8A / 0x51 / 0x8A, master contrast 0x0F, unlocked with A2 B1 B3 BB BE C1 inaccessible.
        // (The remap reset byte is ⚠️ derived and not asserted.)
        var buffer = new ColorFrameBuffer(128, 128);

        using var controller = new Ssd1351Controller(buffer);

        Assert.False(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);
        Assert.Equal(byte.MaxValue, buffer.Brightness);
        Assert.Equal(127, controller.MultiplexRatio);
        Assert.Equal(0, controller.StartLine);
        Assert.Equal(127, controller.ColumnEnd);
        Assert.Equal(127, controller.RowEnd);
        Assert.Equal(0x8A, controller.ContrastA);
        Assert.Equal(0x51, controller.ContrastB);
        Assert.Equal(0x8A, controller.ContrastC);
        Assert.Equal(0x0F, controller.MasterContrast);
        Assert.False(controller.IsLocked);
        Assert.False(controller.IsRestrictedAccessible);
        Assert.All(controller.GetRam(), word => Assert.Equal(0, word));
    }

    [Fact]
    public void ParametersAsData_SetTheWindow()
    {
        // ref: §SSD1351.2: "D/C# = 0 for the first byte, D/C# = 1 for other bytes"; 0x15 / 0x75 take start and end as data.
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        AdafruitInit(controller, 128);

        controller.WriteCommand(0x15);
        controller.WriteData([0x0A]);
        controller.WriteData([0x0C]);
        controller.WriteCommand(0x75);
        controller.WriteData([0x05, 0x06]);

        Assert.Equal(10, controller.ColumnStart);
        Assert.Equal(12, controller.ColumnEnd);
        Assert.Equal(5, controller.RowStart);
        Assert.Equal(6, controller.RowEnd);
        Assert.Equal(10, controller.ColumnPointer);
        Assert.Equal(5, controller.RowPointer);

        // ref: §SSD1351.5 horizontal increment: column + 1, at the column end back to the start and row + 1; wraps in the window.
        Send(controller, 0x5C);
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

        // A command byte (D/C low) is never a parameter: 0x15 followed by the command 0xA7 leaves the window and inverts the display.
        controller.WriteCommands([0x15, 0xA7]);

        Assert.True(buffer.IsInverted);
        Assert.Equal(10, controller.ColumnStart);
        Assert.Equal(12, controller.ColumnEnd);
    }

    [Fact]
    public void DataWithout5C_IsNotWrittenToRam()
    {
        // ref: §SSD1351.2 0x5C: "This command must be sent before write data into RAM"; stray data is ignored (⚠️ derived, tested
        // only as "not written").
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        AdafruitInit(controller, 128);
        Send(controller, 0x15, 0x00, 0x7F);
        Send(controller, 0x75, 0x00, 0x7F);

        controller.WriteData(Red565);

        Assert.All(controller.GetRam(), word => Assert.Equal(0, word));
        Assert.Equal(Black, Rgb(buffer, 0, 0));

        Send(controller, 0x5C);
        controller.WriteData(Red565);
        Assert.Equal(Red, Rgb(buffer, 0, 0));

        // ref: §SSD1351.2 0x5C: data is written "until another command is written".
        controller.WriteCommand(0xA6);
        controller.WriteData(Green565);
        Assert.Equal(Black, Rgb(buffer, 1, 0));

        // Data after the last parameter of a command is not RAM data either.
        Send(controller, 0xA1, 0x80);
        controller.WriteData(Green565);
        Assert.Equal(Black, Rgb(buffer, 1, 0));
        Assert.Equal(1, controller.ColumnPointer);
    }

    [Fact]
    public void AdafruitInit_ShowsRamUpright()
    {
        // ref: §LIB.ASSD1351.1: after begin() + setRotation(0): unlocked, restricted commands accessible, display on, MUX 128,
        // remap 0x74, start line HEIGHT (128 -> bit 7 ignored = 0), offset 0, normal mode.
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);

        AdafruitInit(controller, 128);

        Assert.Equal(UprightRemap, controller.Remap);
        Assert.Equal(0, controller.StartLine);
        Assert.Equal(0, controller.DisplayOffset);
        Assert.Equal(127, controller.MultiplexRatio);
        Assert.False(controller.IsLocked);
        Assert.True(controller.IsRestrictedAccessible);
        Assert.True(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);

        // ref: §SSD1351.3 upright reference, §SSD1351.5 upright wiring (⚠️ derived, ledger R14): panel (x, y) shows RAM (x, y);
        // §SSD1351.4: A[2] = 1, C red.
        DrawMarker(controller, 0, 0);
        AssertMarkerAtTopLeft(buffer, 128);
    }

    [Fact]
    public void Remap_Rotation2_ShowsTheRamRotated180Degrees()
    {
        // ref: §LIB.ASSD1351.2, §SSD1351.5: rotation 2 (0x66, start line 0) is rotation 0 turned by 180 degrees.
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        AdafruitInit(controller, 128);
        Send(controller, 0xA0, RotatedRemap);
        Send(controller, 0xA1, 0x00);

        DrawMarker(controller, 0, 0);

        Assert.Equal(Red, Rgb(buffer, 127, 127));
        Assert.Equal(Green, Rgb(buffer, 126, 127));
        Assert.Equal(Blue, Rgb(buffer, 127, 126));
        Assert.Equal(Black, Rgb(buffer, 0, 0));
    }

    [Fact]
    public void Remap_ColumnRemap_MirrorsThePicture()
    {
        // ref: §SSD1351.3: setting A[1] mirrors horizontally. The remap is sent before drawing, so the result is the same whether the
        // bit acts on the display side or on the writes (⚠️ derived which).
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        AdafruitInit(controller, 128);
        Send(controller, 0xA0, UprightRemap | 0x02);

        DrawMarker(controller, 0, 0);

        Assert.Equal(Red, Rgb(buffer, 127, 0));
        Assert.Equal(Green, Rgb(buffer, 126, 0));
        Assert.Equal(Blue, Rgb(buffer, 127, 1));
        Assert.Equal(Black, Rgb(buffer, 0, 0));
    }

    [Fact]
    public void VerticalIncrement_WalksDownTheColumnFirst()
    {
        // ref: §SSD1351.3 A[0] = 1, §SSD1351.5: row + 1 first, then column + 1; wraps in the window.
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        AdafruitInit(controller, 128);
        Send(controller, 0xA0, UprightRemap | 0x01);
        Send(controller, 0x15, 0x00, 0x01);
        Send(controller, 0x75, 0x00, 0x01);
        Send(controller, 0x5C);

        controller.WriteData([.. Red565, .. Green565, .. Blue565]);

        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(Green, Rgb(buffer, 0, 1));
        Assert.Equal(Blue, Rgb(buffer, 1, 0));
        Assert.Equal(Black, Rgb(buffer, 1, 1));
    }

    [Fact]
    public void Remap_262k_TakesThreeBytesInTheLowSixBits()
    {
        // ref: §SSD1351.4 262k format 1 (A[7:6] = 10): three bytes C, B, A with the colour in bits 5:0.
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        AdafruitInit(controller, 128);
        Send(controller, 0xA0, (UprightRemap & 0x3F) | 0x80);
        Send(controller, 0x15, 0x00, 0x02);
        Send(controller, 0x75, 0x00, 0x00);
        Send(controller, 0x5C);

        controller.WriteData([0x3F, 0x00, 0x00, 0xC0, 0x3F]);
        controller.WriteData([0xC0, 0x00, 0x00, 0x3F]);

        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(Green, Rgb(buffer, 1, 0));
        Assert.Equal(Blue, Rgb(buffer, 2, 0));
    }

    [Fact]
    public void WriteData_PixelSplitAcrossCalls_IsJoined()
    {
        // ref: §SSD1351.4: two bytes per 65k pixel; the bytes may arrive in separate transfers.
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        AdafruitInit(controller, 128);
        Send(controller, 0x5C);

        controller.WriteData([Red565[0]]);
        Assert.Equal(Black, Rgb(buffer, 0, 0));
        controller.WriteData([Red565[1], Blue565[0]]);
        controller.WriteData([Blue565[1]]);

        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(Blue, Rgb(buffer, 1, 0));
    }

    [Fact]
    public void CommandLock_IgnoresCommandsUntilUnlocked()
    {
        // ref: §SSD1351.6, §P2.X item 7: after reset A2 B1 B3 BB BE C1 are inaccessible until FD B1.
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        var offset = controller.DisplayOffset;

        Send(controller, 0xC1, 0x10, 0x20, 0x30);
        Send(controller, 0xA2, 0x05);

        Assert.Equal(0x8A, controller.ContrastA);
        Assert.Equal(0x51, controller.ContrastB);
        Assert.Equal(0x8A, controller.ContrastC);
        Assert.Equal(offset, controller.DisplayOffset);

        Send(controller, 0xFD, 0xB1);
        Send(controller, 0xC1, 0x10, 0x20, 0x30);

        Assert.True(controller.IsRestrictedAccessible);
        Assert.Equal(0x10, controller.ContrastA);
        Assert.Equal(0x20, controller.ContrastB);
        Assert.Equal(0x30, controller.ContrastC);

        // ref: §SSD1351.6 0x16: "will not respond to any newly-entered command (except FDh 12h) and there will be no memory access".
        Send(controller, 0xA0, UprightRemap);
        Send(controller, 0xA1, 0x80);
        Send(controller, 0xFD, 0x16);
        Assert.True(controller.IsLocked);

        Send(controller, 0xAF);
        Send(controller, 0xA7);
        Send(controller, 0xC7, 0x03);
        Send(controller, 0x5C);
        controller.WriteData(Red565);
        Send(controller, 0xFD, 0xB0);

        Assert.False(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);
        Assert.Equal(0x0F, controller.MasterContrast);
        Assert.Equal(0, controller.GetRam()[0]);
        Assert.True(controller.IsRestrictedAccessible);
        Assert.True(controller.IsLocked);

        Send(controller, 0xFD, 0x12);
        Send(controller, 0xAF);
        Send(controller, 0x5C);
        controller.WriteData(Red565);

        Assert.False(controller.IsLocked);
        Assert.True(buffer.IsDisplayOn);
        Assert.Equal(Red, Rgb(buffer, 0, 0));
    }

    [Fact]
    public void Panel128x96_ShowsTheConfiguredRows()
    {
        // ref: §SSD1351.5 128 x 96 modules: RAM rows 0..95 are the panel (the library's start line 96 with rotation 0);
        // RAM rows 96..127 are not shown.
        var buffer = new ColorFrameBuffer(128, 96);
        using var controller = new Ssd1351Controller(buffer);
        AdafruitInit(controller, 96);

        Assert.Equal(96, controller.StartLine);

        DrawMarker(controller, 0, 0);
        AssertMarkerAtTopLeft(buffer, 96);

        Send(controller, 0x15, 0x05, 0x05);
        Send(controller, 0x75, 95, 95);
        Send(controller, 0x5C);
        controller.WriteData(Green565);
        Assert.Equal(Green, Rgb(buffer, 5, 95));

        Send(controller, 0x15, 0x07, 0x07);
        Send(controller, 0x75, 100, 100);
        Send(controller, 0x5C);
        controller.WriteData(Blue565);
        for (var y = 0; y < 96; y++)
        {
            Assert.NotEqual(Blue, Rgb(buffer, 7, y));
        }

        // Rotation 2 on the 96-row panel: start line 0, RAM row 95 - y on panel row y.
        Send(controller, 0xA0, RotatedRemap);
        Send(controller, 0xA1, 0x00);
        Assert.Equal(Red, Rgb(buffer, 127, 95));
        Assert.Equal(Green, Rgb(buffer, 122, 0));
    }

    [Fact]
    public void ContrastAndModes_SetBufferState()
    {
        // ref: §LIB.ASSD1351.1: contrast C8 80 C8, master 0x0F; the brightness stays full (ledger R15).
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        AdafruitInit(controller, 128);
        Send(controller, 0x5C);
        controller.WriteData(Red565);
        var ram = controller.GetRam();

        Assert.Equal(0xC8, controller.ContrastA);
        Assert.Equal(0x80, controller.ContrastB);
        Assert.Equal(0xC8, controller.ContrastC);
        Assert.Equal(0x0F, controller.MasterContrast);
        Assert.Equal(byte.MaxValue, buffer.Brightness);

        Send(controller, 0xC7, 0x05);
        Assert.Equal(0x05, controller.MasterContrast);
        Assert.Equal(byte.MaxValue, buffer.Brightness);

        // ref: §SSD1351.2: A4 all off, A5 all on (GS63), A6 normal, A7 inverse; AE / AF sleep on / off. The RAM is not touched.
        Send(controller, 0xA5);
        Assert.Equal(White, Rgb(buffer, 0, 0));
        Assert.Equal(White, Rgb(buffer, 127, 127));

        Send(controller, 0xA4);
        Assert.Equal(Black, Rgb(buffer, 0, 0));

        Send(controller, 0xA7);
        Assert.True(buffer.IsInverted);
        Assert.Equal(Red, Rgb(buffer, 0, 0));

        Send(controller, 0xA6);
        Assert.False(buffer.IsInverted);
        Assert.Equal(Red, Rgb(buffer, 0, 0));

        Send(controller, 0xAE);
        Assert.False(buffer.IsDisplayOn);
        Assert.Equal(ram, controller.GetRam());
    }

    [Fact]
    public void ScrollCommands_AreConsumedAndIgnored()
    {
        // ref: §SSD1351.2: 0x96 takes 5 parameters; 0x9E / 0x9F are ignored (out of scope, design spec).
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        AdafruitInit(controller, 128);
        Send(controller, 0x5C);
        controller.WriteData(Red565);
        var ram = controller.GetRam();
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        Send(controller, 0x96, 0x01, 0x00, 0x80, 0x00, 0x01);
        Send(controller, 0x9F);
        Send(controller, 0x9E);

        Assert.Equal(0, changes);
        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(ram, controller.GetRam());
    }

    [Fact]
    public void WriteCalls_RaiseChangedOnceAndOnlyForVisibleChanges()
    {
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        Send(controller, 0xB6, 0x01);
        Send(controller, 0x15, 0x00, 0x7F);
        Assert.Equal(0, changes);

        controller.WriteCommands([0xAF, 0xA7]);
        Assert.Equal(1, changes);

        Send(controller, 0x5C);
        controller.WriteData(Repeat(Red565, 4));
        Assert.Equal(2, changes);
    }

    [Fact]
    public void Resize_DrawsTheRamAgain()
    {
        var buffer = new ColorFrameBuffer(10, 10);
        using var controller = new Ssd1351Controller(buffer);
        AdafruitInit(controller, 128);
        Send(controller, 0x15, 0x50, 0x50);
        Send(controller, 0x75, 0x7F, 0x7F);
        Send(controller, 0x5C);
        controller.WriteData(Red565);

        buffer.Resize(128, 128);

        Assert.Equal(Red, Rgb(buffer, 0x50, 0x7F));
    }

    [Fact]
    public void Reset_AppliesThePowerOnStateAndKeepsTheRam()
    {
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        AdafruitInit(controller, 128);
        Send(controller, 0x5C);
        controller.WriteData(Red565);

        controller.Reset();

        Assert.False(buffer.IsDisplayOn);
        Assert.False(controller.IsRestrictedAccessible);
        Assert.NotEqual(0, controller.GetRam()[0]);
    }

    [Fact]
    public void UnknownBytes_AreIgnored()
    {
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);

        controller.WriteCommands([0x00, 0x01, 0xFF, 0x00, 0xAF]);

        Assert.True(buffer.IsDisplayOn);
    }

    [Fact]
    public void IsPanelBgr_False_SwapsRedAndBlue()
    {
        // ref: §SSD1351.4, §P2.X item 1: RGB565 host data is correct with A[2] = 1 on the Adafruit modules (the default);
        // a panel wired the other way shows red and blue swapped.
        var buffer = new ColorFrameBuffer(128, 128);
        using var controller = new Ssd1351Controller(buffer);
        Assert.True(controller.IsPanelBgr);
        AdafruitInit(controller, 128);
        DrawMarker(controller, 0, 0);

        controller.IsPanelBgr = false;

        Assert.False(controller.IsPanelBgr);
        Assert.Equal(Blue, Rgb(buffer, 0, 0));
        Assert.Equal(Red, Rgb(buffer, 0, 1));

        // With A[2] = 0 too (sent before drawing), the colours are right again.
        Send(controller, 0xA0, UprightRemap & ~0x04);
        DrawMarker(controller, 10, 10);
        Assert.Equal(Red, Rgb(buffer, 10, 10));
        Assert.Equal(Green, Rgb(buffer, 11, 10));
        Assert.Equal(Blue, Rgb(buffer, 10, 11));
    }

    [Fact]
    public void Dispose_LaterWritesAreIgnored()
    {
        // Disposing makes the controller inert: nothing reaches the RAM or the buffer afterwards.
        var buffer = new ColorFrameBuffer(128, 128);
        var controller = new Ssd1351Controller(buffer);
        controller.Dispose();
        var ram = controller.GetRam();
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        controller.WriteCommand(0xAF);
        controller.WriteCommands([0xA7, 0x5C]);
        controller.WriteData(Repeat(Red565, 4));
        controller.Reset();
        controller.IsPanelBgr = false;

        Assert.Equal(0, changes);
        Assert.Equal(ram, controller.GetRam());
        Assert.False(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);
        Assert.True(controller.IsPanelBgr);
    }

    // ref: §LIB.ASSD1351.1 begin(), command (D/C low) and data (D/C high) as the library sends them, then setRotation(0).
    private static void AdafruitInit(
        Ssd1351Controller controller,
        int height)
    {
        Send(controller, 0xFD, 0x12);
        Send(controller, 0xFD, 0xB1);
        Send(controller, 0xAE);
        Send(controller, 0xB3, 0xF1);
        Send(controller, 0xCA, 0x7F);
        Send(controller, 0xA2, 0x00);
        Send(controller, 0xB5, 0x00);
        Send(controller, 0xAB, 0x01);
        Send(controller, 0xB1, 0x32);
        Send(controller, 0xBE, 0x05);
        Send(controller, 0xA6);
        Send(controller, 0xC1, 0xC8, 0x80, 0xC8);
        Send(controller, 0xC7, 0x0F);
        Send(controller, 0xB4, 0xA0, 0xB5, 0x55);
        Send(controller, 0xB6, 0x01);
        Send(controller, 0xAF);
        Send(controller, 0xA0, UprightRemap);
        Send(controller, 0xA1, (byte)height);
    }

    private static void Send(
        Ssd1351Controller controller,
        byte command,
        params byte[] data)
    {
        controller.WriteCommand(command);
        if (data.Length > 0)
        {
            controller.WriteData(data);
        }
    }

    // ref: §LIB.ASSD1351.3: C 15 D x1 x2 · C 75 D y1 y2 · C 5C, then the pixels as data.
    private static void DrawMarker(
        Ssd1351Controller controller,
        int xStart,
        int yStart)
    {
        (int X, int Y, byte[] Color)[] marker = [(0, 0, Red565), (1, 0, Green565), (0, 1, Blue565)];
        foreach (var (x, y, color) in marker)
        {
            Send(controller, 0x15, (byte)(x + xStart), (byte)(x + xStart));
            Send(controller, 0x75, (byte)(y + yStart), (byte)(y + yStart));
            Send(controller, 0x5C);
            controller.WriteData(color);
        }
    }

    private static void AssertMarkerAtTopLeft(
        ColorFrameBuffer buffer,
        int height)
    {
        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(Green, Rgb(buffer, 1, 0));
        Assert.Equal(Blue, Rgb(buffer, 0, 1));
        Assert.Equal(Black, Rgb(buffer, 1, 1));
        Assert.Equal(Black, Rgb(buffer, 127, 0));
        Assert.Equal(Black, Rgb(buffer, 0, height - 1));
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