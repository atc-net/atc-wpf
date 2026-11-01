namespace Atc.Wpf.Controls.Tests.Selectors;

[Collection(nameof(PackResourceTestGroup))]
public sealed class TokenBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_MatchTheDocumentedApi()
    {
        var sut = new TokenBox();

        Assert.Empty(sut.Items);
        Assert.Empty(sut.SelectedKeys);
        Assert.Empty(sut.FixedKeys);
        Assert.False(sut.AllowCreate);
        Assert.Null(sut.CreateValidator);
        Assert.Equal(",;", sut.Separators);
        Assert.Equal(TokenFilterMode.Contains, sut.FilterMode);
        Assert.Null(sut.CustomFilter);
        Assert.Equal(1, sut.MinimumPrefixLength);
        Assert.Equal(50, sut.MaxSuggestions);
        Assert.True(sut.TokensWrap);
        Assert.Null(sut.MaxVisibleTokens);
        Assert.Null(sut.MaxSelectionCount);
        Assert.False(sut.AllowEditTokens);
        Assert.False(sut.ShowClearButton);
        Assert.Null(sut.ItemTemplate);
        Assert.Null(sut.TokenTemplate);
        Assert.Null(sut.SuggestionsProvider);
        Assert.Equal(TimeSpan.FromMilliseconds(250), sut.SuggestionDelay);
        Assert.Equal(string.Empty, sut.Text);
        Assert.False(sut.HasError);
    }

    [StaFact]
    public void Typing_shows_matching_suggestions_without_picked_items_and_highlights_the_first()
    {
        var sut = Create();
        sut.SelectedKeys.Add("payments");

        sut.Text = "ing";

        Assert.True(sut.IsDropDownOpen);
        Assert.Equal(["Shipping", "Billing"], sut.Suggestions.Select(x => x.DisplayValue).ToList());
        Assert.Equal(0, sut.HighlightedIndex);
        var first = sut.Suggestions[0];
        Assert.Equal("Shipp", first.TextBeforeMatch);
        Assert.Equal("ing", first.MatchedText);
        Assert.Equal(string.Empty, first.TextAfterMatch);

        sut.Text = "pay";

        Assert.Empty(sut.Suggestions);
        sut.SetCurrentValue(TokenBox.IsDropDownOpenProperty, false);
    }

    [StaFact]
    public void StartsWith_filter_only_suggests_items_starting_with_the_text()
    {
        var sut = Create();
        sut.FilterMode = TokenFilterMode.StartsWith;

        sut.Text = "ing";
        Assert.Empty(sut.Suggestions);

        sut.Text = "sh";
        Assert.Equal(["Shipping"], sut.Suggestions.Select(x => x.DisplayValue).ToList());
        sut.SetCurrentValue(TokenBox.IsDropDownOpenProperty, false);
    }

    [StaFact]
    public void Custom_filter_decides_and_MaxSuggestions_limits_the_rows()
    {
        var sut = Create();
        sut.FilterMode = TokenFilterMode.Custom;
        sut.CustomFilter = (_, pair) => pair.Key.Length > 6;

        sut.Text = "x";
        Assert.Equal(["Payments", "Shipping", "Billing", "Returns"], sut.Suggestions.Select(x => x.DisplayValue).ToList());

        sut.MaxSuggestions = 2;
        sut.Text = "xy";
        Assert.Equal(2, sut.Suggestions.Count);
        sut.SetCurrentValue(TokenBox.IsDropDownOpenProperty, false);
    }

    [StaFact]
    public void Enter_picks_the_highlighted_suggestion_with_one_event()
    {
        var sut = Create();
        var events = Record(sut);

        sut.Text = "or";
        var handled = sut.HandleKey(Key.Enter, ModifierKeys.None);

        Assert.True(handled);
        Assert.Equal(["orders"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Empty(change.OldValue);
        Assert.Equal(["orders"], change.NewValue);
        Assert.Equal(string.Empty, sut.Text);
        Assert.False(sut.IsDropDownOpen);
        Assert.Equal(["Orders"], sut.GetTokenItems().Select(x => x.DisplayValue).ToList());
    }

    [StaFact]
    public void Arrow_keys_move_the_highlight_and_Tab_picks_it()
    {
        var sut = Create();
        var events = Record(sut);

        sut.Text = "i";
        Assert.Equal(["Shipping", "Billing"], sut.Suggestions.Select(x => x.DisplayValue).ToList());

        Assert.True(sut.HandleKey(Key.Down, ModifierKeys.None));
        Assert.Equal(1, sut.HighlightedIndex);
        Assert.True(sut.HandleKey(Key.Down, ModifierKeys.None));
        Assert.Equal(1, sut.HighlightedIndex);
        Assert.True(sut.HandleKey(Key.Up, ModifierKeys.None));
        Assert.True(sut.HandleKey(Key.Down, ModifierKeys.None));

        Assert.True(sut.HandleKey(Key.Tab, ModifierKeys.None));

        Assert.Equal(["billing"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Tab_without_a_highlighted_suggestion_is_not_used()
    {
        var sut = Create();

        Assert.False(sut.HandleKey(Key.Tab, ModifierKeys.None));

        sut.Text = "zzz";
        Assert.False(sut.HandleKey(Key.Tab, ModifierKeys.None));
        Assert.Empty(sut.SelectedKeys);
        sut.SetCurrentValue(TokenBox.IsDropDownOpenProperty, false);
    }

    [StaFact]
    public void A_separator_picks_the_highlighted_suggestion_with_one_event()
    {
        var sut = Create();
        var events = Record(sut);

        sut.Text = "pay";
        var handled = sut.HandleTextInput(";");

        Assert.True(handled);
        Assert.Equal(["payments"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.Equal(string.Empty, sut.Text);
    }

    [StaFact]
    public void A_separator_in_the_text_commits_the_parts_before_it()
    {
        var sut = Create();
        var events = Record(sut);

        sut.Text = "orders, bill";

        Assert.Equal(["orders"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.Equal("bill", sut.Text);
        sut.SetCurrentValue(TokenBox.IsDropDownOpenProperty, false);
    }

    [StaFact]
    public void Clicking_a_suggestion_picks_it_with_one_event()
    {
        var sut = Create();
        var events = Record(sut);

        sut.Text = "s";
        var shipping = sut.Suggestions.Single(x => x.Key == "shipping");
        sut.PickSuggestion(shipping);

        Assert.Equal(["shipping"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.False(sut.IsDropDownOpen);
    }

    [StaFact]
    public void Selecting_a_suggestion_row_from_outside_picks_it()
    {
        var sut = Create();
        var events = Record(sut);

        sut.Text = "s";
        sut.PartSuggestionList.SelectedIndex = 1;

        Assert.Single(sut.SelectedKeys);
        Assert.Single(events);
        Assert.Equal(string.Empty, sut.Text);
    }

    [StaFact]
    public void Paste_splits_on_separators_and_new_lines_with_one_event_and_ignores_duplicates()
    {
        var sut = new TokenBox
        {
            Items = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["a"] = "Alpha",
                ["b"] = "Beta",
                ["c"] = "Gamma",
            },
        };
        var events = Record(sut);

        sut.Paste("alpha; beta\r\ngamma, ALPHA");

        Assert.Equal(["a", "b", "c"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Equal(["a", "b", "c"], change.NewValue);
        Assert.Equal(string.Empty, sut.Text);
    }

    [StaFact]
    public void Paste_with_AllowCreate_creates_the_values_with_one_event()
    {
        var sut = new TokenBox { AllowCreate = true, AddNewValuesToItems = true };
        var events = Record(sut);

        sut.Paste("a; b\r\nc, a");

        Assert.Equal(["a", "b", "c"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.Equal("c", sut.Items["c"]);
    }

    [StaFact]
    public void Paste_without_AllowCreate_leaves_the_unknown_parts_in_the_text()
    {
        var sut = Create();
        var events = Record(sut);

        sut.Paste("orders, nope; billing");

        Assert.Equal(["orders", "billing"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.Equal("nope", sut.Text);
        sut.SetCurrentValue(TokenBox.IsDropDownOpenProperty, false);
    }

    [StaFact]
    public void AllowCreate_off_ignores_unknown_text()
    {
        var sut = Create();
        var events = Record(sut);

        sut.Text = "unknown";
        sut.HandleKey(Key.Enter, ModifierKeys.None);

        Assert.Empty(sut.SelectedKeys);
        Assert.Empty(events);
        Assert.Equal("unknown", sut.Text);
        sut.SetCurrentValue(TokenBox.IsDropDownOpenProperty, false);
    }

    [StaFact]
    public void AllowCreate_on_creates_a_token_keyed_by_its_text_and_adds_it_to_Items()
    {
        var sut = Create();
        sut.AllowCreate = true;
        sut.AddNewValuesToItems = true;
        var events = Record(sut);

        sut.Text = "Ord";
        Assert.Equal(-1, sut.HighlightedIndex);
        sut.HandleKey(Key.Enter, ModifierKeys.None);

        Assert.Equal(["Ord"], sut.SelectedKeys);
        Assert.Equal("Ord", sut.Items["Ord"]);
        Assert.Single(events);
        Assert.Equal(string.Empty, sut.Text);
    }

    [StaFact]
    public void AllowCreate_still_picks_an_item_whose_name_is_typed()
    {
        var sut = Create();
        sut.AllowCreate = true;

        sut.Text = "orders";
        Assert.Equal(0, sut.HighlightedIndex);
        sut.HandleKey(Key.Enter, ModifierKeys.None);

        Assert.Equal(["orders"], sut.SelectedKeys);
        Assert.Equal(5, sut.Items.Count);
    }

    [StaFact]
    public void CreateValidator_rejects_a_value_and_keeps_the_text_with_an_error()
    {
        var sut = new TokenBox
        {
            AllowCreate = true,
            CreateValidator = x => x.Contains('@', StringComparison.Ordinal) ? null : "Not an e-mail",
        };
        var events = Record(sut);

        sut.Text = "john";
        sut.HandleKey(Key.Enter, ModifierKeys.None);

        Assert.Empty(sut.SelectedKeys);
        Assert.Empty(events);
        Assert.Equal("john", sut.Text);
        Assert.Equal("Not an e-mail", sut.ErrorText);
        Assert.True(sut.HasError);

        sut.Text = "john@contoso.com";
        Assert.False(sut.HasError);
        sut.HandleKey(Key.Enter, ModifierKeys.None);

        Assert.Equal(["john@contoso.com"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void TokenCreating_can_set_the_key_or_cancel()
    {
        var sut = new TokenBox { AllowCreate = true, AddNewValuesToItems = true };
        var events = Record(sut);
        sut.TokenCreating += (_, e) =>
        {
            if (e.Text == "bad")
            {
                e.Cancel = true;
                return;
            }

            e.Key = "tag:" + e.Text;
        };

        sut.Paste("good, bad");

        Assert.Equal(["tag:good"], sut.SelectedKeys);
        Assert.Equal("good", sut.Items["tag:good"]);
        Assert.Single(events);
        Assert.Equal("bad", sut.Text);
        Assert.Equal(["good"], sut.GetTokenItems().Select(x => x.DisplayValue).ToList());
    }

    [StaFact]
    public void Backspace_twice_removes_the_last_token_skipping_fixed_ones()
    {
        var sut = Create();
        sut.SelectedKeys = ["orders", "payments", "shipping"];
        sut.FixedKeys.Add("shipping");
        var events = Record(sut);

        Assert.True(sut.HandleKey(Key.Back, ModifierKeys.None));

        Assert.Equal(1, sut.SelectedTokenIndex);
        Assert.Empty(events);

        Assert.True(sut.HandleKey(Key.Back, ModifierKeys.None));

        Assert.Equal(["orders", "shipping"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Equal(["orders", "payments", "shipping"], change.OldValue);
        Assert.Equal(-1, sut.SelectedTokenIndex);
        Assert.False(sut.GetTokenItems().Single(x => x.Key == "shipping").CanRemove);
    }

    [StaFact]
    public void Backspace_with_text_is_left_to_the_text_box()
    {
        var sut = Create();
        sut.SelectedKeys = ["orders"];
        sut.Text = "x";
        sut.PartTextBox.CaretIndex = 1;

        Assert.False(sut.HandleKey(Key.Back, ModifierKeys.None));
        Assert.Equal(["orders"], sut.SelectedKeys);
        sut.SetCurrentValue(TokenBox.IsDropDownOpenProperty, false);
    }

    [StaFact]
    public void Arrow_keys_move_across_tokens_and_Delete_removes_the_selected_one()
    {
        var sut = Create();
        sut.SelectedKeys = ["orders", "payments", "shipping"];
        var events = Record(sut);

        Assert.True(sut.HandleKey(Key.Left, ModifierKeys.None));
        Assert.Equal(2, sut.SelectedTokenIndex);
        Assert.True(sut.HandleKey(Key.Left, ModifierKeys.None));
        Assert.True(sut.HandleKey(Key.Left, ModifierKeys.None));
        Assert.True(sut.HandleKey(Key.Left, ModifierKeys.None));
        Assert.Equal(0, sut.SelectedTokenIndex);
        Assert.True(sut.HandleKey(Key.Right, ModifierKeys.None));
        Assert.Equal(1, sut.SelectedTokenIndex);

        Assert.True(sut.HandleKey(Key.Delete, ModifierKeys.None));

        Assert.Equal(["orders", "shipping"], sut.SelectedKeys);
        Assert.Single(events);

        Assert.True(sut.HandleKey(Key.Left, ModifierKeys.None));
        Assert.True(sut.HandleKey(Key.Right, ModifierKeys.None));
        Assert.Equal(-1, sut.SelectedTokenIndex);
    }

    [StaFact]
    public void Arrow_keys_across_tokens_are_mirrored_in_right_to_left()
    {
        var sut = Create();
        sut.FlowDirection = FlowDirection.RightToLeft;
        sut.SelectedKeys = ["orders", "payments", "shipping"];

        Assert.False(sut.HandleKey(Key.Left, ModifierKeys.None));
        Assert.Equal(-1, sut.SelectedTokenIndex);

        Assert.True(sut.HandleKey(Key.Right, ModifierKeys.None));
        Assert.Equal(2, sut.SelectedTokenIndex);
        Assert.True(sut.HandleKey(Key.Right, ModifierKeys.None));
        Assert.Equal(1, sut.SelectedTokenIndex);
        Assert.True(sut.HandleKey(Key.Left, ModifierKeys.None));
        Assert.Equal(2, sut.SelectedTokenIndex);
    }

    [StaFact]
    public void Escape_closes_the_suggestions_then_clears_the_text()
    {
        var sut = Create();
        sut.Text = "or";
        Assert.True(sut.IsDropDownOpen);

        Assert.True(sut.HandleKey(Key.Escape, ModifierKeys.None));
        Assert.False(sut.IsDropDownOpen);
        Assert.Equal("or", sut.Text);

        Assert.True(sut.HandleKey(Key.Escape, ModifierKeys.None));
        Assert.Equal(string.Empty, sut.Text);

        Assert.False(sut.HandleKey(Key.Escape, ModifierKeys.None));
    }

    [StaFact]
    public void Removing_a_token_raises_one_event_and_fixed_tokens_stay()
    {
        var sut = Create();
        sut.SelectedKeys = ["orders", "payments"];
        sut.FixedKeys.Add("orders");
        var events = Record(sut);

        sut.GetTokenItems().Single(x => x.Key == "payments").RemoveCommand.Execute(parameter: null);
        sut.RemoveKey("orders");

        Assert.Equal(["orders"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void ClearTokens_removes_every_token_that_is_not_fixed_with_one_event()
    {
        var sut = Create();
        sut.ShowClearButton = true;
        sut.SelectedKeys = ["orders", "payments", "shipping"];
        sut.FixedKeys.Add("payments");
        var events = Record(sut);

        Assert.Equal(Visibility.Visible, sut.PartClearButton.Visibility);

        sut.ClearTokens();

        Assert.Equal(["payments"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.Equal(Visibility.Collapsed, sut.PartClearButton.Visibility);
    }

    [StaFact]
    public void The_limit_blocks_typing_and_shows_the_count()
    {
        var sut = Create();
        sut.MaxSelectionCount = 2;
        var events = Record(sut);

        sut.Paste("orders, payments, shipping");

        Assert.Equal(["orders", "payments"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.True(sut.IsAtLimit);
        Assert.True(sut.PartTextBox.IsReadOnly);
        Assert.True(sut.HandleTextInput("x"));
        Assert.Equal(
            string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.CountOfMax2, 2, 2),
            sut.PlaceholderText);

        sut.RemoveKey("orders");

        Assert.False(sut.IsAtLimit);
        Assert.False(sut.PartTextBox.IsReadOnly);
        Assert.False(sut.HandleTextInput("x"));
        Assert.Equal(string.Empty, sut.PlaceholderText);
    }

    [StaFact]
    public void The_watermark_shows_only_while_there_are_no_tokens()
    {
        var sut = Create();
        TextBoxHelper.SetWatermark(sut, "Assign people");

        Assert.Equal("Assign people", sut.PlaceholderText);

        sut.SelectedKeys.Add("orders");
        Assert.Equal(string.Empty, sut.PlaceholderText);

        sut.SelectedKeys.Clear();
        Assert.Equal("Assign people", sut.PlaceholderText);
    }

    [StaFact]
    public void MaxVisibleTokens_shows_a_more_chip_when_not_focused()
    {
        var sut = CreateStyled();
        sut.MaxVisibleTokens = 2;
        sut.SelectedKeys = ["orders", "payments", "shipping", "billing", "returns"];

        Layout(sut, 600);

        Assert.Equal(2, sut.ChipPanel.VisibleCount);
        Assert.Equal(3, sut.ChipPanel.HiddenCount);
        Assert.Equal(
            string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.MoreCount1, 3),
            sut.OverflowText);
        Assert.Equal("Shipping, Billing, Returns", System.Windows.Automation.AutomationProperties.GetName(sut.OverflowButton));

        sut.TokensWrap = false;
        Layout(sut, 600);

        Assert.Equal(2, sut.ChipPanel.VisibleCount);
        Assert.Equal(3, sut.ChipPanel.HiddenCount);

        sut.MaxVisibleTokens = null;
        Layout(sut, 600);

        Assert.Equal(5, sut.ChipPanel.VisibleCount);
        Assert.Equal(0, sut.ChipPanel.HiddenCount);
    }

    [StaFact]
    public void Tokens_wrap_onto_more_lines_and_the_field_grows()
    {
        var sut = CreateStyled();
        sut.SelectedKeys = ["orders", "payments", "shipping", "billing", "returns"];

        Layout(sut, 600);
        var oneLineHeight = sut.ActualHeight;

        Layout(sut, 120);

        Assert.Equal(5, sut.ChipPanel.VisibleCount);
        Assert.True(sut.ActualHeight > oneLineHeight);
    }

    [StaFact]
    public void Items_dropping_a_selected_key_removes_the_token_with_one_event()
    {
        var sut = Create();
        sut.SelectedKeys = ["orders", "payments"];
        var events = Record(sut);

        sut.Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["orders"] = "Orders" };

        Assert.Equal(["orders"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Items_change_keeps_created_values_but_drops_known_items_when_AllowCreate_is_set()
    {
        var sut = Create();
        sut.AllowCreate = true;
        sut.SelectedKeys = ["orders", "free"];
        sut.Paste("brand new,");
        var events = Record(sut);

        sut.Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["payments"] = "Payments" };

        Assert.Equal(["free", "brand new"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Equal(["orders", "free", "brand new"], change.OldValue);
        Assert.Equal(["free", "brand new"], sut.GetTokenItems().Select(x => x.DisplayValue).ToList());
    }

    [StaFact]
    public void Editing_a_created_token_replaces_it_in_place_with_one_event()
    {
        var sut = Create();
        sut.AllowCreate = true;
        sut.AddNewValuesToItems = true;
        sut.AllowEditTokens = true;
        sut.SelectedKeys = ["orders"];
        sut.Paste("drafts, misc,");
        var events = Record(sut);

        Assert.False(sut.BeginEditToken("orders"));
        Assert.True(sut.BeginEditToken("drafts"));
        Assert.NotNull(sut.EditBox);
        Assert.Equal("drafts", sut.EditBox!.Text);

        sut.EditBox.Text = "archive";
        sut.CommitEditToken();

        Assert.Equal(["orders", "archive", "misc"], sut.SelectedKeys);
        Assert.Single(events);
        Assert.False(sut.Items.ContainsKey("drafts"));
        Assert.Equal("archive", sut.Items["archive"]);
        Assert.Null(sut.EditingKey);
    }

    [StaFact]
    public void Escape_cancels_editing_a_token()
    {
        var sut = new TokenBox { AllowCreate = true, AllowEditTokens = true };
        sut.Paste("drafts,");
        var events = Record(sut);

        Assert.True(sut.BeginEditToken("drafts"));
        sut.EditBox!.Text = "other";
        Assert.True(sut.HandleKey(Key.Escape, ModifierKeys.None));

        Assert.Equal(["drafts"], sut.SelectedKeys);
        Assert.Empty(events);
        Assert.Null(sut.EditingKey);
    }

    [StaFact]
    public void SuggestionsProvider_is_debounced_cancels_the_previous_request_and_shows_the_results()
    {
        var calls = new List<(string Text, CancellationToken Token)>();
        var first = new TaskCompletionSource<IReadOnlyList<KeyValuePair<string, string>>>();
        var sut = new TokenBox
        {
            SuggestionDelay = TimeSpan.FromMilliseconds(80),
            SuggestionsProvider = (text, token) =>
            {
                calls.Add((text, token));
                return calls.Count == 1
                    ? first.Task
                    : Task.FromResult<IReadOnlyList<KeyValuePair<string, string>>>(
                    [
                        new("u1", text + " one"),
                        new("u2", text + " two"),
                    ]);
            },
        };

        sut.Text = "j";
        sut.Text = "je";
        sut.Text = "jen";
        Assert.True(sut.IsBusy);
        Assert.Equal(Atc.Wpf.Resources.Miscellaneous.PleaseWait3Dots, sut.StatusText);
        Assert.Empty(calls);

        Pump(TimeSpan.FromMilliseconds(400));

        var call = Assert.Single(calls);
        Assert.Equal("jen", call.Text);
        Assert.True(sut.IsBusy);

        sut.Text = "jenn";
        Assert.True(call.Token.IsCancellationRequested);

        Pump(TimeSpan.FromMilliseconds(400));
        first.SetResult([new("stale", "stale result")]);
        Pump(TimeSpan.FromMilliseconds(50));

        Assert.Equal(2, calls.Count);
        Assert.False(sut.IsBusy);
        Assert.Equal(["jenn one", "jenn two"], sut.Suggestions.Select(x => x.DisplayValue).ToList());

        sut.HandleKey(Key.Enter, ModifierKeys.None);

        Assert.Equal(["u1"], sut.SelectedKeys);
        Assert.Equal(["jenn one"], sut.GetTokenItems().Select(x => x.DisplayValue).ToList());
        Assert.False(sut.Items.ContainsKey("u1"));
    }

    [StaFact]
    public void SuggestionsProvider_throwing_its_own_cancellation_ends_the_busy_state_and_shows_no_matches()
    {
        var sut = new TokenBox
        {
            SuggestionDelay = TimeSpan.FromMilliseconds(20),
            SuggestionsProvider = (_, _) => throw new TaskCanceledException("The request timed out."),
        };

        sut.Text = "jen";
        Assert.True(sut.IsBusy);

        Pump(TimeSpan.FromMilliseconds(200));

        Assert.False(sut.IsBusy);
        Assert.Empty(sut.Suggestions);
        Assert.Equal(Atc.Wpf.Resources.Miscellaneous.NoMatches, sut.StatusText);
    }

    [StaFact]
    public void Automation_peer_is_a_combo_box_with_the_typed_text_as_value_and_a_token_list()
    {
        var sut = CreateStyled();
        sut.SelectedKeys = ["orders", "payments"];
        sut.FixedKeys.Add("orders");
        Layout(sut, 600);

        var peer = (TokenBoxAutomationPeer)UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.Equal(AutomationControlType.ComboBox, peer.GetAutomationControlType());
        var value = (IValueProvider)peer.GetPattern(PatternInterface.Value)!;
        Assert.Equal(string.Empty, value.Value);
        Assert.NotNull(peer.GetPattern(PatternInterface.ExpandCollapse));

        var children = peer.GetChildren();
        var list = children.First();
        Assert.Equal(AutomationControlType.List, list.GetAutomationControlType());
        Assert.Contains(children, x => x.GetAutomationControlType() == AutomationControlType.Edit);

        var items = list.GetChildren();
        Assert.Equal(["Orders", "Payments"], items.Select(x => x.GetName()).ToList());
        Assert.All(items, x => Assert.Equal(AutomationControlType.ListItem, x.GetAutomationControlType()));
        Assert.Null(items[0].GetChildren());
        var remove = Assert.Single(items[1].GetChildren());
        Assert.Equal(AutomationControlType.Button, remove.GetAutomationControlType());
        Assert.Equal(
            string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.Remove1, "Payments"),
            remove.GetName());

        ((IInvokeProvider)remove.GetPattern(PatternInterface.Invoke)!).Invoke();
        Pump(TimeSpan.FromMilliseconds(50));

        Assert.Equal(["orders"], sut.SelectedKeys);
    }

    [StaFact]
    public void Automation_value_with_separators_is_handled_as_a_paste()
    {
        var sut = Create();
        var events = Record(sut);
        var peer = (TokenBoxAutomationPeer)UIElementAutomationPeer.CreatePeerForElement(sut);

        peer.SetValue("or");
        Assert.Equal("or", sut.Text);
        Assert.True(sut.IsDropDownOpen);
        Assert.Equal(System.Windows.Automation.ExpandCollapseState.Expanded, peer.ExpandCollapseState);

        sut.HandleKey(Key.Enter, ModifierKeys.None);
        peer.SetValue("payments, billing");

        Assert.Equal(["orders", "payments", "billing"], sut.SelectedKeys);
        Assert.Equal(2, events.Count);
        Assert.Equal(string.Empty, peer.Value);
    }

    [StaFact]
    public void MinimumPrefixLength_zero_suggests_everything_when_expanded()
    {
        var sut = Create();
        sut.MinimumPrefixLength = 0;
        var peer = (TokenBoxAutomationPeer)UIElementAutomationPeer.CreatePeerForElement(sut);

        peer.Expand();

        Assert.True(sut.IsDropDownOpen);
        Assert.Equal(5, sut.Suggestions.Count);

        peer.Collapse();
        Assert.False(sut.IsDropDownOpen);
    }

    [StaFact]
    public void The_suggestions_hold_the_mouse_capture_and_close_on_a_click_outside()
    {
        var sut = Create();
        var window = CreateWindow(sut);
        window.Show();
        try
        {
            sut.Text = "ing";
            Assert.True(sut.IsDropDownOpen);
            Assert.Same(sut, Mouse.Captured);

            sut.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = Mouse.PreviewMouseDownOutsideCapturedElementEvent,
            });

            Assert.False(sut.IsDropDownOpen);
            Assert.Null(Mouse.Captured);
        }
        finally
        {
            Mouse.Capture(null);
            window.Close();
        }
    }

    [StaFact]
    public void The_suggestions_close_when_a_scroll_viewer_around_the_box_scrolls()
    {
        var sut = Create();
        var scrollViewer = new ScrollViewer
        {
            Content = new StackPanel { Children = { sut, new Border { Height = 2000 } } },
        };
        var window = CreateWindow(scrollViewer);
        window.Show();
        try
        {
            window.UpdateLayout();
            sut.Text = "ing";
            Assert.True(sut.IsDropDownOpen);

            scrollViewer.ScrollToVerticalOffset(100);
            window.UpdateLayout();

            Assert.False(sut.IsDropDownOpen);
        }
        finally
        {
            Mouse.Capture(null);
            window.Close();
        }
    }

    private static Window CreateWindow(UIElement content)
        => new()
        {
            Content = content,
            Width = 300,
            Height = 200,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };

    [StaFact]
    public void Created_values_leave_a_shared_Items_catalog_unchanged_by_default()
    {
        var catalog = new Dictionary<string, string>(StringComparer.Ordinal) { ["orders"] = "Orders" };
        var first = new TokenBox { AllowCreate = true, Items = catalog };
        var second = new TokenBox { AllowCreate = true, Items = catalog };
        var creating = 0;
        first.TokenCreating += (_, _) => creating++;

        first.Text = "ledger";
        first.HandleKey(Key.Enter, ModifierKeys.None);

        Assert.Equal(1, creating);
        Assert.Equal(["ledger"], first.SelectedKeys);
        Assert.Equal(["ledger"], first.GetTokenItems().Select(x => x.DisplayValue).ToList());
        Assert.Equal(["orders"], catalog.Keys.ToList());

        second.Text = "led";
        Assert.DoesNotContain(second.Suggestions, x => string.Equals(x.Key, "ledger", StringComparison.Ordinal));
    }

    [StaFact]
    public void AddNewValuesToItems_writes_created_values_into_Items()
    {
        var catalog = new Dictionary<string, string>(StringComparer.Ordinal) { ["orders"] = "Orders" };
        var sut = new TokenBox { AllowCreate = true, AddNewValuesToItems = true, Items = catalog };

        sut.Text = "ledger";
        sut.HandleKey(Key.Enter, ModifierKeys.None);

        Assert.Equal("ledger", catalog["ledger"]);
    }

    private static TokenBox Create()
        => new()
        {
            Items = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["orders"] = "Orders",
                ["payments"] = "Payments",
                ["shipping"] = "Shipping",
                ["billing"] = "Billing",
                ["returns"] = "Returns",
            },
        };

    private static TokenBox CreateStyled()
    {
        var resources = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls;component/DataDisplay/Chip.xaml"),
        };

        var sut = Create();
        sut.Resources.MergedDictionaries.Add(resources);
        sut.Resources[typeof(Chip)] = resources["AtcApps.Styles.Chip"];
        return sut;
    }

    private static void Layout(
        TokenBox sut,
        double width)
    {
        sut.Width = width;
        sut.Measure(new Size(width, double.PositiveInfinity));
        sut.Arrange(new Rect(sut.DesiredSize));
        sut.UpdateLayout();
    }

    private static void Pump(TimeSpan duration)
    {
        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = duration,
        };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            frame.Continue = false;
        };
        timer.Start();
        Dispatcher.PushFrame(frame);
    }

    private static List<ValueChangedEventArgs<IReadOnlyList<string>>> Record(
        TokenBox sut)
    {
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);
        return events;
    }
}