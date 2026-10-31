namespace Atc.Wpf.Tests.DragDrop;

[Collection(nameof(DragDropTestGroup))]
public sealed class DragDropMultiItemTests
{
    private static readonly FieldInfo PressedSelectionField = typeof(DragDropManager)
        .GetField("pressedSelection", BindingFlags.NonPublic | BindingFlags.Static)!;

    [StaFact]
    public void SourceItems_NotSet_HoldsTheSourceItem()
    {
        // Arrange
        var dragInfo = new DragInfo
        {
            SourceItem = "a",
            VisualSource = new ListBox(),
        };

        // Act
        var items = dragInfo.SourceItems;

        // Assert
        Assert.Equal(["a"], items);
    }

    [StaTheory]
    [InlineData(5, "a,c,e,b,d")]
    [InlineData(0, "b,d,a,c,e")]
    [InlineData(3, "a,c,b,d,e")]
    [InlineData(2, "a,b,d,c,e")]
    public void Drop_ReorderOfSeveralItems_MovesThemTogetherInOrder(
        int insertIndex,
        string expected)
    {
        // Arrange
        var items = new ObservableCollection<string> { "a", "b", "c", "d", "e" };
        var actions = new List<NotifyCollectionChangedAction>();
        items.CollectionChanged += (_, e) => actions.Add(e.Action);
        var dropInfo = CreateDropInfo(items, ["b", "d"], items, insertIndex);

        // Act
        var handler = new DefaultDropHandler();
        handler.DragOver(dropInfo);
        handler.Drop(dropInfo);

        // Assert
        Assert.Equal(DragDropEffects.Move, dropInfo.Effects);
        Assert.Equal(expected, string.Join(",", items));
        Assert.All(actions, x => Assert.Equal(NotifyCollectionChangedAction.Move, x));
    }

    [StaFact]
    public void Drop_ReorderOfSeveralItemsInAPlainList_MovesThemTogetherInOrder()
    {
        // Arrange
        var items = new List<string> { "a", "b", "c", "d", "e" };
        var dropInfo = CreateDropInfo(items, ["b", "d"], items, insertIndex: 5);

        // Act
        new DefaultDropHandler().Drop(dropInfo);

        // Assert
        Assert.Equal("a,c,e,b,d", string.Join(",", items));
    }

    [StaFact]
    public void Drop_TransferOfSeveralItems_InsertsThemInOrderAtTheSlot()
    {
        // Arrange
        var source = new ObservableCollection<string> { "a", "b", "c", "d" };
        var target = new ObservableCollection<string> { "x", "y" };
        var dropInfo = CreateDropInfo(source, ["a", "c", "d"], target, insertIndex: 1);

        // Act
        new DefaultDropHandler().Drop(dropInfo);

        // Assert
        Assert.Equal("b", string.Join(",", source));
        Assert.Equal("x,a,c,d,y", string.Join(",", target));
    }

    [StaFact]
    public void DragOver_OneDraggedItemDoesNotFitTheTarget_DeniesTheDrop()
    {
        // Arrange
        var source = new ObservableCollection<object> { "a", 1 };
        var target = new ObservableCollection<string> { "x" };
        var dropInfo = CreateDropInfo(source, ["a", 1], target, insertIndex: 0);

        // Act
        new DefaultDropHandler().DragOver(dropInfo);

        // Assert
        Assert.Equal(DragDropEffects.None, dropInfo.Effects);
    }

    [StaFact]
    public void GetDraggedItems_PressOnOneOfSeveralSelectedItems_ReturnsTheSelectionInSourceOrder()
    {
        // Arrange
        var listBox = CreateExtendedListBox();
        listBox.SelectedItems.Add("d");
        listBox.SelectedItems.Add("b");

        // Act
        var items = DragDropManager.GetDraggedItems(listBox, "b");

        // Assert
        Assert.Equal(["b", "d"], items);
    }

    [StaFact]
    public void GetDraggedItems_PressOnAnUnselectedItem_ReturnsOnlyThatItem()
    {
        // Arrange
        var listBox = CreateExtendedListBox();
        listBox.SelectedItems.Add("d");
        listBox.SelectedItems.Add("b");

        // Act
        var items = DragDropManager.GetDraggedItems(listBox, "c");

        // Assert
        Assert.Equal(["c"], items);
    }

    [StaFact]
    public void PreviewMouseDown_OnASelectedItemOfSeveral_KeepsTheSelectionUntilMouseUp()
    {
        // Arrange
        var listBox = CreateExtendedListBox();
        DragDropAttach.SetIsDragSource(listBox, value: true);
        using var host = new DragDropTestWindow(listBox);
        listBox.SelectedItems.Add("b");
        listBox.SelectedItems.Add("d");
        var container = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem("d");
        var down = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
        {
            RoutedEvent = Mouse.PreviewMouseDownEvent,
        };

        // Act
        container.RaiseEvent(down);
        var selectionWhilePressed = listBox.SelectedItems.Count;
        var draggedItems = (IReadOnlyList<object>?)PressedSelectionField.GetValue(obj: null);
        container.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
        {
            RoutedEvent = Mouse.PreviewMouseUpEvent,
        });

