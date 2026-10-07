namespace Atc.Wpf.Controls.Tests.Layouts;

public sealed class ReversibleStackPanelTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var panel = new ReversibleStackPanel();

        // Assert
        Assert.False(panel.ReverseOrder);
        Assert.Equal(Orientation.Vertical, panel.Orientation);
    }

    [StaFact]
    public void Layout_EmptyPanel_DesiresZeroSizeAndArrangesWithZeroHeight()
    {
        // Arrange
        var panel = new ReversibleStackPanel { ReverseOrder = true };

        // Act
        panel.Measure(new Size(100, 50));
        panel.Arrange(new Rect(0, 0, 100, 50));

        // Assert
        Assert.Equal(new Size(0, 0), panel.DesiredSize);
        Assert.Equal(new Size(100, 0), panel.RenderSize);
    }

    [StaFact]
    public void Layout_VerticalNotReversed_StacksChildrenInDeclarationOrder()
    {
        // Arrange
        var panel = CreateVerticalPanel(reverseOrder: false);

        // Act
        panel.Measure(new Size(100, double.PositiveInfinity));
        panel.Arrange(new Rect(0, 0, 100, panel.DesiredSize.Height));

        // Assert
        Assert.Equal(new Size(50, 60), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 100, 10);
        AssertSlot(panel.Children[1], 0, 10, 100, 20);
        AssertSlot(panel.Children[2], 0, 30, 100, 30);
    }

    [StaFact]
    public void Layout_VerticalReversed_StacksLastChildFirst()
    {
        // Arrange
        var panel = CreateVerticalPanel(reverseOrder: true);

        // Act
        panel.Measure(new Size(100, double.PositiveInfinity));
        panel.Arrange(new Rect(0, 0, 100, panel.DesiredSize.Height));

        // Assert
        Assert.Equal(new Size(50, 60), panel.DesiredSize);
        AssertSlot(panel.Children[2], 0, 0, 100, 30);
        AssertSlot(panel.Children[1], 0, 30, 100, 20);
        AssertSlot(panel.Children[0], 0, 50, 100, 10);
    }

    [StaFact]
    public void Layout_HorizontalReversed_PlacesLastChildLeftmost()
    {
        // Arrange
        var panel = new ReversibleStackPanel
        {
            Orientation = Orientation.Horizontal,
            ReverseOrder = true,
        };
        panel.Children.Add(new Border { Width = 10, Height = 40 });
        panel.Children.Add(new Border { Width = 20, Height = 40 });
        panel.Children.Add(new Border { Width = 30, Height = 40 });

        // Act
        panel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        panel.Arrange(new Rect(panel.DesiredSize));

        // Assert
        Assert.Equal(new Size(60, 40), panel.DesiredSize);
        AssertSlot(panel.Children[2], 0, 0, 30, 40);
        AssertSlot(panel.Children[1], 30, 0, 20, 40);
        AssertSlot(panel.Children[0], 50, 0, 10, 40);
    }

    [StaFact(Skip = "Bug: ReversibleStackPanel.ReverseOrderProperty uses plain PropertyMetadata without AffectsArrange.")]
    public void ReverseOrder_ChangedAfterLayout_InvalidatesArrange()
    {
        // Arrange
        var panel = CreateVerticalPanel(reverseOrder: false);
        panel.Measure(new Size(100, double.PositiveInfinity));
        panel.Arrange(new Rect(0, 0, 100, panel.DesiredSize.Height));

        // Act
        panel.ReverseOrder = true;

        // Assert
        Assert.False(panel.IsArrangeValid);
    }

    private static ReversibleStackPanel CreateVerticalPanel(bool reverseOrder)
    {
        var panel = new ReversibleStackPanel { ReverseOrder = reverseOrder };
        panel.Children.Add(new Border { Width = 50, Height = 10 });
        panel.Children.Add(new Border { Width = 50, Height = 20 });
        panel.Children.Add(new Border { Width = 50, Height = 30 });
        return panel;
    }

    private static void AssertSlot(
        UIElement child,
        double x,
        double y,
        double width,
        double height)
        => Assert.Equal(new Rect(x, y, width, height), LayoutInformation.GetLayoutSlot((FrameworkElement)child));
}