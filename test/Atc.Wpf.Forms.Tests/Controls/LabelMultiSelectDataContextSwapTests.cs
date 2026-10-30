namespace Atc.Wpf.Forms.Tests.Controls;

/// <summary>
/// The Label wrappers forward Items and SelectedKeys to an inner control: swapping the DataContext between two view
/// models with disjoint items must not let the inner control's stale-key cleanup write into either view model.
/// </summary>
public sealed class LabelMultiSelectDataContextSwapTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void LabelCheckListBox_keeps_both_view_models_unchanged(
        bool itemsFirst)
        => AssertSwapKeepsViewModels(
            new LabelCheckListBox(),
            LabelCheckListBox.ItemsProperty,
            LabelCheckListBox.SelectedKeysProperty,
            itemsFirst);

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void LabelChipGroup_keeps_both_view_models_unchanged(bool itemsFirst)
        => AssertSwapKeepsViewModels(
            new LabelChipGroup(),
            LabelChipGroup.ItemsProperty,
            LabelChipGroup.SelectedKeysProperty,
            itemsFirst);

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void LabelTokenBox_keeps_both_view_models_unchanged(bool itemsFirst)
        => AssertSwapKeepsViewModels(
            new LabelTokenBox(),
            LabelTokenBox.ItemsProperty,
            LabelTokenBox.SelectedKeysProperty,
            itemsFirst);

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void LabelMultiSelectComboBox_keeps_both_view_models_unchanged(
        bool itemsFirst)
        => AssertSwapKeepsViewModels(
            new LabelMultiSelectComboBox(),
            LabelMultiSelectComboBox.ItemsProperty,
            LabelMultiSelectComboBox.SelectedKeysProperty,
            itemsFirst);

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void LabelMultiColumnComboBox_keeps_both_view_models_unchanged(
        bool itemsFirst)
        => AssertSwapKeepsViewModels(
            new LabelMultiColumnComboBox { KeyMemberPath = nameof(Row.Key), DisplayMemberPath = nameof(Row.Name) },
            LabelMultiColumnComboBox.ItemsSourceProperty,
            LabelMultiColumnComboBox.SelectedKeysProperty,
            itemsFirst,
            nameof(ViewModel.Rows));

    [StaFact]
    public void Unbound_LabelCheckListBox_drops_stale_keys_at_once()
    {
        var sut = new LabelCheckListBox
        {
            Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["a1"] = "A1", ["a2"] = "A2" },
        };
        sut.SelectedKeys.Add("a1");
        sut.SelectedKeys.Add("a2");

        sut.Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["a1"] = "A1" };

        Assert.Equal(["a1"], sut.SelectedKeys);
    }

    private static void AssertSwapKeepsViewModels(
        FrameworkElement sut,
        DependencyProperty itemsProperty,
        DependencyProperty selectedKeysProperty,
        bool itemsFirst,
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

        sut.DataContext = second;
        Pump();

        Assert.Same(second.SelectedKeys, sut.GetValue(selectedKeysProperty));
        Assert.Equal(["a1", "a2"], first.SelectedKeys);
        Assert.Equal(["b2"], second.SelectedKeys);

        sut.DataContext = first;
        Pump();

        Assert.Same(first.SelectedKeys, sut.GetValue(selectedKeysProperty));
        Assert.Equal(["a1", "a2"], first.SelectedKeys);
        Assert.Equal(["b2"], second.SelectedKeys);
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
            SelectedKeys = new System.Collections.ObjectModel.ObservableCollection<string>(selectedKeys);
        }

        public Dictionary<string, string> Items { get; set; }

        public IReadOnlyList<Row> Rows { get; set; }

        public System.Collections.ObjectModel.ObservableCollection<string> SelectedKeys { get; set; }
    }
}