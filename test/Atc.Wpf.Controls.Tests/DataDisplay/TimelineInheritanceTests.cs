namespace Atc.Wpf.Controls.Tests.DataDisplay;

/// <summary>
/// Timeline items are logical children of the timeline, so element content (e.g. a TextBlock) inherits the
/// timeline's text color instead of the system default (black in a dark theme).
/// </summary>
public sealed class TimelineInheritanceTests
{
    [StaFact]
    public void ItemContent_InheritsTheTimelineForeground()
    {
        var text = new TextBlock { Text = "Project Kickoff" };
        var item = new TimelineItem { Content = text };
        var sut = new Timeline { Foreground = Brushes.White };

        sut.Items.Add(item);

        Assert.Same(sut, item.Parent);
        Assert.Same(Brushes.White, text.Foreground);
    }

    [StaFact]
    public void ForegroundChanges_ReachTheItemContent()
    {
        var text = new TextBlock { Text = "Release" };
        var sut = new Timeline { Foreground = Brushes.White };
        sut.Items.Add(new TimelineItem { Content = text });

        sut.Foreground = Brushes.Orange;

        Assert.Same(Brushes.Orange, text.Foreground);
    }

    [StaFact]
    public void RemovedAndClearedItems_AreReleased()
    {
        var first = new TimelineItem { Content = "Alpha" };
        var second = new TimelineItem { Content = "Beta" };
        var sut = new Timeline();
        sut.Items.Add(first);
        sut.Items.Add(second);

        sut.Items.Remove(first);
        Assert.Null(first.Parent);
        Assert.Same(sut, second.Parent);

        sut.Items.Clear();
        Assert.Null(second.Parent);

        // A released item can join another timeline.
        var other = new Timeline();
        other.Items.Add(second);
        Assert.Same(other, second.Parent);
    }
}