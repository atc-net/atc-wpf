namespace Atc.Wpf.Controls.Tests.Selectors;

public sealed class CheckTreeViewTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_match_the_documented_api()
    {
        var sut = new CheckTreeView();

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
        Assert.Null(sut.LoadChildrenAsync);
        Assert.Equal(1, sut.ExpandDepth);
        Assert.Null(sut.ItemTemplate);
        Assert.False(sut.IsAtSelectionLimit);
    }

    [StaFact]
    public void Each_instance_gets_its_own_collections()
    {
        var first = new CheckTreeView();
        var second = new CheckTreeView();

        Assert.NotSame(first.Nodes, second.Nodes);
        Assert.NotSame(first.SelectedKeys, second.SelectedKeys);
    }

    [StaTheory]
    [InlineData(CheckedKeysStrategy.Leaves, new[] { "dk-aarhus", "dk-cph", "dk-odense" })]
    [InlineData(CheckedKeysStrategy.Parents, new[] { "dk" })]
    [InlineData(CheckedKeysStrategy.All, new[] { "dk", "dk-aarhus", "dk-cph", "dk-odense" })]
    public void Ticking_Denmark_gives_the_keys_of_the_strategy(
        CheckedKeysStrategy strategy,
        string[] expected)
    {
        var sut = CreateRegions();
        sut.CheckedKeysStrategy = strategy;

        sut.ToggleNode(sut.FindNode("dk")!);

        Assert.Equal(expected, sut.SelectedKeys.ToArray());
    }

    [StaFact]
    public void A_parent_with_500_children_is_ticked_with_one_event()
    {
        var parent = new CheckTreeNode("p", "Parent");
        for (var i = 0; i < 500; i++)
        {
            parent.Children.Add(new CheckTreeNode($"c{i}", $"Child {i}"));
        }

        var sut = new CheckTreeView { Name = "Tree", Nodes = [parent] };
        var events = Record(sut);
        var collectionChanges = 0;
        sut.SelectedKeys.CollectionChanged += (_, _) => collectionChanges++;

        sut.ToggleNode(parent);

        Assert.Equal(500, sut.SelectedKeys.Count);
        var change = Assert.Single(events);
        Assert.Equal("Tree", change.Identifier);
        Assert.Empty(change.OldValue);
        Assert.Equal(500, change.NewValue.Count);

        sut.ToggleNode(parent);

        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(2, events.Count);
    }

    [StaFact]
    public void Unticking_one_child_makes_the_parent_partial()
    {
        var sut = CreateRegions();
        var dk = sut.FindNode("dk")!;
        sut.ToggleNode(dk);

        sut.ToggleNode(sut.FindNode("dk-cph")!);

        Assert.Null(dk.IsChecked);
        Assert.Equal(["dk-aarhus", "dk-odense"], sut.SelectedKeys);
    }

    [StaFact]
    public void Setting_SelectedKeys_from_code_rebuilds_the_states_without_an_event()
    {
        var sut = CreateRegions();
        var events = Record(sut);

        sut.SelectedKeys = ["dk-cph", "se"];

        Assert.Null(sut.FindNode("dk")!.IsChecked);
        Assert.True(sut.FindNode("se")!.IsChecked);
        Assert.True(sut.FindNode("se-got")!.IsChecked);

        sut.SelectedKeys.Remove("se");

        Assert.False(sut.FindNode("se")!.IsChecked);
        Assert.False(sut.FindNode("se-got")!.IsChecked);
        Assert.Empty(events);
    }

    [StaFact]
    public void A_bound_SelectedKeys_instance_is_updated_in_place()
    {
        var sut = CreateRegions();
        var source = new System.Collections.ObjectModel.ObservableCollection<string>();
        sut.SelectedKeys = source;

        sut.ToggleNode(sut.FindNode("se")!);

        Assert.Same(source, sut.SelectedKeys);
        Assert.Equal(["se-sto", "se-got"], source);
    }

    [StaFact]
    public void Disabled_children_keep_their_state()
    {
        var sut = CreateRegions();
        sut.FindNode("dk-odense")!.IsEnabled = false;

        sut.ToggleNode(sut.FindNode("dk")!);

        Assert.False(sut.FindNode("dk-odense")!.IsChecked);
        Assert.Null(sut.FindNode("dk")!.IsChecked);
        Assert.Equal(["dk-aarhus", "dk-cph"], sut.SelectedKeys);
    }

    [StaFact]
    public void Unlinked_every_node_is_independent()
    {
        var sut = CreateRegions();
        sut.CheckedKeysStrategy = CheckedKeysStrategy.Parents;
        sut.IsCheckLinked = false;

        sut.ToggleNode(sut.FindNode("dk")!);
        sut.ToggleNode(sut.FindNode("se-sto")!);

        Assert.Equal(["dk", "se-sto"], sut.SelectedKeys);
        Assert.False(sut.FindNode("dk-cph")!.IsChecked);
        Assert.False(sut.FindNode("se")!.IsChecked);
    }

    [StaFact]
    public void MaxSelectionCount_refuses_a_tick_beyond_the_limit()
    {
        var sut = CreateRegions();
        sut.MaxSelectionCount = 2;
        var events = Record(sut);

        Assert.False(sut.ToggleNode(sut.FindNode("dk")!));

        Assert.Empty(sut.SelectedKeys);
        Assert.False(sut.FindNode("dk")!.IsChecked);
        Assert.Empty(events);

        sut.ToggleNode(sut.FindNode("se")!);

        Assert.Equal(["se-sto", "se-got"], sut.SelectedKeys);
        Assert.True(sut.IsAtSelectionLimit);

        Assert.False(sut.ToggleNode(sut.FindNode("dk-cph")!));
        Assert.Equal(2, sut.SelectedKeys.Count);

        sut.ToggleNode(sut.FindNode("se-sto")!);

        Assert.False(sut.IsAtSelectionLimit);
    }

    [StaFact]
    public void SelectAll_and_ClearSelection_raise_one_event_each()
    {
        var sut = CreateRegions();
        var events = Record(sut);

        sut.SelectAll();

        Assert.Equal(["dk-aarhus", "dk-cph", "dk-odense", "se-sto", "se-got"], sut.SelectedKeys);

        sut.ClearSelection();

        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(2, events.Count);
    }

    [StaFact]
    public void The_filter_keeps_the_ancestors_of_matches_and_expands_them()
    {
        var sut = CreateRegions();
        sut.FindNode("se")!.IsExpanded = false;

        sut.FilterText = "stock";

        var world = Assert.Single(sut.ShownRoots);
        Assert.Equal("world", world.Key);
        var se = Assert.Single(sut.GetShownChildren(world));
        Assert.Equal("se", se.Key);
        Assert.Equal(["se-sto"], sut.GetShownChildren(se).Select(x => x.Key).ToList());
        Assert.True(se.IsExpanded);

        sut.FilterText = string.Empty;

        Assert.False(se.IsExpanded);
        Assert.Equal(["dk", "se"], sut.GetShownChildren(world).Select(x => x.Key).ToList());
    }

    [StaFact]
    public void A_matching_parent_keeps_all_its_children()
    {
        var sut = CreateRegions();

        sut.FilterText = "denmark";

        var world = Assert.Single(sut.ShownRoots);
        var dk = Assert.Single(sut.GetShownChildren(world));
        Assert.Equal(3, sut.GetShownChildren(dk).Count);
    }

    [StaFact]
    public void Ticking_a_filtered_parent_ticks_only_the_shown_children()
    {
        var sut = CreateRegions();
        sut.FilterText = "ense";

        // Under Denmark only Odense matches.
        sut.ToggleNode(sut.FindNode("dk")!);

        Assert.False(sut.FindNode("dk-aarhus")!.IsChecked);
        Assert.False(sut.FindNode("dk-cph")!.IsChecked);
        Assert.True(sut.FindNode("dk-odense")!.IsChecked);
        Assert.Null(sut.FindNode("dk")!.IsChecked);
        Assert.Equal(["dk-odense"], sut.SelectedKeys);
    }

    [StaFact]
    public void FlatResults_lists_the_matches_with_their_path()
    {
        var sut = CreateRegions();
        sut.FilterMode = TreeFilterMode.FlatResults;

        sut.FilterText = "en";

        Assert.Equal(["dk", "dk-cph", "dk-odense", "se", "se-got"], sut.ShownRoots.Select(x => x.Key).ToList());
        Assert.Empty(sut.GetShownChildren(sut.FindNode("dk")!));
        Assert.Equal("Nordics › Denmark", sut.GetPathText(sut.FindNode("dk-cph")!));
    }

    [StaFact]
    public void No_matches_shows_the_no_matches_text()
    {
        var sut = CreateRegions();

        sut.FilterText = "zzz";

        Assert.Empty(sut.ShownRoots);
        Assert.Equal(Visibility.Visible, sut.PartNoMatches.Visibility);
    }

    [StaFact]
    public void The_filter_box_is_shown_automatically_above_20_nodes()
    {
        var small = CreateRegions();
        var large = new CheckTreeView { Nodes = CheckTreeStateTests.CreateLargeTree(2, 3, 4) };

        Assert.Equal(Visibility.Collapsed, small.PartFilter.Visibility);
        Assert.Equal(Visibility.Visible, large.PartFilter.Visibility);

        small.IsFilterVisible = true;

        Assert.Equal(Visibility.Visible, small.PartFilter.Visibility);
    }

    [StaFact]
    public void ExpandDepth_expands_the_first_levels()
    {
        var nodes = CheckTreeStateTests.CreateRegionNodes();
        _ = new CheckTreeView { ExpandDepth = 2, Nodes = nodes };

        Assert.True(nodes[0].IsExpanded);
        Assert.True(nodes[0].Children[0].IsExpanded);
    }

    [StaFact]
    public void An_unloaded_node_shows_a_please_wait_row_until_its_children_load()
    {
        var lazy = new CheckTreeNode("lazy", "Lazy") { HasUnloadedChildren = true };
        var calls = 0;
        var sut = new CheckTreeView
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

        var placeholder = Assert.Single(sut.GetShownChildren(lazy));
        Assert.True(sut.IsPlaceholder(placeholder));

        sut.LoadChildrenOfAsync(lazy).GetAwaiter().GetResult();

        Assert.Equal(1, calls);
        Assert.False(lazy.HasUnloadedChildren);
        Assert.Equal(["lazy-a", "lazy-b"], sut.GetShownChildren(lazy).Select(x => x.Key).ToList());

        sut.LoadChildrenOfAsync(lazy).GetAwaiter().GetResult();

        Assert.Equal(1, calls);
    }

    [StaFact]
    public void Children_loaded_under_a_ticked_node_inherit_the_tick()
    {
        var lazy = new CheckTreeNode("lazy", "Lazy") { HasUnloadedChildren = true };
        var sut = new CheckTreeView
        {
            Nodes = [lazy],
            LoadChildrenAsync = (node, _) => Task.FromResult<IReadOnlyList<CheckTreeNode>>(
            [
                new CheckTreeNode(node.Key + "-a", "A"),
                new CheckTreeNode(node.Key + "-b", "B"),
            ]),
        };
        sut.ToggleNode(lazy);
        Assert.Equal(["lazy"], sut.SelectedKeys);
        var events = Record(sut);

        sut.LoadChildrenOfAsync(lazy).GetAwaiter().GetResult();

        Assert.All(lazy.Children, x => Assert.True(x.IsChecked));
        Assert.Equal(["lazy-a", "lazy-b"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Equal(["lazy"], change.OldValue);
    }

    [StaFact]
    public void Expanding_an_unloaded_node_in_a_window_loads_its_children()
    {
        var lazy = new CheckTreeNode("lazy", "Lazy") { HasUnloadedChildren = true };
        var sut = new CheckTreeView
        {
            Nodes = [lazy],
            LoadChildrenAsync = (node, _) => Task.FromResult<IReadOnlyList<CheckTreeNode>>([new CheckTreeNode(node.Key + "-a", "A")]),
        };

        RunInWindow(sut, () =>
        {
            lazy.IsExpanded = true;
            sut.UpdateLayout();

            Assert.Single(lazy.Children);
            Assert.False(lazy.HasUnloadedChildren);
        });
    }

    [StaFact]
    public void A_failing_load_leaves_the_node_unloaded()
    {
        var lazy = new CheckTreeNode("lazy", "Lazy") { HasUnloadedChildren = true, IsExpanded = true };
        var sut = new CheckTreeView
        {
            Nodes = [lazy],
            LoadChildrenAsync = (_, _) => Task.FromException<IReadOnlyList<CheckTreeNode>>(new InvalidOperationException("offline")),
        };

        sut.LoadChildrenOfAsync(lazy).GetAwaiter().GetResult();

        Assert.True(lazy.HasUnloadedChildren);
        Assert.False(lazy.IsExpanded);
        Assert.Empty(lazy.Children);
    }

    [StaFact]
    public void Rows_are_tree_items_with_a_three_state_toggle_named_by_the_node_text()
    {
        var sut = CreateRegions();

        RunInWindow(sut, () =>
        {
            var treePeer = UIElementAutomationPeer.CreatePeerForElement(sut.PartTree);
            var world = Assert.IsAssignableFrom<AutomationPeer>(Assert.Single(treePeer.GetChildren()));
            Assert.Equal(AutomationControlType.TreeItem, world.GetAutomationControlType());
            Assert.Equal("Nordics", world.GetName());

            var denmark = world.GetChildren().Single(x => x.GetName() == "Denmark");
            var expandCollapse = (IExpandCollapseProvider)denmark.GetPattern(PatternInterface.ExpandCollapse);
            expandCollapse.Expand();
            sut.UpdateLayout();
            denmark.ResetChildrenCache();

            var toggle = (IToggleProvider)denmark.GetPattern(PatternInterface.Toggle);
            Assert.Equal(System.Windows.Automation.ToggleState.Off, toggle.ToggleState);

            toggle.Toggle();

            Assert.Equal(System.Windows.Automation.ToggleState.On, toggle.ToggleState);
            var copenhagen = denmark.GetChildren().Single(x => x.GetName() == "Copenhagen");
            var childToggle = (IToggleProvider)copenhagen.GetPattern(PatternInterface.Toggle);
            Assert.Equal(System.Windows.Automation.ToggleState.On, childToggle.ToggleState);

            childToggle.Toggle();

            Assert.Equal(System.Windows.Automation.ToggleState.Indeterminate, toggle.ToggleState);
            Assert.Equal(["dk-aarhus", "dk-odense"], sut.SelectedKeys);
        });
    }

    [StaFact]
    public void Space_on_a_row_toggles_its_node()
    {
        var sut = CreateRegions();

        RunInWindow(sut, () =>
        {
            var worldContainer = (TreeViewItem)sut.PartTree.ItemContainerGenerator.ContainerFromIndex(0);
            worldContainer.UpdateLayout();
            var dkContainer = (TreeViewItem)worldContainer.ItemContainerGenerator.ContainerFromIndex(0);

            KeyPress.Send(dkContainer, Key.Space);

            Assert.True(sut.FindNode("dk")!.IsChecked);
        });
    }

    [StaFact]
    public void Multiply_on_a_row_expands_everything_under_it()
    {
        var nodes = CheckTreeStateTests.CreateRegionNodes();
        var sut = new CheckTreeView { ExpandDepth = 0, Nodes = nodes };

        RunInWindow(sut, () =>
        {
            var worldContainer = (TreeViewItem)sut.PartTree.ItemContainerGenerator.ContainerFromIndex(0);

            KeyPress.Send(worldContainer, Key.Multiply);

            Assert.True(nodes[0].IsExpanded);
            Assert.All(nodes[0].Children, x => Assert.True(x.IsExpanded));
        });
    }

    [StaFact]
    public void A_tree_of_10000_nodes_is_virtualized_and_filters_quickly()
    {
        var sut = new CheckTreeView
        {
            Height = 300,
            Nodes = CheckTreeStateTests.CreateLargeTree(10, 30, 33),
        };

        RunInWindow(sut, () =>
        {
            var realized = CountVisualChildren<TreeViewItem>(sut.PartTree);
            Assert.InRange(realized, 1, 100);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            sut.FilterText = "Leaf 4.7.1";
            sut.FlushPendingFilter();
            sut.UpdateLayout();
            stopwatch.Stop();

            Assert.InRange(stopwatch.ElapsedMilliseconds, 0, 2000);
            Assert.Equal(["r4"], sut.ShownRoots.Select(x => x.Key).ToList());
            Assert.InRange(CountVisualChildren<TreeViewItem>(sut.PartTree), 1, 100);
        });
    }

    [StaFact]
    public void A_new_child_under_a_parent_key_comes_up_ticked()
    {
        var sut = CreateRegions();
        sut.CheckedKeysStrategy = CheckedKeysStrategy.Parents;
        sut.ToggleNode(sut.FindNode("dk")!);
        var events = Record(sut);

        sut.FindNode("dk")!.Children.Add(new CheckTreeNode("dk-aalborg", "Aalborg"));

        Assert.True(sut.FindNode("dk-aalborg")!.IsChecked);
        Assert.Equal(["dk"], sut.SelectedKeys);
        Assert.Empty(events);
        Assert.Equal(4, sut.GetShownChildren(sut.FindNode("dk")!).Count);
    }

    [StaFact]
    public void A_structure_change_does_not_raise_SelectedKeysChanged_when_only_the_order_differs()
    {
        var sut = CreateRegions();
        sut.ToggleNode(sut.FindNode("dk-odense")!);
        sut.ToggleNode(sut.FindNode("dk-aarhus")!);
        Assert.Equal(["dk-odense", "dk-aarhus"], sut.SelectedKeys);
        var events = Record(sut);

        sut.FindNode("se")!.Children.Add(new CheckTreeNode("se-mal", "Malmo"));

        Assert.Empty(events);
        Assert.Equal(["dk-odense", "dk-aarhus"], sut.SelectedKeys);
    }

    private static CheckTreeView CreateRegions()
        => new() { Nodes = CheckTreeStateTests.CreateRegionNodes(), ExpandDepth = 2 };

    private static List<ValueChangedEventArgs<IReadOnlyList<string>>> Record(
        CheckTreeView sut)
    {
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);
        return events;
    }

    private static void RunInWindow(
        FrameworkElement content,
        Action action)
    {
        var window = new Window
        {
            Content = content,
            Width = 300,
            Height = 400,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };
        window.Show();
        try
        {
            window.UpdateLayout();
            action();
        }
        finally
        {
            window.Close();
        }
    }

    private static int CountVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        var count = 0;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T)
            {
                count++;
            }

            count += CountVisualChildren<T>(child);
        }

        return count;
    }
}