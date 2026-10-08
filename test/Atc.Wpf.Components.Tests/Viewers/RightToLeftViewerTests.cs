namespace Atc.Wpf.Components.Tests.Viewers;

/// <summary>
/// JSON reads left to right in every language, so the tree stays left-to-right in a right-to-left layout
/// while the toolbar around it mirrors. (TerminalViewer does the same, but needs application theme resources to construct.)
/// </summary>
public sealed class RightToLeftViewerTests
{
    [StaFact]
    public void JsonViewer_TreeStaysLeftToRight()
        => AssertContentStaysLeftToRight(new JsonViewer(), "JsonTreeView");

    private static void AssertContentStaysLeftToRight(
        UserControl viewer,
        string contentName)
    {
        _ = new Border { FlowDirection = FlowDirection.RightToLeft, Child = viewer };

        var content = Assert.IsAssignableFrom<FrameworkElement>(viewer.FindName(contentName));

        Assert.Equal(FlowDirection.RightToLeft, viewer.FlowDirection);
        Assert.Equal(FlowDirection.LeftToRight, content.FlowDirection);
    }
}