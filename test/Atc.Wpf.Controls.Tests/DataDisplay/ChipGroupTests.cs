namespace Atc.Wpf.Controls.Tests.DataDisplay;

public sealed class ChipGroupTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_MatchTheDocumentedApi()
    {
        var sut = new ChipGroup();

        Assert.NotNull(sut.Items);
        Assert.Empty(sut.Items);
        Assert.NotNull(sut.SelectedKeys);
        Assert.Empty(sut.SelectedKeys);
        Assert.NotNull(sut.DisabledKeys);
        Assert.Empty(sut.DisabledKeys);
        Assert.Equal(ChipSelectionMode.Multiple, sut.SelectionMode);
        Assert.False(sut.SelectNewItems);
        Assert.Equal(ChipGroupVariant.Filled, sut.ChipVariant);
        Assert.Equal(ChipSize.Medium, sut.ChipSize);
        Assert.True(sut.ShowCheckMark);
        Assert.Equal(Orientation.Horizontal, sut.Orientation);
        Assert.True(sut.Wrap);
        Assert.Equal(4d, sut.ItemSpacing);
        Assert.False(sut.ShowAllChip);
        Assert.Null(sut.MaxSelectionCount);
        Assert.Null(sut.ItemIcons);
        Assert.Null(sut.ItemCounts);
    }

    [StaFact]
    public void Each_instance_gets_its_own_collections()
    {
        var first = new ChipGroup();
        var second = new ChipGroup();

        Assert.NotSame(first.SelectedKeys, second.SelectedKeys);
        Assert.NotSame(first.DisabledKeys, second.DisabledKeys);
        Assert.NotSame(first.Items, second.Items);
    }

    [StaFact]
    public void One_chip_per_item_in_item_order_with_the_display_value_as_name()
    {
        var sut = Create();

        Assert.Equal(["Error", "Warning", "Info", "Debug"], sut.Chips.Select(x => System.Windows.Automation.AutomationProperties.GetName(x)), StringComparer.Ordinal);
        Assert.Equal("Warning", UIElementAutomationPeer.CreatePeerForElement(sut.GetChip("warn")!).GetName());
    }

    [StaFact]
    public void Multiple_toggle_ticks_and_unticks_with_one_event_each()
    {
        var sut = Create();
        var events = Record(sut);

        Toggle(sut, "warn");
        Toggle(sut, "error");

        Assert.Equal(["warn", "error"], sut.SelectedKeys);
        Assert.True(sut.GetChip("warn")!.IsSelected);
        Assert.True(sut.GetChip("error")!.IsSelected);
        Assert.Equal(2, events.Count);

        Toggle(sut, "warn");

        Assert.Equal(["error"], sut.SelectedKeys);
        Assert.False(sut.GetChip("warn")!.IsSelected);
        Assert.Equal(3, events.Count);
        Assert.Equal(["warn", "error"], events[2].OldValue);
        Assert.Equal(["error"], events[2].NewValue);
    }

    [StaFact]
    public void Multiple_chips_are_check_boxes_and_the_group_is_a_list_in_UI_Automation()
    {
        var sut = Create();

        Assert.Equal(AutomationControlType.List, UIElementAutomationPeer.CreatePeerForElement(sut).GetAutomationControlType());
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut.GetChip("info")!);
        Assert.Equal(AutomationControlType.CheckBox, peer.GetAutomationControlType());
        Assert.NotNull(peer.GetPattern(PatternInterface.Toggle));
    }

    [StaFact]
    public void Single_selects_one_key_and_switching_raises_one_event()
    {
        var sut = Create(ChipSelectionMode.Single);
        var events = Record(sut);

        Select(sut, "info");
        Select(sut, "error");

        Assert.Equal(["error"], sut.SelectedKeys);
        Assert.False(sut.GetChip("info")!.IsSelected);
        Assert.True(sut.GetChip("error")!.IsSelected);
        Assert.Equal(2, events.Count);
        Assert.Equal(["info"], events[1].OldValue);
        Assert.Equal(["error"], events[1].NewValue);
    }

    [StaFact]
    public void Single_clicking_the_selected_chip_keeps_it_selected()
    {
        var sut = Create(ChipSelectionMode.Single);
        Select(sut, "info");
        var events = Record(sut);

        sut.GetChip("info")!.PerformClick();

        Assert.Equal(["info"], sut.SelectedKeys);
        Assert.Empty(events);
    }

    [StaFact]
    public void Single_chips_are_radio_buttons_and_the_group_has_a_selection_pattern()
    {
        var sut = Create(ChipSelectionMode.Single);
        Select(sut, "warn");

        var chipPeer = UIElementAutomationPeer.CreatePeerForElement(sut.GetChip("warn")!);
        Assert.Equal(AutomationControlType.RadioButton, chipPeer.GetAutomationControlType());
        Assert.Null(chipPeer.GetPattern(PatternInterface.Toggle));
        Assert.True(((ISelectionItemProvider)chipPeer.GetPattern(PatternInterface.SelectionItem)).IsSelected);

        var groupPeer = UIElementAutomationPeer.CreatePeerForElement(sut);
        var selection = Assert.IsAssignableFrom<ISelectionProvider>(groupPeer.GetPattern(PatternInterface.Selection));
        Assert.False(selection.CanSelectMultiple);
        Assert.True(selection.IsSelectionRequired);
        Assert.Single(selection.GetSelection());
    }

    [StaFact]
    public void SingleOrNone_clicking_the_selected_chip_clears_it()
    {
        var sut = Create(ChipSelectionMode.SingleOrNone);
        Select(sut, "debug");
        var events = Record(sut);

        sut.GetChip("debug")!.PerformClick();

        Assert.Empty(sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Single_arrow_keys_move_and_select()
    {
        var sut = Create(ChipSelectionMode.Single);
        Select(sut, "warn");
        var events = Record(sut);

        KeyPress.Send(sut.GetChip("warn")!, Key.Right);

        Assert.Equal(["info"], sut.SelectedKeys);

        KeyPress.Send(sut.GetChip("info")!, Key.Left);
        KeyPress.Send(sut.GetChip("warn")!, Key.Up);

        Assert.Equal(["error"], sut.SelectedKeys);
        Assert.Equal(3, events.Count);
    }

    [StaFact]
    public void Single_arrow_keys_skip_disabled_chips()
    {
        var sut = Create(ChipSelectionMode.Single);
        sut.DisabledKeys.Add("info");
        Select(sut, "warn");

        KeyPress.Send(sut.GetChip("warn")!, Key.Down);

        Assert.Equal(["debug"], sut.SelectedKeys);
    }

    [StaFact]
    public void Multiple_arrow_keys_do_not_change_the_selection_and_space_toggles()
    {
        var sut = Create();
        var events = Record(sut);

        KeyPress.Send(sut.GetChip("warn")!, Key.Right);

        Assert.Empty(sut.SelectedKeys);

        KeyPress.Send(sut.GetChip("warn")!, Key.Space);

        Assert.Equal(["warn"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void All_chip_selects_all_and_clears_with_one_event_each()
    {
        var sut = Create();
        sut.ShowAllChip = true;
        var events = Record(sut);

        Assert.Equal(Visibility.Visible, sut.AllChip.Visibility);
        Assert.False(sut.AllChip.IsSelected);
        Assert.Equal(Atc.Wpf.Resources.Word.All, System.Windows.Automation.AutomationProperties.GetName(sut.AllChip));

        Toggle(sut, "info");
        sut.AllChip.PerformClick();

        Assert.Equal(["info", "error", "warn", "debug"], sut.SelectedKeys);
        Assert.True(sut.AllChip.IsSelected);
        Assert.Equal(2, events.Count);

        sut.AllChip.PerformClick();

        Assert.Empty(sut.SelectedKeys);
        Assert.False(sut.AllChip.IsSelected);
        Assert.Equal(3, events.Count);
    }

    [StaFact]
    public void All_chip_is_selected_when_every_key_is_ticked_by_hand()
    {
        var sut = Create();
        sut.ShowAllChip = true;

        foreach (var key in new[] { "error", "warn", "info", "debug" })
        {
            Toggle(sut, key);
        }

        Assert.True(sut.AllChip.IsSelected);
    }

    [StaFact]
    public void All_chip_is_hidden_unless_shown_and_in_Multiple_mode()
    {
        var sut = Create();

        Assert.Equal(Visibility.Collapsed, sut.AllChip.Visibility);

        sut.ShowAllChip = true;
        sut.SelectionMode = ChipSelectionMode.Single;

        Assert.Equal(Visibility.Collapsed, sut.AllChip.Visibility);
    }

    [StaFact]
    public void Limit_disables_the_unticked_chips_at_the_limit()
    {
        var sut = Create();
        sut.MaxSelectionCount = 2;
        sut.ShowAllChip = true;

        Toggle(sut, "error");
        Toggle(sut, "warn");

        Assert.True(sut.GetChip("error")!.IsEnabled);
        Assert.False(sut.GetChip("info")!.IsEnabled);
        Assert.False(sut.GetChip("debug")!.IsEnabled);
        Assert.False(sut.AllChip.IsEnabled);

        sut.GetChip("info")!.PerformClick();

        Assert.Equal(["error", "warn"], sut.SelectedKeys);

        Toggle(sut, "warn");

        Assert.True(sut.GetChip("info")!.IsEnabled);
    }

    [StaFact]
    public void Disabled_keys_cannot_be_toggled_and_All_skips_them()
    {
        var sut = Create();
        sut.ShowAllChip = true;
        sut.DisabledKeys.Add("debug");
        var events = Record(sut);

        Assert.False(sut.GetChip("debug")!.IsEnabled);

        sut.GetChip("debug")!.PerformClick();

        Assert.Empty(sut.SelectedKeys);
        Assert.Empty(events);

        sut.AllChip.PerformClick();

        Assert.Equal(["error", "warn", "info"], sut.SelectedKeys);
        Assert.True(sut.AllChip.IsSelected);
    }

    [StaFact]
    public void Counts_are_shown_next_to_the_display_value()
    {
        var sut = Create();

        Assert.Null(sut.GetCountText("error"));

        sut.ItemCounts = new Dictionary<string, int>(StringComparer.Ordinal) { ["error"] = 12, ["warn"] = 3 };

        Assert.Equal("12", sut.GetCountText("error"));
        Assert.Equal("3", sut.GetCountText("warn"));
        Assert.Null(sut.GetCountText("info"));
    }

    [StaFact]
    public void Icons_are_set_on_the_chips()
    {
        var sut = Create();
        var icon = new TextBlock { Text = "!" };

        sut.ItemIcons = new Dictionary<string, object>(StringComparer.Ordinal) { ["error"] = icon };

        Assert.Same(icon, sut.GetChip("error")!.Icon);
        Assert.Null(sut.GetChip("warn")!.Icon);

        sut.Items = new Dictionary<string, string>(sut.Items, StringComparer.Ordinal) { ["trace"] = "Trace" };

        Assert.Same(icon, sut.GetChip("error")!.Icon);
    }

    [StaFact]
    public void A_new_SelectedKeys_instance_is_picked_up_without_an_event()
    {
        var sut = Create();
        var events = Record(sut);

        sut.SelectedKeys = ["info", "debug"];

        Assert.True(sut.GetChip("info")!.IsSelected);
        Assert.True(sut.GetChip("debug")!.IsSelected);
        Assert.Empty(events);

        sut.SelectedKeys.Remove("info");

        Assert.False(sut.GetChip("info")!.IsSelected);

        Toggle(sut, "error");

        Assert.Equal(["debug", "error"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void SelectedKeys_is_updated_in_place()
    {
        var sut = Create();
        var keys = sut.SelectedKeys;

        Toggle(sut, "warn");

        Assert.Same(keys, sut.SelectedKeys);
        Assert.Equal(["warn"], keys);
    }

    [StaFact]
    public void Items_change_drops_stale_keys_in_one_event()
    {
        var sut = Create();
        sut.SelectedKeys = ["error", "warn", "info"];
        var events = Record(sut);

        sut.Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["error"] = "Error", ["trace"] = "Trace" };

        Assert.Equal(["error"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.Equal(["Error", "Trace"], sut.Chips.Select(x => System.Windows.Automation.AutomationProperties.GetName(x)), StringComparer.Ordinal);
        Assert.Null(sut.GetChip("warn"));
    }

    [StaFact]
    public void SelectNewItems_ticks_new_keys()
    {
        var sut = new ChipGroup { SelectNewItems = true };

        sut.Items = Items();

        Assert.Equal(["error", "warn", "info", "debug"], sut.SelectedKeys);

        sut.SelectedKeys.Remove("debug");
        sut.Items = new Dictionary<string, string>(sut.Items, StringComparer.Ordinal) { ["trace"] = "Trace" };

        Assert.Equal(["error", "warn", "info", "trace"], sut.SelectedKeys);
    }

    [StaFact]
    public void Layout_follows_Wrap_Orientation_and_ItemSpacing()
    {
        var sut = Create();

        Assert.Equal(VisualWrappingType.Wrap, sut.ItemsPanel.ChildWrapping);
        Assert.Equal(Orientation.Horizontal, sut.ItemsPanel.Orientation);
        Assert.Equal(4d, sut.ItemsPanel.Spacing);
        Assert.Equal(ScrollBarVisibility.Disabled, sut.ScrollHost.HorizontalScrollBarVisibility);

        sut.Wrap = false;
        sut.ItemSpacing = 8;

        Assert.Equal(VisualWrappingType.NoWrap, sut.ItemsPanel.ChildWrapping);
        Assert.Equal(8d, sut.ItemsPanel.Spacing);
        Assert.Equal(ScrollBarVisibility.Auto, sut.ScrollHost.HorizontalScrollBarVisibility);
        Assert.Equal(ScrollBarVisibility.Disabled, sut.ScrollHost.VerticalScrollBarVisibility);

        sut.Orientation = Orientation.Vertical;

        Assert.Equal(Orientation.Vertical, sut.ItemsPanel.Orientation);
        Assert.Equal(ScrollBarVisibility.Disabled, sut.ScrollHost.HorizontalScrollBarVisibility);
        Assert.Equal(ScrollBarVisibility.Auto, sut.ScrollHost.VerticalScrollBarVisibility);
    }

    [StaFact]
    public void Chip_options_are_applied_to_every_chip()
    {
        var sut = Create();

        sut.ChipSize = ChipSize.Small;
        sut.ShowCheckMark = false;
        sut.ChipVariant = ChipGroupVariant.Outlined;

        foreach (var chip in sut.Chips)
        {
            Assert.Equal(ChipSize.Small, chip.Size);
            Assert.False(chip.ShowCheckMark);
            Assert.NotEqual(DependencyProperty.UnsetValue, chip.ReadLocalValue(Chip.SelectedBorderBrushProperty));
        }

        sut.ChipVariant = ChipGroupVariant.Filled;

        Assert.Equal(DependencyProperty.UnsetValue, sut.Chips[0].ReadLocalValue(Chip.SelectedBorderBrushProperty));
    }

    [StaFact]
    public void SelectAll_and_ClearSelection_raise_one_event_each()
    {
        var sut = Create();
        var events = Record(sut);

        sut.SelectAll();

        Assert.Equal(4, sut.SelectedKeys.Count);

        sut.ClearSelection();

        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(2, events.Count);
    }

    private static ChipGroup Create(
        ChipSelectionMode mode = ChipSelectionMode.Multiple)
        => new()
        {
            SelectionMode = mode,
            Items = Items(),
        };

    private static Dictionary<string, string> Items()
        => new(StringComparer.Ordinal)
        {
            ["error"] = "Error",
            ["warn"] = "Warning",
            ["info"] = "Info",
            ["debug"] = "Debug",
        };

    private static void Toggle(
        ChipGroup sut,
        string key)
        => ((IToggleProvider)UIElementAutomationPeer.CreatePeerForElement(sut.GetChip(key)!).GetPattern(PatternInterface.Toggle)).Toggle();

    private static void Select(
        ChipGroup sut,
        string key)
        => ((ISelectionItemProvider)UIElementAutomationPeer.CreatePeerForElement(sut.GetChip(key)!).GetPattern(PatternInterface.SelectionItem)).Select();

    private static List<ValueChangedEventArgs<IReadOnlyList<string>>> Record(
        ChipGroup sut)
    {
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);
        return events;
    }
}