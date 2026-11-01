namespace Atc.Wpf.Tests.Collections;

/// <summary>
/// Checks that <see cref="ObservableCollectionEx{T}.AddRange"/> raises a notification that
/// <see cref="ListCollectionView"/> (and thus every ItemsControl) can consume.
/// </summary>
public sealed class ObservableCollectionExCollectionViewTests
{
    [Fact]
    public void AddRange_raises_a_single_Reset_event()
    {
        var sut = new ObservableCollectionEx<int>();
        var events = new List<NotifyCollectionChangedEventArgs>();
        sut.CollectionChanged += (_, e) => events.Add(e);

        sut.AddRange([1, 2, 3]);

        var e = Assert.Single(events);
        Assert.Equal(NotifyCollectionChangedAction.Reset, e.Action);
        Assert.Null(e.NewItems);
        Assert.Equal(-1, e.NewStartingIndex);
        Assert.Equal(new[] { 1, 2, 3 }, sut);
    }

    [Fact]
    public void AddRange_with_empty_input_raises_no_event()
    {
        var sut = new ObservableCollectionEx<int> { 1 };
        var events = new List<NotifyCollectionChangedEventArgs>();
        sut.CollectionChanged += (_, e) => events.Add(e);

        sut.AddRange(Array.Empty<int>());

        Assert.Empty(events);
        Assert.Equal(new[] { 1 }, sut);
    }

    [Fact]
    public void AddRange_while_notifications_are_suppressed_keeps_them_suppressed()
    {
        var sut = new ObservableCollectionEx<int>();
        var events = new List<NotifyCollectionChangedEventArgs>();
        sut.CollectionChanged += (_, e) => events.Add(e);
        sut.SuppressOnChangedNotification = true;

        sut.AddRange([1, 2]);

        Assert.Empty(events);
        Assert.True(sut.SuppressOnChangedNotification);
        Assert.Equal(new[] { 1, 2 }, sut);
    }

    [Fact]
    public void AddRange_enumerates_the_input_only_once()
    {
        var sut = new ObservableCollectionEx<int>();
        var enumerations = 0;

        sut.AddRange(Items());

        Assert.Equal(1, enumerations);
        Assert.Equal(new[] { 1, 2 }, sut);

        IEnumerable<int> Items()
        {
            enumerations++;
            yield return 1;
            yield return 2;
        }
    }

    [StaFact]
    public void ListCollectionView_follows_AddRange_without_throwing()
    {
        var sut = new ObservableCollectionEx<string>();
        var view = (ListCollectionView)CollectionViewSource.GetDefaultView(sut);

        var exception = Record.Exception(() => sut.AddRange(["b", "a", "c"]));

        Assert.Null(exception);
        Assert.Equal(3, view.Count);
        Assert.Equal(["b", "a", "c"], view.Cast<string>(), StringComparer.Ordinal);
    }

    [StaFact]
    public void Sorted_ListCollectionView_contains_each_added_item_once_after_AddRange()
    {
        var sut = new ObservableCollectionEx<string> { "m" };
        var view = (ListCollectionView)CollectionViewSource.GetDefaultView(sut);
        view.SortDescriptions.Add(new SortDescription(string.Empty, ListSortDirection.Ascending));

        var exception = Record.Exception(() => sut.AddRange(["b", "a"]));

        Assert.Null(exception);
        Assert.Equal(3, view.Count);
        Assert.Equal(["a", "b", "m"], view.Cast<string>(), StringComparer.Ordinal);
    }

    [StaFact]
    public void ListCollectionView_raises_Add_not_Reset_for_a_single_Add()
    {
        var sut = new ObservableCollectionEx<string> { "a" };
        var view = new ListCollectionView(sut);
        var events = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)view).CollectionChanged += (_, e) => events.Add(e.Action);

        sut.Add("b");

        Assert.Equal([NotifyCollectionChangedAction.Add], events);
        Assert.Equal(["a", "b"], view.Cast<string>(), StringComparer.Ordinal);
    }

    [StaFact]
    public void ListCollectionView_raises_Remove_Move_and_Replace_for_single_changes()
    {
        var sut = new ObservableCollectionEx<string> { "a", "b", "c" };
        var view = new ListCollectionView(sut);
        var events = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)view).CollectionChanged += (_, e) => events.Add(e.Action);

        sut.Remove("b");
        sut.Move(0, 1);
        sut[0] = "x";

        Assert.Equal(
            [NotifyCollectionChangedAction.Remove, NotifyCollectionChangedAction.Move, NotifyCollectionChangedAction.Replace],
            events);
        Assert.Equal(["x", "a"], view.Cast<string>(), StringComparer.Ordinal);
    }

    [StaFact]
    public void Add_while_the_view_has_a_pending_EditItem_does_not_throw()
    {
        var sut = new ObservableCollectionEx<EditableRow> { new() { Name = "a" } };
        var view = new ListCollectionView(sut);
        view.EditItem(sut[0]);

        var exception = Record.Exception(() => sut.Add(new EditableRow { Name = "b" }));

        Assert.Null(exception);
        Assert.True(view.IsEditingItem);
        Assert.Equal(2, view.Count);
        view.CancelEdit();
    }

    [StaFact]
    public void Add_while_a_DataGrid_is_editing_a_row_does_not_throw()
    {
        var sut = new ObservableCollectionEx<EditableRow> { new() { Name = "a" } };
        var dataGrid = new DataGrid { CanUserAddRows = false, ItemsSource = sut };
        var view = (IEditableCollectionView)dataGrid.Items;
        view.EditItem(sut[0]);

        var exception = Record.Exception(() => sut.Add(new EditableRow { Name = "b" }));

        Assert.Null(exception);
        Assert.Equal(2, dataGrid.Items.Count);
        view.CancelEdit();
    }

    private sealed class EditableRow : IEditableObject
    {
        public string Name { get; set; } = string.Empty;

        public void BeginEdit()
        {
        }

        public void CancelEdit()
        {
        }

        public void EndEdit()
        {
        }
    }
}