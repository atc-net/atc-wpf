namespace Atc.Wpf.Tests.Collections;

/// <summary>
/// Checks that <see cref="ObservableDictionary{TKey, TValue}"/> raises the one indexed collection-changed
/// notification per change that <see cref="ListCollectionView"/> (and thus every ItemsControl) relies on.
/// </summary>
public sealed class ObservableDictionaryCollectionViewTests
{
    [Fact]
    public void Add_raises_one_Add_event_with_the_index_of_the_new_pair()
    {
        var sut = new ObservableDictionary<string, int> { { "a", 1 } };
        var events = new List<NotifyCollectionChangedEventArgs>();
        sut.CollectionChanged += (_, e) => events.Add(e);

        sut.Add("b", 2);

        var e = Assert.Single(events);
        Assert.Equal(NotifyCollectionChangedAction.Add, e.Action);
        Assert.Equal(1, e.NewStartingIndex);
        var pair = Assert.IsType<ObservableKeyValuePair<string, int>>(Assert.Single(e.NewItems!));
        Assert.Equal("b", pair.Key);
        Assert.Equal(2, pair.Value);
    }

    [Fact]
    public void Indexer_setter_for_a_new_key_raises_one_Add_event()
    {
        var sut = new ObservableDictionary<string, int>();
        var events = new List<NotifyCollectionChangedEventArgs>();
        sut.CollectionChanged += (_, e) => events.Add(e);

        sut["a"] = 1;

        var e = Assert.Single(events);
        Assert.Equal(NotifyCollectionChangedAction.Add, e.Action);
        Assert.Equal(0, e.NewStartingIndex);
    }

    [Fact]
    public void Remove_by_key_raises_one_Remove_event_with_the_old_index()
    {
        var sut = new ObservableDictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
        var events = new List<NotifyCollectionChangedEventArgs>();
        sut.CollectionChanged += (_, e) => events.Add(e);

        var removed = sut.Remove("b");

        Assert.True(removed);
        var e = Assert.Single(events);
        Assert.Equal(NotifyCollectionChangedAction.Remove, e.Action);
        Assert.Equal(1, e.OldStartingIndex);
        var pair = Assert.IsType<ObservableKeyValuePair<string, int>>(Assert.Single(e.OldItems!));
        Assert.Equal("b", pair.Key);
    }

    [Fact]
    public void Remove_by_pair_raises_one_Remove_event_with_the_old_index()
    {
        var sut = new ObservableDictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
        var events = new List<NotifyCollectionChangedEventArgs>();
        sut.CollectionChanged += (_, e) => events.Add(e);

        var removed = sut.Remove(new KeyValuePair<string, int>("c", 3));

        Assert.True(removed);
        var e = Assert.Single(events);
        Assert.Equal(NotifyCollectionChangedAction.Remove, e.Action);
        Assert.Equal(2, e.OldStartingIndex);
    }

    [Fact]
    public void Remove_of_a_missing_key_raises_no_event()
    {
        var sut = new ObservableDictionary<string, int> { { "a", 1 } };
        var events = new List<NotifyCollectionChangedEventArgs>();
        sut.CollectionChanged += (_, e) => events.Add(e);

        var removed = sut.Remove("missing");

        Assert.False(removed);
        Assert.Empty(events);
    }

    [Fact]
    public void Update_with_int_keys_replaces_the_value_and_keeps_the_position()
    {
        var sut = new ObservableDictionary<int, string> { { 10, "a" }, { 20, "b" }, { 30, "c" } };

        sut.Update(new KeyValuePair<int, string>(20, "x"));

        Assert.Equal(new[] { 10, 20, 30 }, sut.Keys);
        Assert.Equal("x", sut[20]);
        Assert.Equal(3, sut.Count);
    }

    [Fact]
    public void Update_with_string_keys_replaces_the_value_and_keeps_the_position()
    {
        var sut = new ObservableDictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };

        sut.Update(new KeyValuePair<string, int>("c", 33));

        Assert.Equal(new[] { "a", "b", "c" }, sut.Keys, StringComparer.Ordinal);
        Assert.Equal(33, sut["c"]);
    }

    [Fact]
    public void Update_raises_one_Replace_event_at_the_position_of_the_key()
    {
        var sut = new ObservableDictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
        var events = new List<NotifyCollectionChangedEventArgs>();
        sut.CollectionChanged += (_, e) => events.Add(e);

        sut.Update(new KeyValuePair<string, int>("b", 22));

        var e = Assert.Single(events);
        Assert.Equal(NotifyCollectionChangedAction.Replace, e.Action);
        Assert.Equal(1, e.NewStartingIndex);
        Assert.Equal(1, e.OldStartingIndex);
        var newPair = Assert.IsType<ObservableKeyValuePair<string, int>>(Assert.Single(e.NewItems!));
        Assert.Equal(22, newPair.Value);
    }

    [Fact]
    public void Update_with_an_unknown_key_appends_the_pair()
    {
        var sut = new ObservableDictionary<string, int> { { "a", 1 }, { "b", 2 } };

        sut.Update(new KeyValuePair<string, int>("z", 26));

        Assert.Equal(new[] { "a", "b", "z" }, sut.Keys, StringComparer.Ordinal);
        Assert.Equal(26, sut["z"]);
    }

    [StaFact]
    public void ListCollectionView_follows_Add_Remove_and_Update_without_throwing()
    {
        var sut = new ObservableDictionary<string, int>();
        var view = (ListCollectionView)CollectionViewSource.GetDefaultView(sut);

        sut.Add("a", 1);
        sut.Add("b", 2);
        sut.Add("c", 3);
        Assert.Equal(3, view.Count);

        var removeException = Record.Exception(() => sut.Remove("b"));
        Assert.Null(removeException);
        Assert.Equal(2, view.Count);
        Assert.Equal(["a", "c"], ViewKeys(view), StringComparer.Ordinal);

        var updateException = Record.Exception(() => sut.Update(new KeyValuePair<string, int>("c", 33)));
        Assert.Null(updateException);
        Assert.Equal(2, view.Count);
        Assert.Equal(33, ((ObservableKeyValuePair<string, int>)view.GetItemAt(1)).Value);
    }

    [StaFact]
    public void Sorted_ListCollectionView_contains_each_added_pair_once()
    {
        var sut = new ObservableDictionary<string, int>();
        var view = (ListCollectionView)CollectionViewSource.GetDefaultView(sut);
        view.SortDescriptions.Add(new SortDescription("Key", ListSortDirection.Ascending));

        sut.Add("b", 2);
        sut.Add("a", 1);
        sut["c"] = 3;

        Assert.Equal(3, view.Count);
        Assert.Equal(["a", "b", "c"], ViewKeys(view), StringComparer.Ordinal);

        sut.Remove(new KeyValuePair<string, int>("a", 1));

        Assert.Equal(2, view.Count);
        Assert.Equal(["b", "c"], ViewKeys(view), StringComparer.Ordinal);
    }

    private static List<string> ViewKeys(ListCollectionView view)
        => view
            .Cast<ObservableKeyValuePair<string, int>>()
            .Select(p => p.Key)
            .ToList();
}