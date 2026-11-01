namespace Atc.Wpf.Controls.Tests.Selectors;

public sealed class MultiColumnComboBoxTests : IDisposable
{
    private readonly List<MultiColumnComboBox> openedBoxes = [];

    public void Dispose()
    {
        foreach (var box in openedBoxes)
        {
            box.IsDropDownOpen = false;
        }

        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaFact]
    public void Defaults_match_the_documented_api()
    {
        var sut = new MultiColumnComboBox();

        Assert.Null(sut.ItemsSource);
        Assert.Equal(string.Empty, sut.KeyMemberPath);
        Assert.Equal(string.Empty, sut.DisplayMemberPath);
        Assert.NotNull(sut.Columns);
        Assert.Empty(sut.Columns);
        Assert.False(sut.AutoGenerateColumns);
        Assert.NotNull(sut.SelectedKeys);
        Assert.Empty(sut.SelectedKeys);
        Assert.Empty(sut.SelectedItems);
        Assert.Null(sut.MaxSelectionCount);
        Assert.Equal(string.Empty, sut.IsEnabledMemberPath);
        Assert.Null(sut.IsFilterVisible);
        Assert.Equal(string.Empty, sut.FilterText);
        Assert.Null(sut.FilterMemberPaths);
        Assert.False(sut.KeepSelectedOnTop);
        Assert.True(double.IsNaN(sut.DropDownWidth));
        Assert.Equal(ComboBox.MaxDropDownHeightProperty.DefaultMetadata.DefaultValue, sut.MaxDropDownHeight);
        Assert.Equal(MultiSelectCommitMode.Immediate, sut.CommitMode);
        Assert.Equal(MultiSelectDisplayMode.Text, sut.DisplayMode);
        Assert.Equal(2, sut.MaxNamesInSummary);
        Assert.False(sut.IsDropDownOpen);
        Assert.Equal(string.Empty, sut.SummaryText);
    }

    [StaFact]
    public void Each_instance_gets_its_own_collections()
    {
        var first = new MultiColumnComboBox();
        var second = new MultiColumnComboBox();

        Assert.NotSame(first.SelectedKeys, second.SelectedKeys);
        Assert.NotSame(first.Columns, second.Columns);
    }

    [StaFact]
    public void Columns_move_into_the_pop_up_grid_after_the_check_column()
    {
        var sut = Create();

        Assert.Equal(4, sut.PartGrid.Columns.Count);
        Assert.IsType<MultiColumnCheckColumn>(sut.PartGrid.Columns[0]);
        Assert.Same(sut.Columns[0], sut.PartGrid.Columns[1]);
        Assert.Same(sut.Columns[2], sut.PartGrid.Columns[3]);

        var city = sut.Columns[2];
        sut.Columns.Remove(city);

        Assert.Equal(3, sut.PartGrid.Columns.Count);
        Assert.DoesNotContain(city, sut.PartGrid.Columns);

        // A released column can join another control.
        var other = new MultiColumnComboBox();
        other.Columns.Add(city);
        Assert.Same(city, other.PartGrid.Columns[1]);
    }

    [StaFact]
    public void A_new_Columns_collection_replaces_the_grid_columns()
    {
        var sut = Create();
        var old = sut.Columns.ToList();

        sut.Columns = [new DataGridTextColumn { Header = "Only", Binding = new Binding("Name") }];

        Assert.Equal(2, sut.PartGrid.Columns.Count);
        Assert.Equal("Only", sut.PartGrid.Columns[1].Header);
        Assert.All(old, x => Assert.DoesNotContain(x, sut.PartGrid.Columns));

        old[0].Header = "Changed";
        sut.Columns.Add(new DataGridTextColumn { Header = "Second", Binding = new Binding("City") });
        Assert.Equal(3, sut.PartGrid.Columns.Count);
    }

