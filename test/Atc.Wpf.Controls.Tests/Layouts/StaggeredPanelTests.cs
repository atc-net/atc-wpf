namespace Atc.Wpf.Controls.Tests.Layouts;

public sealed class StaggeredPanelTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var panel = new StaggeredPanel();

        // Assert
        Assert.Equal(250d, panel.DesiredItemWidth);
        Assert.Equal(0d, panel.HorizontalSpacing);
        Assert.Equal(0d, panel.VerticalSpacing);
        Assert.Equal(default, panel.Padding);
    }

    [StaFact]
    public void Measure_EmptyPanel_DesiresZeroSize()
    {
        // Arrange
        var panel = new StaggeredPanel();

        // Act
        panel.Measure(new Size(300, 300));

        // Assert
        Assert.Equal(new Size(0, 0), panel.DesiredSize);
    }

    [StaFact]
    public void Measure_EmptyPanel_WithInfiniteSize_DesiresZeroSize()
    {
        // Arrange
        var panel = new StaggeredPanel();

        // Act
        panel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        // Assert
        Assert.Equal(new Size(0, 0), panel.DesiredSize);
    }

    [StaFact]
    public void Layout_ColumnCountDerivedFromWidth_PlacesItemsInShortestColumn()
    {
        // Arrange
        var panel = CreatePanel(HorizontalAlignment.Left, 50, 30, 40, 20);
        panel.DesiredItemWidth = 100;

        // Act
        Layout(panel, new Size(300, double.PositiveInfinity));

        // Assert
        Assert.Equal(new Size(300, 50), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 100, 50);
        AssertSlot(panel.Children[1], 100, 0, 100, 30);
        AssertSlot(panel.Children[2], 200, 0, 100, 40);
        AssertSlot(panel.Children[3], 100, 30, 100, 20);
    }

    [StaFact]
    public void Layout_NarrowerWidth_ReducesColumnCount()
    {
        // Arrange
        var panel = CreatePanel(HorizontalAlignment.Left, 50, 30, 40);
        panel.DesiredItemWidth = 100;

        // Act
        Layout(panel, new Size(250, double.PositiveInfinity));

        // Assert
        Assert.Equal(new Size(250, 70), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 100, 50);
        AssertSlot(panel.Children[1], 100, 0, 100, 30);
        AssertSlot(panel.Children[2], 100, 30, 100, 40);
    }

    [StaFact]
    public void Layout_WidthBelowDesiredItemWidth_UsesSingleColumnOfAvailableWidth()
    {
        // Arrange
        var panel = CreatePanel(HorizontalAlignment.Left, 50, 30);
        panel.DesiredItemWidth = 100;

        // Act
        Layout(panel, new Size(80, double.PositiveInfinity));

        // Assert
        Assert.Equal(new Size(80, 80), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 80, 50);
        AssertSlot(panel.Children[1], 0, 50, 80, 30);
    }

    [StaFact]
    public void Layout_HorizontalAndVerticalSpacing_AreAppliedBetweenItemsOnly()
    {
        // Arrange
        var panel = CreatePanel(HorizontalAlignment.Left, 50, 30, 40);
        panel.DesiredItemWidth = 100;
        panel.HorizontalSpacing = 10;
        panel.VerticalSpacing = 5;

        // Act
        Layout(panel, new Size(230, double.PositiveInfinity));

        // Assert
        Assert.Equal(new Size(230, 75), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 100, 50);
        AssertSlot(panel.Children[1], 110, 0, 100, 30);
        AssertSlot(panel.Children[2], 110, 35, 100, 40);
    }

    [StaFact]
    public void Layout_StretchAlignment_ExpandsColumnsToFillWidth()
    {
        // Arrange
        var panel = CreatePanel(HorizontalAlignment.Stretch, 50, 30, 40);
        panel.DesiredItemWidth = 100;
        panel.HorizontalSpacing = 10;
        panel.VerticalSpacing = 5;

        // Act
        Layout(panel, new Size(230, double.PositiveInfinity));

        // Assert
        AssertSlot(panel.Children[0], 0, 0, 110, 50);
        AssertSlot(panel.Children[1], 120, 0, 110, 30);
        AssertSlot(panel.Children[2], 120, 35, 110, 40);
    }

    [StaFact]
    public void Layout_StretchAlignment_ArrangedAtDesiredSize_KeepsColumnCount()
    {
        // Arrange
        var panel = CreatePanel(HorizontalAlignment.Stretch, 50, 30, 40);
        panel.DesiredItemWidth = 100;
        panel.HorizontalSpacing = 10;
        panel.VerticalSpacing = 5;

        // Act
        panel.Measure(new Size(230, double.PositiveInfinity));
        panel.Arrange(new Rect(panel.DesiredSize));

        // Assert
        Assert.Equal(new Size(230, 75), panel.DesiredSize);
        AssertSlot(panel.Children[1], 120, 0, 110, 30);
    }

    [StaTheory]
    [InlineData(HorizontalAlignment.Right, 50)]
    [InlineData(HorizontalAlignment.Center, 25)]
    public void Layout_RightOrCenterAlignment_OffsetsColumnsByRemainingWidth(
        HorizontalAlignment alignment,
        double expectedFirstColumnX)
    {
        // Arrange
        var panel = CreatePanel(alignment, 50, 30);
        panel.DesiredItemWidth = 100;

        // Act
        Layout(panel, new Size(250, double.PositiveInfinity));

        // Assert
        AssertSlot(panel.Children[0], expectedFirstColumnX, 0, 100, 50);
        AssertSlot(panel.Children[1], expectedFirstColumnX + 100, 0, 100, 30);
    }

    [StaFact]
    public void Layout_Padding_OffsetsItemsByLeftAndTopPadding()
    {
        // Arrange
        var panel = CreatePanel(HorizontalAlignment.Left, 50);
        panel.DesiredItemWidth = 100;
        panel.Padding = new Thickness(10);

        // Act
        Layout(panel, new Size(120, double.PositiveInfinity));

        // Assert
        AssertSlot(panel.Children[0], 10, 10, 100, 50);
    }

    [StaFact]
    public void Measure_Padding_IsIncludedInDesiredSize()
    {
        // Arrange
        var panel = CreatePanel(HorizontalAlignment.Left, 50);
        panel.DesiredItemWidth = 100;
        panel.Padding = new Thickness(10);

        // Act
        panel.Measure(new Size(120, double.PositiveInfinity));

        // Assert
        Assert.Equal(new Size(120, 70), panel.DesiredSize);
    }

    [StaFact]
    public void Layout_ColumnsThatFitExactlyWithSpacing_AreAllUsed()
    {
        // Arrange
        var panel = CreatePanel(HorizontalAlignment.Stretch, 50, 30);
        panel.DesiredItemWidth = 100;
        panel.HorizontalSpacing = 10;

        // Act
        Layout(panel, new Size(210, double.PositiveInfinity));

        // Assert
        Assert.Equal(50d, panel.DesiredSize.Height);
        AssertSlot(panel.Children[0], 0, 0, 100, 50);
        AssertSlot(panel.Children[1], 110, 0, 100, 30);
    }

    [StaFact]
    public void Measure_InfiniteWidth_DoesNotThrow()
    {
        // Arrange
        var panel = CreatePanel(HorizontalAlignment.Left, 50, 30);
        panel.DesiredItemWidth = 100;

        // Act
        var exception = Record.Exception(() => panel.Measure(new Size(double.PositiveInfinity, 300)));

        // Assert
        Assert.Null(exception);
    }

    private static StaggeredPanel CreatePanel(
        HorizontalAlignment alignment,
        params double[] childHeights)
    {
        var panel = new StaggeredPanel
        {
            HorizontalAlignment = alignment,
        };

        foreach (var height in childHeights)
        {
            panel.Children.Add(new Border { Height = height });
        }

        return panel;
    }

    private static void Layout(
        UIElement panel,
        Size availableSize)
    {
        panel.Measure(availableSize);
        panel.Arrange(new Rect(0, 0, availableSize.Width, panel.DesiredSize.Height));
    }

    private static void AssertSlot(
        UIElement child,
        double x,
        double y,
        double width,
        double height)
        => Assert.Equal(new Rect(x, y, width, height), LayoutInformation.GetLayoutSlot((FrameworkElement)child));
}