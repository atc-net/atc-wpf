namespace Atc.Wpf.Controls.Tests.Selectors;

/// <summary>
/// The multi-select controls listen to the key, node and row collections they are given. A long-lived collection
/// (a cached or singleton view model, a static node tree) must not keep a dropped control alive.
/// </summary>
[Collection(nameof(PackResourceTestGroup))]
public sealed class MultiSelectCollectionLeakTests : IDisposable
{
    public static TheoryData<string> ControlNames
        => new(
            nameof(CheckListBox),
            nameof(MultiSelectComboBox),
            nameof(TokenBox),
            nameof(ChipGroup),
            nameof(Segmented),
            nameof(CheckTreeView),
            nameof(TreeSelectComboBox),
            nameof(CascaderComboBox),
            nameof(MultiColumnComboBox),
            nameof(CardSelector));

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaTheory]
    [MemberData(nameof(ControlNames))]
    public void Control_without_given_collections_can_be_collected(
        string controlName)
    {
        var reference = Create(controlName, inputs: null);

        Assert.True(ControlLeakProbe.IsCollected(reference));
    }

    [StaTheory]
    [MemberData(nameof(ControlNames))]
    public void Control_given_long_lived_collections_can_be_collected(
        string controlName)
    {
        var inputs = new LongLivedInputs();

        var reference = Create(controlName, inputs);

        Assert.True(ControlLeakProbe.IsCollected(reference));
        GC.KeepAlive(inputs);
    }

    [StaTheory]
    [MemberData(nameof(ControlNames))]
    public void Loaded_then_unloaded_control_given_long_lived_collections_can_be_collected(
        string controlName)
    {
        var inputs = new LongLivedInputs();

        var reference = Create(controlName, inputs, raiseLoadedAndUnloaded: true);

        Assert.True(ControlLeakProbe.IsCollected(reference));
        GC.KeepAlive(inputs);
    }

    [StaFact]
    public void Control_still_follows_its_long_lived_collection()
    {
        // Guards against "fixing" the leak by not listening at all.
        var inputs = new LongLivedInputs();
        var sut = new CheckListBox { Items = inputs.Items, SelectedKeys = inputs.SelectedKeys };

        inputs.SelectedKeys.Add("k2");

        Assert.True(sut.GetItem("k2")!.IsChecked);
    }

    [StaFact]
    public void Tree_control_still_follows_its_long_lived_nodes()
    {
        var inputs = new LongLivedInputs();
        var sut = new CheckTreeView { Nodes = inputs.Nodes, SelectedKeys = inputs.SelectedKeys };

        inputs.Nodes[0].Children.Add(new CheckTreeNode("k1-new", "New"));
        inputs.SelectedKeys.Add("k1-new");

        Assert.True(sut.FindNode("k1-new")!.IsChecked);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference Create(
        string controlName,
        LongLivedInputs? inputs,
        bool raiseLoadedAndUnloaded = false)
    {
        FrameworkElement control = controlName switch
        {
            nameof(CheckListBox) => inputs is null
                ? new CheckListBox()
                : new CheckListBox { Items = inputs.Items, SelectedKeys = inputs.SelectedKeys, DisabledKeys = inputs.DisabledKeys },
            nameof(MultiSelectComboBox) => inputs is null
                ? new MultiSelectComboBox()
                : new MultiSelectComboBox { Items = inputs.Items, SelectedKeys = inputs.SelectedKeys, DisabledKeys = inputs.DisabledKeys },
            nameof(TokenBox) => inputs is null
                ? new TokenBox()
                : new TokenBox { Items = inputs.Items, SelectedKeys = inputs.SelectedKeys, FixedKeys = inputs.DisabledKeys },
            nameof(ChipGroup) => inputs is null
                ? new ChipGroup()
                : new ChipGroup { Items = inputs.Items, SelectedKeys = inputs.SelectedKeys, DisabledKeys = inputs.DisabledKeys },
            nameof(Segmented) => CreateSegmented(inputs),
            nameof(CheckTreeView) => inputs is null
                ? new CheckTreeView()
                : new CheckTreeView { Nodes = inputs.Nodes, SelectedKeys = inputs.SelectedKeys },
            nameof(TreeSelectComboBox) => inputs is null
                ? new TreeSelectComboBox()
                : new TreeSelectComboBox { Nodes = inputs.Nodes, SelectedKeys = inputs.SelectedKeys },
            nameof(CascaderComboBox) => inputs is null
                ? new CascaderComboBox()
                : new CascaderComboBox { Nodes = inputs.Nodes, SelectedKeys = inputs.SelectedKeys },
            nameof(MultiColumnComboBox) => inputs is null
                ? new MultiColumnComboBox()
                : new MultiColumnComboBox { KeyMemberPath = nameof(Row.Key), ItemsSource = inputs.Rows, SelectedKeys = inputs.SelectedKeys },
            nameof(CardSelector) => inputs is null
                ? new CardSelector()
                : new CardSelector
                {
                    SelectionMode = CardSelectionMode.Multiple,
                    KeyMemberPath = nameof(Row.Key),
                    ItemsSource = inputs.Rows,
                    SelectedKeys = inputs.SelectedKeys,
                },
            _ => throw new ArgumentOutOfRangeException(nameof(controlName), controlName, message: null),
        };

        if (raiseLoadedAndUnloaded)
        {
            control.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
            control.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
        }

        return new WeakReference(control);
    }

    private static Segmented CreateSegmented(LongLivedInputs? inputs)
    {
        var segmented = new Segmented { SelectionMode = SegmentedSelectionMode.Multiple };
        foreach (var key in new[] { "k1", "k2", "k3" })
        {
            segmented.Items.Add(new SegmentedItem { Key = key, Content = key });
        }

        if (inputs is not null)
        {
            segmented.SelectedKeys = inputs.SelectedKeys;
        }

        return segmented;
    }

    private sealed record Row(string Key, string Name);

    private sealed class LongLivedInputs
    {
        public Dictionary<string, string> Items { get; } = new(StringComparer.Ordinal)
        {
            ["k1"] = "One",
            ["k2"] = "Two",
            ["k3"] = "Three",
        };

        public System.Collections.ObjectModel.ObservableCollection<string> SelectedKeys { get; } = ["k1"];

        public System.Collections.ObjectModel.ObservableCollection<string> DisabledKeys { get; } = ["k3"];

        public System.Collections.ObjectModel.ObservableCollection<Row> Rows { get; } =
        [
            new("k1", "One"),
            new("k2", "Two"),
            new("k3", "Three"),
        ];

        public System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Nodes { get; } = CreateNodes();

        private static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> CreateNodes()
        {
            var parent = new CheckTreeNode("k1", "One");
            parent.Children.Add(new CheckTreeNode("k1-a", "One A"));
            parent.Children.Add(new CheckTreeNode("k1-b", "One B"));
            return [parent, new CheckTreeNode("k2", "Two")];
        }
    }
}