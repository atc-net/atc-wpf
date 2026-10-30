// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A drop-down where the user ticks several items. Closed, it looks like a <see cref="ComboBox"/> and shows a summary
/// of the ticked items ("orders, payments", "3 of 5 selected", "All"); open, it shows a <see cref="CheckListBox"/>
/// with Select all, Clear and an optional filter. Ticking an item does not close the pop-up.
/// </summary>
/// <remarks>
/// The pop-up list is a <see cref="CheckListBox"/>; the options are passed through to it. Ticks made in the pop-up
/// update <see cref="SelectedKeys"/> in place and raise <see cref="SelectedKeysChanged"/> once per action. Changes made
/// to <see cref="SelectedKeys"/> from outside (or a new instance) only update the ticks and the summary.
/// </remarks>
public partial class MultiSelectComboBox
{
    /// <summary>
    /// Identifies the <see cref="MaxDropDownHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxDropDownHeightProperty = ComboBox.MaxDropDownHeightProperty.AddOwner(
        typeof(MultiSelectComboBox));

    private static readonly DependencyPropertyKey SummaryTextPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(SummaryText),
        typeof(string),
        typeof(MultiSelectComboBox),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="SummaryText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SummaryTextProperty = SummaryTextPropertyKey.DependencyProperty;

    private readonly TextBlock summaryTextBlock;

    /// <summary>
    /// The items, keyed by their selection key, with their display value. Changing it keeps the ticks of the keys
    /// still present and drops the others from <see cref="SelectedKeys"/>.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnSummaryInputChanged))]
    private Dictionary<string, string> items;

    /// <summary>
    /// The keys of the ticked items. Two-way by default; updated in place.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// Whether keys that appear in <see cref="Items"/> (including the first items) are ticked automatically.
    /// </summary>
    [DependencyProperty]
    private bool selectNewItems;

    /// <summary>
    /// Whether the pop-up shows the top row with the tri-state "Select all" check box and the count.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showSelectAllRow;

    /// <summary>
    /// Whether the pop-up shows a "Clear" button in the "Select all" row.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool showClearButton;

    /// <summary>
    /// Whether the pop-up shows the filter box; <see langword="null"/> (the default) shows it when there are more
    /// than 10 items.
    /// </summary>
    [DependencyProperty]
    private bool? isFilterVisible;

    /// <summary>
    /// The text in the pop-up's filter box; only items whose display value contains it are shown.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private string filterText;

    /// <summary>
    /// The group of each key (key to group name); <see langword="null"/> for no groups.
    /// </summary>
    [DependencyProperty]
    private Dictionary<string, string>? itemGroups;

    /// <summary>
    /// Whether a group header in the pop-up has a tri-state check box that ticks or unticks its shown items.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isGroupCheckable;

    /// <summary>
    /// The maximum number of ticked keys; <see langword="null"/> for no limit. At the limit the unticked items are disabled.
    /// </summary>
    [DependencyProperty]
    private int? maxSelectionCount;

    /// <summary>
    /// The keys whose items are shown but cannot be toggled.
    /// </summary>
    [DependencyProperty]
    private ObservableCollection<string> disabledKeys;

    /// <summary>
    /// The template for the text part of a pop-up row; its data context is the <see cref="CheckListItem"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// The most display values the summary lists (comma-separated) before it switches to <see cref="CountFormat"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = 2, PropertyChangedCallback = nameof(OnSummaryInputChanged))]
    private int maxNamesInSummary;

    /// <summary>
    /// The summary when every item is ticked; <see langword="null"/> (the default) for the translated "All".
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnSummaryInputChanged))]
    private string? allText;

    /// <summary>
    /// The summary when nothing is ticked, shown in the watermark style; empty (the default) shows
    /// <c>TextBoxHelper.Watermark</c> instead.
    /// </summary>
    [DependencyProperty(DefaultValue = "", PropertyChangedCallback = nameof(OnSummaryInputChanged))]
    private string noneText;

    /// <summary>
    /// The summary format when more than <see cref="MaxNamesInSummary"/> items (but not all) are ticked:
    /// <c>{0}</c> is the ticked count and <c>{1}</c> the item count. <see langword="null"/> (the default) for the
    /// translated "{0} of {1} selected".
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnSummaryInputChanged))]
    private string? countFormat;

    /// <summary>
    /// Whether the pop-up is open. Two-way by default.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnIsDropDownOpenChanged))]
    private bool isDropDownOpen;

    private ObservableCollection<string>? subscribedSelectedKeys;
    private int tickedCount;

    /// <summary>
    /// Occurs when the pop-up list changes <see cref="SelectedKeys"/>: once per tick, Select all, Clear, group tick,
    /// or <see cref="Items"/> change that drops or adds keys.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Occurs when the pop-up opens.
    /// </summary>
    public event EventHandler? DropDownOpened;

    /// <summary>
    /// Occurs when the pop-up closes.
    /// </summary>
    public event EventHandler? DropDownClosed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiSelectComboBox"/> class.
    /// </summary>
    public MultiSelectComboBox()
    {
        InitializeComponent();

        summaryTextBlock = new TextBlock
        {
            TextTrimming = TextTrimming.CharacterEllipsis,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Bind(summaryTextBlock, MarginProperty, PaddingProperty);
        PartSummaryHost.Content = summaryTextBlock;

        Bind(PartWatermark, TextBlock.PaddingProperty, PaddingProperty);
        Bind(PartWatermark, TextBlock.ForegroundProperty, ForegroundProperty);
        Bind(PartToggleColumn, ColumnDefinition.WidthProperty, TextBoxHelper.ButtonWidthProperty);
        Bind(PartDropDownToggle, TextBoxHelper.ButtonWidthProperty, TextBoxHelper.ButtonWidthProperty);
        Bind(PartDropDownToggle, ControlsHelper.CornerRadiusProperty, ControlsHelper.CornerRadiusProperty);
        Bind(PartDropDownToggle, ToggleButton.IsCheckedProperty, IsDropDownOpenProperty, BindingMode.TwoWay);
        Bind(PartPopup, Popup.IsOpenProperty, IsDropDownOpenProperty, BindingMode.TwoWay);
        Bind(PartPopupBorder, MinWidthProperty, ActualWidthProperty);
        Bind(PartPopupBorder, MaxHeightProperty, MaxDropDownHeightProperty);
        Bind(PartPopupBorder, FlowDirectionProperty, FlowDirectionProperty);
        PartPopup.PlacementTarget = this;

        PartCheckList = new CheckListBox();
        PartPopupBorder.Child = PartCheckList;

        SetCurrentValue(SelectedKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(DisabledKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(ItemsProperty, new Dictionary<string, string>(StringComparer.Ordinal));

        // Options first, then SelectNewItems, SelectedKeys and Items last, so a first Items value is handled
        // with the final options.
        Forward(CheckListBox.ShowSelectAllRowProperty, ShowSelectAllRowProperty);
        Forward(CheckListBox.ShowClearButtonProperty, ShowClearButtonProperty);
        Forward(CheckListBox.IsFilterVisibleProperty, IsFilterVisibleProperty);
        Forward(CheckListBox.FilterTextProperty, FilterTextProperty, BindingMode.TwoWay);
        Forward(CheckListBox.ItemGroupsProperty, ItemGroupsProperty);
        Forward(CheckListBox.IsGroupCheckableProperty, IsGroupCheckableProperty);
        Forward(CheckListBox.MaxSelectionCountProperty, MaxSelectionCountProperty);
        Forward(CheckListBox.DisabledKeysProperty, DisabledKeysProperty);
        Forward(CheckListBox.ItemTemplateProperty, ItemTemplateProperty);
        Forward(CheckListBox.SelectNewItemsProperty, SelectNewItemsProperty);
        Forward(CheckListBox.SelectedKeysProperty, SelectedKeysProperty, BindingMode.TwoWay);
        Forward(CheckListBox.ItemsProperty, ItemsProperty);

        PartCheckList.SelectedKeysChanged += OnListSelectedKeysChanged;
        CultureManager.UiCultureChanged += OnUiCultureChanged;

        // Like ComboBox: while open, the control holds the mouse capture (sub-tree), so a click outside closes the
        // pop-up and a click on the box toggles it.
        Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideCapturedElement);
        AddHandler(Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCaptureWithin));

        UpdateSummary();
    }

    /// <summary>
    /// Gets or sets the maximum height of the pop-up; the list scrolls (virtualized) beyond it.
    /// </summary>
    public double MaxDropDownHeight
    {
        get => (double)GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    /// <summary>
    /// Gets the text the closed box shows for the ticked items (also the UI Automation value).
    /// </summary>
    public string SummaryText
    {
        get => (string)GetValue(SummaryTextProperty);
        private set => SetValue(SummaryTextPropertyKey, value);
    }

    /// <summary>
    /// Gets the check list shown in the pop-up.
    /// </summary>
    internal CheckListBox PartCheckList { get; }

    /// <summary>
    /// Ticks every shown, enabled item in one change.
    /// </summary>
    public void SelectAll()
        => PartCheckList.SelectAll();

    /// <summary>
    /// Unticks every enabled item in one change.
    /// </summary>
    public void ClearSelection()
        => PartCheckList.ClearSelection();

    /// <summary>
    /// Returns the pop-up row of an item, or <see langword="null"/> when the key is not in <see cref="Items"/>.
    /// Setting its <see cref="CheckListItem.IsChecked"/> ticks or unticks the item as a click would.
    /// </summary>
    /// <param name="key">The item key.</param>
    /// <returns>The row, or <see langword="null"/>.</returns>
    public CheckListItem? GetItem(string key)
        => PartCheckList.GetItem(key);

    /// <summary>
    /// Opens or closes the pop-up for a key press: <c>F4</c> and <c>Alt+Down</c> / <c>Alt+Up</c> toggle it,
    /// <c>Esc</c> and <c>Enter</c> close it.
    /// </summary>
    /// <returns><see langword="true"/> when the key was used.</returns>
    internal bool HandleKey(
        Key key,
        Key systemKey,
        ModifierKeys modifiers)
    {
        if (key == Key.F4 && modifiers == ModifierKeys.None)
        {
            SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
            return true;
        }

        if (key == Key.System &&
            systemKey is Key.Down or Key.Up &&
            modifiers.HasFlag(ModifierKeys.Alt))
        {
            SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
            return true;
        }

        if (IsDropDownOpen && key is Key.Escape or Key.Enter)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new MultiSelectComboBoxAutomationPeer(this);

    /// <inheritdoc />
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewKeyDown(e);
        if (!e.Handled && HandleKey(e.Key, e.SystemKey, Keyboard.Modifiers))
        {
            e.Handled = true;
        }
    }

    /// <inheritdoc />
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonDown(e);
        if (!IsDropDownOpen && !IsKeyboardFocusWithin)
        {
            Focus();
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == TextBoxHelper.WatermarkProperty)
        {
            UpdateWatermark();
        }
    }

    private static void OnSummaryInputChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((MultiSelectComboBox)d).UpdateSummary();

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiSelectComboBox)d;
        if (control.subscribedSelectedKeys is not null)
        {
            control.subscribedSelectedKeys.CollectionChanged -= control.OnSelectedKeysCollectionChanged;
        }

        control.subscribedSelectedKeys = e.NewValue as ObservableCollection<string>;
        if (control.subscribedSelectedKeys is not null)
        {
            control.subscribedSelectedKeys.CollectionChanged += control.OnSelectedKeysCollectionChanged;
        }

        control.UpdateSummary();
    }

    private static void OnIsDropDownOpenChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiSelectComboBox)d;
        var isOpen = (bool)e.NewValue;

        if (isOpen)
        {
            Mouse.Capture(control, CaptureMode.SubTree);
            _ = control.Dispatcher.BeginInvoke(DispatcherPriority.Input, control.FocusPopupContent);
            control.DropDownOpened?.Invoke(control, EventArgs.Empty);
        }
        else
        {
            if (control.PartPopupBorder.IsKeyboardFocusWithin)
            {
                control.Focus();
            }

            if (Mouse.Captured == control)
            {
                Mouse.Capture(null);
            }

            control.DropDownClosed?.Invoke(control, EventArgs.Empty);
        }

        if (UIElementAutomationPeer.FromElement(control) is MultiSelectComboBoxAutomationPeer peer)
        {
            peer.RaiseExpandCollapseStateChanged((bool)e.OldValue, isOpen);
        }
    }

    private void Bind(
        DependencyObject target,
        DependencyProperty targetProperty,
        DependencyProperty sourceProperty,
        BindingMode mode = BindingMode.OneWay)
        => BindingOperations.SetBinding(
            target,
            targetProperty,
            new Binding
            {
                Path = new PropertyPath(sourceProperty),
                Source = this,
                Mode = mode,
            });

    private void Forward(
        DependencyProperty innerProperty,
        DependencyProperty outerProperty,
        BindingMode mode = BindingMode.OneWay)
        => Bind(PartCheckList, innerProperty, outerProperty, mode);

    private void OnMouseDownOutsideCapturedElement(
        object sender,
        MouseButtonEventArgs e)
        => SetCurrentValue(IsDropDownOpenProperty, false);

    private void OnLostMouseCaptureWithin(
        object sender,
        MouseEventArgs e)
    {
        if (!IsDropDownOpen || Mouse.Captured == this)
        {
            return;
        }

        if (ReferenceEquals(e.OriginalSource, this))
        {
            // The control lost the capture: keep the pop-up open only when a part of it took the capture
            // (the arrow, a check box, the filter box, a scroll bar).
            if (!IsWithin(Mouse.Captured as DependencyObject))
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }

            return;
        }

        if (!IsWithin(e.OriginalSource as DependencyObject))
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
            return;
        }

        // A part released the capture it took: take it back, so the next click outside still closes the pop-up.
        if (Mouse.Captured is null)
        {
            Mouse.Capture(this, CaptureMode.SubTree);
            e.Handled = true;
        }
        else if (!IsWithin(Mouse.Captured as DependencyObject))
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
        }
    }

    private bool IsWithin(DependencyObject? element)
        => element is Visual visual &&
           (ReferenceEquals(visual, this) ||
            IsAncestorOf(visual) ||
            ReferenceEquals(visual, PartPopupBorder) ||
            PartPopupBorder.IsAncestorOf(visual));

    private void FocusPopupContent()
    {
        if (!IsDropDownOpen || PartPopupBorder.IsKeyboardFocusWithin)
        {
            return;
        }

        if (PartCheckList.PartFilter.IsVisible)
        {
            PartCheckList.PartFilter.Focus();
            return;
        }

        var list = PartCheckList.PartList;
        if (list.Items.Count > 0)
        {
            list.UpdateLayout();
            if (list.ItemContainerGenerator.ContainerFromIndex(0) is ListBoxItem container)
            {
                container.Focus();
                return;
            }
        }

        if (PartCheckList.PartSelectAll.IsVisible)
        {
            PartCheckList.PartSelectAll.Focus();
        }
    }

    private void OnListSelectedKeysChanged(
        object? sender,
        ValueChangedEventArgs<IReadOnlyList<string>> e)
    {
        UpdateSummary();
        SelectedKeysChanged?.Invoke(
            this,
            new ValueChangedEventArgs<IReadOnlyList<string>>(
                Name,
                e.OldValue,
                e.NewValue));
    }

    private void OnSelectedKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
        => UpdateSummary();

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
        => UpdateSummary();

    private void UpdateSummary()
    {
        if (summaryTextBlock is null)
        {
            return;
        }

        var text = BuildSummaryText();
        var oldText = SummaryText;
        SummaryText = text;
        summaryTextBlock.Text = text;
        UpdateWatermark();

        if (!string.Equals(oldText, text, StringComparison.Ordinal) &&
            UIElementAutomationPeer.FromElement(this) is MultiSelectComboBoxAutomationPeer peer)
        {
            peer.RaiseValueChanged(oldText, text);
        }
    }

    private string BuildSummaryText()
    {
        var source = Items;
        var keys = SelectedKeys;
        tickedCount = 0;
        if (source is null || source.Count == 0 || keys is null || keys.Count == 0)
        {
            return NoneText ?? string.Empty;
        }

        var selectedSet = new HashSet<string>(keys, StringComparer.Ordinal);
        var names = source
            .Where(x => selectedSet.Contains(x.Key))
            .Select(x => x.Value ?? string.Empty)
            .ToList();

        tickedCount = names.Count;
        if (names.Count == 0)
        {
            return NoneText ?? string.Empty;
        }

        if (names.Count == source.Count)
        {
            return AllText ?? Atc.Wpf.Resources.Word.All;
        }

        if (names.Count <= MaxNamesInSummary)
        {
            return string.Join(", ", names);
        }

        return string.Format(
            CultureInfo.CurrentCulture,
            CountFormat ?? Atc.Wpf.Resources.Miscellaneous.CountOfSelected2,
            names.Count,
            source.Count);
    }

    private void UpdateWatermark()
    {
        if (summaryTextBlock is null)
        {
            return;
        }

        var nothingTicked = tickedCount == 0;
        if (nothingTicked && !string.IsNullOrEmpty(NoneText))
        {
            summaryTextBlock.Visibility = Visibility.Collapsed;
            PartWatermark.Text = NoneText;
            PartWatermark.Visibility = Visibility.Visible;
            return;
        }

        summaryTextBlock.Visibility = Visibility.Visible;
        PartWatermark.Text = TextBoxHelper.GetWatermark(this);
        PartWatermark.Visibility = nothingTicked
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}