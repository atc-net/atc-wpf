namespace Atc.Wpf.Displays.Tests;

public sealed class Ssd1306ControllerTests
{
    private const int Columns = 128;

    // ref: §SSD1306.5.3, §SH1106.1: SEG remap A1 + COM scan C8 show the RAM upright (the power-on A0 + C0 show it rotated 180 degrees).
    private static readonly byte[] Upright = [0xA1, 0xC8];

    [Fact]
    public void Constructor_NullBuffer_Throws()
        => Assert.Throws<ArgumentNullException>(() => new Ssd1306Controller(null!));

    [Fact]
    public void Constructor_AppliesThePowerOnState()
    {
        // ref: §SSD1306.1 display off, contrast 0x7F, page addressing.
        var buffer = new MonochromeFrameBuffer();
        buffer.Fill();

        using var controller = new Ssd1306Controller(buffer);

        Assert.False(buffer.IsDisplayOn);
        Assert.Equal(0x7F, buffer.Contrast);
        Assert.All(buffer.GetFrame(), value => Assert.Equal(0, value));
        Assert.Equal(128, controller.RamWidth);
        Assert.Equal(0, controller.ColumnOffset);
    }

    [Fact]
    public void PageMode_DataWritesAlongThePage()
    {
        // ref: §SSD1306.4 page mode: column + 1 after each byte; B0-B7 and 00-1F set the pointers (§SSD1306.2).
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(Upright);

        controller.WriteCommands([0xB3, 0x0A, 0x11]);
        controller.WriteData([0x01, 0x02, 0x03]);

        var frame = buffer.GetFrame();
        Assert.Equal(0x01, frame[(3 * Columns) + 26]);
        Assert.Equal(0x02, frame[(3 * Columns) + 27]);
        Assert.Equal(0x03, frame[(3 * Columns) + 28]);
        Assert.Equal(29, controller.ColumnPointer);
        Assert.Equal(3, controller.PagePointer);
    }

    [Fact]
    public void PageMode_DataPastThePageEnd_StaysOnTheSamePage()
    {
        // ref: §SSD1306.4 "page address pointer is not changed" at the column end. The wrap target (0 or the nibble start)
        // is ⚠️ unverified, so the pointer starts at column 0, where both readings agree.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        var data = Enumerable.Repeat((byte)0x11, 128).Append((byte)0x80).ToArray();
        controller.WriteCommands(Upright);

        controller.WriteCommands([0xB2, 0x00, 0x10]);
        controller.WriteData(data);

        var frame = buffer.GetFrame();
        Assert.Equal(0x80, frame[(2 * Columns) + 0]);
        Assert.Equal(0x11, frame[(2 * Columns) + 1]);
        Assert.Equal(0x11, frame[(2 * Columns) + 127]);
        Assert.All(frame.AsSpan(3 * Columns, Columns).ToArray(), value => Assert.Equal(0, value));
        Assert.Equal(2, controller.PagePointer);
    }

    [Fact]
    public void HorizontalMode_DataPastTheWindow_WrapsToTheWindowStart()
    {
        // ref: §SSD1306.4 horizontal mode, §10.1.5 Figure 10-5: column end -> column start of the next page; window end -> window start.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(Upright);

        controller.WriteCommands([0x20, 0x00, 0x21, 0x00, 0x03, 0x22, 0x00, 0x01]);
        controller.WriteData([0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09]);

        var frame = buffer.GetFrame();
        Assert.Equal(0x09, frame[0]);
        Assert.Equal(0x02, frame[1]);
        Assert.Equal(0x04, frame[3]);
        Assert.Equal(0, frame[4]);
        Assert.Equal(0x05, frame[Columns + 0]);
        Assert.Equal(0x08, frame[Columns + 3]);
        Assert.Equal(0, frame[(2 * Columns) + 0]);
        Assert.Equal(1, controller.ColumnPointer);
        Assert.Equal(0, controller.PagePointer);
    }

    [Fact]
    public void VerticalMode_AdvancesPageFirst()
    {
        // ref: §SSD1306.4 vertical mode, Figure 10-4: page + 1, at the page end back to the page start and column + 1.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands(Upright);

        controller.WriteCommands([0x20, 0x01, 0x21, 0x05, 0x06, 0x22, 0x02, 0x03]);
        controller.WriteData([0x01, 0x02, 0x03, 0x04, 0x05]);

        var frame = buffer.GetFrame();
        Assert.Equal(0x05, frame[(2 * Columns) + 5]);
        Assert.Equal(0x02, frame[(3 * Columns) + 5]);
        Assert.Equal(0x03, frame[(2 * Columns) + 6]);
        Assert.Equal(0x04, frame[(3 * Columns) + 6]);
        Assert.Equal(0, frame[(4 * Columns) + 5]);
        Assert.Equal(0, frame[(2 * Columns) + 7]);
        Assert.Equal(5, controller.ColumnPointer);
        Assert.Equal(3, controller.PagePointer);
    }

