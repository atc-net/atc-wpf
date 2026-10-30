// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <content>
/// The chips display mode and the deferred (OK / Cancel) commit mode.
/// </content>
public partial class MultiSelectComboBox
{
    /// <summary>
    /// In one-line chips mode, no more chip elements than this are created (more never fit on one line).
    /// </summary>
    private const int MaxOneLineChipElements = 100;

    private readonly List<MultiSelectChipItem> chipItems = [];
    private readonly SelectedKeysCleanup selectionCleanup;
    private ObservableCollection<string>? subscribedDisabledKeys;
    private ObservableCollection<string>? draftKeys;
    private DataDisplay.Chip? overflowChip;
    private Button? overflowButton;
    private string? chipsSignature;
    private int lastOverflowHidden = -1;
    private bool isCommitRequested;
    private bool areRemoveButtonsNamed = true;

    /// <summary>
    /// Gets the panel holding the chips and the "+N" element in <see cref="MultiSelectDisplayMode.Chips"/> mode.
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
    /// Gets a value indicating whether the pop-up is open with a draft of the ticks (<see cref="MultiSelectCommitMode.Deferred"/>).
    /// </summary>
    internal bool IsDrafting => draftKeys is not null;

    /// <summary>
    /// Returns the chips (one per ticked item, in item order) of <see cref="MultiSelectDisplayMode.Chips"/> mode.
    /// </summary>
    internal IReadOnlyList<MultiSelectChipItem> GetChipItems()
        => chipItems;

