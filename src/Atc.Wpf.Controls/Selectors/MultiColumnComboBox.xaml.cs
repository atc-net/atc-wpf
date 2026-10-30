// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A multi-select drop-down for choosing items by comparing several attributes (number, name, city). Closed, it looks
/// like a <see cref="ComboBox"/> and shows a summary of the ticked items (names, removable chips or a count); open, it
/// shows a small sortable grid with a check-box column, an optional filter box and a "{0} of {1} selected" footer.
/// Ticking a row does not close the pop-up.
/// </summary>
/// <remarks>
/// <para>
/// The rows are the objects of <see cref="ItemsSource"/>; <see cref="KeyMemberPath"/> gives each its key (kept in
/// <see cref="SelectedKeys"/>) and <see cref="DisplayMemberPath"/> its name (the summary, chips and the automation name
/// of its check box). The grid shows the <see cref="DataGridColumn"/>s of <see cref="Columns"/>, or, with
/// <see cref="AutoGenerateColumns"/>, one text column per public property.
/// </para>
/// <para>
/// A <see cref="DataGridColumn"/> can belong to one <see cref="DataGrid"/> only: the columns of <see cref="Columns"/>
/// are moved into the pop-up grid, so a column instance must not be shared with another grid or control. Removing it
/// from <see cref="Columns"/> releases it.
/// </para>
/// <para>
/// Ticks made in the pop-up update <see cref="SelectedKeys"/> in place and raise <see cref="SelectedKeysChanged"/> once
/// per action; changes made to <see cref="SelectedKeys"/> from outside (or a new instance) only update the ticks and the
/// summary. Sorting and filtering never change <see cref="SelectedKeys"/>.
/// </para>
/// </remarks>
public partial class MultiColumnComboBox
{
    /// <summary>
    /// Identifies the <see cref="MaxDropDownHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxDropDownHeightProperty = ComboBox.MaxDropDownHeightProperty.AddOwner(
        typeof(MultiColumnComboBox));

    private static readonly DependencyPropertyKey SummaryTextPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(SummaryText),
        typeof(string),
        typeof(MultiColumnComboBox),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="SummaryText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SummaryTextProperty = SummaryTextPropertyKey.DependencyProperty;

    private static readonly DependencyPropertyKey SelectedItemsPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(SelectedItems),
        typeof(IReadOnlyList<object>),
        typeof(MultiColumnComboBox),
        new PropertyMetadata(Array.Empty<object>()));

    /// <summary>
    /// Identifies the <see cref="SelectedItems"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;

    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(System.Collections.IEnumerable),
        typeof(MultiColumnComboBox),
        new PropertyMetadata(defaultValue: null, OnItemsSourceChanged));

    private readonly TextBlock summaryTextBlock;

    /// <summary>
    /// The (dotted) property path of an item's key; the value is turned into a string. Empty uses the item's
    /// <see cref="object.ToString"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = "", PropertyChangedCallback = nameof(OnKeyMemberPathChanged))]
    private string keyMemberPath;

    /// <summary>
    /// The (dotted) property path of an item's name, used by the summary, the chips and the automation name of the
    /// row's check box. Empty uses the item's <see cref="object.ToString"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = "", PropertyChangedCallback = nameof(OnDisplayMemberPathChanged))]
    private string displayMemberPath;

    /// <summary>
    /// The grid columns after the check-box column, as on <see cref="DataGrid"/>. The columns are moved into the
    /// pop-up grid, so a column instance belongs to this control only.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnColumnsChanged))]
    private ObservableCollection<DataGridColumn> columns;

    /// <summary>
    /// Whether, when <see cref="Columns"/> is empty, the grid shows one text column per public property of the items.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnAutoGenerateColumnsChanged))]
    private bool autoGenerateColumns;

    /// <summary>
    /// The keys of the ticked items. Two-way by default; updated in place. Keys whose item is not (yet) in
    /// <see cref="ItemsSource"/> are kept until an item with that key is removed.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// The maximum number of ticked items; <see langword="null"/> for no limit. At the limit the unticked rows are
    /// disabled.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnRowStateInputChanged))]
    private int? maxSelectionCount;

    /// <summary>
    /// The (dotted) path of a <see cref="bool"/> property telling whether an item can be toggled; rows whose value is
    /// <see langword="false"/> are shown but cannot be ticked or unticked. Empty enables every row.
    /// </summary>
    [DependencyProperty(DefaultValue = "", PropertyChangedCallback = nameof(OnRowStateInputChanged))]
    private string isEnabledMemberPath;

    /// <summary>
    /// Whether the pop-up shows the filter box; <see langword="null"/> (the default) shows it when there are more than
    /// 10 items.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnIsFilterVisibleChanged))]
    private bool? isFilterVisible;

    /// <summary>
    /// The text in the pop-up's filter box; only rows where a visible column (or a path of
    /// <see cref="FilterMemberPaths"/>) contains it, ignoring case, are shown.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnFilterTextChanged))]
    private string filterText;

    /// <summary>
    /// Comma-separated (dotted) property paths the filter searches instead of the visible columns;
    /// <see langword="null"/> (the default) searches the visible columns.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnFilterMemberPathsChanged))]
    private string? filterMemberPaths;

    /// <summary>
    /// Whether the ticked rows are shown first when the pop-up opens (any column sort is cleared then). Rows do not
    /// move while ticking.
    /// </summary>
    [DependencyProperty]
    private bool keepSelectedOnTop;

    /// <summary>
    /// The width of the pop-up; <see cref="double.NaN"/> (the default) makes it at least as wide as the box and as wide
    /// as the columns need, up to the screen width.
    /// </summary>
    [DependencyProperty(DefaultValue = double.NaN)]
    private double dropDownWidth;

    /// <summary>
    /// When ticks in the pop-up reach <see cref="SelectedKeys"/>: right away
    /// (<see cref="MultiSelectCommitMode.Immediate"/>), or on OK (<see cref="MultiSelectCommitMode.Deferred"/>, the
    /// pop-up then shows OK and Cancel; Cancel, Esc and a click outside discard the ticks, Enter is OK).
    /// </summary>
    [DependencyProperty(DefaultValue = MultiSelectCommitMode.Immediate)]
    private MultiSelectCommitMode commitMode;

    /// <summary>
    /// What the closed box shows for the ticked items: the names (<see cref="MultiSelectDisplayMode.Text"/>), a
    /// removable chip per item (<see cref="MultiSelectDisplayMode.Chips"/>) or only the count
    /// (<see cref="MultiSelectDisplayMode.Count"/>).
    /// </summary>
    [DependencyProperty(
        DefaultValue = MultiSelectDisplayMode.Text,
        PropertyChangedCallback = nameof(OnChipsInputChanged))]
    private MultiSelectDisplayMode displayMode;

    /// <summary>
    /// In <see cref="MultiSelectDisplayMode.Chips"/> mode, the most chips shown before the "+N" chip;
    /// <see langword="null"/> (the default) shows as many as fit on one line.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnChipsInputChanged))]
    private int? maxVisibleChips;

    /// <summary>
    /// In <see cref="MultiSelectDisplayMode.Chips"/> mode, whether the chips wrap over several lines (the box grows in
    /// height and shows every chip, with no "+N").
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnChipsInputChanged))]
    private bool chipsWrap;

    /// <summary>
    /// In <see cref="MultiSelectDisplayMode.Chips"/> mode, the template of a chip; its data context is a
    /// <see cref="MultiSelectChipItem"/>. <see langword="null"/> (the default) uses a small input <c>Chip</c>.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnChipsInputChanged))]
    private DataTemplate? chipTemplate;

    /// <summary>
    /// The most names the summary lists (comma-separated) before it switches to <see cref="CountFormat"/>.
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
    /// Occurs when the pop-up (or a chip) changes <see cref="SelectedKeys"/>: once per row toggle, header
    /// "select all", <c>Ctrl+A</c>, chip removal, OK (in <see cref="MultiSelectCommitMode.Deferred"/> mode), or item
    /// change that drops keys.
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
    /// Initializes a new instance of the <see cref="MultiColumnComboBox"/> class.
    /// </summary>
    public MultiColumnComboBox()
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
        Bind(PartPopupBorder, WidthProperty, DropDownWidthProperty);
        Bind(PartPopupBorder, MaxHeightProperty, MaxDropDownHeightProperty);
        Bind(PartPopupBorder, FlowDirectionProperty, FlowDirectionProperty);
        PartPopupBorder.MaxWidth = SystemParameters.VirtualScreenWidth;
        PartPopup.PlacementTarget = this;
        Bind(PartFilter, TextBox.TextProperty, FilterTextProperty, BindingMode.TwoWay);

        ChipPanel = CreateChipPanel();
        InitializeGrid();
        ApplyTexts();

        SetCurrentValue(SelectedKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(ColumnsProperty, new ObservableCollection<DataGridColumn>());

        CultureManager.UiCultureChanged += OnUiCultureChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        // Like ComboBox: while open, the control holds the mouse capture (sub-tree), so a click outside closes the
        // pop-up and a click on the box toggles it.
        Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideCapturedElement);
        AddHandler(Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCaptureWithin));

        UpdateSummary();
    }

    /// <summary>
    /// Gets or sets the objects shown as rows. An <see cref="INotifyCollectionChanged"/> source is followed; keys of
    /// removed items are dropped from <see cref="SelectedKeys"/>.
    /// </summary>
    public System.Collections.IEnumerable? ItemsSource
    {
        get => (System.Collections.IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum height of the pop-up; the grid scrolls (virtualized) beyond it.
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
    /// Gets the ticked items, in the order of <see cref="SelectedKeys"/> (keys without an item are skipped).
    /// </summary>
    public IReadOnlyList<object> SelectedItems
    {
        get => (IReadOnlyList<object>)GetValue(SelectedItemsProperty);
        private set => SetValue(SelectedItemsPropertyKey, value);
    }

    /// <summary>
    /// Opens or closes the pop-up and acts on the grid for a key press: <c>F4</c> and <c>Alt+Down</c> / <c>Alt+Up</c>
    /// toggle the pop-up; while open, <c>Space</c> toggles the focused row, <c>Ctrl+A</c> ticks all shown rows,
    /// <c>Home</c> / <c>End</c> move to the first / last row, <c>Enter</c> closes (OK when deferred) and <c>Esc</c>
    /// clears the filter text first, then closes (Cancel when deferred).
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

        if (IsDropDownOpen)
        {
            return HandleOpenKey(key, modifiers);
        }

        if (key == Key.Back &&
            modifiers == ModifierKeys.None &&
            DisplayMode == MultiSelectDisplayMode.Chips)
        {
            return RemoveLastChip();
        }

        return false;
    }

    /// <summary>
    /// Sends typed text to the filter box: while closed it opens the pop-up first; while the grid has the focus it
    /// moves the focus to the filter box. Does nothing when the filter box is hidden or already has the focus.
    /// </summary>
    /// <returns><see langword="true"/> when the text was used.</returns>
    internal bool HandleTextInput(string? text)
    {
        if (string.IsNullOrEmpty(text) ||
            text.Any(char.IsControl) ||
            !IsFilterShown ||
            PartFilter.IsKeyboardFocusWithin ||
            (!IsDropDownOpen && string.IsNullOrWhiteSpace(text)))
        {
            return false;
        }

        if (!IsDropDownOpen)
        {
            SetCurrentValue(IsDropDownOpenProperty, true);
        }

        SetCurrentValue(FilterTextProperty, (FilterText ?? string.Empty) + text);
        _ = Dispatcher.BeginInvoke(DispatcherPriority.Input, FocusFilterAtEnd);
        return true;
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new MultiColumnComboBoxAutomationPeer(this);

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
    protected override void OnPreviewTextInput(TextCompositionEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewTextInput(e);
        if (!e.Handled && HandleTextInput(e.Text))
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
        => ((MultiColumnComboBox)d).UpdateSummary();

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiColumnComboBox)d;
        if (control.subscribedSelectedKeys is not null)
        {
            control.subscribedSelectedKeys.CollectionChanged -= control.OnSelectedKeysCollectionChanged;
        }

        control.subscribedSelectedKeys = e.NewValue as ObservableCollection<string>;
        if (control.subscribedSelectedKeys is not null)
        {
            control.subscribedSelectedKeys.CollectionChanged += control.OnSelectedKeysCollectionChanged;
        }

        control.OnSelectionChanged();
    }

    private static void OnIsDropDownOpenChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (MultiColumnComboBox)d;
        var isOpen = (bool)e.NewValue;

        if (isOpen)
        {
            control.BeginDraft();
            control.ApplyKeepSelectedOnTop();
            control.RefreshPopupState();
            Mouse.Capture(control, CaptureMode.SubTree);
            _ = control.Dispatcher.BeginInvoke(DispatcherPriority.Input, control.FocusPopupContent);
            control.DropDownOpened?.Invoke(control, EventArgs.Empty);
        }
        else
        {
            control.FlushPendingFilter();
            control.EndDraft();
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

        if (UIElementAutomationPeer.FromElement(control) is MultiColumnComboBoxAutomationPeer peer)
        {
            peer.RaiseExpandCollapseStateChanged((bool)e.OldValue, isOpen);
        }
    }

    private bool HandleOpenKey(
        Key key,
        ModifierKeys modifiers)
    {
        var isInFilter = PartFilter.IsKeyboardFocusWithin;
        switch (key)
        {
            case Key.Enter:
                CommitDraftAndClose();
                return true;
            case Key.Escape when !string.IsNullOrEmpty(FilterText):
                SetCurrentValue(FilterTextProperty, string.Empty);
                FlushPendingFilter();
                return true;
            case Key.Escape:
                SetCurrentValue(IsDropDownOpenProperty, false);
                return true;
            case Key.Space when modifiers == ModifierKeys.None && !isInFilter:
                return ToggleFocusedRow();
            case Key.A when modifiers == ModifierKeys.Control && !isInFilter:
                SelectAll();
                return true;
            case Key.Home when modifiers == ModifierKeys.None && !isInFilter:
                return FocusRowAt(0);
            case Key.End when modifiers == ModifierKeys.None && !isInFilter:
                return FocusRowAt(PartGrid.Items.Count - 1);
            default:
                return false;
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
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
        => ApplyGridStyles();

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
        => filterTimer?.Stop();

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
            // (the arrow, the grid, a column gripper, the filter box, a scroll bar).
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

        if (IsFilterShown)
        {
            PartFilter.Focus();
            return;
        }

        if (!FocusRowAt(0))
        {
            PartGrid.Focus();
        }
    }

    private void FocusFilterAtEnd()
    {
        if (!IsDropDownOpen || !IsFilterShown)
        {
            return;
        }

        PartFilter.Focus();
        PartFilter.CaretIndex = PartFilter.Text.Length;
    }

    private void OnFilterPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key == Key.Down && Keyboard.Modifiers == ModifierKeys.None)
        {
            var index = PartGrid.SelectedIndex >= 0 ? PartGrid.SelectedIndex : 0;
            e.Handled = FocusRowAt(index);
        }
    }

    private void RaiseSelectedKeysChanged(
        IReadOnlyList<string> oldKeys,
        IReadOnlyList<string> newKeys)
        => SelectedKeysChanged?.Invoke(
            this,
            new ValueChangedEventArgs<IReadOnlyList<string>>(
                Name,
                oldKeys,
                newKeys));

    private void OnSelectedKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (!isWritingSelectedKeys)
        {
            OnSelectionChanged();
        }
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        ApplyTexts();
        chipsSignature = null;
        UpdateSummary();
        RefreshPopupState();
    }

    private void ApplyTexts()
    {
        PartOkButton.Content = Atc.Wpf.Resources.Word.Ok;
        PartCancelButton.Content = Atc.Wpf.Resources.Word.Cancel;
        PartNoMatches.Text = Atc.Wpf.Resources.Miscellaneous.NoMatches;
        TextBoxHelper.SetWatermark(PartFilter, Atc.Wpf.Resources.Word.Filter);
        AutomationProperties.SetName(PartFilter, Atc.Wpf.Resources.Word.Filter);
        AutomationProperties.SetName(HeaderCheckBox, Atc.Wpf.Resources.Miscellaneous.SelectAll);
        HeaderCheckBox.ToolTip = Atc.Wpf.Resources.Miscellaneous.SelectAll;
    }

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
        UpdateChips();
        UpdateWatermark();

        if (!string.Equals(oldText, text, StringComparison.Ordinal) &&
            UIElementAutomationPeer.FromElement(this) is MultiColumnComboBoxAutomationPeer peer)
        {
            peer.RaiseValueChanged(oldText, text);
        }
    }

    private string BuildSummaryText()
    {
        var names = GetTickedItems()
            .Select(x => x.Text)
            .ToList();

        tickedCount = names.Count;
        if (names.Count == 0)
        {
            return NoneText ?? string.Empty;
        }

        if (DisplayMode == MultiSelectDisplayMode.Chips)
        {
            return string.Join(", ", names);
        }

        var total = itemsByKey.Count;
        if (names.Count == total)
        {
            return AllText ?? Atc.Wpf.Resources.Word.All;
        }

        if (DisplayMode == MultiSelectDisplayMode.Text &&
            names.Count <= MaxNamesInSummary)
        {
            return string.Join(", ", names);
        }

        return string.Format(
            CultureInfo.CurrentCulture,
            CountFormat ?? Atc.Wpf.Resources.Miscellaneous.CountOfSelected2,
            names.Count,
            total);
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