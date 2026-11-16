namespace Atc.Wpf.Displays.Tests;

public sealed class EPaperFrameBufferTests
{
    [Fact]
    public void Constructor_Defaults_Is128x296BlackWhiteAndAllWhite()
    {
        var buffer = new EPaperFrameBuffer();

        Assert.Equal(128, buffer.Width);
        Assert.Equal(296, buffer.Height);
        Assert.False(buffer.HasRed);
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(127, 295));

        // ref: §SSD1680.3 (⚠️ the RAM at power-on is not stated; the buffer starts white, no red, by design).
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 0, 0));
        Assert.False(buffer.GetRamPixel(EPaperPlane.Red, 0, 0));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(10, 0)]
    [InlineData(401, 10)]
    [InlineData(10, 401)]
    public void Constructor_SizeOutOfRange_Throws(
        int width,
        int height)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new EPaperFrameBuffer(width, height));

    [Fact]
    public void WriteRam_PlaneLayoutIsMsbFirst()
    {
        // ref: §SSD1680.4 item 6: X in bytes, bit 7 = leftmost pixel; item 1: black plane bit 0 = black.
        var buffer = new EPaperFrameBuffer(24, 4);

        buffer.WriteRam(EPaperPlane.Black, 1, 2, 0b0111_1110);

        Assert.False(buffer.GetRamPixel(EPaperPlane.Black, 8, 2));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 9, 2));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 14, 2));
        Assert.False(buffer.GetRamPixel(EPaperPlane.Black, 15, 2));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 7, 2));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 16, 2));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 8, 1));

        buffer.Refresh(EPaperRefreshKind.Full);

        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(8, 2));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(9, 2));
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(15, 2));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(8, 3));
    }

    [Fact]
    public void WriteRam_ChangesRamOnly_UntilARefresh()
    {
        // ref: §SSD1680.4 item 7: the shown image is rendered only at a refresh, never from RAM writes.
        var buffer = new EPaperFrameBuffer(16, 2);
        var changes = 0;
        var refreshes = new List<EPaperRefreshKind>();
        buffer.Changed += (_, _) => changes++;
        buffer.Refreshed += (_, e) => refreshes.Add(e.Kind);

        buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x00);
        buffer.LoadRam(new byte[4], []);

        Assert.Equal(0, changes);
        Assert.Empty(refreshes);
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(15, 1));

        buffer.Refresh(EPaperRefreshKind.Partial);

        Assert.Equal(1, changes);
        Assert.Equal([EPaperRefreshKind.Partial], refreshes);
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(15, 1));

        // Writing RAM after the refresh does not touch the shown image.
        buffer.WriteRam(EPaperPlane.Black, 0, 0, 0xFF);

        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(0, 0));
    }

    [Fact]
    public void Refresh_PaddingOnlyDifference_DoesNotRaiseChanged()
    {
        // ref: §SSD1680.4 item 6: rows are padded to whole bytes and the padding bits are ignored. 10 pixels: bits 6-7 of byte 1 are padding.
        var buffer = new EPaperFrameBuffer(10, 2, hasRed: true);
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        buffer.WriteRam(EPaperPlane.Black, 1, 0, 0xC0);
        buffer.WriteRam(EPaperPlane.Red, 1, 1, 0x3F);
        buffer.Refresh(EPaperRefreshKind.Partial);

        Assert.Equal(0, changes);

        buffer.WriteRam(EPaperPlane.Black, 1, 0, 0x40);
        buffer.Refresh(EPaperRefreshKind.Partial);

        Assert.Equal(1, changes);
    }

    [Fact]
    public void Refresh_RaisesRefreshedOncePerCall_AndChangedOnlyWhenThePictureChanges()
    {
        var buffer = new EPaperFrameBuffer(8, 1);
        var changes = 0;
        var refreshes = new List<EPaperRefreshKind>();
        buffer.Changed += (_, _) => changes++;
        buffer.Refreshed += (_, e) => refreshes.Add(e.Kind);

        buffer.Refresh(EPaperRefreshKind.Full);

        Assert.Equal(0, changes);
        Assert.Equal([EPaperRefreshKind.Full], refreshes);

        buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x0F);
        buffer.Refresh(EPaperRefreshKind.Full);

        Assert.Equal(1, changes);
        Assert.Equal([EPaperRefreshKind.Full, EPaperRefreshKind.Full], refreshes);
    }

    [Fact]
    public void BeginBatch_RaisesChangedOnceAndTheRefreshesInOrder_WhenTheBatchEnds()
    {
        var buffer = new EPaperFrameBuffer(8, 1);
        var events = new List<string>();
        buffer.Changed += (_, _) => events.Add("Changed");
        buffer.Refreshed += (_, e) => events.Add(e.Kind.ToString());

        using (buffer.BeginBatch())
        {
            buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x00);
            buffer.Refresh(EPaperRefreshKind.Full);
            buffer.WriteRam(EPaperPlane.Black, 0, 0, 0xF0);
            buffer.Refresh(EPaperRefreshKind.Partial);

            Assert.Empty(events);
        }

        Assert.Equal(["Changed", nameof(EPaperRefreshKind.Full), nameof(EPaperRefreshKind.Partial)], events);
    }

    [Fact]
    public void LoadRam_WrongLength_ThrowsAndLeavesTheRam()
    {
        // ref: protocol EPaperFrame: each plane is ceil(width / 8) x height bytes; 12 x 2 -> 2 x 2 = 4 bytes.
        var buffer = new EPaperFrameBuffer(12, 2, hasRed: true);

        Assert.Throws<ArgumentException>(() => buffer.LoadRam(new byte[3], []));
        Assert.Throws<ArgumentException>(() => buffer.LoadRam(new byte[5], []));
        Assert.Throws<ArgumentException>(() => buffer.LoadRam(new byte[4], new byte[3]));

        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 0, 0));
        Assert.False(buffer.GetRamPixel(EPaperPlane.Red, 0, 0));
    }

    [Fact]
    public void LoadRam_EmptyRed_LeavesTheRedPlane()
    {
        var buffer = new EPaperFrameBuffer(8, 1, hasRed: true);
        buffer.LoadRam([0xFF], [0x80]);

        buffer.LoadRam([0x00], []);
        buffer.Refresh(EPaperRefreshKind.Full);

        Assert.True(buffer.GetRamPixel(EPaperPlane.Red, 0, 0));
        Assert.Equal(EPaperInk.Red, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(1, 0));
    }

    [Fact]
    public void RedPlane_ShowsRedOnlyOnARedPanel()
    {
        // ref: §SSD1680.4 items 2, 3, 7: red if the plane-2 bit is 1 (whatever the BW bit), else the BW bit; a BW panel ignores plane 2.
        var buffer = new EPaperFrameBuffer(8, 1, hasRed: true);
        buffer.LoadRam([0b0011_1111], [0b1010_0000]);
        buffer.Refresh(EPaperRefreshKind.Full);

        Assert.Equal(EPaperInk.Red, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(1, 0));
        Assert.Equal(EPaperInk.Red, buffer.GetShownPixel(2, 0));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(3, 0));

        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        buffer.HasRed = false;

        Assert.Equal(1, changes);
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(2, 0));

        buffer.HasRed = false;

        Assert.Equal(1, changes);
    }

    [Fact]
    public void Resize_KeepsTheRamAndTheShownPixelsThatFit()
    {
        var buffer = new EPaperFrameBuffer(16, 4);
        buffer.WriteRam(EPaperPlane.Black, 1, 3, 0x7F);
        buffer.Refresh(EPaperRefreshKind.Full);
        buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
        var changes = 0;
        buffer.Changed += (_, _) => changes++;

        buffer.Resize(20, 5);

        Assert.Equal(1, changes);
        Assert.Equal(20, buffer.Width);
        Assert.Equal(5, buffer.Height);
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(8, 3));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(9, 3));
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(19, 4));
        Assert.False(buffer.GetRamPixel(EPaperPlane.Black, 0, 0));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 16, 0));

        buffer.Resize(20, 5);

        Assert.Equal(1, changes);
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Resize(401, 5));
    }

    [Fact]
    public void Pixels_OutsideTheBuffer_ThrowOnReadAndAreIgnoredOnWrite()
    {
        var buffer = new EPaperFrameBuffer(10, 2);

        buffer.WriteRam(EPaperPlane.Black, 2, 0, 0x00);
        buffer.WriteRam(EPaperPlane.Black, -1, 0, 0x00);
        buffer.WriteRam(EPaperPlane.Black, 0, 2, 0x00);

        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetShownPixel(10, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetShownPixel(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.GetRamPixel(EPaperPlane.Red, 0, 2));
        Assert.True(buffer.GetRamPixel(EPaperPlane.Black, 9, 1));
    }

    [Fact]
    public void Target_EPaperFrame_LoadsTheRamAndRefreshes()
    {
        var buffer = new EPaperFrameBuffer(12, 2, hasRed: true);
        var target = DisplayMirrorTargets.For(buffer);
        var refreshes = new List<EPaperRefreshKind>();
        var changes = 0;
        buffer.Refreshed += (_, e) => refreshes.Add(e.Kind);
        buffer.Changed += (_, _) => changes++;

        var result = target.Apply(Frame(12, 2, [0x7F, 0xFF, 0xFF, 0xFF], [0x00, 0x00, 0x00, 0x10], isPartial: true));

        Assert.Equal(DisplayMirrorRejectReason.None, result);
        Assert.Equal([EPaperRefreshKind.Partial], refreshes);
        Assert.Equal(1, changes);
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.Red, buffer.GetShownPixel(11, 1));

        // One plane on a red-capable buffer leaves the red RAM as it is.
        result = target.Apply(Frame(12, 2, [0xFF, 0xFF, 0xFF, 0xFF], [], isPartial: false));

        Assert.Equal(DisplayMirrorRejectReason.None, result);
        Assert.Equal([EPaperRefreshKind.Partial, EPaperRefreshKind.Full], refreshes);
        Assert.Equal(EPaperInk.White, buffer.GetShownPixel(0, 0));
        Assert.Equal(EPaperInk.Red, buffer.GetShownPixel(11, 1));
    }

    [Fact]
    public void Target_RejectsOtherSizesMalformedFramesAndOtherTypes()
    {
        var buffer = new EPaperFrameBuffer(12, 2);
        var target = DisplayMirrorTargets.For(buffer);
        var refreshes = 0;
        buffer.Refreshed += (_, _) => refreshes++;

        Assert.Equal(DisplayMirrorRejectReason.SizeMismatch, target.Apply(Frame(16, 2, new byte[4], [], isPartial: false)));

        var good = DisplayMirrorProtocol.CreateEPaperFramePayload(12, 2, new byte[4], [], isPartial: false);

        var threePlanes = (byte[])good.Clone();
        threePlanes[4] = 3;
        Assert.Equal(DisplayMirrorRejectReason.Malformed, target.Apply(Packet(DisplayMirrorMessageType.EPaperFrame, threePlanes)));

        var badKind = (byte[])good.Clone();
        badKind[^1] = 2;
        Assert.Equal(DisplayMirrorRejectReason.Malformed, target.Apply(Packet(DisplayMirrorMessageType.EPaperFrame, badKind)));

        Assert.Equal(DisplayMirrorRejectReason.Malformed, target.Apply(Packet(DisplayMirrorMessageType.EPaperFrame, good[..^2])));
        Assert.Equal(DisplayMirrorRejectReason.Malformed, target.Apply(Packet(DisplayMirrorMessageType.EPaperFrame, [0x0C])));
        Assert.Equal(DisplayMirrorRejectReason.Unsupported, target.Apply(Packet(DisplayMirrorMessageType.MonoFrame, good)));
        Assert.Equal(0, refreshes);
    }

    [Fact]
    public void Target_NullBuffer_Throws()
        => Assert.Throws<ArgumentNullException>(() => DisplayMirrorTargets.For((EPaperFrameBuffer)null!));

    private static DisplayMirrorPacket Frame(
        int width,
        int height,
        byte[] black,
        byte[] red,
        bool isPartial)
        => Packet(
            DisplayMirrorMessageType.EPaperFrame,
            DisplayMirrorProtocol.CreateEPaperFramePayload(width, height, black, red, isPartial));

    private static DisplayMirrorPacket Packet(
        DisplayMirrorMessageType type,
        byte[] payload)
        => new(0, type, 0, payload);

    [Fact]
    public void CopyShown_ReturnsTheShownPlanesAndCountsFullRefreshes()
    {
        var buffer = new EPaperFrameBuffer(16, 2, hasRed: true);
        buffer.WriteRam(EPaperPlane.Black, 1, 1, 0x0F);
        buffer.WriteRam(EPaperPlane.Red, 0, 0, 0x80);
        byte[] black = [];
        byte[] red = [];

        var before = buffer.CopyShown(ref black, ref red);
        buffer.Refresh(EPaperRefreshKind.Full);
        buffer.Refresh(EPaperRefreshKind.Partial);
        var reused = black;
        var after = buffer.CopyShown(ref black, ref red);

        Assert.Equal(0, before.FullRefreshCount);
        Assert.Equal(new EPaperShownSnapshot(16, 2, HasRed: true, FullRefreshCount: 1), after);
        Assert.Same(reused, black);
        Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFF, 0x0F }, black);
        Assert.Equal(new byte[] { 0x80, 0x00, 0x00, 0x00 }, red);
    }
}