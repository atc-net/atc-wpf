namespace Atc.Wpf.Controls.Tests.Displays;

public sealed class TftControllerTests
{
    private const int Black = 0x000000;
    private const int Red = 0xFF0000;
    private const int Green = 0x00FF00;
    private const int Blue = 0x0000FF;
    private const int White = 0xFFFFFF;
    private const int Yellow = 0xFFFF00;

    // ref: §TFT.6 16-bit: high byte first; expanded by bit replication (§TFT.6 note, global constraints).
    private static readonly byte[] Red565 = [0xF8, 0x00];
    private static readonly byte[] Green565 = [0x07, 0xE0];
    private static readonly byte[] Blue565 = [0x00, 0x1F];
    private static readonly byte[] White565 = [0xFF, 0xFF];
    private static readonly byte[] Yellow565 = [0xFF, 0xE0];
    private static readonly byte[] Grey565 = [0x84, 0x10];

    [Fact]
    public void Constructor_NullBuffer_Throws()
        => Assert.Throws<ArgumentNullException>(() => new TftController(null!, TftChip.St7789));

    [Fact]
    public void Constructor_AppliesThePowerOnState()
    {
        // ref: §TFT.5: sleep in, display off, inversion off. RAM extents per chip (§ST7735.1, §ST7789.1, §ILI9341.1);
        // panel colour order per §P2.X item 1. The ST7789 IPS panel is inverted (§ST7789.2 correction), so with the chip's inversion off
        // the buffer shows inverted.
        var buffer = new ColorFrameBuffer(240, 320);

        using var st7789 = new TftController(buffer, TftChip.St7789);

        Assert.True(buffer.IsSleeping);
        Assert.False(buffer.IsDisplayOn);
        Assert.True(buffer.IsInverted);
        Assert.Equal(240, st7789.RamWidth);
        Assert.Equal(320, st7789.RamHeight);
        Assert.False(st7789.IsPanelBgr);

        using var st7735 = new TftController(new ColorFrameBuffer(), TftChip.St7735);
        Assert.Equal(132, st7735.RamWidth);
        Assert.Equal(162, st7735.RamHeight);
        Assert.True(st7735.IsPanelBgr);

        using var ili9341 = new TftController(new ColorFrameBuffer(240, 320), TftChip.Ili9341);
        Assert.True(ili9341.IsPanelBgr);
    }

    [Fact]
    public void CasetRasetRamwr_FillsTheWindow()
    {
        // ref: §TFT.1: CASET XS..XE, RASET YS..YE, RAMWR starts at (XS, YS); X + 1 per pixel, at XE back to XS and Y + 1.
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.St7789);
        Send(controller, 0x3A, 0x55);

        Send(controller, 0x2A, 0x00, 0x0A, 0x00, 0x0C);
        Send(controller, 0x2B, 0x00, 0x05, 0x00, 0x06);
        Send(controller, 0x2C, Repeat(Red565, 6));

        for (var y = 5; y <= 6; y++)
        {
            for (var x = 10; x <= 12; x++)
            {
                Assert.Equal(Red, Ram(buffer, controller, x, y));
            }
        }

