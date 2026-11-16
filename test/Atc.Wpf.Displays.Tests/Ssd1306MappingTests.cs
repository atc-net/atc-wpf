namespace Atc.Wpf.Displays.Tests;

public sealed class Ssd1306MappingTests
{
    private const int Columns = 128;

    // ref: §LIB.A1306.1, the 128x64 stream with switch-cap VCC.
    private static readonly byte[] AdafruitInit128x64 =
    [
        0xAE, 0xD5, 0x80, 0xA8, 0x3F, 0xD3, 0x00, 0x40, 0x8D, 0x14, 0x20, 0x00, 0xA1, 0xC8, 0xDA, 0x12,
        0x81, 0xCF, 0xD9, 0xF1, 0xDB, 0x40, 0xA4, 0xA6, 0x2E, 0xAF,
    ];

    // ref: §LIB.A1306.1, the 128x32 stream with switch-cap VCC.
    private static readonly byte[] AdafruitInit128x32 =
    [
        0xAE, 0xD5, 0x80, 0xA8, 0x1F, 0xD3, 0x00, 0x40, 0x8D, 0x14, 0x20, 0x00, 0xA1, 0xC8, 0xDA, 0x02,
        0x81, 0x8F, 0xD9, 0xF1, 0xDB, 0x40, 0xA4, 0xA6, 0x2E, 0xAF,
    ];

    // ref: §LIB.U8G2.1, the SSD1306 128x64 noname init followed by power-save off (0xAF).
    private static readonly byte[] U8g2Init128x64 =
    [
        0xAE, 0xD5, 0x80, 0xA8, 0x3F, 0xD3, 0x00, 0x40, 0x8D, 0x14, 0x20, 0x00, 0xA1, 0xC8, 0xDA, 0x12,
        0x81, 0xCF, 0xD9, 0xF1, 0xDB, 0x40, 0x2E, 0xA4, 0xA6, 0xAF,
    ];

    // ref: §SSD1306.8: frame period = D x K x MUX / Fosc with D = 1 (0xD5 0x80), K = P1 + P2 + 50 = 1 + 15 + 50 (0xD9 0xF1),
    // MUX 64 and Fosc 370 kHz (⚠️ derived): 11.42 ms after the library 128x64 init. Interval code 000b = 5 frames: 57.08 ms per step.
    private static readonly TimeSpan AdafruitScrollStep = TimeSpan.FromSeconds(5.0 * 1 * 66 * 64 / 370_000);

    [Fact]
    public void AdafruitInit128x64_ShowsRamUpright()
    {
        // ref: §SSD1306.5.3 (128x64 alternative wiring + 0xDA 0x12, 0xC8, 0xA1 = upright), §LIB.A1306.2 frame transfer.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(AdafruitInit128x64);
        var frame = Frame(1024, (0, 0), (5, 1), (127, 63));

        SendAdafruitFrame(controller, frame);

        Assert.True(buffer.IsDisplayOn);
        Assert.True(buffer.GetPixel(0, 0));
        Assert.True(buffer.GetPixel(5, 1));
        Assert.True(buffer.GetPixel(127, 63));
        Assert.False(buffer.GetPixel(127, 0));
        Assert.False(buffer.GetPixel(0, 63));
        Assert.False(buffer.GetPixel(122, 62));
        Assert.Equal(frame, buffer.GetFrame());
    }

    [Fact]
    public void AdafruitInit128x32_ShowsRamUpright()
    {
        // ref: §SSD1306.5.3 (128x32 sequential wiring + 0xDA 0x02, MUX 32, 0xC8, 0xA1 = upright), §LIB.A1306.2: 512 bytes.
        var buffer = new MonochromeFrameBuffer(Columns, 32);
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(AdafruitInit128x32);
        var frame = Frame(512, (0, 0), (5, 1), (127, 31));

        SendAdafruitFrame(controller, frame);

        Assert.True(buffer.GetPixel(0, 0));
        Assert.True(buffer.GetPixel(5, 1));
        Assert.True(buffer.GetPixel(127, 31));
        Assert.False(buffer.GetPixel(0, 31));
        Assert.False(buffer.GetPixel(122, 30));
        Assert.Equal(frame, buffer.GetFrame());
    }

