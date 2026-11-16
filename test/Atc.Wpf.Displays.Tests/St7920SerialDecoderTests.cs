namespace Atc.Wpf.Displays.Tests;

[Collection(nameof(TraceTestGroup))]
public sealed class St7920SerialDecoderTests
{
    [Fact]
    public void Constructor_NullController_Throws()
        => Assert.Throws<ArgumentNullException>(() => new St7920SerialDecoder(null!));

    [Fact]
    public void SerialDecoder_SyncAndNibbles_DecodeToInstructions()
    {
        // ref: §ST7920.6: 0xF8 = instruction start byte, 0xFA = data start byte; each byte v follows as (v & 0xF0), (v << 4) & 0xF0.
        // Examples from the note: instruction 0x38 -> F8 30 80, data 0x41 -> FA 40 10. One start byte per pair (ESPHome, §LIB.ESPHOMEST7920).
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        var decoder = new St7920SerialDecoder(controller);

        decoder.Write([0xF8, 0x30, 0x80, 0xF8, 0x00, 0x10, 0xF8, 0x00, 0xC0]);
        Assert.True(buffer.IsDisplayOn);

        decoder.Write([0xF8, 0x80, 0x10, 0xFA, 0x40, 0x10, 0xFA, 0x40, 0x20]);

        // 0x81 = DDRAM word 1: 'A' at column 16, 'B' at column 24 (§ST7920.4).
        St7920ControllerTests.AssertGlyph(buffer, 16, 0, (byte)'A');
        St7920ControllerTests.AssertGlyph(buffer, 24, 0, (byte)'B');
        Assert.DoesNotContain(St7920ControllerTests.LitPixels(buffer), p => p.X < 16);
    }

    [Fact]
    public void SerialDecoder_PairSplitAcrossWrites_IsJoined()
    {
        // ref: §ST7920.6: the framing state carries over from one Write to the next.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        var decoder = new St7920SerialDecoder(controller);

        decoder.Write([0xF8, 0x00]);
        decoder.Write([0xC0]);

        Assert.True(buffer.IsDisplayOn);
    }

    [Fact]
    public void SerialDecoder_ResyncsAfterGarbage()
    {
        // ref: §ST7920.6: bytes before the first start byte are dropped; a start byte (0b11111xx0) clears a pending high nibble;
        // a start byte with RW = 1 (0xFC) makes the decoder ignore the bytes up to the next start byte.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        var decoder = new St7920SerialDecoder(controller);

        // Dropped before any start byte: would be data 0x41 or instruction 0x01.
        decoder.Write([0x40, 0x10, 0x00, 0x10]);

        // Clear, display on, DDRAM address 0x81.
        decoder.Write([0xF8, 0x00, 0x10, 0xF8, 0x00, 0xC0, 0xF8, 0x80, 0x10]);

        // A truncated pair (would be 0x80 with a second 0x00), then a data start byte: the pending nibble is dropped.
        decoder.Write([0xF8, 0x80, 0xFA, 0x40, 0x10, 0x40, 0x10]);

        St7920ControllerTests.AssertGlyph(buffer, 16, 0, (byte)'A');
        St7920ControllerTests.AssertGlyph(buffer, 24, 0, (byte)'A');
        Assert.DoesNotContain(St7920ControllerTests.LitPixels(buffer), p => p.X < 16);

        // Read form start byte: 00 80 (display off) is ignored; the next instruction start byte resyncs and the same pair applies.
        decoder.Write([0xFC, 0x00, 0x80]);
        Assert.True(buffer.IsDisplayOn);

        decoder.Write([0xF8, 0x00, 0x80]);
        Assert.False(buffer.IsDisplayOn);
    }

    [Fact]
    public void SerialDecoder_ResetFraming_DropsThePendingState()
    {
        // ref: §ST7920.6: ResetFraming forgets the start byte and any pending high nibble.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        var decoder = new St7920SerialDecoder(controller);

        decoder.Write([0xF8, 0x00]);
        decoder.ResetFraming();
        decoder.Write([0xC0, 0x00, 0xC0]);

        Assert.False(buffer.IsDisplayOn);
    }

    [Fact]
    public void U8g2St7920Replay_ShowsTheScene()
    {
        // ref: §LIB.U8G2ST7920: the begin() of U8g2 (init, a blank frame for clearDisplay, power save off 0x38 0x0C) then one
        // sendBuffer of the scene. Each tile row is sent as inst 3E, inst 3E, then per pixel row: inst 80|y, inst 80|x and one
        // start byte FA followed by the pairs of 16 data bytes (one start byte for many pairs: ⚠️ derived, §ST7920.6).
        // The bottom half goes to GDRAM rows 0..31, words 8..15 (the ⚠️ derived fold, §ST7920.5), so the glass shows the host picture.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        var decoder = new St7920SerialDecoder(controller);

        var stream = new List<byte>();
        foreach (var instruction in new byte[] { 0x38, 0x08, 0x06, 0x02, 0x01 })
        {
            AddInstruction(stream, instruction);
        }

        AddFrame(stream, new HashSet<(int X, int Y)>());
        AddInstruction(stream, 0x38);
        AddInstruction(stream, 0x0C);
        decoder.Write(stream.ToArray());

        Assert.True(buffer.IsDisplayOn);
        Assert.Equal(0, St7920ControllerTests.CountLit(buffer));

        // The scene: (0, 0), (5, 40), (127, 63) and the whole row 20.
        var scene = new HashSet<(int X, int Y)> { (0, 0), (5, 40), (127, 63) };
        for (var x = 0; x < 128; x++)
        {
            scene.Add((x, 20));
        }

        stream.Clear();
        AddFrame(stream, scene);
        decoder.Write(stream.ToArray());

        var lit = St7920ControllerTests.LitPixels(buffer).ToList();
        Assert.Equal(128 + 3, lit.Count);
        Assert.Contains((0, 0), lit);
        Assert.Contains((5, 40), lit);
        Assert.Contains((127, 63), lit);
        Assert.Equal(128, lit.Count(p => p.Y == 20));
    }

    private static void AddInstruction(
        List<byte> stream,
        byte value)
    {
        stream.Add(0xF8);
        stream.Add((byte)(value & 0xF0));
        stream.Add((byte)((value << 4) & 0xF0));
    }

    private static void AddFrame(
        List<byte> stream,
        IReadOnlySet<(int X, int Y)> picture)
    {
        for (var tileRow = 0; tileRow < 8; tileRow++)
        {
            var y = tileRow * 8;
            var x = 0;
            if (y >= 32)
            {
                y -= 32;
                x += 8;
            }

            AddInstruction(stream, 0x3E);
            AddInstruction(stream, 0x3E);
            for (var i = 0; i < 8; i++)
            {
                AddInstruction(stream, (byte)(0x80 | (y + i)));
                AddInstruction(stream, (byte)(0x80 | x));
                stream.Add(0xFA);
                var hostRow = (tileRow * 8) + i;
                for (var column = 0; column < 16; column++)
                {
                    var value = 0;
                    for (var bit = 0; bit < 8; bit++)
                    {
                        if (picture.Contains(((column * 8) + bit, hostRow)))
                        {
                            value |= 0x80 >> bit;
                        }
                    }

                    stream.Add((byte)(value & 0xF0));
                    stream.Add((byte)((value << 4) & 0xF0));
                }
            }
        }
    }
}