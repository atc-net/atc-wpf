namespace Atc.Wpf.Displays.Tests;

[Collection(nameof(TraceTestGroup))]
public sealed class Max7219ControllerTests
{
    [Fact]
    public void Constructor_NullBuffer_Throws()
        => Assert.Throws<ArgumentNullException>(() => new Max7219Controller(null!));

    [Fact]
    public void PowerOn_IsShutdown()
    {
        // ref: §MAX7219.2: shutdown, display blanked, decode 0, intensity 0, scan limit 0 (digit 0 only), display test off.
        var buffer = new Max7219ChainBuffer(3);
        buffer.SetDigit(1, 4, 0xFF);
        buffer.SetIntensity(1, 9);
        buffer.SetShutdown(1, false);
        buffer.SetDisplayTest(2, true);

        using var controller = new Max7219Controller(buffer);

        for (var device = 0; device < 3; device++)
        {
            Assert.True(buffer.IsShutdown(device));
            Assert.False(buffer.IsDisplayTest(device));
            Assert.Equal(0, buffer.GetDecodeMode(device));
            Assert.Equal(0, buffer.GetIntensity(device));
            Assert.Equal(0, buffer.GetScanLimit(device));
            for (var digit = 0; digit < 8; digit++)
            {
                Assert.Equal(0, buffer.GetDigit(device, digit));
            }
        }
    }

    [Fact]
    public void WriteData_LiteralFrameOnAFourDeviceChain_WritesOnlyTheAddressedDevice()
    {
        // ref: §MAX7219.1 (word = address byte, data byte; 0x0 is No-op) and §MAX7219.6 (the first word of a frame reaches the far end).
        // The second of four words lands on device 2: Digit 3 (address 0x4) = 0xA0; the No-op words leave devices 3, 1 and 0 alone.
        var buffer = new Max7219ChainBuffer(4);
        using var controller = new Max7219Controller(buffer);

        controller.WriteData([0x00, 0x00, 0x04, 0xA0, 0x00, 0x00, 0x00, 0x00]);

        for (var device = 0; device < 4; device++)
        {
            for (var digit = 0; digit < 8; digit++)
            {
                var expected = device == 2 && digit == 3 ? 0xA0 : 0x00;
                Assert.Equal(expected, buffer.GetDigit(device, digit));
            }
        }
    }

    [Fact]
    public void Reset_ReturnsToThePowerOnStateAndClearsTheHeldWords()
    {
        // ref: §MAX7219.2, §MAX7219.6 (the words held between frames).
        var buffer = new Max7219ChainBuffer(2);
        using var controller = new Max7219Controller(buffer);
        controller.WriteData([0x01, 0xAA, 0x01, 0xBB, 0x0C, 0x01, 0x0C, 0x01]);

        controller.Reset();
        controller.WriteData([0x02, 0x11]);

        Assert.True(buffer.IsShutdown(0));
        Assert.Equal(0, buffer.GetDigit(0, 0));
        Assert.Equal(0x11, buffer.GetDigit(0, 1));

        // Device 1 re-executes the word device 0 held before this frame: none after the reset, a no-op.
        Assert.Equal(0, buffer.GetDigit(1, 1));
    }

    [Fact]
    public void Frame_FirstPairGoesToTheFarthestDevice()
    {
        // ref: §MAX7219.6: the first word shifted out ends up in the device farthest from DIN, the last word in device 0.
        var buffer = new Max7219ChainBuffer(4);
        using var controller = new Max7219Controller(buffer);

        controller.WriteData([0x01, 0x11, 0x01, 0x22, 0x01, 0x33, 0x01, 0x44]);

        Assert.Equal(0x44, buffer.GetDigit(0, 0));
        Assert.Equal(0x33, buffer.GetDigit(1, 0));
        Assert.Equal(0x22, buffer.GetDigit(2, 0));
        Assert.Equal(0x11, buffer.GetDigit(3, 0));
    }

