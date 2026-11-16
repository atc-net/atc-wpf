namespace Atc.Wpf.Displays.Tests;

public sealed class DisplayMirrorProtocolTests
{
    private sealed record Vector(
        string Name,
        byte DisplayId,
        DisplayMirrorMessageType Type,
        byte Flags,
        byte[] Payload,
        byte[] Packet);

    private sealed record Reject(
        string Name,
        byte[] Packet,
        DisplayMirrorRejectReason Reason);

    [Fact]
    public void Crc_CheckValue_Is29B1()
        => Assert.Equal(
            0x29B1,
            DisplayMirrorProtocol.ComputeCrc(Encoding.ASCII.GetBytes("123456789")));

    [Fact]
    public void Crc_EmptyInput_IsTheInitialValue()
        => Assert.Equal(0xFFFF, DisplayMirrorProtocol.ComputeCrc([]));

    [Fact]
    public void Encode_MatchesEveryGoldenVector()
    {
        foreach (var v in LoadVectors())
        {
            var allocated = DisplayMirrorProtocol.Encode(v.DisplayId, v.Type, v.Payload, v.Flags);
            Assert.True(v.Packet.AsSpan().SequenceEqual(allocated), v.Name);

            var destination = new byte[v.Packet.Length + 3];
            var written = DisplayMirrorProtocol.Encode(v.DisplayId, v.Type, v.Payload, destination, v.Flags);
            Assert.Equal(v.Packet.Length, written);
            Assert.True(v.Packet.AsSpan().SequenceEqual(destination.AsSpan(0, written)), v.Name);
        }
    }

