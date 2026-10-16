namespace Atc.Wpf.Controls.Tests.Layouts;

public sealed class VirtualizingStaggeredPanelTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    private const string ItemsControlTemplateXaml =
        "<ControlTemplate " +
        "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
        "TargetType=\"ItemsControl\">" +
        "<ItemsPresenter />" +
        "</ControlTemplate>";

    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var panel = new VirtualizingStaggeredPanel();

        // Assert
        Assert.Equal(250d, panel.DesiredItemWidth);
        Assert.Equal(0d, panel.HorizontalSpacing);
        Assert.Equal(0d, panel.VerticalSpacing);
        Assert.Equal(default, panel.Padding);
    }

    #region Standalone (non-virtualized) Tests

    [StaFact]
    public void Measure_Standalone_EmptyPanel_DesiresZeroSize()
    {
        // Arrange
        var panel = new VirtualizingStaggeredPanel();

        // Act
        panel.Measure(new Size(300, 300));

        // Assert
        Assert.Equal(new Size(0, 0), panel.DesiredSize);
    }

    [StaFact]
    public void Layout_Standalone_ColumnCountDerivedFromWidth_PlacesItemsInShortestColumn()
    {
        // Arrange
        var panel = CreateStandalonePanel(HorizontalAlignment.Left, 50, 30, 40, 20);
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
    public void Layout_Standalone_ColumnsThatFitExactlyWithSpacing_AreAllUsed()
    {
        // Arrange
        var panel = CreateStandalonePanel(HorizontalAlignment.Stretch, 50, 30, 40);
        panel.DesiredItemWidth = 100;
        panel.HorizontalSpacing = 10;
        panel.VerticalSpacing = 5;

        // Act
        Layout(panel, new Size(210, double.PositiveInfinity));

        // Assert
        Assert.Equal(new Size(210, 75), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 100, 50);
        AssertSlot(panel.Children[1], 110, 0, 100, 30);
        AssertSlot(panel.Children[2], 110, 35, 100, 40);
    }

    [StaFact]
    public void Layout_Standalone_WidthBelowDesiredItemWidth_UsesSingleColumnOfAvailableWidth()
    {
        // Arrange
        var panel = CreateStandalonePanel(HorizontalAlignment.Left, 50, 30);
        panel.DesiredItemWidth = 100;
        panel.VerticalSpacing = 5;

        // Act
        Layout(panel, new Size(80, double.PositiveInfinity));

        // Assert
        Assert.Equal(new Size(80, 85), panel.DesiredSize);
        AssertSlot(panel.Children[0], 0, 0, 80, 50);
        AssertSlot(panel.Children[1], 0, 55, 80, 30);
    }

    [StaFact]
    public void Layout_Standalone_Padding_IsAppliedToOffsetsAndDesiredHeight()
    {
        // Arrange
        var panel = CreateStandalonePanel(HorizontalAlignment.Left, 50);
        panel.DesiredItemWidth = 100;
        panel.Padding = new Thickness(10);

        // Act
        Layout(panel, new Size(120, double.PositiveInfinity));

        // Assert
        Assert.Equal(new Size(120, 70), panel.DesiredSize);
        AssertSlot(panel.Children[0], 10, 10, 100, 50);
    }

    [StaTheory]
    [InlineData(HorizontalAlignment.Right, 50)]
    [InlineData(HorizontalAlignment.Center, 25)]
    public void Layout_Standalone_RightOrCenterAlignment_OffsetsColumnsByRemainingWidth(
        HorizontalAlignment alignment,
        double expectedFirstColumnX)
    {
        // Arrange
        var panel = CreateStandalonePanel(alignment, 50, 30);
        panel.DesiredItemWidth = 100;

        // Act
        Layout(panel, new Size(250, double.PositiveInfinity));

        // Assert
        AssertSlot(panel.Children[0], expectedFirstColumnX, 0, 100, 50);
        AssertSlot(panel.Children[1], expectedFirstColumnX + 100, 0, 100, 30);
    }

    #endregion

    #region ItemsControl-hosted (virtualized) Tests

    [StaFact]
    public void Measure_Hosted_NoItems_DesiresZeroSize()
    {
        // Arrange
        var (itemsControl, panel) = CreateHosted();

        // Act
        itemsControl.Measure(new Size(100, 250));

        // Assert
        Assert.Equal(new Size(0, 0), panel.DesiredSize);
        Assert.Equal(0d, panel.ExtentHeight);
    }

    [StaFact]
    public void Measure_Hosted_FirstPass_RealizesItems()
    {
        // Arrange
        var (itemsControl, panel) = CreateHosted(100, 100, 100);

        // Act
        itemsControl.Measure(new Size(100, 250));

        // Assert
        Assert.Equal(3, panel.Children.Count);
        Assert.Equal(300d, panel.ExtentHeight);
    }

    [StaFact]
    public void Measure_Hosted_SecondPass_RealizesOnlyItemsInViewport()
    {
        // Arrange
        var (itemsControl, panel) = CreateHosted(100, 100, 100, 100, 100, 100, 100, 100, 100, 100);

        // Act
        LayoutHosted(itemsControl, panel, new Size(100, 250));

        // Assert
        Assert.Equal(3, panel.Children.Count);
        Assert.Same(itemsControl.Items[0], panel.Children[0]);
        Assert.Same(itemsControl.Items[2], panel.Children[2]);
        Assert.Equal(1000d, panel.ExtentHeight);
        Assert.Equal(250d, panel.ViewportHeight);
    }

    [StaFact]
    public void SetVerticalOffset_Hosted_RealizesScrolledInItemsAndShiftsSlots()
    {
        // Arrange
        var (itemsControl, panel) = CreateHosted(100, 100, 100, 100, 100, 100, 100, 100, 100, 100);
        LayoutHosted(itemsControl, panel, new Size(100, 250));

        // Act
        panel.SetVerticalOffset(400);
        RelayoutPanel(panel, new Size(100, 250));

        // Assert
        Assert.Equal(400d, panel.VerticalOffset);
        Assert.Equal(4, panel.Children.Count);
        Assert.Same(itemsControl.Items[3], panel.Children[0]);
        Assert.Same(itemsControl.Items[6], panel.Children[3]);
        AssertSlot((UIElement)itemsControl.Items[4], 0, 0, 100, 100);
    }

    [StaFact]
    public void SetVerticalOffset_Hosted_NonContiguousVisibleSet_RealizesRangeAndCachesHeightsPerItem()
    {
        // Arrange: two columns, so a tall item in one column stays visible while the
        // following short items in the other column have already scrolled out.
        // Column 0: item0 0-300, item4 300-600. Column 1: item1 0-50, item2 50-100,
        // item3 100-400, item5 400-450, item6 450-500, item7 500-800.
        var (itemsControl, panel) = CreateHosted(300, 50, 50, 300, 300, 50, 50, 300);
        LayoutHosted(itemsControl, panel, new Size(200, 350));

        // Act: viewport 120-470 shows items 0, 3, 4, 5 and 6 but not 1, 2 and 7.
        panel.SetVerticalOffset(120);
        RelayoutPanel(panel, new Size(200, 350));

        // Assert: the whole range 0..6 is realized in item order, 7 is virtualized away.
        Assert.Equal(120d, panel.VerticalOffset);
        Assert.Equal(800d, panel.ExtentHeight);
        Assert.Equal(7, panel.Children.Count);
        for (var i = 0; i <= 6; i++)
        {
            Assert.Same(itemsControl.Items[i], panel.Children[i]);
        }

        AssertSlot((UIElement)itemsControl.Items[0], 0, -120, 100, 300);
        AssertSlot((UIElement)itemsControl.Items[1], 100, -120, 100, 50);
        AssertSlot((UIElement)itemsControl.Items[3], 100, -20, 100, 300);
        AssertSlot((UIElement)itemsControl.Items[4], 0, 180, 100, 300);
        AssertSlot((UIElement)itemsControl.Items[5], 100, 280, 100, 50);
        AssertSlot((UIElement)itemsControl.Items[6], 100, 330, 100, 50);

        var expectedHeights = new[] { 300d, 50d, 50d, 300d, 300d, 50d, 50d, 300d };
        for (var i = 0; i < expectedHeights.Length; i++)
        {
            Assert.True(panel.CachedItemHeights.TryGetValue(i, out var cachedHeight));
            Assert.Equal(expectedHeights[i], cachedHeight);
        }
    }

    [StaFact]
    public void SetVerticalOffset_Hosted_BeyondExtent_IsClampedToLastPage()
    {
        // Arrange
        var (itemsControl, panel) = CreateHosted(100, 100, 100, 100, 100, 100, 100, 100, 100, 100);
        LayoutHosted(itemsControl, panel, new Size(100, 250));

        // Act
        panel.SetVerticalOffset(5000);

        // Assert
        Assert.Equal(750d, panel.VerticalOffset);
    }

    [StaFact]
    public void Layout_Hosted_TwoColumns_PlacesItemsInShortestColumnWithSpacing()
    {
        // Arrange
        var (itemsControl, panel) = CreateHosted(50, 30, 40);
        panel.DesiredItemWidth = 100;
        panel.HorizontalSpacing = 10;
        panel.VerticalSpacing = 5;

        // Act
        LayoutHosted(itemsControl, panel, new Size(210, 500));

        // Assert
        Assert.Equal(3, panel.Children.Count);
        Assert.Equal(75d, panel.ExtentHeight);
        AssertSlot((UIElement)itemsControl.Items[0], 0, 0, 100, 50);
        AssertSlot((UIElement)itemsControl.Items[1], 110, 0, 100, 30);
        AssertSlot((UIElement)itemsControl.Items[2], 110, 35, 100, 40);
    }

    [StaFact]
    public void Measure_Hosted_InfiniteHeight_DoesNotThrow()
    {
        // Arrange
        var (itemsControl, panel) = CreateHosted(50, 30);
        var availableSize = new Size(300, double.PositiveInfinity);
        itemsControl.Measure(availableSize);

        // Act
        var exception = Record.Exception(() =>
        {
            panel.InvalidateMeasure();
            panel.Measure(availableSize);
        });

        // Assert
        Assert.Null(exception);
    }

    [StaFact]
    public void Measure_Standalone_InfiniteWidth_GivesEachChildAColumn()
    {
        // Arrange
        var panel = CreateStandalonePanel(HorizontalAlignment.Left, 50, 30);
        panel.DesiredItemWidth = 100;
        panel.HorizontalSpacing = 10;

        // Act
        panel.Measure(new Size(double.PositiveInfinity, 300));

        // Assert
        Assert.Equal(new Size(210, 50), panel.DesiredSize);
    }

    [StaFact]
    public void Measure_Hosted_InfiniteWidth_DoesNotThrow()
    {
        // Arrange
        var (itemsControl, panel) = CreateHosted(50, 30);
        panel.DesiredItemWidth = 100;

        // Act
        var exception = Record.Exception(() => itemsControl.Measure(new Size(double.PositiveInfinity, 300)));

        // Assert
        Assert.Null(exception);
        Assert.False(double.IsInfinity(panel.DesiredSize.Width));
    }

    #endregion

    private static VirtualizingStaggeredPanel CreateStandalonePanel(
        HorizontalAlignment alignment,
        params double[] childHeights)
    {
        var panel = new VirtualizingStaggeredPanel
        {
            HorizontalAlignment = alignment,
        };

        foreach (var height in childHeights)
        {
            panel.Children.Add(new Border { Height = height });
        }

        return panel;
    }

    private static (ItemsControl ItemsControl, VirtualizingStaggeredPanel Panel) CreateHosted(
        params double[] itemHeights)
    {
        var panelFactory = new FrameworkElementFactory(typeof(VirtualizingStaggeredPanel));
        panelFactory.SetValue(VirtualizingStaggeredPanel.DesiredItemWidthProperty, 100d);

        var itemsControl = new ItemsControl
        {
            Template = (ControlTemplate)XamlReader.Parse(ItemsControlTemplateXaml),
            ItemsPanel = new ItemsPanelTemplate(panelFactory),
        };

        foreach (var height in itemHeights)
        {
            itemsControl.Items.Add(new Border { Height = height });
        }

        itemsControl.ApplyTemplate();
        var itemsPresenter = (ItemsPresenter)VisualTreeHelper.GetChild(itemsControl, 0);
        itemsPresenter.ApplyTemplate();
        var panel = (VirtualizingStaggeredPanel)VisualTreeHelper.GetChild(itemsPresenter, 0);

        return (itemsControl, panel);
    }

    private static void LayoutHosted(
        ItemsControl itemsControl,
        VirtualizingStaggeredPanel panel,
        Size size)
    {
        // Workaround for the bug covered by Measure_Hosted_FirstPass_RealizesItems: the first
        // ItemsControl pass realizes nothing because the panel's generator is still null.
        // The next pass realizes every item to learn its height, and the last pass virtualizes.
        itemsControl.Measure(size);
        itemsControl.Arrange(new Rect(size));
        RelayoutPanel(panel, size);
        RelayoutPanel(panel, size);
    }

    private static void RelayoutPanel(
        VirtualizingStaggeredPanel panel,
        Size size)
    {
        panel.InvalidateMeasure();
        panel.Measure(size);
        panel.Arrange(new Rect(size));
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