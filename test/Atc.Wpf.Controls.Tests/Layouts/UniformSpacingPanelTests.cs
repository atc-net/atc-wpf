namespace Atc.Wpf.Controls.Tests.Layouts;

public sealed class UniformSpacingPanelTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var panel = new UniformSpacingPanel();

        // Assert
        Assert.Equal(Orientation.Horizontal, panel.Orientation);
        Assert.Equal(VisualWrappingType.NoWrap, panel.ChildWrapping);
        Assert.True(double.IsNaN(panel.Spacing));
        Assert.True(double.IsNaN(panel.ItemWidth));
        Assert.True(double.IsNaN(panel.ItemHeight));
    }

    [StaTheory]
    [InlineData(-1)]
    [InlineData(-0.5)]
    public void Spacing_Negative_IsRejected(double spacing)
    {
        // Arrange
        var panel = new UniformSpacingPanel();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => panel.Spacing = spacing);
    }

    [StaFact]
    public void Measure_EmptyPanel_DesiresZeroSize()
    {
        // Arrange
        var panel = new UniformSpacingPanel { Spacing = 10 };

        // Act
        panel.Measure(new Size(100, 100));

        // Assert
        Assert.Equal(new Size(0, 0), panel.DesiredSize);
    }

    [StaFact]
    public void Layout_HorizontalNoWrap_PlacesSpacingBetweenChildrenOnly()
    {
        // Arrange
        var panel = CreatePanel(
            new Size(40, 20),
            new Size(30, 30),
            new Size(20, 10));
        panel.Spacing = 10;

        // Act
        Layout(panel, new Size(500, 500));

        // Assert
        Assert.Equal(new Size(110, 30), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 40, 30);
        AssertSlot(panel.Children[1], 50, 0, 30, 30);
        AssertSlot(panel.Children[2], 90, 0, 20, 30);
    }

    [StaFact]
    public void Layout_VerticalNoWrap_UsesVerticalSpacingAlongMainAxis()
    {
        // Arrange
        var panel = CreatePanel(
            new Size(40, 20),
            new Size(30, 30),
            new Size(20, 10));
        panel.Orientation = Orientation.Vertical;
        panel.HorizontalSpacing = 4;
        panel.VerticalSpacing = 8;

        // Act
        panel.Measure(new Size(500, 500));
        panel.Arrange(new Rect(panel.DesiredSize));

        // Assert
        Assert.Equal(new Size(40, 76), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 40, 20);
        AssertSlot(panel.Children[1], 0, 28, 40, 30);
        AssertSlot(panel.Children[2], 0, 66, 40, 10);
    }

    [StaFact]
    public void Layout_HorizontalNoWrap_WithInfiniteSize_KeepsSingleLine()
    {
        // Arrange
        var panel = CreatePanel(
            new Size(40, 20),
            new Size(30, 30));
        panel.Spacing = 5;

        // Act
        panel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        panel.Arrange(new Rect(panel.DesiredSize));

        // Assert
        Assert.Equal(new Size(75, 30), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 40, 30);
        AssertSlot(panel.Children[1], 45, 0, 30, 30);
    }

    [StaFact]
    public void Layout_Wrap_MovesOverflowingChildToNextLineWithVerticalSpacing()
    {
        // Arrange
        var panel = CreatePanel(
            new Size(40, 20),
            new Size(40, 20),
            new Size(40, 20));
        panel.ChildWrapping = VisualWrappingType.Wrap;
        panel.HorizontalSpacing = 4;
        panel.VerticalSpacing = 8;

        // Act
        Layout(panel, new Size(100, 500));

        // Assert
        Assert.Equal(new Size(84, 48), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 40, 20);
        AssertSlot(panel.Children[1], 44, 0, 40, 20);
        AssertSlot(panel.Children[2], 0, 28, 40, 20);
    }

    [StaFact]
    public void Layout_Wrap_ChildrenFittingExactlyStayOnOneLine()
    {
        // Arrange
        var panel = CreatePanel(
            new Size(45, 20),
            new Size(45, 20));
        panel.ChildWrapping = VisualWrappingType.Wrap;
        panel.Spacing = 10;

        // Act
        Layout(panel, new Size(100, 500));

        // Assert
        Assert.Equal(new Size(100, 20), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 45, 20);
        AssertSlot(panel.Children[1], 55, 0, 45, 20);
    }

    [StaFact]
    public void Layout_Wrap_WithInfiniteWidth_NeverWraps()
    {
        // Arrange
        var panel = CreatePanel(
            new Size(40, 20),
            new Size(40, 20),
            new Size(40, 20));
        panel.ChildWrapping = VisualWrappingType.Wrap;
        panel.Spacing = 10;

        // Act
        panel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        panel.Arrange(new Rect(panel.DesiredSize));

        // Assert
        Assert.Equal(new Size(140, 20), panel.DesiredSize);
        AssertSlot(panel.Children[2], 100, 0, 40, 20);
    }

    [StaFact]
    public void Layout_ItemWidth_OverridesChildDesiredWidth()
    {
        // Arrange
        var panel = CreatePanel(
            new Size(10, 20),
            new Size(30, 20));
        panel.ItemWidth = 50;
        panel.Spacing = 5;

        // Act
        Layout(panel, new Size(500, 500));

        // Assert
        Assert.Equal(new Size(105, 20), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 50, 20);
        AssertSlot(panel.Children[1], 55, 0, 50, 20);
    }

    [StaFact]
    public void Layout_Wrap_FirstChildNearlyFillingWidth_DoesNotAddSpacingToHeight()
    {
        // Arrange
        var panel = CreatePanel(new Size(95, 20));
        panel.ChildWrapping = VisualWrappingType.Wrap;
        panel.Spacing = 10;

        // Act
        Layout(panel, new Size(100, 500));

        // Assert
        Assert.Equal(new Size(95, 20), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 95, 20);
    }

    [StaFact]
    public void Measure_NoWrap_CollapsedChild_DoesNotContributeSpacing()
    {
        // Arrange
        var panel = CreatePanel(
            new Size(40, 20),
            new Size(30, 20),
            new Size(20, 20));
        panel.Children[1].Visibility = Visibility.Collapsed;
        panel.Spacing = 10;

        // Act
        Layout(panel, new Size(500, 500));

        // Assert
        AssertSlot(panel.Children[2], 50, 0, 20, 20);
        Assert.Equal(new Size(70, 20), panel.DesiredSize);
    }

    private static UniformSpacingPanel CreatePanel(params Size[] childSizes)
    {
        var panel = new UniformSpacingPanel();

        foreach (var size in childSizes)
        {
            panel.Children.Add(new Border { Width = size.Width, Height = size.Height });
        }

        return panel;
    }

    private static void Layout(
        UIElement panel,
        Size availableSize)
    {
        panel.Measure(availableSize);
        panel.Arrange(new Rect(new Size(availableSize.Width, panel.DesiredSize.Height)));
    }

    private static void AssertSlot(
        UIElement child,
        double x,
        double y,
        double width,
        double height)
        => Assert.Equal(new Rect(x, y, width, height), LayoutInformation.GetLayoutSlot((FrameworkElement)child));
}