    [Fact]
    public void NoRemap_ShowsThePictureRotated180()
    {
        // ref: §SSD1306.6 (A0: column x drives SEG x, the module shows SEG 127 - q at panel column q) and
        // §SSD1306.5.1 + §SSD1306.5.3 (C0, alternative COM: panel row y shows RAM row 63 - y). Ruling R3: A0 before the data.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(AdafruitInit128x64);
        controller.WriteCommands([0xA0, 0xC0]);

        SendAdafruitFrame(controller, Frame(1024, (0, 0), (5, 1)));

        Assert.True(buffer.GetPixel(127, 63));
        Assert.True(buffer.GetPixel(122, 62));
        Assert.False(buffer.GetPixel(0, 0));
        Assert.False(buffer.GetPixel(5, 1));
    }

    [Fact]
    public void SequentialComOn128x64_InterleavesRows()
    {
        // ref: §SSD1306.5.4 item 1: an alternative-wired 128x64 module sent 0xDA 0x02 shows RAM row y / 2 on even panel rows
        // and RAM row 32 + (y - 1) / 2 on odd panel rows.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(AdafruitInit128x64);
        controller.WriteCommands([0xDA, 0x02]);

        SendAdafruitFrame(controller, Frame(1024, (0, 1), (1, 32), (2, 33), (3, 31), (4, 63)));

        Assert.True(buffer.GetPixel(0, 2));
        Assert.True(buffer.GetPixel(1, 1));
        Assert.True(buffer.GetPixel(2, 3));
        Assert.True(buffer.GetPixel(3, 62));
        Assert.True(buffer.GetPixel(4, 63));
        Assert.False(buffer.GetPixel(0, 1));
        Assert.False(buffer.GetPixel(1, 32));
        Assert.Equal(5, CountLit(buffer));
    }

    [Fact]
    public void StartLine_ShiftsTheRamRows()
    {
        // ref: §SSD1306.5.1 (start line: a lit pin shows RAM((sigma + start) mod 64)); upright, so panel row y shows RAM row (y + 8) mod 64.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(AdafruitInit128x64);
        SendAdafruitFrame(controller, Frame(1024, (0, 8), (1, 0), (2, 20)));

        controller.WriteCommand(0x48);

        Assert.True(buffer.GetPixel(0, 0));
        Assert.True(buffer.GetPixel(1, 56));
        Assert.True(buffer.GetPixel(2, 12));
        Assert.Equal(3, CountLit(buffer));
    }

    [Fact]
    public void DisplayOffset_ShiftsTheComRows()
    {
        // ref: §SSD1306.5.1 C8: j = (cSeq - off) mod 64, sigma = MUX - 1 - j; upright wiring gives cSeq = 63 - y,
        // so with offset 8 and MUX 64 panel row y shows RAM row (y + 8) mod 64 (Table 10-2 (b)).
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(AdafruitInit128x64);
        SendAdafruitFrame(controller, Frame(1024, (0, 8), (1, 0), (2, 63)));

        controller.WriteCommands([0xD3, 0x08]);

        Assert.True(buffer.GetPixel(0, 0));
        Assert.True(buffer.GetPixel(1, 56));
        Assert.True(buffer.GetPixel(2, 55));
        Assert.Equal(3, CountLit(buffer));
    }

    [Fact]
    public void Multiplex_DarkensRowsBeyondIt()
    {
        // ref: §SSD1306.5.1 (pin lit only if j < MUX) with §SSD1306.5.3 128x32 sequential wiring (pin = 31 - y) and C8:
        // MUX 16 gives j = 31 - y, lit for y >= 16, sigma = 15 - j = y - 16. So panel rows 0-15 are dark and rows 16-31 show RAM rows 0-15.
        var buffer = new MonochromeFrameBuffer(Columns, 32);
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(AdafruitInit128x32);
        SendAdafruitFrame(controller, Enumerable.Repeat((byte)0xFF, 512).ToArray());

        controller.WriteCommands([0xA8, 0x0F]);

        for (var y = 0; y < 32; y++)
        {
            Assert.Equal(y >= 16, buffer.GetPixel(10, y));
        }
    }