    [Fact]
    public void Frame_DatasheetExample_WordThenThreeNoOpsAddressesTheFourthChip()
    {
        // ref: §MAX7219.6 (DS-7219 P 10): the desired word, then three no-op words: only the fourth chip receives the data.
        var buffer = new Max7219ChainBuffer(4);
        using var controller = new Max7219Controller(buffer);

        controller.WriteData([0x03, 0xAB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]);

        Assert.Equal(0xAB, buffer.GetDigit(3, 2));
        for (var device = 0; device < 3; device++)
        {
            Assert.Equal(0, buffer.GetDigit(device, 2));
        }
    }

    [Fact]
    public void NoOpRegister_LeavesADeviceUnchanged()
    {
        // ref: §MAX7219.1: address 0 is a no-op whatever its data byte; used to skip a device in a chain.
        var buffer = new Max7219ChainBuffer(2);
        using var controller = new Max7219Controller(buffer);
        controller.WriteData([0x01, 0x5A, 0x01, 0xA5]);
        controller.WriteData([0x0C, 0x01, 0x0C, 0x01]);

        controller.WriteData([0x00, 0xFF, 0x01, 0x77]);

        Assert.Equal(0x77, buffer.GetDigit(0, 0));
        Assert.Equal(0x5A, buffer.GetDigit(1, 0));
        Assert.False(buffer.IsShutdown(1));
        Assert.Equal(0, buffer.GetDecodeMode(1));
        Assert.Equal(0, buffer.GetIntensity(1));
        Assert.Equal(0, buffer.GetScanLimit(1));
        Assert.False(buffer.IsDisplayTest(1));
    }

    [Theory]
    [InlineData(0x01, 0)]
    [InlineData(0x02, 1)]
    [InlineData(0x03, 2)]
    [InlineData(0x04, 3)]
    [InlineData(0x05, 4)]
    [InlineData(0x06, 5)]
    [InlineData(0x07, 6)]
    [InlineData(0x08, 7)]
    public void EveryRegister_SetsItsState_Digits(
        byte address,
        int digit)
    {
        // ref: §MAX7219.1: 0x1..0x8 = digit 0..7, 8 bits each.
        var buffer = new Max7219ChainBuffer(1);
        using var controller = new Max7219Controller(buffer);

        controller.WriteData([address, 0xC3]);

        for (var other = 0; other < 8; other++)
        {
            Assert.Equal(other == digit ? 0xC3 : 0, buffer.GetDigit(0, other));
        }
    }

    [Fact]
    public void EveryRegister_SetsItsState_Control()
    {
        // ref: §MAX7219.1: 0x9 decode (8 bits), 0xA intensity (low nibble), 0xB scan limit (low 3 bits), 0xC shutdown (bit 0,
        // 0 = shutdown), 0xF display test (bit 0).
        var buffer = new Max7219ChainBuffer(1);
        using var controller = new Max7219Controller(buffer);

        controller.WriteData([0x09, 0xA5]);
        controller.WriteData([0x0A, 0xFB]);
        controller.WriteData([0x0B, 0xFD]);
        controller.WriteData([0x0C, 0xFF]);
        controller.WriteData([0x0F, 0x03]);

        Assert.Equal(0xA5, buffer.GetDecodeMode(0));
        Assert.Equal(0x0B, buffer.GetIntensity(0));
        Assert.Equal(0x05, buffer.GetScanLimit(0));
        Assert.False(buffer.IsShutdown(0));
        Assert.True(buffer.IsDisplayTest(0));

        controller.WriteData([0x0C, 0xFE]);
        controller.WriteData([0x0F, 0xFE]);

        Assert.True(buffer.IsShutdown(0));
        Assert.False(buffer.IsDisplayTest(0));
    }

    [Fact]
    public void Address_UsesOnlyTheLowNibbleOfTheFirstByte()
    {
        // ref: §MAX7219.1: D15..D12 are don't care.
        var buffer = new Max7219ChainBuffer(1);
        using var controller = new Max7219Controller(buffer);

        controller.WriteData([0xF3, 0x42]);

        Assert.Equal(0x42, buffer.GetDigit(0, 2));
    }