    [StaFact]
    public void AutoGenerateColumns_adds_one_text_column_per_public_property_when_Columns_is_empty()
    {
        var sut = new MultiColumnComboBox
        {
            KeyMemberPath = nameof(Customer.Number),
            AutoGenerateColumns = true,
            ItemsSource = Customers(),
        };

        var headers = sut.PartGrid.Columns.Skip(1).Select(x => x.Header as string).ToList();
        Assert.Equal(["Number", "Name", "City", "IsActive", "Info"], headers);
        Assert.All(sut.PartGrid.Columns.Skip(1), x => Assert.IsType<DataGridTextColumn>(x));

        sut.AutoGenerateColumns = false;
        Assert.Single(sut.PartGrid.Columns);
    }

    [StaFact]
    public void AutoGenerateColumns_is_ignored_when_Columns_are_given()
    {
        var sut = Create();
        sut.AutoGenerateColumns = true;

        Assert.Equal(4, sut.PartGrid.Columns.Count);
        Assert.Same(sut.Columns[0], sut.PartGrid.Columns[1]);
    }

    [StaFact]
    public void Toggling_a_row_updates_SelectedKeys_and_raises_one_event_each()
    {
        var sut = Create();
        sut.Name = "Customers";
        var items = (List<Customer>)sut.ItemsSource!;
        var events = Record(sut);

        Assert.True(sut.ToggleItem(items[1]));

        Assert.Equal(["1002"], sut.SelectedKeys);
        Assert.True(sut.IsKeyChecked("1002"));
        var tick = Assert.Single(events);
        Assert.Equal("Customers", tick.Identifier);
        Assert.Empty(tick.OldValue);
        Assert.Equal(["1002"], tick.NewValue);

        sut.ToggleItem(items[1]);

        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(2, events.Count);
    }

    [StaFact]
    public void Clicking_a_row_anywhere_toggles_it_but_a_header_click_does_not()
    {
        var sut = Create();
        var events = Record(sut);
        OpenAndRealize(sut);

        var row = FindVisualChildren<DataGridRow>(sut.PartGrid).First(x => x.Item is Customer { Number: "1003" });
        var cell = FindVisualChildren<DataGridCell>(row).Last();

        Assert.True(sut.HandleRowPress(cell));
        Assert.Equal(["1003"], sut.SelectedKeys);
        Assert.Single(events);

        var header = FindVisualChildren<DataGridColumnHeader>(sut.PartGrid).First(x => x.Column is not null);
        Assert.False(sut.HandleRowPress(header));
        Assert.Single(events);
    }

    [StaFact]
    public void Ticked_rows_are_marked_for_the_selected_background()
    {
        var sut = Create();
        sut.SelectedKeys.Add("1002");
        OpenAndRealize(sut);

        var rows = FindVisualChildren<DataGridRow>(sut.PartGrid).ToList();
        var ticked = rows.Single(x => x.Item is Customer { Number: "1002" });
        var other = rows.First(x => x.Item is Customer { Number: "1001" });

        Assert.True(MultiColumnRowState.GetIsChecked(ticked));
        Assert.False(MultiColumnRowState.GetIsChecked(other));

        sut.ToggleItem(other.Item);
        Assert.True(MultiColumnRowState.GetIsChecked(other));
    }

    [StaFact]
    public void Space_toggles_the_focused_row()
    {
        var sut = Create();
        var items = (List<Customer>)sut.ItemsSource!;
        var events = Record(sut);
        sut.IsDropDownOpen = true;
        sut.PartGrid.SelectedItem = items[2];

        Assert.True(sut.HandleKey(Key.Space, Key.None, ModifierKeys.None));

        Assert.Equal(["1003"], sut.SelectedKeys);
        Assert.Single(events);
        sut.IsDropDownOpen = false;
    }

