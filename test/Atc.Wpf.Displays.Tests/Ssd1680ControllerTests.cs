namespace Atc.Wpf.Displays.Tests;

[Collection(nameof(TraceTestGroup))]
public sealed class Ssd1680ControllerTests
{
    // ref: §SSD1680.1: 176 sources = 22 bytes per RAM row, 296 gates = 296 rows, for each plane.
    private const int RamBytesPerRow = 22;
    private const int RamRows = 296;

    [Fact]
    public void Constructor_NullBuffer_Throws()
        => Assert.Throws<ArgumentNullException>(() => new Ssd1680Controller(null!));

    [Theory]
    [InlineData(-1)]
    [InlineData(22)]
    public void Constructor_RamXOffsetOutOfRange_Throws(int offset)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new Ssd1680Controller(new EPaperFrameBuffer(), offset));

    [Fact]
    public void Constructor_AppliesThePowerOnState()
    {
        // ref: §SSD1680.3: 0x11 = 0x03, 0x44 = 0x00..0x15, 0x45 = 0..0x127, 0x4E = 0, 0x4F = 0, 0x3C = 0xC0, 0x22 = 0xFF, 0x10 normal.
        var buffer = new EPaperFrameBuffer();

        using var controller = new Ssd1680Controller(buffer);

        Assert.Equal(0x03, controller.DataEntryMode);
        Assert.Equal(0x00, controller.XStart);
        Assert.Equal(0x15, controller.XEnd);
        Assert.Equal(0, controller.YStart);
        Assert.Equal(0x127, controller.YEnd);
        Assert.Equal(0, controller.XCounter);
        Assert.Equal(0, controller.YCounter);
        Assert.Equal(0xC0, controller.BorderWaveform);
        Assert.Equal(0xFF, controller.UpdateControl2);
        Assert.False(controller.IsDeepSleep);
        Assert.Equal(0, controller.RamXOffsetBytes);
        Assert.Equal(RamBytesPerRow * RamRows, controller.GetRam(EPaperPlane.Black).Length);
    }

    [Fact]
    public void WindowAndCounterCommands_TakeTheirParametersAsData()
    {
        // ref: §SSD1680.5: 0x44 XSA, XEA in bytes; 0x45 YSA lo, hi, YEA lo, hi (bit 8 in the high byte); 0x4E X; 0x4F Y lo, hi.
        using var controller = new Ssd1680Controller(new EPaperFrameBuffer());

        Send(controller, 0x44, 0x02, 0x0F);
        controller.WriteCommand(0x45);
        controller.WriteData([0x05, 0x00]);
        controller.WriteData([0x27, 0x01]);
        Send(controller, 0x4E, 0x03);
        Send(controller, 0x4F, 0x2A, 0x01);

        Assert.Equal(2, controller.XStart);
        Assert.Equal(15, controller.XEnd);
        Assert.Equal(5, controller.YStart);
        Assert.Equal(295, controller.YEnd);
        Assert.Equal(3, controller.XCounter);
        Assert.Equal(298, controller.YCounter);

        // ref: §SSD1680.2: a new command byte ends the previous command and drops the parameters not yet received.
        controller.WriteCommand(0x4F);
        controller.WriteData([0x07]);
        Send(controller, 0x11, 0x01);

        Assert.Equal(298, controller.YCounter);
        Assert.Equal(0x01, controller.DataEntryMode);
    }

    [Fact]
    public void WriteRam24_XIncrementYIncrement_FillsTheWindowRowByRow()
    {
        // ref: §SSD1680.5: 0x11 0x03 (X and Y increment, AM = 0): each byte moves the X counter; the libraries stream w x h bytes row-major.
        var buffer = new EPaperFrameBuffer();
        using var controller = new Ssd1680Controller(buffer);

        Window(controller, 16, 10, 16, 2);
        Send(controller, 0x24, 0x00, 0x0F, 0xF0, 0x7E);

        var ram = controller.GetRam(EPaperPlane.Black);
        Assert.Equal(0x00, ram[(10 * RamBytesPerRow) + 2]);
        Assert.Equal(0x0F, ram[(10 * RamBytesPerRow) + 3]);
        Assert.Equal(0xF0, ram[(11 * RamBytesPerRow) + 2]);
        Assert.Equal(0x7E, ram[(11 * RamBytesPerRow) + 3]);
        Assert.Equal(4, ram.Count(value => value != 0xFF));

        // The panel window (offset 0) mirrors the RAM into the buffer RAM: bit 7 is the leftmost pixel, 0 = black.
        Assert.False(buffer.GetRamPixel(EPaperPlane.Black, 16, 10));
        Assert.False(buffer.GetRamPixel(EPaperPlane.Black, 24, 10));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 28, 10));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 16, 11));
        Assert.False(buffer.GetRamPixel(EPaperPlane.Black, 20, 11));
        Assert.False(buffer.GetRamPixel(EPaperPlane.Black, 24, 11));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 25, 11));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 15, 10));
    }

    [Theory]
    [InlineData(0x00)]
    [InlineData(0x01)]
    [InlineData(0x02)]
    [InlineData(0x03)]
    [InlineData(0x04)]
    [InlineData(0x07)]
    public void WriteRam24_AnyDataEntryMode_StaysInsideTheWindow(byte mode)
    {
        // ref: §SSD1680.5: "data are written to the RAM area specified by the window addresses in the manner specified with ID[1:0]
        // and AM". The order and the wrap at the window edge are ⚠️ unverified (§P3.X item 6): only containment is asserted,
        // with more bytes than the window holds.
        using var controller = new Ssd1680Controller(new EPaperFrameBuffer());
        Send(controller, 0x11, mode);
        Send(controller, 0x44, 0x02, 0x04);
        Send(controller, 0x45, 0x0A, 0x00, 0x0C, 0x00);
        Send(controller, 0x4E, (mode & 0x01) != 0 ? (byte)0x02 : (byte)0x04);
        Send(controller, 0x4F, (mode & 0x02) != 0 ? (byte)0x0A : (byte)0x0C, 0x00);

        controller.WriteCommand(0x24);
        controller.WriteData(new byte[25]);

        var ram = controller.GetRam(EPaperPlane.Black);
        for (var y = 0; y < RamRows; y++)
        {
            for (var x = 0; x < RamBytesPerRow; x++)
            {
                var isInside = x is >= 2 and <= 4 && y is >= 10 and <= 12;
                if (!isInside)
                {
                    Assert.Equal(0xFF, ram[(y * RamBytesPerRow) + x]);
                }
            }
        }

        Assert.Equal(0x00, ram[(((mode & 0x02) != 0 ? 10 : 12) * RamBytesPerRow) + ((mode & 0x01) != 0 ? 2 : 4)]);
    }

    [Fact]
    public void WriteRam26_WritesTheRedPlane_AndACommandEndsTheStream()
    {
        // ref: §SSD1680.2 0x26: "For Red pixel: Content of RAM0x26 = 1"; 0x7F NOP "can be used to terminate frame memory write".
        var buffer = new EPaperFrameBuffer(128, 296, hasRed: true);
        using var controller = new Ssd1680Controller(buffer);
        Window(controller, 0, 0, 128, 296);

        Send(controller, 0x26, 0x80);
        controller.WriteCommand(0x7F);
        controller.WriteData([0xFF]);

        var red = controller.GetRam(EPaperPlane.Red);
        Assert.Equal(0x80, red[0]);
        Assert.Equal(1, red.Count(value => value != 0x00));
        Assert.All(controller.GetRam(EPaperPlane.Black), value => Assert.Equal(0xFF, value));

        Send(controller, 0x22, 0xF7);
        controller.WriteCommand(0x20);

        Assert.Equal(EPaperInk.Red, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(1, 0));
    }

    [Theory]
    [InlineData(0xF7, EPaperRefreshKind.Full)]
    [InlineData(0xF4, EPaperRefreshKind.Full)]
    [InlineData(0xC7, EPaperRefreshKind.Full)]
    [InlineData(0xFC, EPaperRefreshKind.Partial)]
    [InlineData(0xCC, EPaperRefreshKind.Partial)]
    [InlineData(0x0C, EPaperRefreshKind.Partial)]
    [InlineData(0xCF, EPaperRefreshKind.Partial)]
    public void MasterActivation_RefreshesWithTheKindOfTheLast22Value(
        byte value,
        EPaperRefreshKind expected)
    {
        // ref: §SSD1680.6 table "Values the libraries send" (column "Emulator"); the bit decoding behind it is ⚠️ derived.
        var buffer = new EPaperFrameBuffer();
        using var controller = new Ssd1680Controller(buffer);
        var refreshes = new List<EPaperRefreshKind>();
        buffer.Refreshed += (_, e) => refreshes.Add(e.Kind);
        Window(controller, 0, 0, 128, 296);
        Send(controller, 0x24, 0x00);

        Send(controller, 0x22, value);

        Assert.Empty(refreshes);
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(0, 0));

        controller.WriteCommand(0x20);

        Assert.Equal([expected], refreshes);
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(0, 0));
    }

    [Theory]
    [InlineData(0xF8)]
    [InlineData(0xE0)]
    [InlineData(0x83)]
    public void MasterActivation_WithAPowerOrLutValue_DoesNotRefresh(byte value)
    {
        // ref: §SSD1680.6: F8 / E0 (power on) and 83 (power off) have no display stage: no refresh.
        var buffer = new EPaperFrameBuffer();
        using var controller = new Ssd1680Controller(buffer);
        var refreshes = 0;
        buffer.Refreshed += (_, _) => refreshes++;
        Window(controller, 0, 0, 128, 296);
        Send(controller, 0x24, 0x00);

        Send(controller, 0x22, value);
        controller.WriteCommand(0x20);

        Assert.Equal(0, refreshes);
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(0, 0));
    }

    [Fact]
    public void SoftwareReset_KeepsTheRamAndEndsTheRamWrite()
    {
        // ref: §SSD1680.2 0x12: "RAM are unaffected". (The S/W reset register defaults are ⚠️ unverified, §P3.X item 6: not asserted.)
        using var controller = new Ssd1680Controller(new EPaperFrameBuffer());
        Window(controller, 0, 0, 128, 296);
        Send(controller, 0x24, 0x12, 0x34);
        var before = controller.GetRam(EPaperPlane.Black);

        controller.WriteCommand(0x12);
        controller.WriteData([0x00, 0x00]);

        Assert.Equal(before, controller.GetRam(EPaperPlane.Black));
        Assert.False(controller.IsDeepSleep);
    }

    [Theory]
    [InlineData(0x12)]
    [InlineData(0x20)]
    [InlineData(0x22)]
    [InlineData(0x24)]
    [InlineData(0x26)]
    public void DeepSleep_IgnoresOtherCommandsAndData_UntilAWakeCommand(
        byte wakeCommand)
    {
        // ref: §SSD1680.7 0x10 D 01 = deep sleep. ⚠️ design (§SSD1680.7 / §P3.X item 5, the note's lenient recommendation): while asleep
        // data and other commands are ignored; 0x12, a refresh command (0x22, 0x20) or a RAM write command (0x24, 0x26) wakes the emulator.
        var buffer = new EPaperFrameBuffer();
        using var controller = new Ssd1680Controller(buffer);
        Window(controller, 0, 0, 128, 296);

        Send(controller, 0x10, 0x01);

        Assert.True(controller.IsDeepSleep);

        Send(controller, 0x4E, 0x05);
        Send(controller, 0x3C, 0x05);
        controller.WriteData([0x00, 0x00]);

        Assert.True(controller.IsDeepSleep);
        Assert.Equal(0, controller.XCounter);
        Assert.Equal(0xC0, controller.BorderWaveform);
        Assert.All(controller.GetRam(EPaperPlane.Black), value => Assert.Equal(0xFF, value));

        controller.WriteCommand(wakeCommand);

        Assert.False(controller.IsDeepSleep);
    }

    [Fact]
    public void DeepSleep_RamWriteCommandWakesAndWrites()
    {
        // ⚠️ design (lenient wake, §SSD1680.7): the waking 0x24 is applied, so its data goes to the RAM at the counters.
        var buffer = new EPaperFrameBuffer();
        using var controller = new Ssd1680Controller(buffer);
        var refreshes = 0;
        buffer.Refreshed += (_, _) => refreshes++;
        Window(controller, 0, 0, 128, 296);
        Send(controller, 0x10, 0x01);

        Send(controller, 0x24, 0x00);
        Send(controller, 0x22, 0xF7);
        controller.WriteCommand(0x20);

        Assert.False(controller.IsDeepSleep);
        Assert.Equal(1, refreshes);
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(0, 0));
    }

    [Fact]
    public void DeepSleep_ResetAlwaysWakes()
    {
        // ref: §SSD1680.7: "To Exit deep sleep mode, user required to send HWRESET".
        using var controller = new Ssd1680Controller(new EPaperFrameBuffer());
        Send(controller, 0x10, 0x03);

        Assert.True(controller.IsDeepSleep);

        controller.Reset();

        Assert.False(controller.IsDeepSleep);
    }

    [Fact]
    public void AdafruitFramesWithDeepSleepBetween_AndNoReset_DrawTheSecondFrame()
    {
        // ref: §LIB.ADAFRUITEPD: display() = HWRESET (not seen by a mirror), the init list starting with C 12, the planes, F4 + 20,
        // powerDown (10 01). ⚠️ design (lenient wake, §P3.X item 5): without the RST pulse the 0x12 of the next display() wakes the emulator.
        var buffer = new EPaperFrameBuffer(128, 296);
        using var controller = new Ssd1680Controller(buffer, ramXOffsetBytes: 1);
        var refreshes = new List<EPaperRefreshKind>();
        buffer.Refreshed += (_, e) => refreshes.Add(e.Kind);
        var first = Image(16, 296, 1);
        var second = Image(16, 296, 2);
        var blank = new byte[16 * 296];

        Send(controller, 0x10, 0x01);
        AdafruitDisplay(controller, 1, 0x27, 0x01, first, blank, isResetSeen: false);
        AdafruitDisplay(controller, 1, 0x27, 0x01, second, blank, isResetSeen: false);

        Assert.True(controller.IsDeepSleep);
        Assert.Equal([EPaperRefreshKind.Full, EPaperRefreshKind.Full], refreshes);
        AssertShows(buffer, second, 128, 296);
    }

    [Fact]
    public void Reset_AppliesThePowerOnState()
    {
        // ref: §SSD1680.3 POR values; §SSD1680.9 item 3: Reset (HWRESET) restores the registers. RAM retention is ⚠️ (not asserted).
        using var controller = new Ssd1680Controller(new EPaperFrameBuffer());
        Send(controller, 0x11, 0x01);
        Send(controller, 0x44, 0x01, 0x10);
        Send(controller, 0x3C, 0x05);
        Send(controller, 0x22, 0xF7);
        Send(controller, 0x4E, 0x05);

        controller.Reset();

        Assert.Equal(0x03, controller.DataEntryMode);
        Assert.Equal(0x00, controller.XStart);
        Assert.Equal(0x15, controller.XEnd);
        Assert.Equal(0xC0, controller.BorderWaveform);
        Assert.Equal(0xFF, controller.UpdateControl2);
        Assert.Equal(0, controller.XCounter);
    }

    [Fact]
    public void LongAndReadCommands_AreConsumedWithoutTouchingTheRam()
    {
        // ref: §SSD1680.2: 0x32 takes 153 bytes, 0x37 takes 10; read commands (0x27, 0x2F) return data and take none.
        using var capture = TraceCapture.Start();
        var buffer = new EPaperFrameBuffer();
        using var controller = new Ssd1680Controller(buffer);
        Window(controller, 0, 0, 128, 296);
        Send(controller, 0x24, 0x00);

        controller.WriteCommand(0x32);
        controller.WriteData(new byte[100]);
        controller.WriteData(new byte[53]);
        Send(controller, 0x37, new byte[10]);
        Send(controller, 0x3F, 0x07);
        controller.WriteCommand(0x27);
        controller.WriteCommand(0x2F);
        controller.WriteData([0x00]);

        var ram = controller.GetRam(EPaperPlane.Black);
        Assert.Equal(0x00, ram[0]);
        Assert.Equal(1, ram.Count(value => value != 0xFF));
        Assert.Equal(0, capture.Count(nameof(Ssd1680Controller)));
    }

    [Fact]
    public void GxEpd2T94_FullAndPartialUpdates_ShowTheImage()
    {
        // ref: §LIB.GXEPD2.1 (GxEPD2_290_T94, 128 x 296): HWRESET, _InitDisplay, _PowerOn (E0), writeImage (0x24), _Update_Full (F7),
        // _PowerOff (83), hibernate (10 01); then HWRESET and a partial update (FC).
        using var capture = TraceCapture.Start();
        var buffer = new EPaperFrameBuffer(128, 296);
        using var controller = new Ssd1680Controller(buffer);
        var refreshes = new List<EPaperRefreshKind>();
        buffer.Refreshed += (_, e) => refreshes.Add(e.Kind);
        var image = Image(16, 296, 0);

        controller.Reset();
        GxEpd2T94Init(controller);
        Send(controller, 0x22, 0xE0);
        controller.WriteCommand(0x20);
        Window(controller, 0, 0, 128, 296);
        Send(controller, 0x24, image);
        Send(controller, 0x22, 0xF7);
        controller.WriteCommand(0x20);
        Send(controller, 0x22, 0x83);
        controller.WriteCommand(0x20);
        Send(controller, 0x10, 0x01);

        Assert.Equal([EPaperRefreshKind.Full], refreshes);
        AssertShows(buffer, image, 128, 296);

        // The next use: HWRESET, init, the partial window (bytes 2..3, rows 8..15), then _Update_Part.
        var patch = Enumerable.Repeat((byte)0x00, 16).ToArray();
        controller.Reset();
        GxEpd2T94Init(controller);
        Window(controller, 16, 8, 16, 8);
        Send(controller, 0x24, patch);
        Window(controller, 16, 8, 16, 8);
        Send(controller, 0x22, 0xFC);
        controller.WriteCommand(0x20);

        for (var y = 8; y < 16; y++)
        {
            for (var bx = 2; bx < 4; bx++)
            {
                image[(y * 16) + bx] = 0x00;
            }
        }

        Assert.Equal([EPaperRefreshKind.Full, EPaperRefreshKind.Partial], refreshes);
        AssertShows(buffer, image, 128, 296);
        Assert.Equal(0, capture.Count(nameof(Ssd1680Controller)));
    }

    [Fact]
    public void GxEpd2Bn_FullUpdate_RefreshesOnce()
    {
        // ref: §LIB.GXEPD2.2 GxEPD2_213_BN: 296 MUX on 250 rows; _Update_Full = _PowerOn (F8 + 20) then F4 + 20; _PowerOff 83 + 20.
        var buffer = new EPaperFrameBuffer(122, 250);
        using var controller = new Ssd1680Controller(buffer);
        var refreshes = new List<EPaperRefreshKind>();
        buffer.Refreshed += (_, e) => refreshes.Add(e.Kind);
        var image = Image(16, 250, 3);

        controller.Reset();
        GxEpd2T94Init(controller);
        Window(controller, 0, 0, 128, 250);
        Send(controller, 0x24, image);
        Send(controller, 0x22, 0xF8);
        controller.WriteCommand(0x20);
        Send(controller, 0x22, 0xF4);
        controller.WriteCommand(0x20);
        Send(controller, 0x22, 0x83);
        controller.WriteCommand(0x20);

        Assert.Equal([EPaperRefreshKind.Full], refreshes);
        Assert.Equal(122, buffer.Width);
        Assert.Equal(250, buffer.Height);
        AssertShows(buffer, image, 122, 250);
    }

    [Fact]
    public void AdafruitMono290_WithOffset1_ShowsTheImageAndIgnoresPlane2()
    {
        // ref: §LIB.ADAFRUITEPD (ThinkInk_290_Mono_BN, _xram_offset = 1): begin() ends with powerDown (10 01); display() = HWRESET,
        // the default init list, 0x44 01 10, 0x45 00 00 27 01, 0x01 27 01 00, plane 1 (0x24), plane 2 (0x26, never drawn into),
        // update (22 F4, 20), powerDown. §SSD1680.1: the offset is a property of the panel (RamXOffsetBytes = 1).
        using var capture = TraceCapture.Start();
        var buffer = new EPaperFrameBuffer(128, 296);
        using var controller = new Ssd1680Controller(buffer, ramXOffsetBytes: 1);
        var refreshes = new List<EPaperRefreshKind>();
        buffer.Refreshed += (_, e) => refreshes.Add(e.Kind);
        var image = Image(16, 296, 5);

        controller.Reset();
        Send(controller, 0x10, 0x01);
        AdafruitDisplay(controller, 1, 0x27, 0x01, image, Enumerable.Repeat((byte)0xA5, image.Length).ToArray());

        Assert.Equal([EPaperRefreshKind.Full], refreshes);
        AssertShows(buffer, image, 128, 296);
        Assert.Equal(0, capture.Count(nameof(Ssd1680Controller)));

        // Without the offset the same traffic shows the picture 8 pixels to the left.
        controller.RamXOffsetBytes = 0;
        controller.Reset();
        AdafruitDisplay(controller, 1, 0x27, 0x01, image, new byte[image.Length]);

        Assert.Equal(Ink(image, 16, 8, 0), buffer.GetShownPixel(16, 0));
        Assert.Equal(Ink(image, 16, 0, 7), buffer.GetShownPixel(8, 7));
    }

    [Fact]
    public void AdafruitTricolor290_ShowsBlackAndRed()
    {
        // ref: §LIB.ADAFRUITEPD (ThinkInk_290_Tricolor_Z94, _xram_offset = 0): plane 1 0 = black, plane 2 1 = red, unchanged on the wire.
        var buffer = new EPaperFrameBuffer(128, 296, hasRed: true);
        using var controller = new Ssd1680Controller(buffer);
        var black = Enumerable.Repeat((byte)0xFF, 16 * 296).ToArray();
        var red = new byte[16 * 296];
        black[0] = 0x7F;
        red[0] = 0x40;
        red[(295 * 16) + 15] = 0x01;

        controller.Reset();
        AdafruitDisplay(controller, 0, 0x27, 0x01, black, red);

        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.Red, buffer.GetShownPixel(1, 0));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(2, 0));
        Assert.Equal(EPaperInk.Red, buffer.GetShownPixel(127, 295));
    }

    [Fact]
    public void Bw_IgnoresRedPlane()
    {
        // ref: §SSD1680.4 item 3: on a black/white panel the red plane (0x26) is not an image (previous image or garbage).
        var buffer = new EPaperFrameBuffer(128, 296);
        using var controller = new Ssd1680Controller(buffer);
        Window(controller, 0, 0, 128, 296);
        controller.WriteCommand(0x26);
        controller.WriteData(Enumerable.Repeat((byte)0xFF, 16 * 296).ToArray());
        Window(controller, 0, 0, 128, 296);
        Send(controller, 0x24, 0x0F);

        Send(controller, 0x22, 0xF7);
        controller.WriteCommand(0x20);

        Assert.True(buffer.GetRamPixel(EPaperPlane.Red, 0, 0));
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(4, 0));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(127, 295));
    }

    [Theory]
    [InlineData(128, 296)]
    [InlineData(122, 250)]
    public void FramePathAndRamPath_ShowTheSamePixels(
        int width,
        int height)
    {
        // ref: protocol EPaperFrame = the SSD1680 RAM layout (R8, §SSD1680.4 items 1 and 6); RAM path per §LIB.GXEPD2.1 (T94, 128 x 296)
        // and §LIB.GXEPD2.2 (B74, 122 x 250: 16 bytes per row, bits of pixels 122..127 are padding).
        var image = Image(16, height, 9);
        var framed = new EPaperFrameBuffer(width, height);
        var streamed = new EPaperFrameBuffer(width, height);

        var result = DisplayMirrorTargets.For(framed).Apply(
            new DisplayMirrorPacket(
                0,
                DisplayMirrorMessageType.EPaperFrame,
                0,
                DisplayMirrorProtocol.CreateEPaperFramePayload(width, height, image, [], isPartial: false)));

        using var controller = new Ssd1680Controller(streamed);
        controller.Reset();
        if (height == 250)
        {
            GxEpd2B74Init(controller);
        }
        else
        {
            GxEpd2T94Init(controller);
        }

        Window(controller, 0, 0, 128, height);
        Send(controller, 0x24, image);
        Send(controller, 0x22, 0xF7);
        controller.WriteCommand(0x20);

        Assert.Equal(DisplayMirrorRejectReason.None, result);
        var blacks = 0;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var shown = framed.GetShownPixel(x, y);
                Assert.Equal(shown, streamed.GetShownPixel(x, y));
                blacks += shown == EPaperInk.Black ? 1 : 0;
            }
        }

        Assert.InRange(blacks, 1, (width * height) - 1);
    }

    [Fact]
    public void RamXOffsetAndResize_RemapTheWindowIntoTheBufferRam()
    {
        var buffer = new EPaperFrameBuffer(16, 4);
        using var controller = new Ssd1680Controller(buffer);
        Send(controller, 0x44, 0x00, 0x15);
        Send(controller, 0x45, 0x00, 0x00, 0x03, 0x00);
        Send(controller, 0x4E, 0x00);
        Send(controller, 0x4F, 0x00, 0x00);
        controller.WriteCommand(0x24);
        controller.WriteData(Enumerable.Range(0, 22).Select(i => (byte)(i == 1 ? 0x00 : (i == 2 ? 0x0F : 0xFF))).ToArray());

        Assert.False(buffer.GetRamPixel(EPaperPlane.Black, 8, 0));

        controller.RamXOffsetBytes = 1;

        Assert.False(buffer.GetRamPixel(EPaperPlane.Black, 0, 0));
        Assert.False(buffer.GetRamPixel(EPaperPlane.Black, 8, 0));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 12, 0));

        buffer.Resize(24, 4);

        Assert.False(buffer.GetRamPixel(EPaperPlane.Black, 0, 0));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 16, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => controller.RamXOffsetBytes = 22);
    }

    [Fact]
    public void WriteCalls_RaiseChangedOnlyForVisibleChanges()
    {
        var buffer = new EPaperFrameBuffer();
        using var controller = new Ssd1680Controller(buffer);
        var changes = 0;
        buffer.Changed += (_, _) => changes++;
        Window(controller, 0, 0, 128, 296);

        Send(controller, 0x24, 0x00, 0x00);

        Assert.Equal(0, changes);

        controller.WriteCommands([0x7F, 0x7F]);
        Send(controller, 0x22, 0xF7);
        controller.WriteCommand(0x20);

        Assert.Equal(1, changes);

        controller.WriteCommand(0x20);

        Assert.Equal(1, changes);
    }

    [Fact]
    public void Refreshed_IsRaisedAfterTheControllerLockIsReleased()
    {
        var buffer = new EPaperFrameBuffer();
        using var controller = new Ssd1680Controller(buffer);
        var isOtherThreadDone = false;
        buffer.Refreshed += (_, _) =>
        {
            using var done = new ManualResetEventSlim();
            var thread = new Thread(() =>
            {
                controller.WriteCommand(0x7F);
                done.Set();
            });
            thread.Start();
            isOtherThreadDone = done.Wait(TimeSpan.FromSeconds(5));
        };

        Send(controller, 0x22, 0xF7);
        controller.WriteCommand(0x20);

        Assert.True(isOtherThreadDone);
    }

    [Fact]
    public void Dispose_LaterWritesAreIgnored()
    {
        var buffer = new EPaperFrameBuffer();
        var controller = new Ssd1680Controller(buffer);
        Window(controller, 0, 0, 128, 296);
        controller.Dispose();
        var refreshes = 0;
        buffer.Refreshed += (_, _) => refreshes++;

        Send(controller, 0x24, 0x00);
        Send(controller, 0x22, 0xF7);
        controller.WriteCommand(0x20);
        controller.Reset();
        controller.RamXOffsetBytes = 1;
        buffer.Resize(64, 64);
        controller.Dispose();

        Assert.Equal(0, refreshes);
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 0, 0));
    }

    // A deterministic, asymmetric test picture: bytesPerRow x rows bytes.
    private static byte[] Image(
        int bytesPerRow,
        int rows,
        int seed)
    {
        var image = new byte[bytesPerRow * rows];
        for (var y = 0; y < rows; y++)
        {
            for (var bx = 0; bx < bytesPerRow; bx++)
            {
                image[(y * bytesPerRow) + bx] = (byte)((bx * 37) ^ (y * 11) ^ (y >> 3) ^ seed);
            }
        }

        return image;
    }

    // ref: §SSD1680.4 items 1 and 6: bit 7 - x % 8 of byte x / 8, 0 = black.
    private static EPaperInk Ink(
        byte[] image,
        int bytesPerRow,
        int x,
        int y)
        => (image[(y * bytesPerRow) + (x / 8)] & (0x80 >> (x % 8))) == 0
            ? EPaperInk.Black
            : EPaperInk.White;

    private static void AssertShows(
        EPaperFrameBuffer buffer,
        byte[] image,
        int width,
        int height)
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                Assert.Equal(Ink(image, 16, x, y), buffer.GetShownPixel(x, y));
            }
        }
    }

    // ref: §LIB.GXEPD2.1 _InitDisplay of GxEPD2_290_T94 (also BS / BN, §LIB.GXEPD2.2), with _setPartialRamArea(0, 0, 128, 296).
    private static void GxEpd2T94Init(Ssd1680Controller controller)
    {
        controller.WriteCommand(0x12);
        Send(controller, 0x01, 0x27, 0x01, 0x00);
        Send(controller, 0x11, 0x03);
        Send(controller, 0x3C, 0x05);
        Send(controller, 0x21, 0x00, 0x80);
        Send(controller, 0x18, 0x80);
        Window(controller, 0, 0, 128, 296);
    }

    // ref: §LIB.GXEPD2.2 GxEPD2_213_B74::_InitDisplay: 250 MUX lines, the window 128 x 250.
    private static void GxEpd2B74Init(Ssd1680Controller controller)
    {
        controller.WriteCommand(0x12);
        Send(controller, 0x01, 0xF9, 0x00, 0x00);
        Send(controller, 0x3C, 0x05);
        Send(controller, 0x21, 0x00, 0x80);
        Send(controller, 0x18, 0x80);
        Window(controller, 0, 0, 128, 250);
    }

    // ref: §SSD1680.5 GxEPD2 _setPartialRamArea(x, y, w, h).
    private static void Window(
        Ssd1680Controller controller,
        int x,
        int y,
        int w,
        int h)
    {
        Send(controller, 0x11, 0x03);
        Send(controller, 0x44, (byte)(x / 8), (byte)((x + w - 1) / 8));
        Send(controller, 0x45, (byte)(y % 256), (byte)(y / 256), (byte)((y + h - 1) % 256), (byte)((y + h - 1) / 256));
        Send(controller, 0x4E, (byte)(x / 8));
        Send(controller, 0x4F, (byte)(y % 256), (byte)(y / 256));
    }

    // ref: §LIB.ADAFRUITEPD display(): powerUp (HWRESET, default init list, 0x44, 0x45, 0x01), plane 1, plane 2, update (F4), powerDown.
    private static void AdafruitDisplay(
        Ssd1680Controller controller,
        byte offset,
        byte lastRowLow,
        byte lastRowHigh,
        byte[] plane1,
        byte[] plane2,
        bool isResetSeen = true)
    {
        if (isResetSeen)
        {
            controller.Reset();
        }

        controller.WriteCommand(0x12);
        Send(controller, 0x11, 0x03);
        Send(controller, 0x3C, 0x05);
        Send(controller, 0x2C, 0x36);
        Send(controller, 0x03, 0x17);
        Send(controller, 0x04, 0x41, 0x00, 0x32);
        Send(controller, 0x4E, 0x01);
        Send(controller, 0x4F, 0x00, 0x00);
        Send(controller, 0x44, offset, (byte)(15 + offset));
        Send(controller, 0x45, 0x00, 0x00, lastRowLow, lastRowHigh);
        Send(controller, 0x01, lastRowLow, lastRowHigh, 0x00);
        Send(controller, 0x4E, offset);
        Send(controller, 0x4F, 0x00, 0x00);
        Send(controller, 0x24, plane1);
        Send(controller, 0x4E, offset);
        Send(controller, 0x4F, 0x00, 0x00);
        Send(controller, 0x26, plane2);
        Send(controller, 0x22, 0xF4);
        controller.WriteCommand(0x20);
        Send(controller, 0x10, 0x01);
    }

    private static void Send(
        Ssd1680Controller controller,
        byte command,
        params byte[] data)
    {
        controller.WriteCommand(command);
        controller.WriteData(data);
    }
}