namespace Atc.Wpf.Components.Tests.Selectors;

/// <summary>
/// Moving thousands of items must update the footer and header counts once per action, and a collection
/// the control owns must tell its view about the move with one Reset instead of one event per item.
/// </summary>
public sealed class DualListSelectorBulkTransferTests : IDisposable
{
    private const int LargeCount = 5000;

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void OwnedCollections_AreRangeCapable()
    {
        var selector = new DualListSelector();

        Assert.IsType<ObservableCollectionEx<DualListSelectorItem>>(selector.AvailableItems);
        Assert.IsType<ObservableCollectionEx<DualListSelectorItem>>(selector.SelectedItems);
    }

    [StaFact]
    public void MoveAllToSelected_OwnedCollections_UpdatesCountsOnce_AndRaisesOneResetPerList()
    {
        var items = DualListSelectorTestHost.CreateItems("Item", LargeCount);
        using var host = CreateOwnedHost(items, DualListItemSelectionStyle.CheckBox);
        var transfers = new List<DualListSelectorItemsTransferredEventArgs>();
        host.Selector.ItemsTransferred += (_, e) => transfers.Add(e);
        var availableEvents = Record(host.Selector.AvailableItems);
        var selectedEvents = Record(host.Selector.SelectedItems);
        var countUpdatesBefore = host.Selector.ItemCountUpdateCount;

        host.Selector.MoveAllToSelected();

        Assert.Equal(1, host.Selector.ItemCountUpdateCount - countUpdatesBefore);
        Assert.Equal([NotifyCollectionChangedAction.Reset], availableEvents);
        Assert.Equal([NotifyCollectionChangedAction.Reset], selectedEvents);
        Assert.Empty(host.Selector.AvailableItems);
        Assert.Equal(items, host.Selector.SelectedItems);
        Assert.Equal(Enumerable.Range(0, LargeCount), host.Selector.SelectedItems.Select(i => i.SortOrderNumber!.Value));
        var transfer = Assert.Single(transfers);
        Assert.Equal(DualListSelectorTransferDirection.ToSelected, transfer.Direction);
        Assert.Equal(items, transfer.TransferredItems);
        Assert.Contains(LargeCount.ToString(CultureInfo.CurrentCulture), host.SelectedItemCount.Text, StringComparison.Ordinal);
    }

    [StaFact]
    public void MoveToSelected_2500HighlightedOwnedItems_UpdatesCountsOnce_AndKeepsTheOrder()
    {
        var items = DualListSelectorTestHost.CreateItems("Item", LargeCount);
        using var host = CreateOwnedHost(items, DualListItemSelectionStyle.CheckBox);
        var highlighted = items.Where((_, i) => i % 2 == 0).ToList();
        foreach (var item in highlighted)
        {
            host.AvailableList.SelectedItems.Add(item);
        }

        var availableEvents = Record(host.Selector.AvailableItems);
        var selectedEvents = Record(host.Selector.SelectedItems);
        var selectionChanges = 0;
        host.Selector.SelectionChanged += (_, _) => selectionChanges++;
        var countUpdatesBefore = host.Selector.ItemCountUpdateCount;

        host.Selector.MoveToSelected();

        Assert.Equal(1, host.Selector.ItemCountUpdateCount - countUpdatesBefore);
        Assert.Equal([NotifyCollectionChangedAction.Reset], availableEvents);
        Assert.Equal([NotifyCollectionChangedAction.Reset], selectedEvents);
        Assert.InRange(selectionChanges, 0, 2);
        Assert.Equal(items.Where((_, i) => i % 2 == 1), host.Selector.AvailableItems);
        Assert.Equal(highlighted, host.Selector.SelectedItems);
    }

    [StaFact]
    public void MoveAll_ConsumerCollections_UpdatesCountsOnce_AndRaisesOneEventPerMovedItemAtMost()
    {
        var items = DualListSelectorTestHost.CreateItems("Item", LargeCount);
        using var host = new DualListSelectorTestHost(s =>
        {
            s.ItemSelectionStyle = DualListItemSelectionStyle.CheckBox;
            s.AvailableItems = items;
            s.SelectedItems = [];
        });
        var availableEvents = Record(host.Selector.AvailableItems);
        var selectedEvents = Record(host.Selector.SelectedItems);
        var countUpdatesBefore = host.Selector.ItemCountUpdateCount;

        host.Selector.MoveAllToSelected();

        Assert.Equal(1, host.Selector.ItemCountUpdateCount - countUpdatesBefore);

        // A plain ObservableCollection cannot raise one Reset for a range, but emptying it can (Clear).
        Assert.Equal([NotifyCollectionChangedAction.Reset], availableEvents);
        Assert.Equal(LargeCount, selectedEvents.Count);
        Assert.Same(items, host.Selector.AvailableItems);
        Assert.Empty(items);
        Assert.Equal(LargeCount, host.Selector.SelectedItems.Count);
    }

