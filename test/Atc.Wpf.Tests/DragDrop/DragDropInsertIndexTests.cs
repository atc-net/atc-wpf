namespace Atc.Wpf.Tests.DragDrop;

[Collection(nameof(DragDropTestGroup))]
public sealed class DragDropInsertIndexTests
{
    private const string FixedSizeItemStyleXaml = """
        <Style xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" TargetType="ListBoxItem">
            <Setter Property="Width" Value="60" />
            <Setter Property="Height" Value="24" />
            <Setter Property="Margin" Value="0,0,10,0" />
        </Style>
        """;

    [StaFact]
    public void GetInsertIndex_VerticalList_UsesTheHalfOfTheItemUnderThePointer()
    {
        // Arrange
        var listBox = CreateListBox(Letters(5));
        using var host = new DragDropTestWindow(listBox);

        // Act
        var upperHalf = DragDropHelper.GetInsertIndex(listBox, PointIn(listBox, "b", 0.5, 0.25));
        var lowerHalf = DragDropHelper.GetInsertIndex(listBox, PointIn(listBox, "b", 0.5, 0.75));

        // Assert
        Assert.Equal(1, upperHalf);
        Assert.Equal(2, lowerHalf);
    }

    [StaFact]
    public void GetInsertIndex_BelowTheLastItem_AppendsAtTheEnd()
    {
        // Arrange
        var listBox = CreateListBox(Letters(3));
        using var host = new DragDropTestWindow(listBox);

        // Act
        var index = DragDropHelper.GetInsertIndex(listBox, new Point(20, listBox.ActualHeight - 5));

        // Assert
        Assert.Equal(3, index);
    }

    [StaFact]
    public void GetInsertIndex_HorizontalStackPanel_UsesTheHorizontalHalf()
    {
        // Arrange
        var listBox = CreateListBox(Letters(4), PanelTemplate("StackPanel", "Orientation=\"Horizontal\""));
        using var host = new DragDropTestWindow(listBox, width: 500);

        // Act
        var leftHalf = DragDropHelper.GetInsertIndex(listBox, PointIn(listBox, "b", 0.25, 0.75));
        var rightHalf = DragDropHelper.GetInsertIndex(listBox, PointIn(listBox, "b", 0.75, 0.25));

        // Assert
        Assert.Equal(1, leftHalf);
        Assert.Equal(2, rightHalf);
    }

    [StaFact]
    public void GetInsertIndex_GapBetweenHorizontalItems_UsesTheNearestItem()
    {
        // Arrange
        var listBox = CreateListBox(Letters(4), PanelTemplate("StackPanel", "Orientation=\"Horizontal\""));
        using var host = new DragDropTestWindow(listBox, width: 500);
        var rightEdgeOfA = PointIn(listBox, "a", 1, 0.5);

        // Act
        var index = DragDropHelper.GetInsertIndex(listBox, new Point(rightEdgeOfA.X + 3, rightEdgeOfA.Y));

        // Assert
        Assert.Equal(1, index);
    }

    [StaFact]
    public void GetInsertIndex_WrapPanel_UsesTheItemInTheSecondRow()
    {
        // Arrange
        var listBox = CreateListBox(Letters(6), PanelTemplate("WrapPanel", string.Empty));
        ScrollViewer.SetHorizontalScrollBarVisibility(listBox, ScrollBarVisibility.Disabled);
        using var host = new DragDropTestWindow(listBox, width: 200);
        var containerA = Container(listBox, "a");
        var firstInSecondRow = listBox.Items
            .Cast<string>()
            .First(x => Container(listBox, x).TranslatePoint(default, containerA).Y > 1);
        var expectedIndex = listBox.Items.IndexOf(firstInSecondRow);

        // Act
        var leftHalf = DragDropHelper.GetInsertIndex(listBox, PointIn(listBox, firstInSecondRow, 0.25, 0.5));
        var rightHalf = DragDropHelper.GetInsertIndex(listBox, PointIn(listBox, firstInSecondRow, 0.75, 0.5));

        // Assert
        Assert.True(expectedIndex > 0);
        Assert.Equal(expectedIndex, leftHalf);
        Assert.Equal(expectedIndex + 1, rightHalf);
    }

    [StaFact]
    public void GetInsertIndex_RightToLeftHorizontalList_UsesTheLogicalHalf()
    {
        // Arrange
        var listBox = CreateListBox(Letters(4), PanelTemplate("StackPanel", "Orientation=\"Horizontal\""));
        listBox.FlowDirection = FlowDirection.RightToLeft;
        using var host = new DragDropTestWindow(listBox, width: 500);
        var leadingQuarter = PointIn(listBox, "b", 0.25, 0.5);
        var trailingQuarter = PointIn(listBox, "b", 0.75, 0.5);

        // Act
        var leading = DragDropHelper.GetInsertIndex(listBox, leadingQuarter);
        var trailing = DragDropHelper.GetInsertIndex(listBox, trailingQuarter);

        // Assert
        Assert.Equal(1, leading);
        Assert.Equal(2, trailing);
    }