    [StaFact]
    public void Header_select_all_ticks_only_the_shown_rows_with_one_event()
    {
        var sut = Create();
        var events = Record(sut);
        sut.FilterText = "berlin";
        sut.FlushPendingFilter();

        Assert.False(sut.HeaderCheckState);

        sut.ToggleAllShown();

        Assert.Equal(["1002", "1005"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.True(sut.HeaderCheckState);

        sut.FilterText = string.Empty;
        sut.FlushPendingFilter();
        Assert.Null(sut.HeaderCheckState);

        sut.FilterText = "berlin";
        sut.FlushPendingFilter();
        sut.ToggleAllShown();

        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(2, events.Count);
    }

    [StaFact]
    public void Ctrl_A_ticks_all_shown_rows()
    {
        var sut = Create();
        var events = Record(sut);
        sut.IsDropDownOpen = true;
        sut.FilterText = "Aa";
        sut.FlushPendingFilter();

        Assert.True(sut.HandleKey(Key.A, Key.None, ModifierKeys.Control));

        Assert.Equal(["1001", "1004"], sut.SelectedKeys);
        Assert.Single(events);
        sut.IsDropDownOpen = false;
    }

    [StaFact]
    public void Filter_matches_any_visible_column_case_insensitively()
    {
        var sut = Create();

        sut.FilterText = "COPEN";
        sut.FlushPendingFilter();
        Assert.Equal(["1003"], ShownKeys(sut));

        sut.FilterText = "100";
        sut.FlushPendingFilter();
        Assert.Equal(5, ShownKeys(sut).Count);

        // A hidden column is not searched.
        sut.Columns[2].Visibility = Visibility.Collapsed;
        sut.FilterText = "Copenhagen";
        sut.FlushPendingFilter();
        Assert.Empty(ShownKeys(sut));
        Assert.True(sut.IsNoMatchesShown);
    }

    [StaFact]
    public void FilterMemberPaths_limits_the_search_to_those_paths()
    {
        var sut = Create();
        sut.FilterMemberPaths = "Name, Info.Label";

        sut.FilterText = "Berlin";
        sut.FlushPendingFilter();
        Assert.Empty(ShownKeys(sut));

        sut.FilterText = "label-1004";
        sut.FlushPendingFilter();
        Assert.Equal(["1004"], ShownKeys(sut));

        sut.FilterText = "echo";
        sut.FlushPendingFilter();
        Assert.Equal(["1005"], ShownKeys(sut));
    }

    [StaFact]
    public void Filtering_and_sorting_never_change_SelectedKeys()
    {
        var sut = Create();
        sut.SelectedKeys.Add("1004");
        sut.SelectedKeys.Add("1001");
        var events = Record(sut);

        sut.PartGrid.Items.SortDescriptions.Add(new SortDescription("Number", ListSortDirection.Descending));
        sut.FilterText = "Berlin";
        sut.FlushPendingFilter();

        Assert.Equal(["1004", "1001"], sut.SelectedKeys);
        Assert.Empty(events);
        Assert.Equal(["1005", "1002"], ShownKeys(sut));
    }

    [StaFact]
    public void Dotted_KeyMemberPath_and_DisplayMemberPath_are_read()
    {
        var sut = Create();
        sut.KeyMemberPath = "Info.Id";
        sut.DisplayMemberPath = "Info.Label";
        var items = (List<Customer>)sut.ItemsSource!;

        sut.ToggleItem(items[0]);
        sut.ToggleItem(items[2]);

        Assert.Equal(["id-1001", "id-1003"], sut.SelectedKeys);
        Assert.Equal("label-1001, label-1003", sut.SummaryText);
    }

    [StaFact]
    public void SelectedItems_follows_SelectedKeys_in_key_order()
    {
        var sut = Create();
        var items = (List<Customer>)sut.ItemsSource!;

        sut.SelectedKeys.Add("1003");
        sut.SelectedKeys.Add("1001");
        sut.SelectedKeys.Add("9999");

        Assert.Equal([items[2], items[0]], sut.SelectedItems);

        sut.SelectedKeys = ["1005"];
        Assert.Equal([items[4]], sut.SelectedItems);

        sut.ToggleItem(items[1]);
        Assert.Equal([items[4], items[1]], sut.SelectedItems);
    }

    [StaFact]
    public void Summary_lists_display_values_then_counts()
    {
        var sut = Create();

        sut.SelectedKeys.Add("1002");
        Assert.Equal("Bravo", sut.SummaryText);

        sut.SelectedKeys.Add("1001");
        Assert.Equal("Alpha, Bravo", sut.SummaryText);

        sut.SelectedKeys.Add("1003");
        Assert.Equal(
            string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.CountOfSelected2, 3, 5),
            sut.SummaryText);
        Assert.Equal(sut.SummaryText, sut.PartFooterText.Text);
    }

    [StaFact]
    public void At_the_limit_the_unticked_rows_cannot_be_toggled()
    {
        var sut = Create();
        sut.MaxSelectionCount = 2;
        var items = (List<Customer>)sut.ItemsSource!;
        var events = Record(sut);

        sut.ToggleItem(items[0]);
        sut.ToggleItem(items[1]);

        Assert.False(sut.ToggleItem(items[2]));
        Assert.False(sut.CanToggle(items[2]));
        Assert.True(sut.CanToggle(items[0]));
        Assert.Equal(["1001", "1002"], sut.SelectedKeys);

        sut.ClearSelection();
        sut.SelectAll();

        // Five shown rows do not fit a limit of two: nothing is ticked.
        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(3, events.Count);
    }

    [StaFact]
    public void Disabled_rows_cannot_be_toggled_and_select_all_skips_them()
    {
        var sut = Create();
        sut.IsEnabledMemberPath = nameof(Customer.IsActive);
        var items = (List<Customer>)sut.ItemsSource!;

        Assert.False(sut.ToggleItem(items[4]));
        Assert.False(sut.CanToggle(items[4]));

        sut.SelectAll();

        Assert.Equal(["1001", "1002", "1003", "1004"], sut.SelectedKeys);

        sut.SelectedKeys.Add("1005");
        sut.ClearSelection();
        Assert.Equal(["1005"], sut.SelectedKeys);
    }

    [StaFact]
    public void Keys_bound_before_the_items_load_are_kept()
    {
        var sut = new MultiColumnComboBox { KeyMemberPath = nameof(Customer.Number), DisplayMemberPath = nameof(Customer.Name) };
        sut.SelectedKeys = ["1002", "1004"];
        var events = Record(sut);

        sut.ItemsSource = Customers();

        Assert.Equal(["1002", "1004"], sut.SelectedKeys);
        Assert.Empty(events);
        Assert.Equal("Bravo, Delta", sut.SummaryText);
    }

    [StaFact]
    public void Replacing_the_items_drops_only_keys_whose_item_was_present_before_with_one_event()
    {
        var sut = Create();
        sut.SelectedKeys = ["1001", "1002", "7777"];
        var events = Record(sut);

        sut.ItemsSource = Customers().Skip(2).ToList();

        Assert.Equal(["7777"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Equal(["1001", "1002", "7777"], change.OldValue);
    }

    [StaFact]
    public void Removing_an_item_from_an_observable_source_drops_its_key()
    {
        var source = new System.Collections.ObjectModel.ObservableCollection<Customer>(Customers());
        var sut = Create();
        sut.ItemsSource = source;
        sut.SelectedKeys = ["1001", "1003"];
        var events = Record(sut);

        source.RemoveAt(0);

        Assert.Equal(["1003"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.Equal(4, ShownKeys(sut).Count);

        source.Add(new Customer("1006", "Foxtrot", "Odense"));
        Assert.Contains("1006", ShownKeys(sut), StringComparer.Ordinal);
    }

    [StaFact]
    public void KeepSelectedOnTop_puts_the_ticked_rows_first_when_the_pop_up_opens()
    {
        var sut = Create();
        sut.KeepSelectedOnTop = true;
        sut.SelectedKeys = ["1004", "1002"];
        sut.PartGrid.Items.SortDescriptions.Add(new SortDescription("City", ListSortDirection.Ascending));

        sut.IsDropDownOpen = true;

        Assert.Equal(["1002", "1004", "1001", "1003", "1005"], ShownKeys(sut));

        // Ticking while open does not move rows.
        sut.ToggleItem(((List<Customer>)sut.ItemsSource!)[4]);
        Assert.Equal(["1002", "1004", "1001", "1003", "1005"], ShownKeys(sut));

        sut.IsDropDownOpen = false;
        sut.IsDropDownOpen = true;
        Assert.Equal(["1002", "1004", "1005", "1001", "1003"], ShownKeys(sut));
        sut.IsDropDownOpen = false;
    }

    [StaFact]
    public void Deferred_cancel_restores_and_ok_raises_one_event()
    {
        var sut = Create();
        sut.CommitMode = MultiSelectCommitMode.Deferred;
        sut.SelectedKeys.Add("1001");
        var items = (List<Customer>)sut.ItemsSource!;
        var events = Record(sut);

        sut.IsDropDownOpen = true;
        Assert.Equal(Visibility.Visible, sut.PartCommitFooter.Visibility);
        sut.ToggleItem(items[1]);
        sut.ToggleItem(items[0]);

        Assert.True(sut.IsKeyChecked("1002"));
        Assert.False(sut.IsKeyChecked("1001"));
        Assert.Equal(["1001"], sut.SelectedKeys);
        Assert.Empty(events);

        sut.PartCancelButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        Assert.False(sut.IsDropDownOpen);
        Assert.Equal(["1001"], sut.SelectedKeys);
        Assert.True(sut.IsKeyChecked("1001"));
        Assert.Empty(events);

        sut.IsDropDownOpen = true;
        sut.ToggleItem(items[1]);
        sut.ToggleItem(items[2]);
        sut.PartOkButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        Assert.False(sut.IsDropDownOpen);
        Assert.Equal(["1001", "1002", "1003"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Equal(["1001"], change.OldValue);
    }

    [StaFact]
    public void Deferred_enter_commits_and_escape_cancels()
    {
        var sut = Create();
        sut.CommitMode = MultiSelectCommitMode.Deferred;
        var items = (List<Customer>)sut.ItemsSource!;
        var events = Record(sut);

        sut.IsDropDownOpen = true;
        sut.ToggleItem(items[0]);
        sut.HandleKey(Key.Escape, Key.None, ModifierKeys.None);
        Assert.Empty(sut.SelectedKeys);

        sut.IsDropDownOpen = true;
        sut.ToggleItem(items[3]);
        sut.HandleKey(Key.Enter, Key.None, ModifierKeys.None);

        Assert.False(sut.IsDropDownOpen);
        Assert.Equal(["1004"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaTheory]
    [InlineData(Key.F4, Key.None, ModifierKeys.None, false, true)]
    [InlineData(Key.F4, Key.None, ModifierKeys.None, true, false)]
    [InlineData(Key.System, Key.Down, ModifierKeys.Alt, false, true)]
    [InlineData(Key.System, Key.Up, ModifierKeys.Alt, true, false)]
    [InlineData(Key.Escape, Key.None, ModifierKeys.None, true, false)]
    [InlineData(Key.Enter, Key.None, ModifierKeys.None, true, false)]
    public void Keyboard_opens_and_closes_the_pop_up(
        Key key,
        Key systemKey,
        ModifierKeys modifiers,
        bool isOpen,
        bool expectedOpen)
    {
        var sut = Create();
        sut.IsDropDownOpen = isOpen;

        Assert.True(sut.HandleKey(key, systemKey, modifiers));
        Assert.Equal(expectedOpen, sut.IsDropDownOpen);
        sut.IsDropDownOpen = false;
    }

    [StaFact]
    public void Escape_clears_the_filter_text_before_closing()
    {
        var sut = Create();
        sut.IsDropDownOpen = true;
        sut.FilterText = "Berlin";

        Assert.True(sut.HandleKey(Key.Escape, Key.None, ModifierKeys.None));
        Assert.Equal(string.Empty, sut.FilterText);
        Assert.True(sut.IsDropDownOpen);

        Assert.True(sut.HandleKey(Key.Escape, Key.None, ModifierKeys.None));
        Assert.False(sut.IsDropDownOpen);
    }

    [StaFact]
    public void Typing_while_closed_opens_the_pop_up_and_goes_to_the_filter_box()
    {
        var sut = Create();
        sut.IsFilterVisible = true;

        Assert.True(sut.HandleTextInput("Ber"));

        Assert.True(sut.IsDropDownOpen);
        Assert.Equal("Ber", sut.FilterText);

        Assert.True(sut.HandleTextInput("l"));
        Assert.Equal("Berl", sut.FilterText);
        sut.IsDropDownOpen = false;
    }

    [StaFact]
    public void Typing_is_ignored_when_the_filter_box_is_hidden()
    {
        var sut = Create();

        Assert.False(sut.IsFilterShown);
        Assert.False(sut.HandleTextInput("B"));
        Assert.False(sut.IsDropDownOpen);
    }

    [StaFact]
    public void The_filter_box_is_shown_above_ten_items_by_default()
    {
        var sut = Create();
        Assert.False(sut.IsFilterShown);

        sut.ItemsSource = Parts(11);
        Assert.True(sut.IsFilterShown);

        sut.IsFilterVisible = false;
        Assert.False(sut.IsFilterShown);
    }

    [StaFact]
    public void Chips_mode_shows_a_chip_per_ticked_item_and_removing_one_raises_one_event()
    {
        var sut = Create();
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.SelectedKeys = ["1001", "1003"];
        var events = Record(sut);

        Assert.Equal(["Alpha", "Charlie"], sut.GetChipItems().Select(x => x.DisplayValue), StringComparer.Ordinal);

        sut.RemoveKey("1001");

        Assert.Equal(["1003"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.Equal(["Charlie"], sut.GetChipItems().Select(x => x.DisplayValue), StringComparer.Ordinal);
    }

    [StaFact]
    public void Automation_peer_is_a_combo_box_with_ExpandCollapse_and_the_summary_as_value()
    {
        var sut = Create();
        sut.SelectedKeys = ["1001", "1002"];
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.Equal(AutomationControlType.ComboBox, peer.GetAutomationControlType());
        var value = (IValueProvider)peer.GetPattern(PatternInterface.Value);
        Assert.Equal("Alpha, Bravo", value.Value);
        Assert.True(value.IsReadOnly);

        var expandCollapse = (IExpandCollapseProvider)peer.GetPattern(PatternInterface.ExpandCollapse);
        expandCollapse.Expand();
        Assert.True(sut.IsDropDownOpen);
        expandCollapse.Collapse();
        Assert.False(sut.IsDropDownOpen);
    }

    [StaFact]
    public void Automation_exposes_the_grid_and_a_named_toggle_check_box_per_row_while_open()
    {
        var sut = Create();
        var events = Record(sut);
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.DoesNotContain(peer.GetChildren() ?? [], x => x.GetAutomationControlType() == AutomationControlType.DataGrid);

        OpenAndRealize(sut);
        peer.ResetChildrenCache();

        Assert.Contains(peer.GetChildren() ?? [], x => x.GetAutomationControlType() == AutomationControlType.DataGrid);

        var checkBoxes = FindVisualChildren<MultiColumnCheckBox>(sut.PartGrid)
            .Where(x => x.IsRowCheckBox)
            .Select(UIElementAutomationPeer.CreatePeerForElement)
            .ToList();
        var bravo = checkBoxes.Single(x => string.Equals(x.GetName(), "Bravo", StringComparison.Ordinal));
        Assert.Equal(AutomationControlType.CheckBox, bravo.GetAutomationControlType());

        var toggle = (IToggleProvider)bravo.GetPattern(PatternInterface.Toggle);
        toggle.Toggle();

        Assert.Equal(["1002"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.Equal(System.Windows.Automation.ToggleState.On, toggle.ToggleState);
    }

    [StaFact]
    public void The_header_check_box_is_a_named_tri_state_toggle()
    {
        var sut = Create();
        var header = sut.HeaderCheckBox;
        var peer = UIElementAutomationPeer.CreatePeerForElement(header);

        Assert.Equal(Atc.Wpf.Resources.Miscellaneous.SelectAll, peer.GetName());

        ((IToggleProvider)peer.GetPattern(PatternInterface.Toggle)).Toggle();

        Assert.Equal(5, sut.SelectedKeys.Count);
        Assert.True(header.IsChecked);
    }

    [StaFact]
    public void Ten_thousand_rows_are_virtualized_and_the_filter_is_debounced()
    {
        var sut = new MultiColumnComboBox
        {
            KeyMemberPath = nameof(Part.Number),
            DisplayMemberPath = nameof(Part.Description),
            AutoGenerateColumns = true,
            ItemsSource = Parts(10_000),
            MaxDropDownHeight = 300,
        };

        OpenAndRealize(sut, 400);

        Assert.True(sut.PartPopupBorder.DesiredSize.Height <= 300);
        Assert.InRange(FindVisualChildren<DataGridRow>(sut.PartGrid).Count(), 1, 100);

        sut.FilterText = "P-0004";
        Assert.Equal(10_000, sut.PartGrid.Items.Count);
        Assert.True(sut.IsFilterPending);

        sut.FlushPendingFilter();
        Assert.Equal(10, sut.PartGrid.Items.Count);

        sut.FilterText = "P-000";
        sut.IsDropDownOpen = false;
        Assert.False(sut.IsFilterPending);
    }

    private static MultiColumnComboBox Create()
    {
        var sut = new MultiColumnComboBox
        {
            KeyMemberPath = nameof(Customer.Number),
            DisplayMemberPath = nameof(Customer.Name),
        };
        sut.Columns.Add(new DataGridTextColumn { Header = "No.", Binding = new Binding(nameof(Customer.Number)) });
        sut.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding(nameof(Customer.Name)) });
        sut.Columns.Add(new DataGridTextColumn { Header = "City", Binding = new Binding(nameof(Customer.City)) });
        sut.ItemsSource = Customers();
        return sut;
    }

    private static List<Customer> Customers()
        =>
        [
            new("1001", "Alpha", "Aarhus"),
            new("1002", "Bravo", "Berlin"),
            new("1003", "Charlie", "Copenhagen"),
            new("1004", "Delta", "Aalborg"),
            new("1005", "Echo", "Berlin", isActive: false),
        ];

    private static List<Part> Parts(int count)
        => Enumerable.Range(1, count)
            .Select(i => new Part(
                "P-" + i.ToString("D5", CultureInfo.InvariantCulture),
                "Part " + i.ToString(CultureInfo.InvariantCulture),
                i % 50))
            .ToList();

    private static List<string> ShownKeys(MultiColumnComboBox sut)
        => sut.PartGrid.Items
            .Cast<object>()
            .Select(x => x is Customer c ? c.Number : ((Part)x).Number)
            .ToList();

    private void OpenAndRealize(
        MultiColumnComboBox sut,
        double height = 400)
    {
        openedBoxes.Add(sut);
        sut.IsDropDownOpen = true;
        sut.PartPopupBorder.Measure(new Size(500, height));
        sut.PartPopupBorder.Arrange(new Rect(0, 0, 500, System.Math.Min(height, sut.PartPopupBorder.DesiredSize.Height)));
        sut.PartPopupBorder.UpdateLayout();
    }

    private static List<ValueChangedEventArgs<IReadOnlyList<string>>> Record(
        MultiColumnComboBox sut)
    {
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);
        return events;
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
            {
                yield return match;
            }

            foreach (var descendant in FindVisualChildren<T>(child))
            {
                yield return descendant;
            }
        }
    }

    private sealed class Customer
    {
        public Customer(
            string number,
            string name,
            string city,
            bool isActive = true)
        {
            Number = number;
            Name = name;
            City = city;
            IsActive = isActive;
            Info = new CustomerInfo("id-" + number, "label-" + number);
        }

        public string Number { get; }

        public string Name { get; }

        public string City { get; }

        public bool IsActive { get; }

        public CustomerInfo Info { get; }
    }

    private sealed record CustomerInfo(string Id, string Label);

    private sealed record Part(string Number, string Description, int Stock);
}