    [Fact]
    public void Encode_DestinationTooSmall_Throws()
    {
        var destination = new byte[11];
        Assert.Throws<ArgumentException>(
            () => DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Reset, [], destination));
    }

    [Fact]
    public void TryDecode_RoundTripsEveryGoldenVector()
    {
        foreach (var v in LoadVectors())
        {
            var reason = DisplayMirrorProtocol.TryDecode(v.Packet, out var packet);

            Assert.True(reason == DisplayMirrorRejectReason.None, v.Name);
            Assert.Equal(v.DisplayId, packet.DisplayId);
            Assert.Equal(v.Type, packet.Type);
            Assert.Equal(v.Flags, packet.Flags);
            Assert.True(v.Payload.AsSpan().SequenceEqual(packet.Payload.Span), v.Name);
        }
    }

    [Fact]
    public void TryDecode_GoldenRejects_ReturnTheirReason()
    {
        var rejects = LoadRejects();
        Assert.NotEmpty(rejects);

        foreach (var r in rejects)
        {
            var reason = DisplayMirrorProtocol.TryDecode(r.Packet, out var packet);

            Assert.True(r.Reason == reason, $"{r.Name}: expected {r.Reason} but got {reason}");
            Assert.Equal(default, packet);
        }
    }

    [Fact]
    public void TryDecode_PayloadIsACopy_SurvivesTheCallersBuffer()
    {
        var buffer = DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [1, 2, 3]);

        Assert.Equal(DisplayMirrorRejectReason.None, DisplayMirrorProtocol.TryDecode(buffer, out var packet));
        Array.Clear(buffer);

        Assert.Equal(new byte[] { 1, 2, 3 }, packet.Payload.ToArray());
    }

    [Fact]
    public void TryDecode_UnknownFlagBits_AreKept()
    {
        var bytes = DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Reset, [], flags: 0xFF);

        Assert.Equal(DisplayMirrorRejectReason.None, DisplayMirrorProtocol.TryDecode(bytes, out var packet));
        Assert.Equal(0xFF, packet.Flags);
    }

    [Fact]
    public void TryDecode_CapIsCheckedBeforeLength()
    {
        // A Data header declaring 65,537 bytes is noise even though no body follows (Truncated would be the other answer).
        // Encode refuses a payload over the cap, so the header is built by hand: length 0x00010001 little-endian, then 2 CRC-sized bytes.
        byte[] headerOnly = [0xA7, 0xC0, 0x01, 0x00, (byte)DisplayMirrorMessageType.Data, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00];

        Assert.Equal(DisplayMirrorRejectReason.TooLarge, DisplayMirrorProtocol.TryDecode(headerOnly, out _));
    }

    [Fact]
    public void TryDecode_FrameOverTheTrafficCap_IsAccepted()
    {
        var bytes = DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.MonoFrame, new byte[65_537]);

        Assert.Equal(DisplayMirrorRejectReason.None, DisplayMirrorProtocol.TryDecode(bytes, out _));
    }

    [Theory]
    [InlineData(DisplayMirrorMessageType.Data, 65_537)]
    [InlineData(DisplayMirrorMessageType.Commands, 65_537)]
    [InlineData(DisplayMirrorMessageType.MonoFrame, 4_194_305)]
    [InlineData(DisplayMirrorMessageType.EPaperFrame, 4_194_305)]
    public void Encode_PayloadOverTheCapOfItsType_Throws(
        DisplayMirrorMessageType type,
        int length)
    {
        var payload = new byte[length];
        var destination = new byte[length + 12];

        var array = Assert.Throws<ArgumentException>(() => DisplayMirrorProtocol.Encode(0, type, payload));
        var span = Assert.Throws<ArgumentException>(() => DisplayMirrorProtocol.Encode(0, type, payload, destination));

        Assert.Equal("payload", array.ParamName);
        Assert.Equal("payload", span.ParamName);
        Assert.Contains((length - 1).ToString(CultureInfo.InvariantCulture), array.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Encode_FrameAtTheFrameCap_Succeeds()
    {
        var bytes = DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.ColorFrame, new byte[4_194_304]);

        Assert.Equal(4_194_304 + 12, bytes.Length);
    }

    [Fact]
    public void TryDecode_TrafficAtTheCap_IsAccepted()
    {
        var bytes = DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, new byte[65_536]);

        Assert.Equal(DisplayMirrorRejectReason.None, DisplayMirrorProtocol.TryDecode(bytes, out _));
    }

    [Fact]
    public void IsFrameType_OnlyFrames()
    {
        Assert.True(DisplayMirrorProtocol.IsFrameType(DisplayMirrorMessageType.MonoFrame));
        Assert.True(DisplayMirrorProtocol.IsFrameType(DisplayMirrorMessageType.ColorFrame));
        Assert.True(DisplayMirrorProtocol.IsFrameType(DisplayMirrorMessageType.EPaperFrame));
        Assert.False(DisplayMirrorProtocol.IsFrameType(DisplayMirrorMessageType.Describe));
        Assert.False(DisplayMirrorProtocol.IsFrameType(DisplayMirrorMessageType.Commands));
        Assert.False(DisplayMirrorProtocol.IsFrameType(DisplayMirrorMessageType.Latch));
    }

    [Fact]
    public void Describe_RoundTrips()
    {
        var describe = new DisplayMirrorDescribe(
            DisplayMirrorDisplayKind.GraphicLcd,
            DisplayMirrorChip.St7920,
            128,
            64,
            "ÆØÅ");

        var payload = describe.ToPayload();

        Assert.Equal(
            new byte[] { 0x07, 0x50, 0x80, 0x00, 0x40, 0x00, 0xC3, 0x86, 0xC3, 0x98, 0xC3, 0x85 },
            payload);
        Assert.True(DisplayMirrorDescribe.TryParse(payload, out var parsed));
        Assert.Equal(describe, parsed);
    }

    [Fact]
    public void Describe_TryParse_ShortPayload_Fails()
    {
        Assert.False(DisplayMirrorDescribe.TryParse(new byte[5], out var parsed));
        Assert.Null(parsed);
    }

    [Fact]
    public void Describe_TryParse_UnknownKindChipAndInvalidUtf8_StillParses()
    {
        byte[] payload = [0xEE, 0xDD, 0x01, 0x01, 0x02, 0x00, 0x41, 0xFF, 0x42];

        Assert.True(DisplayMirrorDescribe.TryParse(payload, out var parsed));
        Assert.NotNull(parsed);
        Assert.Equal(0xEE, (byte)parsed.Kind);
        Assert.Equal(0xDD, (byte)parsed.Chip);
        Assert.Equal(257, parsed.Width);
        Assert.Equal(2, parsed.Height);
        Assert.Equal("A�B", parsed.Name);
    }

    [Fact]
    public void Describe_ToPayload_InvalidValues_Throw()
    {
        var tooWide = new DisplayMirrorDescribe(DisplayMirrorDisplayKind.MonoOled, DisplayMirrorChip.Ssd1306, 65_536, 1, "x");
        var negative = new DisplayMirrorDescribe(DisplayMirrorDisplayKind.MonoOled, DisplayMirrorChip.Ssd1306, 1, -1, "x");
        var tooLong = new DisplayMirrorDescribe(DisplayMirrorDisplayKind.MonoOled, DisplayMirrorChip.Ssd1306, 1, 1, new string('x', 65_536));

        Assert.Throws<ArgumentOutOfRangeException>(() => tooWide.ToPayload());
        Assert.Throws<ArgumentOutOfRangeException>(() => negative.ToPayload());
        Assert.Throws<ArgumentException>(() => tooLong.ToPayload());
    }

    [Fact]
    public void FramePayloadHelpers_WriteDimensionsLittleEndian()
    {
        // 300 = 0x012C, 258 = 0x0102.
        var mono = DisplayMirrorProtocol.CreateMonoFramePayload(300, 8, new byte[300]);
        Assert.Equal(new byte[] { 0x2C, 0x01, 0x08, 0x00 }, mono[..4]);
        Assert.Equal(304, mono.Length);

        var color = DisplayMirrorProtocol.CreateColorFramePayload(258, 1, new byte[516]);
        Assert.Equal(new byte[] { 0x02, 0x01, 0x01, 0x00 }, color[..4]);
        Assert.Equal(520, color.Length);

        var twoPlanes = DisplayMirrorProtocol.CreateEPaperFramePayload(8, 2, [0xF0, 0x0F], [0x01, 0x80], isPartial: true);
        Assert.Equal(Convert.FromHexString("0800020002F00F018001"), twoPlanes);

        var onePlane = DisplayMirrorProtocol.CreateEPaperFramePayload(8, 2, [0xF0, 0x0F], [], isPartial: false);
        Assert.Equal(Convert.FromHexString("0800020001F00F00"), onePlane);

        // 10 pixels per row take 2 bytes (rows padded to whole bytes).
        var padded = DisplayMirrorProtocol.CreateEPaperFramePayload(10, 3, new byte[6], [], isPartial: false);
        Assert.Equal(4 + 1 + 6 + 1, padded.Length);
    }

    [Fact]
    public void FramePayloadHelpers_MatchTheGoldenFrameVectors()
    {
        var vectors = LoadVectors().ToDictionary(v => v.Name, StringComparer.Ordinal);

        Assert.Equal(
            vectors["mono-frame-8x8"].Payload,
            DisplayMirrorProtocol.CreateMonoFramePayload(8, 8, Convert.FromHexString("ff818181818181ff")));
        Assert.Equal(
            vectors["color-frame-2x2"].Payload,
            DisplayMirrorProtocol.CreateColorFramePayload(2, 2, Convert.FromHexString("f80007e0001fffff")));
    }

    [Fact]
    public void FramePayloadHelpers_Misuse_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DisplayMirrorProtocol.CreateMonoFramePayload(0, 8, []));
        Assert.Throws<ArgumentOutOfRangeException>(() => DisplayMirrorProtocol.CreateMonoFramePayload(8, 65_536, []));
        Assert.Throws<ArgumentOutOfRangeException>(() => DisplayMirrorProtocol.CreateColorFramePayload(1, 0, []));

        // Wrong data lengths: mono needs width x ceil(height / 8), colour width x height x 2, e-paper ceil(width / 8) x height.
        Assert.Throws<ArgumentException>(() => DisplayMirrorProtocol.CreateMonoFramePayload(8, 9, new byte[8]));
        Assert.Throws<ArgumentException>(() => DisplayMirrorProtocol.CreateColorFramePayload(2, 2, new byte[7]));
        Assert.Throws<ArgumentException>(() => DisplayMirrorProtocol.CreateEPaperFramePayload(8, 2, new byte[3], [], isPartial: false));
        Assert.Throws<ArgumentException>(() => DisplayMirrorProtocol.CreateEPaperFramePayload(8, 2, new byte[2], new byte[1], isPartial: false));
    }

    private static string FindVectorsPath()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "firmware", "protocol", "test-vectors.json");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            dir = dir.Parent;
        }

        throw new FileNotFoundException("firmware/protocol/test-vectors.json was not found above " + AppContext.BaseDirectory);
    }

    private static JsonElement LoadRoot()
    {
        using var document = JsonDocument.Parse(File.ReadAllText(FindVectorsPath()));
        return document.RootElement.Clone();
    }

    private static List<Vector> LoadVectors()
    {
        var result = new List<Vector>();
        foreach (var e in LoadRoot().GetProperty("vectors").EnumerateArray())
        {
            result.Add(
                new Vector(
                    e.GetProperty("name").GetString()!,
                    e.GetProperty("displayId").GetByte(),
                    (DisplayMirrorMessageType)e.GetProperty("type").GetByte(),
                    e.GetProperty("flags").GetByte(),
                    Convert.FromHexString(e.GetProperty("payloadHex").GetString()!),
                    Convert.FromHexString(e.GetProperty("packetHex").GetString()!)));
        }

        Assert.NotEmpty(result);
        return result;
    }

    private static List<Reject> LoadRejects()
    {
        var result = new List<Reject>();
        foreach (var e in LoadRoot().GetProperty("rejects").EnumerateArray())
        {
            result.Add(
                new Reject(
                    e.GetProperty("name").GetString()!,
                    Convert.FromHexString(e.GetProperty("packetHex").GetString()!),
                    Enum.Parse<DisplayMirrorRejectReason>(e.GetProperty("reason").GetString()!)));
        }

        return result;
    }
}