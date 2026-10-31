namespace Atc.Wpf.Controls.Tests.Selectors;

public sealed class CheckTreeStateTests
{
    [Theory]
    [InlineData(CheckedKeysStrategy.Leaves, new[] { "dk-aarhus", "dk-cph", "dk-odense" })]
    [InlineData(CheckedKeysStrategy.Parents, new[] { "dk" })]
    [InlineData(CheckedKeysStrategy.All, new[] { "dk", "dk-aarhus", "dk-cph", "dk-odense" })]
    public void Denmark_with_all_its_sites_ticked_gives_the_keys_of_the_strategy(
        CheckedKeysStrategy strategy,
        string[] expected)
    {
        var sut = CreateRegions(strategy);

        sut.Toggle(sut.Find("dk")!);

        Assert.Equal(expected, sut.CommitKeys());
        Assert.True(sut.Find("dk")!.IsChecked);
        Assert.True(sut.Find("dk-cph")!.IsChecked);
        Assert.False(sut.Find("se")!.IsChecked);
    }

    [Theory]
    [InlineData(CheckedKeysStrategy.Leaves)]
    [InlineData(CheckedKeysStrategy.Parents)]
    [InlineData(CheckedKeysStrategy.All)]
    public void Ticking_every_site_one_by_one_gives_the_same_keys_as_ticking_Denmark(
        CheckedKeysStrategy strategy)
    {
        var byParent = CreateRegions(strategy);
        var byLeaves = CreateRegions(strategy);

        byParent.Toggle(byParent.Find("dk")!);
        byLeaves.Toggle(byLeaves.Find("dk-aarhus")!);
        byLeaves.Toggle(byLeaves.Find("dk-cph")!);
        byLeaves.Toggle(byLeaves.Find("dk-odense")!);

        Assert.Equal(byParent.CommitKeys(), byLeaves.CommitKeys());
    }

    [Fact]
    public void Ticking_a_parent_ticks_all_children_and_unticking_it_unticks_them()
    {
        var sut = CreateRegions();
        var dk = sut.Find("dk")!;

        sut.Toggle(dk);

        Assert.All(dk.Children, x => Assert.True(x.IsChecked));

        sut.Toggle(dk);

        Assert.False(dk.IsChecked);
        Assert.All(dk.Children, x => Assert.False(x.IsChecked));
        Assert.Empty(sut.CommitKeys());
    }

    [Fact]
    public void Some_ticked_children_make_the_parent_partial()
    {
        var sut = CreateRegions();

        sut.Toggle(sut.Find("dk-cph")!);

        Assert.Null(sut.Find("dk")!.IsChecked);
        Assert.Null(sut.Find("world")!.IsChecked);
        Assert.Equal(["dk-cph"], sut.CommitKeys());
    }

    [Fact]
    public void Ticking_a_partial_parent_ticks_all_its_children()
    {
        var sut = CreateRegions();
        sut.Toggle(sut.Find("dk-cph")!);

        sut.Toggle(sut.Find("dk")!);

        Assert.True(sut.Find("dk")!.IsChecked);
        Assert.Equal(["dk-aarhus", "dk-cph", "dk-odense"], sut.CommitKeys());
    }

    [Fact]
    public void Partial_state_propagates_through_every_level()
    {
        var sut = CreateRegions();

        sut.Toggle(sut.Find("dk")!);

        Assert.Null(sut.Find("world")!.IsChecked);

        sut.Toggle(sut.Find("se")!);

        Assert.True(sut.Find("world")!.IsChecked);

        sut.Toggle(sut.Find("se-sto")!);

        Assert.Null(sut.Find("se")!.IsChecked);
        Assert.Null(sut.Find("world")!.IsChecked);
    }

    [Fact]
    public void Disabled_children_keep_their_state_and_make_the_parent_partial()
    {
        var sut = CreateRegions();
        var odense = sut.Find("dk-odense")!;
        odense.IsEnabled = false;

        sut.Toggle(sut.Find("dk")!);

        Assert.False(odense.IsChecked);
        Assert.True(sut.Find("dk-cph")!.IsChecked);
        Assert.Null(sut.Find("dk")!.IsChecked);
        Assert.Equal(["dk-aarhus", "dk-cph"], sut.CommitKeys());
    }