    [Fact]
    public void UnusedAddresses_AreIgnoredAndTracedOncePerAddress()
    {
        // ref: §MAX7219.1: 0xD and 0xE are not registers (⚠️ unverified: ignored).
        var buffer = new Max7219ChainBuffer(1);
        using var capture = TraceCapture.Start();
        using var controller = new Max7219Controller(buffer);

        controller.WriteData([0x0D, 0xFF]);
        controller.WriteData([0x0D, 0x01]);
        controller.WriteData([0x0E, 0xFF]);

        Assert.Equal(1, capture.Count(nameof(Max7219Controller), "0xD"));
        Assert.Equal(1, capture.Count(nameof(Max7219Controller), "0xE"));
        Assert.Equal(0, buffer.GetDecodeMode(0));
        Assert.True(buffer.IsShutdown(0));
    }

    [Fact]
    public void ShortFrame_PerNote()
    {
        // ref: §MAX7219.6 (⚠️ derived from the shift-register description, not hardware): a frame of k words sets the nearest k devices;
        // devices k..N-1 execute the words that devices 0..N-1-k held before the frame.
        var buffer = new Max7219ChainBuffer(4);
        using var controller = new Max7219Controller(buffer);

        // Held afterwards: device 3 = (1, 0x01), device 2 = (1, 0x02), device 1 = (1, 0x03), device 0 = (1, 0x04).
        controller.WriteData([0x01, 0x01, 0x01, 0x02, 0x01, 0x03, 0x01, 0x04]);

        // Two words: 0x0A -> device 1, 0x0B -> device 0; device 2 repeats device 0's old word, device 3 repeats device 1's old word.
        controller.WriteData([0x02, 0x0A, 0x02, 0x0B]);

        Assert.Equal(0x0B, buffer.GetDigit(0, 1));
        Assert.Equal(0x0A, buffer.GetDigit(1, 1));
        Assert.Equal(0x00, buffer.GetDigit(2, 1));
        Assert.Equal(0x00, buffer.GetDigit(3, 1));
        Assert.Equal(0x04, buffer.GetDigit(0, 0));
        Assert.Equal(0x03, buffer.GetDigit(1, 0));
        Assert.Equal(0x04, buffer.GetDigit(2, 0));
        Assert.Equal(0x03, buffer.GetDigit(3, 0));
    }

    [Fact]
    public void ShortFrame_OneWord_OnlyDeviceZeroGetsNewData()
    {
        // ref: §MAX7219.6 (⚠️ derived): one word goes to device 0; the others repeat the previous frame's words shifted along.
        var buffer = new Max7219ChainBuffer(3);
        using var controller = new Max7219Controller(buffer);
        controller.WriteData([0x01, 0x01, 0x01, 0x02, 0x01, 0x03]);

        controller.WriteData([0x05, 0x99]);

        Assert.Equal(0x99, buffer.GetDigit(0, 4));
        Assert.Equal(0x00, buffer.GetDigit(1, 4));
        Assert.Equal(0x00, buffer.GetDigit(2, 4));
        Assert.Equal(0x03, buffer.GetDigit(1, 0));
        Assert.Equal(0x02, buffer.GetDigit(2, 0));
    }

    [Fact]
    public void LongFrame_LosesTheFirstWords()
    {
        // ref: §MAX7219.6 (⚠️ derived): a frame of more than N words loses the first words, which fall out of DOUT.
        var buffer = new Max7219ChainBuffer(2);
        using var controller = new Max7219Controller(buffer);

        controller.WriteData([0x01, 0xEE, 0x01, 0x11, 0x01, 0x22, 0x01, 0x33]);

        Assert.Equal(0x33, buffer.GetDigit(0, 0));
        Assert.Equal(0x22, buffer.GetDigit(1, 0));
    }

    [Fact]
    public void OddTrailingByte_IsIgnoredAndTracedOnce()
    {
        // The brief: an odd trailing byte is ignored and traced (⚠️ the note says it would misalign the words on hardware).
        var buffer = new Max7219ChainBuffer(2);
        using var capture = TraceCapture.Start();
        using var controller = new Max7219Controller(buffer);

        controller.WriteData([0x01, 0x11, 0x01, 0x22, 0x07]);
        controller.WriteData([0x01, 0x11, 0x01, 0x22, 0x07]);

        Assert.Equal(0x22, buffer.GetDigit(0, 0));
        Assert.Equal(0x11, buffer.GetDigit(1, 0));
        Assert.Equal(1, capture.Count(nameof(Max7219Controller), "odd"));
    }

