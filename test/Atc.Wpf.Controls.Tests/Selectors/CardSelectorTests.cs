namespace Atc.Wpf.Controls.Tests.Selectors;

public sealed class CardSelectorTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_MatchTheDocumentedApi()
    {
        var sut = new CardSelector();

        Assert.Null(sut.KeyMemberPath);
        Assert.NotNull(sut.SelectedKeys);
        Assert.Empty(sut.SelectedKeys);
        Assert.Empty(sut.SelectedItems);
        Assert.Equal(CardSelectionMode.Multiple, sut.SelectionMode);
        Assert.Equal(200d, sut.CardWidth);
        Assert.True(double.IsNaN(sut.CardHeight));
        Assert.Equal(8d, sut.ItemSpacing);
        Assert.True(sut.ShowCheckMark);
        Assert.Null(sut.MaxSelectionCount);
        Assert.Null(sut.IsEnabledMemberPath);
    }

    [StaFact]
    public void Each_instance_gets_its_own_SelectedKeys()
        => Assert.NotSame(new CardSelector().SelectedKeys, new CardSelector().SelectedKeys);

    [StaFact]
    public void SelectionMode_is_settable_from_xaml_and_hides_the_ListBox_one()
    {
        var sut = (CardSelector)XamlReader.Parse(
            "<atc:CardSelector xmlns:atc=\"https://github.com/atc-net/atc-wpf/tree/main/schemas\" SelectionMode=\"SingleOrNone\" />");

        Assert.Equal(CardSelectionMode.SingleOrNone, sut.SelectionMode);
        Assert.Equal(System.Windows.Controls.SelectionMode.Multiple, ((ListBox)sut).SelectionMode);

        // Bindings (as used by the card template triggers) resolve to the new property.
        var probe = new TextBlock();
        probe.SetBinding(TextBlock.TagProperty, new Binding(nameof(CardSelector.SelectionMode)) { Source = sut });
        Assert.Equal(CardSelectionMode.SingleOrNone, probe.Tag);
    }

    [StaFact]
    public void Multiple_toggles_cards_on_and_off_with_one_event_each()
    {
        var (sut, items) = Create();
        var events = Record(sut);
        var selectionChanged = 0;
        sut.SelectionChanged += (_, _) => selectionChanged++;

        Assert.True(sut.RequestToggle(items[0]));
        Assert.True(sut.RequestToggle(items[2]));

        Assert.Equal([items[0], items[2]], sut.SelectedItems.Cast<Plan>());
        Assert.Equal(["basic", "pro"], sut.SelectedKeys);
        Assert.Equal(2, events.Count);
        Assert.Empty(events[0].OldValue);
        Assert.Equal(["basic"], events[0].NewValue);
        Assert.Equal(["basic", "pro"], events[1].NewValue);
        Assert.Equal(2, selectionChanged);

        sut.RequestToggle(items[0]);

        Assert.Equal(["pro"], sut.SelectedKeys);
        Assert.Equal(3, events.Count);
        Assert.Equal(3, selectionChanged);
    }

    [StaFact]
    public void Single_keeps_exactly_one_card()
    {
        var (sut, items) = Create(x => x.SelectionMode = CardSelectionMode.Single);
        var events = Record(sut);

        sut.RequestToggle(items[0]);
        sut.RequestToggle(items[1]);
        sut.RequestToggle(items[1]);

        Assert.Equal([items[1]], sut.SelectedItems.Cast<Plan>());
        Assert.Equal(["standard"], sut.SelectedKeys);
        Assert.Equal(2, events.Count);
        Assert.Equal(["basic"], events[1].OldValue);
    }

    [StaFact]
    public void SingleOrNone_allows_clearing_the_card()
    {
        var (sut, items) = Create(x => x.SelectionMode = CardSelectionMode.SingleOrNone);

        sut.RequestToggle(items[0]);
        sut.RequestToggle(items[1]);
        Assert.Equal(["standard"], sut.SelectedKeys);

        sut.RequestToggle(items[1]);

        Assert.Empty(sut.SelectedItems);
        Assert.Empty(sut.SelectedKeys);
    }

    [StaFact]
    public void Switching_to_Single_keeps_the_first_selected_card()
    {
        var (sut, items) = Create();
        sut.RequestToggle(items[1]);
        sut.RequestToggle(items[2]);
        var events = Record(sut);

        sut.SelectionMode = CardSelectionMode.Single;

        Assert.Equal(["standard"], sut.SelectedKeys);
        Assert.Single(sut.SelectedItems);
        Assert.Single(events);
    }

    [StaFact]
    public void Without_KeyMemberPath_SelectedItems_works_and_SelectedKeys_stays_empty()
    {
        var (sut, items) = Create(x => x.KeyMemberPath = null);
        var events = Record(sut);

        sut.RequestToggle(items[1]);

        Assert.Equal([items[1]], sut.SelectedItems.Cast<Plan>());
        Assert.Empty(sut.SelectedKeys);
        Assert.Empty(events);
    }

    [StaFact]
    public void SelectedKeys_set_before_ItemsSource_selects_the_cards_without_an_event()
    {
        var items = Plans();
        var sut = new CardSelector { KeyMemberPath = nameof(Plan.Id) };
        var events = Record(sut);
        sut.SelectedKeys = ["pro", "basic"];

        sut.ItemsSource = items;

        Assert.Equal([items[0], items[2]], sut.SelectedItems.Cast<Plan>().OrderBy(x => items.IndexOf(x)));
        Assert.Equal(["pro", "basic"], sut.SelectedKeys);
        Assert.Empty(events);
    }

    [StaFact]
    public void SelectedKeys_changed_from_outside_resyncs_the_cards_without_an_event()
    {
        var (sut, items) = Create();
        var events = Record(sut);

        sut.SelectedKeys.Add("standard");
        Assert.Equal([items[1]], sut.SelectedItems.Cast<Plan>());

        sut.SelectedKeys = ["pro"];
        Assert.Equal([items[2]], sut.SelectedItems.Cast<Plan>());

        sut.SelectedKeys.Clear();
        Assert.Empty(sut.SelectedItems);
        Assert.Empty(events);
    }

    [StaFact]
    public void Toggling_updates_the_bound_collection_in_place()
    {
        var (sut, items) = Create();
        var keys = new System.Collections.ObjectModel.ObservableCollection<string>();
        sut.SelectedKeys = keys;

        sut.RequestToggle(items[2]);

        Assert.Same(keys, sut.SelectedKeys);
        Assert.Equal(["pro"], keys);
    }

    [StaFact]
    public void Removing_a_selected_item_drops_its_key_with_one_event()
    {
        var (sut, items) = Create();
        sut.RequestToggle(items[0]);
        sut.RequestToggle(items[1]);
        var events = Record(sut);

        items.RemoveAt(0);

        Assert.Equal(["standard"], sut.SelectedKeys);
        Assert.Equal([items[0]], sut.SelectedItems.Cast<Plan>());
        var change = Assert.Single(events);
        Assert.Equal(["basic", "standard"], change.OldValue);
        Assert.Equal(["standard"], change.NewValue);
    }

    [StaFact]
    public void Reloading_the_items_keeps_the_selection_by_key()
    {
        var (sut, items) = Create();
        sut.RequestToggle(items[1]);
        var events = Record(sut);
        var reloaded = Plans();

        sut.ItemsSource = reloaded;

        Assert.Equal(["standard"], sut.SelectedKeys);
        Assert.Equal([reloaded[1]], sut.SelectedItems.Cast<Plan>());
        Assert.Empty(events);
    }

    [StaFact]
    public void Reloading_without_a_selected_key_drops_it_with_one_event()
    {
        var (sut, items) = Create();
        sut.RequestToggle(items[0]);
        sut.RequestToggle(items[1]);
        var events = Record(sut);
        var reloaded = Plans();
        reloaded.RemoveAt(0);

        sut.ItemsSource = reloaded;

        Assert.Equal(["standard"], sut.SelectedKeys);
        Assert.Equal([reloaded[0]], sut.SelectedItems.Cast<Plan>());
        Assert.Single(events);
    }

    [StaFact]
    public void MaxSelectionCount_blocks_selecting_beyond_the_limit()
    {
        var (sut, items) = Create(x => x.MaxSelectionCount = 2);
        sut.RequestToggle(items[0]);
        sut.RequestToggle(items[1]);
        var events = Record(sut);

        Assert.False(sut.RequestToggle(items[2]));
        Assert.Equal(["basic", "standard"], sut.SelectedKeys);
        Assert.Empty(events);

        sut.SelectAll();
        Assert.Equal(2, sut.SelectedItems.Count);
        Assert.Empty(events);

        Assert.True(sut.RequestToggle(items[0]));
        Assert.True(sut.RequestToggle(items[2]));
        Assert.Equal(["standard", "pro"], sut.SelectedKeys);
    }

    [StaFact]
    public void Disabled_cards_cannot_be_toggled_and_are_skipped_by_select_all_and_clear()
    {
        var (sut, items) = Create(x => x.IsEnabledMemberPath = nameof(Plan.IsAvailable));
        items[3].IsAvailable = false;
        Assert.False(sut.IsItemEnabled(items[3]));
        Assert.True(sut.IsItemEnabled(items[0]));

        Assert.False(sut.RequestToggle(items[3]));
        Assert.Empty(sut.SelectedItems);

        sut.SelectAll();
        Assert.Equal(["basic", "standard", "pro"], sut.SelectedKeys);

        sut.SelectedKeys.Add("enterprise");
        var events = Record(sut);
        sut.ClearSelection();

        Assert.Equal(["enterprise"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void SelectAll_raises_one_event_and_only_works_in_Multiple()
    {
        var (sut, _) = Create();
        var events = Record(sut);

        sut.SelectAll();

        Assert.Equal(["basic", "standard", "pro", "enterprise"], sut.SelectedKeys);
        Assert.Single(events);

        var (single, _) = Create(x => x.SelectionMode = CardSelectionMode.Single);
        single.SelectAll();
        Assert.Empty(single.SelectedItems);
    }

    [StaFact]
    public void Ctrl_A_selects_all_in_Multiple_only()
    {
        var (sut, _) = Create();

        Assert.True(sut.HandleKey(null, Key.A, ModifierKeys.Control));
        Assert.Equal(4, sut.SelectedItems.Count);

        var (single, _) = Create(x => x.SelectionMode = CardSelectionMode.Single);
        Assert.False(single.HandleKey(null, Key.A, ModifierKeys.Control));
        Assert.Empty(single.SelectedItems);
    }

    [StaFact]
    public void Nested_KeyMemberPath_is_supported()
    {
        var items = new System.Collections.ObjectModel.ObservableCollection<Device>
        {
            new(new Plan("cam-1", "Camera 1")),
            new(new Plan("cam-2", "Camera 2")),
        };
        var sut = new CardSelector
        {
            KeyMemberPath = "Info.Id",
            ItemsSource = items,
        };

        sut.RequestToggle(items[1]);

        Assert.Equal(["cam-2"], sut.SelectedKeys);
    }

    internal static System.Collections.ObjectModel.ObservableCollection<Plan> Plans()
        =>
        [
            new("basic", "Basic"),
            new("standard", "Standard"),
            new("pro", "Pro"),
            new("enterprise", "Enterprise"),
        ];

    [StaTheory]
    [InlineData(CardSelectionMode.Single)]
    [InlineData(CardSelectionMode.SingleOrNone)]
    public void A_single_mode_writes_only_the_clicked_key_when_a_restored_key_has_no_card(
        CardSelectionMode mode)
    {
        var (sut, items) = Create(x => x.SelectionMode = mode);
        sut.SelectedKeys = ["legacy"];
        var events = Record(sut);

        Assert.True(sut.RequestToggle(items[0]));

        Assert.Equal(["basic"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Equal(["legacy"], change.OldValue);
        Assert.Equal(["basic"], change.NewValue);
    }

    private static (CardSelector Sut, System.Collections.ObjectModel.ObservableCollection<Plan> Items) Create(
        Action<CardSelector>? configure = null)
    {
        var items = Plans();
        var sut = new CardSelector
        {
            KeyMemberPath = nameof(Plan.Id),
            ItemsSource = items,
        };
        configure?.Invoke(sut);
        return (sut, items);
    }

    private static List<ValueChangedEventArgs<IReadOnlyList<string>>> Record(
        CardSelector sut)
    {
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);
        return events;
    }

    internal sealed class Plan(
        string id,
        string title)
    {
        public string Id { get; } = id;

        public string Title { get; } = title;

        public bool IsAvailable { get; set; } = true;
    }

    internal sealed record Device(Plan Info);
}