// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// A row of toggle chips, all visible, bound to a key/value <see cref="Items"/> dictionary and a
/// <see cref="SelectedKeys"/> collection: quick filters and facets such as "Error · Warning · Info · Debug".
/// Each item is shown as a <see cref="Chip"/>.
/// </summary>
/// <remarks>
/// Toggles made in the control (a chip, the "All" chip, an arrow key in a single-select group) update
/// <see cref="SelectedKeys"/> in place and raise <see cref="SelectedKeysChanged"/> once per action. Changes made to
/// <see cref="SelectedKeys"/> from outside (or a new instance) only re-sync the chips and raise no event.
/// </remarks>
public sealed partial class ChipGroup : UserControl
{
    private readonly Dictionary<string, ChipEntry> entriesByKey = new(StringComparer.Ordinal);
    private readonly Dictionary<Chip, ChipEntry> entriesByChip = [];
    private readonly List<ChipEntry> entries = [];
    private readonly List<Chip> chips = [];

    /// <summary>
    /// The items, keyed by their selection key, with their display value. Changing it keeps the selection of the keys
    /// still present and drops the others from <see cref="SelectedKeys"/>.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemsChanged))]
    private Dictionary<string, string> items;

    /// <summary>
    /// The keys of the selected chips. Two-way by default; updated in place.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// How many chips can be selected: any number, exactly one (a radio group) or at most one.
    /// </summary>
    [DependencyProperty(DefaultValue = ChipSelectionMode.Multiple, PropertyChangedCallback = nameof(OnSelectionModeChanged))]
    private ChipSelectionMode selectionMode;

    /// <summary>
    /// Whether keys that appear in <see cref="Items"/> (including the first items) are selected automatically.
    /// Only applies when <see cref="SelectionMode"/> is <see cref="ChipSelectionMode.Multiple"/>.
    /// </summary>
    [DependencyProperty]
    private bool selectNewItems;

    /// <summary>
    /// How a selected chip looks: filled with the accent colour, or outlined with an accent border.
    /// </summary>
    [DependencyProperty(DefaultValue = ChipGroupVariant.Filled, PropertyChangedCallback = nameof(OnChipAppearanceChanged))]
    private ChipGroupVariant chipVariant;

    /// <summary>
    /// The size of the chips.
    /// </summary>
    [DependencyProperty(DefaultValue = ChipSize.Medium, PropertyChangedCallback = nameof(OnChipAppearanceChanged))]
    private ChipSize chipSize;