    [StaFact]
    public void MoveToSelected_2500HighlightedConsumerItems_UpdatesCountsOnce()
    {
        var items = DualListSelectorTestHost.CreateItems("Item", LargeCount);
        using var host = new DualListSelectorTestHost(s =>
        {
            s.ItemSelectionStyle = DualListItemSelectionStyle.CheckBox;
            s.AvailableItems = items;
            s.SelectedItems = [];
        });
        var highlighted = items.Where((_, i) => i % 2 == 0).ToList();
        foreach (var item in highlighted)
        {
            host.AvailableList.SelectedItems.Add(item);
        }

        var availableEvents = Record(host.Selector.AvailableItems);
        var selectedEvents = Record(host.Selector.SelectedItems);
        var countUpdatesBefore = host.Selector.ItemCountUpdateCount;

        host.Selector.MoveToSelected();

        Assert.Equal(1, host.Selector.ItemCountUpdateCount - countUpdatesBefore);
        Assert.Equal(highlighted.Count, availableEvents.Count);
        Assert.Equal(highlighted.Count, selectedEvents.Count);
        Assert.Equal(highlighted, host.Selector.SelectedItems);
        Assert.Equal(LargeCount - highlighted.Count, host.Selector.AvailableItems.Count);
    }

    [StaFact]
    public void MoveAllToAvailable_OwnedCollections_KeepsLockedItems_AndTheirHighlight()
    {
        var items = DualListSelectorTestHost.CreateItems("Item", 10);
        using var host = new DualListSelectorTestHost(s =>
        {
            ((ObservableCollectionEx<DualListSelectorItem>)s.SelectedItems).AddRange(items);
            items[3].IsLocked = true;
        });
        host.SelectedList.SelectedItems.Add(items[3]);
        host.SelectedList.SelectedItems.Add(items[5]);
        host.SelectedList.SelectedItems.Add(items[1]);
        var transfers = new List<DualListSelectorItemsTransferredEventArgs>();
        host.Selector.ItemsTransferred += (_, e) => transfers.Add(e);
        var countUpdatesBefore = host.Selector.ItemCountUpdateCount;

        host.Selector.MoveAllToAvailable();
        host.Flush();

        Assert.Equal(1, host.Selector.ItemCountUpdateCount - countUpdatesBefore);
        Assert.Equal(items.Except([items[5], items[1]]), host.Selector.SelectedItems);
        Assert.Equal([items[5], items[1]], host.Selector.AvailableItems);
        Assert.Equal([items[5], items[1]], Assert.Single(transfers).TransferredItems);
        Assert.Equal([items[3]], host.SelectedList.SelectedItems.Cast<DualListSelectorItem>());
    }

    [StaFact]
    public void MoveAllToSelected_WithFilter_MovesTheShownItems_ClearsTheFilter_AndUpdatesCountsOnce()
    {
        var items = DualListSelectorTestHost.CreateItems("Item", 200);
        using var host = CreateOwnedHost(items, DualListItemSelectionStyle.Highlight);
        host.AvailableFilter.Text = "Item 1";
        var shown = items.Where(i => i.Name.Contains("Item 1", StringComparison.Ordinal)).ToList();
        var countUpdatesBefore = host.Selector.ItemCountUpdateCount;

        host.Selector.MoveAllToSelected();

        Assert.Equal(1, host.Selector.ItemCountUpdateCount - countUpdatesBefore);
        Assert.Equal(string.Empty, host.AvailableFilter.Text);
        Assert.Equal(shown, host.Selector.SelectedItems);
        Assert.Equal(items.Except(shown), host.Selector.AvailableItems);
        Assert.Equal(items.Count - shown.Count, host.AvailableList.Items.Count);
    }

    [StaFact]
    public void MoveAllToSelected_OwnedCollections_GroupsTheSelectedList_ByItemGroup()
    {
        var items = DualListSelectorTestHost.CreateItems("Item", 6);
        for (var i = 0; i < items.Count; i++)
        {
            items[i].Group = i % 2 == 0 ? "Even" : "Odd";
        }

        using var host = CreateOwnedHost(items, DualListItemSelectionStyle.Highlight);

        host.Selector.MoveAllToSelected();
        host.Flush();

        var view = host.SelectedList.Items;
        Assert.NotNull(view.Groups);
        Assert.Equal(["Even", "Odd"], view.Groups.Cast<CollectionViewGroup>().Select(g => g.Name));
    }

