// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <content>
/// The suggestion pop-up: filtering <see cref="Items"/>, the debounced <see cref="SuggestionsProvider"/>, the
/// highlighted row and picking.
/// </content>
public partial class TokenBox
{
    private IReadOnlyList<TokenSuggestion> suggestions = [];
    private IReadOnlyList<KeyValuePair<string, string>> lastProviderResults = [];
    private DispatcherTimer? lookupTimer;
    private CancellationTokenSource? lookupCancellation;
    private string? pendingLookupText;
    private bool isFillingSuggestions;
    private bool isSettingHighlight;

    /// <summary>
    /// Gets the suggestions shown in the pop-up.
    /// </summary>
    internal IReadOnlyList<TokenSuggestion> Suggestions => suggestions;

    /// <summary>
    /// Gets a value indicating whether a <see cref="SuggestionsProvider"/> lookup is waiting or running.
    /// </summary>
    internal bool IsBusy { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the control is writing its own change into <c>SelectedKeys</c>; a wrapper that
    /// shares the collection validates once afterwards instead of once per notification.
    /// </summary>
    internal bool IsWritingSelectedKeys => isWritingSelectedKeys;

    /// <summary>
    /// Gets or sets the index of the highlighted suggestion; -1 for none.
    /// </summary>
    internal int HighlightedIndex
    {
        get => PartSuggestionList.SelectedIndex;
        set
        {
            isSettingHighlight = true;
            try
            {
                PartSuggestionList.SelectedIndex = value < 0 || value >= suggestions.Count ? -1 : value;
                if (PartSuggestionList.SelectedItem is { } item)
                {
                    PartSuggestionList.ScrollIntoView(item);
                }
            }
            finally
            {
                isSettingHighlight = false;
            }
        }
    }

    /// <summary>
    /// Gets the highlighted suggestion, or <see langword="null"/>.
    /// </summary>
    internal TokenSuggestion? HighlightedSuggestion
        => PartSuggestionList.SelectedItem as TokenSuggestion;

    /// <summary>
    /// Gets the status line of the pop-up ("Please wait...", "No matches"), or empty when hidden.
    /// </summary>
    internal string StatusText
        => PartStatusText.Visibility == Visibility.Visible
            ? PartStatusText.Text
            : string.Empty;

    /// <summary>
    /// Picks a suggestion: it becomes a token, the text is cleared and the pop-up closes.
    /// </summary>
    /// <returns><see langword="true"/> when a token was added.</returns>
    internal bool PickSuggestion(TokenSuggestion suggestion)
    {
        ArgumentNullException.ThrowIfNull(suggestion);

        if (IsAtLimit)
        {
            return false;
        }

        var added = AddTokens([new KeyValuePair<string, string>(suggestion.Key, suggestion.DisplayValue)]);
        SetText(string.Empty);
        ErrorText = string.Empty;
        SetCurrentValue(IsDropDownOpenProperty, false);
        ClearSuggestions();
        return added;
    }

    /// <summary>
    /// Refreshes the suggestions for the typed text and opens or closes the pop-up.
    /// </summary>
    /// <param name="force">Show suggestions even when the text is shorter than <see cref="MinimumPrefixLength"/>.</param>
    internal void UpdateSuggestions(bool force)
    {
        var typed = (Text ?? string.Empty).Trim();
        if (IsAtLimit || !IsEnabled || EditingKey is not null || (!force && typed.Length < System.Math.Max(0, MinimumPrefixLength)))
        {
            CancelLookup();
            ClearSuggestions();
            SetCurrentValue(IsDropDownOpenProperty, false);
            return;
        }

        if (SuggestionsProvider is not null)
        {
            ScheduleLookup(typed);
            return;
        }

        FillSuggestions(FilterItems(typed), typed);
    }

    private List<KeyValuePair<string, string>> FilterItems(string typed)
    {
        var result = new List<KeyValuePair<string, string>>();
        var source = Items;
        var max = System.Math.Max(0, MaxSuggestions);
        if (source is null || max == 0)
        {
            return result;
        }

        var selectedKeySet = new HashSet<string>(tokenItems.Select(x => x.Key), StringComparer.Ordinal);
        var selectedDisplays = new HashSet<string>(tokenItems.Select(x => x.DisplayValue), StringComparer.OrdinalIgnoreCase);
        var mode = FilterMode;
        var filter = CustomFilter;
        foreach (var pair in source)
        {
            var display = pair.Value ?? string.Empty;
            if (selectedKeySet.Contains(pair.Key) || selectedDisplays.Contains(display))
            {
                continue;
            }

            var isMatch = mode switch
            {
                TokenFilterMode.StartsWith => display.StartsWith(typed, StringComparison.OrdinalIgnoreCase),
                TokenFilterMode.Custom => filter?.Invoke(typed, pair) ?? KeySelectionHelper.MatchesFilter(display, typed),
                _ => KeySelectionHelper.MatchesFilter(display, typed),
            };
            if (!isMatch)
            {
                continue;
            }

            result.Add(new KeyValuePair<string, string>(pair.Key, display));
            if (result.Count >= max)
            {
                break;
            }
        }

        return result;
    }

    private void FillSuggestions(
        IReadOnlyList<KeyValuePair<string, string>> pairs,
        string typed)
    {
        isFillingSuggestions = true;
        try
        {
            suggestions = pairs
                .Select(x => new TokenSuggestion(x.Key, x.Value, typed))
                .ToList();
            PartSuggestionList.ItemsSource = suggestions;

            // Without AllowCreate the first row is highlighted, so Enter picks it; with AllowCreate only an exact match
            // is, so Enter creates the typed value instead of picking a longer one.
            var highlight = -1;
            if (suggestions.Count > 0)
            {
                highlight = AllowCreate
                    ? suggestions
                        .Select((x, i) => (x, i))
                        .Where(x => string.Equals(x.x.DisplayValue, typed, StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.i)
                        .DefaultIfEmpty(-1)
                        .First()
                    : 0;
            }

            HighlightedIndex = highlight;
            UpdateStatusText();

            var showPopup = suggestions.Count > 0 || PartStatusText.Visibility == Visibility.Visible;
            SetCurrentValue(IsDropDownOpenProperty, showPopup);
            PartPopup.IsOpen = showPopup;
            if (UIElementAutomationPeer.FromElement(PartSuggestionList) is { } listPeer)
            {
                listPeer.ResetChildrenCache();
            }
        }
        finally
        {
            isFillingSuggestions = false;
        }
    }

    private void ClearSuggestions()
    {
        if (suggestions.Count == 0 && PartStatusText.Visibility != Visibility.Visible)
        {
            return;
        }

        suggestions = [];
        PartSuggestionList.ItemsSource = suggestions;
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        if (IsBusy)
        {
            PartStatusText.Text = Atc.Wpf.Resources.Miscellaneous.PleaseWait3Dots;
            PartStatusText.Visibility = Visibility.Visible;
            return;
        }

        var typed = (Text ?? string.Empty).Trim();
        if (suggestions.Count == 0 && typed.Length > 0 && !AllowCreate && IsDropDownOpenOrFilling)
        {
            PartStatusText.Text = Atc.Wpf.Resources.Miscellaneous.NoMatches;
            PartStatusText.Visibility = Visibility.Visible;
            return;
        }

        PartStatusText.Visibility = Visibility.Collapsed;
    }

    private bool IsDropDownOpenOrFilling
        => IsDropDownOpen || isFillingSuggestions;

    private bool MoveHighlight(int delta)
    {
        if (IsAtLimit || EditingKey is not null)
        {
            return false;
        }

        if (!IsDropDownOpen || suggestions.Count == 0)
        {
            UpdateSuggestions(force: true);
            return true;
        }

        var index = HighlightedIndex;
        index = index < 0
            ? (delta > 0 ? 0 : suggestions.Count - 1)
            : System.Math.Clamp(index + delta, 0, suggestions.Count - 1);
        HighlightedIndex = index;
        return true;
    }

    private void ScheduleLookup(string typed)
    {
        CancelLookup();
        IsBusy = true;
        pendingLookupText = typed;
        isFillingSuggestions = true;
        try
        {
            suggestions = [];
            PartSuggestionList.ItemsSource = suggestions;
            UpdateStatusText();
            SetCurrentValue(IsDropDownOpenProperty, true);
            PartPopup.IsOpen = true;
        }
        finally
        {
            isFillingSuggestions = false;
        }

        var delay = SuggestionDelay;
        if (delay <= TimeSpan.Zero)
        {
            _ = RunLookupAsync(typed);
            return;
        }

        lookupTimer = new DispatcherTimer(DispatcherPriority.Background, Dispatcher)
        {
            Interval = delay,
        };
        lookupTimer.Tick += OnLookupTimerTick;
        lookupTimer.Start();
    }

    private void OnLookupTimerTick(
        object? sender,
        EventArgs e)
    {
        StopLookupTimer();
        if (pendingLookupText is { } typed)
        {
            _ = RunLookupAsync(typed);
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A failing lookup shows no suggestions instead of crashing the UI.")]
    private async Task RunLookupAsync(string typed)
    {
        var provider = SuggestionsProvider;
        if (provider is null)
        {
            return;
        }

        var cancellation = new CancellationTokenSource();
        lookupCancellation = cancellation;
        IReadOnlyList<KeyValuePair<string, string>> found;
        try
        {
            found = await provider(typed, cancellation.Token).ConfigureAwait(true) ?? [];
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            // Our own cancellation: a newer lookup or CancelLookup owns the busy state.
            return;
        }
        catch (Exception)
        {
            found = [];
        }

        // Providers may ignore the token: a result for an older text is dropped.
        if (cancellation.IsCancellationRequested ||
            !ReferenceEquals(lookupCancellation, cancellation) ||
            !string.Equals((Text ?? string.Empty).Trim(), typed, StringComparison.Ordinal))
        {
            return;
        }

        lookupCancellation = null;
        cancellation.Dispose();
        IsBusy = false;
        pendingLookupText = null;
        ShowLookupResult(found, typed);
    }

    private void ShowLookupResult(
        IReadOnlyList<KeyValuePair<string, string>> found,
        string typed)
    {
        lastProviderResults = found;
        var selectedKeySet = new HashSet<string>(tokenItems.Select(x => x.Key), StringComparer.Ordinal);
        var selectedDisplays = new HashSet<string>(tokenItems.Select(x => x.DisplayValue), StringComparer.OrdinalIgnoreCase);
        var pairs = found
            .Where(x => !selectedKeySet.Contains(x.Key) && !selectedDisplays.Contains(x.Value ?? string.Empty))
            .Take(System.Math.Max(0, MaxSuggestions))
            .Select(x => new KeyValuePair<string, string>(x.Key, x.Value ?? string.Empty))
            .ToList();
        FillSuggestions(pairs, typed);
    }

    private void StopLookupTimer()
    {
        if (lookupTimer is null)
        {
            return;
        }

        lookupTimer.Stop();
        lookupTimer.Tick -= OnLookupTimerTick;
        lookupTimer = null;
    }

    /// <summary>
    /// Stops a waiting lookup and cancels a running one.
    /// </summary>
    private void CancelLookup()
    {
        StopLookupTimer();
        pendingLookupText = null;
        if (lookupCancellation is not null)
        {
            lookupCancellation.Cancel();
            lookupCancellation.Dispose();
            lookupCancellation = null;
        }

        if (IsBusy)
        {
            IsBusy = false;
            UpdateStatusText();
        }
    }

    private void OnSuggestionListPreviewMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        var element = e.OriginalSource as DependencyObject;
        while (element is not null && element is not ListBoxItem)
        {
            element = element is Visual or System.Windows.Media.Media3D.Visual3D
                ? VisualTreeHelper.GetParent(element)
                : LogicalTreeHelper.GetParent(element);
        }

        if (element is ListBoxItem { DataContext: TokenSuggestion suggestion })
        {
            // Picked on the press, so the keyboard focus stays in the text box.
            e.Handled = true;
            PickSuggestion(suggestion);
            PartTextBox.Focus();
        }
    }

    private void OnSuggestionListSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (isSettingHighlight || isFillingSuggestions)
        {
            return;
        }

        // Selected from outside (UI Automation): pick it.
        if (PartSuggestionList.SelectedItem is TokenSuggestion suggestion)
        {
            PickSuggestion(suggestion);
        }
    }
}