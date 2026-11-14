namespace Atc.Wpf.Controls.Tests.Clocks;

[Collection(nameof(PackResourceTestGroup))]
public sealed class SevenSegmentTests : IDisposable
{
    private const double Unlit = 0.05;

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [Theory]
    [InlineData(0, "Top,TopRight,BottomRight,Bottom,BottomLeft,TopLeft")]
    [InlineData(1, "TopRight,BottomRight")]
    [InlineData(2, "Top,TopRight,Bottom,BottomLeft,Middle")]
    [InlineData(3, "Top,TopRight,BottomRight,Bottom,Middle")]
    [InlineData(4, "TopRight,BottomRight,TopLeft,Middle")]
    [InlineData(5, "Top,BottomRight,Bottom,TopLeft,Middle")]
    [InlineData(6, "Top,BottomRight,Bottom,BottomLeft,TopLeft,Middle")]
    [InlineData(7, "Top,TopRight,BottomRight")]
    [InlineData(8, "Top,TopRight,BottomRight,Bottom,BottomLeft,TopLeft,Middle")]
    [InlineData(9, "Top,TopRight,BottomRight,Bottom,TopLeft,Middle")]
    public void GetSegments_LightsTheSegmentsOfTheDigit(
        int digit,
        string expected)
    {
        var parts = SevenSegment.GetSegments(digit);

        var lit = Enum.GetValues<SevenSegmentParts>()
            .Where(p => p is not SevenSegmentParts.None and not SevenSegmentParts.All && parts.HasFlag(p))
            .Select(p => p.ToString());

        Assert.Equal(expected, string.Join(',', lit));
    }

    [Fact]
    public void GetSegments_ForNull_LightsNothing()
        => Assert.Equal(SevenSegmentParts.None, SevenSegment.GetSegments(digit: null));

    [Theory]
    [InlineData(-1)]
    [InlineData(10)]
    public void GetSegments_OutOfRange_Throws(int digit)
        => Assert.Throws<ArgumentOutOfRangeException>(() => SevenSegment.GetSegments(digit));

    [StaFact]
    public void Constructor_WithoutApplicationResources_ShowsZero()
    {
        var sut = new SevenSegment();

        Assert.Equal(0, sut.Digit);
        Assert.Equal(1.0, sut.GetSegmentOpacity(SevenSegmentParts.Top));
        Assert.Equal(Unlit, sut.GetSegmentOpacity(SevenSegmentParts.Middle));
    }

    [StaFact]
    public void Digit_WithoutAnimation_SetsTheSegmentOpacities()
    {
        var sut = new SevenSegment { PlayAnimation = false };

        sut.Digit = 1;

        Assert.Equal(SevenSegmentParts.TopRight | SevenSegmentParts.BottomRight, sut.LitSegments);
        Assert.Equal(1.0, sut.GetSegmentOpacity(SevenSegmentParts.TopRight));
        Assert.Equal(1.0, sut.GetSegmentOpacity(SevenSegmentParts.BottomRight));
        Assert.Equal(Unlit, sut.GetSegmentOpacity(SevenSegmentParts.Top));
        Assert.Equal(Unlit, sut.GetSegmentOpacity(SevenSegmentParts.Middle));
    }

    [StaFact]
    public void Digit_Null_DimsEverySegment()
    {
        var sut = new SevenSegment { PlayAnimation = false, NumberMinimumOpacity = 0 };

        sut.Digit = null;

        Assert.Equal(0, sut.GetSegmentOpacity(SevenSegmentParts.Top));
        Assert.Equal(0, sut.GetSegmentOpacity(SevenSegmentParts.BottomRight));
    }

    [StaFact]
    public void Digit_AfterAnAnimatedChange_WithoutAnimation_IsNotHeldByTheOldAnimation()
    {
        var sut = new SevenSegment { PlayAnimation = true };
        sut.Digit = 8;

        sut.PlayAnimation = false;
        sut.Digit = 1;

        Assert.Equal(Unlit, sut.GetSegmentOpacity(SevenSegmentParts.Top));
    }

    [StaFact]
    public void Digit_OutOfRange_IsRejected()
    {
        var sut = new SevenSegment();

        Assert.Throws<ArgumentException>(() => sut.Digit = 10);
        Assert.Throws<ArgumentOutOfRangeException>(() => sut.SetNumber(-1));
    }

    [StaFact]
    public void SetNumber_SetsTheDigit()
    {
        var sut = new SevenSegment();

        sut.SetNumber(7);

        Assert.Equal(7, sut.Digit);
    }

    [StaFact]
    public void PulseColon_WithoutAnimateColon_KeepsTheColonLit()
    {
        var sut = new SevenSegment { ShowColon = true, AnimateColon = false };

        sut.PulseColon();

        Assert.Equal(1.0, sut.GetColonOpacity());
    }
}