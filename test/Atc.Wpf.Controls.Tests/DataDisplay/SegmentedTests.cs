namespace Atc.Wpf.Controls.Tests.DataDisplay;

public sealed class SegmentedTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_keep_the_single_select_horizontal_look()
    {
        var sut = new Segmented();

        Assert.Equal(SegmentedSelectionMode.Single, sut.SelectionMode);
        Assert.Equal(Orientation.Horizontal, sut.Orientation);
        Assert.True(sut.IsJoined);
        Assert.Equal(4d, sut.ItemSpacing);
        Assert.False(sut.IconOnly);
        Assert.Equal(SegmentedSize.Medium, sut.Size);
        Assert.Equal(-1, sut.SelectedIndex);
        Assert.NotNull(sut.SelectedKeys);
        Assert.Empty(sut.SelectedKeys);
        Assert.Empty(sut.SelectedItems);
        Assert.NotSame(sut.SelectedKeys, new Segmented().SelectedKeys);
    }

    [StaFact]
    public void Item_key_falls_back_to_the_content_text()
    {
        Assert.Equal("mon", new SegmentedItem { Key = "mon", Content = "Mon" }.EffectiveKey);
        Assert.Equal("Tue", new SegmentedItem { Content = "Tue" }.EffectiveKey);
        Assert.Null(new SegmentedItem().EffectiveKey);
    }

    [StaFact]
    public void Single_click_selects_one_item_and_keeps_keys_in_sync()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);
        var events = Record(sut);
        var selectionEvents = RecordSelection(sut);

        sut.Items[1].PerformClick();
        sut.Items[3].PerformClick();

        Assert.Equal(3, sut.SelectedIndex);
        Assert.Same(sut.Items[3], sut.SelectedItem);
        Assert.Equal(["thu"], sut.SelectedKeys);
        Assert.Equal([sut.Items[3]], sut.SelectedItems);
        Assert.False(sut.Items[1].IsSelected);
        Assert.True(sut.Items[3].IsSelected);
        Assert.Equal(2, events.Count);
        Assert.Equal(["tue"], events[1].OldValue);
        Assert.Equal(["thu"], events[1].NewValue);
        Assert.Equal(2, selectionEvents.Count);
        Assert.Equal(1, selectionEvents[1].OldIndex);
        Assert.Equal(3, selectionEvents[1].NewIndex);
    }

    [StaFact]
    public void Single_clicking_the_selected_item_keeps_it()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);
        sut.Items[2].PerformClick();
        var events = Record(sut);

        sut.Items[2].PerformClick();

        Assert.Equal(2, sut.SelectedIndex);
        Assert.Equal(["wed"], sut.SelectedKeys);
        Assert.Empty(events);
    }

    [StaFact]
    public void Single_selected_index_set_in_xaml_before_the_items_marks_the_item_selected()
    {
        var sut = new Segmented { SelectedIndex = 1 };
        sut.Items.Add(new SegmentedItem { Content = "A" });
        sut.Items.Add(new SegmentedItem { Content = "B" });

        Assert.True(sut.Items[1].IsSelected);
        Assert.Same(sut.Items[1], sut.SelectedItem);
        Assert.Equal(["B"], sut.SelectedKeys);
    }

    [StaFact]
    public void SingleOrNone_clicking_the_selected_item_clears_it()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.SingleOrNone);
        sut.Items[0].PerformClick();
        var events = Record(sut);

        sut.Items[0].PerformClick();

        Assert.Equal(-1, sut.SelectedIndex);
        Assert.Null(sut.SelectedItem);
        Assert.Empty(sut.SelectedKeys);
        Assert.Empty(sut.SelectedItems);
        Assert.False(sut.Items[0].IsSelected);
        var e = Assert.Single(events);
        Assert.Equal(["mon"], e.OldValue);
        Assert.Empty(e.NewValue);
    }

    [StaFact]
    public void Multiple_toggles_items_on_and_off_with_one_event_each()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);
        var events = Record(sut);

        sut.Items[0].PerformClick();
        sut.Items[2].PerformClick();

        Assert.True(sut.Items[0].IsSelected);
        Assert.False(sut.Items[1].IsSelected);
        Assert.True(sut.Items[2].IsSelected);
        Assert.Equal(["mon", "wed"], sut.SelectedKeys);
        Assert.Equal([sut.Items[0], sut.Items[2]], sut.SelectedItems);
        Assert.Equal(2, events.Count);

        sut.Items[0].PerformClick();

        Assert.False(sut.Items[0].IsSelected);
        Assert.True(sut.Items[2].IsSelected);
        Assert.Equal(["wed"], sut.SelectedKeys);
        Assert.Equal(3, events.Count);
        Assert.Equal(["mon", "wed"], events[2].OldValue);
        Assert.Equal(["wed"], events[2].NewValue);
    }

    [StaFact]
    public void Multiple_selected_index_is_the_most_recently_selected_item()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);
        var selectionEvents = RecordSelection(sut);

        sut.Items[4].PerformClick();
        sut.Items[1].PerformClick();

        Assert.Equal(1, sut.SelectedIndex);
        Assert.Same(sut.Items[1], sut.SelectedItem);

        sut.Items[1].PerformClick();

        Assert.Equal(4, sut.SelectedIndex);
        Assert.Same(sut.Items[4], sut.SelectedItem);

        sut.Items[4].PerformClick();

        Assert.Equal(-1, sut.SelectedIndex);
        Assert.Null(sut.SelectedItem);

        Assert.Equal(4, selectionEvents.Count);
        Assert.All(selectionEvents.Zip([4, 1, 1, 4]), x => Assert.Equal(x.Second, x.First.NewIndex));
        Assert.Same(sut.Items[1], selectionEvents[2].NewItem);
        Assert.False(selectionEvents[2].NewItem!.IsSelected);
    }

    [StaFact]
    public void Multiple_disabled_item_cannot_be_toggled()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);
        sut.Items[1].IsEnabled = false;
        var events = Record(sut);

        sut.Items[1].PerformClick();

        Assert.Empty(sut.SelectedKeys);
        Assert.Empty(events);
        Assert.Throws<System.Windows.Automation.ElementNotEnabledException>(
            () => ((IToggleProvider)UIElementAutomationPeer.CreatePeerForElement(sut.Items[1]).GetPattern(PatternInterface.Toggle)).Toggle());
    }

    [StaFact]
    public void Multiple_space_toggles_and_arrow_keys_do_not_change_the_selection()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);

        KeyPress.Send(sut.Items[2], Key.Space);

        Assert.Equal(["wed"], sut.SelectedKeys);

        KeyPress.Send(sut, Key.Right);
        KeyPress.Send(sut, Key.End);
        KeyPress.Send(sut, Key.Home);

        Assert.Equal(["wed"], sut.SelectedKeys);
        Assert.Equal(2, sut.SelectedIndex);

        KeyPress.Send(sut.Items[2], Key.Enter);

        Assert.Empty(sut.SelectedKeys);
    }

    [StaFact]
    public void Single_vertical_up_and_down_select_the_previous_and_next_segment()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);
        sut.Orientation = Orientation.Vertical;
        sut.SelectedIndex = 2;

        KeyPress.Send(sut, Key.Down);

        Assert.Equal(3, sut.SelectedIndex);

        KeyPress.Send(sut, Key.Up);
        KeyPress.Send(sut, Key.Up);

        Assert.Equal(1, sut.SelectedIndex);
        Assert.Equal(["tue"], sut.SelectedKeys);
    }

    [StaFact]
    public void Items_without_a_key_use_the_text_of_their_content()
    {
        var sut = new Segmented { SelectionMode = SegmentedSelectionMode.Multiple };
        sut.Items.Add(new SegmentedItem { Content = new TextBlock { Text = "Push" } });
        sut.Items.Add(new SegmentedItem { Content = new TextBlock { Text = "SMS" } });

        sut.Items[1].PerformClick();

        Assert.Equal(["SMS"], sut.SelectedKeys);
        Assert.False(sut.Items[0].IsSelected);
    }

    [StaFact]
    public void SelectedKeys_changed_from_outside_updates_the_items_without_an_event()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);
        var events = Record(sut);

        sut.SelectedKeys.Add("tue");
        sut.SelectedKeys.Add("fri");

        Assert.True(sut.Items[1].IsSelected);
        Assert.True(sut.Items[4].IsSelected);
        Assert.Equal([sut.Items[1], sut.Items[4]], sut.SelectedItems);
        Assert.Equal(4, sut.SelectedIndex);

        var replacement = new System.Collections.ObjectModel.ObservableCollection<string> { "mon" };
        sut.SelectedKeys = replacement;

        Assert.True(sut.Items[0].IsSelected);
        Assert.False(sut.Items[1].IsSelected);
        Assert.False(sut.Items[4].IsSelected);
        Assert.Equal(0, sut.SelectedIndex);

        sut.Items[2].PerformClick();

        Assert.Same(replacement, sut.SelectedKeys);
        Assert.Equal(["mon", "wed"], replacement);
        Assert.Single(events);
    }

    [StaFact]
    public void SelectedKeys_changed_from_outside_in_single_mode_moves_the_selected_index()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);
        var events = Record(sut);

        sut.SelectedKeys.Add("thu");

        Assert.Equal(3, sut.SelectedIndex);
        Assert.True(sut.Items[3].IsSelected);
        Assert.Empty(events);
    }

    [StaFact]
    public void SelectedKeys_bound_before_the_items_are_added_selects_them_when_added()
    {
        var sut = new Segmented
        {
            SelectionMode = SegmentedSelectionMode.Multiple,
            SelectedKeys = ["b", "c"],
        };
        sut.Items.Add(new SegmentedItem { Key = "a", Content = "A" });
        sut.Items.Add(new SegmentedItem { Key = "b", Content = "B" });
        sut.Items.Add(new SegmentedItem { Key = "c", Content = "C" });

        Assert.False(sut.Items[0].IsSelected);
        Assert.True(sut.Items[1].IsSelected);
        Assert.True(sut.Items[2].IsSelected);
        Assert.Equal(["b", "c"], sut.SelectedKeys);
    }

    [StaFact]
    public void Single_inserting_a_segment_before_the_selected_one_keeps_the_selected_key()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);
        sut.Items[2].PerformClick();
        var selected = sut.Items[2];
        var events = Record(sut);

        sut.Items.Insert(0, new SegmentedItem { Key = "sun", Content = "Sun" });

        Assert.Equal(["wed"], sut.SelectedKeys);
        Assert.Equal(3, sut.SelectedIndex);
        Assert.Same(selected, sut.SelectedItem);
        Assert.True(selected.IsSelected);
        Assert.False(sut.Items[2].IsSelected);
        Assert.Equal([selected], sut.SelectedItems);
        Assert.Empty(events);
    }

    [StaFact]
    public void Single_removing_a_segment_before_the_selected_one_keeps_the_selected_key()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);
        sut.Items[2].PerformClick();
        var events = Record(sut);

        sut.Items.RemoveAt(0);

        Assert.Equal(["wed"], sut.SelectedKeys);
        Assert.Equal(1, sut.SelectedIndex);
        Assert.True(sut.Items[1].IsSelected);
        Assert.Empty(events);
    }

    [StaFact]
    public void Single_removing_the_selected_segment_clears_the_selection_in_one_event()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);
        sut.Items[2].PerformClick();
        var events = Record(sut);

        sut.Items.RemoveAt(2);

        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(-1, sut.SelectedIndex);
        Assert.Null(sut.SelectedItem);
        Assert.DoesNotContain(sut.Items, x => x.IsSelected);
        var e = Assert.Single(events);
        Assert.Equal(["wed"], e.OldValue);
        Assert.Empty(e.NewValue);
    }

    [StaFact]
    public void Single_changing_the_key_of_the_selected_segment_updates_the_key_in_one_event()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);
        sut.Items[2].PerformClick();
        var events = Record(sut);

        sut.Items[2].Key = "wednesday";

        Assert.Equal(["wednesday"], sut.SelectedKeys);
        Assert.Equal(2, sut.SelectedIndex);
        var e = Assert.Single(events);
        Assert.Equal(["wed"], e.OldValue);
        Assert.Equal(["wednesday"], e.NewValue);
    }

    [StaFact]
    public void Single_selected_index_set_before_the_items_raises_one_event_when_the_item_arrives()
    {
        var sut = new Segmented { SelectedIndex = 1 };
        var events = Record(sut);

        sut.Items.Add(new SegmentedItem { Content = "A" });
        sut.Items.Add(new SegmentedItem { Content = "B" });

        Assert.Equal(["B"], sut.SelectedKeys);
        var e = Assert.Single(events);
        Assert.Empty(e.OldValue);
        Assert.Equal(["B"], e.NewValue);
    }

    [StaFact]
    public void Multiple_removing_a_selected_segment_drops_its_key_in_one_event()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);
        sut.Items[0].PerformClick();
        sut.Items[2].PerformClick();
        sut.Items[4].PerformClick();
        var events = Record(sut);

        sut.Items.RemoveAt(2);

        Assert.Equal(["mon", "fri"], sut.SelectedKeys);
        Assert.Equal([sut.Items[0], sut.Items[3]], sut.SelectedItems);
        var e = Assert.Single(events);
        Assert.Equal(["mon", "wed", "fri"], e.OldValue);
        Assert.Equal(["mon", "fri"], e.NewValue);
    }

    [StaFact]
    public void Multiple_clearing_the_segments_drops_their_keys_but_keeps_keys_without_a_segment()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);
        sut.SelectedKeys.Add("tue");
        sut.SelectedKeys.Add("sat");
        var events = Record(sut);

        sut.Items.Clear();

        Assert.Equal(["sat"], sut.SelectedKeys);
        var e = Assert.Single(events);
        Assert.Equal(["tue", "sat"], e.OldValue);
        Assert.Equal(["sat"], e.NewValue);
    }

    [StaFact]
    public void Multiple_inserting_a_segment_keeps_the_keys_without_an_event()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);
        sut.Items[1].PerformClick();
        var events = Record(sut);

        sut.Items.Insert(0, new SegmentedItem { Key = "sun", Content = "Sun" });

        Assert.Equal(["tue"], sut.SelectedKeys);
        Assert.True(sut.Items[2].IsSelected);
        Assert.Equal(2, sut.SelectedIndex);
        Assert.Empty(events);
    }

    [StaFact]
    public void Switching_from_multiple_to_single_keeps_only_the_most_recent_item()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);
        sut.Items[0].PerformClick();
        sut.Items[3].PerformClick();
        var events = Record(sut);

        sut.SelectionMode = SegmentedSelectionMode.Single;

        Assert.Equal(["thu"], sut.SelectedKeys);
        Assert.False(sut.Items[0].IsSelected);
        Assert.True(sut.Items[3].IsSelected);
        Assert.Equal(3, sut.SelectedIndex);
        Assert.Single(events);
    }

    [StaFact]
    public void Horizontal_corner_radius_rounds_the_left_of_the_first_and_the_right_of_the_last()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);
        sut.CornerRadius = new CornerRadius(6);

        Assert.Equal(new CornerRadius(6, 0, 0, 6), sut.Items[0].Tag);
        Assert.Equal(new CornerRadius(0), sut.Items[2].Tag);
        Assert.Equal(new CornerRadius(0, 6, 6, 0), sut.Items[4].Tag);
    }

    [StaFact]
    public void Vertical_corner_radius_rounds_the_top_of_the_first_and_the_bottom_of_the_last()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);
        sut.CornerRadius = new CornerRadius(6);

        sut.Orientation = Orientation.Vertical;

        Assert.Equal(new CornerRadius(6, 6, 0, 0), sut.Items[0].Tag);
        Assert.Equal(new CornerRadius(0), sut.Items[2].Tag);
        Assert.Equal(new CornerRadius(0, 0, 6, 6), sut.Items[4].Tag);
    }

    [StaFact]
    public void Separate_pills_each_get_the_full_corner_radius_and_the_items_know_it()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);
        sut.CornerRadius = new CornerRadius(8);

        sut.IsJoined = false;

        Assert.All(sut.Items, x => Assert.Equal(new CornerRadius(8), x.Tag));
        Assert.All(sut.Items, x => Assert.False(x.IsJoined));
    }

    [StaFact]
    public void Size_is_passed_to_the_items()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);

        sut.Size = SegmentedSize.Large;

        Assert.All(sut.Items, x => Assert.Equal(SegmentedSize.Large, x.Size));

        sut.Items.Add(new SegmentedItem { Content = "Sat" });

        Assert.Equal(SegmentedSize.Large, sut.Items[^1].Size);
    }

    [StaFact]
    public void Icon_only_uses_the_text_as_automation_name_and_tooltip()
    {
        var sut = new Segmented { SelectionMode = SegmentedSelectionMode.Multiple };
        sut.Items.Add(new SegmentedItem { Key = "b", Content = "Bold", Icon = "B" });
        sut.Items.Add(new SegmentedItem { Key = "i", Content = "Italic", Icon = "I", ToolTip = "Italic (Ctrl+I)" });

        sut.IconOnly = true;

        Assert.True(sut.Items[0].IsIconOnly);
        Assert.Equal("Bold", UIElementAutomationPeer.CreatePeerForElement(sut.Items[0]).GetName());
        Assert.Equal("Bold", sut.Items[0].ToolTip);
        Assert.Equal("Italic (Ctrl+I)", sut.Items[1].ToolTip);

        sut.IconOnly = false;

        Assert.False(sut.Items[0].IsIconOnly);
        Assert.Null(sut.Items[0].ToolTip);
        Assert.Equal("Italic (Ctrl+I)", sut.Items[1].ToolTip);
    }

    [StaFact]
    public void Multiple_items_are_toggle_buttons_in_UI_Automation()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut.Items[2]);

        Assert.Equal(AutomationControlType.Button, peer.GetAutomationControlType());
        Assert.Equal("Wed", peer.GetName());
        Assert.Null(peer.GetPattern(PatternInterface.SelectionItem));
        var toggle = Assert.IsAssignableFrom<IToggleProvider>(peer.GetPattern(PatternInterface.Toggle));
        Assert.Equal(System.Windows.Automation.ToggleState.Off, toggle.ToggleState);

        toggle.Toggle();

        Assert.Equal(System.Windows.Automation.ToggleState.On, toggle.ToggleState);
        Assert.Equal(["wed"], sut.SelectedKeys);
        Assert.Null(UIElementAutomationPeer.CreatePeerForElement(sut).GetPattern(PatternInterface.Selection));
    }

    [StaFact]
    public void Single_items_are_radio_buttons_and_the_control_has_a_selection_pattern()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut.Items[1]);

        Assert.Equal(AutomationControlType.RadioButton, peer.GetAutomationControlType());
        Assert.Null(peer.GetPattern(PatternInterface.Toggle));
        var item = Assert.IsAssignableFrom<ISelectionItemProvider>(peer.GetPattern(PatternInterface.SelectionItem));

        item.Select();

        Assert.True(item.IsSelected);
        Assert.Equal(1, sut.SelectedIndex);

        var controlPeer = UIElementAutomationPeer.CreatePeerForElement(sut);
        Assert.Equal(AutomationControlType.List, controlPeer.GetAutomationControlType());
        var selection = Assert.IsAssignableFrom<ISelectionProvider>(controlPeer.GetPattern(PatternInterface.Selection));
        Assert.False(selection.CanSelectMultiple);
        Assert.True(selection.IsSelectionRequired);
        Assert.Single(selection.GetSelection());
    }

    [StaFact]
    public void Clearing_and_re_adding_the_same_items_hooks_each_click_once()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);
        var items = sut.Items.ToList();

        sut.Items.Clear();

        Assert.All(items, x => Assert.Null(x.Owner));

        foreach (var item in items)
        {
            sut.Items.Add(item);
        }

        var events = Record(sut);

        items[1].PerformClick();

        Assert.Equal(["tue"], sut.SelectedKeys);
        Assert.True(items[1].IsSelected);
        Assert.Single(events);
        Assert.Same(sut, items[1].Owner);
    }

    [StaFact]
    public void A_single_mode_has_one_tab_stop_on_the_selected_segment()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);

        Assert.Equal([true, false, false, false, false], sut.Items.Select(x => x.IsTabStop).ToList());

        sut.SelectedIndex = 3;

        Assert.Equal([false, false, false, true, false], sut.Items.Select(x => x.IsTabStop).ToList());

        KeyPress.Send(sut, Key.Left);

        Assert.Equal(2, sut.SelectedIndex);
        Assert.Equal([false, false, true, false, false], sut.Items.Select(x => x.IsTabStop).ToList());
    }

    [StaFact]
    public void Multiple_mode_has_one_tab_stop_on_the_first_selected_segment()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Multiple);

        Assert.Equal(1, sut.Items.Count(x => x.IsTabStop));
        Assert.True(sut.Items[0].IsTabStop);

        sut.Items[3].PerformClick();

        Assert.Equal(1, sut.Items.Count(x => x.IsTabStop));
        Assert.True(sut.Items[3].IsTabStop);
    }

    [StaFact]
    public void A_disabled_selected_segment_hands_the_tab_stop_to_the_first_enabled_one()
    {
        var sut = CreateWeekdays(SegmentedSelectionMode.Single);
        sut.SelectedIndex = 2;

        sut.Items[2].IsEnabled = false;

        Assert.True(sut.Items[0].IsTabStop);
        Assert.Equal(1, sut.Items.Count(x => x.IsTabStop));
    }

    private static Segmented CreateWeekdays(SegmentedSelectionMode mode)
    {
        var sut = new Segmented { SelectionMode = mode };
        sut.Items.Add(new SegmentedItem { Key = "mon", Content = "Mon" });
        sut.Items.Add(new SegmentedItem { Key = "tue", Content = "Tue" });
        sut.Items.Add(new SegmentedItem { Key = "wed", Content = "Wed" });
        sut.Items.Add(new SegmentedItem { Key = "thu", Content = "Thu" });
        sut.Items.Add(new SegmentedItem { Key = "fri", Content = "Fri" });
        return sut;
    }

    private static List<ValueChangedEventArgs<IReadOnlyList<string>>> Record(
        Segmented sut)
    {
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);
        return events;
    }

    private static List<SegmentedSelectionChangedEventArgs> RecordSelection(
        Segmented sut)
    {
        var events = new List<SegmentedSelectionChangedEventArgs>();
        sut.SelectionChanged += (_, e) => events.Add(e);
        return events;
    }
}