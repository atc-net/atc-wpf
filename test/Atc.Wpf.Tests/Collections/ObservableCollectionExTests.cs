namespace Atc.Wpf.Tests.Collections;

public sealed class ObservableCollectionExTests
{
    [Fact]
    public void AddRange_adds_all_items_and_raises_one_collection_changed_event()
    {
        var sut = new ObservableCollectionEx<int>();
        var events = new List<NotifyCollectionChangedAction>();
        sut.CollectionChanged += (_, e) => events.Add(e.Action);

        sut.AddRange([1, 2, 3, 4]);

        Assert.Equal(new[] { 1, 2, 3, 4 }, sut);
        Assert.Single(events);
        Assert.Equal(NotifyCollectionChangedAction.Add, events[0]);
    }

    [Fact]
    public void AddRange_throws_for_null_input()
    {
        var sut = new ObservableCollectionEx<int>();

        Assert.Throws<ArgumentNullException>(() => sut.AddRange(list: null!));
    }

    [Fact]
    public void AddRange_with_empty_input_still_raises_one_Add_event()
    {
        var sut = new ObservableCollectionEx<int>();
        var events = new List<NotifyCollectionChangedAction>();
        sut.CollectionChanged += (_, e) => events.Add(e.Action);

        sut.AddRange(Array.Empty<int>());

        Assert.Empty(sut);
        Assert.Single(events);
        Assert.Equal(NotifyCollectionChangedAction.Add, events[0]);
    }

    [Fact]
    public void Refresh_raises_a_single_Reset_event()
    {
        var sut = new ObservableCollectionEx<int> { 1, 2, 3 };
        var events = new List<NotifyCollectionChangedAction>();
        sut.CollectionChanged += (_, e) => events.Add(e.Action);

        sut.Refresh();

        Assert.Single(events);
        Assert.Equal(NotifyCollectionChangedAction.Reset, events[0]);
    }

    [Fact]
    public void Toggling_SuppressOnChangedNotification_off_after_changes_raises_a_Reset_event()
    {
        var sut = new ObservableCollectionEx<int>();
        sut.SuppressOnChangedNotification = true;

        var events = new List<NotifyCollectionChangedAction>();
        sut.CollectionChanged += (_, e) => events.Add(e.Action);

        sut.Add(1);
        sut.Add(2);
        Assert.Empty(events);

        sut.SuppressOnChangedNotification = false;

        Assert.Single(events);
        Assert.Equal(NotifyCollectionChangedAction.Reset, events[0]);
    }

    [Fact]
    public void Toggling_SuppressOnChangedNotification_to_the_same_value_is_a_no_op()
    {
        var sut = new ObservableCollectionEx<int>();
        var events = new List<NotifyCollectionChangedAction>();
        sut.CollectionChanged += (_, e) => events.Add(e.Action);

        sut.SuppressOnChangedNotification = false;

        Assert.Empty(events);
    }

    [Fact]
    public void Add_outside_AddRange_raises_one_Add_event_per_item()
    {
        var sut = new ObservableCollectionEx<int>();
        var events = new List<NotifyCollectionChangedAction>();
        sut.CollectionChanged += (_, e) => events.Add(e.Action);

        sut.Add(1);
        sut.Add(2);

        Assert.Equal(2, events.Count);
        Assert.All(events, a => Assert.Equal(NotifyCollectionChangedAction.Add, a));
    }
}