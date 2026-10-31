namespace Atc.Wpf.Tests.Helpers;

public sealed class PanelHelperTests
{
    [StaFact]
    public void Spacing_VerticalStackPanel_AppliesBottomMarginToAllButLastChild()
    {
        // Arrange
        var panel = CreatePanelWithChildren(3);

        // Act
        PanelHelper.SetSpacing(panel, 10);
        RaiseLoaded(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 0, 10), ((FrameworkElement)panel.Children[0]).Margin);
        Assert.Equal(new Thickness(0, 0, 0, 10), ((FrameworkElement)panel.Children[1]).Margin);
        Assert.Equal(new Thickness(0), ((FrameworkElement)panel.Children[2]).Margin);
    }

    [StaFact]
    public void HorizontalSpacing_AppliesRightMarginToAllButLastChild()
    {
        // Arrange
        var panel = CreatePanelWithChildren(2);
        panel.Orientation = Orientation.Horizontal;

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
        Assert.Equal(new Thickness(0, 0, 0, 20), ((FrameworkElement)panel.Children[0]).Margin);
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
        Assert.Equal(new Thickness(0, 0, 0, 10), ((FrameworkElement)panel.Children[0]).Margin);
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
        Assert.Equal(new Thickness(0, 0, 0, 10), ((FrameworkElement)panel.Children[0]).Margin);
        Assert.Equal(new Thickness(0, 0, 0, 10), ((FrameworkElement)panel.Children[1]).Margin);
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

    [StaFact]
    public void Spacing_HorizontalStackPanel_GivesTheButtonsEqualHeights()
    {
        // Arrange
        var panel = CreatePanelWithChildren(3);
        panel.Orientation = Orientation.Horizontal;
        panel.Height = 40;
        PanelHelper.SetSpacing(panel, 8);

        // Act
        RaiseLoaded(panel);
        Layout(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 8, 0), ((FrameworkElement)panel.Children[0]).Margin);
        Assert.All(panel.Children.Cast<FrameworkElement>(), x => Assert.Equal(40, x.ActualHeight));
    }

    [StaFact]
    public void Spacing_OrientationSetAfterSpacing_UsesTheOrientationAtLoad()
    {
        // Arrange
        var panel = CreatePanelWithChildren(2);
        PanelHelper.SetSpacing(panel, 10);

        // Act
        panel.Orientation = Orientation.Horizontal;
        RaiseLoaded(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 10, 0), ((FrameworkElement)panel.Children[0]).Margin);
    }

    [StaFact]
    public void Spacing_OrientationChangedAfterLoaded_IsReapplied()
    {
        // Arrange
        var panel = CreatePanelWithChildren(2);
        PanelHelper.SetSpacing(panel, 10);
        RaiseLoaded(panel);
        Layout(panel);

        // Act
        panel.Orientation = Orientation.Horizontal;
        Layout(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 10, 0), ((FrameworkElement)panel.Children[0]).Margin);
    }

    [StaFact]
    public void Spacing_CollapsedLastChild_TheLastShownChildGetsNoTrailingMargin()
    {
        // Arrange
        var panel = CreatePanelWithChildren(3);
        ((FrameworkElement)panel.Children[2]).Visibility = Visibility.Collapsed;

        // Act
        PanelHelper.SetSpacing(panel, 10);
        RaiseLoaded(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 0, 10), ((FrameworkElement)panel.Children[0]).Margin);
        Assert.Equal(new Thickness(0), ((FrameworkElement)panel.Children[1]).Margin);
    }

    [StaFact]
    public void Spacing_LastChildShownAfterLoaded_IsReapplied()
    {
        // Arrange
        var panel = CreatePanelWithChildren(3);
        var last = (FrameworkElement)panel.Children[2];
        last.Visibility = Visibility.Collapsed;
        PanelHelper.SetSpacing(panel, 10);
        RaiseLoaded(panel);
        Layout(panel);

        // Act
        last.Visibility = Visibility.Visible;
        Layout(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 0, 10), ((FrameworkElement)panel.Children[1]).Margin);
        Assert.Equal(new Thickness(0), last.Margin);
    }

    [StaFact]
    public void Spacing_DockPanel_PutsTheGapOnTheSideFacingTheRemainingSpace()
    {
        // Arrange
        var left = new Button();
        var top = new Button();
        var right = new Button();
        var bottom = new Button();
        var fill = new Button();
        DockPanel.SetDock(left, Dock.Left);
        DockPanel.SetDock(top, Dock.Top);
        DockPanel.SetDock(right, Dock.Right);
        DockPanel.SetDock(bottom, Dock.Bottom);
        var panel = new DockPanel { Children = { left, top, right, bottom, fill } };

        // Act
        PanelHelper.SetSpacing(panel, 6);
        RaiseLoaded(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 6, 0), left.Margin);
        Assert.Equal(new Thickness(0, 0, 0, 6), top.Margin);
        Assert.Equal(new Thickness(6, 0, 0, 0), right.Margin);
        Assert.Equal(new Thickness(0, 6, 0, 0), bottom.Margin);
        Assert.Equal(new Thickness(0), fill.Margin);
    }

    [StaFact]
    public void Spacing_WrapPanel_KeepsTheGapOnBothAxes()
    {
        // Arrange
        var panel = new WrapPanel { Children = { new Button(), new Button() } };

        // Act
        PanelHelper.SetSpacing(panel, 10);
        RaiseLoaded(panel);

        // Assert
        Assert.Equal(new Thickness(0, 0, 10, 10), ((FrameworkElement)panel.Children[0]).Margin);
    }

    private static void Layout(FrameworkElement element)
    {
        element.Measure(new Size(200, 200));
        element.Arrange(new Rect(0, 0, 200, 200));
        element.UpdateLayout();
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