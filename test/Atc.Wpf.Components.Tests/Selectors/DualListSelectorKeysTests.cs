namespace Atc.Wpf.Components.Tests.Selectors;

public sealed class DualListSelectorKeysTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void MaxSelectionCount_is_an_alias_of_MaxSelectedCount_both_ways()
    {
        var selector = new DualListSelector { MaxSelectionCount = 3 };

        Assert.Equal(3, selector.MaxSelectedCount);

        selector.MaxSelectedCount = 5;

        Assert.Equal(5, selector.MaxSelectionCount);

        selector.MaxSelectionCount = null;

        Assert.Null(selector.MaxSelectedCount);
    }

    [StaFact]
    public void MaxSelectionCount_can_be_bound_in_xaml()
    {
        var selector = (DualListSelector)XamlReader.Parse("""
            <s:DualListSelector
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:s="clr-namespace:Atc.Wpf.Components.Selectors;assembly=Atc.Wpf.Components"
                MaxSelectionCount="{Binding Max}" />
            """);
        selector.DataContext = new MaxModel { Max = 4 };
        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

        Assert.Equal(4, selector.MaxSelectionCount);
        Assert.Equal(4, selector.MaxSelectedCount);
    }

    [StaFact]
    public void A_transfer_writes_the_selected_identifiers_to_SelectedKeys_with_one_event()
    {
        using var host = new DualListSelectorTestHost(s => s.AvailableItems = DualListSelectorTestHost.CreateItems("A", 3));
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        host.Selector.SelectedKeysChanged += (_, e) => events.Add(e);

        host.Selector.MoveAllToSelected();

        Assert.Equal(["A0", "A1", "A2"], host.Selector.SelectedKeys!);
        var single = Assert.Single(events);
        Assert.Empty(single.OldValue);
        Assert.Equal(["A0", "A1", "A2"], single.NewValue);
    }

    [StaFact]
    public void Setting_SelectedKeys_moves_the_items_and_removing_a_key_moves_it_back()
    {
        using var host = new DualListSelectorTestHost(s => s.AvailableItems = DualListSelectorTestHost.CreateItems("A", 4));
        var keys = new ObservableCollection<string> { "A2", "A0", "missing" };

        host.Selector.SelectedKeys = keys;
        host.Flush();

        Assert.Equal(["A2", "A0"], host.Selector.SelectedItems.Select(x => x.Identifier), StringComparer.Ordinal);
        Assert.Equal(["A1", "A3"], host.Selector.AvailableItems.Select(x => x.Identifier), StringComparer.Ordinal);
        Assert.Same(keys, host.Selector.SelectedKeys);
        Assert.Equal(["A2", "A0"], keys);

        keys.Remove("A2");
        host.Flush();

        Assert.Equal(["A0"], host.Selector.SelectedItems.Select(x => x.Identifier), StringComparer.Ordinal);
        Assert.Contains(host.Selector.AvailableItems, x => x.Identifier == "A2");
    }

    [StaFact]
    public void Keys_bound_before_the_items_are_applied_once_the_items_arrive()
    {
        var model = new KeysModel("A", ["A1"]);
        using var host = new DualListSelectorTestHost(s =>
        {
            s.DataContext = model;
            BindingOperations.SetBinding(s, DualListSelector.SelectedKeysProperty, new Binding(nameof(KeysModel.Keys)));
            BindingOperations.SetBinding(s, DualListSelector.SelectedItemsProperty, new Binding(nameof(KeysModel.Selected)));
            BindingOperations.SetBinding(s, DualListSelector.AvailableItemsProperty, new Binding(nameof(KeysModel.Available)));
        });
        host.Flush();

        Assert.Equal(["A1"], model.Selected.Select(x => x.Identifier), StringComparer.Ordinal);
        Assert.Equal(["A1"], model.Keys);
    }

    [StaFact]
    public void A_DataContext_swap_leaves_both_models_untouched()
    {
        var first = new KeysModel("A", ["A1"]);
        var second = new KeysModel("B", ["B0", "B2"]);
        first.Apply();
        second.Apply();
        using var host = new DualListSelectorTestHost(s =>
        {
            s.DataContext = first;
            BindingOperations.SetBinding(s, DualListSelector.AvailableItemsProperty, new Binding(nameof(KeysModel.Available)));
            BindingOperations.SetBinding(s, DualListSelector.SelectedKeysProperty, new Binding(nameof(KeysModel.Keys)));
            BindingOperations.SetBinding(s, DualListSelector.SelectedItemsProperty, new Binding(nameof(KeysModel.Selected)));
        });
        host.Flush();
        var events = 0;
        host.Selector.SelectedKeysChanged += (_, _) => events++;

        host.Selector.DataContext = second;
        host.Flush();

        Assert.Equal(["A1"], first.Keys);
        Assert.Equal(["A1"], first.Selected.Select(x => x.Identifier), StringComparer.Ordinal);
        Assert.Equal(["A0", "A2"], first.Available.Select(x => x.Identifier), StringComparer.Ordinal);
        Assert.Equal(["B0", "B2"], second.Keys);
        Assert.Equal(["B0", "B2"], second.Selected.Select(x => x.Identifier), StringComparer.Ordinal);
        Assert.Equal(0, events);
    }

    private sealed class MaxModel
    {
        public int? Max { get; set; }
    }

    private sealed class KeysModel
    {
        public KeysModel(
            string prefix,
            IEnumerable<string> keys)
        {
            Available = DualListSelectorTestHost.CreateItems(prefix, 3);
            Keys = new ObservableCollection<string>(keys);
        }

        public ObservableCollection<DualListSelectorItem> Available { get; set; }

        public ObservableCollection<DualListSelectorItem> Selected { get; set; } = [];

        public ObservableCollection<string> Keys { get; set; }

        public void Apply()
        {
            foreach (var item in Available.Where(x => Keys.Contains(x.Identifier!)).ToList())
            {
                Available.Remove(item);
                Selected.Add(item);
            }
        }
    }
}