    [Fact]
    public void A_partial_parent_whose_enabled_children_are_all_ticked_is_unticked_by_a_click()
    {
        var sut = CreateRegions();
        sut.Find("dk-odense")!.IsEnabled = false;
        var dk = sut.Find("dk")!;
        sut.Toggle(dk);
        Assert.Null(dk.IsChecked);

        sut.Toggle(dk);

        Assert.False(dk.IsChecked);
        Assert.Empty(sut.CommitKeys());
    }

    [Fact]
    public void A_ticked_disabled_child_stays_ticked_when_the_parent_is_unticked()
    {
        var sut = CreateRegions();
        sut.ApplyKeys(["dk-odense"]);
        var odense = sut.Find("dk-odense")!;
        odense.IsEnabled = false;
        var dk = sut.Find("dk")!;

        sut.Toggle(dk);
        Assert.True(dk.IsChecked);

        sut.Toggle(dk);

        Assert.True(odense.IsChecked);
        Assert.Null(dk.IsChecked);
        Assert.Equal(["dk-odense"], sut.CommitKeys());
    }

    [Fact]
    public void A_disabled_node_cannot_be_toggled()
    {
        var sut = CreateRegions();
        var cph = sut.Find("dk-cph")!;
        cph.IsEnabled = false;

        Assert.False(sut.Toggle(cph));
        Assert.False(cph.IsChecked);
    }

    [Fact]
    public void Unlinked_every_node_is_independent_and_the_strategy_is_ignored()
    {
        var sut = CreateRegions(CheckedKeysStrategy.Parents);
        sut.IsCheckLinked = false;
        var dk = sut.Find("dk")!;

        sut.Toggle(dk);

        Assert.True(dk.IsChecked);
        Assert.All(dk.Children, x => Assert.False(x.IsChecked));
        Assert.Equal(["dk"], sut.CommitKeys());

        sut.Toggle(sut.Find("dk-cph")!);

        Assert.True(dk.IsChecked);
        Assert.Equal(["dk", "dk-cph"], sut.CommitKeys());

        sut.Toggle(dk);

        Assert.False(dk.IsChecked);
        Assert.Equal(["dk-cph"], sut.CommitKeys());
    }

    [Fact]
    public void Unlinked_keys_from_code_tick_only_their_own_nodes()
    {
        var sut = CreateRegions();
        sut.IsCheckLinked = false;

        sut.ApplyKeys(["dk"]);

        Assert.True(sut.Find("dk")!.IsChecked);
        Assert.False(sut.Find("dk-cph")!.IsChecked);
        Assert.False(sut.Find("world")!.IsChecked);
    }

    [Theory]
    [InlineData(CheckedKeysStrategy.Leaves)]
    [InlineData(CheckedKeysStrategy.Parents)]
    [InlineData(CheckedKeysStrategy.All)]
    public void Keys_from_code_rebuild_every_state(CheckedKeysStrategy strategy)
    {
        var sut = CreateRegions(strategy);
        sut.Toggle(sut.Find("se")!);

        sut.ApplyKeys(["dk", "se-got"]);

        Assert.True(sut.Find("dk")!.IsChecked);
        Assert.True(sut.Find("dk-aarhus")!.IsChecked);
        Assert.Null(sut.Find("se")!.IsChecked);
        Assert.False(sut.Find("se-sto")!.IsChecked);
        Assert.True(sut.Find("se-got")!.IsChecked);
        Assert.Null(sut.Find("world")!.IsChecked);
    }

    [Fact]
    public void Keys_of_nodes_not_in_the_tree_are_kept()
    {
        var sut = CreateRegions();
        sut.ApplyKeys(["unknown", "dk-cph"]);

        sut.Toggle(sut.Find("se-sto")!);

        Assert.Equal(["dk-cph", "se-sto", "unknown"], sut.CommitKeys());
    }

