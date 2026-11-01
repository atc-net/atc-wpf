// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <content>
/// The tokens: adding (pick, create, paste), removing, the "+N" chip, fixed keys, editing and the limit.
/// </content>
public partial class TokenBox
{
    private readonly List<MultiSelectChipItem> tokenItems = [];
    private readonly Dictionary<string, string> knownValues = new(StringComparer.Ordinal);
    private readonly HashSet<string> createdKeys = new(StringComparer.Ordinal);
    private readonly HashSet<string> createdPending = new(StringComparer.Ordinal);
    private readonly SelectedKeysCleanup selectionCleanup;
    private DataDisplay.Chip? overflowChip;
    private Button? overflowButton;
    private TextBox? editBox;
    private string? tokensSignature;
    private int lastOverflowHidden = -1;
    private int selectedTokenIndex = -1;
    private bool areRemoveButtonsNamed = true;

    /// <summary>
    /// Gets the panel holding the tokens, the "+N" element and the text box.
    /// </summary>
    internal MultiSelectChipPanel ChipPanel { get; }

    /// <summary>
    /// Gets the "+N" element (a button showing a small chip; its automation name lists the hidden names).
    /// </summary>
    internal Button OverflowButton => overflowButton!;

    /// <summary>
    /// Gets the text of the "+N" chip.
    /// </summary>
    internal string OverflowText
        => overflowChip?.Content as string ?? string.Empty;

    /// <summary>
    /// Gets the text shown in the empty text box: the watermark when there are no tokens, "{0} of max {1}" at the
    /// limit, otherwise empty.
    /// </summary>
    internal string PlaceholderText
        => TextBoxHelper.GetWatermark(PartTextBox) ?? string.Empty;

    /// <summary>
    /// Gets a value indicating whether <see cref="MaxSelectionCount"/> is reached.
    /// </summary>
    internal bool IsAtLimit
        => MaxSelectionCount is { } max && tokenItems.Count >= max;

    /// <summary>
    /// Gets the key of the token being edited, or <see langword="null"/>.
    /// </summary>
    internal string? EditingKey { get; private set; }

    /// <summary>
    /// Gets the text box of the token being edited, or <see langword="null"/>.
    /// </summary>
    internal TextBox? EditBox => editBox;

    /// <summary>
    /// Gets or sets the index of the token selected with <c>Backspace</c> or the arrow keys; -1 for none.
    /// </summary>
    internal int SelectedTokenIndex
    {
        get => selectedTokenIndex;
        set
        {
            var index = value < 0 || value >= tokenItems.Count ? -1 : value;
            if (index == selectedTokenIndex)
            {
                return;
            }

            selectedTokenIndex = index;
            ApplyTokenSelection();
        }
    }

    /// <summary>
    /// Returns the tokens, in token order.
    /// </summary>
    internal IReadOnlyList<MultiSelectChipItem> GetTokenItems()
        => tokenItems;

    /// <summary>
    /// Handles pasted text: the selection in the text box is replaced, and the result is split on the separators and
    /// on new lines; every part becomes a token in one change. Parts that match nothing (and cannot be created) stay
    /// in the text box.
    /// </summary>
    internal void Paste(string pasted)
    {
        pasted ??= string.Empty;
        var current = Text ?? string.Empty;
        var start = System.Math.Min(PartTextBox.SelectionStart, current.Length);
        var length = System.Math.Min(PartTextBox.SelectionLength, current.Length - start);
        var combined = current.Remove(start, length).Insert(start, pasted);

        if (!ContainsSeparator(combined))
        {
            SetText(combined);
            PartTextBox.CaretIndex = start + pasted.Length;
            UpdateSuggestions(force: false);
            return;
        }

        var leftover = CommitText(combined, commitLast: true);
        SetText(leftover);
        PartTextBox.CaretIndex = leftover.Length;
        UpdateSuggestions(force: false);
    }

    /// <summary>
    /// Removes a token, raising <see cref="SelectedKeysChanged"/> once. Does nothing for a fixed key or a key that is
    /// not a token.
    /// </summary>
    internal void RemoveKey(string key)
        => RemoveKeys([key]);

