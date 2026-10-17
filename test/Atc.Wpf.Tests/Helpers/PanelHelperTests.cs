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

    [StaFact]
    public void Spacing_ChangedAfterLoaded_IsReapplied()
    {
        // Arrange
        var panel = CreatePanelWithChildren(2);
        PanelHelper.SetSpacing(panel, 10);
        RaiseLoaded(panel);

        // Act
        PanelHelper.SetSpacing(panel, 20);
        RaiseLoaded(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 20, 20), ((FrameworkElement)panel.Children[0]).Margin);
        Assert.Equal(new Thickness(0), ((FrameworkElement)panel.Children[1]).Margin);
    }

    [StaFact]
    public void Spacing_UserMargin_IsPreserved()
    {
        // Arrange
        var panel = CreatePanelWithChildren(2);
        ((FrameworkElement)panel.Children[0]).Margin = new Thickness(5);

        // Act
        PanelHelper.SetSpacing(panel, 10);
        RaiseLoaded(panel);

        // Assert
        Assert.Equal(new Thickness(5), ((FrameworkElement)panel.Children[0]).Margin);
        Assert.Equal(new Thickness(0), ((FrameworkElement)panel.Children[1]).Margin);
    }

    [StaFact]
    public void Spacing_StyleMargin_IsReplaced()
    {
        // Arrange
        var style = new Style(typeof(Button));
        style.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(7)));
        var panel = CreatePanelWithChildren(2);
        foreach (Button child in panel.Children)
        {
            child.Style = style;
        }

        // Act
        PanelHelper.SetSpacing(panel, 10);
        RaiseLoaded(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 10, 10), ((FrameworkElement)panel.Children[0]).Margin);
        Assert.Equal(new Thickness(0), ((FrameworkElement)panel.Children[1]).Margin);
    }

    [StaFact]
    public void Spacing_ChildAddedAfterLoaded_GetsSpacing()
    {
        // Arrange
        var panel = CreatePanelWithChildren(2);
        PanelHelper.SetSpacing(panel, 10);
        RaiseLoaded(panel);

        // Act
        var added = new Button { Content = "added" };
        panel.Children.Add(added);
        panel.Measure(new Size(200, 200));
        panel.Arrange(new Rect(0, 0, 200, 200));
        panel.UpdateLayout();

        // Assert
        Assert.Equal(new Thickness(0, 0, 10, 10), ((FrameworkElement)panel.Children[0]).Margin);
        Assert.Equal(new Thickness(0, 0, 10, 10), ((FrameworkElement)panel.Children[1]).Margin);
        Assert.Equal(new Thickness(0), added.Margin);
    }

    [StaFact]
    public void Spacing_ChildAddedAfterUnloaded_IsNotTouched()
    {
        // Arrange
        var panel = CreatePanelWithChildren(2);
        PanelHelper.SetSpacing(panel, 10);
        RaiseLoaded(panel);
        panel.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));

        // Act
        panel.Children.Add(new Button { Content = "added" });
        panel.Measure(new Size(200, 200));
        panel.Arrange(new Rect(0, 0, 200, 200));
        panel.UpdateLayout();

        // Assert - the previous last child keeps its trailing (zero) margin, proving no handler ran
        Assert.Equal(new Thickness(0), ((FrameworkElement)panel.Children[1]).Margin);
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