    [Fact]
    public void A_child_added_under_a_parent_key_comes_up_ticked_with_the_Parents_strategy()
    {
        var sut = CreateRegions(CheckedKeysStrategy.Parents);
        sut.Toggle(sut.Find("dk")!);
        Assert.Equal(["dk"], sut.CommitKeys());
        var structureChanges = new List<CheckTreeNode?>();
        sut.StructureChanged = structureChanges.Add;

        var aalborg = new CheckTreeNode("dk-aalborg", "Aalborg");
        sut.Find("dk")!.Children.Add(aalborg);

        Assert.True(aalborg.IsChecked);
        Assert.True(sut.Find("dk")!.IsChecked);
        Assert.Equal(["dk"], sut.GetKeys());
        Assert.Same(sut.Find("dk"), Assert.Single(structureChanges));
    }

    [Fact]
    public void A_child_added_under_ticked_leaves_makes_the_parent_partial_with_the_Leaves_strategy()
    {
        var sut = CreateRegions();
        sut.Toggle(sut.Find("dk")!);
        sut.CommitKeys();

        var aalborg = new CheckTreeNode("dk-aalborg", "Aalborg");
        sut.Find("dk")!.Children.Add(aalborg);

        Assert.False(aalborg.IsChecked);
        Assert.Null(sut.Find("dk")!.IsChecked);
        Assert.Null(sut.Find("world")!.IsChecked);
        Assert.Equal(["dk-aarhus", "dk-cph", "dk-odense"], sut.GetKeys());
    }

    [Fact]
    public void A_child_added_under_a_ticked_parent_joins_the_keys_with_the_All_strategy()
    {
        var sut = CreateRegions(CheckedKeysStrategy.All);
        sut.Toggle(sut.Find("dk")!);
        sut.CommitKeys();

        var aalborg = new CheckTreeNode("dk-aalborg", "Aalborg");
        sut.Find("dk")!.Children.Add(aalborg);

        Assert.True(aalborg.IsChecked);
        Assert.Equal(["dk", "dk-aarhus", "dk-cph", "dk-odense", "dk-aalborg"], sut.GetKeys());
    }

    [Fact]
    public void Removing_a_child_updates_the_parent_and_drops_it_from_the_index()
    {
        var sut = CreateRegions();
        sut.Toggle(sut.Find("dk-aarhus")!);
        sut.Toggle(sut.Find("dk-cph")!);
        sut.CommitKeys();
        var dk = sut.Find("dk")!;
        var odense = sut.Find("dk-odense")!;

        dk.Children.Remove(odense);

        Assert.True(dk.IsChecked);
        Assert.Null(sut.Find("dk-odense"));
    }

    [Theory]
    [InlineData(CheckedKeysStrategy.Leaves, new[] { "lazy" }, new[] { "lazy-a", "lazy-b" })]
    [InlineData(CheckedKeysStrategy.Parents, new[] { "lazy" }, new[] { "lazy" })]
    [InlineData(CheckedKeysStrategy.All, new[] { "lazy" }, new[] { "lazy", "lazy-a", "lazy-b" })]
    public void Lazily_loaded_children_inherit_the_tick_of_their_parent(
        CheckedKeysStrategy strategy,
        string[] keysBeforeLoad,
        string[] keysAfterLoad)
    {
        var lazy = new CheckTreeNode("lazy", "Lazy") { HasUnloadedChildren = true };
        var sut = Create(strategy, lazy);

        sut.Toggle(lazy);

        Assert.True(lazy.IsChecked);
        Assert.Equal(keysBeforeLoad, sut.CommitKeys());

        using (sut.DeferStructureChanges())
        {
            lazy.Children.Add(new CheckTreeNode("lazy-a", "A"));
            lazy.Children.Add(new CheckTreeNode("lazy-b", "B"));
        }

        Assert.All(lazy.Children, x => Assert.True(x.IsChecked));
        Assert.True(lazy.IsChecked);
        Assert.Equal(keysAfterLoad, sut.GetKeys());
    }

