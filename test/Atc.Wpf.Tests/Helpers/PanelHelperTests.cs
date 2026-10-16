namespace Atc.Wpf.Tests.Helpers;

public sealed class PanelHelperTests
{
    [StaFact]
    public void Spacing_AppliesItemMarginToAllButLastChild()
    {
        // Arrange
        var panel = CreatePanelWithChildren(3);

        // Act
        PanelHelper.SetSpacing(panel, 10);
        RaiseLoaded(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 10, 10), ((FrameworkElement)panel.Children[0]).Margin);
        Assert.Equal(new Thickness(0, 0, 10, 10), ((FrameworkElement)panel.Children[1]).Margin);
        Assert.Equal(new Thickness(0), ((FrameworkElement)panel.Children[2]).Margin);
    }

    [StaFact]
    public void HorizontalSpacing_AppliesRightMarginToAllButLastChild()
    {
        // Arrange
        var panel = CreatePanelWithChildren(2);

        // Act
        PanelHelper.SetHorizontalSpacing(panel, 15);
        RaiseLoaded(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 15, 0), ((FrameworkElement)panel.Children[0]).Margin);
        Assert.Equal(new Thickness(0), ((FrameworkElement)panel.Children[1]).Margin);
    }

    [StaFact]
    public void VerticalSpacing_AppliesBottomMarginToAllButLastChild()
    {
        // Arrange
        var panel = CreatePanelWithChildren(2);

        // Act
        PanelHelper.SetVerticalSpacing(panel, 20);
        RaiseLoaded(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 0, 20), ((FrameworkElement)panel.Children[0]).Margin);
        Assert.Equal(new Thickness(0), ((FrameworkElement)panel.Children[1]).Margin);
    }

    [StaFact]
    public void Spacing_SingleChild_GetsNoTrailingMargin()
    {
        // Arrange
        var panel = CreatePanelWithChildren(1);

        // Act
        PanelHelper.SetSpacing(panel, 10);
        RaiseLoaded(panel);

        // Assert
        Assert.Equal(new Thickness(0), ((FrameworkElement)panel.Children[0]).Margin);
    }

    private static StackPanel CreatePanelWithChildren(int count)
    {
        var panel = new StackPanel();
        for (var i = 0; i < count; i++)
        {
            panel.Children.Add(new Button { Content = i });
        }

        return panel;
    }

    private static void RaiseLoaded(FrameworkElement element)
        => element.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
}