    [Fact]
    public void Multiplex_ShowsTheFirstRamRowsAtTheBottomBand()
    {
        // ref: §SSD1306.5.1 as above: with MUX 16 panel row 16 shows RAM row 0 and panel row 31 shows RAM row 15.
        var buffer = new MonochromeFrameBuffer(Columns, 32);
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(AdafruitInit128x32);
        SendAdafruitFrame(controller, Frame(512, (0, 0), (1, 15), (2, 16)));

        controller.WriteCommands([0xA8, 0x0F]);

        Assert.True(buffer.GetPixel(0, 16));
        Assert.True(buffer.GetPixel(1, 31));
        Assert.Equal(2, CountLit(buffer));
    }

    [Fact]
    public void Sh1106_ShowsRamFromColumnTwo()
    {
        // ref: §SH1106.1 single convention: under A1 + C8, data at RAM column q + 2 appears at panel column q.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);
        controller.WriteCommands([0xA1, 0xC8, 0xAF]);

        controller.WriteCommands([0xB0, 0x10, 0x02]);
        controller.WriteData([0x01, 0x02]);
        controller.WriteCommands([0xB7, 0x18, 0x01]);
        controller.WriteData([0x80]);

        Assert.True(buffer.GetPixel(0, 0));
        Assert.True(buffer.GetPixel(1, 1));
        Assert.True(buffer.GetPixel(127, 63));
        Assert.Equal(3, CountLit(buffer));
    }

    [Fact]
    public void Sh1106_IgnoresHorizontalAddressing()
    {
        // ref: §SH1106.4: 0x20 is not an SH1106 command and consumes no parameter; the 0x03 that follows is the column low nibble 3.
        // Page addressing stays (§SH1106.3), so the second byte lands on the same page, not the next one (vertical mode on an SSD1306).
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);
        controller.WriteCommands([0xA1, 0xC8, 0xAF]);

        controller.WriteCommands([0xB2, 0x10, 0x05, 0x20, 0x03]);
        controller.WriteData([0x01, 0x02]);

