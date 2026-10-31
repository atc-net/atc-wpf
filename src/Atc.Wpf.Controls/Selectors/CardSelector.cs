// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A wrapping grid of cards to pick one or several visual options from: report templates with a thumbnail, devices
/// with an image and a status, plans. It is a styled <see cref="ListBox"/>: the <see cref="ItemsControl.ItemTemplate"/>
/// draws the card body and the control draws the <see cref="DataDisplay.Card"/> frame, the selection border and the
/// check mark (a radio dot in <see cref="CardSelectionMode.Single"/>).
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ListBox.SelectedItems"/> always holds the selected items. With <see cref="KeyMemberPath"/> set,
/// <see cref="SelectedKeys"/> holds their keys too: clicks, keys and UI Automation update it in place and raise
/// <see cref="SelectedKeysChanged"/> once per action; changes made to it from outside (or a new instance) only
/// re-sync the cards and raise no event. When the items change, the keys no longer present are dropped (one event)
/// and items with a selected key are selected, so reloading the items keeps the selection.
/// </para>
/// <para>Meant for a few cards (a soft limit of about 100): the cards are not virtualized.</para>
/// </remarks>
public sealed partial class CardSelector : ListBox
{
    /// <summary>
    /// Identifies the <see cref="SelectionMode"/> dependency property.
    /// </summary>
    public static new readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(
        nameof(SelectionMode),
        typeof(CardSelectionMode),
        typeof(CardSelector),
        new FrameworkPropertyMetadata(CardSelectionMode.Multiple, OnSelectionModeChanged));

    /// <summary>
    /// The path of the property that holds an item's key (e.g. <c>Id</c> or <c>Info.Id</c>). When set,
    /// <see cref="SelectedKeys"/> and <see cref="SelectedKeysChanged"/> are available.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnKeyMemberPathChanged))]
    private string? keyMemberPath;

    /// <summary>
    /// The keys of the selected cards (needs <see cref="KeyMemberPath"/>). Two-way by default; updated in place.
    /// Keys without a matching item are kept until the items change.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// The width of each card; a fixed width makes a tidy grid. <see cref="double.NaN"/> sizes each card to its content.
    /// </summary>
    [DependencyProperty(DefaultValue = 200d, PropertyChangedCallback = nameof(OnStateChanged))]
    private double cardWidth;

    /// <summary>
    /// The height of each card; <see cref="double.NaN"/> (default) sizes the cards to their content.
    /// </summary>
    [DependencyProperty(DefaultValue = double.NaN, PropertyChangedCallback = nameof(OnStateChanged))]
    private double cardHeight;

    /// <summary>
    /// The space between two cards (and between two rows).
    /// </summary>
    [DependencyProperty(DefaultValue = 8d)]
    private double itemSpacing;

