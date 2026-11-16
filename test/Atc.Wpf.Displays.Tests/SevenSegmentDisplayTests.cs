namespace Atc.Wpf.Displays.Tests;

[Collection(nameof(PackResourceTestGroup))]
public sealed class SevenSegmentDisplayTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaTheory]
    [InlineData(0x0, "Top,TopRight,BottomRight,Bottom,BottomLeft,TopLeft")]
    [InlineData(0x1, "TopRight,BottomRight")]
    [InlineData(0x2, "Top,TopRight,Bottom,BottomLeft,Middle")]
    [InlineData(0x3, "Top,TopRight,BottomRight,Bottom,Middle")]
    [InlineData(0x4, "TopRight,BottomRight,TopLeft,Middle")]
    [InlineData(0x5, "Top,BottomRight,Bottom,TopLeft,Middle")]
    [InlineData(0x6, "Top,BottomRight,Bottom,BottomLeft,TopLeft,Middle")]
    [InlineData(0x7, "Top,TopRight,BottomRight")]
    [InlineData(0x8, "Top,TopRight,BottomRight,Bottom,BottomLeft,TopLeft,Middle")]
    [InlineData(0x9, "Top,TopRight,BottomRight,Bottom,TopLeft,Middle")]
    [InlineData(0xA, "Middle")]
    [InlineData(0xB, "Top,Bottom,BottomLeft,TopLeft,Middle")]
    [InlineData(0xC, "TopRight,BottomRight,BottomLeft,TopLeft,Middle")]
    [InlineData(0xD, "Bottom,BottomLeft,TopLeft")]
    [InlineData(0xE, "Top,TopRight,BottomLeft,TopLeft,Middle")]
    [InlineData(0xF, "")]
    public void CodeB_ShowsDigitsAndLetters(
        int code,
        string expected)
    {
        // ref: §MAX7219.4 (Code B: 0-9, '-', E, H, L, P, blank; segments A = Top, B = TopRight, C = BottomRight, D = Bottom,
        //      E = BottomLeft, F = TopLeft, G = Middle per §MAX7219.3)
        var sut = new SevenSegmentDisplay();
        var buffer = sut.Buffer;
        PowerUp(buffer);
        buffer.SetDecodeMode(0, 0xFF);

        buffer.SetDigit(0, 0, (byte)code);

        var digit0 = DigitAt(sut, 0);
        Assert.Equal(expected, Names(digit0.Segments!.Value));
        Assert.False(digit0.ShowDot);

        // D7 turns the decimal point on regardless of the decoder; D6..D4 are ignored (§MAX7219.4).
        buffer.SetDigit(0, 0, (byte)(0x80 | 0x70 | code));

        Assert.Equal(expected, Names(digit0.Segments!.Value));
        Assert.True(digit0.ShowDot);
    }

    [StaFact]
    public void RawMode_ShowsSegmentBits()
    {
        // ref: §MAX7219.3 (bits D7..D0 = DP A B C D E F G), §MAX7219.1 (decode register: one bit per digit)
        var sut = new SevenSegmentDisplay();
        var buffer = sut.Buffer;
        PowerUp(buffer);

        buffer.SetDigit(0, 0, 0x81);
        buffer.SetDigit(0, 1, 0x40);
        buffer.SetDigit(0, 2, 0x08);
        buffer.SetDigit(0, 3, 0x02);

        Assert.Equal(SevenSegmentParts.Middle, DigitAt(sut, 0).Segments);
        Assert.True(DigitAt(sut, 0).ShowDot);
        Assert.Equal(SevenSegmentParts.Top, DigitAt(sut, 1).Segments);
        Assert.Equal(SevenSegmentParts.Bottom, DigitAt(sut, 2).Segments);
        Assert.Equal(SevenSegmentParts.TopLeft, DigitAt(sut, 3).Segments);
        Assert.False(DigitAt(sut, 1).ShowDot);

        // Decode digit 0 only (0x01): 0x01 on digit 0 is Code B '1' (B C), on digit 1 the raw G segment.
        buffer.SetDecodeMode(0, 0x01);
        buffer.SetDigit(0, 0, 0x01);
        buffer.SetDigit(0, 1, 0x01);

        Assert.Equal(SevenSegmentParts.TopRight | SevenSegmentParts.BottomRight, DigitAt(sut, 0).Segments);
        Assert.Equal(SevenSegmentParts.Middle, DigitAt(sut, 1).Segments);
    }

    [StaFact]
    public void ScanLimit_HidesDigitsBeyond()
    {
        // ref: §MAX7219.5 (a digit is displayed only if its index <= scan limit)
        var sut = new SevenSegmentDisplay();
        var buffer = sut.Buffer;
        PowerUp(buffer);
        for (var digit = 0; digit < 8; digit++)
        {
            buffer.SetDigit(0, digit, 0xFF);
        }

        buffer.SetScanLimit(0, 2);

        for (var digit = 0; digit < 8; digit++)
        {
            var expected = digit <= 2
                ? SevenSegmentParts.All
                : SevenSegmentParts.None;
            Assert.Equal(expected, DigitAt(sut, digit).Segments);
            Assert.Equal(digit <= 2, DigitAt(sut, digit).ShowDot);
        }
    }

    [StaFact]
    public void Digit0_IsTheRightmost()
    {
        // The control's documented default (§MAX7219.8: common 8-digit modules have DIG0 at the right; ⚠️ unverified for hardware).
        var sut = new SevenSegmentDisplay { DigitCount = 4 };
        PowerUp(sut.Buffer);

        sut.Buffer.SetDigit(0, 0, 0x01);

        var controls = sut.SegmentControls;
        Assert.Equal(4, controls.Count);
        Assert.Equal(SevenSegmentParts.Middle, controls[3].Segments);
        Assert.Equal(SevenSegmentParts.None, controls[0].Segments);
    }

    [StaFact]
    public void Shutdown_DarkensEverySegment()
    {
        // ref: §MAX7219.5 (shutdown: display blank), §MAX7219.2 (power-on = shut down)
        var sut = new SevenSegmentDisplay();
        var buffer = sut.Buffer;
        buffer.SetScanLimit(0, 7);
        buffer.SetDigit(0, 0, 0xFF);

        Assert.All(sut.SegmentControls, c => Assert.Equal(SevenSegmentParts.None, c.Segments));
        Assert.All(sut.SegmentControls, c => Assert.False(c.ShowDot));
    }

    [StaFact]
    public void DisplayTest_LightsEverySegmentAndDot()
    {
        // ref: §MAX7219.5 (display test overrides shutdown, scan limit, decode and intensity)
        var sut = new SevenSegmentDisplay();

        sut.Buffer.SetDisplayTest(0, isDisplayTest: true);

        Assert.All(sut.SegmentControls, c => Assert.Equal(SevenSegmentParts.All, c.Segments));
        Assert.All(sut.SegmentControls, c => Assert.True(c.ShowDot));
        Assert.All(sut.SegmentControls, c => Assert.Equal(1.0, c.Opacity, 6));
    }

    [StaFact]
    public void Intensity_MapsToSegmentOpacity()
    {
        // ref: §MAX7219.1 (intensity 0..15), §MAX7219.5 (n = 0 is dim but not off)
        var sut = new SevenSegmentDisplay();

        Assert.All(sut.SegmentControls, c => Assert.Equal(0.25, c.Opacity, 6));

        sut.Buffer.SetIntensity(0, 15);
        Assert.All(sut.SegmentControls, c => Assert.Equal(1.0, c.Opacity, 6));

        sut.Buffer.SetIntensity(0, 5);
        Assert.All(sut.SegmentControls, c => Assert.Equal(0.25 + (0.75 * 5 / 15.0), c.Opacity, 6));
    }

    [StaFact]
    public void Brushes_ReachTheDigits()
    {
        var segmentBrush = Brushes.Red;
        var dotBrush = Brushes.Blue;

        var sut = new SevenSegmentDisplay { SegmentBrush = segmentBrush, DotBrush = dotBrush };

        Assert.All(sut.SegmentControls, c => Assert.Same(segmentBrush, c.NumberColor));
        Assert.All(sut.SegmentControls, c => Assert.Same(dotBrush, c.ColonColor));
    }

    [StaFact]
    public void DigitCount_IsCoerced()
    {
        var sut = new SevenSegmentDisplay { DigitCount = 12 };

        Assert.Equal(8, sut.DigitCount);
        Assert.Equal(8, sut.SegmentControls.Count);

        sut.DigitCount = 0;

        Assert.Equal(1, sut.DigitCount);
        Assert.Single(sut.SegmentControls);
    }

    [StaFact]
    public void ChangesOnAWorkerThread_ReachTheDigits()
    {
        var sut = new SevenSegmentDisplay();
        var buffer = sut.Buffer;
        PowerUp(buffer);

        Task.Run(() => buffer.SetDigit(0, 0, 0x40)).Wait();
        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

        Assert.Equal(SevenSegmentParts.Top, DigitAt(sut, 0).Segments);
    }

    [StaFact]
    public void AutomationPeer_NamesTheDigitCount()
    {
        var sut = new SevenSegmentDisplay { DigitCount = 4 };

        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.IsType<SevenSegmentDisplayAutomationPeer>(peer);
        Assert.Equal(nameof(SevenSegmentDisplay), peer.GetClassName());
        Assert.Equal("Seven-segment display, 4 digits", peer.GetName());
    }

    private static SevenSegment DigitAt(
        SevenSegmentDisplay sut,
        int digit)
        => sut.SegmentControls[sut.DigitCount - 1 - digit];

    private static void PowerUp(Max7219ChainBuffer buffer)
    {
        buffer.SetShutdown(0, isShutdown: false);
        buffer.SetScanLimit(0, 7);
        buffer.SetIntensity(0, 15);
    }

    private static string Names(SevenSegmentParts parts)
        => string.Join(
            ',',
            Enum.GetValues<SevenSegmentParts>()
                .Where(p => p is not SevenSegmentParts.None and not SevenSegmentParts.All && parts.HasFlag(p))
                .Select(p => p.ToString()));
}