    [Fact]
    public void EmptyFrame_DoesNothing()
    {
        var buffer = new Max7219ChainBuffer(2);
        using var controller = new Max7219Controller(buffer);
        controller.WriteData([0x01, 0x11, 0x01, 0x22]);

        controller.WriteData([]);

        Assert.Equal(0x22, buffer.GetDigit(0, 0));
        Assert.Equal(0x11, buffer.GetDigit(1, 0));
    }

    [Fact]
    public void WriteCommand_IsIgnoredAndTracedOncePerValue()
    {
        var buffer = new Max7219ChainBuffer(1);
        using var capture = TraceCapture.Start();
        using var controller = new Max7219Controller(buffer);

        controller.WriteCommand(0x0C);
        controller.WriteCommands([0x0C, 0x0F]);

        Assert.True(buffer.IsShutdown(0));
        Assert.Equal(1, capture.Count(nameof(Max7219Controller), "0x0C"));
        Assert.Equal(1, capture.Count(nameof(Max7219Controller), "0x0F"));
    }

    [Fact]
    public void WriteData_RaisesChangedOncePerFrame_AndNothingForANoOpWrite()
    {
        var buffer = new Max7219ChainBuffer(4);
        using var controller = new Max7219Controller(buffer);
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        controller.WriteData([0x01, 0x11, 0x01, 0x22, 0x01, 0x33, 0x01, 0x44]);
        Assert.Equal(1, changes);

        controller.WriteData([0x01, 0x11, 0x01, 0x22, 0x01, 0x33, 0x01, 0x44]);
        Assert.Equal(1, changes);
    }

    [Fact]
    public void Dispose_MakesTheControllerInert()
    {
        var buffer = new Max7219ChainBuffer(1);
        var controller = new Max7219Controller(buffer);
        controller.WriteData([0x01, 0x11]);

        controller.Dispose();
        controller.WriteData([0x01, 0x22]);
        controller.WriteCommand(0x01);
        controller.Reset();
        controller.Dispose();

        Assert.Equal(0x11, buffer.GetDigit(0, 0));
    }

    [Fact]
    public void Resize_KeepsTheHeldWordsOfTheRemainingDevices()
    {
        // The words held follow the chain when the buffer is resized: a short frame repeats what the nearest devices held.
        var buffer = new Max7219ChainBuffer(2);
        using var controller = new Max7219Controller(buffer);
        controller.WriteData([0x01, 0x01, 0x01, 0x02]);

        buffer.Resize(3);
        controller.WriteData([0x02, 0x55]);

        Assert.Equal(0x55, buffer.GetDigit(0, 1));
        Assert.Equal(0x02, buffer.GetDigit(1, 0));
        Assert.Equal(0x01, buffer.GetDigit(2, 0));
    }

