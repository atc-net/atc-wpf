namespace Atc.Wpf.Tests.DragDrop;

[Collection(nameof(DragDropTestGroup))]
public sealed class DragDropPressTests
{
    private const string ItemTemplateXaml = """
        <DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
            <StackPanel>
                <TextBlock><Run Text="{Binding Mode=OneWay}" /></TextBlock>
                <TextBox Text="{Binding Mode=OneWay}" />
            </StackPanel>
        </DataTemplate>
        """;

    private static readonly FieldInfo PressedItemField = typeof(DragDropManager)
        .GetField("pressedItem", BindingFlags.NonPublic | BindingFlags.Static)!;

    [StaFact]
    public void ResolvePressedItem_OnTextBlockInItem_ReturnsThatItem()
    {
        // Arrange
        var listBox = CreateListBox();
        using var host = new DragDropTestWindow(listBox);
        var textBlock = DragDropTestWindow.FindDescendant<TextBlock>(ContainerOf(listBox, "b"));

        // Act
        var item = DragDropManager.ResolvePressedItem(listBox, textBlock);

        // Assert
        Assert.Equal("b", item);
    }

    [StaFact]
    public void ResolvePressedItem_OnRunInsideTextBlock_ReturnsThatItemWithoutThrowing()
    {
        // Arrange
        var listBox = CreateListBox();
        using var host = new DragDropTestWindow(listBox);
        var run = DragDropTestWindow.FindDescendant<TextBlock>(ContainerOf(listBox, "c")).Inlines.FirstInline;

        // Act
        var item = DragDropManager.ResolvePressedItem(listBox, run);

        // Assert
        Assert.Equal("c", item);
    }

    [StaFact]
    public void ResolvePressedItem_OnTextBoxInItem_StartsNoDrag()
    {
        // Arrange
        var listBox = CreateListBox();
        using var host = new DragDropTestWindow(listBox);
        var textBox = DragDropTestWindow.FindDescendant<TextBox>(ContainerOf(listBox, "a"));
        var textBoxContent = DragDropTestWindow.FindDescendant<ScrollViewer>(textBox);

        // Act
        var onTextBox = DragDropManager.ResolvePressedItem(listBox, textBox);
        var insideTextBox = DragDropManager.ResolvePressedItem(listBox, textBoxContent);

        // Assert
        Assert.Null(onTextBox);
        Assert.Null(insideTextBox);
    }

    [StaFact]
    public void ResolvePressedItem_OnListScrollBar_StartsNoDrag()
    {
        // Arrange
        var listBox = CreateListBox(itemCount: 200);
        listBox.Height = 100;
        using var host = new DragDropTestWindow(listBox);
        var scrollBar = FindVisibleVerticalScrollBar(listBox);
        var thumb = DragDropTestWindow.FindDescendant<Thumb>(scrollBar);

        // Act
        var onScrollBar = DragDropManager.ResolvePressedItem(listBox, scrollBar);
        var onThumb = DragDropManager.ResolvePressedItem(listBox, thumb);

        // Assert
        Assert.Null(onScrollBar);
        Assert.Null(onThumb);
    }

    [StaFact]
    public void PreviewMouseDown_RemembersTheItemUnderThePress()
    {
        // Arrange
        var listBox = CreateListBox();
        DragDropAttach.SetIsDragSource(listBox, value: true);
        using var host = new DragDropTestWindow(listBox);
        var textBlock = DragDropTestWindow.FindDescendant<TextBlock>(ContainerOf(listBox, "b"));

        try
        {
            // Act
            textBlock.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = Mouse.PreviewMouseDownEvent,
            });

            // Assert
            Assert.Equal("b", PressedItemField.GetValue(obj: null));
        }
        finally
        {
            listBox.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = Mouse.PreviewMouseUpEvent,
            });
        }
    }

    [StaFact]
    public void PreviewMouseDown_OnTextBoxInItem_RemembersNothing()
    {
        // Arrange
        var listBox = CreateListBox();
        DragDropAttach.SetIsDragSource(listBox, value: true);
        using var host = new DragDropTestWindow(listBox);
        var textBox = DragDropTestWindow.FindDescendant<TextBox>(ContainerOf(listBox, "b"));

        // Act
        textBox.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
        {
            RoutedEvent = Mouse.PreviewMouseDownEvent,
        });

        // Assert
        Assert.Null(PressedItemField.GetValue(obj: null));
    }

    private static ListBox CreateListBox(int itemCount = 3)
    {
        var items = new ObservableCollection<string>();
        for (var i = 0; i < itemCount; i++)
        {
            items.Add(i < 26 ? ((char)('a' + i)).ToString() : $"item {i}");
        }

        return new ListBox
        {
            ItemsSource = items,
            ItemTemplate = (DataTemplate)XamlReader.Parse(ItemTemplateXaml),
        };
    }

    private static ListBoxItem ContainerOf(
        ListBox listBox,
        string item)
        => (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(item);

    private static ScrollBar FindVisibleVerticalScrollBar(DependencyObject root)
        => TryFindVisibleVerticalScrollBar(root)
           ?? throw new InvalidOperationException("No visible vertical ScrollBar found.");

    private static ScrollBar? TryFindVisibleVerticalScrollBar(
        DependencyObject root)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is ScrollBar { Orientation: Orientation.Vertical, IsVisible: true } scrollBar)
            {
                return scrollBar;
            }

            var found = TryFindVisibleVerticalScrollBar(child);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }
}