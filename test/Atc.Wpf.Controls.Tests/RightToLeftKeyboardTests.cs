namespace Atc.Wpf.Controls.Tests;

/// <summary>
/// In a right-to-left layout the controls are mirrored, so Left and Right must swap meaning.
/// </summary>
public sealed class RightToLeftKeyboardTests
{
    [StaTheory]
    [InlineData(FlowDirection.LeftToRight, Key.Right, 3.0)]
    [InlineData(FlowDirection.LeftToRight, Key.Left, 1.0)]
    [InlineData(FlowDirection.RightToLeft, Key.Left, 3.0)]
    [InlineData(FlowDirection.RightToLeft, Key.Right, 1.0)]
    public void Rating_ArrowKeysFollowTheFlowDirection(
        FlowDirection flowDirection,
        Key key,
        double expected)
    {
        var rating = new Rating { Value = 2, FlowDirection = flowDirection };

        KeyPress.Send(rating, key);

        Assert.Equal(expected, rating.Value);
    }

    [StaTheory]
    [InlineData(FlowDirection.LeftToRight, Key.Right, 2)]
    [InlineData(FlowDirection.LeftToRight, Key.Left, 0)]
    [InlineData(FlowDirection.RightToLeft, Key.Left, 2)]
    [InlineData(FlowDirection.RightToLeft, Key.Right, 0)]
    public void Segmented_ArrowKeysFollowTheFlowDirection(
        FlowDirection flowDirection,
        Key key,
        int expected)
    {
        var segmented = new Segmented { FlowDirection = flowDirection };
        segmented.Items.Add(new SegmentedItem());
        segmented.Items.Add(new SegmentedItem());
        segmented.Items.Add(new SegmentedItem());
        segmented.SelectedIndex = 1;

        KeyPress.Send(segmented, key);

        Assert.Equal(expected, segmented.SelectedIndex);
    }

    [StaTheory]
    [InlineData(FlowDirection.LeftToRight, Key.Right, 2)]
    [InlineData(FlowDirection.LeftToRight, Key.Left, 0)]
    [InlineData(FlowDirection.RightToLeft, Key.Left, 2)]
    [InlineData(FlowDirection.RightToLeft, Key.Right, 0)]
    public void Carousel_ArrowKeysFollowTheFlowDirection(
        FlowDirection flowDirection,
        Key key,
        int expected)
    {
        var carousel = new Carousel { FlowDirection = flowDirection, IsInfiniteLoop = false };
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.Items.Add(new Border());
        carousel.SelectedIndex = 1;

        KeyPress.Send(carousel, key);

        Assert.Equal(expected, carousel.SelectedIndex);
    }

    [Theory]
    [InlineData(FlowDirection.LeftToRight, Key.Left, Key.Left)]
    [InlineData(FlowDirection.LeftToRight, Key.Right, Key.Right)]
    [InlineData(FlowDirection.RightToLeft, Key.Left, Key.Right)]
    [InlineData(FlowDirection.RightToLeft, Key.Right, Key.Left)]
    [InlineData(FlowDirection.RightToLeft, Key.Up, Key.Up)]
    [InlineData(FlowDirection.RightToLeft, Key.Home, Key.Home)]
    public void FlowDirectionKeyHelper_SwapsLeftAndRightInRightToLeft(
        FlowDirection flowDirection,
        Key key,
        Key expected)
        => Assert.Equal(expected, FlowDirectionKeyHelper.ToLayoutKey(key, flowDirection));
}