    /// <summary>
    /// Whether a selected card shows a check mark in its corner (a radio dot in <see cref="CardSelectionMode.Single"/>
    /// and <see cref="CardSelectionMode.SingleOrNone"/>).
    /// </summary>
    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnStateChanged))]
    private bool showCheckMark;

    /// <summary>
    /// The maximum number of selected cards in <see cref="CardSelectionMode.Multiple"/>; <see langword="null"/> for no
    /// limit. At the limit the unselected cards are disabled.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnStateChanged), ValidateValueCallback = nameof(ValidateMaxSelectionCount))]
    private int? maxSelectionCount;

    /// <summary>
    /// The path of a <see cref="bool"/> property that tells whether an item can be selected. Disabled cards are greyed
    /// out and skipped by the keyboard. Re-read on every selection or item change, or by <see cref="RefreshItemStates"/>.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnStateChanged))]
    private string? isEnabledMemberPath;

    private HashSet<string> previousItemKeys = new(StringComparer.Ordinal);
    private ObservableCollection<string>? subscribedSelectedKeys;
    private bool isWritingSelectedKeys;
    private bool isApplyingSelection;
    private bool isItemsChanging;

    static CardSelector()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CardSelector),
            new FrameworkPropertyMetadata(typeof(CardSelector)));

        // The control enforces its own modes; the ListBox underneath always allows several items.
        ListBox.SelectionModeProperty.OverrideMetadata(
            typeof(CardSelector),
            new FrameworkPropertyMetadata(System.Windows.Controls.SelectionMode.Multiple, null, CoerceListBoxSelectionMode));

        CommandManager.RegisterClassCommandBinding(
            typeof(CardSelector),
            new CommandBinding(ApplicationCommands.SelectAll, OnSelectAllExecuted, OnSelectAllCanExecute));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CardSelector"/> class.
    /// </summary>
    public CardSelector()
        => SetCurrentValue(SelectedKeysProperty, new ObservableCollectionEx<string>());

    /// <summary>
    /// Occurs when the control changes <see cref="SelectedKeys"/> (only with <see cref="KeyMemberPath"/>): once per
    /// card toggle, select all, clear, or item change that drops keys.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Gets or sets how many cards can be selected: any number (default), exactly one, or at most one.
    /// </summary>
    public new CardSelectionMode SelectionMode
    {
        get => (CardSelectionMode)GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    /// <summary>
    /// Selects every enabled card in one change. Does nothing when the selection limit has no room for them all,
    /// or when <see cref="SelectionMode"/> is not <see cref="CardSelectionMode.Multiple"/>.
    /// </summary>
    public new void SelectAll()
    {
        if (SelectionMode != CardSelectionMode.Multiple)
        {
            return;
        }

        var current = GetSelectedItemsInOrder();
        var toAdd = Items
            .Cast<object>()
            .Where(x => IsItemEnabled(x) && !current.Contains(x))
            .ToList();
        if (MaxSelectionCount is { } max && current.Count + toAdd.Count > max)
        {
            RefreshItemStates();
            return;
        }

        Commit([.. current, .. toAdd]);
    }

    /// <summary>
    /// Clears every enabled card in one change. Disabled cards keep their state.
    /// </summary>
    public void ClearSelection()
        => Commit(GetSelectedItemsInOrder().Where(x => !IsItemEnabled(x)).ToList());

    /// <summary>
    /// Re-reads <see cref="IsEnabledMemberPath"/> for every item and updates the enabled state of the cards.
    /// </summary>
    public void RefreshItemStates()
    {
        var selected = new HashSet<object>(SelectedItems.Cast<object>());
        var atLimit = SelectionMode == CardSelectionMode.Multiple &&
                      MaxSelectionCount is { } max &&
                      selected.Count >= max;

        foreach (var item in Items)
        {
            if (ItemContainerGenerator.ContainerFromItem(item) is CardSelectorItem container)
            {
                ApplyState(container, item, selected.Contains(item), atLimit);
            }
        }
    }

    /// <summary>
    /// Toggles an item the way a click does, following <see cref="SelectionMode"/>, the limit and the disabled items.
    /// Returns whether the item can be toggled.
    /// </summary>
    internal bool RequestToggle(object? item)
    {
        if (item is null || !Items.Contains(item) || !IsItemEnabled(item))
        {
            RefreshItemStates();
            return false;
        }

        var current = GetSelectedItemsInOrder();
        var isSelected = current.Contains(item);
        switch (SelectionMode)
        {
            case CardSelectionMode.Single:
                if (!isSelected || current.Count > 1)
                {
                    Commit([item]);
                }

                return true;
            case CardSelectionMode.SingleOrNone:
                Commit(isSelected ? [] : [item]);
                return true;
            default:
                if (isSelected)
                {
                    current.Remove(item);
                    Commit(current);
                    return true;
                }

                if (MaxSelectionCount is { } max && current.Count >= max)
                {
                    RefreshItemStates();
                    return false;
                }

                Commit([.. current, item]);
                return true;
        }
    }

    /// <summary>
    /// Returns whether an item can be selected (<see cref="IsEnabledMemberPath"/>).
    /// </summary>
    internal bool IsItemEnabled(object? item)
        => string.IsNullOrWhiteSpace(IsEnabledMemberPath) ||
           MemberPathEvaluator.GetValue(item, IsEnabledMemberPath) is not false;

    /// <summary>
    /// Returns the key of an item (<see cref="KeyMemberPath"/>), or <see langword="null"/>.
    /// </summary>
    internal string? GetKey(object? item)
        => MemberPathEvaluator.GetString(item, KeyMemberPath);

    /// <summary>
    /// Handles a key press on a card (or on the control when <paramref name="container"/> is <see langword="null"/>).
    /// Returns whether the key was handled.
    /// </summary>
    internal bool HandleKey(
        CardSelectorItem? container,
        Key key,
        ModifierKeys modifiers)
    {
        switch (key)
        {
            case Key.A when modifiers.HasFlag(ModifierKeys.Control):
                if (SelectionMode != CardSelectionMode.Multiple)
                {
                    return false;
                }

                SelectAll();
                return true;
            case Key.Space:
            case Key.Enter:
                if (container is null)
                {
                    return false;
                }

                RequestToggle(ItemContainerGenerator.ItemFromContainer(container));
                return true;
            case Key.Left:
            case Key.Right:
            case Key.Up:
            case Key.Down:
            case Key.Home:
            case Key.End:
                MoveTo(GetNeighbour(container, key));
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Returns the enabled card the key moves to from <paramref name="from"/>: Left / Right in item order, Up / Down to
    /// the nearest card of the row above or below, Home / End to the first / last card. <see langword="null"/> when
    /// there is none.
    /// </summary>
    internal CardSelectorItem? GetNeighbour(
        CardSelectorItem? from,
        Key key)
    {
        var all = Items
            .Cast<object>()
            .Select(x => ItemContainerGenerator.ContainerFromItem(x))
            .OfType<CardSelectorItem>()
            .ToList();
        var navigable = all
            .Where(x => x.IsEnabled && x.Visibility == Visibility.Visible)
            .ToList();
        if (navigable.Count == 0)
        {
            return null;
        }

        if (from is null || key == Key.Home)
        {
            return navigable[0];
        }

        if (key == Key.End)
        {
            return navigable[^1];
        }

        var isRightToLeft = FlowDirection == FlowDirection.RightToLeft;
        switch (key)
        {
            case Key.Left:
            case Key.Right:
                var forward = (key == Key.Right) != isRightToLeft;
                var index = all.IndexOf(from);
                return forward
                    ? all.Skip(index + 1).FirstOrDefault(navigable.Contains)
                    : all.Take(System.Math.Max(index, 0)).LastOrDefault(navigable.Contains);
            default:
                return GetVerticalNeighbour(from, navigable, key == Key.Down);
        }
    }

    /// <inheritdoc />
    protected override DependencyObject GetContainerForItemOverride()
        => new CardSelectorItem();

    /// <inheritdoc />
    protected override bool IsItemItsOwnContainerOverride(object item)
        => item is CardSelectorItem;

    /// <inheritdoc />
    protected override void PrepareContainerForItemOverride(
        DependencyObject element,
        object item)
    {
        base.PrepareContainerForItemOverride(element, item);
        if (element is CardSelectorItem container)
        {
            var atLimit = SelectionMode == CardSelectionMode.Multiple &&
                          MaxSelectionCount is { } max &&
                          SelectedItems.Count >= max;
            ApplyState(container, item, SelectedItems.Contains(item), atLimit);
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new CardSelectorAutomationPeer(this);

    /// <inheritdoc />
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        // Ctrl+A is taken before the ListBox select-all command sees it, so the limit and the disabled cards apply.
        // A text box inside a card keeps its own Ctrl+A.
        if (e.Key == Key.A &&
            Keyboard.Modifiers.HasFlag(ModifierKeys.Control) &&
            e.OriginalSource is not TextBoxBase)
        {
            HandleKey(null, e.Key, Keyboard.Modifiers);
            e.Handled = true;
            return;
        }

        base.OnPreviewKeyDown(e);
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        var container = ContainerFromElement(e.OriginalSource as DependencyObject) as CardSelectorItem;
        if (HandleKey(container, e.Key, Keyboard.Modifiers))
        {
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    /// <inheritdoc />
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        isItemsChanging = true;
        try
        {
            base.OnItemsChanged(e);
        }
        finally
        {
            isItemsChanging = false;
        }

        ReconcileAfterItemsChanged();
    }

    /// <inheritdoc />
    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnSelectionChanged(e);

        if (SelectionMode == CardSelectionMode.Multiple &&
            UIElementAutomationPeer.FromElement(this) is CardSelectorAutomationPeer peer)
        {
            peer.RaiseToggleStateChanged(e.AddedItems, e.RemovedItems);
        }

        if (isApplyingSelection || isItemsChanging)
        {
            return;
        }

        // A change made outside the control's own actions: SelectedItem / SelectedIndex, UnselectAll, or the items.
        if (e.RemovedItems.Cast<object>().Any(x => !Items.Contains(x)))
        {
            ReconcileAfterItemsChanged();
            return;
        }

        var current = GetSelectedItemsInOrder();
        if (SelectionMode != CardSelectionMode.Multiple && current.Count > 1)
        {
            Commit([e.AddedItems.Count > 0 ? e.AddedItems[e.AddedItems.Count - 1]! : current[0]]);
            return;
        }

        WriteKeysFrom(current);
        RefreshItemStates();
    }

    private static object CoerceListBoxSelectionMode(
        DependencyObject d,
        object baseValue)
        => System.Windows.Controls.SelectionMode.Multiple;

    private static void OnSelectAllExecuted(
        object sender,
        ExecutedRoutedEventArgs e)
    {
        if (sender is CardSelector control)
        {
            control.SelectAll();
            e.Handled = true;
        }
    }

    private static void OnSelectAllCanExecute(
        object sender,
        CanExecuteRoutedEventArgs e)
    {
        if (sender is CardSelector control)
        {
            e.CanExecute = control.SelectionMode == CardSelectionMode.Multiple;
            e.Handled = true;
        }
    }

    private static void OnSelectionModeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CardSelector)d;
        var current = control.GetSelectedItemsInOrder();
        if (control.SelectionMode != CardSelectionMode.Multiple && current.Count > 1)
        {
            control.Commit([current[0]]);
            return;
        }

        control.RefreshItemStates();
    }

    private static void OnKeyMemberPathChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CardSelector)d;
        control.previousItemKeys = control.GetItemKeys();
        control.ApplyKeysToItems();
        control.RefreshItemStates();
    }

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CardSelector)d;
        if (control.subscribedSelectedKeys is not null)
        {
            CollectionChangedEventManager.RemoveHandler(control.subscribedSelectedKeys, control.OnSelectedKeysCollectionChanged);
        }

        control.subscribedSelectedKeys = e.NewValue as ObservableCollection<string>;
        if (control.subscribedSelectedKeys is not null)
        {
            CollectionChangedEventManager.AddHandler(control.subscribedSelectedKeys, control.OnSelectedKeysCollectionChanged);
        }

        if (!control.isWritingSelectedKeys)
        {
            control.ApplyKeysToItems();
            control.RefreshItemStates();
        }
    }

    private static void OnStateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CardSelector)d).RefreshItemStates();

    private void OnSelectedKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (!isWritingSelectedKeys)
        {
            ApplyKeysToItems();
            RefreshItemStates();
        }
    }

    private void ApplyState(
        CardSelectorItem container,
        object item,
        bool isSelected,
        bool atLimit)
    {
        container.Width = CardWidth;
        container.Height = CardHeight;
        container.IsEnabled = IsItemEnabled(item) && (isSelected || !atLimit);
        container.IsRadioItem = SelectionMode != CardSelectionMode.Multiple;
        container.ShowCheckMark = ShowCheckMark;
    }

    private void MoveTo(CardSelectorItem? target)
    {
        if (target is null)
        {
            return;
        }

        target.Focus();
        target.BringIntoView();

        // A single-select card group is a radio group: moving also selects.
        if (SelectionMode != CardSelectionMode.Multiple)
        {
            var item = ItemContainerGenerator.ItemFromContainer(target);
            if (!SelectedItems.Contains(item))
            {
                RequestToggle(item);
            }
        }
    }

    private CardSelectorItem? GetVerticalNeighbour(
        CardSelectorItem from,
        IReadOnlyList<CardSelectorItem> navigable,
        bool down)
    {
        const double tolerance = 0.5;
        var origin = GetBounds(from);
        var candidates = navigable
            .Select(x => (Container: x, Bounds: GetBounds(x)))
            .Where(x => down
                ? x.Bounds.Top > origin.Top + tolerance
                : x.Bounds.Top < origin.Top - tolerance)
            .ToList();
        if (candidates.Count == 0)
        {
            return null;
        }

        var rowTop = down
            ? candidates.Min(x => x.Bounds.Top)
            : candidates.Max(x => x.Bounds.Top);
        var centerX = origin.Left + (origin.Width / 2);
        return candidates
            .Where(x => System.Math.Abs(x.Bounds.Top - rowTop) <= tolerance)
            .OrderBy(x => System.Math.Abs(x.Bounds.Left + (x.Bounds.Width / 2) - centerX))
            .First()
            .Container;
    }

    private Rect GetBounds(FrameworkElement element)
        => new(element.TranslatePoint(default, this), element.RenderSize);

    private List<object> GetSelectedItemsInOrder()
        => SelectedItems.Cast<object>().ToList();

    private HashSet<string> GetItemKeys()
    {
        var keys = new HashSet<string>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(KeyMemberPath))
        {
            return keys;
        }

        foreach (var item in Items)
        {
            if (GetKey(item) is { } key)
            {
                keys.Add(key);
            }
        }

        return keys;
    }

    private IReadOnlyList<string> GetCurrentKeys()
        => SelectedKeys is null
            ? []
            : SelectedKeys.ToList();

    private void Commit(IReadOnlyList<object> newSelection)
    {
        var current = GetSelectedItemsInOrder();
        if (current.Count != newSelection.Count || !current.All(newSelection.Contains))
        {
            isApplyingSelection = true;
            try
            {
                SetSelectedItems(newSelection);
            }
            finally
            {
                isApplyingSelection = false;
            }
        }

        WriteKeysFrom(newSelection);
        RefreshItemStates();
    }

    private void WriteKeysFrom(IReadOnlyList<object> selection)
    {
        if (string.IsNullOrWhiteSpace(KeyMemberPath))
        {
            return;
        }

        var keysOfSelection = selection
            .Select(GetKey)
            .OfType<string>()
            .ToList();

        // A single mode holds exactly the selected card's key: a restored key without a card is replaced, not kept.
        if (SelectionMode != CardSelectionMode.Multiple)
        {
            WriteKeys(keysOfSelection.Distinct(StringComparer.Ordinal).ToList());
            return;
        }

        var unselectedKeys = GetItemKeys();
        unselectedKeys.ExceptWith(keysOfSelection);

        WriteKeys(KeySelectionHelper.GetSelectionWith(GetCurrentKeys(), keysOfSelection, unselectedKeys));
    }

    private void WriteKeys(IReadOnlyList<string> newKeys)
    {
        var oldKeys = GetCurrentKeys();
        if (KeySelectionHelper.SetEquals(oldKeys, newKeys))
        {
            return;
        }

        isWritingSelectedKeys = true;
        try
        {
            var target = SelectedKeys;
            if (target is null)
            {
                target = new ObservableCollectionEx<string>();
                SetCurrentValue(SelectedKeysProperty, target);
            }

            KeySelectionHelper.ApplyInPlace(target, newKeys);
        }
        finally
        {
            isWritingSelectedKeys = false;
        }

        SelectedKeysChanged?.Invoke(
            this,
            new ValueChangedEventArgs<IReadOnlyList<string>>(
                Name,
                oldKeys,
                SelectedKeys.ToList()));
    }

    private void ReconcileAfterItemsChanged()
    {
        if (string.IsNullOrWhiteSpace(KeyMemberPath))
        {
            RefreshItemStates();
            return;
        }

        var itemKeys = GetItemKeys();
        var stale = GetCurrentKeys()
            .Where(x => previousItemKeys.Contains(x) && !itemKeys.Contains(x))
            .ToList();
        previousItemKeys = itemKeys;

        if (stale.Count > 0)
        {
            WriteKeys(KeySelectionHelper.GetSelectionWith(GetCurrentKeys(), [], stale));
        }

        ApplyKeysToItems();
        RefreshItemStates();
    }

    private void ApplyKeysToItems()
    {
        if (string.IsNullOrWhiteSpace(KeyMemberPath))
        {
            return;
        }

        var keys = GetCurrentKeys();
        var keySet = new HashSet<string>(keys, StringComparer.Ordinal);
        var items = Items.Cast<object>().ToList();
        List<object> target;
        if (SelectionMode == CardSelectionMode.Multiple)
        {
            target = items
                .Where(x => GetKey(x) is { } key && keySet.Contains(key))
                .ToList();
        }
        else
        {
            var first = keys
                .Select(key => items.FirstOrDefault(x => string.Equals(GetKey(x), key, StringComparison.Ordinal)))
                .FirstOrDefault(x => x is not null);
            target = first is null ? [] : [first];
        }

        var current = GetSelectedItemsInOrder();
        if (current.Count == target.Count && current.All(target.Contains))
        {
            return;
        }

        isApplyingSelection = true;
        try
        {
            SetSelectedItems(target);
        }
        finally
        {
            isApplyingSelection = false;
        }
    }

    private static bool ValidateMaxSelectionCount(object? value)
        => KeySelectionHelper.IsValidMaxSelectionCount(value);
}