    [Fact]
    public void AdafruitDisplay_1024Bytes_FillTheRamInOrder()
    {
        // ref: §LIB.A1306.2: 22 00 FF (end masked to 7, §SSD1306.2), 21 00 7F, then 1,024 data bytes in horizontal mode.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        var data = new byte[1024];
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = (byte)((i * 31) + 7);
        }

        controller.WriteCommands(Upright);
        controller.WriteCommands([0x20, 0x00]);
        controller.WriteCommands([0x22, 0x00, 0xFF, 0x21]);
        controller.WriteCommand(0x00);
        controller.WriteCommand(0x7F);
        controller.WriteData(data);

        // The RAM is kept in SEG order: with A1, column x drives SEG 127 - x (§SSD1306.6, ruling R3).
        var ram = controller.GetRam();
        for (var i = 0; i < data.Length; i++)
        {
            Assert.Equal(data[i], ram[((i / Columns) * Columns) + (Columns - 1 - (i % Columns))]);
        }

        Assert.Equal(data, buffer.GetFrame());
        Assert.Equal(0, controller.ColumnPointer);
        Assert.Equal(0, controller.PagePointer);
    }

    [Fact]
    public void AdafruitDisplay_128x32_FillsTheFirstFourPages()
    {
        // ref: §LIB.A1306.2: a 128x32 module sends 512 bytes with the page end 0xFF (= 7), which is never reached.
        var buffer = new MonochromeFrameBuffer(128, 32);
        using var controller = new Ssd1306Controller(buffer);
        var data = new byte[512];
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = (byte)(i + 1);
        }

        // ref: §SSD1306.5.3: the 128x32 module is upright with MUX 32, sequential COM, A1 and C8.
        controller.WriteCommands([0xA8, 0x1F, 0xDA, 0x02, 0xA1, 0xC8]);
        controller.WriteCommands([0x20, 0x00, 0x22, 0x00, 0xFF, 0x21, 0x00, 0x7F]);
        controller.WriteData(data);

        Assert.Equal(data, buffer.GetFrame());
        Assert.Equal(4, controller.PagePointer);
    }

    [Fact]
    public void Contrast_SetsBufferContrast()
    {
        // ref: §SSD1306.2 0x81 has one parameter, §SSD1306.7.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);

        controller.WriteCommands([0x81, 0xCF]);

        Assert.Equal(0xCF, buffer.Contrast);
    }

    [Fact]
    public void DisplayOnOff_EntireOn_Invert_SetBufferState()
    {
        // ref: §SSD1306.7 AE/AF, A4/A5, A6/A7.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);

        controller.WriteCommands([0xAF, 0xA5, 0xA7]);

        Assert.True(buffer.IsDisplayOn);
        Assert.True(buffer.IsEntireDisplayOn);
        Assert.True(buffer.IsInverted);

        controller.WriteCommands([0xAE, 0xA4, 0xA6]);

        Assert.False(buffer.IsDisplayOn);
        Assert.False(buffer.IsEntireDisplayOn);
        Assert.False(buffer.IsInverted);
    }

    [Fact]
    public void MissingParameters_TakeTheNextCommandBytes()
    {
        // ref: §0.2 parameters are the following command-mode bytes; §SSD1306.10 they persist across writes.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);

        controller.WriteCommand(0x81);
        controller.WriteCommand(0xAF);

        Assert.Equal(0xAF, buffer.Contrast);
        Assert.False(buffer.IsDisplayOn);
    }

    [Fact]
    public void UnknownCommand_IsIgnored()
    {
        // ref: §SSD1306.2 "Unknown / invalid bytes": ignored and traced, no parameters consumed.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);

        var exception = Record.Exception(() => controller.WriteCommands([0xFF, 0xFF, 0xAF]));

        Assert.Null(exception);
        Assert.True(buffer.IsDisplayOn);
    }

    [Fact]
    public void ChargePump_Ssd1306_ConsumesItsParameter()
    {
        // ref: §SSD1306.2 0x8D has one parameter.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);

        controller.WriteCommands([0x8D, 0xAF]);

        Assert.False(buffer.IsDisplayOn);
    }

    [Fact]
    public void ChargePump_Sh1106_IsUnknownAndItsParameterIsACommand()
    {
        // ref: §SH1106.4: 0x8D is not an SH1106 command; the following 0x14 is the column high nibble 4.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);

        controller.WriteCommands([0xB1, 0x02, 0x8D, 0x14]);

        Assert.Equal(0x42, controller.ColumnPointer);
        Assert.Equal(1, controller.PagePointer);
    }

    [Fact]
    public void ChargePump_Ssd1309_IsUnknown()
    {
        // ref: §SSD1309.2, §SSD1309.5: the SSD1309 has no 0x8D; no parameter is consumed.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Ssd1309);

        controller.WriteCommands([0x8D, 0xAF]);

        Assert.True(buffer.IsDisplayOn);
    }

    [Fact]
    public void AddressingMode_Sh1106_IsUnknown()
    {
        // ref: §SH1106.4: 0x20 consumes no parameter; Adafruit_SH110X's "20 10" sets the column high nibble 0 (§LIB.A110X.1).
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);

        controller.WriteCommands([0x13, 0x05, 0x20, 0x10]);

        Assert.Equal(0x05, controller.ColumnPointer);
    }

    [Fact]
    public void HorizontalScrollSetup_Ssd1306_ConsumesSixParameters()
    {
        // ref: §SSD1306.2 0x26 has 6 parameters.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);

        controller.WriteCommands([0x26, 0x00, 0x00, 0x00, 0x07, 0x00, 0xFF, 0xAF]);

        Assert.True(buffer.IsDisplayOn);
        Assert.Equal([0x26, 0x00, 0x00, 0x00, 0x07, 0x00, 0xFF], controller.ScrollSetup);
        Assert.False(controller.IsScrollActive);
    }

    [Fact]
    public void HorizontalScrollSetup_Ssd1309_ConsumesSevenParameters()
    {
        // ref: §SSD1309.2 0x26 has 7 parameters (F start column, G end column).
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Ssd1309);

        controller.WriteCommands([0x26, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0xAF]);

        Assert.False(buffer.IsDisplayOn);
        Assert.Equal([0x26, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0xAF], controller.ScrollSetup);
        Assert.False(controller.IsScrollActive);

        controller.WriteCommand(0xAF);

        Assert.True(buffer.IsDisplayOn);
    }

    [Fact]
    public void DiagonalScrollSetup_Ssd1306_ConsumesFiveParameters()
    {
        // ref: §SSD1306.2 0x29 has 5 parameters, 0xA3 has 2.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);

        controller.WriteCommands([0xA3, 0x00, 0x40, 0x29, 0x00, 0x00, 0x00, 0x07, 0x01, 0xAF]);

        Assert.True(buffer.IsDisplayOn);
        Assert.Equal([0x29, 0x00, 0x00, 0x00, 0x07, 0x01], controller.ScrollSetup);
        Assert.Equal(0, controller.ScrollTopFixedRows);
        Assert.Equal(64, controller.ScrollRows);
        Assert.False(controller.IsScrollActive);
    }

    [Fact]
    public void DiagonalScrollSetup_Ssd1309_ConsumesSevenParameters()
    {
        // ref: §SSD1309.2 0x29 has 7 parameters (A column offset, F start column, G end column).
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Ssd1309);

        controller.WriteCommands([0x29, 0x01, 0x00, 0x00, 0x07, 0x01, 0x00, 0x7F, 0xAF]);

        Assert.Equal([0x29, 0x01, 0x00, 0x00, 0x07, 0x01, 0x00, 0x7F], controller.ScrollSetup);
        Assert.True(buffer.IsDisplayOn);
    }

    [Fact]
    public void Sh1106_ShowsRamFromColumnTwo()
    {
        // ref: §SH1106.1: 132-column RAM, a 128-wide module shows RAM columns 2..129.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);
        controller.WriteCommands(Upright);

        controller.WriteCommands([0xB0, 0x10, 0x00]);
        controller.WriteData([0x01, 0x02, 0x03]);

        var frame = buffer.GetFrame();
        Assert.Equal(132, controller.RamWidth);
        Assert.Equal(2, controller.ColumnOffset);
        Assert.Equal(0x03, frame[0]);
        Assert.Equal(0, frame[1]);
        Assert.Equal(0x80, buffer.Contrast);
    }

    [Fact]
    public void Sh1106_PageMode_DoesNotAdvanceToTheNextPage()
    {
        // ref: §SH1106.3: the page address is not changed by data writes.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);
        controller.WriteCommands(Upright);

        controller.WriteCommands([0xB4, 0x12, 0x00]);
        controller.WriteData(Enumerable.Repeat((byte)0xFF, 100).ToArray());

        Assert.Equal(4, controller.PagePointer);
        Assert.All(buffer.GetFrame().AsSpan(5 * Columns, Columns).ToArray(), value => Assert.Equal(0, value));
    }

    [Fact]
    public void ColumnOffset_Set_RemapsTheVisiblePicture()
    {
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer, OledChip.Sh1106);
        controller.WriteCommands(Upright);
        controller.WriteCommands([0xB0, 0x10, 0x00]);
        controller.WriteData([0x01, 0x02, 0x03]);

        controller.ColumnOffset = 0;

        Assert.Equal(0x01, buffer.GetFrame()[0]);
    }

    [Fact]
    public void ColumnOffset_OutOfRange_Throws()
    {
        using var controller = new Ssd1306Controller(new MonochromeFrameBuffer());

        Assert.Throws<ArgumentOutOfRangeException>(() => controller.ColumnOffset = -1);
        Assert.Throws<ArgumentOutOfRangeException>(() => controller.ColumnOffset = 128);
    }

    [Fact]
    public void WriteData_RaisesChangedOnce()
    {
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        var count = 0;
        buffer.Changed += (_, _) => count++;

        controller.WriteData([0x01, 0x02, 0x03, 0x04]);

        Assert.Equal(1, count);
    }

    [Fact]
    public void WriteCommands_RaisesChangedOnce()
    {
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        var count = 0;
        buffer.Changed += (_, _) => count++;

        controller.WriteCommands([0xAF, 0xA7, 0x81, 0x10, 0xA5]);

        Assert.Equal(1, count);
    }

    [Fact]
    public void Nop_RaisesNoChanged()
    {
        // ref: §SSD1306.2 0xE3 NOP.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        var count = 0;
        buffer.Changed += (_, _) => count++;

        controller.WriteCommands([0xE3, 0xD5, 0x80, 0xD9, 0xF1, 0xDB, 0x40]);

        Assert.Equal(0, count);
    }

    [Fact]
    public void Commands_RecordTheMappingState()
    {
        // ref: §SSD1306.2: 0x40-0x7F start line, 0xD3 offset, 0xA8 multiplex, 0xA1 remap, 0xC8 scan, 0xDA COM pins.
        using var controller = new Ssd1306Controller(new MonochromeFrameBuffer());

        controller.WriteCommands([0x48, 0xD3, 0x05, 0xA8, 0x1F, 0xA1, 0xC8, 0xDA, 0x02]);

        Assert.Equal(8, controller.StartLine);
        Assert.Equal(5, controller.DisplayOffset);
        Assert.Equal(0x1F, controller.MultiplexRatio);
        Assert.True(controller.IsSegmentRemapped);
        Assert.True(controller.IsComScanReversed);
        Assert.Equal(0x02, controller.ComPinsConfiguration);
    }

    [Fact]
    public void Reset_TurnsTheDisplayOffAndRestoresPageMode()
    {
        // ref: §SSD1306.1 power-on state; RAM is left as it is.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        controller.WriteCommands([0xAF, 0xA7, 0xA5, 0x81, 0x10, 0x20, 0x00, 0x48, 0xD3, 0x05, 0xA8, 0x1F, 0xA1, 0xC8]);
        controller.WriteData([0x5A]);
        controller.WriteCommand(0x81);

        controller.Reset();

        Assert.False(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);
        Assert.False(buffer.IsEntireDisplayOn);
        Assert.Equal(0x7F, buffer.Contrast);
        Assert.Equal(0, controller.StartLine);
        Assert.Equal(0, controller.DisplayOffset);
        Assert.Equal(63, controller.MultiplexRatio);
        Assert.False(controller.IsSegmentRemapped);
        Assert.False(controller.IsComScanReversed);
        Assert.Equal(0x5A, controller.GetRam()[Columns - 1]);
        Assert.Equal(0, controller.ColumnPointer);
        Assert.Equal(0, controller.PagePointer);

        // Page mode: the page does not advance past the column end (§SSD1306.4), and the pending 0x81 was dropped.
        controller.WriteCommand(0xAF);
        controller.WriteData(Enumerable.Repeat((byte)0x01, 129).ToArray());

        Assert.True(buffer.IsDisplayOn);
        Assert.Equal(0, controller.PagePointer);
        Assert.Equal(0, controller.GetRam()[Columns]);
    }

    [Fact]
    public void Dispose_LaterWritesAreIgnored()
    {
        // Disposing makes the controller inert: nothing reaches the RAM or the buffer afterwards.
        var buffer = new MonochromeFrameBuffer();
        var controller = new Ssd1306Controller(buffer);
        controller.Dispose();
        var ram = controller.GetRam();
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        controller.WriteCommand(0xAF);
        controller.WriteCommands([0xA7, 0x81, 0x10]);
        controller.WriteData([0xFF, 0xFF, 0xFF, 0xFF]);
        controller.Reset();
        controller.ColumnOffset = 4;

        Assert.Equal(0, changes);
        Assert.Equal(ram, controller.GetRam());
        Assert.False(buffer.IsDisplayOn);
        Assert.False(buffer.IsInverted);
        Assert.Equal(0x7F, buffer.Contrast);
        Assert.Equal(0, controller.ColumnOffset);
        Assert.Equal(0, controller.ColumnPointer);
    }
}