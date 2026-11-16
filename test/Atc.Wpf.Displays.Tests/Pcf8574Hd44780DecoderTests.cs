namespace Atc.Wpf.Displays.Tests;

public sealed class Pcf8574Hd44780DecoderTests
{
    [Fact]
    public void FourBitInitSequence_EntersFourBitMode()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        var controller = new Hd44780Controller(buffer);
        var decoder = new Pcf8574Hd44780Decoder(controller);

        WriteNibble(decoder, 0x3, rs: false);
        WriteNibble(decoder, 0x3, rs: false);
        WriteNibble(decoder, 0x3, rs: false);
        WriteNibble(decoder, 0x2, rs: false);
        WriteNibble(decoder, 0x2, rs: false);
        WriteNibble(decoder, 0x8, rs: false);

        Assert.True(controller.IsTwoLineMode);
    }

    [Fact]
    public void NibblePairs_WithRsHigh_WriteData()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        var decoder = new Pcf8574Hd44780Decoder(new Hd44780Controller(buffer));
        InitFourBit(decoder);

        WriteByte(decoder, (byte)'A', rs: true);

        Assert.Equal((byte)'A', buffer.GetCode(0, 0));
    }

    [Fact]
    public void NibblePairs_WithRsLow_AreCommands()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        var decoder = new Pcf8574Hd44780Decoder(new Hd44780Controller(buffer));
        InitFourBit(decoder);
        WriteByte(decoder, (byte)'A', rs: true);

        WriteNibble(decoder, 0x0, rs: false);
        WriteNibble(decoder, 0x1, rs: false);

        Assert.Equal((byte)' ', buffer.GetCode(0, 0));
        Assert.Equal(0, buffer.CursorAddress);
    }

    [Fact]
    public void OnlyTheFallingEdgeOfEnable_LatchesANibble()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        var decoder = new Pcf8574Hd44780Decoder(new Hd44780Controller(buffer));
        InitFourBit(decoder);

        // High nibble 0x4 with EN high repeated three times, then EN low: one latch.
        decoder.Write(Bytes(0x4, rs: true, enable: false));
        decoder.Write(Bytes(0x4, rs: true, enable: true));
        decoder.Write(Bytes(0x4, rs: true, enable: true));
        decoder.Write(Bytes(0x4, rs: true, enable: true));
        decoder.Write(Bytes(0x4, rs: true, enable: false));
        WriteNibble(decoder, 0x1, rs: true);

        Assert.Equal((byte)'A', buffer.GetCode(0, 0));
        Assert.Equal((byte)' ', buffer.GetCode(0, 1));
    }

    [Fact]
    public void BacklightBit_IsReported()
    {
        var decoder = new Pcf8574Hd44780Decoder(new Hd44780Controller(new CharacterDisplayBuffer(16, 2)));
        var raised = 0;
        decoder.BacklightChanged += (_, _) => raised++;
        Assert.True(decoder.IsBacklightOn);

        decoder.Write(0x00);
        decoder.Write(0x00);
        decoder.Write(0x00);

        Assert.False(decoder.IsBacklightOn);
        Assert.Equal(1, raised);

        decoder.Write(0x08);

        Assert.True(decoder.IsBacklightOn);
        Assert.Equal(2, raised);
    }

    [Fact]
    public void ReadCycles_AreIgnored()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        var decoder = new Pcf8574Hd44780Decoder(new Hd44780Controller(buffer));
        InitFourBit(decoder);

        // RW high (0x02): EN pulses with a data nibble must not latch.
        decoder.Write(
        [
            (byte)(0x40 | 0x08 | 0x02),
            (byte)(0x40 | 0x08 | 0x02 | 0x04),
            (byte)(0x40 | 0x08 | 0x02),
        ]);
        decoder.Write(
        [
            (byte)(0x10 | 0x08 | 0x02),
            (byte)(0x10 | 0x08 | 0x02 | 0x04),
            (byte)(0x10 | 0x08 | 0x02),
        ]);

        Assert.Equal((byte)' ', buffer.GetCode(0, 0));

        // A real write after the read cycles still pairs from a clean state.
        WriteByte(decoder, (byte)'B', rs: true);

        Assert.Equal((byte)'B', buffer.GetCode(0, 0));
    }

    [Fact]
    public void CustomPinMap_IsHonoured()
    {
        var pinMap = new Pcf8574PinMap
        {
            Rs = 4,
            Rw = 5,
            Enable = 6,
            Backlight = 7,
            Data4 = 0,
        };
        var buffer = new CharacterDisplayBuffer(16, 2);
        var decoder = new Pcf8574Hd44780Decoder(new Hd44780Controller(buffer), pinMap);

        foreach (var nibble in new byte[] { 0x3, 0x3, 0x3, 0x2, 0x2, 0x8 })
        {
            decoder.Write(CustomPulse(nibble, rs: false));
        }

        decoder.Write(CustomPulse(0x4, rs: true));
        decoder.Write(CustomPulse(0x1, rs: true));

        Assert.Equal((byte)'A', buffer.GetCode(0, 0));

        static byte[] CustomPulse(
            byte nibble,
            bool rs)
        {
            var value = nibble | 0x80 | (rs ? 0x10 : 0);
            return [(byte)value, (byte)(value | 0x40), (byte)value];
        }
    }

    [Fact]
    public void PinMap_WithDuplicateBitPositions_Throws()
    {
        var controller = new Hd44780Controller(new CharacterDisplayBuffer(16, 2));

        // Rs = 4 collides with D4 at the default Data4 = 4.
        var pinMap = new Pcf8574PinMap { Rs = 4 };

        Assert.Throws<ArgumentException>(() => new Pcf8574Hd44780Decoder(controller, pinMap));
    }

    [Fact]
    public void PinMap_WithBitPositionOutOfRange_Throws()
    {
        var controller = new Hd44780Controller(new CharacterDisplayBuffer(16, 2));
        var pinMap = new Pcf8574PinMap
        {
            Rs = 8,
            Rw = 5,
            Enable = 6,
            Backlight = 7,
            Data4 = 0,
        };

        Assert.Throws<ArgumentException>(() => new Pcf8574Hd44780Decoder(controller, pinMap));
    }

    [Fact]
    public void PinMap_WithDataNibbleBeyondBitSeven_Throws()
    {
        var controller = new Hd44780Controller(new CharacterDisplayBuffer(16, 2));
        var pinMap = new Pcf8574PinMap
        {
            Rs = 0,
            Rw = 1,
            Enable = 2,
            Backlight = 3,
            Data4 = 5,
        };

        Assert.Throws<ArgumentException>(() => new Pcf8574Hd44780Decoder(controller, pinMap));
    }

    [Fact]
    public void FunctionSetWithEightBitFlag_InFourBitMode_ReturnsToEightBitHandling()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        var controller = new Hd44780Controller(buffer);
        var decoder = new Pcf8574Hd44780Decoder(controller);
        InitFourBit(decoder);

        // 0x33 sent as one 4-bit pair puts the chip back in 8-bit mode; 0x3, 0x2 then re-enter 4-bit mode.
        WriteNibble(decoder, 0x3, rs: false);
        WriteNibble(decoder, 0x3, rs: false);
        WriteNibble(decoder, 0x3, rs: false);
        WriteNibble(decoder, 0x2, rs: false);
        WriteNibble(decoder, 0x2, rs: false);
        WriteNibble(decoder, 0x8, rs: false);
        WriteByte(decoder, (byte)'Z', rs: true);

        Assert.True(controller.IsTwoLineMode);
        Assert.Equal((byte)'Z', buffer.GetCode(0, 0));
    }

    [Fact]
    public void AnyByteValue_DoesNotThrow()
    {
        var decoder = new Pcf8574Hd44780Decoder(new Hd44780Controller(new CharacterDisplayBuffer(16, 2)));
        var all = new byte[256 * 3];
        for (var i = 0; i < all.Length; i++)
        {
            all[i] = (byte)((i * 37) + (i / 256));
        }

        var exception = Record.Exception(() => decoder.Write(all));

        Assert.Null(exception);
    }

    [Fact]
    public void LiquidCrystalI2cInitAndText_ShowsHiOnRowZero()
    {
        var buffer = new CharacterDisplayBuffer(16, 2);
        var decoder = new Pcf8574Hd44780Decoder(new Hd44780Controller(buffer));

        WriteNibble(decoder, 0x3, rs: false);
        WriteNibble(decoder, 0x3, rs: false);
        WriteNibble(decoder, 0x3, rs: false);
        WriteNibble(decoder, 0x2, rs: false);
        WriteByte(decoder, 0x28, rs: false);
        WriteByte(decoder, 0x0C, rs: false);
        WriteByte(decoder, 0x06, rs: false);
        WriteByte(decoder, 0x01, rs: false);
        WriteByte(decoder, (byte)'H', rs: true);
        WriteByte(decoder, (byte)'i', rs: true);

        var rows = buffer.GetVisibleText(CharacterRom.A00).Split('\n');
        Assert.Equal("Hi", rows[0].TrimEnd());
        Assert.True(decoder.IsBacklightOn);
    }

    private static byte[] Pulse(
        byte nibble,
        bool rs,
        bool backlight)
        =>
        [
            .. Bytes(nibble, rs, enable: false, backlight),
            .. Bytes(nibble, rs, enable: true, backlight),
            .. Bytes(nibble, rs, enable: false, backlight),
        ];

    private static byte[] Bytes(
        byte nibble,
        bool rs,
        bool enable,
        bool backlight = true)
    {
        var value = (nibble << 4) | (rs ? 0x01 : 0) | (enable ? 0x04 : 0) | (backlight ? 0x08 : 0);
        return [(byte)value];
    }

    private static void WriteNibble(
        Pcf8574Hd44780Decoder decoder,
        byte nibble,
        bool rs)
        => decoder.Write(Pulse(nibble, rs, backlight: true));

    private static void WriteByte(
        Pcf8574Hd44780Decoder decoder,
        byte value,
        bool rs)
    {
        WriteNibble(decoder, (byte)(value >> 4), rs);
        WriteNibble(decoder, (byte)(value & 0x0F), rs);
    }

    private static void InitFourBit(Pcf8574Hd44780Decoder decoder)
    {
        WriteNibble(decoder, 0x3, rs: false);
        WriteNibble(decoder, 0x3, rs: false);
        WriteNibble(decoder, 0x3, rs: false);
        WriteNibble(decoder, 0x2, rs: false);
        WriteByte(decoder, 0x28, rs: false);
    }
}