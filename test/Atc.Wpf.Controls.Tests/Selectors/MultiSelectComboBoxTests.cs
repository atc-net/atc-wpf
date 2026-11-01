namespace Atc.Wpf.Controls.Tests.Selectors;

public sealed class MultiSelectComboBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_MatchTheDocumentedApi()
    {
        var sut = new MultiSelectComboBox();

        Assert.NotNull(sut.Items);
        Assert.Empty(sut.Items);
        Assert.NotNull(sut.SelectedKeys);
        Assert.Empty(sut.SelectedKeys);
        Assert.NotNull(sut.DisabledKeys);
        Assert.False(sut.SelectNewItems);
        Assert.True(sut.ShowSelectAllRow);
        Assert.True(sut.ShowClearButton);
        Assert.Null(sut.IsFilterVisible);
        Assert.Equal(string.Empty, sut.FilterText);
        Assert.Equal(2, sut.MaxNamesInSummary);
        Assert.Null(sut.AllText);
        Assert.Equal(string.Empty, sut.NoneText);
        Assert.Null(sut.CountFormat);
        Assert.Equal(ComboBox.MaxDropDownHeightProperty.DefaultMetadata.DefaultValue, sut.MaxDropDownHeight);
        Assert.False(sut.IsDropDownOpen);
        Assert.Equal(string.Empty, sut.SummaryText);
    }

    [StaFact]
    public void Each_instance_gets_its_own_collections()
    {
        var first = new MultiSelectComboBox();
        var second = new MultiSelectComboBox();

        Assert.NotSame(first.SelectedKeys, second.SelectedKeys);
        Assert.NotSame(first.DisabledKeys, second.DisabledKeys);
    }

    [StaFact]
    public void Items_and_SelectedKeys_reach_the_pop_up_list()
    {
        var sut = Create(3);
        sut.SelectedKeys.Add("k2");

        Assert.Same(sut.Items, sut.PartCheckList.Items);
        Assert.Same(sut.SelectedKeys, sut.PartCheckList.SelectedKeys);
        Assert.True(sut.PartCheckList.GetItem("k2")!.IsChecked);
    }

    [StaFact]
    public void Options_are_passed_through_to_the_pop_up_list()
    {
        var sut = Create(3);
        var template = new DataTemplate();

        sut.ShowSelectAllRow = false;
        sut.ShowClearButton = false;
        sut.IsFilterVisible = true;
        sut.FilterText = "Item 1";
        sut.SelectNewItems = true;
        sut.ItemGroups = new Dictionary<string, string>(StringComparer.Ordinal) { ["k1"] = "G" };
        sut.IsGroupCheckable = false;
        sut.MaxSelectionCount = 2;
        sut.DisabledKeys.Add("k3");
        sut.ItemTemplate = template;

        Assert.False(sut.PartCheckList.ShowSelectAllRow);
        Assert.False(sut.PartCheckList.ShowClearButton);
        Assert.True(sut.PartCheckList.IsFilterVisible);
        Assert.Equal("Item 1", sut.PartCheckList.FilterText);
        Assert.True(sut.PartCheckList.SelectNewItems);
        Assert.Same(sut.ItemGroups, sut.PartCheckList.ItemGroups);
        Assert.False(sut.PartCheckList.IsGroupCheckable);
        Assert.Equal(2, sut.PartCheckList.MaxSelectionCount);
        Assert.Same(sut.DisabledKeys, sut.PartCheckList.DisabledKeys);
        Assert.Same(template, sut.PartCheckList.ItemTemplate);
    }

    [StaFact]
    public void The_pop_up_list_shows_Select_all_and_Clear_by_default()
    {
        var sut = Create(3);

        Assert.True(sut.PartCheckList.ShowSelectAllRow);
        Assert.True(sut.PartCheckList.ShowClearButton);
    }

    [StaFact]
    public void Tick_and_untick_update_SelectedKeys_and_raise_one_event_each()
    {
        var sut = Create(5);
        sut.Name = "Systems";
        var events = Record(sut);
        object? sender = null;
        sut.SelectedKeysChanged += (s, _) => sender = s;

        sut.PartCheckList.GetItem("k2")!.IsChecked = true;

        Assert.Equal(["k2"], sut.SelectedKeys);
        var tick = Assert.Single(events);
        Assert.Same(sut, sender);
        Assert.Equal("Systems", tick.Identifier);
        Assert.Empty(tick.OldValue);
        Assert.Equal(["k2"], tick.NewValue);

        sut.PartCheckList.GetItem("k2")!.IsChecked = false;

        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(2, events.Count);
        Assert.Equal(["k2"], events[1].OldValue);
        Assert.Empty(events[1].NewValue);
    }

    [StaFact]
    public void SelectAll_and_ClearSelection_raise_one_event_each()
    {
        var sut = Create(4);
        var events = Record(sut);

        sut.SelectAll();

        Assert.Equal(["k1", "k2", "k3", "k4"], sut.SelectedKeys);
        Assert.Single(events);

        sut.ClearSelection();

        Assert.Empty(sut.SelectedKeys);
        Assert.Equal(2, events.Count);
    }

    [StaFact]
    public void Select_all_with_a_filter_ticks_only_the_shown_items()
    {
        var sut = new MultiSelectComboBox
        {
            Items = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["a"] = "Apple",
                ["b"] = "Banana",
                ["c"] = "Apricot",
                ["d"] = "Cherry",
            },
        };
        var events = Record(sut);

        sut.FilterText = "ap";
        sut.PartCheckList.FlushPendingFilter();
        sut.SelectAll();

        Assert.Equal(["a", "c"], sut.SelectedKeys);
        Assert.Single(events);
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
        Assert.Equal(["k1", "k2"], change.NewValue);
    }

    [StaFact]
    public void SelectNewItems_ticks_keys_that_were_not_there_before()
    {
        var sut = new MultiSelectComboBox { SelectNewItems = true };
        sut.Items = Items(2);
        Assert.Equal(["k1", "k2"], sut.SelectedKeys);

        sut.SelectedKeys.Remove("k1");
        var events = Record(sut);

        sut.Items = Items(4);

        Assert.Equal(["k2", "k3", "k4"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Changing_items_keeps_the_ticks()
    {
        var sut = Create(3);
        sut.PartCheckList.GetItem("k2")!.IsChecked = true;
        sut.IsDropDownOpen = true;

        sut.Items = Items(5);

        Assert.True(sut.PartCheckList.GetItem("k2")!.IsChecked);
        Assert.Equal(["k2"], sut.SelectedKeys);
        Assert.True(sut.IsDropDownOpen);
    }

    [StaFact]
    public void Summary_with_nothing_ticked_is_NoneText()
    {
        var sut = Create(3);

        Assert.Equal(string.Empty, sut.SummaryText);
        Assert.Equal(Visibility.Visible, sut.PartWatermark.Visibility);

        sut.NoneText = "Nothing";

        Assert.Equal("Nothing", sut.SummaryText);
        Assert.Equal("Nothing", sut.PartWatermark.Text);
        Assert.Equal(Visibility.Visible, sut.PartWatermark.Visibility);
    }

    [StaFact]
    public void Summary_shows_the_watermark_text_when_nothing_is_ticked()
    {
        var sut = Create(3);

        TextBoxHelper.SetWatermark(sut, "Source systems");

        Assert.Equal("Source systems", sut.PartWatermark.Text);
        Assert.Equal(Visibility.Visible, sut.PartWatermark.Visibility);

        sut.SelectedKeys.Add("k1");

        Assert.Equal(Visibility.Collapsed, sut.PartWatermark.Visibility);
    }

    [StaFact]
    public void Summary_with_all_ticked_is_AllText()
    {
        var sut = Create(3);

        sut.SelectAll();

        Assert.Equal(Atc.Wpf.Resources.Word.All, sut.SummaryText);

        sut.AllText = "Every system";

        Assert.Equal("Every system", sut.SummaryText);
    }

    [StaFact]
    public void Summary_with_one_of_one_ticked_is_AllText()
    {
        var sut = Create(1);

        sut.SelectedKeys.Add("k1");

        Assert.Equal(Atc.Wpf.Resources.Word.All, sut.SummaryText);
    }

    [StaFact]
    public void Summary_up_to_MaxNamesInSummary_lists_the_names_in_item_order()
    {
        var sut = Create(5);

        sut.SelectedKeys.Add("k3");
        Assert.Equal("Item 3", sut.SummaryText);

        sut.SelectedKeys.Add("k1");
        Assert.Equal("Item 1, Item 3", sut.SummaryText);
    }

    [StaFact]
    public void Summary_beyond_MaxNamesInSummary_is_the_count()
    {
        var sut = Create(5);

        sut.SelectedKeys = ["k1", "k2", "k3"];

        Assert.Equal(
            string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.CountOfSelected2, 3, 5),
            sut.SummaryText);

        sut.CountFormat = "{0}/{1}";
        Assert.Equal("3/5", sut.SummaryText);

        sut.MaxNamesInSummary = 3;
        Assert.Equal("Item 1, Item 2, Item 3", sut.SummaryText);

        sut.MaxNamesInSummary = 0;
        sut.SelectedKeys = ["k1"];
        Assert.Equal("1/5", sut.SummaryText);
    }

    [StaFact]
    public void Summary_ignores_keys_that_are_not_in_Items()
    {
        var sut = Create(3);

        sut.SelectedKeys = ["k1", "zz"];

        Assert.Equal("Item 1", sut.SummaryText);
    }

    [StaFact]
    public void Summary_follows_ticks_items_and_a_new_SelectedKeys_instance()
    {
        var sut = Create(3);

        sut.PartCheckList.GetItem("k2")!.IsChecked = true;
        Assert.Equal("Item 2", sut.SummaryText);

        sut.Items = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["k1"] = "One",
            ["k2"] = "Two",
        };
        Assert.Equal("Two", sut.SummaryText);

        var replaced = new System.Collections.ObjectModel.ObservableCollection<string> { "k1" };
        sut.SelectedKeys = replaced;
        Assert.Equal("One", sut.SummaryText);
        Assert.True(sut.PartCheckList.GetItem("k1")!.IsChecked);
        Assert.False(sut.PartCheckList.GetItem("k2")!.IsChecked);

        replaced.Add("k2");
        Assert.Equal(Atc.Wpf.Resources.Word.All, sut.SummaryText);
    }

    [StaFact]
    public void A_new_SelectedKeys_instance_from_a_binding_is_picked_up()
    {
        var source = new SelectionSource { Keys = ["k1"] };
        var sut = Create(3);
        sut.SetBinding(
            MultiSelectComboBox.SelectedKeysProperty,
            new Binding(nameof(SelectionSource.Keys)) { Source = source });

        Assert.Same(source.Keys, sut.SelectedKeys);
        Assert.Equal("Item 1", sut.SummaryText);

        sut.PartCheckList.GetItem("k3")!.IsChecked = true;
        Assert.Equal(["k1", "k3"], source.Keys);

        source.Keys = ["k2"];

        Assert.Same(source.Keys, sut.SelectedKeys);
        Assert.Same(source.Keys, sut.PartCheckList.SelectedKeys);
        Assert.Equal("Item 2", sut.SummaryText);
    }

    [StaFact]
    public void IsDropDownOpen_opens_and_closes_the_pop_up()
    {
        var sut = Create(3);
        var opened = 0;
        var closed = 0;
        sut.DropDownOpened += (_, _) => opened++;
        sut.DropDownClosed += (_, _) => closed++;
        var window = new Window
        {
            Content = sut,
            Width = 300,
            Height = 100,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };
        window.Show();
        try
        {
            sut.IsDropDownOpen = true;
            Assert.True(sut.PartPopup.IsOpen);

            sut.IsDropDownOpen = false;
            Assert.False(sut.PartPopup.IsOpen);
        }
        finally
        {
            sut.IsDropDownOpen = false;
            window.Close();
        }

        Assert.Equal(1, opened);
        Assert.Equal(1, closed);
    }

    [StaFact]
    public void Ticking_does_not_close_the_pop_up()
    {
        var sut = Create(3);
        sut.IsDropDownOpen = true;

        sut.PartCheckList.GetItem("k1")!.IsChecked = true;
        sut.SelectAll();

        Assert.True(sut.IsDropDownOpen);
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
        var sut = Create(3);
        sut.IsDropDownOpen = isOpen;

        var handled = sut.HandleKey(key, systemKey, modifiers);

        Assert.True(handled);
        Assert.Equal(expectedOpen, sut.IsDropDownOpen);
    }

    [StaTheory]
    [InlineData(Key.Escape, false)]
    [InlineData(Key.Enter, false)]
    [InlineData(Key.A, true)]
    [InlineData(Key.Space, true)]
    public void Keys_that_do_not_open_or_close_are_not_handled(
        Key key,
        bool isOpen)
    {
        var sut = Create(3);
        sut.IsDropDownOpen = isOpen;

        var handled = sut.HandleKey(key, Key.None, ModifierKeys.None);

        Assert.False(handled);
        Assert.Equal(isOpen, sut.IsDropDownOpen);
    }

    [StaFact]
    public void Automation_peer_is_a_combo_box_with_ExpandCollapse_and_the_summary_as_value()
    {
        var sut = Create(3);
        sut.SelectedKeys = ["k1", "k2"];
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.Equal(AutomationControlType.ComboBox, peer.GetAutomationControlType());

        var value = (IValueProvider)peer.GetPattern(PatternInterface.Value);
        Assert.Equal("Item 1, Item 2", value.Value);
        Assert.True(value.IsReadOnly);

        var expandCollapse = (IExpandCollapseProvider)peer.GetPattern(PatternInterface.ExpandCollapse);
        Assert.Equal(System.Windows.Automation.ExpandCollapseState.Collapsed, expandCollapse.ExpandCollapseState);

        expandCollapse.Expand();
        Assert.True(sut.IsDropDownOpen);
        Assert.Equal(System.Windows.Automation.ExpandCollapseState.Expanded, expandCollapse.ExpandCollapseState);

        expandCollapse.Collapse();
        Assert.False(sut.IsDropDownOpen);
    }

    [StaFact]
    public void Automation_peer_exposes_the_pop_up_rows_only_while_open()
    {
        var sut = Create(3);
        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.DoesNotContain(peer.GetChildren() ?? [], x => x.GetAutomationControlType() == AutomationControlType.CheckBox);

        sut.IsDropDownOpen = true;
        sut.PartPopupBorder.Measure(new Size(300, 400));
        sut.PartPopupBorder.Arrange(new Rect(0, 0, 300, 400));
        sut.PartPopupBorder.UpdateLayout();
        peer.ResetChildrenCache();

        var names = Descendants(peer)
            .Where(x => x.GetAutomationControlType() == AutomationControlType.CheckBox)
            .Select(x => x.GetName())
            .ToList();
        Assert.Contains("Item 1", names, StringComparer.Ordinal);
        Assert.Contains("Item 3", names, StringComparer.Ordinal);
    }

    [StaFact]
    public void The_pop_up_list_is_virtualized_for_1000_items()
    {
        var sut = Create(1000);
        sut.MaxDropDownHeight = 300;
        sut.IsDropDownOpen = true;

        sut.PartPopupBorder.Measure(new Size(300, double.PositiveInfinity));
        sut.PartPopupBorder.Arrange(new Rect(sut.PartPopupBorder.DesiredSize));
        sut.PartPopupBorder.UpdateLayout();

        Assert.True(sut.PartPopupBorder.DesiredSize.Height <= 300);
        var realized = CountVisualChildren<ListBoxItem>(sut.PartCheckList);
        Assert.InRange(realized, 1, 100);
    }

    [StaFact]
    public void Escape_clears_the_filter_first_then_closes()
    {
        var sut = Create(3);
        sut.IsDropDownOpen = true;
        sut.FilterText = "2";

        Assert.True(sut.HandleKey(Key.Escape, Key.None, ModifierKeys.None));
        Assert.Equal(string.Empty, sut.FilterText);
        Assert.True(sut.IsDropDownOpen);

        Assert.True(sut.HandleKey(Key.Escape, Key.None, ModifierKeys.None));
        Assert.False(sut.IsDropDownOpen);
    }

    private static MultiSelectComboBox Create(int itemCount)
        => new() { Items = Items(itemCount) };

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

    private static IEnumerable<AutomationPeer> Descendants(AutomationPeer peer)
    {
        foreach (var child in peer.GetChildren() ?? [])
        {
            yield return child;
            foreach (var descendant in Descendants(child))
            {
                yield return descendant;
            }
        }
    }

    private static int CountVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        var count = 0;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T)
            {
                count++;
            }

            count += CountVisualChildren<T>(child);
        }

        return count;
    }

    private sealed class SelectionSource : INotifyPropertyChanged
    {
        private System.Collections.ObjectModel.ObservableCollection<string> keys = [];

        public event PropertyChangedEventHandler? PropertyChanged;

        public System.Collections.ObjectModel.ObservableCollection<string> Keys
        {
            get => keys;
            set
            {
                keys = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Keys)));
            }
        }
    }
}