        Assert.Equal(2, controller.PagePointer);
        Assert.Equal(5, controller.ColumnPointer);
        Assert.True(buffer.GetPixel(1, 16));
        Assert.True(buffer.GetPixel(2, 17));
        Assert.Equal(2, CountLit(buffer));
    }

    [Fact]
    public void Sh1106_DoesNotWrapIntoTheNextPage()
    {
        // ref: §SH1106.3: "The page address is not changed"; the column past 131 is ⚠️ unverified, so only the next page is checked.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);
        controller.WriteCommands([0xA1, 0xC8, 0xAF]);

        controller.WriteCommands([0xB4, 0x10, 0x02]);
        controller.WriteData(Enumerable.Repeat((byte)0xFF, 140).ToArray());

        Assert.Equal(4, controller.PagePointer);
        Assert.All(buffer.GetFrame().AsSpan(5 * Columns, Columns).ToArray(), value => Assert.Equal(0, value));
        Assert.All(buffer.GetFrame().AsSpan(4 * Columns, Columns).ToArray(), value => Assert.Equal(0xFF, value));
    }

    [Fact]
    public void Ssd1306CodeOnSh1106_ShowsTheTwoColumnShift()
    {
        // ref: §LIB.U8G2.1 + §LIB.U8G2.2: U8g2's SSD1306 driver (x offset 0) sends each page as 40, 10, 00, B0+p, 128 bytes.
        // On an SH1106 (§SH1106.1) the frame lands in RAM columns 0..127 and the module shows RAM columns 2..129, so the picture
        // moves 2 columns left: frame column x appears at panel column x - 2, and panel columns 126-127 show the empty RAM columns 128-129.
        // (Adafruit_SSD1306's horizontal-mode stream is not used: on the SH1106 its 21 00 7F sets the start line and its column runs past 131,
        // which is ⚠️ unverified, §SH1106.3.)
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);
        controller.WriteCommands(U8g2Init128x64);
        var frame = Frame(1024, (0, 0), (2, 0), (10, 9), (127, 63));

        for (var page = 0; page < 8; page++)
        {
            controller.WriteCommands([0x40, 0x10, 0x00, (byte)(0xB0 + page)]);
            controller.WriteData(frame.AsSpan(page * Columns, Columns));
        }

        Assert.True(buffer.GetPixel(0, 0));
        Assert.True(buffer.GetPixel(8, 9));
        Assert.True(buffer.GetPixel(125, 63));
        Assert.False(buffer.GetPixel(127, 63));
        Assert.Equal(3, CountLit(buffer));
    }

    [Fact]
    public void RightScroll_MovesOneColumnPerStep()
    {
        // ref: §LIB.A1306.4 startscrollright(0, 3): 26 00 00 00 03 00 FF 2F; §SSD1306.8: pages 0-3 move one column per step,
        // wrapping around all 128 columns; interval 000b = 5 frames. Right = towards higher panel columns on an upright module
        // (⚠️ derived: the library's name for A1 + C8; the datasheet figure is drawn in SEG terms).
        var buffer = new MonochromeFrameBuffer();
        var time = new ManualTimeProvider();
        using var controller = new Ssd1306Controller(buffer, OledChip.Ssd1306, time);
        controller.WriteCommands(AdafruitInit128x64);
        SendAdafruitFrame(controller, Frame(1024, (10, 3), (127, 5), (10, 60)));

        controller.WriteCommands([0x26, 0x00, 0x00, 0x00, 0x03, 0x00, 0xFF, 0x2F]);
        time.Advance(AdafruitScrollStep * 0.99);

        Assert.True(controller.IsScrollActive);
        Assert.True(buffer.GetPixel(10, 3));

        time.Advance(AdafruitScrollStep * 0.02);

        Assert.True(buffer.GetPixel(11, 3));
        Assert.True(buffer.GetPixel(0, 5));
        Assert.True(buffer.GetPixel(10, 60));
        Assert.Equal(3, CountLit(buffer));

        time.Advance(AdafruitScrollStep);

        Assert.True(buffer.GetPixel(12, 3));
        Assert.True(buffer.GetPixel(1, 5));
        Assert.True(buffer.GetPixel(10, 60));
        Assert.Equal(3, CountLit(buffer));
    }

    [Fact]
    public void LeftScroll_MovesOneColumnPerStep()
    {
        // ref: §LIB.A1306.4 startscrollleft(0, 7): 27 00 00 00 07 00 FF 2F; §SSD1306.8 (direction ⚠️ derived as for the right scroll).
        var buffer = new MonochromeFrameBuffer();
        var time = new ManualTimeProvider();
        using var controller = new Ssd1306Controller(buffer, OledChip.Ssd1306, time);
        controller.WriteCommands(AdafruitInit128x64);
        SendAdafruitFrame(controller, Frame(1024, (10, 3), (0, 60)));

        controller.WriteCommands([0x27, 0x00, 0x00, 0x00, 0x07, 0x00, 0xFF, 0x2F]);
        time.Advance(AdafruitScrollStep * 1.01);

        Assert.True(buffer.GetPixel(9, 3));
        Assert.True(buffer.GetPixel(127, 60));
        Assert.Equal(2, CountLit(buffer));
    }

    [Fact]
    public void DeactivateScroll_KeepsTheCurrentShift()
    {
        // ref: §SSD1306.8 setup rules: the design keeps the picture where it is after 2E (⚠️ unverified against hardware).
        var buffer = new MonochromeFrameBuffer();
        var time = new ManualTimeProvider();
        using var controller = new Ssd1306Controller(buffer, OledChip.Ssd1306, time);
        controller.WriteCommands(AdafruitInit128x64);
        SendAdafruitFrame(controller, Frame(1024, (10, 3)));
        controller.WriteCommands([0x26, 0x00, 0x00, 0x00, 0x07, 0x00, 0xFF, 0x2F]);
        time.Advance(AdafruitScrollStep * 2.01);

        controller.WriteCommand(0x2E);
        time.Advance(AdafruitScrollStep * 10);

        Assert.False(controller.IsScrollActive);
        Assert.Equal(0, time.ActiveTimerCount);
        Assert.True(buffer.GetPixel(12, 3));
        Assert.Equal(1, CountLit(buffer));
    }

    [Fact]
    public void ScrollStep_FollowsTheFramePeriodRegisters()
    {
        // ref: §SSD1306.8: frame period = D x K x MUX / Fosc. With 0xD9 0x22 (K = 54) instead of 0xF1 (K = 66) one step is
        // 5 x 54 x 64 / 370 kHz = 46.7 ms; the step at the library init (57.1 ms) would not yet have come.
        var buffer = new MonochromeFrameBuffer();
        var time = new ManualTimeProvider();
        using var controller = new Ssd1306Controller(buffer, OledChip.Ssd1306, time);
        controller.WriteCommands(AdafruitInit128x64);
        controller.WriteCommands([0xD9, 0x22]);
        SendAdafruitFrame(controller, Frame(1024, (10, 3)));
        var step = TimeSpan.FromSeconds(5.0 * 54 * 64 / 370_000);

        controller.WriteCommands([0x26, 0x00, 0x00, 0x00, 0x07, 0x00, 0xFF, 0x2F]);
        time.Advance(step * 0.99);

        Assert.True(buffer.GetPixel(10, 3));

        time.Advance(step * 0.02);

        Assert.True(buffer.GetPixel(11, 3));
    }

    [Fact]
    public void ScrollStep_Ssd1309_UsesItsOwnTableAndFramePeriod()
    {
        // ref: §SSD1309.3: interval code 111b = 1 frame (the SSD1306 table gives 2); §SSD1309.4: K = P1 + P2 + 65 and Fosc = 450 kHz,
        // so at reset (0xD5 0x70: D = 1, 0xD9 0x22: K = 69, MUX 64) one step is 1 x 69 x 64 / 450 kHz = 9.81 ms (⚠️ derived).
        var buffer = new MonochromeFrameBuffer();
        var time = new ManualTimeProvider();
        using var controller = new Ssd1306Controller(buffer, OledChip.Ssd1309, time);
        controller.WriteCommands([0x20, 0x00, 0xA1, 0xC8, 0xAF]);
        SendAdafruitFrame(controller, Frame(1024, (10, 3)));
        var step = TimeSpan.FromSeconds(1.0 * 69 * 64 / 450_000);

        controller.WriteCommands([0x26, 0x00, 0x00, 0x07, 0x07, 0x00, 0x00, 0x7F, 0x2F]);
        time.Advance(step * 0.99);

        Assert.True(buffer.GetPixel(10, 3));

        time.Advance(step * 0.02);

        Assert.True(buffer.GetPixel(11, 3));
        Assert.Equal(1, CountLit(buffer));
    }

    [Fact]
    public void Sh1106_OwnCommands_TakeTheirParameterCounts()
    {
        // ref: §SH1106.2, §SH1106.4: 0xAD takes one parameter (0x8A/0x8B), 0x30-0x33 take none; nothing visible changes.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);

        controller.WriteCommands([0xAD, 0xAF]);

        Assert.False(buffer.IsDisplayOn);

        controller.WriteCommands([0xAD, 0x8B, 0x32, 0xAF]);

        Assert.True(buffer.IsDisplayOn);
    }

    [Fact]
    public void Sh1106_ReadModifyWriteEnd_ReturnsTheColumn()
    {
        // ref: §SH1106.3: 0xEE returns the column to the address it had at 0xE0 (⚠️ derived); writes still advance the column.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);

        controller.WriteCommands([0xB0, 0x10, 0x05, 0xE0]);
        controller.WriteData([0x01, 0x02, 0x03]);

        Assert.Equal(8, controller.ColumnPointer);

        controller.WriteCommand(0xEE);

        Assert.Equal(5, controller.ColumnPointer);
    }

    [Fact]
    public void DisplayOffset_WithSmallMultiplex_MovesTheLitBand()
    {
        // ref: §SSD1306.5.1 (Table 10-2 (e): the offset moves the blank band) with the 128x32 sequential wiring (pin = 31 - y, §SSD1306.5.3),
        // C8, MUX 16, offset 8: j = (31 - y - 8) mod 64, lit for j < 16, i.e. panel rows 8-23; sigma = 15 - j = y - 8.
        var buffer = new MonochromeFrameBuffer(Columns, 32);
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(AdafruitInit128x32);
        SendAdafruitFrame(controller, Enumerable.Repeat((byte)0xFF, 512).ToArray());

        controller.WriteCommands([0xA8, 0x0F, 0xD3, 0x08]);

        for (var y = 0; y < 32; y++)
        {
            Assert.Equal(y is >= 8 and <= 23, buffer.GetPixel(10, y));
        }
    }

    [Fact]
    public void StartLine_WithSmallMultiplex_KeepsTheLitBand()
    {
        // ref: §SSD1306.5.1 (Table 10-2 (f): the start line moves the RAM rows, not the blank band): C8, MUX 16, start 8:
        // j = 31 - y, lit for panel rows 16-31; RAM row = (15 - j) + 8 = y - 8, so panel row 16 shows RAM row 8 and RAM row 0 is not shown.
        var buffer = new MonochromeFrameBuffer(Columns, 32);
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(AdafruitInit128x32);
        SendAdafruitFrame(controller, Frame(512, (0, 0), (1, 8), (2, 23)));

        controller.WriteCommands([0xA8, 0x0F, 0x48]);

        Assert.True(buffer.GetPixel(1, 16));
        Assert.True(buffer.GetPixel(2, 31));
        Assert.Equal(2, CountLit(buffer));
    }

    [Fact]
    public void Sh1106_ColumnAddressCommand_IsUnknown()
    {
        // ref: §SH1106.4: 0x21 is not an SH1106 command and consumes no parameter: in "21 00 7F" the 0x00 is a column low nibble
        // and the 0x7F a display start line command (§SH1106.2).
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);

        controller.WriteCommands([0xB3, 0x10, 0x05, 0x21, 0x00, 0x7F]);

        Assert.Equal(0, controller.ColumnPointer);
        Assert.Equal(63, controller.StartLine);
        Assert.Equal(3, controller.PagePointer);
    }

    [Fact]
    public void Sh1106_PageAddressCommand_IsUnknown()
    {
        // ref: §SH1106.4: 0x22 is not an SH1106 command and consumes no parameter: in "22 00 07" both bytes are column low nibbles.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);

        controller.WriteCommands([0xB3, 0x11, 0x05, 0x22, 0x00, 0x07]);

        Assert.Equal(0x17, controller.ColumnPointer);
        Assert.Equal(3, controller.PagePointer);
    }

    [Fact]
    public void ContentScroll_Ssd1309_MovesOneColumnPerCommand()
    {
        // ref: §SSD1309.2: 2C (right) / 2D (left) move pages B..D, columns F..G by one column per command, without a timer.
        var buffer = new MonochromeFrameBuffer();
        var time = new ManualTimeProvider();
        using var controller = new Ssd1306Controller(buffer, OledChip.Ssd1309, time);
        controller.WriteCommands([0x20, 0x00, 0xA1, 0xC8, 0xAF]);
        SendAdafruitFrame(controller, Frame(1024, (10, 3), (10, 60)));

        controller.WriteCommands([0x2C, 0x00, 0x00, 0x01, 0x03, 0x00, 0x00, 0x7F]);

        Assert.True(buffer.GetPixel(11, 3));
        Assert.True(buffer.GetPixel(10, 60));
        Assert.Equal(0, time.ActiveTimerCount);

        controller.WriteCommands([0x2D, 0x00, 0x00, 0x01, 0x03, 0x00, 0x00, 0x7F]);
        controller.WriteCommands([0x2D, 0x00, 0x00, 0x01, 0x03, 0x00, 0x00, 0x7F]);

        Assert.True(buffer.GetPixel(9, 3));
        Assert.Equal(2, CountLit(buffer));
    }

    [Fact]
    public void ChangedSubscriber_WaitingForAnotherFeedThread_DoesNotDeadlock()
    {
        // The buffer raises Changed outside any lock; the controller must not hold its own lock while the buffer raises it.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        var completed = WaitInChangedForAnotherWrite(buffer, controller, () => controller.WriteData([0x01]));

        Assert.True(completed);
    }

    [Fact]
    public void ChangedSubscriber_DuringAScrollStep_DoesNotDeadlock()
    {
        var buffer = new MonochromeFrameBuffer();
        var time = new ManualTimeProvider();
        using var controller = new Ssd1306Controller(buffer, OledChip.Ssd1306, time);
        controller.WriteCommands(AdafruitInit128x64);
        SendAdafruitFrame(controller, Frame(1024, (10, 3)));
        controller.WriteCommands([0x26, 0x00, 0x00, 0x00, 0x07, 0x00, 0xFF, 0x2F]);

        var completed = WaitInChangedForAnotherWrite(buffer, controller, () => time.Advance(AdafruitScrollStep * 1.01));

        Assert.True(completed);
    }

    [Fact]
    public void Reset_StopsTheScrollTimer()
    {
        var time = new ManualTimeProvider();
        using var controller = new Ssd1306Controller(new MonochromeFrameBuffer(), OledChip.Ssd1306, time);
        controller.WriteCommands([0x26, 0x00, 0x00, 0x00, 0x07, 0x00, 0xFF, 0x2F]);

        Assert.Equal(1, time.ActiveTimerCount);

        controller.Reset();

        Assert.False(controller.IsScrollActive);
        Assert.Equal(0, time.ActiveTimerCount);
    }

    [Fact]
    public void ScrollSetup_WhileScrolling_StopsTheTimer()
    {
        // ref: §SSD1306.2: a setup must be preceded by 2E; the emulator stops the running scroll when its parameters change.
        var time = new ManualTimeProvider();
        using var controller = new Ssd1306Controller(new MonochromeFrameBuffer(), OledChip.Ssd1306, time);
        controller.WriteCommands([0x26, 0x00, 0x00, 0x00, 0x07, 0x00, 0xFF, 0x2F]);

        controller.WriteCommands([0x27, 0x00, 0x00, 0x00, 0x07, 0x00, 0xFF]);

        Assert.False(controller.IsScrollActive);
        Assert.Equal(0, time.ActiveTimerCount);
    }

    [Fact]
    public void Dispose_StopsTheScrollTimer()
    {
        var time = new ManualTimeProvider();
        using var controller = new Ssd1306Controller(new MonochromeFrameBuffer(), OledChip.Ssd1306, time);
        controller.WriteCommands([0x26, 0x00, 0x00, 0x00, 0x07, 0x00, 0xFF, 0x2F]);

        controller.Dispose();

        Assert.Equal(0, time.ActiveTimerCount);
    }

    [Fact]
    public void BufferResized_KeepsTheRamAndRemaps()
    {
        // ref: §SSD1306.5.3: the wiring follows the buffer height (> 32 alternative, <= 32 sequential). The controller remaps on a
        // resize without waiting for the next write, and keeps its own RAM, so rows the smaller buffer cropped come back.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(AdafruitInit128x64);
        SendAdafruitFrame(controller, Frame(1024, (3, 2), (3, 40)));

        // The 128x32 settings while the panel is still 64 rows: (3, 2) is not on panel row 2 (⚠️ derived, not asserted).
        controller.WriteCommands([0xA8, 0x1F, 0xDA, 0x02]);
        buffer.Resize(Columns, 32);

        Assert.True(buffer.GetPixel(3, 2));
        Assert.Equal(1, CountLit(buffer));

        controller.WriteCommands([0xA8, 0x3F, 0xDA, 0x12]);
        buffer.Resize(Columns, 64);

        Assert.True(buffer.GetPixel(3, 2));
        Assert.True(buffer.GetPixel(3, 40));
        Assert.Equal(2, CountLit(buffer));
    }

    [Fact]
    public void SegmentRemap_RaisesNoChanged()
    {
        // ref: §SSD1306.6, ruling R3: A0/A1 only affects subsequent data writes; nothing visible changes.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(AdafruitInit128x64);
        SendAdafruitFrame(controller, Frame(1024, (3, 2)));
        var count = 0;
        buffer.Changed += (_, _) => count++;

        controller.WriteCommand(0xA0);
        controller.WriteCommand(0xA1);

        Assert.Equal(0, count);
        Assert.True(buffer.GetPixel(3, 2));
    }

    [Fact]
    public void Sh1106_DataInHiddenColumns_RaisesNoChanged()
    {
        // ref: §SH1106.1: RAM columns 0, 1, 130 and 131 are not shown on a module with column offset 2.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);
        controller.WriteCommands([0xA1, 0xC8, 0xAF]);
        var count = 0;
        buffer.Changed += (_, _) => count++;

        controller.WriteCommands([0xB0, 0x10, 0x00]);
        controller.WriteData([0xFF, 0xFF]);
        controller.WriteCommands([0xB0, 0x18, 0x02]);
        controller.WriteData([0xFF, 0xFF]);

        Assert.Equal(0, count);
    }

    [Fact]
    public void ScrollActivate_RaisesNoChangedUntilTheFirstStep()
    {
        var buffer = new MonochromeFrameBuffer();
        var time = new ManualTimeProvider();
        using var controller = new Ssd1306Controller(buffer, OledChip.Ssd1306, time);
        controller.WriteCommands(AdafruitInit128x64);
        SendAdafruitFrame(controller, Frame(1024, (10, 3)));
        var count = 0;
        buffer.Changed += (_, _) => count++;

        controller.WriteCommands([0x26, 0x00, 0x00, 0x00, 0x07, 0x00, 0xFF, 0x2F]);

        Assert.Equal(0, count);

        time.Advance(AdafruitScrollStep * 1.01);

        Assert.Equal(1, count);

        controller.WriteCommand(0x2E);

        Assert.Equal(1, count);
    }

    // ref: §LIB.A1306.2: 22 00 FF 21, 00, 7F, then the frame.
    // Subscribes a Changed handler that, the first time it runs, feeds the controller from a second thread and waits (5 s at most)
    // for that call to finish, as a handler doing Dispatcher.Invoke to a busy UI thread would. Returns whether the second call finished.
    private static bool WaitInChangedForAnotherWrite(
        MonochromeFrameBuffer buffer,
        Ssd1306Controller controller,
        Action trigger)
    {
        var entered = 0;
        bool? completed = null;
        buffer.Changed += (_, _) =>
        {
            if (Interlocked.Exchange(ref entered, 1) != 0)
            {
                return;
            }

            var feeder = new Thread(() => controller.WriteData([0x55, 0xAA]));
            feeder.Start();
            completed = feeder.Join(TimeSpan.FromSeconds(5));
        };

        trigger();

        return completed == true;
    }

    private static void SendAdafruitFrame(
        Ssd1306Controller controller,
        byte[] frame)
    {
        controller.WriteCommands([0x22, 0x00, 0xFF, 0x21, 0x00, 0x7F]);
        controller.WriteData(frame);
    }

    private static byte[] Frame(
        int length,
        params (int X, int Y)[] pixels)
    {
        var frame = new byte[length];
        foreach (var (x, y) in pixels)
        {
            frame[((y / 8) * Columns) + x] |= (byte)(1 << (y % 8));
        }

        return frame;
    }

    private static int CountLit(MonochromeFrameBuffer buffer)
    {
        var count = 0;
        foreach (var value in buffer.GetFrame())
        {
            count += System.Numerics.BitOperations.PopCount(value);
        }

        return count;
    }
}