    [Fact]
    public void LedControlReplay_FourDevices()
    {
        // ref: §LIB.LEDCONTROL: the constructor sends, for each device in turn, test off (0x0F 0), scan limit 7 (0x0B 7), decode off
        // (0x09 0), clearDisplay (0x01..0x08 = 0) and shutdown on (0x0C 0); every call is a full-length frame, the other devices get
        // 0x00 0x00. spiTransfer shifts spidata[2N - 1] first: device N-1's pair is first, device 0's last (§MAX7219.6).
        var buffer = new Max7219ChainBuffer(4);
        buffer.SetDigit(2, 5, 0x66);
        buffer.SetIntensity(1, 7);
        using var controller = new Max7219Controller(buffer);

        for (var addr = 0; addr < 4; addr++)
        {
            LedControlTransfer(controller, 4, addr, 0x0F, 0x00);
            LedControlTransfer(controller, 4, addr, 0x0B, 0x07);
            LedControlTransfer(controller, 4, addr, 0x09, 0x00);
            for (var row = 0; row < 8; row++)
            {
                LedControlTransfer(controller, 4, addr, (byte)(row + 1), 0x00);
            }

            LedControlTransfer(controller, 4, addr, 0x0C, 0x00);
        }

        for (var device = 0; device < 4; device++)
        {
            Assert.Equal(7, buffer.GetScanLimit(device));
            Assert.Equal(0, buffer.GetDecodeMode(device));
            Assert.True(buffer.IsShutdown(device));
            Assert.False(buffer.IsDisplayTest(device));
            Assert.Equal(0, buffer.GetIntensity(device));
        }

        Assert.Equal(0, buffer.GetDigit(2, 5));

        // shutdown(1, false); setIntensity(2, 8); setRow(1, 3, 0xA5); setRow(3, 0, 0xFF); setRow(0, 7, 0x01).
        LedControlTransfer(controller, 4, 1, 0x0C, 0x01);
        LedControlTransfer(controller, 4, 2, 0x0A, 0x08);
        LedControlTransfer(controller, 4, 1, 0x04, 0xA5);
        LedControlTransfer(controller, 4, 3, 0x01, 0xFF);
        LedControlTransfer(controller, 4, 0, 0x08, 0x01);

        Assert.False(buffer.IsShutdown(1));
        Assert.True(buffer.IsShutdown(0));
        Assert.True(buffer.IsShutdown(2));
        Assert.True(buffer.IsShutdown(3));
        Assert.Equal(8, buffer.GetIntensity(2));
        Assert.Equal(0, buffer.GetIntensity(1));
        Assert.Equal(0xA5, buffer.GetDigit(1, 3));
        Assert.Equal(0xFF, buffer.GetDigit(3, 0));
        Assert.Equal(0x01, buffer.GetDigit(0, 7));
        Assert.Equal(0, buffer.GetDigit(0, 3));

        // clearDisplay(1): eight frames of 0 for device 1 only.
        for (var row = 0; row < 8; row++)
        {
            LedControlTransfer(controller, 4, 1, (byte)(row + 1), 0x00);
        }

        Assert.Equal(0, buffer.GetDigit(1, 3));
        Assert.Equal(0xFF, buffer.GetDigit(3, 0));
        Assert.Equal(0x01, buffer.GetDigit(0, 7));
    }

    [Fact]
    public void MdMax72xxReplay_ThreeDevices()
    {
        // ref: §LIB.MDMAX72XX.1: begin() sends the same pair for every device: 0x0F 0, 0x0B 7, 0x0A 7, 0x09 0, clear() (0x01..0x08 =
        // 0), 0x0C 1. A digit write is a frame with [0x01 + digit, byte] for one device and 0x00 0x00 for the others: the example
        // frame "00 00 (dev 2), 03 5A (dev 1), 00 00 (dev 0)" writes digit 2 of device 1.
        var buffer = new Max7219ChainBuffer(3);
        using var controller = new Max7219Controller(buffer);

        SendToAll(controller, 3, 0x0F, 0x00);
        SendToAll(controller, 3, 0x0B, 0x07);
        SendToAll(controller, 3, 0x0A, 0x07);
        SendToAll(controller, 3, 0x09, 0x00);
        for (var row = 0; row < 8; row++)
        {
            SendToAll(controller, 3, (byte)(0x01 + row), 0x00);
        }

        SendToAll(controller, 3, 0x0C, 0x01);

        for (var device = 0; device < 3; device++)
        {
            Assert.False(buffer.IsShutdown(device));
            Assert.Equal(7, buffer.GetIntensity(device));
            Assert.Equal(7, buffer.GetScanLimit(device));
        }

        controller.WriteData([0x00, 0x00, 0x03, 0x5A, 0x00, 0x00]);

        Assert.Equal(0x5A, buffer.GetDigit(1, 2));
        Assert.Equal(0, buffer.GetDigit(0, 2));
        Assert.Equal(0, buffer.GetDigit(2, 2));

        // flushBufferAll for digit 0: one frame with a pair for every changed device (here 0x80 for module 2, 0x01 for module 0).
        controller.WriteData([0x01, 0x80, 0x00, 0x00, 0x01, 0x01]);

        Assert.Equal(0x80, buffer.GetDigit(2, 0));
        Assert.Equal(0x00, buffer.GetDigit(1, 0));
        Assert.Equal(0x01, buffer.GetDigit(0, 0));
        Assert.Equal(0x5A, buffer.GetDigit(1, 2));
    }

