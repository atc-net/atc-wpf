namespace Atc.Wpf.Components.Tests.Selectors;

public sealed class DualListSelectorCheckBoxStyleTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_KeepTodaysBehaviour()
    {
        var sut = new DualListSelector();

        Assert.Equal(DualListItemSelectionStyle.Highlight, sut.ItemSelectionStyle);
        Assert.False(sut.IsOneWay);
        Assert.Null(sut.MaxSelectedCount);
        Assert.True(sut.IsVirtualizing);
        Assert.Null(sut.GroupMemberPath);
        Assert.Null(sut.HeaderCountFormat);
        Assert.False(sut.ShowItemCount);
    }

    [StaFact]
    public void SelectAll_ChecksEveryShownEnabledItem_AndShowsCheckedOfTotal()
    {
        using var host = new DualListSelectorTestHost(s =>
        {
            s.ItemSelectionStyle = DualListItemSelectionStyle.CheckBox;
            s.AvailableItems = DualListSelectorTestHost.CreateItems("Item", 5);
            s.AvailableItems[4].IsEnabled = false;
        });

        Assert.Equal("0/5", host.AvailableHeaderCount.Text);
        Assert.False(host.AvailableSelectAll.IsChecked);

        DualListSelectorTestHost.ToggleViaAutomation(host.AvailableSelectAll);

        Assert.Equal(4, host.AvailableList.SelectedItems.Count);
        Assert.DoesNotContain(host.Selector.AvailableItems[4], host.AvailableList.SelectedItems.Cast<DualListSelectorItem>());
        Assert.Equal("4/5", host.AvailableHeaderCount.Text);
        Assert.True(host.AvailableSelectAll.IsChecked);
    }

    [StaFact]
    public void SelectAll_WhenAllChecked_UnchecksAll_AndPartialSelectionIsIndeterminate()
    {
        using var host = new DualListSelectorTestHost(s =>
        {
            s.ItemSelectionStyle = DualListItemSelectionStyle.CheckBox;
            s.AvailableItems = DualListSelectorTestHost.CreateItems("Item", 4);
        });

        host.AvailableList.SelectedItems.Add(host.Selector.AvailableItems[1]);

        Assert.Null(host.AvailableSelectAll.IsChecked);
        Assert.Equal("1/4", host.AvailableHeaderCount.Text);

        // From indeterminate a click selects all (a plain three-state check box would go to unchecked).
        DualListSelectorTestHost.ToggleViaAutomation(host.AvailableSelectAll);
        Assert.Equal(4, host.AvailableList.SelectedItems.Count);
        Assert.True(host.AvailableSelectAll.IsChecked);

        DualListSelectorTestHost.ToggleViaAutomation(host.AvailableSelectAll);
        Assert.Empty(host.AvailableList.SelectedItems);
        Assert.False(host.AvailableSelectAll.IsChecked);
        Assert.Equal("0/4", host.AvailableHeaderCount.Text);
    }

    [StaFact]
    public void SelectAll_OnAFilteredList_ChecksOnlyTheMatches_AndMovesOnlyThem()
    {
        using var host = new DualListSelectorTestHost(s =>
        {
            s.ItemSelectionStyle = DualListItemSelectionStyle.CheckBox;
            s.ShowFilter = true;
            s.AvailableItems =
            [
                new DualListSelectorItem { Identifier = "apple", Name = "Apple" },
                new DualListSelectorItem { Identifier = "banana", Name = "Banana" },
                new DualListSelectorItem { Identifier = "apricot", Name = "Apricot" },
                new DualListSelectorItem { Identifier = "cherry", Name = "Cherry" },
            ];
        });
        var transferred = 0;
        host.Selector.ItemsTransferred += (_, _) => transferred++;

        host.AvailableFilter.Text = "ap";
        Assert.Equal("0/2", host.AvailableHeaderCount.Text);

        DualListSelectorTestHost.ToggleViaAutomation(host.AvailableSelectAll);
        Assert.Equal("2/2", host.AvailableHeaderCount.Text);

        host.Selector.MoveToSelected();

        Assert.Equal(["apple", "apricot"], host.Selector.SelectedItems.Select(x => x.Identifier), StringComparer.Ordinal);
        Assert.Equal(["banana", "cherry"], host.Selector.AvailableItems.Select(x => x.Identifier), StringComparer.Ordinal);
        Assert.Equal(1, transferred);
    }

    [StaFact]
    public void HeaderCountFormat_IsUsedForTheHeaderCount()
    {
        using var host = new DualListSelectorTestHost(s =>
        {
            s.ItemSelectionStyle = DualListItemSelectionStyle.CheckBox;
            s.HeaderCountFormat = "{0} of {1}";
            s.SelectedItems = DualListSelectorTestHost.CreateItems("Item", 3);
        });

        host.SelectedList.SelectedItems.Add(host.Selector.SelectedItems[0]);

        Assert.Equal("1 of 3", host.SelectedHeaderCount.Text);
    }
}