    [StaFact]
    public void GetInsertIndex_GroupedList_ReturnsTheIndexInTheShownItems()
    {
        // Arrange
        var items = new ObservableCollection<string> { "apple", "avocado", "banana", "blueberry", "cherry" };
        var listBox = CreateListBox(items);
        listBox.GroupStyle.Add(new GroupStyle());
        listBox.Items.GroupDescriptions.Add(new PropertyGroupDescription(propertyName: null, new FirstLetterConverter()));
        using var host = new DragDropTestWindow(listBox, height: 400);

        // Act
        var index = DragDropHelper.GetInsertIndex(listBox, PointIn(listBox, "blueberry", 0.5, 0.75));

        // Assert
        Assert.Equal(listBox.Items.IndexOf("blueberry") + 1, index);
    }

    [StaFact]
    public void GetInsertIndex_VirtualizedLongList_ScrolledToTheMiddle_FindsTheVisibleItem()
    {
        // Arrange
        var items = new ObservableCollection<string>(Enumerable.Range(0, 20_000).Select(i => $"item {i}"));
        var listBox = CreateListBox(items);
        listBox.Height = 200;
        using var host = new DragDropTestWindow(listBox);
        listBox.ScrollIntoView(items[10_000]);
        listBox.UpdateLayout();

        // Act
        var index = DragDropHelper.GetInsertIndex(listBox, PointIn(listBox, items[10_000], 0.5, 0.25));

        // Assert
        Assert.Equal(10_000, index);
    }

    [StaFact]
    public void CreateDropInfo_FilteredTarget_MapsTheShownSlotToTheSourceCollection()
    {
        // Arrange
        var items = Letters(5);
        var listBox = CreateListBox(items);
        listBox.Items.Filter = x => x is not ("b" or "d");
        using var host = new DragDropTestWindow(listBox);
        var data = new DataObject("x");

        // Act
        var beforeC = DragDropManager.CreateDropInfo(listBox, data, PointIn(listBox, "c", 0.5, 0.25));
        var afterE = DragDropManager.CreateDropInfo(listBox, data, PointIn(listBox, "e", 0.5, 0.75));
        var beforeA = DragDropManager.CreateDropInfo(listBox, data, PointIn(listBox, "a", 0.5, 0.25));

        // Assert
        Assert.Equal(2, beforeC.InsertIndex);
        Assert.Equal(5, afterE.InsertIndex);
        Assert.Equal(0, beforeA.InsertIndex);
    }

    [StaFact]
    public void ToSourceInsertIndex_SortedView_InsertsBeforeTheShownNeighbour()
    {
        // Arrange
        var items = new ObservableCollection<string> { "c", "a", "b" };
        var listBox = new ListBox { ItemsSource = items };
        listBox.Items.SortDescriptions.Add(new SortDescription(propertyName: string.Empty, ListSortDirection.Ascending));

        // Act
        var beforeB = DragDropHelper.ToSourceInsertIndex(listBox, items, viewInsertIndex: 1);
        var afterC = DragDropHelper.ToSourceInsertIndex(listBox, items, viewInsertIndex: 3);

        // Assert
        Assert.Equal("a,b,c", string.Join(",", listBox.Items.Cast<string>()));
        Assert.Equal(2, beforeB);
        Assert.Equal(1, afterC);
    }

    [StaFact]
    public void ToSourceInsertIndex_EmptyView_AppendsAtTheEnd()
    {
        // Arrange
        var items = Letters(3);
        var listBox = new ListBox { ItemsSource = items };
        listBox.Items.Filter = _ => false;

        // Act
        var index = DragDropHelper.ToSourceInsertIndex(listBox, items, viewInsertIndex: 0);

        // Assert
        Assert.Equal(3, index);
    }

    [StaFact]
    public void GetDropIndicatorLine_HorizontalList_IsAVerticalLine()
    {
        // Arrange
        var listBox = CreateListBox(Letters(3), PanelTemplate("StackPanel", "Orientation=\"Horizontal\""));
        using var host = new DragDropTestWindow(listBox, width: 500);

        // Act
        var line = DragDropHelper.GetDropIndicatorLine(listBox, dropIndex: 1);

        // Assert
        Assert.NotNull(line);
        Assert.Equal(line.Value.Start.X, line.Value.End.X, precision: 3);
        Assert.Equal(PointIn(listBox, "b", 0, 0).X, line.Value.Start.X, precision: 3);
    }

    private static ObservableCollection<string> Letters(int count)
        => new(Enumerable.Range(0, count).Select(i => ((char)('a' + i)).ToString()));

    private static ListBox CreateListBox(
        ObservableCollection<string> items,
        ItemsPanelTemplate? panel = null)
    {
        var listBox = new ListBox
        {
            ItemsSource = items,
            ItemContainerStyle = (Style)XamlReader.Parse(FixedSizeItemStyleXaml),
        };

        if (panel is not null)
        {
            listBox.ItemsPanel = panel;
        }

        return listBox;
    }

    private static ItemsPanelTemplate PanelTemplate(
        string panel,
        string attributes)
        => (ItemsPanelTemplate)XamlReader.Parse(
            $"""<ItemsPanelTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"><{panel} {attributes} /></ItemsPanelTemplate>""");

    private static FrameworkElement Container(
        ListBox listBox,
        string item)
        => (FrameworkElement)listBox.ItemContainerGenerator.ContainerFromItem(item);

    private static Point PointIn(
        ListBox listBox,
        string item,
        double relativeX,
        double relativeY)
    {
        var container = Container(listBox, item);
        return container.TranslatePoint(
            new Point(container.ActualWidth * relativeX, container.ActualHeight * relativeY),
            listBox);
    }

    private sealed class FirstLetterConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
            => ((string)value)[..1];

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
            => throw new NotSupportedException();
    }
}