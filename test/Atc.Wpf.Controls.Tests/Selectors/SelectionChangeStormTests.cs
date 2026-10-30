namespace Atc.Wpf.Controls.Tests.Selectors;

/// <summary>
/// A bulk selection change (Select all, Clear, or a range change from outside) must update the summary, chips and
/// check states once, not once per key.
/// </summary>
[Collection(nameof(PackResourceTestGroup))]
public sealed class SelectionChangeStormTests : IDisposable
{
    private const int ItemCount = 1000;

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void MultiSelectComboBox_chips_select_all_rebuilds_the_summary_and_chips_once()
    {
        var sut = new MultiSelectComboBox { Items = Items(), DisplayMode = MultiSelectDisplayMode.Chips };
        var notifications = Record(sut.SelectedKeys);
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;
        var summaryUpdates = sut.SummaryUpdateCount;
        var chipRebuilds = sut.ChipRebuildCount;

        sut.SelectAll();

        Assert.Equal(ItemCount, sut.SelectedKeys.Count);
        Assert.Equal(1, sut.SummaryUpdateCount - summaryUpdates);
        Assert.Equal(1, sut.ChipRebuildCount - chipRebuilds);
        Assert.Equal([NotifyCollectionChangedAction.Reset], notifications);
        Assert.Equal(1, events);
    }

    [StaFact]
    public void MultiSelectComboBox_chips_select_all_into_a_view_model_collection_rebuilds_once()
    {
        var keys = new System.Collections.ObjectModel.ObservableCollection<string>();
        var sut = new MultiSelectComboBox { Items = Items(), DisplayMode = MultiSelectDisplayMode.Chips, SelectedKeys = keys };
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;
        var summaryUpdates = sut.SummaryUpdateCount;
        var chipRebuilds = sut.ChipRebuildCount;

        sut.SelectAll();

        Assert.Equal(ItemCount, keys.Count);
        Assert.Equal(1, sut.SummaryUpdateCount - summaryUpdates);
        Assert.Equal(1, sut.ChipRebuildCount - chipRebuilds);
        Assert.Equal(1, events);

        summaryUpdates = sut.SummaryUpdateCount;
        sut.ClearSelection();

        Assert.Empty(keys);
        Assert.Equal(1, sut.SummaryUpdateCount - summaryUpdates);
        Assert.Equal(2, events);
    }

    [StaFact]
    public void MultiSelectComboBox_range_change_from_outside_rebuilds_once()
    {
        var keys = new Atc.Wpf.Collections.ObservableCollectionEx<string>();
        var sut = new MultiSelectComboBox { Items = Items(), DisplayMode = MultiSelectDisplayMode.Chips, SelectedKeys = keys };
        var summaryUpdates = sut.SummaryUpdateCount;
        var refreshes = sut.PartCheckList.StateRefreshCount;

        keys.AddRange(Items().Keys);

        Assert.Equal(1, sut.SummaryUpdateCount - summaryUpdates);
        Assert.Equal(1, sut.PartCheckList.StateRefreshCount - refreshes);
        Assert.True(sut.GetItem("k500")!.IsChecked);
    }

    [StaFact]
    public void CheckListBox_select_all_refreshes_the_states_once_and_notifies_once()
    {
        var sut = new CheckListBox { Items = Items() };
        var notifications = Record(sut.SelectedKeys);
        var refreshes = sut.StateRefreshCount;

        sut.SelectAll();

        Assert.Equal(ItemCount, sut.SelectedKeys.Count);
        Assert.Equal(1, sut.StateRefreshCount - refreshes);
        Assert.Equal([NotifyCollectionChangedAction.Reset], notifications);
    }