    [StaFact]
    public void DropOnSelected_OwnedCollections_InsertsAtTheDropSlot()
    {
        var available = DualListSelectorTestHost.CreateItems("A", 4);
        var selected = DualListSelectorTestHost.CreateItems("S", 3);
        using var host = new DualListSelectorTestHost(s =>
        {
            ((ObservableCollectionEx<DualListSelectorItem>)s.AvailableItems).AddRange(available);
            ((ObservableCollectionEx<DualListSelectorItem>)s.SelectedItems).AddRange(selected);
        });

        host.Selector.DropOnSelected([available[2], available[0]], fromAvailable: true, viewInsertIndex: 1);

        Assert.Equal([selected[0], available[2], available[0], selected[1], selected[2]], host.Selector.SelectedItems);
        Assert.Equal([available[1], available[3]], host.Selector.AvailableItems);
    }

    [StaFact]
    public void Reorder_OwnedCollections_KeepsTheHighlight_AndRaisesTheReorders()
    {
        using var host = CreateLargeOwnedHost();
        var selected = host.Selector.SelectedItems;
        var reorders = new List<DualListSelectorItemsReorderedEventArgs>();
        host.Selector.ItemsReordered += (_, e) => reorders.Add(e);

        var item = selected[2500];
        host.SelectedList.ScrollIntoView(item);
        host.Flush();
        host.SelectedList.SelectedItem = item;

        host.Selector.MoveUp();
        host.Flush();
        Assert.Same(item, selected[2499]);
        Assert.Same(item, host.SelectedList.SelectedItem);

        host.Selector.MoveDown();
        host.Selector.MoveDown();
        host.Flush();
        Assert.Same(item, selected[2501]);
        Assert.Same(item, host.SelectedList.SelectedItem);

        host.Selector.MoveToTop();
        host.Flush();
        Assert.Same(item, selected[0]);
        Assert.Same(item, host.SelectedList.SelectedItem);

        host.Selector.DropOnSelected([item], fromAvailable: false, viewInsertIndex: 10);

        Assert.Same(item, selected[9]);
        Assert.Equal(Enumerable.Range(0, LargeCount), selected.Select(x => x.SortOrderNumber!.Value));
        Assert.Equal(5, reorders.Count);
    }

    [StaFact]
    public void MoveToSelected_OwnedCollections_KeepsTheScrollPositionOfTheSelectedList()
    {
        using var host = CreateLargeOwnedHost();
        var middle = host.Selector.SelectedItems[2500];
        host.SelectedList.ScrollIntoView(middle);
        host.Flush();
        var scrollViewer = FindScrollViewer(host.SelectedList)!;
        var offset = scrollViewer.VerticalOffset;
        Assert.True(offset > 0);

        host.AvailableList.SelectedItem = host.Selector.AvailableItems[0];
        host.Selector.MoveToSelected();
        host.Flush();

        Assert.Equal(offset, scrollViewer.VerticalOffset);
        Assert.NotNull(host.SelectedList.ItemContainerGenerator.ContainerFromItem(middle));
    }

    [StaFact]
    public void RemoveFromSelected_OfARowThatIsNotHighlighted_LeavesTheHighlightAlone()
    {
        var items = DualListSelectorTestHost.CreateItems("Item", 5);
        using var host = new DualListSelectorTestHost(s =>
        {
            s.IsOneWay = true;
            ((ObservableCollectionEx<DualListSelectorItem>)s.SelectedItems).AddRange(items);
        });
        host.SelectedList.SelectedItems.Add(items[1]);
        host.SelectedList.SelectedItems.Add(items[3]);
        var selectionChanges = 0;
        host.Selector.SelectionChanged += (_, _) => selectionChanges++;

        host.Selector.RemoveFromSelected(items[4]);

        Assert.Equal(0, selectionChanges);
        Assert.Equal([items[1], items[3]], host.SelectedList.SelectedItems.Cast<DualListSelectorItem>());
        Assert.Equal([items[4]], host.Selector.AvailableItems);
    }

    private static DualListSelectorTestHost CreateLargeOwnedHost()
        => new(s =>
        {
            ((ObservableCollectionEx<DualListSelectorItem>)s.AvailableItems).AddRange(DualListSelectorTestHost.CreateItems("A", LargeCount));
            ((ObservableCollectionEx<DualListSelectorItem>)s.SelectedItems).AddRange(DualListSelectorTestHost.CreateItems("S", LargeCount));
        });

    private static ScrollViewer? FindScrollViewer(DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if ((child as ScrollViewer ?? FindScrollViewer(child)) is { } scrollViewer)
            {
                return scrollViewer;
            }
        }

        return null;
    }

    private static DualListSelectorTestHost CreateOwnedHost(
        IEnumerable<DualListSelectorItem> items,
        DualListItemSelectionStyle style)
        => new(s =>
        {
            s.ItemSelectionStyle = style;
            ((ObservableCollectionEx<DualListSelectorItem>)s.AvailableItems).AddRange(items);
        });

    private static List<NotifyCollectionChangedAction> Record(
        ObservableCollection<DualListSelectorItem> items)
    {
        var actions = new List<NotifyCollectionChangedAction>();
        items.CollectionChanged += (_, e) => actions.Add(e.Action);
        return actions;
    }
}