namespace Atc.Wpf.Controls.Tests.Selectors;

/// <summary>
/// Swapping the DataContext between two view models with disjoint items must not let the stale-key cleanup of the
/// new items write into either view model's SelectedKeys, whichever binding updates first.
/// </summary>
public sealed class MultiSelectDataContextSwapTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckListBox_keeps_both_view_models_unchanged(bool itemsFirst)
    {
        var sut = new CheckListBox();
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        AssertSwapKeepsViewModels(sut, CheckListBox.ItemsProperty, CheckListBox.SelectedKeysProperty, itemsFirst, () => events);
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ChipGroup_keeps_both_view_models_unchanged(bool itemsFirst)
    {
        var sut = new ChipGroup();
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        AssertSwapKeepsViewModels(sut, ChipGroup.ItemsProperty, ChipGroup.SelectedKeysProperty, itemsFirst, () => events);
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ChipGroup_with_a_bound_selection_mode_keeps_both_view_models_unchanged(
        bool modeFirst)
    {
        var first = CreateFirst();
        var second = CreateSecond();
        second.SelectionMode = ChipSelectionMode.Single;
        var sut = new ChipGroup { DataContext = first };
        if (modeFirst)
        {
            Bind(sut, ChipGroup.SelectionModeProperty, nameof(ViewModel.SelectionMode));
        }

        Bind(sut, ChipGroup.ItemsProperty, nameof(ViewModel.Items));
        Bind(sut, ChipGroup.SelectedKeysProperty, nameof(ViewModel.SelectedKeys));
        if (!modeFirst)
        {
            Bind(sut, ChipGroup.SelectionModeProperty, nameof(ViewModel.SelectionMode));
        }

        Pump();
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        sut.DataContext = second;
        Pump();

        Assert.Equal(["a1", "a2"], first.SelectedKeys);
        Assert.Equal(["b2"], second.SelectedKeys);
        Assert.Equal(0, events);

        sut.DataContext = first;
        Pump();

        Assert.Equal(["a1", "a2"], first.SelectedKeys);
        Assert.Equal(["b2"], second.SelectedKeys);
        Assert.Equal(0, events);
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TokenBox_keeps_both_view_models_unchanged(bool itemsFirst)
    {
        var sut = new TokenBox();
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        AssertSwapKeepsViewModels(sut, TokenBox.ItemsProperty, TokenBox.SelectedKeysProperty, itemsFirst, () => events);
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void MultiSelectComboBox_keeps_both_view_models_unchanged(
        bool itemsFirst)
    {
        var sut = new MultiSelectComboBox();
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        AssertSwapKeepsViewModels(sut, MultiSelectComboBox.ItemsProperty, MultiSelectComboBox.SelectedKeysProperty, itemsFirst, () => events);
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void MultiColumnComboBox_keeps_both_view_models_unchanged(
        bool itemsFirst)
    {
        var sut = new MultiColumnComboBox { KeyMemberPath = nameof(Row.Key), DisplayMemberPath = nameof(Row.Name) };
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        AssertSwapKeepsViewModels(
            sut,
            MultiColumnComboBox.ItemsSourceProperty,
            MultiColumnComboBox.SelectedKeysProperty,
            itemsFirst,
            () => events,
            nameof(ViewModel.Rows));
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckTreeView_keeps_both_view_models_unchanged(bool itemsFirst)
    {
        var sut = new CheckTreeView();
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        AssertSwapKeepsViewModels(sut, CheckTreeView.NodesProperty, CheckTreeView.SelectedKeysProperty, itemsFirst, () => events, nameof(ViewModel.Nodes));
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeSelectComboBox_keeps_both_view_models_unchanged(
        bool itemsFirst)
    {
        var sut = new TreeSelectComboBox();
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        AssertSwapKeepsViewModels(sut, TreeSelectComboBox.NodesProperty, TreeSelectComboBox.SelectedKeysProperty, itemsFirst, () => events, nameof(ViewModel.Nodes));
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CascaderComboBox_keeps_both_view_models_unchanged(
        bool itemsFirst)
    {
        var sut = new CascaderComboBox();
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        AssertSwapKeepsViewModels(sut, CascaderComboBox.NodesProperty, CascaderComboBox.SelectedKeysProperty, itemsFirst, () => events, nameof(ViewModel.Nodes));
    }

    [StaFact]
    public void Bound_CheckListBox_drops_stale_keys_in_one_event_once_the_bindings_settle()
    {
        var first = CreateFirst();
        var sut = new CheckListBox { DataContext = first };
        Bind(sut, CheckListBox.ItemsProperty, nameof(ViewModel.Items));
        Bind(sut, CheckListBox.SelectedKeysProperty, nameof(ViewModel.SelectedKeys));
        Pump();
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        sut.Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["a1"] = "A1" };
        sut.Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["a1"] = "A1", ["a9"] = "A9" };
        Pump();

        Assert.Equal(["a1"], first.SelectedKeys);
        Assert.Equal(1, events);
    }

    private static void AssertSwapKeepsViewModels(
        FrameworkElement sut,
        DependencyProperty itemsProperty,
        DependencyProperty selectedKeysProperty,
        bool itemsFirst,
        Func<int> events,
        string itemsPath = nameof(ViewModel.Items))
    {
        var first = CreateFirst();
        var second = CreateSecond();
        sut.DataContext = first;
        if (itemsFirst)
        {
            Bind(sut, itemsProperty, itemsPath);
            Bind(sut, selectedKeysProperty, nameof(ViewModel.SelectedKeys));
        }
        else
        {
            Bind(sut, selectedKeysProperty, nameof(ViewModel.SelectedKeys));
            Bind(sut, itemsProperty, itemsPath);
        }

        Pump();
        var eventsBefore = events();

        sut.DataContext = second;
        Pump();

        Assert.Same(second.SelectedKeys, sut.GetValue(selectedKeysProperty));
        Assert.Equal(["a1", "a2"], first.SelectedKeys);
        Assert.Equal(["b2"], second.SelectedKeys);
        Assert.Equal(eventsBefore, events());

        sut.DataContext = first;
        Pump();

        Assert.Same(first.SelectedKeys, sut.GetValue(selectedKeysProperty));
        Assert.Equal(["a1", "a2"], first.SelectedKeys);
        Assert.Equal(["b2"], second.SelectedKeys);
        Assert.Equal(eventsBefore, events());
    }

    private static void Bind(
        FrameworkElement target,
        DependencyProperty property,
        string path)
        => BindingOperations.SetBinding(target, property, new Binding(path));

    private static void Pump()
    {
        var frame = new DispatcherFrame();
        _ = Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Background,
            () => frame.Continue = false);
        Dispatcher.PushFrame(frame);
    }

    private static ViewModel CreateFirst()
        => new(
            new Dictionary<string, string>(StringComparer.Ordinal) { ["a1"] = "A1", ["a2"] = "A2", ["a3"] = "A3" },
            ["a1", "a2"]);

    private static ViewModel CreateSecond()
        => new(
            new Dictionary<string, string>(StringComparer.Ordinal) { ["b1"] = "B1", ["b2"] = "B2" },
            ["b2"]);

    private sealed record Row(string Key, string Name);

    private sealed class ViewModel
    {
        public ViewModel(
            Dictionary<string, string> items,
            IEnumerable<string> selectedKeys)
        {
            Items = items;
            Rows = items.Select(x => new Row(x.Key, x.Value)).ToList();
            Nodes = new System.Collections.ObjectModel.ObservableCollection<CheckTreeNode>(
                items.Select(x => new CheckTreeNode(x.Key, x.Value)));
            SelectedKeys = new System.Collections.ObjectModel.ObservableCollection<string>(selectedKeys);
        }

        public Dictionary<string, string> Items { get; set; }

        public IReadOnlyList<Row> Rows { get; set; }

        public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Nodes { get; set; }

        public System.Collections.ObjectModel.ObservableCollection<string> SelectedKeys { get; set; }

        public ChipSelectionMode SelectionMode { get; set; } = ChipSelectionMode.Multiple;
    }
}