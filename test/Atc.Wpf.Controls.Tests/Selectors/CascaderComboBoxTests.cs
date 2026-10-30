namespace Atc.Wpf.Controls.Tests.Selectors;

public sealed class CascaderComboBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_match_the_documented_api()
    {
        var sut = new CascaderComboBox();

        Assert.NotNull(sut.Nodes);
        Assert.Empty(sut.Nodes);
        Assert.NotNull(sut.SelectedKeys);
        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(CheckedKeysStrategy.Parents, sut.CheckedKeysStrategy);
        Assert.True(sut.IsCheckLinked);
        Assert.Equal(CascaderExpandTrigger.Click, sut.ExpandTrigger);
        Assert.Equal(180d, sut.ColumnWidth);
        Assert.Null(sut.IsFilterVisible);
        Assert.Equal(string.Empty, sut.FilterText);
        Assert.Null(sut.MaxSelectionCount);
        Assert.Null(sut.LoadChildrenAsync);
        Assert.Equal(" / ", sut.PathSeparator);
        Assert.False(sut.ShowFullPath);
        Assert.Null(sut.ItemTemplate);
        Assert.Equal(MultiSelectDisplayMode.Text, sut.DisplayMode);
        Assert.Equal(2, sut.MaxNamesInSummary);
        Assert.False(sut.IsDropDownOpen);
        Assert.Equal(string.Empty, sut.SummaryText);
    }

    [StaFact]
    public void The_default_strategy_reaches_the_engine()
    {
        var sut = Create();

        sut.ToggleNode(sut.FindNode("dk-jut")!);

        Assert.Equal(["dk-jut"], sut.SelectedKeys);
    }

    [StaFact]
    public void The_first_column_shows_the_roots()
    {
        var sut = Create();

        var column = Assert.Single(sut.PartColumns.Columns);
        Assert.Equal(["dk", "se"], Keys(column));
        Assert.Empty(sut.PartColumns.ActivePath);
    }

    [StaFact]
    public void Activating_a_row_shows_its_children_in_the_next_column()
    {
        var sut = Create();

        sut.PartColumns.Activate(sut.FindNode("dk")!);
        sut.PartColumns.Activate(sut.FindNode("dk-jut")!);

        Assert.Equal(3, sut.PartColumns.Columns.Count);
        Assert.Equal(["dk-jut", "dk-zea"], Keys(sut.PartColumns.Columns[1]));
        Assert.Equal(["dk-jut-aar", "dk-jut-aal"], Keys(sut.PartColumns.Columns[2]));
        Assert.Equal(["dk", "dk-jut"], sut.PartColumns.ActivePath.Select(x => x.Key).ToList());
    }

    [StaFact]
    public void Activating_a_row_higher_up_drops_the_deeper_columns()
    {
        var sut = Create();
        sut.PartColumns.Activate(sut.FindNode("dk")!);
        sut.PartColumns.Activate(sut.FindNode("dk-jut")!);

        sut.PartColumns.Activate(sut.FindNode("se")!);

        Assert.Equal(2, sut.PartColumns.Columns.Count);
        Assert.Equal(["se-sve"], Keys(sut.PartColumns.Columns[1]));
        Assert.Equal(["se"], sut.PartColumns.ActivePath.Select(x => x.Key).ToList());
    }

    [StaFact]
    public void Activating_a_leaf_does_not_add_a_column()
    {
        var sut = Create();
        sut.PartColumns.Activate(sut.FindNode("dk")!);
        sut.PartColumns.Activate(sut.FindNode("dk-jut")!);

        sut.PartColumns.Activate(sut.FindNode("dk-jut-aar")!);

        Assert.Equal(3, sut.PartColumns.Columns.Count);
        Assert.Equal(["dk", "dk-jut"], sut.PartColumns.ActivePath.Select(x => x.Key).ToList());
    }

    [StaFact]
    public void A_click_on_the_text_activates_a_parent_and_a_click_on_the_check_box_or_a_leaf_ticks()
    {
        var sut = Create();
        var view = sut.PartColumns;
        var firstColumn = view.ColumnLists[0];

        view.OnRowClicked(firstColumn, sut.FindNode("dk")!, isCheckArea: false);

        Assert.Equal(["dk"], view.ActivePath.Select(x => x.Key).ToList());
        Assert.Empty(sut.SelectedKeys);

        view.OnRowClicked(firstColumn, sut.FindNode("se")!, isCheckArea: true);

        Assert.Equal(["se"], sut.SelectedKeys);
        Assert.Equal(["dk"], view.ActivePath.Select(x => x.Key).ToList());

        view.Activate(sut.FindNode("dk-jut")!);
        view.OnRowClicked(view.ColumnLists[2], sut.FindNode("dk-jut-aar")!, isCheckArea: false);

        Assert.Equal(["se", "dk-jut-aar"], sut.SelectedKeys);
    }

    [StaFact]
    public void Ticking_in_a_deep_column_makes_the_ancestors_partial()
    {
        var sut = Create();

        sut.ToggleNode(sut.FindNode("dk-jut-aar")!);

        Assert.Null(sut.FindNode("dk-jut")!.IsChecked);
        Assert.Null(sut.FindNode("dk")!.IsChecked);
        Assert.False(sut.FindNode("se")!.IsChecked);
    }

    [StaTheory]
    [InlineData(CheckedKeysStrategy.Parents, "dk-jut,dk-zea-cph")]
    [InlineData(CheckedKeysStrategy.Leaves, "dk-jut-aar,dk-jut-aal,dk-zea-cph")]
    [InlineData(CheckedKeysStrategy.All, "dk-jut,dk-jut-aar,dk-jut-aal,dk-zea-cph")]
    public void The_strategies_give_the_expected_keys(
        CheckedKeysStrategy strategy,
        string expected)
    {
        ArgumentNullException.ThrowIfNull(expected);

        var sut = Create();
        sut.CheckedKeysStrategy = strategy;

        sut.ToggleNode(sut.FindNode("dk-jut-aar")!);
        sut.ToggleNode(sut.FindNode("dk-jut-aal")!);
        sut.ToggleNode(sut.FindNode("dk-zea-cph")!);

        Assert.Equal(
            expected.Split(',').Order(StringComparer.Ordinal).ToList(),
            sut.SelectedKeys.Order(StringComparer.Ordinal).ToList());
    }

    [StaFact]
    public void Ticking_a_parent_with_many_descendants_raises_one_event()
    {
        var sut = Create();
        sut.Name = "Regions";
        var events = Record(sut);
        object? sender = null;
        sut.SelectedKeysChanged += (s, _) => sender = s;

        sut.ToggleNode(sut.FindNode("dk")!);

        Assert.Equal(["dk"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Same(sut, sender);
        Assert.Equal("Regions", change.Identifier);
        Assert.Empty(change.OldValue);
        Assert.Equal(["dk"], change.NewValue);
        Assert.True(sut.FindNode("dk-zea-ros")!.IsChecked);
    }

    [StaFact]
    public void Setting_SelectedKeys_from_code_rebuilds_the_states_without_events()
    {
        var sut = Create();
        var events = Record(sut);

        sut.SelectedKeys = ["dk-jut"];

        Assert.True(sut.FindNode("dk-jut-aal")!.IsChecked);
        Assert.Null(sut.FindNode("dk")!.IsChecked);

        sut.SelectedKeys.Add("se");

        Assert.True(sut.FindNode("se-sve-sto")!.IsChecked);
        Assert.Empty(events);
    }

    [StaFact]
    public void Filter_shows_a_flat_list_with_paths_across_levels()
    {
        var sut = Create();
        sut.PartColumns.Activate(sut.FindNode("dk")!);

        sut.FilterText = "a";

        Assert.True(sut.PartColumns.IsShowingFilterResults);
        var results = sut.PartColumns.FilterResults;
        Assert.Contains(results, x => x.Key == "dk");
        Assert.Contains(results, x => x.Key == "dk-jut-aar");
        Assert.Contains(results, x => x.Key == "se-sve");
        Assert.DoesNotContain(results, x => x.Key == "se-sve-sto");
        Assert.Equal("Denmark / Jutland / Aarhus", sut.PartColumns.GetFullPath(sut.FindNode("dk-jut-aar")!));
    }

    [StaFact]
    public void Ticking_a_filter_result_ticks_its_sub_tree_and_clearing_returns_to_the_active_path()
    {
        var sut = Create();
        sut.PartColumns.Activate(sut.FindNode("dk")!);
        sut.PartColumns.Activate(sut.FindNode("dk-zea")!);
        sut.FilterText = "jut";
        var events = Record(sut);

        var result = Assert.Single(sut.PartColumns.FilterResults);
        sut.ToggleNode(result);

        Assert.Equal(["dk-jut"], sut.SelectedKeys);
        Assert.True(sut.FindNode("dk-jut-aal")!.IsChecked);
        Assert.Single(events);

        sut.FilterText = string.Empty;

        Assert.False(sut.PartColumns.IsShowingFilterResults);
        Assert.Equal(["dk", "dk-zea"], sut.PartColumns.ActivePath.Select(x => x.Key).ToList());
        Assert.Equal(3, sut.PartColumns.Columns.Count);
    }

    [StaFact]
    public void Filter_is_shown_automatically_above_twenty_nodes()
    {
        var sut = Create();

        Assert.False(sut.PartColumns.IsFilterShown);

        var big = new CheckTreeNode("big", "Big");
        for (var i = 0; i < 25; i++)
        {
            big.Children.Add(new CheckTreeNode("big-" + i.ToString(CultureInfo.InvariantCulture), "Item " + i.ToString(CultureInfo.InvariantCulture)));
        }

        sut.Nodes.Add(big);

        Assert.True(sut.PartColumns.IsFilterShown);

        sut.IsFilterVisible = false;

        Assert.False(sut.PartColumns.IsFilterShown);
    }

    [StaFact]
    public void Activating_a_lazy_row_shows_a_busy_row_until_its_children_load()
    {
        var lazy = new CheckTreeNode("lazy", "Lazy") { HasUnloadedChildren = true };
        var calls = 0;
        var sut = new CascaderComboBox
        {
            Nodes = [lazy],
            LoadChildrenAsync = (node, _) =>
            {
                calls++;
                return Task.FromResult<IReadOnlyList<CheckTreeNode>>(
                [
                    new CheckTreeNode(node.Key + "-a", "A"),
                    new CheckTreeNode(node.Key + "-b", "B"),
                ]);
            },
        };

        var load = sut.PartColumns.ActivateAsync(lazy);

        load.GetAwaiter().GetResult();

        Assert.Equal(1, calls);
        Assert.False(lazy.HasUnloadedChildren);
        Assert.Equal(["lazy-a", "lazy-b"], Keys(sut.PartColumns.Columns[1]));
    }

    [StaFact]
    public void Activating_another_row_cancels_the_pending_load()
    {
        var first = new CheckTreeNode("first", "First") { HasUnloadedChildren = true };
        var second = new CheckTreeNode("second", "Second") { HasUnloadedChildren = true };
        var pending = new Dictionary<string, TaskCompletionSource<IReadOnlyList<CheckTreeNode>>>(StringComparer.Ordinal);
        var tokens = new Dictionary<string, CancellationToken>(StringComparer.Ordinal);
        var sut = new CascaderComboBox
        {
            Nodes = [first, second],
            LoadChildrenAsync = (node, cancellationToken) =>
            {
                var source = new TaskCompletionSource<IReadOnlyList<CheckTreeNode>>();
                pending[node.Key] = source;
                tokens[node.Key] = cancellationToken;
                return source.Task;
            },
        };

        var firstLoad = sut.PartColumns.ActivateAsync(first);

        var busy = Assert.Single(sut.PartColumns.Columns[1]);
        Assert.True(sut.PartColumns.IsPlaceholder(busy));

        var secondLoad = sut.PartColumns.ActivateAsync(second);

        Assert.True(tokens["first"].IsCancellationRequested);
        Assert.False(tokens["second"].IsCancellationRequested);

        pending["first"].SetResult([new CheckTreeNode("first-a", "A")]);
        Pump();
        firstLoad.GetAwaiter().GetResult();

        Assert.Empty(first.Children);
        Assert.True(first.HasUnloadedChildren);

        pending["second"].SetResult([new CheckTreeNode("second-a", "A")]);
        Pump();
        secondLoad.GetAwaiter().GetResult();

        Assert.Equal(["second-a"], Keys(sut.PartColumns.Columns[1]));
    }

    [StaFact]
    public void Children_loaded_under_a_ticked_row_inherit_the_tick()
    {
        var lazy = new CheckTreeNode("lazy", "Lazy") { HasUnloadedChildren = true };
        var sut = new CascaderComboBox
        {
            CheckedKeysStrategy = CheckedKeysStrategy.Leaves,
            Nodes = [lazy],
            LoadChildrenAsync = (node, _) => Task.FromResult<IReadOnlyList<CheckTreeNode>>(
            [
                new CheckTreeNode(node.Key + "-a", "A"),
                new CheckTreeNode(node.Key + "-b", "B"),
            ]),
        };
        sut.ToggleNode(lazy);
        var events = Record(sut);

        sut.PartColumns.ActivateAsync(lazy).GetAwaiter().GetResult();

        Assert.True(lazy.Children[0].IsChecked);
        Assert.Equal(["lazy-a", "lazy-b"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void A_failing_load_drops_the_busy_column_and_tries_again_next_time()
    {
        var lazy = new CheckTreeNode("lazy", "Lazy") { HasUnloadedChildren = true };
        var calls = 0;
        var sut = new CascaderComboBox
        {
            Nodes = [lazy],
            LoadChildrenAsync = (_, _) =>
            {
                calls++;
                return Task.FromException<IReadOnlyList<CheckTreeNode>>(new InvalidOperationException("offline"));
            },
        };

        sut.PartColumns.ActivateAsync(lazy).GetAwaiter().GetResult();

        Assert.Single(sut.PartColumns.Columns);
        Assert.Empty(sut.PartColumns.ActivePath);
        Assert.True(lazy.HasUnloadedChildren);

        sut.PartColumns.ActivateAsync(lazy).GetAwaiter().GetResult();

        Assert.Equal(2, calls);
    }

    [StaFact]
    public void Keyboard_right_activates_and_moves_into_the_next_column_left_goes_back()
    {
        var sut = Create();
        var view = sut.PartColumns;
        view.FocusNode(sut.FindNode("dk")!);

        Assert.True(view.HandleKey(Key.Right));

        Assert.Equal(["dk"], view.ActivePath.Select(x => x.Key).ToList());
        Assert.Equal("dk-jut", view.FocusedNode?.Key);
        Assert.Equal(1, view.FocusedColumn);

        Assert.True(view.HandleKey(Key.Down));
        Assert.Equal("dk-zea", view.FocusedNode?.Key);

        Assert.True(view.HandleKey(Key.Down));
        Assert.Equal("dk-zea", view.FocusedNode?.Key);

        Assert.True(view.HandleKey(Key.Up));
        Assert.Equal("dk-jut", view.FocusedNode?.Key);

        Assert.True(view.HandleKey(Key.Left));
        Assert.Equal("dk", view.FocusedNode?.Key);
        Assert.Equal(0, view.FocusedColumn);
    }

    [StaFact]
    public void Keyboard_space_toggles_and_enter_activates_a_parent_or_toggles_a_leaf()
    {
        var sut = Create();
        var view = sut.PartColumns;
        view.FocusNode(sut.FindNode("dk")!);

        Assert.True(view.HandleKey(Key.Space));
        Assert.Equal(["dk"], sut.SelectedKeys);

        Assert.True(view.HandleKey(Key.Enter));
        Assert.Equal(["dk"], view.ActivePath.Select(x => x.Key).ToList());
        Assert.Equal("dk-jut", view.FocusedNode?.Key);

        Assert.True(view.HandleKey(Key.Enter));
        Assert.Equal("dk-jut-aar", view.FocusedNode?.Key);

        Assert.True(view.HandleKey(Key.Enter));
        Assert.False(sut.FindNode("dk-jut-aar")!.IsChecked);
        Assert.Equal(["dk-jut-aal", "dk-zea"], sut.SelectedKeys);
    }

    [StaFact]
    public void Escape_clears_the_filter_first_then_closes()
    {
        var sut = Create();
        sut.IsDropDownOpen = true;
        sut.FilterText = "aa";

        Assert.True(sut.HandleKey(Key.Escape, Key.None, ModifierKeys.None));
        Assert.Equal(string.Empty, sut.FilterText);
        Assert.True(sut.IsDropDownOpen);

        Assert.True(sut.HandleKey(Key.Escape, Key.None, ModifierKeys.None));
        Assert.False(sut.IsDropDownOpen);
    }

    [StaTheory]
    [InlineData(Key.F4, Key.None, ModifierKeys.None, false, true)]
    [InlineData(Key.F4, Key.None, ModifierKeys.None, true, false)]
    [InlineData(Key.System, Key.Down, ModifierKeys.Alt, false, true)]
    [InlineData(Key.System, Key.Up, ModifierKeys.Alt, true, false)]
    public void Keyboard_opens_and_closes_the_pop_up(
        Key key,
        Key systemKey,
        ModifierKeys modifiers,
        bool isOpen,
        bool expectedOpen)
    {
        var sut = Create();
        sut.IsDropDownOpen = isOpen;

        Assert.True(sut.HandleKey(key, systemKey, modifiers));
        Assert.Equal(expectedOpen, sut.IsDropDownOpen);

        sut.IsDropDownOpen = false;
    }

    [StaFact]
    public void Hover_trigger_activates_a_row_after_the_delay()
    {
        var sut = Create();
        sut.ExpandTrigger = CascaderExpandTrigger.Hover;
        var view = sut.PartColumns;

        view.OnRowHovered(sut.FindNode("dk")!);

        Assert.Empty(view.ActivePath);
        Assert.True(view.IsHoverPending);

        view.RunPendingHoverActivation();

        Assert.Equal(["dk"], view.ActivePath.Select(x => x.Key).ToList());
        Assert.False(view.IsHoverPending);
    }

    [StaFact]
    public void Click_trigger_ignores_hover()
    {
        var sut = Create();
        var view = sut.PartColumns;

        view.OnRowHovered(sut.FindNode("dk")!);

        Assert.False(view.IsHoverPending);
        Assert.Empty(view.ActivePath);
    }

    [StaFact]
    public void ShowFullPath_puts_the_path_in_the_summary_and_the_chips()
    {
        var sut = Create();
        sut.ShowFullPath = true;
        sut.SelectedKeys = ["dk-jut-aar"];

        Assert.Equal("Denmark / Jutland / Aarhus", sut.SummaryText);

        sut.PathSeparator = " > ";

        Assert.Equal("Denmark > Jutland > Aarhus", sut.SummaryText);

        sut.DisplayMode = MultiSelectDisplayMode.Chips;

        Assert.Equal(["Denmark > Jutland > Aarhus"], sut.GetChipItems().Select(x => x.DisplayValue).ToList());
    }

    [StaFact]
    public void Summary_shows_names_count_and_all()
    {
        var sut = Create();

        sut.SelectedKeys = ["dk-jut-aar"];
        Assert.Equal("Aarhus", sut.SummaryText);

        sut.SelectedKeys = ["dk-jut-aar", "dk-zea-cph", "se"];
        Assert.Equal("3 of 5 selected", sut.SummaryText);

        sut.SelectAll();
        Assert.Equal("All", sut.SummaryText);
    }

    [StaFact]
    public void Removing_a_chip_unticks_the_node_with_one_event()
    {
        var sut = Create();
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.SelectedKeys = ["dk-jut", "se"];
        var events = Record(sut);

        sut.GetChipItems()[0].RemoveCommand.Execute(parameter: null);

        Assert.Equal(["se"], sut.SelectedKeys);
        Assert.False(sut.FindNode("dk-jut-aar")!.IsChecked);
        Assert.Single(events);
    }

    [StaFact]
    public void The_limit_refuses_a_tick_beyond_MaxSelectionCount()
    {
        var sut = Create();
        sut.CheckedKeysStrategy = CheckedKeysStrategy.Leaves;
        sut.MaxSelectionCount = 2;
        sut.ToggleNode(sut.FindNode("dk-jut-aar")!);
        sut.ToggleNode(sut.FindNode("dk-jut-aal")!);
        var events = Record(sut);

        var changed = sut.ToggleNode(sut.FindNode("dk-zea-cph")!);

        Assert.False(changed);
        Assert.Equal(["dk-jut-aar", "dk-jut-aal"], sut.SelectedKeys);
        Assert.False(sut.FindNode("dk-zea-cph")!.IsChecked);
        Assert.True(sut.PartColumns.IsAtSelectionLimit);
        Assert.Empty(events);
    }

    [StaFact]
    public void Columns_are_virtualized_with_ten_thousand_rows()
    {
        var big = new CheckTreeNode("big", "Big");
        for (var i = 0; i < 10_000; i++)
        {
            big.Children.Add(new CheckTreeNode("n" + i.ToString(CultureInfo.InvariantCulture), "Node " + i.ToString(CultureInfo.InvariantCulture)));
        }

        var sut = new CascaderComboBox { Nodes = [big], MaxDropDownHeight = 300 };
        var window = CreateWindow(sut);
        window.Show();
        try
        {
            sut.IsDropDownOpen = true;
            sut.PartColumns.Activate(big);
            sut.PartColumns.UpdateLayout();

            var lists = sut.PartColumns.ColumnLists;
            Assert.Equal(2, lists.Count);
            var realized = 0;
            for (var i = 0; i < 10_000; i++)
            {
                if (lists[1].ItemContainerGenerator.ContainerFromIndex(i) is not null)
                {
                    realized++;
                }
            }

            Assert.InRange(realized, 1, 200);
        }
        finally
        {
            sut.IsDropDownOpen = false;
            window.Close();
        }
    }

    [StaFact]
    public void IsDropDownOpen_opens_and_closes_the_pop_up_with_events()
    {
        var sut = Create();
        var opened = 0;
        var closed = 0;
        sut.DropDownOpened += (_, _) => opened++;
        sut.DropDownClosed += (_, _) => closed++;
        var window = CreateWindow(sut);
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
        sut.SelectedKeys = ["dk-jut-aar"];
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.Equal(AutomationControlType.ComboBox, peer.GetAutomationControlType());

        var value = (IValueProvider)peer.GetPattern(PatternInterface.Value);
        Assert.Equal("Aarhus", value.Value);
        Assert.True(value.IsReadOnly);

        var expandCollapse = (IExpandCollapseProvider)peer.GetPattern(PatternInterface.ExpandCollapse);
        expandCollapse.Expand();
        Assert.True(sut.IsDropDownOpen);
        Assert.NotEmpty(peer.GetChildren() ?? []);

        expandCollapse.Collapse();
        Assert.False(sut.IsDropDownOpen);
    }

    [StaFact]
    public void Rows_are_check_boxes_with_Toggle_and_parents_expose_ExpandCollapse()
    {
        var sut = Create();
        sut.SelectedKeys = ["dk-jut-aar"];
        var window = CreateWindow(sut);
        window.Show();
        try
        {
            sut.IsDropDownOpen = true;
            sut.PartColumns.UpdateLayout();
            var listPeer = UIElementAutomationPeer.CreatePeerForElement(sut.PartColumns.ColumnLists[0]);

            Assert.Equal(AutomationControlType.List, listPeer.GetAutomationControlType());
            var rows = listPeer.GetChildren();
            var denmark = rows.Single(x => x.GetName() == "Denmark");
            Assert.Equal(AutomationControlType.CheckBox, denmark.GetAutomationControlType());

            var toggle = (IToggleProvider)denmark.GetPattern(PatternInterface.Toggle);
            Assert.Equal(System.Windows.Automation.ToggleState.Indeterminate, toggle.ToggleState);

            var expandCollapse = (IExpandCollapseProvider)denmark.GetPattern(PatternInterface.ExpandCollapse);
            Assert.Equal(System.Windows.Automation.ExpandCollapseState.Collapsed, expandCollapse.ExpandCollapseState);

            expandCollapse.Expand();

            Assert.Equal(System.Windows.Automation.ExpandCollapseState.Expanded, expandCollapse.ExpandCollapseState);
            Assert.Equal(2, sut.PartColumns.Columns.Count);

            sut.PartColumns.Activate(sut.FindNode("dk-jut")!);
            sut.PartColumns.UpdateLayout();
            var leafList = UIElementAutomationPeer.CreatePeerForElement(sut.PartColumns.ColumnLists[2]);
            var aarhus = leafList.GetChildren().Single(x => x.GetName() == "Aarhus");
            Assert.Null(aarhus.GetPattern(PatternInterface.ExpandCollapse));

            ((IToggleProvider)aarhus.GetPattern(PatternInterface.Toggle)).Toggle();

            Assert.Empty(sut.SelectedKeys);
        }
        finally
        {
            sut.IsDropDownOpen = false;
            window.Close();
        }
    }

    internal static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> CreateCascaderRegions()
    {
        var jutland = new CheckTreeNode("dk-jut", "Jutland");
        jutland.Children.Add(new CheckTreeNode("dk-jut-aar", "Aarhus"));
        jutland.Children.Add(new CheckTreeNode("dk-jut-aal", "Aalborg"));

        var zealand = new CheckTreeNode("dk-zea", "Zealand");
        zealand.Children.Add(new CheckTreeNode("dk-zea-cph", "Copenhagen"));
        zealand.Children.Add(new CheckTreeNode("dk-zea-ros", "Roskilde"));

        var dk = new CheckTreeNode("dk", "Denmark");
        dk.Children.Add(jutland);
        dk.Children.Add(zealand);

        var svealand = new CheckTreeNode("se-sve", "Svealand");
        svealand.Children.Add(new CheckTreeNode("se-sve-sto", "Stockholm"));

        var se = new CheckTreeNode("se", "Sweden");
        se.Children.Add(svealand);

        return [dk, se];
    }

    private static CascaderComboBox Create()
        => new() { Nodes = CreateCascaderRegions() };

    private static List<string> Keys(IEnumerable<CheckTreeNode> nodes)
        => nodes.Select(x => x.Key).ToList();

    private static Window CreateWindow(UIElement content)
        => new()
        {
            Content = content,
            Width = 300,
            Height = 100,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };

    private static void Pump()
    {
        var frame = new DispatcherFrame();
        _ = Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.Background,
            new Action(() => frame.Continue = false));
        Dispatcher.PushFrame(frame);
    }

    private static List<ValueChangedEventArgs<IReadOnlyList<string>>> Record(
        CascaderComboBox sut)
    {
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);
        return events;
    }
}