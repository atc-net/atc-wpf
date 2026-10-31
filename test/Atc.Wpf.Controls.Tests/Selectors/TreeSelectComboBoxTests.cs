namespace Atc.Wpf.Controls.Tests.Selectors;

public sealed class TreeSelectComboBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_match_the_documented_api()
    {
        var sut = new TreeSelectComboBox();

        Assert.NotNull(sut.Nodes);
        Assert.Empty(sut.Nodes);
        Assert.NotNull(sut.SelectedKeys);
        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(CheckedKeysStrategy.Leaves, sut.CheckedKeysStrategy);
        Assert.True(sut.IsCheckLinked);
        Assert.Null(sut.IsFilterVisible);
        Assert.Equal(TreeFilterMode.KeepAncestors, sut.FilterMode);
        Assert.False(sut.ShowSelectAllRow);
        Assert.Null(sut.MaxSelectionCount);
        Assert.Equal(1, sut.ExpandDepth);
        Assert.Equal(MultiSelectDisplayMode.Text, sut.DisplayMode);
        Assert.Equal(2, sut.MaxNamesInSummary);
        Assert.False(sut.IsDropDownOpen);
        Assert.Equal(string.Empty, sut.SummaryText);
    }

    [StaFact]
    public void Options_are_passed_through_to_the_pop_up_tree()
    {
        var sut = Create();
        var template = new DataTemplate();
        Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>> loader =
            (_, _) => Task.FromResult<IReadOnlyList<CheckTreeNode>>([]);

        sut.CheckedKeysStrategy = CheckedKeysStrategy.Parents;
        sut.IsCheckLinked = false;
        sut.IsFilterVisible = true;
        sut.FilterText = "den";
        sut.FilterMode = TreeFilterMode.FlatResults;
        sut.ShowSelectAllRow = true;
        sut.MaxSelectionCount = 3;
        sut.ExpandDepth = 2;
        sut.ItemTemplate = template;
        sut.LoadChildrenAsync = loader;

        Assert.Same(sut.Nodes, sut.PartTree.Nodes);
        Assert.Same(sut.SelectedKeys, sut.PartTree.SelectedKeys);
        Assert.Equal(CheckedKeysStrategy.Parents, sut.PartTree.CheckedKeysStrategy);
        Assert.False(sut.PartTree.IsCheckLinked);
        Assert.True(sut.PartTree.IsFilterVisible);
        Assert.Equal("den", sut.PartTree.FilterText);
        Assert.Equal(TreeFilterMode.FlatResults, sut.PartTree.FilterMode);
        Assert.True(sut.PartTree.ShowSelectAllRow);
        Assert.Equal(3, sut.PartTree.MaxSelectionCount);
        Assert.Equal(2, sut.PartTree.ExpandDepth);
        Assert.Same(template, sut.PartTree.ItemTemplate);
        Assert.Same(loader, sut.PartTree.LoadChildrenAsync);
    }

    [StaFact]
    public void Ticking_in_the_pop_up_updates_SelectedKeys_with_one_event()
    {
        var sut = Create();
        sut.Name = "Regions";
        var events = Record(sut);
        object? sender = null;
        sut.SelectedKeysChanged += (s, _) => sender = s;

        sut.ToggleNode(sut.FindNode("dk")!);

        Assert.Equal(["dk-aarhus", "dk-cph", "dk-odense"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Same(sut, sender);
        Assert.Equal("Regions", change.Identifier);
        Assert.Empty(change.OldValue);
        Assert.Equal(3, change.NewValue.Count);
    }

    [StaTheory]
    [InlineData(CheckedKeysStrategy.Leaves, "3 of 5 selected")]
    [InlineData(CheckedKeysStrategy.Parents, "Denmark")]
    [InlineData(CheckedKeysStrategy.All, "3 of 5 selected")]
    public void Text_summary_lists_names_up_to_the_limit_then_counts_the_leaves(
        CheckedKeysStrategy strategy,
        string expected)
    {
        var sut = Create();
        sut.CheckedKeysStrategy = strategy;

        sut.ToggleNode(sut.FindNode("dk")!);

        Assert.Equal(expected, sut.SummaryText);
    }

    [StaFact]
    public void Summary_shows_names_none_and_all()
    {
        var sut = Create();

        Assert.Equal(string.Empty, sut.SummaryText);

        sut.SelectedKeys = ["dk-cph", "se-got"];

        Assert.Equal("Copenhagen, Gothenburg", sut.SummaryText);

        sut.SelectAll();

        Assert.Equal("All", sut.SummaryText);
    }

    [StaFact]
    public void Count_mode_shows_the_count_of_ticked_leaves()
    {
        var sut = Create();
        sut.DisplayMode = MultiSelectDisplayMode.Count;

        sut.SelectedKeys = ["dk-cph"];

        Assert.Equal("1 of 5 selected", sut.SummaryText);
    }

    [StaFact]
    public void Chips_mode_shows_one_removable_chip_per_key_and_removing_unticks_the_node()
    {
        var sut = Create();
        sut.CheckedKeysStrategy = CheckedKeysStrategy.Parents;
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.ToggleNode(sut.FindNode("dk")!);
        sut.ToggleNode(sut.FindNode("se-sto")!);
        var events = Record(sut);

        var chips = sut.GetChipItems();
        Assert.Equal(["Denmark", "Stockholm"], chips.Select(x => x.DisplayValue).ToList());

        chips[0].RemoveCommand.Execute(parameter: null);

        Assert.Equal(["se-sto"], sut.SelectedKeys);
        Assert.False(sut.FindNode("dk-cph")!.IsChecked);
        Assert.Single(events);
    }

    [StaFact]
    public void A_disabled_node_has_a_chip_that_cannot_be_removed()
    {
        var sut = Create();
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.SelectedKeys = ["dk-cph"];
        sut.FindNode("dk-cph")!.IsEnabled = false;
        sut.SelectedKeys.Add("se-sto");

        var chip = sut.GetChipItems().Single(x => x.Key == "dk-cph");

        Assert.False(chip.CanRemove);
    }

    [StaFact]
    public void Summary_follows_a_new_Nodes_instance()
    {
        var sut = new TreeSelectComboBox
        {
            SelectedKeys = ["dk-cph"],
        };

        Assert.Equal("dk-cph", sut.SummaryText);

        sut.Nodes = CheckTreeStateTests.CreateRegionNodes();

        Assert.Equal("Copenhagen", sut.SummaryText);
    }

    [StaTheory]
    [InlineData(Key.F4, Key.None, ModifierKeys.None, false, true)]
    [InlineData(Key.F4, Key.None, ModifierKeys.None, true, false)]
    [InlineData(Key.System, Key.Down, ModifierKeys.Alt, false, true)]
    [InlineData(Key.System, Key.Up, ModifierKeys.Alt, true, false)]
    [InlineData(Key.Escape, Key.None, ModifierKeys.None, true, false)]
    [InlineData(Key.Enter, Key.None, ModifierKeys.None, true, false)]
    public void Keyboard_opens_and_closes_the_pop_up(
        Key key,
        Key systemKey,
        ModifierKeys modifiers,
        bool isOpen,
        bool expectedOpen)
    {
        var sut = Create();
        sut.IsDropDownOpen = isOpen;

        var handled = sut.HandleKey(key, systemKey, modifiers);

        Assert.True(handled);
        Assert.Equal(expectedOpen, sut.IsDropDownOpen);
    }

    [StaFact]
    public void IsDropDownOpen_opens_and_closes_the_pop_up()
    {
        var sut = Create();
        var opened = 0;
        var closed = 0;
        sut.DropDownOpened += (_, _) => opened++;
        sut.DropDownClosed += (_, _) => closed++;
        var window = new Window
        {
            Content = sut,
            Width = 300,
            Height = 100,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };
        window.Show();
        try
        {
            sut.IsDropDownOpen = true;
            Assert.True(sut.PartPopup.IsOpen);

            sut.ToggleNode(sut.FindNode("se")!);
            Assert.True(sut.IsDropDownOpen);

            sut.IsDropDownOpen = false;
            Assert.False(sut.PartPopup.IsOpen);
        }
        finally
        {
            sut.IsDropDownOpen = false;
            window.Close();
        }

        Assert.Equal(1, opened);
        Assert.Equal(1, closed);
    }

    [StaFact]
    public void Automation_peer_is_a_combo_box_with_ExpandCollapse_and_the_summary_as_value()
    {
        var sut = Create();
        sut.SelectedKeys = ["dk-cph"];
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.Equal(AutomationControlType.ComboBox, peer.GetAutomationControlType());

        var value = (IValueProvider)peer.GetPattern(PatternInterface.Value);
        Assert.Equal("Copenhagen", value.Value);
        Assert.True(value.IsReadOnly);

        var expandCollapse = (IExpandCollapseProvider)peer.GetPattern(PatternInterface.ExpandCollapse);
        expandCollapse.Expand();
        Assert.True(sut.IsDropDownOpen);
        Assert.Contains(peer.GetChildren() ?? [], x => x is FrameworkElementAutomationPeer { Owner: CheckTreeView });

        expandCollapse.Collapse();
        Assert.False(sut.IsDropDownOpen);
    }

    [StaFact]
    public void Escape_clears_the_filter_first_then_closes()
    {
        var sut = Create();
        sut.IsDropDownOpen = true;
        sut.FilterText = "co";

        Assert.True(sut.HandleKey(Key.Escape, Key.None, ModifierKeys.None));
        Assert.Equal(string.Empty, sut.FilterText);
        Assert.True(sut.IsDropDownOpen);

        Assert.True(sut.HandleKey(Key.Escape, Key.None, ModifierKeys.None));
        Assert.False(sut.IsDropDownOpen);
    }

    private static TreeSelectComboBox Create()
        => new() { Nodes = CheckTreeStateTests.CreateRegionNodes() };

    private static List<ValueChangedEventArgs<IReadOnlyList<string>>> Record(
        TreeSelectComboBox sut)
    {
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);
        return events;
    }
}