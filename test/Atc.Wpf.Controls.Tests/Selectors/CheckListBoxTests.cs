namespace Atc.Wpf.Controls.Tests.Selectors;

public sealed class CheckListBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_MatchTheDocumentedApi()
    {
        var sut = new CheckListBox();

        Assert.NotNull(sut.Items);
        Assert.Empty(sut.Items);
        Assert.NotNull(sut.SelectedKeys);
        Assert.Empty(sut.SelectedKeys);
        Assert.NotNull(sut.DisabledKeys);
        Assert.False(sut.SelectNewItems);
        Assert.True(sut.ShowSelectAllRow);
        Assert.Null(sut.IsFilterVisible);
        Assert.Null(sut.ItemGroups);
        Assert.Null(sut.MaxSelectionCount);
        Assert.Equal(CheckListItemStyle.CheckBox, sut.ItemStyle);
        Assert.Equal(1, sut.Columns);
        Assert.Null(sut.ItemTemplate);
    }

    [StaFact]
    public void Each_instance_gets_its_own_collections()
    {
        var first = new CheckListBox();
        var second = new CheckListBox();

        Assert.NotSame(first.SelectedKeys, second.SelectedKeys);
        Assert.NotSame(first.DisabledKeys, second.DisabledKeys);
    }

    [StaFact]
    public void Tick_and_untick_update_SelectedKeys_and_raise_one_event_each()
    {
        var sut = Create(5);
        var events = Record(sut);

        sut.GetItem("k2")!.IsChecked = true;

        Assert.Equal(["k2"], sut.SelectedKeys);
        var tick = Assert.Single(events);
        Assert.Empty(tick.OldValue);
        Assert.Equal(["k2"], tick.NewValue);

        sut.GetItem("k2")!.IsChecked = false;

        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(2, events.Count);
        Assert.Equal(["k2"], events[1].OldValue);
        Assert.Empty(events[1].NewValue);
    }

    [StaFact]
    public void Select_all_row_is_three_state()
    {
        var sut = Create(3);

        Assert.False(sut.PartSelectAll.IsChecked);

        sut.GetItem("k1")!.IsChecked = true;
        Assert.Null(sut.PartSelectAll.IsChecked);

        sut.GetItem("k2")!.IsChecked = true;
        sut.GetItem("k3")!.IsChecked = true;
        Assert.True(sut.PartSelectAll.IsChecked);
    }

    [StaFact]
    public void Select_all_through_UI_Automation_from_indeterminate_ticks_all_in_one_event()
    {
        var sut = Create(5);
        sut.GetItem("k1")!.IsChecked = true;
        sut.GetItem("k3")!.IsChecked = true;
        var events = Record(sut);
        var toggle = GetToggle(sut.PartSelectAll);
        Assert.Equal(System.Windows.Automation.ToggleState.Indeterminate, toggle.ToggleState);

        toggle.Toggle();

        Assert.Equal(["k1", "k3", "k2", "k4", "k5"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.Equal(System.Windows.Automation.ToggleState.On, toggle.ToggleState);
        Assert.True(sut.PartSelectAll.IsChecked);
        Assert.Equal(string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.CountOf2, 5, 5), sut.PartCount.Text);
    }

    [StaFact]
    public void Select_all_from_all_ticked_clears_in_one_event()
    {
        var sut = Create(4);
        sut.SelectAll();
        var events = Record(sut);

        GetToggle(sut.PartSelectAll).Toggle();

        Assert.Empty(sut.SelectedKeys);
        Assert.Single(events);
        Assert.False(sut.PartSelectAll.IsChecked);
    }

    [StaFact]
    public void ClearSelection_raises_one_event()
    {
        var sut = Create(4);
        sut.SelectAll();
        var events = Record(sut);

        sut.ClearSelection();

        Assert.Empty(sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Select_all_with_a_filter_ticks_only_the_shown_items_and_counts_all_items()
    {
        var sut = Create(0);
        sut.Items = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["a"] = "Apple",
            ["b"] = "Banana",
            ["c"] = "Apricot",
            ["d"] = "Cherry",
        };
        var events = Record(sut);

        sut.FilterText = "ap";
        sut.FlushPendingFilter();

        Assert.Equal(["a", "c"], sut.VisibleRows.Select(x => x.Key), StringComparer.Ordinal);

        sut.SelectAll();

        Assert.Equal(["a", "c"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.True(sut.PartSelectAll.IsChecked);
        Assert.Equal(string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.CountOf2, 2, 4), sut.PartCount.Text);
    }

    [StaFact]
    public void Filter_with_no_matches_shows_the_no_matches_text()
    {
        var sut = Create(3);

        sut.FilterText = "zzz";
        sut.FlushPendingFilter();

        Assert.Empty(sut.VisibleRows);
        Assert.Equal(Visibility.Visible, sut.PartNoMatches.Visibility);

        sut.FilterText = string.Empty;
        sut.FlushPendingFilter();

        Assert.Equal(3, sut.VisibleRows.Count);
        Assert.Equal(Visibility.Collapsed, sut.PartNoMatches.Visibility);
    }

    [StaTheory]
    [InlineData(null, 10, false)]
    [InlineData(null, 11, true)]
    [InlineData(true, 2, true)]
    [InlineData(false, 50, false)]
    public void IsFilterVisible_null_means_more_than_ten_items(
        bool? isFilterVisible,
        int itemCount,
        bool expectedVisible)
    {
        var sut = Create(itemCount);

        sut.IsFilterVisible = isFilterVisible;

        Assert.Equal(expectedVisible ? Visibility.Visible : Visibility.Collapsed, sut.PartFilter.Visibility);
    }

    [StaFact]
    public void Filtering_large_lists_is_debounced_until_flushed()
    {
        var sut = Create(KeySelectionHelperThreshold + 1);

        sut.FilterText = "k1";

        Assert.Equal(KeySelectionHelperThreshold + 1, sut.VisibleRows.Count);

        sut.FlushPendingFilter();

        Assert.True(sut.VisibleRows.Count < KeySelectionHelperThreshold);
        Assert.All(sut.VisibleRows, x => Assert.Contains("k1", x.DisplayValue, StringComparison.Ordinal));
    }

    [StaFact]
    public void Groups_add_tri_state_headers_that_tick_their_items_in_one_event()
    {
        var sut = CreateGrouped();
        var events = Record(sut);

        Assert.Equal(
            ["x", "Fruit", "a", "b", "Veg", "c", "d"],
            sut.VisibleRows.Select(x => x.Key),
            StringComparer.Ordinal);
        var fruit = sut.VisibleRows.Single(x => x.IsGroupHeader && x.Key == "Fruit");
        Assert.False(fruit.IsChecked);

        sut.GetItem("a")!.IsChecked = true;
        Assert.Null(fruit.IsChecked);

        fruit.IsChecked = true;

        Assert.Equal(["a", "b"], sut.SelectedKeys);
        Assert.True(fruit.IsChecked);
        Assert.Equal(2, events.Count);

        fruit.IsChecked = false;

        Assert.Empty(sut.SelectedKeys);
        Assert.False(fruit.IsChecked);
        Assert.Equal(3, events.Count);
    }

    [StaFact]
    public void Group_header_from_indeterminate_ticks_the_whole_group()
    {
        var sut = CreateGrouped();
        sut.GetItem("c")!.IsChecked = true;
        var veg = sut.VisibleRows.Single(x => x.IsGroupHeader && x.Key == "Veg");
        Assert.Null(veg.IsChecked);

        veg.IsChecked = false;

        Assert.Equal(["c", "d"], sut.SelectedKeys);
        Assert.True(veg.IsChecked);
    }

    [StaFact]
    public void Filter_hides_groups_without_matches()
    {
        var sut = CreateGrouped();

        sut.FilterText = "Carrot";
        sut.FlushPendingFilter();

        Assert.Equal(["Veg", "c"], sut.VisibleRows.Select(x => x.Key), StringComparer.Ordinal);
    }

    [StaFact]
    public void Group_headers_are_plain_when_groups_are_not_checkable()
    {
        var sut = CreateGrouped();

        sut.IsGroupCheckable = false;
        var fruit = sut.VisibleRows.Single(x => x.IsGroupHeader && x.Key == "Fruit");
        fruit.IsChecked = true;

        Assert.Empty(sut.SelectedKeys);
    }

    [StaFact]
    public void Shift_click_range_ticks_from_the_last_clicked_row_in_one_event()
    {
        var sut = Create(6);
        sut.GetItem("k2")!.IsChecked = true;
        var events = Record(sut);

        sut.ToggleRange(sut.GetItem("k5")!);

        Assert.Equal(["k2", "k3", "k4", "k5"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Shift_click_range_unticks_when_the_clicked_row_was_ticked()
    {
        var sut = Create(6);
        sut.SelectAll();
        sut.GetItem("k5")!.IsChecked = false;
        var events = Record(sut);

        sut.ToggleRange(sut.GetItem("k2")!);

        Assert.Equal(["k1", "k6"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Shift_click_range_skips_disabled_keys()
    {
        var sut = Create(5);
        sut.DisabledKeys.Add("k3");
        sut.GetItem("k1")!.IsChecked = true;

        sut.ToggleRange(sut.GetItem("k5")!);

        Assert.Equal(["k1", "k2", "k4", "k5"], sut.SelectedKeys);
    }

    [StaFact]
    public void Limit_disables_the_remaining_items_and_blocks_select_all_that_does_not_fit()
    {
        var sut = Create(5);
        sut.MaxSelectionCount = 2;

        sut.GetItem("k1")!.IsChecked = true;
        Assert.False(sut.PartSelectAll.IsEnabled);

        sut.GetItem("k2")!.IsChecked = true;

        Assert.All(new[] { "k3", "k4", "k5" }, key => Assert.False(sut.GetItem(key)!.IsEnabled));
        Assert.True(sut.GetItem("k1")!.IsEnabled);

        sut.GetItem("k3")!.IsChecked = true;
        Assert.Equal(["k1", "k2"], sut.SelectedKeys);

        sut.SelectAll();
        Assert.Equal(["k1", "k2"], sut.SelectedKeys);
        Assert.Equal(string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.CountOfMax2, 2, 2), sut.PartCount.Text);

        sut.GetItem("k1")!.IsChecked = false;
        Assert.All(new[] { "k1", "k3", "k4", "k5" }, key => Assert.True(sut.GetItem(key)!.IsEnabled));
    }

    [StaFact]
    public void Limit_allows_select_all_when_the_shown_items_fit()
    {
        var sut = Create(3);
        sut.MaxSelectionCount = 3;

        sut.SelectAll();

        Assert.Equal(["k1", "k2", "k3"], sut.SelectedKeys);
    }

    [StaFact]
    public void Disabled_keys_cannot_be_toggled_and_select_all_skips_them()
    {
        var sut = Create(4);
        sut.DisabledKeys.Add("k2");

        Assert.False(sut.GetItem("k2")!.IsEnabled);

        sut.GetItem("k2")!.IsChecked = true;
        Assert.Empty(sut.SelectedKeys);

        sut.SelectAll();
        Assert.Equal(["k1", "k3", "k4"], sut.SelectedKeys);
        Assert.True(sut.PartSelectAll.IsChecked);

        sut.ClearSelection();
        Assert.Empty(sut.SelectedKeys);
    }

    [StaFact]
    public void Disabled_ticked_keys_survive_select_all_toggling_and_clear()
    {
        var sut = Create(3);
        sut.SelectedKeys.Add("k2");
        sut.DisabledKeys.Add("k2");

        sut.SelectAll();
        sut.ClearSelection();

        Assert.Equal(["k2"], sut.SelectedKeys);
    }

    [StaFact]
    public void Items_dropping_a_selected_key_removes_it_in_one_event()
    {
        var sut = Create(4);
        sut.SelectAll();
        var events = Record(sut);

        sut.Items = Items(2);

        Assert.Equal(["k1", "k2"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Equal(["k1", "k2", "k3", "k4"], change.OldValue);
    }

    [StaFact]
    public void SelectNewItems_ticks_keys_that_were_not_there_before()
    {
        var sut = Create(2);
        sut.SelectNewItems = true;
        sut.ClearSelection();
        var events = Record(sut);

        sut.Items = Items(4);

        Assert.Equal(["k3", "k4"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Changing_items_keeps_ticks_and_row_instances()
    {
        var sut = Create(3);
        sut.GetItem("k2")!.IsChecked = true;
        var row = sut.GetItem("k2");

        sut.Items = Items(5);

        Assert.Same(row, sut.GetItem("k2"));
        Assert.True(sut.GetItem("k2")!.IsChecked);
        Assert.Equal(["k2"], sut.SelectedKeys);
    }

    [StaFact]
    public void A_new_SelectedKeys_instance_is_picked_up_without_an_event()
    {
        var sut = Create(3);
        var events = Record(sut);

        sut.SelectedKeys = ["k1", "k3"];

        Assert.True(sut.GetItem("k1")!.IsChecked);
        Assert.False(sut.GetItem("k2")!.IsChecked);
        Assert.True(sut.GetItem("k3")!.IsChecked);
        Assert.Null(sut.PartSelectAll.IsChecked);
        Assert.Empty(events);

        sut.GetItem("k2")!.IsChecked = true;
        Assert.Equal(["k1", "k3", "k2"], sut.SelectedKeys);
    }

    [StaFact]
    public void External_changes_to_SelectedKeys_resync_the_rows_without_an_event()
    {
        var sut = Create(3);
        var events = Record(sut);

        sut.SelectedKeys.Add("k2");
        Assert.True(sut.GetItem("k2")!.IsChecked);

        sut.SelectedKeys.Remove("k2");
        Assert.False(sut.GetItem("k2")!.IsChecked);

        Assert.Empty(events);
    }

    [StaFact]
    public void Select_all_on_an_ObservableCollectionEx_raises_a_single_collection_notification()
    {
        var sut = Create(50);
        var keys = new Atc.Wpf.Collections.ObservableCollectionEx<string>();
        sut.SelectedKeys = keys;
        var notifications = 0;
        keys.CollectionChanged += (_, _) => notifications++;

        sut.SelectAll();

        Assert.Equal(50, keys.Count);
        Assert.Equal(1, notifications);
    }

    [StaTheory]
    [InlineData(CheckListItemStyle.CheckBox)]
    [InlineData(CheckListItemStyle.Highlight)]
    [InlineData(CheckListItemStyle.Switch)]
    public void The_three_item_styles_toggle_the_same_way(
        CheckListItemStyle itemStyle)
    {
        var sut = Create(3);
        sut.ItemStyle = itemStyle;
        var events = Record(sut);
        Realize(sut);

        var container = (ListBoxItem)sut.PartList.ItemContainerGenerator.ContainerFromItem(sut.GetItem("k2")!);
        Assert.NotNull(container);

        if (itemStyle == CheckListItemStyle.Switch)
        {
            var toggleSwitch = FindVisualChild<ToggleSwitch>(container);
            Assert.NotNull(toggleSwitch);
            toggleSwitch.IsOn = true;
        }
        else
        {
            var toggleButton = FindVisualChild<ToggleButton>(container);
            Assert.NotNull(toggleButton);
            Assert.Equal(itemStyle == CheckListItemStyle.CheckBox, toggleButton is CheckBox);
            Assert.Equal("Item 2", System.Windows.Automation.AutomationProperties.GetName(toggleButton));
            GetToggle(toggleButton).Toggle();
        }

        Assert.Equal(["k2"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Space_on_a_focused_row_toggles_it()
    {
        var sut = Create(3);
        Realize(sut);
        var container = (ListBoxItem)sut.PartList.ItemContainerGenerator.ContainerFromItem(sut.GetItem("k1")!);

        KeyPress.Send(container, Key.Space);

        Assert.Equal(["k1"], sut.SelectedKeys);
    }

    [StaFact]
    public void Columns_above_one_lay_items_out_in_a_uniform_grid()
    {
        var sut = Create(7);

        sut.Columns = 3;
        Realize(sut);

        var panel = FindVisualChild<System.Windows.Controls.Primitives.UniformGrid>(sut.PartList);
        Assert.NotNull(panel);
        Assert.Equal(3, panel.Columns);
    }

    [StaFact]
    public void ShowSelectAllRow_false_hides_the_row()
    {
        var sut = Create(3);

        sut.ShowSelectAllRow = false;

        Assert.Equal(Visibility.Collapsed, sut.PartSelectAllRow.Visibility);
    }

    private const int KeySelectionHelperThreshold = 2000;

    private static CheckListBox Create(int itemCount)
        => new() { Items = Items(itemCount) };

    private static CheckListBox CreateGrouped()
    {
        var sut = new CheckListBox
        {
            Items = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["a"] = "Apple",
                ["c"] = "Carrot",
                ["b"] = "Banana",
                ["x"] = "Misc",
                ["d"] = "Daikon",
            },
            ItemGroups = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["a"] = "Fruit",
                ["b"] = "Fruit",
                ["c"] = "Veg",
                ["d"] = "Veg",
            },
        };

        return sut;
    }

    private static Dictionary<string, string> Items(int count)
        => Enumerable.Range(1, count).ToDictionary(
            i => "k" + i.ToString(CultureInfo.InvariantCulture),
            i => "Item " + i.ToString(CultureInfo.InvariantCulture),
            StringComparer.Ordinal);

    private static List<ValueChangedEventArgs<IReadOnlyList<string>>> Record(
        CheckListBox sut)
    {
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);
        return events;
    }

    private static IToggleProvider GetToggle(UIElement element)
    {
        var peer = UIElementAutomationPeer.CreatePeerForElement(element);
        return (IToggleProvider)peer.GetPattern(PatternInterface.Toggle);
    }

    private static void Realize(CheckListBox sut)
    {
        sut.Measure(new Size(400, 600));
        sut.Arrange(new Rect(0, 0, 400, 600));
        sut.UpdateLayout();
    }

    private static T? FindVisualChild<T>(DependencyObject parent)
        where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
            {
                return match;
            }

            var descendant = FindVisualChild<T>(child);
            if (descendant is not null)
            {
                return descendant;
            }
        }

        return null;
    }
}