    /// <summary>
    /// Starts editing a created token (only with <see cref="AllowEditTokens"/>).
    /// </summary>
    /// <returns><see langword="true"/> when the edit started.</returns>
    internal bool BeginEditToken(string key)
    {
        var item = tokenItems.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.Ordinal));
        if (item is null || !IsEditable(key) || !IsEnabled)
        {
            return false;
        }

        if (EditingKey is not null)
        {
            CommitEditToken();
        }

        SetCurrentValue(IsDropDownOpenProperty, false);
        SelectedTokenIndex = -1;
        EditingKey = key;
        editBox = new TextBox
        {
            Text = item.DisplayValue,
            MinWidth = 40,
            MinHeight = 0,
            Padding = new Thickness(2, 0, 2, 0),
            Margin = new Thickness(0, 1, 3, 1),
            VerticalAlignment = VerticalAlignment.Center,
        };
        AutomationProperties.SetName(editBox, item.DisplayValue);
        editBox.LostKeyboardFocus += OnEditBoxLostKeyboardFocus;
        tokensSignature = null;
        UpdateTokens();
        editBox.SelectAll();
        _ = editBox.Focus();
        return true;
    }

    /// <summary>
    /// Ends the edit: the new text replaces the token (one change). Empty text removes the token; text equal to
    /// another token, or refused by <see cref="CreateValidator"/> or <see cref="TokenCreating"/>, keeps the old one.
    /// </summary>
    internal void CommitEditToken()
    {
        if (EditingKey is not { } key || editBox is null)
        {
            return;
        }

        var newText = editBox.Text.Trim();
        var oldDisplay = GetDisplayValue(key) ?? key;
        EndEdit();
        UpdateTokens();

        if (newText.Length == 0)
        {
            RemoveKeys([key]);
            return;
        }

        if (string.Equals(newText, oldDisplay, StringComparison.Ordinal) ||
            tokenItems.Any(x => !string.Equals(x.Key, key, StringComparison.Ordinal) &&
                                string.Equals(x.DisplayValue, newText, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        if (CreateValidator?.Invoke(newText) is { Length: > 0 } error)
        {
            ErrorText = error;
            return;
        }

        var args = new TokenCreatingEventArgs(newText);
        TokenCreating?.Invoke(this, args);
        if (!args.Cancel)
        {
            ReplaceToken(key, string.IsNullOrWhiteSpace(args.Key) ? newText : args.Key, newText);
        }
    }

    /// <summary>
    /// Replaces an edited token by a new key and display value at the same position (one change).
    /// </summary>
    private void ReplaceToken(
        string key,
        string newKey,
        string newText)
    {
        var target = SelectedKeys;
        if (target is null)
        {
            return;
        }

        var oldKeys = target.ToList();
        if (!string.Equals(newKey, key, StringComparison.Ordinal) && oldKeys.Contains(newKey, StringComparer.Ordinal))
        {
            return;
        }

        if (createdKeys.Remove(key))
        {
            knownValues.Remove(key);
            if (AddNewValuesToItems)
            {
                Items?.Remove(key);
            }
        }

        RegisterCreated(newKey, newText);
        var index = oldKeys.FindIndex(x => string.Equals(x, key, StringComparison.Ordinal));
        if (index < 0)
        {
            return;
        }

        tokensSignature = null;
        target[index] = newKey;
        UpdateTokens();
        RaiseSelectedKeysChanged(oldKeys, target.ToList());
    }

    /// <summary>
    /// Ends the edit without a change.
    /// </summary>
    internal void CancelEditToken()
    {
        if (EditingKey is null)
        {
            return;
        }

        EndEdit();
        tokensSignature = null;
        UpdateTokens();
    }

    /// <summary>
    /// Returns the automation peers of the shown tokens and the "+N" element.
    /// </summary>
    internal List<FrameworkElement> GetShownTokenElements()
    {
        if (!areRemoveButtonsNamed)
        {
            NameRemoveButtons();
        }

        var elements = new List<FrameworkElement>();
        var chipCount = ChipPanel.ChipCount;
        for (var i = 0; i < ChipPanel.VisibleCount && i < chipCount; i++)
        {
            if (ChipPanel.Children[i] is FrameworkElement element)
            {
                elements.Add(element);
            }
        }

        return elements;
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size constraint)
    {
        var size = base.MeasureOverride(constraint);
        if (!areRemoveButtonsNamed)
        {
            NameRemoveButtons();
        }

        return size;
    }

    private static void OnItemsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (TokenBox)d;
        control.UpdateTokens();
        control.selectionCleanup.Request(
            e.OldValue is Dictionary<string, string> oldItems
                ? new HashSet<string>(oldItems.Keys, StringComparer.Ordinal)
                : new HashSet<string>(StringComparer.Ordinal));
    }

    /// <summary>
    /// Drops the keys gone from <see cref="Items"/> (one event); created values and values picked from the
    /// suggestions provider are kept, and so are keys that were never in <see cref="Items"/> (free values, or keys
    /// restored before the items were loaded), as in the rest of the multi-select family.
    /// </summary>
    private void CleanUpSelection(IReadOnlySet<string> oldItemKeys)
    {
        var target = SelectedKeys;
        if (target is null || Items is not { } newItems)
        {
            return;
        }

        var oldKeys = target.ToList();
        var newKeys = oldKeys
            .Where(x => newItems.ContainsKey(x) ||
                        knownValues.ContainsKey(x) ||
                        !oldItemKeys.Contains(x))
            .Distinct(StringComparer.Ordinal)
            .ToList();
        if (KeySelectionHelper.SetEquals(oldKeys, newKeys))
        {
            return;
        }

        WriteSelectedKeys(target, newKeys);
        UpdateTokens();
        RaiseSelectedKeysChanged(oldKeys, target.ToList());
    }

    private bool IsFixed(string key)
        => FixedKeys?.Contains(key, StringComparer.Ordinal) ?? false;

    private bool IsEditable(string key)
        => AllowEditTokens &&
           !IsFixed(key) &&
           (createdKeys.Contains(key) ||
            (AllowCreate && !(Items?.ContainsKey(key) ?? false) && !knownValues.ContainsKey(key)));

    private string? GetDisplayValue(string key)
    {
        if (Items is not null && Items.TryGetValue(key, out var display))
        {
            return display ?? string.Empty;
        }

        if (knownValues.TryGetValue(key, out display))
        {
            return display;
        }

        return AllowCreate ? key : null;
    }

    private char[] GetSplitCharacters()
        => (Separators ?? string.Empty).Concat(['\r', '\n']).ToArray();

    private bool ContainsSeparator(string value)
        => value.IndexOfAny(GetSplitCharacters()) >= 0;

    private string[] SplitText(string value)
        => value.Split(GetSplitCharacters());

    private string LeftoverSeparator
        => string.IsNullOrEmpty(Separators)
            ? " "
            : Separators[0] + " ";

    /// <summary>
    /// Picks the highlighted suggestion, or commits the typed text (resolve, create, or leave it).
    /// </summary>
    /// <returns><see langword="true"/> when a token was added.</returns>
    private bool CommitCurrent()
    {
        if (IsDropDownOpen && HighlightedSuggestion is { } suggestion)
        {
            return PickSuggestion(suggestion);
        }

        var value = Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var count = tokenItems.Count;
        var leftover = CommitText(value, commitLast: true);
        SetText(leftover);
        PartTextBox.CaretIndex = leftover.Length;
        if (leftover.Length == 0)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
        }

        return tokenItems.Count != count;
    }

    /// <summary>
    /// Splits text on the separators and new lines and adds a token for each part (one change). With
    /// <paramref name="commitLast"/> unset, the last part is not committed (it is still being typed).
    /// </summary>
    /// <returns>The text that stays in the text box: the parts that did not become tokens.</returns>
    private string CommitText(
        string value,
        bool commitLast)
    {
        var parts = SplitText(value);
        var committedCount = commitLast ? parts.Length : parts.Length - 1;
        var leftovers = new List<string>();
        var pairs = new List<KeyValuePair<string, string>>();
        var pending = new HashSet<string>(tokenItems.Select(x => x.DisplayValue), StringComparer.OrdinalIgnoreCase);
        string? error = null;

        for (var i = 0; i < committedCount; i++)
        {
            var part = parts[i].Trim();
            if (part.Length == 0 || pending.Contains(part))
            {
                continue;
            }

            if (Resolve(part) is { } pair)
            {
                if (pending.Add(pair.Value))
                {
                    pairs.Add(pair);
                }

                continue;
            }

            if (!AllowCreate)
            {
                leftovers.Add(part);
                continue;
            }

            if (TryCreate(part, ref error) is { } created)
            {
                pending.Add(part);
                pairs.Add(created);
                createdPending.Add(created.Key);
            }
            else
            {
                leftovers.Add(part);
            }
        }

        AddTokens(pairs);
        createdPending.Clear();
        ErrorText = error ?? string.Empty;

        if (!commitLast)
        {
            leftovers.Add(parts[^1].TrimStart());
        }

        return string.Join(LeftoverSeparator, leftovers.Where(x => x.Length > 0));
    }

    /// <summary>
    /// Runs <see cref="CreateValidator"/> and <see cref="TokenCreating"/> for a new value.
    /// </summary>
    /// <returns>The key and display value of the new token, or <see langword="null"/> when the value is refused.</returns>
    private KeyValuePair<string, string>? TryCreate(
        string part,
        ref string? error)
    {
        if (CreateValidator?.Invoke(part) is { Length: > 0 } validationError)
        {
            error ??= validationError;
            return null;
        }

        var args = new TokenCreatingEventArgs(part);
        TokenCreating?.Invoke(this, args);
        if (args.Cancel)
        {
            return null;
        }

        return new KeyValuePair<string, string>(
            string.IsNullOrWhiteSpace(args.Key) ? part : args.Key,
            part);
    }

    /// <summary>
    /// Finds the item for a typed value: by display value (case-insensitive), then by key.
    /// </summary>
    private KeyValuePair<string, string>? Resolve(string value)
    {
        foreach (var source in EnumerateKnownPairs())
        {
            if (string.Equals(source.Value, value, StringComparison.OrdinalIgnoreCase))
            {
                return source;
            }
        }

        if (Items is not null && Items.TryGetValue(value, out var display))
        {
            return new KeyValuePair<string, string>(value, display ?? string.Empty);
        }

        if (knownValues.TryGetValue(value, out display))
        {
            return new KeyValuePair<string, string>(value, display);
        }

        return null;
    }

    private IEnumerable<KeyValuePair<string, string>> EnumerateKnownPairs()
    {
        if (Items is not null)
        {
            foreach (var pair in Items)
            {
                yield return new KeyValuePair<string, string>(pair.Key, pair.Value ?? string.Empty);
            }
        }

        foreach (var pair in knownValues)
        {
            yield return pair;
        }

        foreach (var pair in lastProviderResults)
        {
            yield return pair;
        }
    }

    /// <summary>
    /// Adds tokens in one change: duplicates (by key, or by display value ignoring case) are skipped, and no more
    /// than <see cref="MaxSelectionCount"/> tokens are kept (the rest is dropped).
    /// </summary>
    /// <returns><see langword="true"/> when a token was added.</returns>
    private bool AddTokens(IReadOnlyList<KeyValuePair<string, string>> pairs)
    {
        if (pairs.Count == 0)
        {
            return false;
        }

        var target = SelectedKeys;
        if (target is null)
        {
            target = new ObservableCollectionEx<string>();
            SetCurrentValue(SelectedKeysProperty, target);
        }

        var oldKeys = target.ToList();
        var keys = new HashSet<string>(oldKeys, StringComparer.Ordinal);
        var displays = new HashSet<string>(tokenItems.Select(x => x.DisplayValue), StringComparer.OrdinalIgnoreCase);
        var count = tokenItems.Count;
        var toAdd = new List<string>();
        foreach (var pair in pairs)
        {
            if (MaxSelectionCount is { } max && count >= max)
            {
                break;
            }

            if (keys.Contains(pair.Key) || displays.Contains(pair.Value))
            {
                continue;
            }

            if (createdPending.Contains(pair.Key))
            {
                RegisterCreated(pair.Key, pair.Value);
            }
            else if (Items is null || !Items.ContainsKey(pair.Key))
            {
                // A value from the suggestions provider: remember it, so its token keeps its display value.
                knownValues[pair.Key] = pair.Value;
                if (AddNewValuesToItems)
                {
                    Items?.TryAdd(pair.Key, pair.Value);
                }
            }

            keys.Add(pair.Key);
            displays.Add(pair.Value);
            toAdd.Add(pair.Key);
            count++;
        }

        if (toAdd.Count == 0)
        {
            return false;
        }

        var newKeys = KeySelectionHelper.GetSelectionWith(oldKeys, toAdd, []);
        WriteSelectedKeys(target, newKeys);
        UpdateTokens();
        RaiseSelectedKeysChanged(oldKeys, target.ToList());
        return true;
    }

    private void RegisterCreated(
        string key,
        string display)
    {
        createdKeys.Add(key);
        knownValues[key] = display;
        if (AddNewValuesToItems)
        {
            Items?.TryAdd(key, display);
        }
    }

    private void RemoveKeys(IReadOnlyList<string> keys)
    {
        var target = SelectedKeys;
        if (target is null)
        {
            return;
        }

        var removable = keys
            .Where(x => !IsFixed(x) && target.Contains(x, StringComparer.Ordinal))
            .ToList();
        if (removable.Count == 0)
        {
            return;
        }

        SelectedTokenIndex = -1;
        var oldKeys = target.ToList();
        var newKeys = KeySelectionHelper.GetSelectionWith(oldKeys, [], removable);
        WriteSelectedKeys(target, newKeys);
        UpdateTokens();
        RaiseSelectedKeysChanged(oldKeys, target.ToList());
    }

    private bool SelectLastRemovableToken()
    {
        for (var i = tokenItems.Count - 1; i >= 0; i--)
        {
            if (tokenItems[i].CanRemove)
            {
                SelectedTokenIndex = i;
                return true;
            }
        }

        return false;
    }

    private bool RemoveSelectedToken()
    {
        var index = SelectedTokenIndex;
        if (index < 0 || index >= tokenItems.Count)
        {
            return false;
        }

        var item = tokenItems[index];
        if (!item.CanRemove)
        {
            return true;
        }

        RemoveKey(item.Key);
        return true;
    }

    private bool MoveTokenSelection(int delta)
    {
        if (tokenItems.Count == 0)
        {
            return false;
        }

        if (SelectedTokenIndex < 0)
        {
            if (delta > 0)
            {
                return false;
            }

            SelectedTokenIndex = tokenItems.Count - 1;
            return true;
        }

        var index = SelectedTokenIndex + delta;
        SelectedTokenIndex = index < 0 ? 0 : index;
        if (index >= tokenItems.Count)
        {
            PartTextBox.CaretIndex = 0;
        }

        return true;
    }

    private void ApplyTokenSelection()
    {
        var chipCount = ChipPanel.ChipCount;
        for (var i = 0; i < chipCount; i++)
        {
            if (ChipPanel.Children[i] is DataDisplay.Chip chip)
            {
                chip.IsSelected = i == selectedTokenIndex;
            }
        }
    }

    private MultiSelectChipPanel CreateChipPanel()
    {
        overflowChip = new DataDisplay.Chip
        {
            Size = ChipSize.Small,
            Padding = new Thickness(6, 1, 6, 1),
            Focusable = false,
            IsHitTestVisible = false,
        };

        overflowButton = new Button
        {
            Content = overflowChip,
            Template = new ControlTemplate(typeof(Button))
            {
                VisualTree = new FrameworkElementFactory(typeof(ContentPresenter)),
            },
            Focusable = false,
            IsTabStop = false,
            Cursor = Cursors.Hand,
            Margin = new Thickness(0, 1, 3, 1),
            VerticalAlignment = VerticalAlignment.Center,
        };
        overflowButton.Click += OnOverflowClick;

        var panel = new MultiSelectChipPanel
        {
            UpdateOverflow = OnUpdateOverflow,
            MinTrailingWidth = 40,
        };
        panel.AutomationPeerFactory = () => new TokenListAutomationPeer(panel, this);
        AutomationProperties.SetAutomationId(panel, "Tokens");
        panel.Children.Add(overflowButton);
        return panel;
    }

    /// <summary>
    /// Rebuilds the tokens when the selection, the items or an option changed, and updates the field state.
    /// </summary>
    private void UpdateTokens()
    {
        if (ChipPanel is null)
        {
            return;
        }

        var keys = SelectedKeys is null
            ? []
            : SelectedKeys.Distinct(StringComparer.Ordinal).ToList();
        var tokens = new List<(string Key, string Display, bool CanRemove)>();
        foreach (var key in keys)
        {
            if (GetDisplayValue(key) is { } display)
            {
                tokens.Add((key, display, !IsFixed(key)));
            }
        }

        var isFocused = IsKeyboardFocusWithin;
        var limitWhenUnfocused = !isFocused && MaxVisibleTokens is not null;
        var signature = string.Join(
            "\u001f",
            tokens.Select(x => x.Key + "\u001e" + x.Display + "\u001e" + (x.CanRemove ? "1" : "0") + (IsEditable(x.Key) ? "1" : "0"))) +
            "\u001d" + TokensWrap + limitWhenUnfocused + isFocused + MaxVisibleTokens + EditingKey + IsEnabled;

        if (!string.Equals(signature, tokensSignature, StringComparison.Ordinal))
        {
            tokensSignature = signature;
            RebuildTokens(tokens, isFocused, limitWhenUnfocused);
        }

        UpdateFieldState();
    }

    private void RebuildTokens(
        List<(string Key, string Display, bool CanRemove)> tokens,
        bool isFocused,
        bool limitWhenUnfocused)
    {
        var selectedKey = selectedTokenIndex >= 0 && selectedTokenIndex < tokenItems.Count
            ? tokenItems[selectedTokenIndex].Key
            : null;

        tokenItems.Clear();
        tokenItems.AddRange(tokens.Select(x => new MultiSelectChipItem(x.Key, x.Display, x.CanRemove, RemoveKey)));
        selectedTokenIndex = selectedKey is null
            ? -1
            : tokenItems.FindIndex(x => string.Equals(x.Key, selectedKey, StringComparison.Ordinal));

        ChipPanel.ClearChips();
        var elementCount = limitWhenUnfocused && TokensWrap
            ? System.Math.Min(tokenItems.Count, System.Math.Max(0, MaxVisibleTokens ?? 0))
            : tokenItems.Count;
        for (var i = 0; i < elementCount; i++)
        {
            var item = tokenItems[i];
            ChipPanel.AddChip(
                string.Equals(item.Key, EditingKey, StringComparison.Ordinal) && editBox is not null
                    ? editBox
                    : CreateTokenElement(item));
        }

        lastOverflowHidden = -1;
        ChipPanel.IsWrapping = TokensWrap;
        ChipPanel.MaxVisibleChips = limitWhenUnfocused ? MaxVisibleTokens : null;
        ChipPanel.TotalCount = tokenItems.Count;

        PartScrollViewer.HorizontalScrollBarVisibility = TokensWrap || !isFocused
            ? ScrollBarVisibility.Disabled
            : ScrollBarVisibility.Hidden;
        PartScrollViewer.VerticalScrollBarVisibility = TokensWrap
            ? ScrollBarVisibility.Auto
            : ScrollBarVisibility.Disabled;

        ApplyTokenSelection();
        areRemoveButtonsNamed = false;
        NameRemoveButtons();

        if (UIElementAutomationPeer.FromElement(this) is { } peer)
        {
            peer.ResetChildrenCache();
        }

        if (UIElementAutomationPeer.FromElement(ChipPanel) is { } listPeer)
        {
            listPeer.ResetChildrenCache();
        }
    }

    private FrameworkElement CreateTokenElement(MultiSelectChipItem item)
    {
        if (TokenTemplate is { } template)
        {
            var presenter = new ContentPresenter
            {
                Content = item,
                ContentTemplate = template,
                VerticalAlignment = VerticalAlignment.Center,
            };
            presenter.MouseLeftButtonDown += (_, e) => OnTokenMouseLeftButtonDown(item, e);
            return presenter;
        }

        var chip = new DataDisplay.Chip
        {
            Variant = ChipVariant.Input,
            Size = ChipSize.Small,
            Content = item.DisplayValue,
            CanRemove = item.CanRemove && IsEnabled,
            DataContext = item,
            Focusable = false,
            IsClickable = false,
            Padding = new Thickness(6, 0, 2, 0),
            Margin = new Thickness(0, 1, 3, 1),
            VerticalAlignment = VerticalAlignment.Center,
            ToolTip = item.DisplayValue,
        };
        AutomationProperties.SetName(chip, item.DisplayValue);
        chip.RemoveClick += (_, _) => RemoveKey(item.Key);
        chip.MouseLeftButtonDown += (_, e) => OnTokenMouseLeftButtonDown(item, e);
        return chip;
    }

    private void OnTokenMouseLeftButtonDown(
        MultiSelectChipItem item,
        MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2 && BeginEditToken(item.Key))
        {
            e.Handled = true;
            return;
        }

        // A press on a token selects it, so Backspace / Delete remove it.
        var index = tokenItems.IndexOf(item);
        _ = Dispatcher.BeginInvoke(DispatcherPriority.Input, () => SelectedTokenIndex = index);
    }

    private void NameRemoveButtons()
    {
        var allNamed = true;
        foreach (var chip in ChipPanel.Children.OfType<DataDisplay.Chip>())
        {
            if (chip.DataContext is not MultiSelectChipItem item)
            {
                continue;
            }

            if (chip.Template is null)
            {
                allNamed = false;
                continue;
            }

            chip.ApplyTemplate();
            if (chip.Template.FindName("PART_RemoveButton", chip) is Button removeButton)
            {
                AutomationProperties.SetName(
                    removeButton,
                    string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.Remove1, item.DisplayValue));
            }
            else
            {
                allNamed = false;
            }
        }

        areRemoveButtonsNamed = allNamed;
    }

    private void OnUpdateOverflow(int hidden)
    {
        if (hidden == lastOverflowHidden || overflowChip is null || overflowButton is null)
        {
            return;
        }

        lastOverflowHidden = hidden;
        if (hidden <= 0)
        {
            overflowChip.Content = string.Empty;
            AutomationProperties.SetName(overflowButton, string.Empty);
            overflowButton.ToolTip = null;
            return;
        }

        var names = tokenItems
            .Skip(System.Math.Max(0, tokenItems.Count - hidden))
            .Select(x => x.DisplayValue)
            .ToList();

        overflowChip.Content = string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.MoreCount1, hidden);
        AutomationProperties.SetName(overflowButton, string.Join(", ", names));
        overflowButton.ToolTip = string.Join(Environment.NewLine, names);
    }

    private void OnOverflowClick(
        object sender,
        RoutedEventArgs e)
    {
        PartTextBox.Focus();
        e.Handled = true;
    }

    private void UpdateFieldState()
    {
        if (PartTextBox is null)
        {
            return;
        }

        var atLimit = IsAtLimit;
        string placeholder;
        if (atLimit)
        {
            placeholder = string.Format(
                CultureInfo.CurrentCulture,
                Atc.Wpf.Resources.Miscellaneous.CountOfMax2,
                tokenItems.Count,
                MaxSelectionCount);
        }
        else if (tokenItems.Count == 0)
        {
            placeholder = TextBoxHelper.GetWatermark(this) ?? string.Empty;
        }
        else
        {
            placeholder = string.Empty;
        }

        TextBoxHelper.SetWatermark(PartTextBox, placeholder);
        PartTextBox.IsReadOnly = atLimit;
        AutomationProperties.SetName(PartTextBox, TextBoxHelper.GetWatermark(this) ?? string.Empty);
        if (atLimit)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
        }

        PartClearButton.Visibility = ShowClearButton && IsEnabled && tokenItems.Any(x => x.CanRemove)
            ? Visibility.Visible
            : Visibility.Collapsed;
        var clearName = Atc.Wpf.Resources.Miscellaneous.Clear;
        AutomationProperties.SetName(PartClearButton, clearName);
        PartClearButton.ToolTip = clearName;
    }

    private void OnEditBoxLostKeyboardFocus(
        object sender,
        KeyboardFocusChangedEventArgs e)
    {
        if (ReferenceEquals(sender, editBox))
        {
            CommitEditToken();
        }
    }

    private void EndEdit()
    {
        if (editBox is not null)
        {
            editBox.LostKeyboardFocus -= OnEditBoxLostKeyboardFocus;
        }

        var hadFocus = editBox?.IsKeyboardFocusWithin ?? false;
        editBox = null;
        EditingKey = null;
        tokensSignature = null;
        if (hadFocus)
        {
            PartTextBox.Focus();
        }
    }

    /// <summary>
    /// Writes a change made by the control into <see cref="SelectedKeys"/>; the tokens are updated once afterwards,
    /// not once per notification.
    /// </summary>
    private void WriteSelectedKeys(
        ObservableCollection<string> target,
        IReadOnlyList<string> newKeys)
    {
        isWritingSelectedKeys = true;
        try
        {
            KeySelectionHelper.ApplyInPlace(target, newKeys);
        }
        finally
        {
            isWritingSelectedKeys = false;
        }
    }
}