    [Fact]
    public void U8g2Max7219Replay_FourDevices()
    {
        // ref: §LIB.U8G2MAX7219: init = frames of N identical pairs: 0x0F 0, 0x0C 0, 0x09 0, 0x0A 0x0A (intensity 10), 0x0B 7; power
        // save off = 0x0C 1. Draw: for i = 0..7 one frame of the pairs [i + 1, byte of tile j], j = 0..N-1, tile 0 first: it lands in
        // the farthest device (device N-1).
        var buffer = new Max7219ChainBuffer(4);
        using var controller = new Max7219Controller(buffer);

        SendToAll(controller, 4, 0x0F, 0x00);
        SendToAll(controller, 4, 0x0C, 0x00);
        SendToAll(controller, 4, 0x09, 0x00);
        SendToAll(controller, 4, 0x0A, 0x0A);
        SendToAll(controller, 4, 0x0B, 0x07);

        for (var device = 0; device < 4; device++)
        {
            Assert.True(buffer.IsShutdown(device));
            Assert.Equal(10, buffer.GetIntensity(device));
            Assert.Equal(7, buffer.GetScanLimit(device));
        }

        for (var row = 0; row < 8; row++)
        {
            // Tile j, row i: byte 0x10 * j + i (hand-picked, distinct per tile and row).
            byte[] frame = new byte[8];
            for (var tile = 0; tile < 4; tile++)
            {
                frame[tile * 2] = (byte)(row + 1);
                frame[(tile * 2) + 1] = (byte)((0x10 * tile) + row);
            }

            controller.WriteData(frame);
        }

        SendToAll(controller, 4, 0x0C, 0x01);

        for (var row = 0; row < 8; row++)
        {
            Assert.Equal((0x10 * 0) + row, buffer.GetDigit(3, row));
            Assert.Equal((0x10 * 1) + row, buffer.GetDigit(2, row));
            Assert.Equal((0x10 * 2) + row, buffer.GetDigit(1, row));
            Assert.Equal((0x10 * 3) + row, buffer.GetDigit(0, row));
        }

        for (var device = 0; device < 4; device++)
        {
            Assert.False(buffer.IsShutdown(device));
        }
    }

    [Fact]
    public void LatchPacket_ReachesTheBufferThroughTheGraphicControllerTarget()
    {
        // DisplayMirrorTargets.For(IGraphicDisplayController) maps Latch to WriteData: one packet is one frame.
        var buffer = new Max7219ChainBuffer(2);
        using var controller = new Max7219Controller(buffer);
        using var receiver = new DisplayMirrorReceiver();
        receiver.Register(5, DisplayMirrorTargets.For(controller));

        receiver.Feed(DisplayMirrorProtocol.Encode(5, DisplayMirrorMessageType.Latch, [0x01, 0x12, 0x01, 0x34]));
        receiver.Feed(DisplayMirrorProtocol.Encode(5, DisplayMirrorMessageType.Latch, [0x0C, 0x01, 0x0C, 0x01]));

        Assert.Equal(0x34, buffer.GetDigit(0, 0));
        Assert.Equal(0x12, buffer.GetDigit(1, 0));
        Assert.False(buffer.IsShutdown(0));
        Assert.False(buffer.IsShutdown(1));
    }

    // A LedControl spiTransfer: a full-length frame with [opcode, data] for device addr and 0x00 0x00 for the others, device N-1's
    // pair first.
    private static void LedControlTransfer(
        Max7219Controller controller,
        int deviceCount,
        int addr,
        byte opcode,
        byte data)
    {
        var frame = new byte[2 * deviceCount];
        var offset = (deviceCount - 1 - addr) * 2;
        frame[offset] = opcode;
        frame[offset + 1] = data;
        controller.WriteData(frame);
    }

    private static void SendToAll(
        Max7219Controller controller,
        int deviceCount,
        byte opcode,
        byte data)
    {
        var frame = new byte[2 * deviceCount];
        for (var device = 0; device < deviceCount; device++)
        {
            frame[device * 2] = opcode;
            frame[(device * 2) + 1] = data;
        }

        controller.WriteData(frame);
    }
}