namespace Atc.Wpf.Tests.Extensions;

public sealed class GradientStopCollectionExtensionsTests
{
    private static GradientStopCollection BlackToWhite()
        => new()
        {
            new GradientStop(Colors.Black, 0d),
            new GradientStop(Colors.White, 1d),
        };

    [Fact]
    public void GetColorAtOffset_returns_first_stop_for_offset_below_zero()
    {
        var actual = BlackToWhite().GetColorAtOffset(-0.5);

        Assert.Equal(Colors.Black, actual);
    }

    [Fact]
    public void GetColorAtOffset_returns_first_stop_at_offset_zero()
    {
        var actual = BlackToWhite().GetColorAtOffset(0d);

        Assert.Equal(Colors.Black, actual);
    }

    [Fact]
    public void GetColorAtOffset_returns_last_stop_at_offset_one()
    {
        var actual = BlackToWhite().GetColorAtOffset(1d);

        Assert.Equal(Colors.White, actual);
    }

    [Fact]
    public void GetColorAtOffset_returns_last_stop_for_offset_above_one()
    {
        var actual = BlackToWhite().GetColorAtOffset(1.5);

        Assert.Equal(Colors.White, actual);
    }

    [Fact]
    public void GetColorAtOffset_interpolates_to_mid_grey_at_midpoint()
    {
        // (255 * 0.5) + 0 = 127.5; the byte cast truncates, so the midpoint
        // value is 127 rather than 128.
        var actual = BlackToWhite().GetColorAtOffset(0.5);

        Assert.Equal(127, actual.R);
        Assert.Equal(127, actual.G);
        Assert.Equal(127, actual.B);
    }

    [Fact]
    public void GetColorAtOffset_interpolates_within_a_single_segment_of_a_three_stop_gradient()
    {
        var collection = new GradientStopCollection
        {
            new GradientStop(Colors.Red, 0d),
            new GradientStop(Colors.Green, 0.5),
            new GradientStop(Colors.Blue, 1d),
        };

        var midpointOfFirstSegment = collection.GetColorAtOffset(0.25);

        // Midway between Red (#FF0000) and Green (#008000): roughly half of each component.
        Assert.InRange(midpointOfFirstSegment.R, 0x7F, 0x80);
        Assert.InRange(midpointOfFirstSegment.G, 0x3F, 0x40);
        Assert.Equal(0, midpointOfFirstSegment.B);
    }

    [Fact]
    public void GetColorAtOffset_handles_unsorted_stops_by_sorting_them_internally()
    {
        var collection = new GradientStopCollection
        {
            new GradientStop(Colors.White, 1d),
            new GradientStop(Colors.Black, 0d),
        };

        var atZero = collection.GetColorAtOffset(0d);
        var atOne = collection.GetColorAtOffset(1d);

        Assert.Equal(Colors.Black, atZero);
        Assert.Equal(Colors.White, atOne);
    }
}