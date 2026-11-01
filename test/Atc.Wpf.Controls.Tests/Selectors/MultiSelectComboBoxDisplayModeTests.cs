namespace Atc.Wpf.Controls.Tests.Selectors;

[Collection(nameof(PackResourceTestGroup))]
public sealed class MultiSelectComboBoxDisplayModeTests : IDisposable
{
    private const double ChipWidth = 50;

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_MatchTheDocumentedApi()
    {
        var sut = new MultiSelectComboBox();

        Assert.Equal(MultiSelectDisplayMode.Text, sut.DisplayMode);
        Assert.Null(sut.MaxVisibleChips);
        Assert.False(sut.ChipsWrap);
        Assert.Null(sut.ChipTemplate);
        Assert.Equal(MultiSelectCommitMode.Immediate, sut.CommitMode);
        Assert.Null(sut.MaxSelectionCount);
        Assert.Null(sut.ItemGroups);
        Assert.True(sut.IsGroupCheckable);
    }

    [StaFact]
    public void Count_mode_shows_the_count_even_for_one_item()
    {
        var sut = Create(5);
        sut.DisplayMode = MultiSelectDisplayMode.Count;
        sut.NoneText = "None";

        Assert.Equal("None", sut.SummaryText);

        sut.SelectedKeys.Add("k2");
        Assert.Equal("1 of 5 selected", sut.SummaryText);

        sut.CountFormat = "{0}/{1}";
        sut.SelectedKeys.Add("k3");
        Assert.Equal("2/5", sut.SummaryText);

        sut.AllText = "Everything";
        sut.SelectAll();
        Assert.Equal("Everything", sut.SummaryText);
    }

    [StaFact]
    public void Chips_mode_summary_lists_every_ticked_name()
    {
        var sut = Create(5);
        sut.DisplayMode = MultiSelectDisplayMode.Chips;

        sut.SelectedKeys = ["k4", "k1", "k3"];

        Assert.Equal("Item 1, Item 3, Item 4", sut.SummaryText);
        Assert.Same(sut.ChipPanel, sut.PartSummaryHost.Content);
        Assert.True(sut.PartSummaryHost.IsHitTestVisible);

        sut.DisplayMode = MultiSelectDisplayMode.Text;

        Assert.False(sut.PartSummaryHost.IsHitTestVisible);
        Assert.NotSame(sut.ChipPanel, sut.PartSummaryHost.Content);
    }

