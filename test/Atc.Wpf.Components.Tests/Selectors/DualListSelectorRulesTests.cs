namespace Atc.Wpf.Components.Tests.Selectors;

public sealed class DualListSelectorRulesTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void RemoveFromSelected_MovesTheItemBack_AndRaisesOneTransfer()
    {
        using var host = new DualListSelectorTestHost(s =>
        {
            s.IsOneWay = true;
            s.SelectedItems = DualListSelectorTestHost.CreateItems("Item", 3);
        });
        var events = new List<DualListSelectorItemsTransferredEventArgs>();
        host.Selector.ItemsTransferred += (_, e) => events.Add(e);
        var item = host.Selector.SelectedItems[1];

        Assert.True(DualListSelectorCommands.RemoveFromSelected.CanExecute(item, host.Selector));
        DualListSelectorCommands.RemoveFromSelected.Execute(item, host.Selector);

        Assert.DoesNotContain(item, host.Selector.SelectedItems);
        Assert.Contains(item, host.Selector.AvailableItems);
        var single = Assert.Single(events);
        Assert.Equal(DualListSelectorTransferDirection.ToAvailable, single.Direction);
        Assert.Equal([item], single.TransferredItems);
        Assert.Equal([0, 1], host.Selector.SelectedItems.Select(x => x.SortOrderNumber!.Value));
    }

    [StaFact]
    public void OneWay_DeleteKey_RemovesTheHighlightedItems_ButNotLockedOnes()
    {
        using var host = new DualListSelectorTestHost(s =>
        {
            s.IsOneWay = true;
            s.SelectedItems = DualListSelectorTestHost.CreateItems("Item", 3);
            s.SelectedItems[0].IsLocked = true;
        });
        host.SelectedList.SelectedItems.Add(host.Selector.SelectedItems[0]);
        host.SelectedList.SelectedItems.Add(host.Selector.SelectedItems[2]);

        host.RaiseKey(host.SelectedList, Key.Delete);

        Assert.Equal(["Item0", "Item1"], host.Selector.SelectedItems.Select(x => x.Identifier), StringComparer.Ordinal);
        Assert.Equal(["Item2"], host.Selector.AvailableItems.Select(x => x.Identifier), StringComparer.Ordinal);
    }

    [StaFact]
    public void DeleteKey_DoesNothing_WhenNotOneWay()
    {
        using var host = new DualListSelectorTestHost(s => s.SelectedItems = DualListSelectorTestHost.CreateItems("Item", 2));
        host.SelectedList.SelectedItems.Add(host.Selector.SelectedItems[0]);

        host.RaiseKey(host.SelectedList, Key.Delete);

        Assert.Equal(2, host.Selector.SelectedItems.Count);
    }

    [StaFact]
    public void LockedItems_SurviveMoveAllBack_MoveBack_Drag_AndRemove()
    {
        using var host = new DualListSelectorTestHost(s =>
        {
            s.SelectedItems = DualListSelectorTestHost.CreateItems("Item", 4);
            s.SelectedItems[1].IsLocked = true;
        });
        var locked = host.Selector.SelectedItems[1];

        Assert.False(DualListSelectorCommands.RemoveFromSelected.CanExecute(locked, host.Selector));
        DualListSelectorCommands.RemoveFromSelected.Execute(locked, host.Selector);
        Assert.Contains(locked, host.Selector.SelectedItems);

        host.SelectedList.SelectedItems.Add(locked);
        Assert.False(host.Selector.CanMoveToAvailable());
        host.Selector.MoveToAvailable();
        Assert.Contains(locked, host.Selector.SelectedItems);
        host.SelectedList.UnselectAll();

        host.Selector.DropOnAvailable([locked, host.Selector.SelectedItems[0]], fromSelected: true);
        Assert.Contains(locked, host.Selector.SelectedItems);
        Assert.Equal(3, host.Selector.SelectedItems.Count);

        host.Selector.MoveAllToAvailable();

        Assert.Equal([locked], host.Selector.SelectedItems);
        Assert.Equal(3, host.Selector.AvailableItems.Count);
        Assert.False(host.Selector.CanMoveAllToAvailable());
    }

    [StaFact]
    public void LockedItems_CanStillBeReordered()
    {
        using var host = new DualListSelectorTestHost(s =>
        {
            s.SelectedItems = DualListSelectorTestHost.CreateItems("Item", 3);
            s.SelectedItems[2].IsLocked = true;
        });
        var locked = host.Selector.SelectedItems[2];
        host.SelectedList.SelectedItem = locked;

        host.Selector.MoveToTop();

        Assert.Same(locked, host.Selector.SelectedItems[0]);
    }

    [StaFact]
    public void MaxSelectedCount_BlocksTransfersThatWouldExceedIt()
    {
        using var host = new DualListSelectorTestHost(s =>
        {
            s.MaxSelectedCount = 3;
            s.ShowItemCount = true;
            s.AvailableItems = DualListSelectorTestHost.CreateItems("A", 4);
            s.SelectedItems = DualListSelectorTestHost.CreateItems("S", 2);
        });
        var available = host.Selector.AvailableItems;

        Assert.Equal(MaxText(2, 3), host.SelectedItemCount.Text);

        host.AvailableList.SelectedItems.Add(available[0]);
        host.AvailableList.SelectedItems.Add(available[1]);
        Assert.False(host.Selector.CanMoveToSelected());
        Assert.False(host.Selector.CanMoveAllToSelected());
        host.Selector.MoveToSelected();
        host.Selector.MoveAllToSelected();
        Assert.Equal(2, host.Selector.SelectedItems.Count);

        Assert.False(host.Selector.CanDropOnSelected([available[0], available[1]], fromAvailable: true));
        host.Selector.DropOnSelected([available[0], available[1]], fromAvailable: true, viewInsertIndex: 0);
        Assert.Equal(2, host.Selector.SelectedItems.Count);

        host.AvailableList.UnselectAll();
        host.AvailableList.SelectedItems.Add(available[2]);
        Assert.True(host.Selector.CanMoveToSelected());
        host.Selector.MoveToSelected();

        Assert.Equal(3, host.Selector.SelectedItems.Count);
        Assert.Equal(MaxText(3, 3), host.SelectedItemCount.Text);
        Assert.False(host.Selector.CanDropOnSelected([available[0]], fromAvailable: true));

        // Reordering inside the full list is not a transfer, so it stays allowed.
        Assert.True(host.Selector.CanDropOnSelected([host.Selector.SelectedItems[0]], fromAvailable: false));
    }

    [StaFact]
    public void MaxSelectedCount_ShowsTheLimitFooter_EvenWithoutShowItemCount()
    {
        using var host = new DualListSelectorTestHost(s =>
        {
            s.MaxSelectedCount = 5;
            s.SelectedItems = DualListSelectorTestHost.CreateItems("S", 1);
        });

        Assert.Equal(Visibility.Visible, host.SelectedItemCount.Visibility);
        Assert.Equal(MaxText(1, 5), host.SelectedItemCount.Text);
    }

    [StaFact]
    public void Grouping_ByItemGroup_AndByGroupMemberPath_KeepsOnlyGroupsWithFilterMatches()
    {
        using var host = new DualListSelectorTestHost(s =>
        {
            s.ShowFilter = true;
            s.AvailableItems =
            [
                new DualListSelectorItem { Identifier = "1", Name = "Apple", Group = "Fruit", Description = "Red" },
                new DualListSelectorItem { Identifier = "2", Name = "Banana", Group = "Fruit", Description = "Yellow" },
                new DualListSelectorItem { Identifier = "3", Name = "Carrot", Group = "Vegetable", Description = "Orange" },
            ];
        });
        var view = CollectionViewSource.GetDefaultView(host.Selector.AvailableItems);

        Assert.Equal(2, view.Groups!.Count);

        host.AvailableFilter.Text = "an";
        Assert.Equal(["Fruit"], view.Groups.Cast<CollectionViewGroup>().Select(g => g.Name));

        host.AvailableFilter.Text = string.Empty;
        host.Selector.GroupMemberPath = nameof(DualListSelectorItem.Description);
        Assert.Equal(3, view.Groups!.Count);
    }

    [StaFact]
    public void Grouping_IsOff_WhenNoItemHasAGroup()
    {
        using var host = new DualListSelectorTestHost(s => s.AvailableItems = DualListSelectorTestHost.CreateItems("A", 3));

        var view = CollectionViewSource.GetDefaultView(host.Selector.AvailableItems);

        Assert.Null(view.Groups);
    }

    private static string MaxText(
        int count,
        int max)
        => string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.CountOfMax2, count, max);
}