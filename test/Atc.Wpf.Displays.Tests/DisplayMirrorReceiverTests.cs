namespace Atc.Wpf.Displays.Tests;

public sealed class DisplayMirrorReceiverTests
{
    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(5);

    [Fact]
    public void Feed_OneChunk_AppliesThePacketToItsTarget()
    {
        using var receiver = new DisplayMirrorReceiver();
        var target = new RecordingTarget();
        receiver.Register(3, target);

        receiver.Feed(DisplayMirrorProtocol.Encode(3, DisplayMirrorMessageType.Data, [1, 2, 3], flags: 0x80));

        var applied = Assert.Single(target.Applied);
        Assert.Equal(3, applied.DisplayId);
        Assert.Equal(DisplayMirrorMessageType.Data, applied.Type);
        Assert.Equal(0x80, applied.Flags);
        Assert.Equal(new byte[] { 1, 2, 3 }, applied.Payload);
    }

    [Fact]
    public void Feed_SplitAtEveryBoundary_AppliesEachPacketOnce()
    {
        // The frames make splits land inside the width / height / plane-count prefix of a frame payload too.
        var monoPages = new byte[8];
        monoPages[0] = 0x01;
        byte[] stream =
        [
            .. DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Commands, [0xAE, 0xD5]),
            .. DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Reset, []),
            .. DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Data, [9, 8, 7, 6, 5]),
            .. DisplayMirrorProtocol.Encode(
                2,
                DisplayMirrorMessageType.MonoFrame,
                DisplayMirrorProtocol.CreateMonoFramePayload(8, 8, monoPages)),
            .. DisplayMirrorProtocol.Encode(
                3,
                DisplayMirrorMessageType.EPaperFrame,
                DisplayMirrorProtocol.CreateEPaperFramePayload(8, 2, [0x7F, 0xFF], [0x00, 0x80], isPartial: true)),
        ];

        for (var split = 0; split <= stream.Length; split++)
        {
            using var receiver = new DisplayMirrorReceiver();
            var target = new RecordingTarget();
            var mono = new MonochromeFrameBuffer(8, 8);
            var paper = new EPaperFrameBuffer(8, 2, hasRed: true);
            var paperRefreshes = 0;
            paper.Refreshed += (_, _) => paperRefreshes++;
            receiver.Register(1, target);
            receiver.Register(2, DisplayMirrorTargets.For(mono));
            receiver.Register(3, DisplayMirrorTargets.For(paper));

            receiver.Feed(stream.AsSpan(0, split));
            receiver.Feed(stream.AsSpan(split));

            Assert.Equal(3, target.Applied.Count);
            Assert.Equal(new byte[] { 0xAE, 0xD5 }, target.Applied[0].Payload);
            Assert.Equal(DisplayMirrorMessageType.Reset, target.Applied[1].Type);
            Assert.Equal(new byte[] { 9, 8, 7, 6, 5 }, target.Applied[2].Payload);
            Assert.True(mono.GetPixel(0, 0));
            Assert.False(mono.GetPixel(1, 0));
            Assert.Equal(1, receiver.GetStatistics(2).Packets);
            Assert.Equal(0, receiver.GetStatistics(2).Rejected);
            Assert.Equal(EPaperInk.Black, paper.GetShownPixel(0, 0));
            Assert.Equal(EPaperInk.Red, paper.GetShownPixel(0, 1));
            Assert.Equal(EPaperInk.White, paper.GetShownPixel(1, 0));
            Assert.Equal(1, paperRefreshes);
            Assert.Equal(0, receiver.GetStatistics(3).Rejected);
            Assert.Equal(0, receiver.GetStreamStatistics().Resyncs);
        }

        // One byte at a time.
        using var byteByByte = new DisplayMirrorReceiver();
        var recorder = new RecordingTarget();
        byteByByte.Register(1, recorder);
        foreach (var b in stream)
        {
            byteByByte.Feed([b]);
        }

        Assert.Equal(3, recorder.Applied.Count);
    }

    [Fact]
    public void Feed_FramesLargerThanTheStreamBuffer_ArriveWhole()
    {
        using var receiver = new DisplayMirrorReceiver();
        var buffer = new ColorFrameBuffer(320, 240);
        receiver.Register(0, DisplayMirrorTargets.For(buffer));
        var pixels = new byte[320 * 240 * 2];
        pixels[0] = 0xF8;
        pixels[^2] = 0x00;
        pixels[^1] = 0x1F;
        var packet = DisplayMirrorProtocol.Encode(
            0,
            DisplayMirrorMessageType.ColorFrame,
            DisplayMirrorProtocol.CreateColorFramePayload(320, 240, pixels));

        for (var round = 0; round < 2; round++)
        {
            for (var offset = 0; offset < packet.Length; offset += 1000)
            {
                receiver.Feed(packet.AsSpan(offset, System.Math.Min(1000, packet.Length - offset)));
            }

            receiver.Feed(DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Reset, []));
        }

        Assert.Equal(2, receiver.GetStatistics(0).Packets);
        Assert.Equal(2, receiver.GetStatistics(0).Unsupported);
        Assert.Equal(Color.FromRgb(0xFF, 0x00, 0x00), buffer.GetPixel(0, 0));
        Assert.Equal(Color.FromRgb(0x00, 0x00, 0xFF), buffer.GetPixel(319, 239));
    }

    [Fact]
    public void Feed_NoiseWithFalseMagic_ResyncsToTheNextGoodPacket()
    {
        using var receiver = new DisplayMirrorReceiver();
        var target = new RecordingTarget();
        receiver.Register(0, target);
        var rejects = new List<DisplayMirrorRejectedEventArgs>();
        receiver.PacketRejected += (_, e) => rejects.Add(e);

        // Noise, a false magic followed by an unsupported version, a false magic with an unknown type, then a good packet.
        byte[] stream =
        [
            0x00, 0x13, 0xA7, 0x55,
            0xA7, 0xC0, 0x09, 0x00, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00,
            0xA7, 0xC0, 0x01, 0x00, 0x77, 0x00, 0x01, 0x00, 0x00, 0x00,
            .. DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [0x42]),
        ];

        receiver.Feed(stream);

        var applied = Assert.Single(target.Applied);
        Assert.Equal(new byte[] { 0x42 }, applied.Payload);
        Assert.Equal(
            [DisplayMirrorRejectReason.UnsupportedVersion, DisplayMirrorRejectReason.UnknownType],
            rejects.Select(r => r.Reason));
        Assert.All(rejects, r => Assert.Null(r.DisplayId));
        Assert.Equal(1, receiver.GetStreamStatistics().Resyncs);
        Assert.Equal(1, receiver.GetStatistics(0).Packets);
    }

    [Fact]
    public void Feed_LengthOverCap_IsNoiseAndResyncs()
    {
        using var receiver = new DisplayMirrorReceiver();
        var target = new RecordingTarget();
        receiver.Register(0, target);
        var rejects = new List<DisplayMirrorRejectReason>();
        receiver.PacketRejected += (_, e) => rejects.Add(e.Reason);

        // A Data header declaring 65,537 bytes (one over the traffic cap) must not make the receiver wait for 64 KiB.
        byte[] stream =
        [
            0xA7, 0xC0, 0x01, 0x00, 0x21, 0x00, 0x01, 0x00, 0x01, 0x00,
            .. DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [0x42]),
        ];

        receiver.Feed(stream);

        Assert.Single(target.Applied);
        Assert.Equal([DisplayMirrorRejectReason.TooLarge], rejects);
        Assert.Equal(1, receiver.GetStreamStatistics().Resyncs);

        // A smaller configured cap applies too.
        receiver.MaxTrafficPayload = 2;
        receiver.Feed(DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [1, 2, 3]));
        receiver.Feed(DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [1, 2]));

        Assert.Equal(2, target.Applied.Count);
        Assert.Equal(DisplayMirrorRejectReason.TooLarge, rejects[^1]);
    }

    [Fact]
    public void ProcessMessage_ExactlyOnePacket_OrMalformed()
    {
        using var receiver = new DisplayMirrorReceiver();
        var target = new RecordingTarget();
        receiver.Register(5, target);
        var packet = DisplayMirrorProtocol.Encode(5, DisplayMirrorMessageType.Data, [7, 7]);

        Assert.Equal(DisplayMirrorRejectReason.None, receiver.ProcessMessage(packet));
        Assert.Equal(DisplayMirrorRejectReason.Malformed, receiver.ProcessMessage([.. packet, .. packet]));
        Assert.Equal(DisplayMirrorRejectReason.Malformed, receiver.ProcessMessage([.. packet, 0x00]));
        Assert.Equal(DisplayMirrorRejectReason.Truncated, receiver.ProcessMessage(packet.AsSpan(0, packet.Length - 1)));

        var corrupted = (byte[])packet.Clone();
        corrupted[10] ^= 0xFF;
        Assert.Equal(DisplayMirrorRejectReason.BadCrc, receiver.ProcessMessage(corrupted));

        Assert.Single(target.Applied);
        Assert.Equal(1, receiver.GetStreamStatistics().CrcErrors);
        Assert.Equal(1, receiver.GetStatistics(5).Packets);
    }

    [Fact]
    public void UnregisteredDisplayId_IsCountedNoTarget()
    {
        using var receiver = new DisplayMirrorReceiver();
        var rejects = new List<DisplayMirrorRejectedEventArgs>();
        receiver.PacketRejected += (_, e) => rejects.Add(e);

        var reason = receiver.ProcessMessage(DisplayMirrorProtocol.Encode(9, DisplayMirrorMessageType.Data, [1]));

        Assert.Equal(DisplayMirrorRejectReason.NoTarget, reason);
        var reject = Assert.Single(rejects);
        Assert.Equal((byte)9, reject.DisplayId);
        Assert.Equal(DisplayMirrorRejectReason.NoTarget, reject.Reason);
        var statistics = receiver.GetStatistics(9);
        Assert.Equal(1, statistics.Rejected);
        Assert.Equal(0, statistics.Packets);

        // Unregister removes a target.
        receiver.Register(9, new RecordingTarget());
        Assert.True(receiver.Unregister(9));
        Assert.False(receiver.Unregister(9));
        Assert.Equal(
            DisplayMirrorRejectReason.NoTarget,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(9, DisplayMirrorMessageType.Data, [1])));
    }

    [Fact]
    public void TargetThatThrows_IsCountedAndTheReceiverContinues()
    {
        using var receiver = new DisplayMirrorReceiver();
        var failing = new ThrowingTarget();
        var recording = new RecordingTarget();
        receiver.Register(1, failing);
        receiver.Register(2, recording);
        var rejects = new List<DisplayMirrorRejectReason>();
        receiver.PacketRejected += (_, e) => rejects.Add(e.Reason);

        receiver.Feed(
        [
            .. DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Data, [1]),
            .. DisplayMirrorProtocol.Encode(2, DisplayMirrorMessageType.Data, [2]),
        ]);

        Assert.Equal([DisplayMirrorRejectReason.TargetFailed], rejects);
        Assert.Equal(1, receiver.GetStatistics(1).Rejected);
        Assert.Single(recording.Applied);
        Assert.Equal(
            DisplayMirrorRejectReason.TargetFailed,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Data, [1])));
    }

    [Fact]
    public void FrameOfAnotherSize_IsRejectedSizeMismatch()
    {
        using var receiver = new DisplayMirrorReceiver();
        var mono = new MonochromeFrameBuffer(128, 32);
        var color = new ColorFrameBuffer(2, 2);
        receiver.Register(0, DisplayMirrorTargets.For(mono));
        receiver.Register(1, DisplayMirrorTargets.For(color));

        var monoFrame = DisplayMirrorProtocol.CreateMonoFramePayload(128, 64, new byte[1024]);
        var colorFrame = DisplayMirrorProtocol.CreateColorFramePayload(3, 2, new byte[12]);

        Assert.Equal(
            DisplayMirrorRejectReason.SizeMismatch,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.MonoFrame, monoFrame)));
        Assert.Equal(
            DisplayMirrorRejectReason.SizeMismatch,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.ColorFrame, colorFrame)));
        Assert.Equal(1, receiver.GetStatistics(0).Rejected);
        Assert.Equal(128, mono.Width);
        Assert.Equal(32, mono.Height);

        // A frame whose data does not match its own dimensions is malformed, not a target failure.
        byte[] shortFrame = [0x80, 0x00, 0x20, 0x00, 0x01];
        Assert.Equal(
            DisplayMirrorRejectReason.Malformed,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.MonoFrame, shortFrame)));

        // The colour frame target loads big-endian RGB565: 0xF800 is red.
        var good = DisplayMirrorProtocol.CreateColorFramePayload(2, 2, [0xF8, 0x00, 0x07, 0xE0, 0x00, 0x1F, 0xFF, 0xFF]);
        Assert.Equal(
            DisplayMirrorRejectReason.None,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.ColorFrame, good)));
        Assert.Equal(Color.FromRgb(0xFF, 0x00, 0x00), color.GetPixel(0, 0));
        Assert.Equal(Color.FromRgb(0x00, 0x00, 0xFF), color.GetPixel(0, 1));

        // A frame target does not take traffic.
        Assert.Equal(
            DisplayMirrorRejectReason.Unsupported,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Data, [1])));
        Assert.Equal(1, receiver.GetStatistics(1).Unsupported);
        Assert.Equal(1, receiver.GetStatistics(1).Rejected);
        Assert.Equal(1, receiver.GetStatistics(1).Packets);
    }

    [Fact]
    public void Describe_RaisesTheEventEvenWithoutATarget()
    {
        using var receiver = new DisplayMirrorReceiver();
        var described = new List<DisplayMirrorDescribeEventArgs>();
        var rejects = new List<DisplayMirrorRejectedEventArgs>();
        receiver.DescribeReceived += (_, e) => described.Add(e);
        receiver.PacketRejected += (_, e) => rejects.Add(e);
        var describe = new DisplayMirrorDescribe(DisplayMirrorDisplayKind.MonoOled, DisplayMirrorChip.Ssd1306, 128, 64, "OLED");
        var packet = DisplayMirrorProtocol.Encode(4, DisplayMirrorMessageType.Describe, describe.ToPayload());

        Assert.Equal(DisplayMirrorRejectReason.None, receiver.ProcessMessage(packet));

        var e = Assert.Single(described);
        Assert.Equal(4, e.DisplayId);
        Assert.Equal(describe, e.Describe);
        Assert.Empty(rejects);

        // With a target that ignores it, it is neither unsupported nor rejected (ruling R1).
        receiver.Register(4, DisplayMirrorTargets.For(new MonochromeFrameBuffer()));
        receiver.Feed(packet);

        Assert.Equal(2, described.Count);
        Assert.Empty(rejects);
        var statistics = receiver.GetStatistics(4);
        Assert.Equal(2, statistics.Packets);
        Assert.Equal(2L * packet.Length, statistics.Bytes);
        Assert.Equal(0, statistics.Unsupported);
        Assert.Equal(0, statistics.Rejected);

        // A Describe payload shorter than 6 bytes is malformed.
        Assert.Equal(
            DisplayMirrorRejectReason.Malformed,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(4, DisplayMirrorMessageType.Describe, [1, 2, 3])));
        Assert.Equal(2, described.Count);
    }

    [Fact]
    public void I2cTarget_WithAddressFilter_IgnoresOtherAddresses()
    {
        using var receiver = new DisplayMirrorReceiver();
        var buffer = new CharacterDisplayBuffer(16, 2);
        var decoder = new Pcf8574Hd44780Decoder(new Hd44780Controller(buffer));
        receiver.Register(0, DisplayMirrorTargets.For(decoder, address: 0x27));

        // A byte with the backlight bit clear, sent to another address, is ignored.
        Assert.Equal(
            DisplayMirrorRejectReason.Unsupported,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.I2cWrite, [0x3F, 0x00])));
        Assert.True(decoder.IsBacklightOn);

        Assert.Equal(
            DisplayMirrorRejectReason.None,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.I2cWrite, [0x27, 0x00])));
        Assert.False(decoder.IsBacklightOn);

        // Without a filter any address is taken; an I2C write without an address byte is malformed.
        var unfiltered = new Pcf8574Hd44780Decoder(new Hd44780Controller(new CharacterDisplayBuffer(16, 2)));
        receiver.Register(1, DisplayMirrorTargets.For(unfiltered));
        Assert.Equal(
            DisplayMirrorRejectReason.None,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.I2cWrite, [0x3F, 0x00])));
        Assert.False(unfiltered.IsBacklightOn);
        Assert.Equal(
            DisplayMirrorRejectReason.Malformed,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.I2cWrite, [])));
    }

    [Fact]
    public void Combine_FirstSupportingTargetWins()
    {
        var first = new RecordingTarget(DisplayMirrorMessageType.Data);
        var second = new RecordingTarget(DisplayMirrorMessageType.Data, DisplayMirrorMessageType.Commands);
        var third = new RecordingTarget();
        var combined = DisplayMirrorTargets.Combine(first, second, third);

        var data = new DisplayMirrorPacket(0, DisplayMirrorMessageType.Data, 0, new byte[] { 1 });
        var commands = new DisplayMirrorPacket(0, DisplayMirrorMessageType.Commands, 0, new byte[] { 2 });
        var latch = new DisplayMirrorPacket(0, DisplayMirrorMessageType.Latch, 0, new byte[] { 3 });

        Assert.Equal(DisplayMirrorRejectReason.None, combined.Apply(data));
        Assert.Equal(DisplayMirrorRejectReason.None, combined.Apply(commands));
        Assert.Single(first.Applied);
        Assert.Single(second.Applied);
        Assert.Empty(third.Applied);

        Assert.Equal(DisplayMirrorRejectReason.None, combined.Apply(latch));
        Assert.Single(third.Applied);

        var none = DisplayMirrorTargets.Combine(first);
        Assert.Equal(DisplayMirrorRejectReason.Unsupported, none.Apply(latch));
    }

    [Fact]
    public void Statistics_CountPacketsBytesCrcErrorsAndResyncs()
    {
        var time = new ManualTimeProvider();
        using var receiver = new DisplayMirrorReceiver(time);
        receiver.Register(1, new RecordingTarget(DisplayMirrorMessageType.Data));

        var good = DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Data, [1, 2, 3]);
        var bad = (byte[])good.Clone();
        bad[^1] ^= 0xFF;
        var unsupported = DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Latch, [1]);

        Assert.Equal(new DisplayMirrorStatistics(0, 0, 0, 0, 0, 0, null), receiver.GetStatistics(1));

        receiver.Feed([.. good, .. bad, .. good]);
        time.Advance(TimeSpan.FromSeconds(2));
        receiver.Feed([0x01, 0x02, 0x03, .. good, .. unsupported]);

        // After the bad CRC packet the receiver skips one byte and rescans, so the good packet that follows it is not lost.
        var statistics = receiver.GetStatistics(1);
        Assert.Equal(3, statistics.Packets);
        Assert.Equal(3L * good.Length, statistics.Bytes);
        Assert.Equal(1, statistics.Unsupported);
        Assert.Equal(0, statistics.Rejected);
        Assert.Equal(0, statistics.CrcErrors);
        Assert.Equal(time.GetUtcNow(), statistics.LastPacket);

        // One resync per loss of sync: after the bad CRC packet, and before the noise bytes.
        var stream = receiver.GetStreamStatistics();
        Assert.Equal(1, stream.CrcErrors);
        Assert.Equal(2, stream.Resyncs);

        var all = receiver.GetAllStatistics();
        Assert.Equal(statistics, Assert.Single(all).Value);
    }

    [Fact]
    public void Feed_PartialPacketOlderThanTheTimeout_IsDropped()
    {
        var time = new ManualTimeProvider();
        using var receiver = new DisplayMirrorReceiver(time);
        var target = new RecordingTarget();
        receiver.Register(0, target);
        var rejects = new List<DisplayMirrorRejectReason>();
        receiver.PacketRejected += (_, e) => rejects.Add(e.Reason);
        var packet = DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [1, 2, 3, 4]);

        Assert.Equal(TimeSpan.FromSeconds(1), receiver.PartialPacketTimeout);

        receiver.Feed(packet.AsSpan(0, 8));
        time.Advance(TimeSpan.FromMilliseconds(1500));
        receiver.Feed(packet.AsSpan(8));
        Assert.Empty(target.Applied);
        Assert.Equal([DisplayMirrorRejectReason.Truncated], rejects);

        receiver.Feed(packet.AsSpan(0, 8));
        time.Advance(TimeSpan.FromMilliseconds(500));
        receiver.Feed(packet.AsSpan(8));
        Assert.Single(target.Applied);

        // ResetPartial drops a held partial packet.
        receiver.Feed(packet.AsSpan(0, 8));
        receiver.ResetPartial();
        receiver.Feed(packet.AsSpan(8));
        Assert.Single(target.Applied);
    }

    [Fact]
    public void Feed_StaleFalseHeader_KeepsTheGoodPacketsHeldBehindIt()
    {
        var time = new ManualTimeProvider();
        using var receiver = new DisplayMirrorReceiver(time);
        var target = new RecordingTarget();
        receiver.Register(0, target);
        var rejects = new List<DisplayMirrorRejectedEventArgs>();
        receiver.PacketRejected += (_, e) => rejects.Add(e);

        // A header that passes version, type and cap checks but declares 1,000 Data bytes (like a packet cut short by a sender reboot):
        // the good packet after it is held inside that "payload" while the parser waits.
        byte[] falseHeader = [0xA7, 0xC0, 0x01, 0x00, 0x21, 0x00, 0xE8, 0x03, 0x00, 0x00];
        receiver.Feed([.. falseHeader, .. DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [1])]);
        Assert.Empty(target.Applied);

        time.Advance(TimeSpan.FromSeconds(2));
        receiver.Feed(DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [2]));

        Assert.Equal([new byte[] { 1 }, new byte[] { 2 }], target.Applied.Select(a => a.Payload));
        var reject = Assert.Single(rejects);
        Assert.Null(reject.DisplayId);
        Assert.Equal(DisplayMirrorRejectReason.Truncated, reject.Reason);
        var stream = receiver.GetStreamStatistics();
        Assert.Equal(1, stream.Resyncs);
        Assert.Equal(1, stream.Rejected);
    }

    [Fact]
    public void Feed_FalseFrameHeaderWithInconsistentLength_IsNoiseWithoutWaiting()
    {
        using var receiver = new DisplayMirrorReceiver();
        var target = new RecordingTarget();
        receiver.Register(0, target);
        var rejects = new List<DisplayMirrorRejectReason>();
        receiver.PacketRejected += (_, e) => rejects.Add(e.Reason);
        var good1 = DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [1]);
        var good2 = DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [2]);
        var good3 = DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [3]);

        // A MonoFrame header declaring 1,000,000 bytes (under the 4 MiB cap); the next bytes (the good packet's magic) read as 0xC0A7 x 1,
        // which needs 4 + 49,319 bytes, not 1,000,000.
        byte[] falseMono = [0xA7, 0xC0, 0x01, 0x00, 0x10, 0x00, 0x40, 0x42, 0x0F, 0x00];

        // A ColorFrame header declaring 1,000 bytes with 128 x 64 dimensions (which need 4 + 16,384 bytes).
        byte[] falseColor = [0xA7, 0xC0, 0x01, 0x00, 0x11, 0x00, 0xE8, 0x03, 0x00, 0x00, 0x80, 0x00, 0x40, 0x00];

        // An EPaperFrame header with three planes.
        byte[] falseEPaper = [0xA7, 0xC0, 0x01, 0x00, 0x12, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x08, 0x00, 0x02, 0x00, 0x03];

        receiver.Feed([.. falseMono, .. good1, .. falseColor, .. good2, .. falseEPaper, .. good3]);

        // Applied at once, with no time advance and no more bytes.
        Assert.Equal([new byte[] { 1 }, new byte[] { 2 }, new byte[] { 3 }], target.Applied.Select(a => a.Payload));
        Assert.Equal(
            [DisplayMirrorRejectReason.Malformed, DisplayMirrorRejectReason.Malformed, DisplayMirrorRejectReason.Malformed],
            rejects);
        Assert.Equal(3, receiver.GetStreamStatistics().Resyncs);
    }

    [Fact]
    public void Feed_PlausibleFrames_ApplyOrFailTheCrc()
    {
        using var receiver = new DisplayMirrorReceiver();
        var buffer = new MonochromeFrameBuffer(8, 8);
        var recording = new RecordingTarget();
        receiver.Register(0, DisplayMirrorTargets.For(buffer));
        receiver.Register(1, recording);
        var frame = DisplayMirrorProtocol.Encode(
            0,
            DisplayMirrorMessageType.MonoFrame,
            DisplayMirrorProtocol.CreateMonoFramePayload(8, 8, [0xFF, 0x81, 0x81, 0x81, 0x81, 0x81, 0x81, 0xFF]));
        var badCrc = (byte[])frame.Clone();
        badCrc[^1] ^= 0xFF;
        var ePaper = DisplayMirrorProtocol.Encode(
            1,
            DisplayMirrorMessageType.EPaperFrame,
            DisplayMirrorProtocol.CreateEPaperFramePayload(8, 2, [0xF0, 0x0F], [0x01, 0x80], isPartial: true));

        receiver.Feed([.. badCrc, .. frame, .. ePaper]);

        Assert.True(buffer.GetPixel(0, 0));
        Assert.False(buffer.GetPixel(1, 1));
        Assert.Single(recording.Applied);
        var stream = receiver.GetStreamStatistics();
        Assert.Equal(1, stream.CrcErrors);
        Assert.Equal(1, stream.Resyncs);
        Assert.Equal(0, stream.Rejected);
    }

    [Fact]
    public void ReentryFromATarget_Throws_AndTheStreamStaysIntact()
    {
        using var receiver = new DisplayMirrorReceiver();
        var recording = new RecordingTarget();
        var reentrant = new ReentrantTarget(receiver);
        receiver.Register(0, reentrant);
        receiver.Register(1, recording);

        receiver.Feed(
        [
            .. DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [9]),
            .. DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Data, [1]),
            .. DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Data, [2]),
        ]);

        Assert.Equal(3, reentrant.Exceptions.Count);
        Assert.All(reentrant.Exceptions, e => Assert.IsType<InvalidOperationException>(e));
        Assert.Equal([new byte[] { 1 }, new byte[] { 2 }], recording.Applied.Select(a => a.Payload));
        Assert.Equal(1, receiver.GetStatistics(0).Packets);
        Assert.Equal(0, receiver.GetStreamStatistics().Resyncs);
    }

    [Fact]
    public void Events_AreRaisedOutsideTheLock()
    {
        using var receiver = new DisplayMirrorReceiver();
        var target = new RecordingTarget(DisplayMirrorMessageType.Data);
        receiver.Register(1, target);
        var describe = new DisplayMirrorDescribe(DisplayMirrorDisplayKind.MonoOled, DisplayMirrorChip.Ssd1306, 128, 64, "x");
        var completed = new List<bool>();

        receiver.DescribeReceived += (_, _) =>
        {
            var other = Task.Run(() => receiver.Feed(DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Data, [1])));
            completed.Add(other.Wait(WaitTimeout, TestContext.Current.CancellationToken));
        };
        receiver.PacketRejected += (_, _) =>
        {
            var other = Task.Run(() => receiver.ProcessMessage(DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Data, [2])));
            completed.Add(other.Wait(WaitTimeout, TestContext.Current.CancellationToken));
        };

        receiver.Feed(DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Describe, describe.ToPayload()));
        receiver.ProcessMessage(DisplayMirrorProtocol.Encode(7, DisplayMirrorMessageType.Data, [3]));

        Assert.Equal([true, true], completed);
        Assert.Equal(2, target.Applied.Count);
    }

    [Fact]
    public void Dispose_UnregistersTargetsAndIgnoresLaterInput()
    {
        var receiver = new DisplayMirrorReceiver();
        var target = new RecordingTarget();
        receiver.Register(0, target);
        var packet = DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, [1]);

        receiver.Dispose();
        receiver.Feed(packet);

        Assert.Equal(DisplayMirrorRejectReason.NoTarget, receiver.ProcessMessage(packet));
        Assert.Empty(target.Applied);
        Assert.Empty(receiver.GetAllStatistics());
        receiver.Dispose();
    }

    [Fact]
    public void MonoFrameEndToEnd_LightsTheOledBuffer()
    {
        using var receiver = new DisplayMirrorReceiver();
        var buffer = new MonochromeFrameBuffer(128, 64);
        receiver.Register(0, DisplayMirrorTargets.For(buffer));

        // Page-major, bit 0 = top row of a page: byte 0x01 at index 0 is pixel (0, 0); 0x80 at 128 * 7 + 127 is pixel (127, 63).
        var pages = new byte[1024];
        pages[0] = 0x01;
        pages[(128 * 7) + 127] = 0x80;
        pages[(128 * 1) + 5] = 0x04;

        receiver.Feed(
            DisplayMirrorProtocol.Encode(
                0,
                DisplayMirrorMessageType.MonoFrame,
                DisplayMirrorProtocol.CreateMonoFramePayload(128, 64, pages)));

        Assert.True(buffer.GetPixel(0, 0));
        Assert.True(buffer.GetPixel(127, 63));
        Assert.True(buffer.GetPixel(5, 10));
        Assert.False(buffer.GetPixel(1, 0));
        Assert.Equal(pages, buffer.GetFrame());
    }

    [Fact]
    public void I2cEndToEnd_ThroughSsd1306Controller_LightsPixels()
    {
        // ref: §LIB.A1306.1 (128x64 init transactions) and §LIB.A1306.2 (display(): the window commands with control byte 0x00,
        // then 0x40 + 31-byte data chunks), each I2C transaction sent as one I2cWrite packet to address 0x3C.
        var buffer = new MonochromeFrameBuffer();
        using var controller = new Ssd1306Controller(buffer);
        using var receiver = new DisplayMirrorReceiver();
        receiver.Register(2, DisplayMirrorTargets.For(new Ssd1306I2cDecoder(controller), address: 0x3C));

        var stream = new List<byte>();
        void Transaction(params byte[] bytes)
            => stream.AddRange(DisplayMirrorProtocol.Encode(2, DisplayMirrorMessageType.I2cWrite, [0x3C, .. bytes]));

        Transaction(0x00, 0xAE, 0xD5, 0x80, 0xA8);
        Transaction(0x00, 0x3F);
        Transaction(0x00, 0xD3, 0x00, 0x40, 0x8D);
        Transaction(0x00, 0x14);
        Transaction(0x00, 0x20, 0x00, 0xA1, 0xC8);
        Transaction(0x00, 0xDA);
        Transaction(0x00, 0x12);
        Transaction(0x00, 0x81);
        Transaction(0x00, 0xCF);
        Transaction(0x00, 0xD9);
        Transaction(0x00, 0xF1);
        Transaction(0x00, 0xDB, 0x40, 0xA4, 0xA6, 0x2E, 0xAF);

        var data = new byte[1024];
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = (byte)((i * 13) + 5);
        }

        Transaction(0x00, 0x22, 0x00, 0xFF, 0x21);
        Transaction(0x00, 0x00);
        Transaction(0x00, 0x7F);
        for (var offset = 0; offset < data.Length; offset += 31)
        {
            var count = System.Math.Min(31, data.Length - offset);
            Transaction([0x40, .. data.AsSpan(offset, count)]);
        }

        // Fed in uneven chunks, as a serial port delivers them.
        var bytes = stream.ToArray();
        for (var offset = 0; offset < bytes.Length; offset += 61)
        {
            receiver.Feed(bytes.AsSpan(offset, System.Math.Min(61, bytes.Length - offset)));
        }

        Assert.True(buffer.IsDisplayOn);

        // With A1 + C8 the RAM shows upright, so the frame equals the streamed bytes (page-major, bit 0 = top row).
        Assert.Equal(data, buffer.GetFrame());
        Assert.Equal(0, receiver.GetStatistics(2).Rejected);
    }

    [Fact]
    public void HdTargetEndToEnd_WritesText()
    {
        using var receiver = new DisplayMirrorReceiver();
        var buffer = new CharacterDisplayBuffer(16, 2);
        receiver.Register(0, DisplayMirrorTargets.For(new Hd44780Controller(buffer)));

        // Clear and set DDRAM address 0x00 (row 0), "Hi" as data, then set DDRAM address 0x40 (0xC0, row 1) and "Mirror" as data.
        receiver.Feed(
        [
            .. DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Commands, [0x01, 0x80]),
            .. DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, "Hi"u8),
            .. DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Commands, [0xC0]),
            .. DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Data, "Mirror"u8),
        ]);

        Assert.Equal(
            "Hi              \nMirror          ",
            buffer.GetVisibleText(CharacterRom.A00));
        Assert.Equal(
            DisplayMirrorRejectReason.Unsupported,
            receiver.ProcessMessage(DisplayMirrorProtocol.Encode(0, DisplayMirrorMessageType.Latch, [1])));
    }

    [Fact]
    public void GraphicControllerTarget_MapsTheTrafficTypes()
    {
        using var controller = new RecordingController();
        var target = DisplayMirrorTargets.For(controller);

        Assert.Equal(DisplayMirrorRejectReason.None, target.Apply(new(0, DisplayMirrorMessageType.Commands, 0, new byte[] { 0xAE })));
        Assert.Equal(DisplayMirrorRejectReason.None, target.Apply(new(0, DisplayMirrorMessageType.Data, 0, new byte[] { 1, 2 })));
        Assert.Equal(DisplayMirrorRejectReason.None, target.Apply(new(0, DisplayMirrorMessageType.Reset, 0, ReadOnlyMemory<byte>.Empty)));
        Assert.Equal(DisplayMirrorRejectReason.None, target.Apply(new(0, DisplayMirrorMessageType.Latch, 0, new byte[] { 0x0C, 0x01 })));
        Assert.Equal(DisplayMirrorRejectReason.Unsupported, target.Apply(new(0, DisplayMirrorMessageType.I2cWrite, 0, new byte[] { 0x3C })));

        Assert.Equal(
            ["C:AE", "D:0102", "R:", "D:0C01"],
            controller.Calls);
    }

    [Fact]
    public void Max7219LatchTarget_AppliesFrames()
    {
        // ref: §MAX7219.6 and §LIB.MDMAX72XX.1: one Latch packet is one CS frame, the first pair goes to the farthest device.
        // On a 2-device chain the frame 00 00 03 5A is a no-op for device 1 and "digit 2 = 0x5A" for device 0.
        var buffer = new Max7219ChainBuffer(2);
        using var controller = new Max7219Controller(buffer);
        using var receiver = new DisplayMirrorReceiver();
        receiver.Register(4, DisplayMirrorTargets.For(controller));

        receiver.Feed(
        [
            .. DisplayMirrorProtocol.Encode(4, DisplayMirrorMessageType.Latch, [0x00, 0x00, 0x03, 0x5A]),

            // Display test on for device 1 only (it is the first pair), no-op for device 0.
            .. DisplayMirrorProtocol.Encode(4, DisplayMirrorMessageType.Latch, [0x0F, 0x01, 0x00, 0x00]),
        ]);

        Assert.Equal(0x5A, buffer.GetDigit(0, 2));
        Assert.Equal(0x00, buffer.GetDigit(1, 2));
        Assert.False(buffer.IsDisplayTest(0));
        Assert.True(buffer.IsDisplayTest(1));
        Assert.Equal(0, receiver.GetStatistics(4).Rejected);

        // A Reset packet is the power-on reset: blank digits, display test off.
        receiver.Feed(DisplayMirrorProtocol.Encode(4, DisplayMirrorMessageType.Reset, []));

        Assert.Equal(0x00, buffer.GetDigit(0, 2));
        Assert.False(buffer.IsDisplayTest(1));
    }

    [Fact]
    public void St7920SerialTarget_AppliesBytes()
    {
        // ref: §ST7920.6 (serial framing: F8 instruction / FA data start byte, then each byte as two nibble bytes) and §ST7920.3,
        // §ST7920.5: instruction 0x0C (display on) = F8 00 C0; 0x3E twice (RE = 1, then G = 1) = F8 30 E0 F8 30 E0; GDRAM vertical 0
        // then horizontal 0 = F8 80 00 F8 80 00; data word 0x8000 = FA 80 00 00 00, which lights glass pixel (0, 0).
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        var decoder = new St7920SerialDecoder(controller);
        var target = DisplayMirrorTargets.For(decoder);
        using var receiver = new DisplayMirrorReceiver();
        receiver.Register(7, target);

        receiver.Feed(
        [
            .. DisplayMirrorProtocol.Encode(
                7,
                DisplayMirrorMessageType.Data,
                [0xF8, 0x00, 0xC0, 0xF8, 0x30, 0xE0, 0xF8, 0x30, 0xE0, 0xF8, 0x80, 0x00, 0xF8, 0x80, 0x00]),

            // The data run split over two packets: the framing carries over.
            .. DisplayMirrorProtocol.Encode(7, DisplayMirrorMessageType.Data, [0xFA, 0x80]),
            .. DisplayMirrorProtocol.Encode(7, DisplayMirrorMessageType.Data, [0x00, 0x00, 0x00]),
        ]);

        Assert.True(buffer.IsDisplayOn);
        Assert.True(buffer.GetPixel(0, 0));
        Assert.False(buffer.GetPixel(1, 0));
        Assert.Equal(0, receiver.GetStatistics(7).Rejected);

        // Commands and Latch are not serial bytes.
        Assert.Equal(DisplayMirrorRejectReason.Unsupported, target.Apply(new(7, DisplayMirrorMessageType.Commands, 0, new byte[] { 0x01 })));
        Assert.Equal(DisplayMirrorRejectReason.Unsupported, target.Apply(new(7, DisplayMirrorMessageType.Latch, 0, new byte[] { 0x01 })));
    }

    [Fact]
    public void St7920SerialTarget_Reset_ResetsTheChipAndTheFraming()
    {
        // ref: §ST7920.1 (power-on: display off, GDRAM 0) and §ST7920.6 (the decoder resets its framing on Reset).
        var buffer = new MonochromeFrameBuffer();
        using var controller = new St7920Controller(buffer);
        var target = DisplayMirrorTargets.For(new St7920SerialDecoder(controller));
        target.Apply(new(7, DisplayMirrorMessageType.Data, 0, new byte[] { 0xF8, 0x00, 0xC0, 0xF8, 0x30, 0xE0, 0xF8, 0x30, 0xE0, 0xF8, 0x80, 0x00, 0xF8, 0x80, 0x00 }));
        Assert.True(buffer.IsDisplayOn);

        // A data start byte and half a pair, then the host resets the module.
        target.Apply(new(7, DisplayMirrorMessageType.Data, 0, new byte[] { 0xFA, 0x80 }));
        Assert.Equal(DisplayMirrorRejectReason.None, target.Apply(new(7, DisplayMirrorMessageType.Reset, 0, ReadOnlyMemory<byte>.Empty)));

        Assert.False(buffer.IsDisplayOn);

        // Without a new start byte the next bytes are dropped: they neither complete the old pair nor reach the chip.
        target.Apply(new(7, DisplayMirrorMessageType.Data, 0, new byte[] { 0x00, 0xC0 }));
        target.Apply(new(7, DisplayMirrorMessageType.Data, 0, new byte[] { 0xF8, 0x00, 0xC0, 0xF8, 0x30, 0xE0, 0xF8, 0x30, 0xE0 }));

        Assert.True(buffer.IsDisplayOn);
        Assert.False(buffer.GetPixel(0, 0));

        // The high nibble of instruction 0x0C (display on) is pending when the host resets: the low nibble after the reset must not
        // complete it.
        target.Apply(new(7, DisplayMirrorMessageType.Reset, 0, ReadOnlyMemory<byte>.Empty));
        target.Apply(new(7, DisplayMirrorMessageType.Data, 0, new byte[] { 0xF8, 0x00 }));
        target.Apply(new(7, DisplayMirrorMessageType.Reset, 0, ReadOnlyMemory<byte>.Empty));
        target.Apply(new(7, DisplayMirrorMessageType.Data, 0, new byte[] { 0xC0 }));

        Assert.False(buffer.IsDisplayOn);
    }

    [Fact]
    public void EPaperFrameTarget_LoadsAndRefreshes()
    {
        // ref: §SSD1680.4 items 1, 2 and 7 (black plane 0 = black, red plane 1 = red, red wins) and the EPaperFrame layout of the protocol.
        var buffer = new EPaperFrameBuffer(8, 2, hasRed: true);
        var refreshes = new List<EPaperRefreshKind>();
        buffer.Refreshed += (_, e) => refreshes.Add(e.Kind);
        using var receiver = new DisplayMirrorReceiver();
        receiver.Register(5, DisplayMirrorTargets.For(buffer));

        // Row 0: 0x7F = pixel 0 black, the rest white. Row 1: white, and the red plane marks pixel 7 red.
        receiver.Feed(
            DisplayMirrorProtocol.Encode(
                5,
                DisplayMirrorMessageType.EPaperFrame,
                DisplayMirrorProtocol.CreateEPaperFramePayload(8, 2, [0x7F, 0xFF], [0x00, 0x01], isPartial: false)));

        Assert.Equal([EPaperRefreshKind.Full], refreshes);
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(1, 0));
        Assert.Equal(EPaperInk.Red, buffer.GetShownPixel(7, 1));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(6, 1));

        // A one-plane partial frame keeps the red RAM and refreshes partially.
        receiver.Feed(
            DisplayMirrorProtocol.Encode(
                5,
                DisplayMirrorMessageType.EPaperFrame,
                DisplayMirrorProtocol.CreateEPaperFramePayload(8, 2, [0xFF, 0xFE], [], isPartial: true)));

        Assert.Equal([EPaperRefreshKind.Full, EPaperRefreshKind.Partial], refreshes);
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.Red, buffer.GetShownPixel(7, 1));

        // A frame of another size is rejected and changes nothing.
        receiver.Feed(
            DisplayMirrorProtocol.Encode(
                5,
                DisplayMirrorMessageType.EPaperFrame,
                DisplayMirrorProtocol.CreateEPaperFramePayload(16, 2, new byte[4], [], isPartial: false)));

        Assert.Equal(2, refreshes.Count);
        Assert.Equal(1, receiver.GetStatistics(5).Rejected);
    }

    private sealed record AppliedPacket(
        byte DisplayId,
        DisplayMirrorMessageType Type,
        byte Flags,
        byte[] Payload);

    private sealed class RecordingTarget : IDisplayMirrorTarget
    {
        private readonly DisplayMirrorMessageType[] supported;

        public RecordingTarget(params DisplayMirrorMessageType[] supported)
            => this.supported = supported;

        public List<AppliedPacket> Applied { get; } = [];

        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
        {
            if (supported.Length > 0 &&
                Array.IndexOf(supported, packet.Type) < 0)
            {
                return DisplayMirrorRejectReason.Unsupported;
            }

            lock (Applied)
            {
                Applied.Add(new AppliedPacket(packet.DisplayId, packet.Type, packet.Flags, packet.Payload.ToArray()));
            }

            return DisplayMirrorRejectReason.None;
        }
    }

    private sealed class ReentrantTarget(DisplayMirrorReceiver receiver) : IDisplayMirrorTarget
    {
        public List<Exception> Exceptions { get; } = [];

        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
        {
            var other = DisplayMirrorProtocol.Encode(1, DisplayMirrorMessageType.Data, [0xEE]);
            Try(() => receiver.Feed(other));
            Try(() => receiver.ProcessMessage(other));
            Try(receiver.ResetPartial);
            return DisplayMirrorRejectReason.None;
        }

        private void Try(Action action)
        {
            try
            {
                action();
            }
            catch (InvalidOperationException ex)
            {
                Exceptions.Add(ex);
            }
        }
    }

    private sealed class ThrowingTarget : IDisplayMirrorTarget
    {
        public DisplayMirrorRejectReason Apply(DisplayMirrorPacket packet)
            => throw new InvalidOperationException("The target failed.");
    }

    private sealed class RecordingController : IGraphicDisplayController
    {
        public List<string> Calls { get; } = [];

        public void WriteCommand(byte command)
            => Calls.Add("C:" + Convert.ToHexString([command]));

        public void WriteCommands(ReadOnlySpan<byte> commands)
            => Calls.Add("C:" + Convert.ToHexString(commands));

        public void WriteData(ReadOnlySpan<byte> data)
            => Calls.Add("D:" + Convert.ToHexString(data));

        public void Reset()
            => Calls.Add("R:");

        public void Dispose()
        {
            // Nothing to release.
        }
    }
}