    [StaFact]
    public void Removing_a_chip_unticks_the_key_with_one_event_without_opening()
    {
        var sut = Create(4);
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.SelectedKeys = ["k1", "k2", "k3"];
        var keys = sut.SelectedKeys;
        var events = Record(sut);

        var chip = sut.GetChipItems().Single(x => x.Key == "k2");
        chip.RemoveCommand.Execute(parameter: null);

        Assert.Same(keys, sut.SelectedKeys);
        Assert.Equal(["k1", "k3"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Equal(["k1", "k2", "k3"], change.OldValue);
        Assert.Equal(["k1", "k3"], change.NewValue);
        Assert.False(sut.IsDropDownOpen);
        Assert.Equal(["k1", "k3"], sut.GetChipItems().Select(x => x.Key).ToList());
    }

    [StaFact]
    public void The_default_chip_remove_button_unticks_the_key()
    {
        var sut = CreateStyled(4);
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.SelectedKeys = ["k1", "k2"];
        Layout(sut, 400);
        var events = Record(sut);

        var chip = sut.ChipPanel.Children.OfType<Chip>().First();
        var removeButton = (Button)chip.Template.FindName("PART_RemoveButton", chip);
        Assert.Equal("Remove Item 1", System.Windows.Automation.AutomationProperties.GetName(removeButton));

        removeButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        Assert.Equal(["k2"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.False(sut.IsDropDownOpen);
    }

    [StaFact]
    public void A_disabled_key_gets_a_chip_that_cannot_be_removed()
    {
        var sut = Create(3);
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.DisabledKeys.Add("k2");
        sut.SelectedKeys = ["k1", "k2"];
        var events = Record(sut);

        var chip = sut.GetChipItems().Single(x => x.Key == "k2");
        Assert.False(chip.CanRemove);
        Assert.False(chip.RemoveCommand.CanExecute(parameter: null));

        sut.RemoveKey("k2");

        Assert.Equal(["k1", "k2"], sut.SelectedKeys);
        Assert.Empty(events);
    }

    [StaFact]
    public void Backspace_on_the_closed_box_removes_the_last_chip()
    {
        var sut = Create(4);
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.SelectedKeys = ["k3", "k1"];
        var events = Record(sut);

        var handled = sut.HandleKey(Key.Back, Key.None, ModifierKeys.None);

        Assert.True(handled);
        Assert.Equal(["k1"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.False(sut.IsDropDownOpen);
    }

    [StaFact]
    public void Backspace_skips_a_disabled_last_chip()
    {
        var sut = Create(4);
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.DisabledKeys.Add("k4");
        sut.SelectedKeys = ["k1", "k2", "k4"];

        Assert.True(sut.HandleKey(Key.Back, Key.None, ModifierKeys.None));

        Assert.Equal(["k1", "k4"], sut.SelectedKeys);
    }

    [StaTheory]
    [InlineData(MultiSelectDisplayMode.Chips, true)]
    [InlineData(MultiSelectDisplayMode.Text, false)]
    [InlineData(MultiSelectDisplayMode.Count, false)]
    public void Backspace_is_not_used_while_open_or_outside_chips_mode(
        MultiSelectDisplayMode mode,
        bool isOpen)
    {
        var sut = Create(4);
        sut.DisplayMode = mode;
        sut.SelectedKeys = ["k1", "k2"];
        sut.IsDropDownOpen = isOpen;

        var handled = sut.HandleKey(Key.Back, Key.None, ModifierKeys.None);

        Assert.False(handled);
        Assert.Equal(["k1", "k2"], sut.SelectedKeys);
        sut.IsDropDownOpen = false;
    }

    [StaFact]
    public void Backspace_with_nothing_ticked_is_not_used()
    {
        var sut = Create(4);
        sut.DisplayMode = MultiSelectDisplayMode.Chips;

        Assert.False(sut.HandleKey(Key.Back, Key.None, ModifierKeys.None));
    }

    [StaFact]
    public void Responsive_chips_show_as_many_as_fit_then_the_overflow()
    {
        var sut = CreateFixedWidthChips(6);
        sut.SelectAll();

        // 230 wide: content area 230 - 2 (border) - 22 (arrow) - 2 (margin) = 204.
        // 3 chips (150) + "+3" fit; 4 chips (200) + "+2" do not.
        Layout(sut, 230);

        Assert.Equal(3, sut.ChipPanel.VisibleCount);
        Assert.Equal(3, sut.ChipPanel.HiddenCount);
        Assert.Equal("+3", sut.OverflowText);
        Assert.Equal("Item 4, Item 5, Item 6", System.Windows.Automation.AutomationProperties.GetName(sut.OverflowButton));
        Assert.True(LayoutInformation.GetLayoutSlot(sut.OverflowButton).Width > 0);

        Layout(sut, 1000);

        Assert.Equal(6, sut.ChipPanel.VisibleCount);
        Assert.Equal(0, sut.ChipPanel.HiddenCount);
        Assert.Equal(0, LayoutInformation.GetLayoutSlot(sut.OverflowButton).Width);
    }

    [StaFact]
    public void Fixed_MaxVisibleChips_shows_at_most_that_many()
    {
        var sut = CreateFixedWidthChips(6);
        sut.MaxVisibleChips = 2;
        sut.SelectAll();

        Layout(sut, 1000);

        Assert.Equal(2, sut.ChipPanel.VisibleCount);
        Assert.Equal(4, sut.ChipPanel.HiddenCount);
        Assert.Equal("+4", sut.OverflowText);
        Assert.Equal("Item 3, Item 4, Item 5, Item 6", System.Windows.Automation.AutomationProperties.GetName(sut.OverflowButton));
    }

    [StaFact]
    public void ChipsWrap_shows_every_chip_and_grows_in_height()
    {
        var sut = CreateFixedWidthChips(6);
        sut.SelectAll();
        Layout(sut, 230);
        var oneLineHeight = sut.DesiredSize.Height;

        sut.ChipsWrap = true;
        Layout(sut, 230);

        Assert.Equal(6, sut.ChipPanel.VisibleCount);
        Assert.Equal(0, sut.ChipPanel.HiddenCount);
        Assert.True(sut.DesiredSize.Height > oneLineHeight);
    }

    [StaFact]
    public void One_line_chips_keep_the_text_mode_height()
    {
        var sut = CreateStyled(5);
        sut.SelectedKeys = ["k1", "k2", "k3"];
        Layout(sut, 400);
        var textHeight = sut.DesiredSize.Height;

        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        Layout(sut, 400);

        Assert.True(sut.ChipPanel.VisibleCount > 0);
        Assert.True(sut.ChipPanel.Children.OfType<Chip>().First().ActualHeight > 0);
        Assert.Equal(textHeight, sut.DesiredSize.Height);
    }

    [StaFact]
    public void Clicking_the_overflow_opens_the_pop_up()
    {
        var sut = CreateFixedWidthChips(6);
        sut.MaxVisibleChips = 1;
        sut.SelectAll();
        Layout(sut, 1000);

        sut.OverflowButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        Assert.True(sut.IsDropDownOpen);
        sut.IsDropDownOpen = false;
    }

    [StaFact]
    public void Automation_exposes_the_chip_remove_buttons_and_the_overflow_while_closed()
    {
        var sut = CreateStyled(6);
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.MaxVisibleChips = 2;
        sut.SelectedKeys = ["k1", "k2", "k3", "k4"];
        Layout(sut, 1000);
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);
        peer.ResetChildrenCache();

        var buttons = (peer.GetChildren() ?? [])
            .Where(x => x.GetAutomationControlType() == AutomationControlType.Button)
            .Select(x => x.GetName())
            .ToList();

        Assert.Equal(["Remove Item 1", "Remove Item 2", "Item 3, Item 4"], buttons);
    }

    [StaFact]
    public void Limit_disables_the_remaining_items_and_blocks_select_all_when_it_does_not_fit()
    {
        var sut = Create(4);
        sut.MaxSelectionCount = 2;
        var events = Record(sut);

        sut.SelectAll();

        Assert.Empty(sut.SelectedKeys);
        Assert.Empty(events);
        Assert.False(sut.PartCheckList.PartSelectAll.IsEnabled);

        sut.GetItem("k1")!.IsChecked = true;
        sut.GetItem("k2")!.IsChecked = true;

        Assert.Equal(["k1", "k2"], sut.SelectedKeys);
        Assert.False(sut.GetItem("k3")!.IsEnabled);
        Assert.False(sut.GetItem("k4")!.IsEnabled);
        Assert.True(sut.GetItem("k1")!.IsEnabled);
        Assert.Equal("2 of max 2", sut.PartCheckList.PartCount.Text);

        sut.GetItem("k3")!.IsChecked = true;
        Assert.Equal(["k1", "k2"], sut.SelectedKeys);
    }

    [StaFact]
    public void SelectedKeys_beyond_the_limit_from_code_are_kept()
    {
        var sut = Create(4);
        sut.MaxSelectionCount = 2;

        sut.SelectedKeys = ["k1", "k2", "k3"];

        Assert.Equal(["k1", "k2", "k3"], sut.SelectedKeys);
    }

    [StaFact]
    public void Deferred_ticks_are_a_draft_and_OK_commits_them_in_one_event()
    {
        var sut = Create(4);
        sut.CommitMode = MultiSelectCommitMode.Deferred;
        sut.SelectedKeys.Add("k1");
        var keys = sut.SelectedKeys;
        var events = Record(sut);

        sut.IsDropDownOpen = true;
        Assert.Equal(Visibility.Visible, sut.PartCommitFooter.Visibility);
        sut.GetItem("k2")!.IsChecked = true;
        sut.GetItem("k3")!.IsChecked = true;

        Assert.Empty(events);
        Assert.Equal(["k1"], keys);
        Assert.Equal("Item 1", sut.SummaryText);

        sut.PartOkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        Assert.False(sut.IsDropDownOpen);
        Assert.Same(keys, sut.SelectedKeys);
        Assert.Same(keys, sut.PartCheckList.SelectedKeys);
        Assert.Equal(["k1", "k2", "k3"], keys);
        var change = Assert.Single(events);
        Assert.Equal(["k1"], change.OldValue);
        Assert.Equal(["k1", "k2", "k3"], change.NewValue);
        Assert.Equal("3 of 4 selected", sut.SummaryText);
    }

    [StaTheory]
    [InlineData("Cancel")]
    [InlineData("Escape")]
    [InlineData("Close")]
    public void Deferred_cancel_escape_and_closing_discard_the_draft(
        string howToClose)
    {
        var sut = Create(4);
        sut.CommitMode = MultiSelectCommitMode.Deferred;
        sut.SelectedKeys.Add("k1");
        var keys = sut.SelectedKeys;
        var events = Record(sut);

        sut.IsDropDownOpen = true;
        sut.GetItem("k1")!.IsChecked = false;
        sut.GetItem("k2")!.IsChecked = true;

        switch (howToClose)
        {
            case "Cancel":
                sut.PartCancelButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                break;
            case "Escape":
                Assert.True(sut.HandleKey(Key.Escape, Key.None, ModifierKeys.None));
                break;
            default:
                sut.IsDropDownOpen = false;
                break;
        }

        Assert.False(sut.IsDropDownOpen);
        Assert.Empty(events);
        Assert.Same(keys, sut.SelectedKeys);
        Assert.Same(keys, sut.PartCheckList.SelectedKeys);
        Assert.Equal(["k1"], keys);
        Assert.True(sut.GetItem("k1")!.IsChecked);
        Assert.False(sut.GetItem("k2")!.IsChecked);

        // The next opening starts from the committed selection.
        sut.IsDropDownOpen = true;
        Assert.True(sut.GetItem("k1")!.IsChecked);
        Assert.False(sut.GetItem("k2")!.IsChecked);
        sut.IsDropDownOpen = false;
    }

    [StaFact]
    public void Deferred_enter_commits_the_draft()
    {
        var sut = Create(4);
        sut.CommitMode = MultiSelectCommitMode.Deferred;
        var events = Record(sut);

        sut.IsDropDownOpen = true;
        sut.GetItem("k2")!.IsChecked = true;
        Assert.True(sut.HandleKey(Key.Enter, Key.None, ModifierKeys.None));

        Assert.False(sut.IsDropDownOpen);
        Assert.Equal(["k2"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Deferred_OK_without_changes_raises_no_event()
    {
        var sut = Create(4);
        sut.CommitMode = MultiSelectCommitMode.Deferred;
        sut.SelectedKeys.Add("k1");
        var events = Record(sut);

        sut.IsDropDownOpen = true;
        sut.PartOkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        Assert.Empty(events);
        Assert.Equal(["k1"], sut.SelectedKeys);
    }

    [StaFact]
    public void Immediate_mode_has_no_footer()
    {
        var sut = Create(4);

        sut.IsDropDownOpen = true;

        Assert.Equal(Visibility.Collapsed, sut.PartCommitFooter.Visibility);
        sut.IsDropDownOpen = false;
    }

    [StaFact]
    public void Deferred_items_change_while_closed_still_drops_stale_keys()
    {
        var sut = Create(4);
        sut.CommitMode = MultiSelectCommitMode.Deferred;
        sut.SelectedKeys = ["k1", "k4"];
        var events = Record(sut);

        sut.Items = Items(3);

        Assert.Equal(["k1"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Deferred_items_change_while_open_drops_stale_keys_from_the_selection_too()
    {
        var sut = Create(4);
        sut.CommitMode = MultiSelectCommitMode.Deferred;
        sut.SelectedKeys = ["k1", "k4"];
        var keys = sut.SelectedKeys;
        var events = Record(sut);

        sut.IsDropDownOpen = true;
        sut.Items = Items(3);

        Assert.Equal(["k1"], keys);
        Assert.Single(events);

        sut.PartCancelButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        Assert.Same(keys, sut.SelectedKeys);
        Assert.Equal(["k1"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Group_header_ticks_and_unticks_its_items_with_three_states()
    {
        var sut = CreateGrouped();
        var events = Record(sut);
        var header = GroupHeader(sut, "Fruit");

        Assert.Equal(false, header.IsChecked);

        sut.GetItem("apple")!.IsChecked = true;
        Assert.Null(header.IsChecked);

        header.IsChecked = true;

        Assert.Equal(true, header.IsChecked);
        Assert.Equal(["apple", "pear"], sut.SelectedKeys);
        Assert.Equal(2, events.Count);

        header.IsChecked = false;

        Assert.Equal(false, header.IsChecked);
        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(3, events.Count);
    }

    [StaFact]
    public void Filter_hides_groups_without_matches()
    {
        var sut = CreateGrouped();

        sut.FilterText = "carr";
        sut.PartCheckList.FlushPendingFilter();

        var rows = sut.PartCheckList.VisibleRows;
        Assert.DoesNotContain(rows, x => x.IsGroupHeader && x.Key == "Fruit");
        Assert.Contains(rows, x => x.IsGroupHeader && x.Key == "Vegetables");
        Assert.Contains(rows, x => x.Key == "carrot");
    }

    private static MultiSelectComboBox Create(int itemCount)
        => new() { Items = Items(itemCount) };

    private static MultiSelectComboBox CreateStyled(int itemCount)
    {
        var resources = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls;component/DataDisplay/Chip.xaml"),
        };

        var sut = Create(itemCount);
        sut.Resources.MergedDictionaries.Add(resources);
        sut.Resources[typeof(Chip)] = resources["AtcApps.Styles.Chip"];
        return sut;
    }

    private static MultiSelectComboBox CreateFixedWidthChips(int itemCount)
    {
        var border = new FrameworkElementFactory(typeof(Border));
        border.SetValue(FrameworkElement.WidthProperty, ChipWidth);
        border.SetValue(FrameworkElement.HeightProperty, 18d);

        var sut = CreateStyled(itemCount);
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.ChipTemplate = new DataTemplate { VisualTree = border };
        return sut;
    }

    private static MultiSelectComboBox CreateGrouped()
        => new()
        {
            Items = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["apple"] = "Apple",
                ["carrot"] = "Carrot",
                ["pear"] = "Pear",
                ["leek"] = "Leek",
            },
            ItemGroups = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["apple"] = "Fruit",
                ["carrot"] = "Vegetables",
                ["pear"] = "Fruit",
                ["leek"] = "Vegetables",
            },
        };

    private static CheckListItem GroupHeader(
        MultiSelectComboBox sut,
        string name)
        => sut.PartCheckList.VisibleRows.Single(x => x.IsGroupHeader && x.Key == name);

    private static void Layout(
        MultiSelectComboBox sut,
        double width)
    {
        sut.Width = width;
        sut.Measure(new Size(width, double.PositiveInfinity));
        sut.Arrange(new Rect(sut.DesiredSize));
        sut.UpdateLayout();
    }

    private static Dictionary<string, string> Items(int count)
        => Enumerable.Range(1, count).ToDictionary(
            i => "k" + i.ToString(CultureInfo.InvariantCulture),
            i => "Item " + i.ToString(CultureInfo.InvariantCulture),
            StringComparer.Ordinal);

    private static List<ValueChangedEventArgs<IReadOnlyList<string>>> Record(
        MultiSelectComboBox sut)
    {
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);
        return events;
    }
}