    [StaFact]
    public void CheckListBox_range_change_from_outside_refreshes_the_states_once()
    {
        var keys = new Atc.Wpf.Collections.ObservableCollectionEx<string>();
        var sut = new CheckListBox { Items = Items(), SelectedKeys = keys };
        var refreshes = sut.StateRefreshCount;

        keys.AddRange(Items().Keys);

        Assert.Equal(1, sut.StateRefreshCount - refreshes);
        Assert.True(sut.GetItem("k1")!.IsChecked);
        Assert.True(sut.GetItem("k1000")!.IsChecked);
    }

    [StaFact]
    public void CheckTreeView_select_all_notifies_once()
    {
        var sut = new CheckTreeView { Nodes = Tree() };
        var notifications = Record(sut.SelectedKeys);

        sut.SelectAll();

        Assert.Equal(ItemCount, sut.SelectedKeys.Count);
        Assert.Equal([NotifyCollectionChangedAction.Reset], notifications);
    }

    [StaFact]
    public void CheckTreeView_range_change_from_outside_applies_the_keys_once()
    {
        var keys = new Atc.Wpf.Collections.ObservableCollectionEx<string>();
        var sut = new CheckTreeView { Nodes = Tree(), SelectedKeys = keys };
        var applies = sut.KeysApplyCount;

        keys.AddRange(LeafKeys());

        Assert.Equal(1, sut.KeysApplyCount - applies);
        Assert.True(sut.FindNode("p0")!.IsChecked);
    }

    [StaFact]
    public void TreeSelectComboBox_select_all_updates_the_summary_once()
    {
        var sut = new TreeSelectComboBox { Nodes = Tree(), DisplayMode = MultiSelectDisplayMode.Chips };
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;
        var summaryUpdates = sut.SummaryUpdateCount;

        sut.SelectAll();

        Assert.Equal(ItemCount, sut.SelectedKeys.Count);
        Assert.Equal(1, sut.SummaryUpdateCount - summaryUpdates);
        Assert.Equal(1, events);
    }

    [StaFact]
    public void CascaderComboBox_select_all_updates_the_summary_once()
    {
        var keys = new System.Collections.ObjectModel.ObservableCollection<string>();
        var sut = new CascaderComboBox { Nodes = Tree(), CheckedKeysStrategy = CheckedKeysStrategy.Leaves, DisplayMode = MultiSelectDisplayMode.Chips, SelectedKeys = keys };
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;
        var summaryUpdates = sut.SummaryUpdateCount;

        sut.SelectAll();

        Assert.Equal(ItemCount, keys.Count);
        Assert.Equal(1, sut.SummaryUpdateCount - summaryUpdates);
        Assert.Equal(1, events);
    }

    private static Dictionary<string, string> Items()
        => Enumerable
            .Range(1, ItemCount)
            .ToDictionary(x => "k" + x.ToString(CultureInfo.InvariantCulture), x => "Item " + x.ToString(CultureInfo.InvariantCulture), StringComparer.Ordinal);

    private static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Tree()
    {
        var nodes = new System.Collections.ObjectModel.ObservableCollection<CheckTreeNode>();
        for (var p = 0; p < 50; p++)
        {
            var parent = new CheckTreeNode("p" + p.ToString(CultureInfo.InvariantCulture), "P" + p.ToString(CultureInfo.InvariantCulture));
            for (var c = 0; c < 20; c++)
            {
                parent.Children.Add(new CheckTreeNode(LeafKey(p, c), "C" + c.ToString(CultureInfo.InvariantCulture)));
            }

            nodes.Add(parent);
        }

        return nodes;
    }

    private static string LeafKey(
        int parent,
        int child)
        => string.Create(CultureInfo.InvariantCulture, $"p{parent}c{child}");

    private static IEnumerable<string> LeafKeys()
        => Enumerable
            .Range(0, 50)
            .SelectMany(p => Enumerable.Range(0, 20).Select(c => LeafKey(p, c)));

    private static List<NotifyCollectionChangedAction> Record(
        INotifyCollectionChanged collection)
    {
        var actions = new List<NotifyCollectionChangedAction>();
        collection.CollectionChanged += (_, e) => actions.Add(e.Action);
        return actions;
    }
}