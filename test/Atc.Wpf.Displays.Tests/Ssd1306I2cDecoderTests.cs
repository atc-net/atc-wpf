namespace Atc.Wpf.Displays.Tests;

public sealed class Ssd1306I2cDecoderTests
{
    private enum Kind
    {
        Command,
        Commands,
        Data,
        Reset,
    }

    [Fact]
    public void Constructor_NullController_Throws()
        => Assert.Throws<ArgumentNullException>(() => new Ssd1306I2cDecoder(null!));

    [Fact]
    public void ControlByte00_StreamsCommands()
    {
        // ref: §SSD1306.10 control byte 0x00: Co = 0, D/C# = 0, every following byte is a command.
        using var recorder = new RecordingController();
        var decoder = new Ssd1306I2cDecoder(recorder);

        decoder.Write([0x00, 0xAE, 0xD5, 0x80]);

        Assert.Equal(
            [(Kind.Commands, new byte[] { 0xAE, 0xD5, 0x80 })],
            recorder.Calls);
    }

    [Fact]
    public void ControlByte40_StreamsData()
    {
        // ref: §SSD1306.10 control byte 0x40: Co = 0, D/C# = 1, one WriteData with all bytes.
        using var recorder = new RecordingController();
        var decoder = new Ssd1306I2cDecoder(recorder);

        decoder.Write([0x40, 0x01, 0x02, 0x03, 0x04]);

        Assert.Equal(
            [(Kind.Data, new byte[] { 0x01, 0x02, 0x03, 0x04 })],
            recorder.Calls);
    }

    [Fact]
    public void ControlByte80_SingleCommandThenControlByte()
    {
        // ref: §SSD1306.10 control byte 0x80: Co = 1, one command byte, then the next control byte.
        using var recorder = new RecordingController();
        var decoder = new Ssd1306I2cDecoder(recorder);

        decoder.Write([0x80, 0xAE, 0x80, 0xAF]);

        Assert.Equal(
            [(Kind.Commands, new byte[] { 0xAE, 0xAF })],
            recorder.Calls);
    }

    [Fact]
    public void ControlByteC0_SingleDataThenControlByte()
    {
        // ref: §SSD1306.10 control byte 0xC0: Co = 1, one data byte, then the next control byte.
        using var recorder = new RecordingController();
        var decoder = new Ssd1306I2cDecoder(recorder);

        decoder.Write([0xC0, 0x11, 0xC0, 0x22]);

        Assert.Equal(
            [(Kind.Data, new byte[] { 0x11, 0x22 })],
            recorder.Calls);
    }

    [Fact]
    public void MixedTransaction_RoutesEachPart()
    {
        // ref: §SSD1306.10: 80 21 80 00 80 7F 40 FF FF is the command run 21 00 7F, then data FF FF.
        using var recorder = new RecordingController();
        var decoder = new Ssd1306I2cDecoder(recorder);

        decoder.Write([0x80, 0x21, 0x80, 0x00, 0x80, 0x7F, 0x40, 0xFF, 0xFF]);

        Assert.Equal(
            [
                (Kind.Commands, new byte[] { 0x21, 0x00, 0x7F }),
                (Kind.Data, new byte[] { 0xFF, 0xFF }),
            ],
            recorder.Calls);
    }

    [Fact]
    public void MixedTransaction_DataBetweenCommands_KeepsTheOrder()
    {
        using var recorder = new RecordingController();
        var decoder = new Ssd1306I2cDecoder(recorder);

        decoder.Write([0x80, 0xB0, 0xC0, 0x55, 0x80, 0xB1]);

        Assert.Equal(
            [
                (Kind.Commands, new byte[] { 0xB0 }),
                (Kind.Data, new byte[] { 0x55 }),
                (Kind.Commands, new byte[] { 0xB1 }),
            ],
            recorder.Calls);
    }