        Assert.Equal(Black, Ram(buffer, controller, 9, 5));
        Assert.Equal(Black, Ram(buffer, controller, 13, 5));
        Assert.Equal(Black, Ram(buffer, controller, 10, 4));
        Assert.Equal(Black, Ram(buffer, controller, 10, 7));
    }

    [Fact]
    public void MemoryWrite_PastTheWindow_WrapsToTheWindowStart()
    {
        // ref: §TFT.1 counter table: X > XE -> XS and Y + 1; X > XE and Y > YE -> back to (XS, YS).
        // The table and the RAMWRC text disagree about pixels past one window (⚠️ unverified, §TFT.1); the emulator follows the table,
        // so the fifth pixel of a 2 x 2 window overwrites the window start.
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.St7789);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0x2A, 0x00, 0x03, 0x00, 0x04);
        Send(controller, 0x2B, 0x00, 0x01, 0x00, 0x02);

        Send(controller, 0x2C, [.. Red565, .. Green565, .. Blue565, .. White565, .. Yellow565]);

        Assert.Equal(Yellow, Ram(buffer, controller, 3, 1));
        Assert.Equal(Green, Ram(buffer, controller, 4, 1));
        Assert.Equal(Blue, Ram(buffer, controller, 3, 2));
        Assert.Equal(White, Ram(buffer, controller, 4, 2));
        Assert.Equal(Black, Ram(buffer, controller, 5, 1));
        Assert.Equal(Black, Ram(buffer, controller, 0, 2));
        Assert.Equal(Black, Ram(buffer, controller, 3, 3));
        Assert.Equal(4, controller.ColumnPointer);
        Assert.Equal(1, controller.RowPointer);
    }

    [Theory]
    [InlineData(0x00)]
    [InlineData(0x40)]
    [InlineData(0xC0)]
    public void MemoryWrite_WindowBeyondTheRam_WritesOnlyTheRam(byte madctl)
    {
        // ref: §TFT.1 (DS-7789 §9.1.20): addresses outside the RAM are ignored; the pointer still walks the window as set.
        // MADCTL 0 writes RAM row 0 from column 0; MX (0x40) mirrors the columns (column 239 - X), MX | MY (0xC0) also the rows (row 319 - Y).
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.St7789);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0x36, madctl);
        Send(controller, 0x2A, 0x00, 0x00, 0xFF, 0xFF);
        Send(controller, 0x2B, 0x00, 0x00, 0xFF, 0xFF);

        var exception = Record.Exception(() => Send(controller, 0x2C, Repeat(Green565, 300)));

        Assert.Null(exception);
        Assert.Equal(300, controller.ColumnPointer);
        Assert.Equal(0, controller.RowPointer);
        var row = (madctl & 0x80) != 0 ? 319 : 0;
        var ram = controller.GetRam();
        for (var index = 0; index < ram.Length; index++)
        {
            Assert.Equal(index / 240 == row ? Green : Black, ram[index]);
        }
    }

    [Fact]
    public void WriteContinue_ContinuesAfterTheLastPixel()
    {
        // ref: §TFT.1 RAMWRC (0x3C, ST7789VW and ILI9341): continues after the last pixel without resetting to (XS, YS).
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.St7789);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0x03);
        Send(controller, 0x2B, 0x00, 0x00, 0x00, 0x00);

        Send(controller, 0x2C, Repeat(Red565, 2));
        Send(controller, 0x3C, Repeat(Green565, 2));

        Assert.Equal(Red, Ram(buffer, controller, 0, 0));
        Assert.Equal(Red, Ram(buffer, controller, 1, 0));
        Assert.Equal(Green, Ram(buffer, controller, 2, 0));
        Assert.Equal(Green, Ram(buffer, controller, 3, 0));
    }

    [Fact]
    public void Madctl_MvSwapsRowsAndColumns()
    {
        // ref: §TFT.1 logical to physical, MV = 1, MX = MY = 0: physical column = Y, physical row = X; X still walks first.
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.St7789);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0x36, 0x20);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0x02);
        Send(controller, 0x2B, 0x00, 0x00, 0x00, 0x01);

        Send(controller, 0x2C, [.. Red565, .. Green565, .. Blue565, .. White565, .. Yellow565, .. Grey565]);

        Assert.Equal(Red, Ram(buffer, controller, 0, 0));
        Assert.Equal(Green, Ram(buffer, controller, 0, 1));
        Assert.Equal(Blue, Ram(buffer, controller, 0, 2));
        Assert.Equal(White, Ram(buffer, controller, 1, 0));
        Assert.Equal(0xFFFF00, Ram(buffer, controller, 1, 1));
        Assert.Equal(0x848284, Ram(buffer, controller, 1, 2));
        Assert.Equal(Black, Ram(buffer, controller, 2, 0));
        Assert.Equal(Black, Ram(buffer, controller, 0, 3));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void St7789_135x240_EveryRotationLandsInThePanel(int rotation)
    {
        // ref: §ST7789.4 rotation table (MADCTL, xstart, ystart, logical size) and its checks through §TFT.1 (Cmax 239, Rmax 319):
        // every rotation fills exactly the window RAM columns 52..186, rows 40..279, which the panel shows (offsets 52 / 40).
        // Logical (0, 0) in RAM: r0 (MX|MY) col 239-53 = 186, row 319-40 = 279; r1 (MY|MV) col 0+52, row 319-40 = 279;
        // r2 (none) col 52, row 40; r3 (MX|MV) col 239-53 = 186, row 0+40 = 40. The panel is wired rotated 180 degrees from RAM order
        // (§P2.X item 8: rotation 0 is upright), so window (k, j) is panel (134 - k, 239 - j): r0 top-left, r1 top-right,
        // r2 bottom-right, r3 bottom-left, the corners a user sees on the module.
        (byte Madctl, int XStart, int YStart, int Width, int Height, int CornerX, int CornerY)[] rotations =
        [
            (0xC0, 53, 40, 135, 240, 0, 0),
            (0xA0, 40, 52, 240, 135, 134, 0),
            (0x00, 52, 40, 135, 240, 134, 239),
            (0x60, 40, 53, 240, 135, 0, 239),
        ];
        var (madctl, xStart, yStart, width, height, cornerX, cornerY) = rotations[rotation];
        var buffer = new ColorFrameBuffer(135, 240);
        using var controller = new TftController(buffer, TftChip.St7789, columnOffset: 52, rowOffset: 40);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0x36, madctl);
        Send(controller, 0x2A, Word(xStart), Word(xStart + width - 1));
        Send(controller, 0x2B, Word(yStart), Word(yStart + height - 1));

        Send(controller, 0x2C, [.. Red565, .. Repeat(White565, (width * height) - 1)]);

        for (var y = 0; y < 240; y++)
        {
            for (var x = 0; x < 135; x++)
            {
                var expected = x == cornerX && y == cornerY
                    ? Red
                    : White;
                Assert.Equal(expected, Rgb(buffer, x, y));
            }
        }
    }

    [Fact]
    public void AdafruitSt7735Init_RedFillShowsRed()
    {
        // ref: §LIB.AST7735.1 Rcmd1 + Rcmd2green + Rcmd3, then setRotation(0) = MADCTL 0xC8 (§LIB.AST7735.2), then fillScreen:
        // setAddrWindow(0, 0, 128, 160) + (xstart 2, ystart 1) (§LIB.AST7735.3). 1.8" green tab: window cols 2..129, rows 1..160 of the
        // 132 x 162 RAM (§ST7735.4). MADCTL BGR set and a BGR panel (§ST7735.3, §P2.X item 1): no swap, so red stays red.
        var buffer = new ColorFrameBuffer(128, 160);
        using var controller = new TftController(buffer, TftChip.St7735, columnOffset: 2, rowOffset: 1);

        Send(controller, 0x01);
        Send(controller, 0x11);
        Send(controller, 0xB1, 0x01, 0x2C, 0x2D);
        Send(controller, 0xB2, 0x01, 0x2C, 0x2D);
        Send(controller, 0xB3, 0x01, 0x2C, 0x2D, 0x01, 0x2C, 0x2D);
        Send(controller, 0xB4, 0x07);
        Send(controller, 0xC0, 0xA2, 0x02, 0x84);
        Send(controller, 0xC1, 0xC5);
        Send(controller, 0xC2, 0x0A, 0x00);
        Send(controller, 0xC3, 0x8A, 0x2A);
        Send(controller, 0xC4, 0x8A, 0xEE);
        Send(controller, 0xC5, 0x0E);
        Send(controller, 0x20);
        Send(controller, 0x36, 0xC8);
        Send(controller, 0x3A, 0x05);
        Send(controller, 0x2A, 0x00, 0x02, 0x00, 0x81);
        Send(controller, 0x2B, 0x00, 0x01, 0x00, 0xA0);
        Send(controller, 0xE0, 0x02, 0x1C, 0x07, 0x12, 0x37, 0x32, 0x29, 0x2D, 0x29, 0x25, 0x2B, 0x39, 0x00, 0x01, 0x03, 0x10);
        Send(controller, 0xE1, 0x03, 0x1D, 0x07, 0x06, 0x2E, 0x2C, 0x29, 0x2D, 0x2E, 0x2E, 0x37, 0x3F, 0x00, 0x00, 0x02, 0x10);
        Send(controller, 0x13);
        Send(controller, 0x29);
        Send(controller, 0x36, 0xC8);
        Send(controller, 0x2A, 0x00, 0x02, 0x00, 0x81);
        Send(controller, 0x2B, 0x00, 0x01, 0x00, 0xA0);
        Send(controller, 0x2C, Repeat(Red565, 128 * 160));

        Assert.True(buffer.IsDisplayOn);
        Assert.False(buffer.IsSleeping);
        Assert.False(buffer.IsInverted);
        for (var y = 0; y < 160; y++)
        {
            for (var x = 0; x < 128; x++)
            {
                Assert.Equal(Red, Rgb(buffer, x, y));
            }
        }

        // ref: §TFT.1: BGR is a live mapping; with an RGB panel the same RAM shows R and B swapped.
        controller.IsPanelBgr = false;
        Assert.Equal(Blue, Rgb(buffer, 0, 0));
    }

    [Fact]
    public void Colmod18Bit_TakesThreeBytesPerPixel()
    {
        // ref: §TFT.6 18-bit: 3 bytes R, G, B, the upper 6 bits of each carry the colour (D1:D0 don't care);
        // 6 -> 8 bits by replication. 0x87 -> 0x21 -> 0x86, 0x40 -> 0x10 -> 0x41, 0xFF -> 0x3F -> 0xFF.
        // The bytes of a pixel may arrive in separate data writes.
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.St7789);
        Send(controller, 0x3A, 0x66);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0x01);
        Send(controller, 0x2B, 0x00, 0x00, 0x00, 0x00);
        Send(controller, 0x2C);

        controller.WriteData([0x87]);
        controller.WriteData([0x40, 0xFF, 0xFC]);
        controller.WriteData([0x00, 0x00]);

        Assert.Equal(0x8641FF, Ram(buffer, controller, 0, 0));
        Assert.Equal(Red, Ram(buffer, controller, 1, 0));
        Assert.Equal(Black, Ram(buffer, controller, 2, 0));
    }

    [Fact]
    public void InvertSleepDisplayOff_SetBufferState()
    {
        // ref: §TFT.2 (0x10/0x11, 0x20/0x21, 0x28/0x29), §TFT.7 (0x51 brightness), §TFT.5 (SWRESET: sleep in, display off, inversion off).
        // A panel that is not inverted, so the buffer follows the chip's inversion state.
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.St7789);
        controller.IsPanelInverted = false;

        Send(controller, 0x11);
        Assert.False(buffer.IsSleeping);
        Send(controller, 0x29);
        Assert.True(buffer.IsDisplayOn);
        Send(controller, 0x21);
        Assert.True(buffer.IsInverted);
        Send(controller, 0x51, 0x80);
        Assert.Equal(0x80, buffer.Brightness);
        Send(controller, 0x20);
        Assert.False(buffer.IsInverted);
        Send(controller, 0x28);
        Assert.False(buffer.IsDisplayOn);
        Send(controller, 0x10);
        Assert.True(buffer.IsSleeping);

        controller.WriteCommands([0x11, 0x29, 0x21]);
        Send(controller, 0x01);
        Assert.True(buffer.IsSleeping);
        Assert.False(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);
    }

    [Fact]
    public void VerticalScroll_ShiftsTheScrollArea()
    {
        // ref: §TFT.4: TFA = 0, VSA = 320, VSP = 40 shows RAM row 40 on display line 0 (the datasheet figure, DS-7789 P 130).
        // Scrolling re-maps the display only; the RAM is unchanged, so VSP 0 shows the RAM as written again. MV = 0, ML = 0.
        // The ILI9341 panel is mirrored in X only (§P2.X item 8, rotation 0 = MX), so RAM column 0 is panel column 239 and rows keep their order.
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.Ili9341);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0x36, 0x08);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0x00);
        Send(controller, 0x2B, 0x00, 0x28, 0x00, 0x28);
        Send(controller, 0x2C, Red565);
        Assert.Equal(Red, Rgb(buffer, 239, 40));

        Send(controller, 0x33, 0x00, 0x00, 0x01, 0x40, 0x00, 0x00);
        Send(controller, 0x37, 0x00, 0x28);

        Assert.Equal(Red, Rgb(buffer, 239, 0));
        Assert.Equal(Black, Rgb(buffer, 239, 40));

        Send(controller, 0x37, 0x00, 0x00);

        Assert.Equal(Black, Rgb(buffer, 239, 0));
        Assert.Equal(Red, Rgb(buffer, 239, 40));
    }

    [Fact]
    public void NewCommand_DropsThePendingParameters()
    {
        // ref: §TFT.2: a new command byte ends the previous command; the data after it belongs to the new command.
        // COLMOD (1 parameter) is interrupted by RAMWR: the pixel bytes are pixels, not the COLMOD parameter (16-bit stays).
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.St7789);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0x03);
        Send(controller, 0x2B, 0x00, 0x00, 0x00, 0x00);

        Send(controller, 0x3A);
        Send(controller, 0x2C, [.. Red565, .. Green565]);

        Assert.Equal(Red, Ram(buffer, controller, 0, 0));
        Assert.Equal(Green, Ram(buffer, controller, 1, 0));
        Assert.Equal(Black, Ram(buffer, controller, 2, 0));
    }

    [Fact]
    public void VendorCommands_AreConsumedAndIgnored()
    {
        // ref: §ST7735.2 vendor tuning commands (FRMCTR1 0xB1 with 01 2C 2D, GMCTRP1 0xE0 ...) take D/C-high parameters and have
        // no visual effect; §TFT.2: a command ends the RAM write, so the parameter 0x2C is data, not a new RAMWR, and the
        // data after the vendor command does not reach the RAM.
        var buffer = new ColorFrameBuffer(128, 160);
        using var controller = new TftController(buffer, TftChip.St7735);
        Send(controller, 0x3A, 0x05);
        Send(controller, 0x36, 0x08);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0x03);
        Send(controller, 0x2B, 0x00, 0x00, 0x00, 0x00);
        Send(controller, 0x2C, Red565);
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        Send(controller, 0xB1, 0x01, 0x2C, 0x2D);
        controller.WriteData(White565);
        Send(controller, 0xE0, 0x02, 0x1C, 0x07, 0x12, 0x37, 0x32, 0x29, 0x2D, 0x29, 0x25, 0x2B, 0x39, 0x00, 0x01, 0x03, 0x10);
        Send(controller, 0xFC, 0x11, 0x15);
        controller.WriteData(White565);

        Assert.Equal(0, changes);
        Assert.Equal(Red, Ram(buffer, controller, 0, 0));
        Assert.Equal(Black, Ram(buffer, controller, 1, 0));
        Assert.Equal(Black, Ram(buffer, controller, 2, 0));
    }

    [Fact]
    public void St7735_HasNoWriteContinue()
    {
        // ref: §TFT.1, §ST7735.2: the ST7735S has no 0x3C; its data is ignored.
        var buffer = new ColorFrameBuffer(128, 160);
        using var controller = new TftController(buffer, TftChip.St7735);
        Send(controller, 0x3A, 0x05);
        Send(controller, 0x36, 0x08);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0x03);
        Send(controller, 0x2B, 0x00, 0x00, 0x00, 0x00);
        Send(controller, 0x2C, Red565);

        Send(controller, 0x3C, White565);

        Assert.Equal(Red, Ram(buffer, controller, 0, 0));
        Assert.Equal(Black, Ram(buffer, controller, 1, 0));
    }

    [Fact]
    public void St7735Mini_RamSizeSetsTheMirrorExtents()
    {
        // ref: §ST7735.4: 0.96" 80 x 160 INITR_MINI160x80 has the 128 x 160 RAM (Cmax 127, Rmax 159) and the window at col 24 / row 0.
        // setRotation(0) = MADCTL 0xC0 with (xstart, ystart) = (24, 0) (§LIB.AST7735.2): cols 127 - (x + 24) = 103..24 fill the window.
        // Logical (0, 0) -> col 103, row 159: the last window column and row, the panel's top-left (§P2.X item 8, panel rotated 180 degrees).
        var buffer = new ColorFrameBuffer(80, 160);
        using var controller = new TftController(buffer, TftChip.St7735, columnOffset: 24, rowOffset: 0);
        controller.SetRamSize(128, 160);
        controller.IsPanelBgr = false;
        Send(controller, 0x3A, 0x05);
        Send(controller, 0x36, 0xC0);
        Send(controller, 0x2A, 0x00, 0x18, 0x00, 0x67);
        Send(controller, 0x2B, 0x00, 0x00, 0x00, 0x9F);

        Send(controller, 0x2C, [.. Red565, .. Repeat(White565, (80 * 160) - 1)]);

        Assert.Equal(128, controller.RamWidth);
        Assert.Equal(160, controller.RamHeight);
        for (var y = 0; y < 160; y++)
        {
            for (var x = 0; x < 80; x++)
            {
                var expected = x == 0 && y == 0
                    ? Red
                    : White;
                Assert.Equal(expected, Rgb(buffer, x, y));
            }
        }
    }

    [Fact]
    public void BufferResized_KeepsTheRamAndRemaps()
    {
        // ref: §ST7789.4: the 135 x 240 panel shows RAM cols 52..186, rows 40..279; RAM outside the window is kept, not shown.
        // The panel is rotated 180 degrees from RAM order (§P2.X item 8): window (k, j) is panel (width - 1 - k, 239 - j).
        var buffer = new ColorFrameBuffer(135, 240);
        using var controller = new TftController(buffer, TftChip.St7789, columnOffset: 52, rowOffset: 40);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0x2A, Word(187), Word(187));
        Send(controller, 0x2B, Word(40), Word(40));
        Send(controller, 0x2C, Red565);
        Send(controller, 0x2A, Word(52), Word(52));
        Send(controller, 0x2C, Green565);
        Assert.Equal(Green, Rgb(buffer, 134, 239));
        Assert.Equal(Black, Rgb(buffer, 0, 239));

        buffer.Resize(136, 240);

        Assert.Equal(Green, Rgb(buffer, 135, 239));
        Assert.Equal(Red, Rgb(buffer, 0, 239));
    }

    [Fact]
    public void Reset_KeepsTheRamAndAppliesThePowerOnState()
    {
        // ref: §TFT.5: frame memory is not cleared by a reset; sleep in, display off. Power-on MADCTL 0 (hardware reset).
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.St7789);
        Send(controller, 0x3A, 0x55);
        controller.WriteCommands([0x11, 0x29]);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0x00);
        Send(controller, 0x2B, 0x00, 0x00, 0x00, 0x00);
        Send(controller, 0x2C, Red565);

        controller.Reset();

        Assert.Equal(Red, Ram(buffer, controller, 0, 0));
        Assert.True(buffer.IsSleeping);
        Assert.False(buffer.IsDisplayOn);
        Assert.Equal(0x00, controller.Madctl);
    }

    [Fact]
    public void SoftwareReset_KeepsMadctlOnSt7789AndClearsItOnSt7735()
    {
        // ref: §TFT.5: SWRESET leaves MADCTL unchanged on the ST7789VW / ILI9341 and sets it to 0 on the ST7735S.
        using var st7789 = new TftController(new ColorFrameBuffer(240, 320), TftChip.St7789);
        using var st7735 = new TftController(new ColorFrameBuffer(), TftChip.St7735);
        Send(st7789, 0x36, 0xC0);
        Send(st7735, 0x36, 0xC8);

        Send(st7789, 0x01);
        Send(st7735, 0x01);

        Assert.Equal(0xC0, st7789.Madctl);
        Assert.Equal(0x00, st7735.Madctl);
    }

    [Fact]
    public void AdafruitSt7735Rotation0_MarkerIsTopLeftAndNotMirrored()
    {
        // ref: §LIB.AST7735.1 green tab init, §LIB.AST7735.2 setRotation(0) = 0xC8 with (xstart, ystart) = (2, 1), §LIB.AST7735.3 window,
        // §P2.X item 8: rotation 0 is upright. Marker: red at logical (0, 0), green at (1, 0), blue at (0, 1).
        var buffer = new ColorFrameBuffer(128, 160);
        using var controller = new TftController(buffer, TftChip.St7735, columnOffset: 2, rowOffset: 1);
        controller.WriteCommands([0x01, 0x11]);
        Send(controller, 0x3A, 0x05);
        Send(controller, 0x2A, 0x00, 0x02, 0x00, 0x81);
        Send(controller, 0x2B, 0x00, 0x01, 0x00, 0xA0);
        controller.WriteCommands([0x13, 0x29]);
        Send(controller, 0x36, 0xC8);

        DrawMarker(controller, xStart: 2, yStart: 1);

        AssertMarkerAtTopLeft(buffer);
    }

    [Fact]
    public void AdafruitSt7789Rotation0_MarkerIsTopLeftAndNotMirrored()
    {
        // ref: §LIB.AST7789.1 init stream, §ST7789.4 rotation 0 = 0xC0 with (xstart, ystart) = (53, 40) for 135 x 240, §P2.X item 8.
        var buffer = new ColorFrameBuffer(135, 240);
        using var controller = new TftController(buffer, TftChip.St7789, columnOffset: 52, rowOffset: 40);
        controller.WriteCommands([0x01, 0x11]);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0x36, 0x08);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0xF0);
        Send(controller, 0x2B, 0x00, 0x00, 0x01, 0x40);
        controller.WriteCommands([0x21, 0x13, 0x29]);
        Send(controller, 0x36, 0xC0);

        DrawMarker(controller, xStart: 53, yStart: 40);

        AssertMarkerAtTopLeft(buffer);
    }

    [Fact]
    public void AdafruitIli9341Rotation0_MarkerIsTopLeftAndNotMirrored()
    {
        // ref: §LIB.AILI9341.1 init (vendor bytes, MADCTL 0x48 = rotation 0, VSCSAD with one byte, COLMOD 0x55), §LIB.AILI9341.2 no offsets,
        // §P2.X item 8.
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.Ili9341);
        Send(controller, 0x01);
        Send(controller, 0xEF, 0x03, 0x80, 0x02);
        Send(controller, 0xC0, 0x23);
        Send(controller, 0x36, 0x48);
        Send(controller, 0x37, 0x00);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0xB1, 0x00, 0x18);
        Send(controller, 0x26, 0x01);
        controller.WriteCommands([0x11, 0x29]);

        DrawMarker(controller, xStart: 0, yStart: 0);

        AssertMarkerAtTopLeft(buffer);
    }

    [Fact]
    public void AdafruitSt7789Init_RedFillShowsRed()
    {
        // ref: §LIB.AST7789.1 generic_st7789 sends 0x21 (INVON) because the IPS panels of the ST7789 modules show inverted colours without it
        // (§ST7789.2 correction). The default IsPanelInverted of the ST7789 cancels it: the buffer is not inverted, so red RAM shows red, not cyan.
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.St7789);
        Assert.True(controller.IsPanelInverted);

        SendAdafruitSt7789InitAndRedFill(controller);

        Assert.True(buffer.IsDisplayOn);
        Assert.False(buffer.IsSleeping);
        Assert.False(buffer.IsInverted);
        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(Red, Rgb(buffer, 239, 319));
    }

    [Fact]
    public void IsPanelInvertedFalse_ShowsTheInversionOn()
    {
        // ref: §TFT.7: INVON on a panel that is not inverted (TN) shows the complement of the stored colour.
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.St7789);
        SendAdafruitSt7789InitAndRedFill(controller);
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        controller.IsPanelInverted = false;

        Assert.False(controller.IsPanelInverted);
        Assert.True(buffer.IsInverted);
        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(1, changes);

        // The same value again changes nothing visible.
        controller.IsPanelInverted = false;
        Assert.Equal(1, changes);

        // The panel flag is XORed with the chip's own state: INVOFF on the TN panel shows the colours as stored.
        Send(controller, 0x20);
        Assert.False(buffer.IsInverted);
    }

    [Fact]
    public void IsPanelInverted_DefaultsFalseForSt7735AndIli9341()
    {
        // ref: §ST7735.3, §ILI9341.3: the libraries send INVOFF (Rcmd1 0x20) or nothing for these modules, so their panels are not inverted.
        using var st7735 = new TftController(new ColorFrameBuffer(), TftChip.St7735);
        using var ili9341 = new TftController(new ColorFrameBuffer(240, 320), TftChip.Ili9341);

        Assert.False(st7735.IsPanelInverted);
        Assert.False(ili9341.IsPanelInverted);
    }

    [Fact]
    public void FullFrame_DoesNotAllocatePerPixel()
    {
        // Global constraints: the steady-state frame update allocates under 1 KB (76,800 pixels: nothing per pixel or per row).
        var buffer = new ColorFrameBuffer(240, 320);
        using var controller = new TftController(buffer, TftChip.Ili9341);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0xEF);
        Send(controller, 0x2B, 0x00, 0x00, 0x01, 0x3F);
        var first = Repeat(Red565, 240 * 320);
        var second = Repeat(Blue565, 240 * 320);
        Send(controller, 0x2C, first);
        Send(controller, 0x2C, second);

        var before = GC.GetAllocatedBytesForCurrentThread();
        controller.WriteCommand(0x2C);
        controller.WriteData(first);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.True(allocated < 1024, $"Allocated {allocated} bytes.");
    }

    [Fact]
    public void SetRamSize_Shrink_ClampsTheOffsetsIntoTheRam()
    {
        // The offsets must stay inside the RAM (the rule of the ColumnOffset / RowOffset setters): a shrink clamps them to the last column / row.
        var buffer = new ColorFrameBuffer(128, 160);
        using var controller = new TftController(buffer, TftChip.St7735, columnOffset: 131, rowOffset: 161);

        controller.SetRamSize(128, 160);

        Assert.Equal(127, controller.ColumnOffset);
        Assert.Equal(159, controller.RowOffset);
        var exception = Record.Exception(() =>
        {
            var column = controller.ColumnOffset;
            var row = controller.RowOffset;
            controller.ColumnOffset = column;
            controller.RowOffset = row;
        });
        Assert.Null(exception);

        // Offsets that fit are kept.
        controller.ColumnOffset = 2;
        controller.RowOffset = 1;
        controller.SetRamSize(132, 132);
        Assert.Equal(2, controller.ColumnOffset);
        Assert.Equal(1, controller.RowOffset);
    }

    [Fact]
    public void SetRamSize_DuringAMemoryWrite_EndsTheWrite()
    {
        // Like a reset (§TFT.5), a new RAM size ends a memory write and drops a half pixel: later data is ignored until the next 0x2C.
        var buffer = new ColorFrameBuffer(240, 240);
        using var controller = new TftController(buffer, TftChip.St7789);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0x2C, [.. Red565, Green565[0]]);

        controller.SetRamSize(240, 240);
        controller.WriteData(Repeat(Blue565, 4));

        var ram = controller.GetRam();
        Assert.Equal(Red, ram[0]);
        Assert.All(ram.AsSpan(1).ToArray(), value => Assert.Equal(Black, value));

        Send(controller, 0x2C, Blue565);
        Assert.Equal(Blue, controller.GetRam()[0]);
    }

    [Fact]
    public void Dispose_LaterWritesAreIgnored()
    {
        // Disposing makes the controller inert: nothing reaches the RAM or the buffer afterwards.
        var buffer = new ColorFrameBuffer(240, 320);
        var controller = new TftController(buffer, TftChip.St7789);
        controller.Dispose();
        var ram = controller.GetRam();
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        controller.WriteCommand(0x29);
        controller.WriteCommands([0x11, 0x21, 0x2C]);
        controller.WriteData(Repeat(Red565, 4));
        controller.Reset();
        controller.ColumnOffset = 52;
        controller.RowOffset = 40;
        controller.IsPanelBgr = true;
        controller.IsPanelInverted = false;
        controller.SetRamSize(135, 240);

        Assert.Equal(0, changes);
        Assert.Equal(ram, controller.GetRam());
        Assert.False(buffer.IsDisplayOn);
        Assert.True(buffer.IsSleeping);
        Assert.True(buffer.IsInverted);
        Assert.Equal(0, controller.ColumnOffset);
        Assert.Equal(0, controller.RowOffset);
        Assert.False(controller.IsPanelBgr);
        Assert.True(controller.IsPanelInverted);
        Assert.Equal(240, controller.RamWidth);
    }

    private static void Send(
        TftController controller,
        byte command,
        params byte[] data)
    {
        controller.WriteCommand(command);
        if (data.Length > 0)
        {
            controller.WriteData(data);
        }
    }

    private static void Send(
        TftController controller,
        byte command,
        byte[] first,
        byte[] second)
        => Send(controller, command, [.. first, .. second]);

    private static byte[] Word(int value)
        => [(byte)(value >> 8), (byte)value];

    // ref: §LIB.AST7789.1 generic_st7789, setRotation(0) = 0xC0 (§ST7789.4), then fillScreen(RED) over 240 x 320.
    private static void SendAdafruitSt7789InitAndRedFill(
        TftController controller)
    {
        controller.WriteCommands([0x01, 0x11]);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0x36, 0x08);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0xF0);
        Send(controller, 0x2B, 0x00, 0x00, 0x01, 0x40);
        controller.WriteCommands([0x21, 0x13, 0x29]);
        Send(controller, 0x36, 0xC0);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0xEF);
        Send(controller, 0x2B, 0x00, 0x00, 0x01, 0x3F);
        Send(controller, 0x2C, Repeat(Red565, 240 * 320));
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

    // Draws red at logical (0, 0), green at (1, 0) and blue at (0, 1), as Adafruit drawPixel does: setAddrWindow(x, y, 1, 1) + offsets, RAMWR.
    private static void DrawMarker(
        TftController controller,
        int xStart,
        int yStart)
    {
        (int X, int Y, byte[] Color)[] marker = [(0, 0, Red565), (1, 0, Green565), (0, 1, Blue565)];
        foreach (var (x, y, color) in marker)
        {
            Send(controller, 0x2A, Word(x + xStart), Word(x + xStart));
            Send(controller, 0x2B, Word(y + yStart), Word(y + yStart));
            Send(controller, 0x2C, color);
        }
    }

    private static void AssertMarkerAtTopLeft(ColorFrameBuffer buffer)
    {
        Assert.Equal(Red, Rgb(buffer, 0, 0));
        Assert.Equal(Green, Rgb(buffer, 1, 0));
        Assert.Equal(Blue, Rgb(buffer, 0, 1));
        Assert.Equal(Black, Rgb(buffer, 1, 1));
        Assert.Equal(Black, Rgb(buffer, buffer.Width - 1, 0));
        Assert.Equal(Black, Rgb(buffer, 0, buffer.Height - 1));
    }

    // The panel pixel that shows RAM (column, row) (§P2.X item 8, ⚠️ derived): every panel is mirrored in X; ST7735 / ST7789 also in Y.
    private static int Ram(
        ColorFrameBuffer buffer,
        TftController controller,
        int column,
        int row)
    {
        var x = buffer.Width - 1 - (column - controller.ColumnOffset);
        var y = controller.Chip == TftChip.Ili9341
            ? row - controller.RowOffset
            : buffer.Height - 1 - (row - controller.RowOffset);
        return Rgb(buffer, x, y);
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