    [Fact]
    public void A_saved_key_of_a_lazy_child_ticks_it_when_it_loads()
    {
        var lazy = new CheckTreeNode("lazy", "Lazy") { HasUnloadedChildren = true };
        var sut = Create(CheckedKeysStrategy.Leaves, lazy);
        sut.ApplyKeys(["lazy-b"]);
        Assert.False(lazy.IsChecked);

        using (sut.DeferStructureChanges())
        {
            lazy.Children.Add(new CheckTreeNode("lazy-a", "A"));
            lazy.Children.Add(new CheckTreeNode("lazy-b", "B"));
        }

        Assert.True(sut.Find("lazy-b")!.IsChecked);
        Assert.Null(lazy.IsChecked);
        Assert.Equal(["lazy-b"], sut.GetKeys());
    }

    [Fact]
    public void Two_nodes_with_the_same_key_in_the_roots_throw_naming_the_key()
    {
        var sut = new CheckTreeState();
        var roots = new System.Collections.ObjectModel.ObservableCollection<CheckTreeNode>
        {
            new("dk", "Denmark") { Children = { new CheckTreeNode("cph", "Copenhagen") } },
            new("se", "Sweden") { Children = { new CheckTreeNode("cph", "Copenhagen (SE)") } },
        };

        var exception = Assert.Throws<InvalidOperationException>(() => sut.SetRoots(roots));

        Assert.Contains("'cph'", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Adding_a_child_with_a_key_already_in_the_tree_throws_naming_the_key()
    {
        var sut = CreateRegions();

        var exception = Assert.Throws<InvalidOperationException>(
            () => sut.Find("se")!.Children.Add(new CheckTreeNode("dk-cph", "Copenhagen")));

        Assert.Contains("'dk-cph'", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_refused_duplicate_child_leaves_the_engine_usable()
    {
        var sut = CreateRegions();
        var sweden = sut.Find("se")!;

        Assert.Throws<InvalidOperationException>(
            () => sweden.Children.Add(new CheckTreeNode("dk-cph", "Copenhagen")));

        sut.ApplyKeys(["se-sto", "dk-cph"]);

        Assert.Equal(["dk-cph", "se-sto"], sut.GetKeys());
        Assert.True(sut.Toggle(sweden));
        Assert.True(sweden.IsChecked);
        Assert.Equal(["dk-cph", "se-sto", "se-got"], sut.CommitKeys());
        Assert.Same(sut.Find("dk")!.Children[1], sut.Find("dk-cph"));
    }

    [Fact]
    public void A_refused_duplicate_root_leaves_the_engine_usable()
    {
        var sut = new CheckTreeState();
        var roots = new System.Collections.ObjectModel.ObservableCollection<CheckTreeNode>
        {
            new("dk", "Denmark") { Children = { new CheckTreeNode("cph", "Copenhagen") } },
            new("se", "Sweden") { Children = { new CheckTreeNode("cph", "Copenhagen (SE)") } },
        };

        Assert.Throws<InvalidOperationException>(() => sut.SetRoots(roots));

        sut.ApplyKeys(["cph"]);

        Assert.Equal(["cph"], sut.GetKeys());
        Assert.True(sut.Find("dk")!.IsChecked);
        Assert.False(sut.Find("se")!.IsChecked);
    }

    [Fact]
    public void Adding_children_one_at_a_time_indexes_each_node_once()
    {
        var parent = new CheckTreeNode("p", "Parent");
        var sut = Create(CheckedKeysStrategy.Parents, parent);
        sut.ApplyKeys(["p"]);
        var before = sut.IndexCount;

        for (var i = 0; i < 1000; i++)
        {
            parent.Children.Add(new CheckTreeNode($"c{i}", $"Child {i}"));
        }

        Assert.Equal(1000, sut.IndexCount - before);
        Assert.True(parent.IsChecked);
        Assert.All(parent.Children, x => Assert.True(x.IsChecked));
        Assert.Equal(["p"], sut.GetKeys());
    }

    [Fact]
    public void Adding_roots_one_at_a_time_indexes_each_node_once()
    {
        var roots = new System.Collections.ObjectModel.ObservableCollection<CheckTreeNode>();
        var sut = new CheckTreeState();
        sut.SetRoots(roots);
        sut.ApplyKeys(["r500"]);
        var before = sut.IndexCount;

        for (var i = 0; i < 1000; i++)
        {
            roots.Add(new CheckTreeNode($"r{i}", $"Root {i}"));
        }

        Assert.Equal(1000, sut.IndexCount - before);
        Assert.True(sut.Find("r500")!.IsChecked);
        Assert.Equal(["r500"], sut.GetKeys());
    }

    [Fact]
    public void Children_added_one_at_a_time_update_a_linked_parent_and_its_ancestors()
    {
        var sut = CreateRegions();
        sut.ApplyKeys(["se-sto", "se-got"]);
        var sweden = sut.Find("se")!;
        Assert.True(sweden.IsChecked);

        sweden.Children.Add(new CheckTreeNode("se-mal", "Malmo"));

        Assert.Null(sweden.IsChecked);
        Assert.Null(sut.Find("world")!.IsChecked);

        sut.SetChecked(sut.Find("se-mal")!, value: true);

        Assert.True(sweden.IsChecked);
        Assert.Equal(["se-sto", "se-got", "se-mal"], sut.GetKeys());
    }

    [Fact]
    public void Lazy_children_and_a_node_moved_back_into_the_tree_keep_their_keys()
    {
        var lazy = new CheckTreeNode("lazy", "Lazy") { HasUnloadedChildren = true };
        var other = new CheckTreeNode("other", "Other");
        var sut = Create(CheckedKeysStrategy.Leaves, lazy, other);
        var child = new CheckTreeNode("lazy-a", "A");

        lazy.Children.Add(child);
        lazy.Children.Add(new CheckTreeNode("lazy-b", "B"));
        lazy.Children.Remove(child);
        other.Children.Add(child);
        other.Children.Remove(child);
        lazy.Children.Insert(0, child);
        lazy.Children.Move(0, 1);

        Assert.Same(child, sut.Find("lazy-a"));
        Assert.Equal(4, sut.NodeCount);

        sut.Toggle(child);

        Assert.Equal(["lazy-a"], sut.GetKeys());
    }

    [Fact]
    public void Deferred_structure_changes_are_handled_once_per_parent()
    {
        var lazy = new CheckTreeNode("lazy", "Lazy");
        var sut = Create(CheckedKeysStrategy.Leaves, lazy);
        var changes = new List<CheckTreeNode?>();
        sut.StructureChanged = changes.Add;

        using (sut.DeferStructureChanges())
        {
            for (var i = 0; i < 50; i++)
            {
                lazy.Children.Add(new CheckTreeNode($"c{i}", $"C{i}"));
            }
        }

        Assert.Same(lazy, Assert.Single(changes));
        Assert.Equal(51, sut.NodeCount);
    }

    [Fact]
    public void Ticking_only_shown_children_with_a_filter()
    {
        var sut = CreateRegions();
        var dk = sut.Find("dk")!;
        var shown = new HashSet<string>(["world", "dk", "dk-cph", "dk-odense"], StringComparer.Ordinal);

        sut.Toggle(dk, x => shown.Contains(x.Key));

        Assert.False(sut.Find("dk-aarhus")!.IsChecked);
        Assert.True(sut.Find("dk-cph")!.IsChecked);
        Assert.True(sut.Find("dk-odense")!.IsChecked);
        Assert.Null(dk.IsChecked);

        // All shown children are ticked: the next click unticks them.
        sut.Toggle(dk, x => shown.Contains(x.Key));

        Assert.False(dk.IsChecked);
    }

    [Fact]
    public void SetAll_ticks_and_unticks_every_enabled_node()
    {
        var sut = CreateRegions();
        sut.Find("se-got")!.IsEnabled = false;

        sut.SetAll(value: true);

        Assert.Equal(["dk-aarhus", "dk-cph", "dk-odense", "se-sto"], sut.CommitKeys());
        Assert.True(sut.GetAllState());

        sut.SetAll(value: false);

        Assert.Empty(sut.CommitKeys());
        Assert.False(sut.GetAllState());
    }

    [Fact]
    public void CountLeaves_counts_ticked_and_total_leaves()
    {
        var sut = CreateRegions();
        sut.Toggle(sut.Find("dk")!);

        Assert.Equal((3, 5), sut.CountLeaves());
    }

    [Fact]
    public void Ticking_one_leaf_in_a_large_tree_touches_only_the_leaf_and_its_ancestors()
    {
        var roots = CreateLargeTree(10, 30, 33);
        var sut = new CheckTreeState();
        sut.SetRoots(roots);
        Assert.Equal(10 + 300 + 9900, sut.NodeCount);
        var leaf = roots[4].Children[7].Children[12];
        var before = sut.StateWriteCount;

        sut.Toggle(leaf);

        Assert.Equal(3, sut.StateWriteCount - before);
        Assert.True(leaf.IsChecked);
        Assert.Null(roots[4].IsChecked);
    }

    [Fact]
    public void Ticking_a_branch_in_a_large_tree_touches_only_the_branch_and_its_ancestors()
    {
        var roots = CreateLargeTree(10, 30, 33);
        var sut = new CheckTreeState();
        sut.SetRoots(roots);
        var branch = roots[4].Children[7];
        var before = sut.StateWriteCount;

        sut.Toggle(branch);

        Assert.Equal(33 + 1 + 1, sut.StateWriteCount - before);
        Assert.Equal(33, sut.CommitKeys().Count);
    }

    [Fact]
    public void The_same_node_twice_in_the_tree_is_refused()
    {
        var node = new CheckTreeNode("a", "A");
        var roots = new System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> { node, node };
        var sut = new CheckTreeState();

        Assert.Throws<InvalidOperationException>(() => sut.SetRoots(roots));
    }

    [Fact]
    public void GetParent_and_GetDepth_follow_the_tree()
    {
        var sut = CreateRegions();
        var cph = sut.Find("dk-cph")!;

        Assert.Same(sut.Find("dk"), sut.GetParent(cph));
        Assert.Equal(2, sut.GetDepth(cph));
        Assert.Null(sut.GetParent(sut.Find("world")!));
    }

    internal static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> CreateLargeTree(
        int rootCount,
        int childCount,
        int leafCount)
    {
        var roots = new System.Collections.ObjectModel.ObservableCollection<CheckTreeNode>();
        for (var r = 0; r < rootCount; r++)
        {
            var root = new CheckTreeNode($"r{r}", $"Root {r}");
            for (var c = 0; c < childCount; c++)
            {
                var child = new CheckTreeNode($"r{r}-c{c}", $"Branch {r}.{c}");
                for (var l = 0; l < leafCount; l++)
                {
                    child.Children.Add(new CheckTreeNode($"r{r}-c{c}-l{l}", $"Leaf {r}.{c}.{l}"));
                }

                root.Children.Add(child);
            }

            roots.Add(root);
        }

        return roots;
    }

    internal static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> CreateRegionNodes()
    {
        var dk = new CheckTreeNode("dk", "Denmark");
        dk.Children.Add(new CheckTreeNode("dk-aarhus", "Aarhus"));
        dk.Children.Add(new CheckTreeNode("dk-cph", "Copenhagen"));
        dk.Children.Add(new CheckTreeNode("dk-odense", "Odense"));

        var se = new CheckTreeNode("se", "Sweden");
        se.Children.Add(new CheckTreeNode("se-sto", "Stockholm"));
        se.Children.Add(new CheckTreeNode("se-got", "Gothenburg"));

        var world = new CheckTreeNode("world", "Nordics");
        world.Children.Add(dk);
        world.Children.Add(se);

        return [world];
    }

    private static CheckTreeState CreateRegions(
        CheckedKeysStrategy strategy = CheckedKeysStrategy.Leaves)
        => Create(strategy, CreateRegionNodes().ToArray());

    private static CheckTreeState Create(
        CheckedKeysStrategy strategy,
        params CheckTreeNode[] roots)
    {
        var sut = new CheckTreeState
        {
            Strategy = strategy,
        };
        sut.SetRoots(new System.Collections.ObjectModel.ObservableCollection<CheckTreeNode>(roots));
        return sut;
    }
}