    [Fact]
    public void EmptyOrTrailingControlByte_IsIgnored()
    {
        // ref: §SSD1306.10: a control byte without the byte it announces carries nothing.
        using var recorder = new RecordingController();
        var decoder = new Ssd1306I2cDecoder(recorder);

        decoder.Write([]);
        decoder.Write([0x80]);
        decoder.Write([0xC0]);
        decoder.Write([0x00]);
        decoder.Write([0x40]);
        decoder.Write([0x80, 0xAE, 0x80]);

        Assert.Equal(
            [(Kind.Commands, new byte[] { 0xAE })],
            recorder.Calls);
    }

    [Fact]
    public void Write_HoldsNoStateBetweenTransactions()
    {
        // ref: §SSD1306.10: every transaction starts with a control byte.
        using var recorder = new RecordingController();
        var decoder = new Ssd1306I2cDecoder(recorder);

        decoder.Write([0x40, 0x01]);
        decoder.Write([0x01, 0x02]);

        Assert.Equal(
            [
                (Kind.Data, new byte[] { 0x01 }),
                (Kind.Commands, new byte[] { 0x02 }),
            ],
            recorder.Calls);
    }

    [Fact]
    public void AdafruitDisplay_ThroughTheDecoder_LightsTheExpectedPixels()
    {
        // ref: §LIB.A1306.1 (128x64 init) and §LIB.A1306.2 (display(): window commands with control byte 0x00,
        // then 0x40 + 32-byte data chunks) into a real Ssd1306Controller.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        var decoder = new Ssd1306I2cDecoder(controller);

        decoder.Write([0x00, 0xAE, 0xD5, 0x80, 0xA8]);
        decoder.Write([0x00, 0x3F]);
        decoder.Write([0x00, 0xD3, 0x00, 0x40, 0x8D]);
        decoder.Write([0x00, 0x14]);
        decoder.Write([0x00, 0x20, 0x00, 0xA1, 0xC8]);
        decoder.Write([0x00, 0xDA]);
        decoder.Write([0x00, 0x12]);
        decoder.Write([0x00, 0x81]);
        decoder.Write([0x00, 0xCF]);
        decoder.Write([0x00, 0xD9]);
        decoder.Write([0x00, 0xF1]);
        decoder.Write([0x00, 0xDB, 0x40, 0xA4, 0xA6, 0x2E, 0xAF]);

        Assert.True(buffer.IsDisplayOn);

        var data = new byte[1024];
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = (byte)((i * 13) + 5);
        }

        decoder.Write([0x00, 0x22, 0x00, 0xFF, 0x21]);
        decoder.Write([0x00, 0x00]);
        decoder.Write([0x00, 0x7F]);
        for (var offset = 0; offset < data.Length; offset += 32)
        {
            var transfer = new byte[33];
            transfer[0] = 0x40;
            Array.Copy(data, offset, transfer, 1, 32);
            decoder.Write(transfer);
        }

        // With A1 + C8 the RAM shows upright, so the frame equals the streamed bytes (page-major, bit 0 = top row).
        Assert.Equal(data, buffer.GetFrame());
    }

    private readonly record struct Call(Kind Kind, byte[] Bytes)
    {
        public static implicit operator Call((Kind Kind, byte[] Bytes) value)
            => new(value.Kind, value.Bytes);

        public bool Equals(Call other)
            => Kind == other.Kind && Bytes.AsSpan().SequenceEqual(other.Bytes);

        public override int GetHashCode()
            => HashCode.Combine(Kind, Bytes.Length);
    }

    private sealed class RecordingController : IGraphicDisplayController
    {
        public List<Call> Calls { get; } = [];

        public void WriteCommand(byte command)
            => Calls.Add((Kind.Command, [command]));

        public void WriteCommands(ReadOnlySpan<byte> commands)
            => Calls.Add((Kind.Commands, commands.ToArray()));

        public void WriteData(ReadOnlySpan<byte> data)
            => Calls.Add((Kind.Data, data.ToArray()));

        public void Reset()
            => Calls.Add((Kind.Reset, []));

        public void Dispose()
        {
            // Nothing to release.
        }
    }
}