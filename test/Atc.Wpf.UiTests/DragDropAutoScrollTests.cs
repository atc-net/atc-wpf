namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the DragDrop sample page, dragging an item of the fixed-height reorder list to its
/// bottom edge and holding the pointer still keeps scrolling the list.
/// </summary>
[Collection("SampleApp")]
public sealed class DragDropAutoScrollTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public DragDropAutoScrollTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    [SuppressMessage("Major Bug", "S2925:Do not use Thread.Sleep", Justification = "The drag is held still on purpose, so real wall time must pass.")]
    public void Holding_a_drag_at_the_bottom_edge_keeps_scrolling_the_list()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpf",
            parentTreeNodeName: "Drag and Drop",
            leafTreeNodeName: "DragDrop");

        var list = Retry.WhileNull(
            () => mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ReorderList")),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(list);

        var scroll = list.Patterns.Scroll.Pattern;
        Assert.True(scroll.VerticallyScrollable.Value);
        var initialPercent = scroll.VerticalScrollPercent.Value;

        var firstItem = list.FindFirstDescendant(cf => cf.ByControlType(ControlType.ListItem));
        Assert.NotNull(firstItem);

        var itemBounds = firstItem.BoundingRectangle;
        var listBounds = list.BoundingRectangle;
        var start = new System.Drawing.Point(itemBounds.X + 20, itemBounds.Y + (itemBounds.Height / 2));
        var bottomEdge = new System.Drawing.Point(listBounds.X + 40, listBounds.Bottom - 8);

        double percentOnArrival;
        double percentAfterHolding;
        Mouse.MoveTo(start);
        Mouse.Down(MouseButton.Left);
        try
        {
            Mouse.MoveTo(start.X, start.Y + 15);
            Thread.Sleep(200);
            Mouse.MoveTo(bottomEdge);
            Thread.Sleep(200);
            percentOnArrival = scroll.VerticalScrollPercent.Value;

            // Hold the pointer still: OLE keeps raising DragOver, so the list keeps scrolling.
            Thread.Sleep(1500);
            percentAfterHolding = scroll.VerticalScrollPercent.Value;
        }
        finally
        {
            Mouse.Up(MouseButton.Left);
            Wait.UntilInputIsProcessed();
        }

        Assert.True(
            percentAfterHolding > percentOnArrival,
            $"Expected scrolling while holding still (start {initialPercent}, arrival {percentOnArrival}, after {percentAfterHolding}).");
    }
}