        // Assert
        Assert.True(down.Handled);
        Assert.Equal(2, selectionWhilePressed);
        Assert.Equal(["b", "d"], draggedItems);
        Assert.Equal(["d"], listBox.SelectedItems.Cast<object>());
    }

    [StaFact]
    public void PreviewMouseDown_OnACheckBoxInASelectedItemOfSeveral_LetsTheCheckBoxGetThePress()
    {
        // Arrange
        var listBox = CreateExtendedListBox();
        listBox.ItemTemplate = (DataTemplate)XamlReader.Parse(
            """<DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"><CheckBox Content="{Binding}" /></DataTemplate>""");
        DragDropAttach.SetIsDragSource(listBox, value: true);
        using var host = new DragDropTestWindow(listBox);
        listBox.SelectedItems.Add("b");
        listBox.SelectedItems.Add("d");
        var container = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem("d");
        var checkBox = DragDropTestWindow.FindDescendant<CheckBox>(container);
        var down = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
        {
            RoutedEvent = Mouse.PreviewMouseDownEvent,
        };

        try
        {
            // Act
            checkBox.RaiseEvent(down);

            // Assert
            Assert.False(down.Handled);
            Assert.Equal(["b", "d"], (IReadOnlyList<object>?)PressedSelectionField.GetValue(obj: null));
        }
        finally
        {
            listBox.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = Mouse.PreviewMouseUpEvent,
            });
        }
    }

    [StaTheory]
    [InlineData(50, 5, 0, -1)]
    [InlineData(50, 195, 0, 1)]
    [InlineData(50, 100, 0, 0)]
    [InlineData(5, 100, -1, 0)]
    [InlineData(295, 100, 1, 0)]
    [InlineData(50, -5, 0, 0)]
    public void GetAutoScrollDirection_EdgeZones(
        double x,
        double y,
        int expectedX,
        int expectedY)
    {
        // Act
        var direction = DragDropHelper.GetAutoScrollDirection(new Size(300, 200), new Point(x, y));

        // Assert
        Assert.Equal(expectedX, direction.X);
        Assert.Equal(expectedY, direction.Y);
    }

    [StaFact]
    public void GetAutoScrollDirection_SmallViewport_KeepsTheMiddleFree()
    {
        // Act
        var middle = DragDropHelper.GetAutoScrollDirection(new Size(100, 40), new Point(50, 15));
        var top = DragDropHelper.GetAutoScrollDirection(new Size(100, 40), new Point(50, 5));

        // Assert
        Assert.Equal(0, middle.Y);
        Assert.Equal(-1, top.Y);
    }

    [StaFact]
    public void AutoScroll_PointerAtTheBottomEdge_ScrollsDown()
    {
        // Arrange
        var listBox = new ListBox
        {
            ItemsSource = Enumerable.Range(0, 200).Select(i => $"item {i}").ToList(),
            Height = 150,
        };
        using var host = new DragDropTestWindow(listBox);
        var scrollViewer = DragDropTestWindow.FindDescendant<ScrollViewer>(listBox);

        // Act
        DragDropManager.AutoScroll(listBox, _ => new Point(20, scrollViewer.ActualHeight - 5));
        listBox.UpdateLayout();
        var afterBottomEdge = scrollViewer.VerticalOffset;
        DragDropManager.AutoScroll(listBox, _ => new Point(20, scrollViewer.ActualHeight / 2));
        listBox.UpdateLayout();
        var afterMiddle = scrollViewer.VerticalOffset;

        // Assert
        Assert.True(afterBottomEdge > 0);
        Assert.Equal(afterBottomEdge, afterMiddle);
    }

    private static ListBox CreateExtendedListBox()
        => new()
        {
            ItemsSource = new ObservableCollection<string> { "a", "b", "c", "d", "e" },
            SelectionMode = SelectionMode.Extended,
        };

    private static DropInfo CreateDropInfo(
        System.Collections.IEnumerable sourceCollection,
        IReadOnlyList<object> items,
        System.Collections.IEnumerable targetCollection,
        int insertIndex)
    {
        var dragInfo = new DragInfo
        {
            SourceItem = items[0],
            SourceItems = items,
            SourceCollection = sourceCollection,
            VisualSource = new ListBox(),
        };

        return new DropInfo
        {
            Data = items[0],
            TargetCollection = targetCollection,
            VisualTarget = new ListBox(),
            InsertIndex = insertIndex,
            DragInfo = dragInfo,
        };
    }
}