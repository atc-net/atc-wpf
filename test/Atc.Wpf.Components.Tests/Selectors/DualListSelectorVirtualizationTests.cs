namespace Atc.Wpf.Components.Tests.Selectors;

public sealed class DualListSelectorVirtualizationTests : IDisposable
{
    private const int LargeCount = 5000;

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void IsVirtualizing_TurnsOnRecyclingVirtualization_InBothLists()
    {
        using var host = CreateLargeHost();

        foreach (var list in new[] { host.AvailableList, host.SelectedList })
        {
            Assert.True(VirtualizingPanel.GetIsVirtualizing(list));
            Assert.True(VirtualizingPanel.GetIsVirtualizingWhenGrouping(list));
            Assert.Equal(VirtualizationMode.Recycling, VirtualizingPanel.GetVirtualizationMode(list));
            Assert.True(ScrollViewer.GetCanContentScroll(list));
        }

        // Only the rows in view are realized.
        Assert.NotNull(host.AvailableList.ItemContainerGenerator.ContainerFromIndex(0));
        Assert.Null(host.AvailableList.ItemContainerGenerator.ContainerFromIndex(4000));
        Assert.Null(host.SelectedList.ItemContainerGenerator.ContainerFromIndex(4000));
    }

    [StaFact]
    public void IsVirtualizing_False_TurnsVirtualizationOff()
    {
        using var host = new DualListSelectorTestHost(s => s.IsVirtualizing = false);

        Assert.False(VirtualizingPanel.GetIsVirtualizing(host.AvailableList));
        Assert.False(VirtualizingPanel.GetIsVirtualizing(host.SelectedList));

        host.Selector.IsVirtualizing = true;

        Assert.True(VirtualizingPanel.GetIsVirtualizing(host.AvailableList));
    }

    [StaFact]
    public void Reorder_ButtonsAndDrop_WorkOnItemsScrolledIntoTheMiddle()
    {
        using var host = CreateLargeHost();
        var selected = host.Selector.SelectedItems;
        var reorders = new List<DualListSelectorItemsReorderedEventArgs>();
        host.Selector.ItemsReordered += (_, e) => reorders.Add(e);

        var item = selected[2500];
        host.SelectedList.ScrollIntoView(item);
        host.Flush();
        host.SelectedList.SelectedItem = item;

        host.Selector.MoveUp();
        Assert.Same(item, selected[2499]);

        host.Selector.MoveDown();
        host.Selector.MoveDown();
        Assert.Same(item, selected[2501]);

        // Drag the item onto the slot before item 10 (not realized: the list is scrolled to the middle).
        host.Selector.DropOnSelected([item], fromAvailable: false, viewInsertIndex: 10);

        Assert.Same(item, selected[10]);
        Assert.Equal(Enumerable.Range(0, LargeCount), selected.Select(x => x.SortOrderNumber!.Value));
        Assert.Equal(4, reorders.Count);
        Assert.Equal(2501, reorders[^1].OldIndex);
        Assert.Equal(10, reorders[^1].NewIndex);
    }

    [StaFact]
    public void Drop_FromAvailable_InsertsAtTheDropSlot_AndRaisesOneTransfer()
    {
        using var host = CreateLargeHost();
        var transfers = 0;
        host.Selector.ItemsTransferred += (_, _) => transfers++;
        var dragged = new List<DualListSelectorItem> { host.Selector.AvailableItems[4000], host.Selector.AvailableItems[4001] };

        Assert.True(host.Selector.CanDropOnSelected(dragged, fromAvailable: true));
        host.Selector.DropOnSelected(dragged, fromAvailable: true, viewInsertIndex: 3000);

        Assert.Same(dragged[0], host.Selector.SelectedItems[3000]);
        Assert.Same(dragged[1], host.Selector.SelectedItems[3001]);
        Assert.Equal(LargeCount - 2, host.Selector.AvailableItems.Count);
        Assert.Equal(1, transfers);
    }

    [StaFact]
    public void Drop_WithAFilter_MapsTheShownSlotToTheUnderlyingIndex()
    {
        using var host = new DualListSelectorTestHost(s =>
        {
            s.ShowFilter = true;
            s.SelectedItems =
            [
                new DualListSelectorItem { Identifier = "a1", Name = "Alpha 1" },
                new DualListSelectorItem { Identifier = "b1", Name = "Beta 1" },
                new DualListSelectorItem { Identifier = "a2", Name = "Alpha 2" },
                new DualListSelectorItem { Identifier = "b2", Name = "Beta 2" },
                new DualListSelectorItem { Identifier = "a3", Name = "Alpha 3" },
            ];
        });
        var filter = (TextBox)host.Selector.Template.FindName("PART_SelectedFilterTextBox", host.Selector);
        filter.Text = "Alpha";
        var a3 = host.Selector.SelectedItems[4];

        // Shown: [Alpha 1, Alpha 2, Alpha 3]; drop Alpha 3 before Alpha 2 (shown slot 1).
        host.Selector.DropOnSelected([a3], fromAvailable: false, viewInsertIndex: 1);

        Assert.Equal(["a1", "b1", "a3", "a2", "b2"], host.Selector.SelectedItems.Select(x => x.Identifier), StringComparer.Ordinal);
    }

    [StaFact]
    public void Keyboard_EnterTransfersFromAVirtualizedList()
    {
        using var host = CreateLargeHost();
        var item = host.Selector.AvailableItems[3500];
        host.AvailableList.ScrollIntoView(item);
        host.Flush();
        host.AvailableList.SelectedItem = item;

        host.RaiseKey(host.AvailableList, Key.Enter);

        Assert.Same(item, host.Selector.SelectedItems[^1]);
        Assert.DoesNotContain(item, host.Selector.AvailableItems);
    }

    private static DualListSelectorTestHost CreateLargeHost()
        => new(s =>
        {
            s.AvailableItems = DualListSelectorTestHost.CreateItems("A", LargeCount);
            s.SelectedItems = DualListSelectorTestHost.CreateItems("S", LargeCount);
        });
}