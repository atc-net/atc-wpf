namespace Atc.Wpf.Controls.Tests.Selectors;

public sealed class KeySelectionHelperTests
{
    [Fact]
    public void ApplyInPlace_on_an_ObservableCollection_keeps_the_kept_keys_in_place_and_appends_new_ones()
    {
        var target = new System.Collections.ObjectModel.ObservableCollection<string>(["a", "b", "c"]);

        KeySelectionHelper.ApplyInPlace(target, ["d", "c", "a"]);

        Assert.Equal(["a", "c", "d"], target);
    }

    [Fact]
    public void ApplyInPlace_on_an_ObservableCollectionEx_gives_the_same_order_in_one_reset()
    {
        var target = new Atc.Wpf.Collections.ObservableCollectionEx<string>();
        target.AddRange(["a", "b", "c"]);
        var actions = Record(target);

        KeySelectionHelper.ApplyInPlace(target, ["d", "c", "a"]);

        Assert.Equal(["a", "c", "d"], target);
        Assert.Equal([NotifyCollectionChangedAction.Reset], actions);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ApplyInPlace_raises_nothing_when_only_the_order_of_the_new_keys_differs(
        bool rangeCollection)
    {
        System.Collections.ObjectModel.ObservableCollection<string> target = rangeCollection
            ? new Atc.Wpf.Collections.ObservableCollectionEx<string>()
            : new System.Collections.ObjectModel.ObservableCollection<string>();
        target.Add("a");
        target.Add("b");
        var actions = Record(target);

        KeySelectionHelper.ApplyInPlace(target, ["b", "a"]);

        Assert.Equal(["a", "b"], target);
        Assert.Empty(actions);
    }

    [Fact]
    public void ApplyInPlace_appends_ticked_keys_without_touching_the_kept_ones()
    {
        var target = new System.Collections.ObjectModel.ObservableCollection<string>(["a", "b"]);
        var actions = Record(target);

        KeySelectionHelper.ApplyInPlace(target, ["a", "b", "c"]);

        Assert.Equal(["a", "b", "c"], target);
        Assert.Equal([NotifyCollectionChangedAction.Add], actions);
    }

    [Fact]
    public void ApplyInPlace_removes_unticked_keys_one_by_one_when_most_keys_stay()
    {
        var target = new System.Collections.ObjectModel.ObservableCollection<string>(["a", "b", "c"]);
        var actions = Record(target);

        KeySelectionHelper.ApplyInPlace(target, ["a", "c"]);

        Assert.Equal(["a", "c"], target);
        Assert.Equal([NotifyCollectionChangedAction.Remove], actions);
    }

    [Fact]
    public void ApplyInPlace_clears_and_refills_when_most_keys_go()
    {
        var target = new System.Collections.ObjectModel.ObservableCollection<string>(["a", "b", "c"]);
        var actions = Record(target);

        KeySelectionHelper.ApplyInPlace(target, ["d"]);

        Assert.Equal(["d"], target);
        Assert.Equal([NotifyCollectionChangedAction.Reset, NotifyCollectionChangedAction.Add], actions);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ApplyInPlace_raises_nothing_when_the_keys_are_unchanged(
        bool rangeCollection)
    {
        System.Collections.ObjectModel.ObservableCollection<string> target = rangeCollection
            ? new Atc.Wpf.Collections.ObservableCollectionEx<string>()
            : new System.Collections.ObjectModel.ObservableCollection<string>();
        target.Add("a");
        target.Add("b");
        var actions = Record(target);

        KeySelectionHelper.ApplyInPlace(target, ["a", "b"]);

        Assert.Equal(["a", "b"], target);
        Assert.Empty(actions);
    }

    [Fact]
    public void ApplyInPlace_skips_duplicate_new_keys()
    {
        var target = new System.Collections.ObjectModel.ObservableCollection<string>();

        KeySelectionHelper.ApplyInPlace(target, ["a", "b", "a"]);

        Assert.Equal(["a", "b"], target);
    }

    [Fact]
    public void GetSelectionAfterItemsChanged_drops_keys_of_items_that_are_gone()
    {
        var result = KeySelectionHelper.GetSelectionAfterItemsChanged(
            ["a", "b", "c"],
            Keys("a", "b", "c"),
            Items("a", "c"),
            selectNewItems: false);

        Assert.Equal(["a", "c"], result);
    }

    [Fact]
    public void GetSelectionAfterItemsChanged_keeps_the_selection_when_there_are_no_items()
    {
        var result = KeySelectionHelper.GetSelectionAfterItemsChanged(
            ["a", "x"],
            Keys("a"),
            newItems: null,
            selectNewItems: true);

        Assert.Equal(["a", "x"], result);
    }

    [Fact]
    public void GetSelectionAfterItemsChanged_ticks_only_items_that_are_new_in_item_order()
    {
        var result = KeySelectionHelper.GetSelectionAfterItemsChanged(
            ["b"],
            Keys("a", "b"),
            Items("a", "b", "d", "c"),
            selectNewItems: true);

        Assert.Equal(["b", "d", "c"], result);
    }

    [Fact]
    public void GetSelectionAfterItemsChanged_ticks_new_items_only_up_to_the_limit()
    {
        var result = KeySelectionHelper.GetSelectionAfterItemsChanged(
            ["a", "stale"],
            Keys("a"),
            Items("a", "b", "c", "d"),
            selectNewItems: true,
            maxSelectionCount: 3);

        Assert.Equal(["a", "b", "c"], result);
    }

    [Fact]
    public void GetSelectionAfterItemsChanged_ticks_nothing_new_when_already_at_the_limit()
    {
        var result = KeySelectionHelper.GetSelectionAfterItemsChanged(
            ["a", "b"],
            Keys("a", "b"),
            Items("a", "b", "c"),
            selectNewItems: true,
            maxSelectionCount: 2);

        Assert.Equal(["a", "b"], result);
    }

    [Fact]
    public void GetSelectionAfterItemsChanged_compares_keys_with_the_comparer_of_ordinal_items()
    {
        var result = KeySelectionHelper.GetSelectionAfterItemsChanged(
            ["A", "b"],
            Keys("a", "b"),
            Items("a", "b"),
            selectNewItems: false);

        Assert.Equal(["b"], result);
    }

    [Fact]
    public void GetSelectionAfterItemsChanged_compares_keys_with_the_comparer_of_case_insensitive_items()
    {
        var items = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["a"] = "A",
            ["b"] = "B",
        };

        var result = KeySelectionHelper.GetSelectionAfterItemsChanged(
            ["A", "b", "x"],
            Keys("a", "b"),
            items,
            selectNewItems: false);

        Assert.Equal(["A", "b"], result);
    }

    [Fact]
    public void GetSelectionAfterItemsChanged_drops_duplicate_keys()
    {
        var result = KeySelectionHelper.GetSelectionAfterItemsChanged(
            ["a", "a", "b"],
            Keys("a", "b"),
            Items("a", "b"),
            selectNewItems: false);

        Assert.Equal(["a", "b"], result);
    }

    [Fact]
    public void GetSelectionWith_keeps_the_order_and_appends_new_keys()
    {
        var result = KeySelectionHelper.GetSelectionWith(["c", "a"], ["b", "a", "d"], ["c"]);

        Assert.Equal(["a", "b", "d"], result);
    }

    [Fact]
    public void GetSelectionWith_lets_a_removal_win_over_an_addition()
    {
        var result = KeySelectionHelper.GetSelectionWith(["a"], ["b"], ["b"]);

        Assert.Equal(["a"], result);
    }

    [Theory]
    [InlineData(new[] { "a", "b" }, new[] { "a", "b" }, true)]
    [InlineData(new[] { "a", "b" }, new[] { "b", "a" }, false)]
    [InlineData(new[] { "a" }, new[] { "a", "b" }, false)]
    [InlineData(new[] { "a" }, new[] { "A" }, false)]
    public void SequenceEquals_compares_ordinal_and_in_order(
        string[] first,
        string[] second,
        bool expected)
        => Assert.Equal(expected, KeySelectionHelper.SequenceEquals(first, second));

    [Theory]
    [InlineData("Apple", null, true)]
    [InlineData("Apple", "  ", true)]
    [InlineData("Apple", "PP", true)]
    [InlineData("Apple", " ple ", true)]
    [InlineData("Apple", "x", false)]
    [InlineData(null, "a", false)]
    public void MatchesFilter_is_a_trimmed_case_insensitive_contains(
        string? displayValue,
        string? filter,
        bool expected)
        => Assert.Equal(expected, KeySelectionHelper.MatchesFilter(displayValue, filter));

    [Theory]
    [InlineData(0, 0, false)]
    [InlineData(0, 3, false)]
    [InlineData(1, 3, null)]
    [InlineData(3, 3, true)]
    public void GetAggregateState_is_false_partial_or_true(
        int selected,
        int total,
        bool? expected)
        => Assert.Equal(expected, KeySelectionHelper.GetAggregateState(selected, total));

    private static HashSet<string> Keys(params string[] keys)
        => new(keys, StringComparer.Ordinal);

    private static Dictionary<string, string> Items(params string[] keys)
        => keys.ToDictionary(x => x, x => x.ToUpperInvariant(), StringComparer.Ordinal);

    private static List<NotifyCollectionChangedAction> Record(
        INotifyCollectionChanged collection)
    {
        var actions = new List<NotifyCollectionChangedAction>();
        collection.CollectionChanged += (_, e) => actions.Add(e.Action);
        return actions;
    }
}