    /// <summary>
    /// Whether a selected chip shows a check mark before its text.
    /// </summary>
    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnChipAppearanceChanged))]
    private bool showCheckMark;

    /// <summary>
    /// The direction the chips are laid out in.
    /// </summary>
    [DependencyProperty(DefaultValue = Orientation.Horizontal, PropertyChangedCallback = nameof(OnLayoutChanged))]
    private Orientation orientation;

    /// <summary>
    /// Whether the chips wrap onto more lines; <see langword="false"/> keeps them on one line that scrolls.
    /// </summary>
    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnLayoutChanged))]
    private bool wrap;

    /// <summary>
    /// The space between two chips (and between two lines).
    /// </summary>
    [DependencyProperty(DefaultValue = 4d, PropertyChangedCallback = nameof(OnLayoutChanged))]
    private double itemSpacing;

    /// <summary>
    /// Whether a leading "All" chip is shown (in <see cref="ChipSelectionMode.Multiple"/> mode only). It is selected
    /// when every enabled key is selected; clicking it selects them all, or clears them, in one change.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnSelectionModeChanged))]
    private bool showAllChip;

    /// <summary>
    /// The maximum number of selected keys; <see langword="null"/> for no limit. At the limit the unselected chips are disabled.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnStateChanged))]
    private int? maxSelectionCount;

    /// <summary>
    /// The keys whose chips are shown but cannot be toggled.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnDisabledKeysChanged))]
    private ObservableCollection<string> disabledKeys;

    /// <summary>
    /// An optional icon per key (a font icon, an image or any other content) shown before the text.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemIconsChanged))]
    private Dictionary<string, object>? itemIcons;

    /// <summary>
    /// An optional count per key shown after the text, e.g. "Error 12" for facets.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemCountsChanged))]
    private Dictionary<string, int>? itemCounts;

    private HashSet<string> itemKeys = new(StringComparer.Ordinal);
    private HashSet<string> selectedSet = new(StringComparer.Ordinal);
    private HashSet<string> disabledSet = new(StringComparer.Ordinal);
    private ObservableCollection<string>? subscribedSelectedKeys;
    private ObservableCollection<string>? subscribedDisabledKeys;
    private bool isWritingSelectedKeys;

    /// <summary>
    /// Occurs when the control changes <see cref="SelectedKeys"/>: once per chip toggle, "All" chip click, arrow key in a
    /// single-select group, or <see cref="Items"/> change that drops or adds keys.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChipGroup"/> class.
    /// </summary>
    public ChipGroup()
    {
        Focusable = false;
        IsTabStop = false;

        ItemsPanel = new Layouts.UniformSpacingPanel
        {
            ItemHorizontalAlignment = null,
            ItemVerticalAlignment = null,
        };
        KeyboardNavigation.SetTabNavigation(ItemsPanel, KeyboardNavigationMode.Once);
        KeyboardNavigation.SetDirectionalNavigation(ItemsPanel, KeyboardNavigationMode.None);

        ScrollHost = new ScrollViewer
        {
            Focusable = false,
            IsTabStop = false,
            Content = ItemsPanel,
        };
        Content = ScrollHost;

        AllChip = CreateChip();
        AllChip.Visibility = Visibility.Collapsed;

        CultureManager.UiCultureChanged += OnUiCultureChanged;
        ApplyTexts();
        ApplyLayout();

        SetCurrentValue(SelectedKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(DisabledKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(ItemsProperty, new Dictionary<string, string>(StringComparer.Ordinal));
    }

    /// <summary>
    /// Gets the chips of the items, in item order (without the "All" chip).
    /// </summary>
    internal IReadOnlyList<Chip> Chips => chips;

    /// <summary>
    /// Gets the leading "All" chip.
    /// </summary>
    internal Chip AllChip { get; }

    /// <summary>
    /// Gets the panel the chips are laid out in.
    /// </summary>
    internal Layouts.UniformSpacingPanel ItemsPanel { get; }

    /// <summary>
    /// Gets the scroll viewer around the panel.
    /// </summary>
    internal ScrollViewer ScrollHost { get; }

    /// <summary>
    /// Selects every enabled chip in one change. Does nothing when the selection limit has no room for them all,
    /// or when <see cref="SelectionMode"/> is not <see cref="ChipSelectionMode.Multiple"/>.
    /// </summary>
    public void SelectAll()
    {
        if (SelectionMode != ChipSelectionMode.Multiple)
        {
            return;
        }

        var keys = GetEnabledKeys();
        if (!Fits(keys))
        {
            RefreshStates();
            return;
        }

        Commit(KeySelectionHelper.GetSelectionWith(GetCurrentSelection(), keys, []));
    }

    /// <summary>
    /// Clears every enabled chip in one change. Disabled keys keep their state.
    /// </summary>
    public void ClearSelection()
        => Commit(KeySelectionHelper.GetSelectionWith(GetCurrentSelection(), [], GetEnabledKeys()));

    /// <summary>
    /// Returns the chip of an item, or <see langword="null"/> when the key is not in <see cref="Items"/>.
    /// </summary>
    internal Chip? GetChip(string key)
        => entriesByKey.TryGetValue(key, out var entry)
            ? entry.Chip
            : null;

    /// <summary>
    /// Returns the count shown on the chip of an item, or <see langword="null"/> when it shows none.
    /// </summary>
    internal string? GetCountText(string key)
        => entriesByKey.TryGetValue(key, out var entry) && entry.CountText.Visibility == Visibility.Visible
            ? entry.CountText.Text
            : null;

    /// <summary>
    /// Gets the chips of the selected items, in item order.
    /// </summary>
    internal IEnumerable<Chip> SelectedChips
        => entries
            .Where(x => x.Chip.IsSelected)
            .Select(x => x.Chip);

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new ChipGroupAutomationPeer(this);

    private static void OnItemsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ChipGroup)d;
        var current = control.GetCurrentSelection();
        var newSelection = KeySelectionHelper.GetSelectionAfterItemsChanged(
            current,
            control.itemKeys,
            control.Items,
            control.SelectNewItems && control.SelectionMode == ChipSelectionMode.Multiple);

        control.RebuildChips();

        if (KeySelectionHelper.SequenceEquals(current, newSelection))
        {
            control.RefreshStates();
            return;
        }

        control.Commit(newSelection);
    }

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ChipGroup)d;
        if (control.subscribedSelectedKeys is not null)
        {
            control.subscribedSelectedKeys.CollectionChanged -= control.OnSelectedKeysCollectionChanged;
        }

        control.subscribedSelectedKeys = e.NewValue as ObservableCollection<string>;
        if (control.subscribedSelectedKeys is not null)
        {
            control.subscribedSelectedKeys.CollectionChanged += control.OnSelectedKeysCollectionChanged;
        }

        if (!control.isWritingSelectedKeys)
        {
            control.RefreshStates();
        }
    }

    private static void OnDisabledKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ChipGroup)d;
        if (control.subscribedDisabledKeys is not null)
        {
            control.subscribedDisabledKeys.CollectionChanged -= control.OnDisabledKeysCollectionChanged;
        }

        control.subscribedDisabledKeys = e.NewValue as ObservableCollection<string>;
        if (control.subscribedDisabledKeys is not null)
        {
            control.subscribedDisabledKeys.CollectionChanged += control.OnDisabledKeysCollectionChanged;
        }

        control.RefreshStates();
    }

    private static void OnSelectionModeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ChipGroup)d;
        foreach (var entry in control.entries)
        {
            control.ApplyAppearance(entry.Chip);
        }

        control.RebuildPanel();
        control.RefreshStates();
    }

    private static void OnChipAppearanceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ChipGroup)d;
        control.ApplyAppearance(control.AllChip);
        foreach (var entry in control.entries)
        {
            control.ApplyAppearance(entry.Chip);
        }
    }

    private static void OnLayoutChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ChipGroup)d).ApplyLayout();

    private static void OnStateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ChipGroup)d).RefreshStates();

    private static void OnItemIconsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ChipGroup)d).ApplyIcons();

    private static void OnItemCountsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ChipGroup)d).ApplyCounts();

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
        => ApplyTexts();

    private void ApplyTexts()
    {
        var text = Atc.Wpf.Resources.Word.All;
        AllChip.Content = text;
        AutomationProperties.SetName(AllChip, text);
    }

    private void OnSelectedKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (!isWritingSelectedKeys)
        {
            RefreshStates();
        }
    }

    private void OnDisabledKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
        => RefreshStates();

    private Chip CreateChip()
    {
        var chip = new Chip
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            ToggleRequested = OnChipToggleRequested,
        };
        chip.KeyDown += OnChipKeyDown;
        ApplyAppearance(chip);
        return chip;
    }

    private void RebuildChips()
    {
        var source = Items ?? new Dictionary<string, string>(StringComparer.Ordinal);
        var oldEntries = new Dictionary<string, ChipEntry>(entriesByKey, StringComparer.Ordinal);

        entriesByKey.Clear();
        entriesByChip.Clear();
        entries.Clear();
        chips.Clear();
        foreach (var (key, value) in source)
        {
            if (oldEntries.Remove(key, out var entry))
            {
                entry.Text.Text = value ?? string.Empty;
            }
            else
            {
                entry = new ChipEntry(key, CreateChip(), value ?? string.Empty);
            }

            AutomationProperties.SetName(entry.Chip, entry.Text.Text);
            entriesByKey[key] = entry;
            entriesByChip[entry.Chip] = entry;
            entries.Add(entry);
            chips.Add(entry.Chip);
        }

        foreach (var removed in oldEntries.Values)
        {
            // Release a shared icon element so it can be placed on another chip.
            removed.Chip.ClearValue(Chip.IconProperty);
            removed.Chip.KeyDown -= OnChipKeyDown;
            removed.Chip.ToggleRequested = null;
        }

        itemKeys = new HashSet<string>(source.Keys, StringComparer.Ordinal);
        ApplyIcons();
        ApplyCounts();
        RebuildPanel();
    }

    private void RebuildPanel()
    {
        var showAll = ShowAllChip && SelectionMode == ChipSelectionMode.Multiple;
        AllChip.Visibility = showAll ? Visibility.Visible : Visibility.Collapsed;

        var children = new List<Chip>(entries.Count + 1);
        if (showAll)
        {
            children.Add(AllChip);
        }

        children.AddRange(entries.Select(x => x.Chip));

        if (ItemsPanel.Children.Cast<UIElement>().SequenceEqual(children))
        {
            return;
        }

        ItemsPanel.Children.Clear();
        foreach (var chip in children)
        {
            ItemsPanel.Children.Add(chip);
        }
    }

    private void ApplyAppearance(Chip chip)
    {
        chip.Size = ChipSize;
        chip.ShowCheckMark = ShowCheckMark;
        chip.IsRadioItem = !ReferenceEquals(chip, AllChip) && SelectionMode != ChipSelectionMode.Multiple;

        if (ChipVariant == ChipGroupVariant.Outlined)
        {
            chip.Background = Brushes.Transparent;
            chip.SetResourceReference(BorderBrushProperty, "AtcApps.Brushes.Control.Border");
            chip.SetResourceReference(Chip.HoverBackgroundProperty, "AtcApps.Brushes.Accent4");
            chip.SetResourceReference(Chip.SelectedBackgroundProperty, "AtcApps.Brushes.Accent4");
            chip.SetResourceReference(Chip.SelectedForegroundProperty, "AtcApps.Brushes.Text");
            chip.SetResourceReference(Chip.SelectedBorderBrushProperty, "AtcApps.Brushes.Accent");
            return;
        }

        chip.ClearValue(BackgroundProperty);
        chip.ClearValue(BorderBrushProperty);
        chip.ClearValue(Chip.HoverBackgroundProperty);
        chip.ClearValue(Chip.SelectedBackgroundProperty);
        chip.ClearValue(Chip.SelectedForegroundProperty);
        chip.ClearValue(Chip.SelectedBorderBrushProperty);
    }

    private void ApplyLayout()
    {
        var isHorizontal = Orientation == Orientation.Horizontal;
        ItemsPanel.Orientation = Orientation;
        ItemsPanel.ChildWrapping = Wrap ? VisualWrappingType.Wrap : VisualWrappingType.NoWrap;
        ItemsPanel.Spacing = ItemSpacing;

        // Wrapping: the chips flow across the main axis and the other one grows. One line: the main axis scrolls.
        var mainAxis = Wrap ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
        var crossAxis = Wrap ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;
        ScrollHost.HorizontalScrollBarVisibility = isHorizontal ? mainAxis : crossAxis;
        ScrollHost.VerticalScrollBarVisibility = isHorizontal ? crossAxis : mainAxis;
    }

    private void ApplyIcons()
    {
        var icons = ItemIcons;

        // Clear first, so an icon element can move from one chip to another.
        foreach (var entry in entries)
        {
            if (icons is null || !icons.TryGetValue(entry.Key, out var icon) || !ReferenceEquals(entry.Chip.Icon, icon))
            {
                entry.Chip.ClearValue(Chip.IconProperty);
            }
        }

        if (icons is null)
        {
            return;
        }

        foreach (var entry in entries)
        {
            if (icons.TryGetValue(entry.Key, out var icon) && !ReferenceEquals(entry.Chip.Icon, icon))
            {
                entry.Chip.Icon = icon;
            }
        }
    }

    private void ApplyCounts()
    {
        var counts = ItemCounts;
        foreach (var entry in entries)
        {
            if (counts is not null && counts.TryGetValue(entry.Key, out var count))
            {
                entry.CountText.Text = count.ToString(CultureInfo.CurrentCulture);
                entry.CountText.Visibility = Visibility.Visible;
            }
            else
            {
                entry.CountText.Text = string.Empty;
                entry.CountText.Visibility = Visibility.Collapsed;
            }
        }
    }

    private void RefreshStates()
    {
        selectedSet = new HashSet<string>((IEnumerable<string>?)SelectedKeys ?? Array.Empty<string>(), StringComparer.Ordinal);
        disabledSet = new HashSet<string>((IEnumerable<string>?)DisabledKeys ?? Array.Empty<string>(), StringComparer.Ordinal);

        var isMultiple = SelectionMode == ChipSelectionMode.Multiple;
        var atLimit = isMultiple && MaxSelectionCount is { } max && GetSelectedItemCount() >= max;

        foreach (var entry in entries)
        {
            var isSelected = selectedSet.Contains(entry.Key);
            entry.Chip.SetCurrentValue(Chip.IsSelectedProperty, isSelected);
            entry.Chip.IsEnabled = !disabledSet.Contains(entry.Key) && (isSelected || !atLimit);
        }

        var enabledKeys = GetEnabledKeys();
        var allSelected = KeySelectionHelper.GetAggregateState(enabledKeys.Count(selectedSet.Contains), enabledKeys.Count) == true;
        AllChip.SetCurrentValue(Chip.IsSelectedProperty, allSelected);
        AllChip.IsEnabled = enabledKeys.Count > 0 && (allSelected || Fits(enabledKeys));
    }

    private void OnChipToggleRequested(Chip chip)
    {
        if (ReferenceEquals(chip, AllChip))
        {
            ToggleAll();
            return;
        }

        if (entriesByChip.TryGetValue(chip, out var entry))
        {
            ToggleKey(entry.Key);
        }
    }

    private void ToggleKey(string key)
    {
        if (!itemKeys.Contains(key) || disabledSet.Contains(key))
        {
            RefreshStates();
            return;
        }

        var current = GetCurrentSelection();
        var isSelected = selectedSet.Contains(key);
        switch (SelectionMode)
        {
            case ChipSelectionMode.Single:
                Commit([key]);
                break;
            case ChipSelectionMode.SingleOrNone:
                Commit(isSelected
                    ? KeySelectionHelper.GetSelectionWith(current, [], [key])
                    : [key]);
                break;
            default:
                if (isSelected)
                {
                    Commit(KeySelectionHelper.GetSelectionWith(current, [], [key]));
                }
                else if (Fits([key]))
                {
                    Commit(KeySelectionHelper.GetSelectionWith(current, [key], []));
                }
                else
                {
                    RefreshStates();
                }

                break;
        }
    }

    private void ToggleAll()
    {
        if (SelectionMode != ChipSelectionMode.Multiple)
        {
            return;
        }

        var keys = GetEnabledKeys();
        if (keys.Count > 0 && keys.TrueForAll(selectedSet.Contains))
        {
            Commit(KeySelectionHelper.GetSelectionWith(GetCurrentSelection(), [], keys));
            return;
        }

        SelectAll();
    }

    private void Commit(IReadOnlyList<string> newSelection)
    {
        var oldSelection = GetCurrentSelection();
        if (KeySelectionHelper.SequenceEquals(oldSelection, newSelection))
        {
            RefreshStates();
            return;
        }

        isWritingSelectedKeys = true;
        try
        {
            var target = SelectedKeys;
            if (target is null)
            {
                target = [];
                SetCurrentValue(SelectedKeysProperty, target);
            }

            KeySelectionHelper.ApplyInPlace(target, newSelection);
        }
        finally
        {
            isWritingSelectedKeys = false;
        }

        RefreshStates();

        SelectedKeysChanged?.Invoke(
            this,
            new ValueChangedEventArgs<IReadOnlyList<string>>(
                Name,
                oldSelection,
                SelectedKeys.ToList()));
    }

    private IReadOnlyList<string> GetCurrentSelection()
        => SelectedKeys is null
            ? []
            : SelectedKeys.ToList();

    private List<string> GetEnabledKeys()
        => entries
            .Where(x => !disabledSet.Contains(x.Key))
            .Select(x => x.Key)
            .ToList();

    private int GetSelectedItemCount()
        => selectedSet.Count(itemKeys.Contains);

    private bool Fits(IEnumerable<string> keysToSelect)
    {
        if (MaxSelectionCount is not { } max)
        {
            return true;
        }

        var additional = keysToSelect.Count(x => !selectedSet.Contains(x));
        return GetSelectedItemCount() + additional <= max;
    }

    private void OnChipKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (sender is not Chip chip)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Space:
            case Key.Enter:
                chip.PerformClick();
                e.Handled = true;
                break;
            case Key.Right:
            case Key.Left:
            case Key.Down:
            case Key.Up:
                Move(chip, IsForward(e.Key));
                e.Handled = true;
                break;
            case Key.A when Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && SelectionMode == ChipSelectionMode.Multiple:
                SelectAll();
                e.Handled = true;
                break;
        }
    }

    private bool IsForward(Key key)
    {
        var isRightToLeft = FlowDirection == FlowDirection.RightToLeft;
        return key switch
        {
            Key.Right => !isRightToLeft,
            Key.Left => isRightToLeft,
            Key.Down => true,
            _ => false,
        };
    }

    private void Move(
        Chip from,
        bool forward)
    {
        var navigable = ItemsPanel.Children
            .OfType<Chip>()
            .Where(x => x.IsEnabled && x.Visibility == Visibility.Visible)
            .ToList();
        if (navigable.Count == 0)
        {
            return;
        }

        var index = navigable.IndexOf(from);
        var next = index < 0
            ? navigable[0]
            : navigable[(index + (forward ? 1 : -1) + navigable.Count) % navigable.Count];

        next.Focus();
        next.BringIntoView();

        // A single-select group is a radio group: moving also selects.
        if (SelectionMode != ChipSelectionMode.Multiple &&
            entriesByChip.TryGetValue(next, out var entry) &&
            !selectedSet.Contains(entry.Key))
        {
            ToggleKey(entry.Key);
        }
    }

    private sealed class ChipEntry
    {
        public ChipEntry(
            string key,
            Chip chip,
            string displayValue)
        {
            Key = key;
            Chip = chip;
            Text = new TextBlock
            {
                Text = displayValue,
                VerticalAlignment = VerticalAlignment.Center,
            };
            CountText = new TextBlock
            {
                Margin = new Thickness(6, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.SemiBold,
                Opacity = 0.7,
                Visibility = Visibility.Collapsed,
            };

            var content = new StackPanel { Orientation = Orientation.Horizontal };
            content.Children.Add(Text);
            content.Children.Add(CountText);
            chip.Content = content;
        }

        public string Key { get; }

        public Chip Chip { get; }

        public TextBlock Text { get; }

        public TextBlock CountText { get; }
    }
}