    /// <summary>
    /// Unticks a key without opening the pop-up, raising <see cref="SelectedKeysChanged"/> once. Does nothing for a
    /// disabled key or a key that is not ticked.
    /// </summary>
    internal void RemoveKey(string key)
    {
        if (SelectedKeys is null ||
            !SelectedKeys.Contains(key, StringComparer.Ordinal) ||
            (DisabledKeys?.Contains(key, StringComparer.Ordinal) ?? false))
        {
            return;
        }

        if (draftKeys is null && PartCheckList.GetItem(key) is { } row)
        {
            // The list unticks it and raises one event, which is passed on.
            row.IsChecked = false;
            return;
        }

        var oldKeys = SelectedKeys.ToList();
        SelectedKeys.Remove(key);
        draftKeys?.Remove(key);
        UpdateSummary();
        RaiseSelectedKeysChanged(oldKeys, SelectedKeys.ToList());
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

    /// <summary>
    /// Returns the automation peers of the chip remove buttons and the "+N" element that are shown.
    /// </summary>
    internal List<AutomationPeer> GetChipAutomationPeers()
    {
        var peers = new List<AutomationPeer>();
        if (DisplayMode != MultiSelectDisplayMode.Chips)
        {
            return peers;
        }

        if (!areRemoveButtonsNamed)
        {
            NameRemoveButtons();
        }

        var chipCount = ChipPanel.Children.Count - 1;
        for (var i = 0; i < ChipPanel.VisibleCount && i < chipCount; i++)
        {
            var element = ChipPanel.Children[i];
            if (element is DataDisplay.Chip chip)
            {
                if (chip.CanRemove &&
                    chip.Template?.FindName("PART_RemoveButton", chip) is Button removeButton &&
                    UIElementAutomationPeer.CreatePeerForElement(removeButton) is { } removePeer)
                {
                    peers.Add(removePeer);
                }
            }
            else if (UIElementAutomationPeer.CreatePeerForElement(element) is { } elementPeer)
            {
                peers.Add(elementPeer);
            }
            else
            {
                peers.AddRange(new FrameworkElementAutomationPeer((FrameworkElement)element).GetChildren() ?? []);
            }
        }

        if (ChipPanel.HiddenCount > 0 &&
            UIElementAutomationPeer.CreatePeerForElement(OverflowButton) is { } overflowPeer)
        {
            peers.Add(overflowPeer);
        }

        return peers;
    }

    private static void OnItemsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiSelectComboBox)d;
        control.UpdateSummary();
        if (control.IsDrafting)
        {
            // The pop-up list cleans only the draft; the real selection drops its stale keys (one event) as well.
            control.selectionCleanup.Request(
                e.OldValue is Dictionary<string, string> oldItems
                    ? new HashSet<string>(oldItems.Keys, StringComparer.Ordinal)
                    : new HashSet<string>(StringComparer.Ordinal));
        }
    }

    /// <summary>
    /// Drops the keys gone from <see cref="Items"/> from the real selection while a deferred draft is open.
    /// </summary>
    private void CleanUpSelection(IReadOnlySet<string> oldItemKeys)
    {
        if (SelectedKeys is not { } target)
        {
            return;
        }

        var oldKeys = target.ToList();
        var newKeys = KeySelectionHelper.GetSelectionAfterItemsChanged(
            oldKeys,
            oldItemKeys,
            Items,
            SelectNewItems,
            MaxSelectionCount);
        if (KeySelectionHelper.SequenceEquals(oldKeys, newKeys))
        {
            return;
        }

        KeySelectionHelper.ApplyInPlace(target, newKeys);
        UpdateSummary();
        RaiseSelectedKeysChanged(oldKeys, target.ToList());
    }

    private static void OnChipsInputChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiSelectComboBox)d;
        control.chipsSignature = null;
        control.UpdateSummary();
    }

    private static void OnDisabledKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiSelectComboBox)d;
        if (control.subscribedDisabledKeys is not null)
        {
            control.subscribedDisabledKeys.CollectionChanged -= control.OnDisabledKeysCollectionChanged;
        }

        control.subscribedDisabledKeys = e.NewValue as ObservableCollection<string>;
        if (control.subscribedDisabledKeys is not null)
        {
            control.subscribedDisabledKeys.CollectionChanged += control.OnDisabledKeysCollectionChanged;
        }

        control.UpdateSummary();
    }

    private void OnDisabledKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
        => UpdateSummary();

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
            Template = CreatePlainButtonTemplate(),
            Focusable = false,
            IsTabStop = false,
            Cursor = Cursors.Hand,
            Margin = new Thickness(0, 1, 0, 1),
            VerticalAlignment = VerticalAlignment.Center,
        };
        overflowButton.Click += OnOverflowClick;

        var panel = new MultiSelectChipPanel
        {
            UpdateOverflow = OnUpdateOverflow,
        };
        panel.Children.Add(overflowButton);
        return panel;
    }

    private static ControlTemplate CreatePlainButtonTemplate()
        => new(typeof(Button))
        {
            VisualTree = new FrameworkElementFactory(typeof(ContentPresenter)),
        };

    private void UpdateChips()
    {
        var isChipsMode = DisplayMode == MultiSelectDisplayMode.Chips;
        PartSummaryHost.IsHitTestVisible = isChipsMode;
        var content = isChipsMode ? (object)ChipPanel : summaryTextBlock;
        if (!ReferenceEquals(PartSummaryHost.Content, content))
        {
            PartSummaryHost.Content = content;
        }

        if (!isChipsMode)
        {
            if (chipItems.Count > 0)
            {
                chipItems.Clear();
                ChipPanel.Children.RemoveRange(0, ChipPanel.Children.Count - 1);
                chipsSignature = null;
            }

            return;
        }

        RebuildChipsIfChanged();
    }

    private void RebuildChipsIfChanged()
    {
        var source = Items;
        var selected = new HashSet<string>((IEnumerable<string>?)SelectedKeys ?? [], StringComparer.Ordinal);
        var disabled = new HashSet<string>((IEnumerable<string>?)DisabledKeys ?? [], StringComparer.Ordinal);
        var ticked = source is null
            ? []
            : source.Where(x => selected.Contains(x.Key)).ToList();

        var signature = string.Join(
            "\u001f",
            ticked.Select(x => x.Key + "\u001e" + x.Value + "\u001e" + (disabled.Contains(x.Key) ? "1" : "0")));
        if (string.Equals(signature, chipsSignature, StringComparison.Ordinal))
        {
            return;
        }

        chipsSignature = signature;
        chipItems.Clear();
        chipItems.AddRange(ticked.Select(x => new MultiSelectChipItem(
            x.Key,
            x.Value ?? string.Empty,
            !disabled.Contains(x.Key),
            RemoveKey)));

        ChipPanel.Children.RemoveRange(0, ChipPanel.Children.Count - 1);
        var elementCount = ChipsWrap
            ? chipItems.Count
            : System.Math.Min(chipItems.Count, MaxVisibleChips is { } max ? System.Math.Max(0, max) : MaxOneLineChipElements);
        for (var i = 0; i < elementCount; i++)
        {
            ChipPanel.Children.Insert(i, CreateChipElement(chipItems[i]));
        }

        lastOverflowHidden = -1;
        ChipPanel.IsWrapping = ChipsWrap;
        ChipPanel.MaxVisibleChips = MaxVisibleChips;
        ChipPanel.TotalCount = chipItems.Count;
        areRemoveButtonsNamed = false;
        NameRemoveButtons();

        if (UIElementAutomationPeer.FromElement(this) is { } peer)
        {
            peer.ResetChildrenCache();
        }
    }

    private FrameworkElement CreateChipElement(MultiSelectChipItem item)
    {
        if (ChipTemplate is { } template)
        {
            return new ContentPresenter
            {
                Content = item,
                ContentTemplate = template,
                VerticalAlignment = VerticalAlignment.Center,
            };
        }

        var chip = new DataDisplay.Chip
        {
            Variant = ChipVariant.Input,
            Size = ChipSize.Small,
            Content = item.DisplayValue,
            CanRemove = item.CanRemove,
            DataContext = item,
            Focusable = false,
            Padding = new Thickness(6, 0, 2, 0),
            Margin = new Thickness(0, 1, 3, 1),
            VerticalAlignment = VerticalAlignment.Center,
            ToolTip = item.DisplayValue,
        };
        AutomationProperties.SetName(chip, item.DisplayValue);
        chip.RemoveClick += (_, _) => RemoveKey(item.Key);
        chip.MouseLeftButtonDown += OnChipMouseLeftButtonDown;
        return chip;
    }

    private void NameRemoveButtons()
    {
        var allNamed = true;
        foreach (var chip in ChipPanel.Children.OfType<DataDisplay.Chip>())
        {
            if (chip.Template is null)
            {
                allNamed = false;
                continue;
            }

            chip.ApplyTemplate();
            if (chip.Template.FindName("PART_RemoveButton", chip) is Button removeButton &&
                chip.DataContext is MultiSelectChipItem item)
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

        var names = chipItems
            .Skip(System.Math.Max(0, chipItems.Count - hidden))
            .Select(x => x.DisplayValue)
            .ToList();

        overflowChip.Content = string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.MoreCount1, hidden);
        AutomationProperties.SetName(overflowButton, string.Join(", ", names));
        overflowButton.ToolTip = string.Join(Environment.NewLine, names);
    }

    private bool RemoveLastChip()
    {
        var last = chipItems.LastOrDefault(x => x.CanRemove);
        if (last is null)
        {
            return false;
        }

        RemoveKey(last.Key);
        return true;
    }

    private void OnChipMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        // A press on the chip (not on its remove button, which handles the press itself) opens or closes the pop-up,
        // as a press anywhere else on the box does.
        SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
        e.Handled = true;
    }

    private void OnOverflowClick(
        object sender,
        RoutedEventArgs e)
    {
        SetCurrentValue(IsDropDownOpenProperty, true);
        e.Handled = true;
    }

    private void ApplyFooterTexts()
    {
        PartOkButton.Content = Atc.Wpf.Resources.Word.Ok;
        PartCancelButton.Content = Atc.Wpf.Resources.Word.Cancel;
    }

    private void BeginDraft()
    {
        var isDeferred = CommitMode == MultiSelectCommitMode.Deferred;
        PartCommitFooter.Visibility = isDeferred ? Visibility.Visible : Visibility.Collapsed;
        if (!isDeferred || draftKeys is not null)
        {
            return;
        }

        draftKeys = new ObservableCollection<string>((IEnumerable<string>?)SelectedKeys ?? []);
        BindingOperations.ClearBinding(PartCheckList, CheckListBox.SelectedKeysProperty);
        PartCheckList.SetCurrentValue(CheckListBox.SelectedKeysProperty, draftKeys);
    }

    private void EndDraft()
    {
        var commit = isCommitRequested;
        isCommitRequested = false;
        if (draftKeys is null)
        {
            return;
        }

        var draft = draftKeys.ToList();
        draftKeys = null;

        // Point the list back at the real selection; this discards the draft.
        Forward(CheckListBox.SelectedKeysProperty, SelectedKeysProperty, BindingMode.TwoWay);

        if (!commit)
        {
            return;
        }

        var target = SelectedKeys;
        var oldKeys = target is null ? [] : target.ToList();
        if (KeySelectionHelper.SequenceEquals(oldKeys, draft))
        {
            return;
        }

        if (target is null)
        {
            target = [];
            SetCurrentValue(SelectedKeysProperty, target);
        }

        KeySelectionHelper.ApplyInPlace(target, draft);
        UpdateSummary();
        RaiseSelectedKeysChanged(oldKeys, target.ToList());
    }

    private void CommitDraftAndClose()
    {
        isCommitRequested = IsDrafting;
        SetCurrentValue(IsDropDownOpenProperty, false);
        isCommitRequested = false;
    }

    private void OnOkClick(
        object sender,
        RoutedEventArgs e)
        => CommitDraftAndClose();

    private void OnCancelClick(
        object sender,
        RoutedEventArgs e)
        => SetCurrentValue(IsDropDownOpenProperty, false);
}