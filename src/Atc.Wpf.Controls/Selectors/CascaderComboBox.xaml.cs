// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A multi-select drop-down for deep, uniform hierarchies (country › region › city). Closed, it looks like a
/// <see cref="ComboBox"/> and shows a summary of the ticked nodes (names, removable chips or a count); open, it shows
/// one column per level side by side, each row with a tri-state check box, so the path is always visible. Ticking a
/// node does not close the pop-up.
/// </summary>
/// <remarks>
/// <para>
/// Clicking the text of a row with children makes it active: its children are shown in the next column and the
/// deeper columns are dropped. Clicking the check box ticks the row (with everything under it when linked); on a leaf a
/// click anywhere ticks it. With <see cref="CascaderExpandTrigger.Hover"/>, resting the mouse on a row also makes it
/// active.
/// </para>
/// <para>
/// The check states are kept from <see cref="SelectedKeys"/> by the same engine as <see cref="CheckTreeView"/>. Ticks
/// made in the pop-up update <see cref="SelectedKeys"/> in place and raise <see cref="SelectedKeysChanged"/> once per
/// action; changes made to <see cref="SelectedKeys"/> from outside (or a new instance) only update the ticks and the
/// summary.
/// </para>
/// </remarks>
public partial class CascaderComboBox
{
    /// <summary>
    /// Identifies the <see cref="MaxDropDownHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxDropDownHeightProperty = ComboBox.MaxDropDownHeightProperty.AddOwner(
        typeof(CascaderComboBox));

    /// <summary>
    /// Identifies the <see cref="LoadChildrenAsync"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LoadChildrenAsyncProperty = DependencyProperty.Register(
        nameof(LoadChildrenAsync),
        typeof(Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>),
        typeof(CascaderComboBox),
        new PropertyMetadata(defaultValue: null));

    private static readonly DependencyPropertyKey SummaryTextPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(SummaryText),
        typeof(string),
        typeof(CascaderComboBox),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="SummaryText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SummaryTextProperty = SummaryTextPropertyKey.DependencyProperty;

    private readonly TextBlock summaryTextBlock;

    /// <summary>
    /// The root nodes (the first column).
    /// </summary>
    [DependencyProperty]
    private ObservableCollection<CheckTreeNode> nodes;

    /// <summary>
    /// The keys of the ticked nodes (which ones depends on <see cref="CheckedKeysStrategy"/>). Two-way by default;
    /// updated in place.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// Which ticked nodes <see cref="SelectedKeys"/> holds: the top-most fully ticked nodes (the default), only leaves,
    /// or every ticked node. Ignored when <see cref="IsCheckLinked"/> is <see langword="false"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = CheckedKeysStrategy.Parents)]
    private CheckedKeysStrategy checkedKeysStrategy;

    /// <summary>
    /// Whether parent and child check boxes are linked; <see langword="false"/> makes every node independent.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isCheckLinked;

    /// <summary>
    /// Whether several nodes can be ticked (the default). <see langword="false"/> is the single-select cascader: the
    /// check boxes are hidden, a click (or <c>Enter</c>) on a leaf makes <see cref="SelectedKeys"/> that one key and
    /// closes the pop-up, and a parent only opens its column. Switching to single keeps the first key.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isMultiSelect;

    /// <summary>
    /// What makes a row active (its children shown in the next column): a click
    /// (<see cref="CascaderExpandTrigger.Click"/>, the default) or also resting the mouse on it
    /// (<see cref="CascaderExpandTrigger.Hover"/>).
    /// </summary>
    [DependencyProperty(DefaultValue = CascaderExpandTrigger.Click)]
    private CascaderExpandTrigger expandTrigger;

    /// <summary>
    /// The width of each column in the pop-up.
    /// </summary>
    [DependencyProperty(DefaultValue = 180.0)]
    private double columnWidth;

    /// <summary>
    /// Whether the pop-up shows the filter box; <see langword="null"/> (the default) shows it when the tree has more
    /// than 20 nodes.
    /// </summary>
    [DependencyProperty]
    private bool? isFilterVisible;

    /// <summary>
    /// The text in the pop-up's filter box; when not empty, the columns are replaced by a flat list of the matching
    /// nodes from every level, each with its full path.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private string filterText;

    /// <summary>
    /// The maximum number of keys in <see cref="SelectedKeys"/>; <see langword="null"/> for no limit. A tick that
    /// would go beyond it is refused.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnSummaryInputChanged), ValidateValueCallback = nameof(ValidateMaxSelectionCount))]
    private int? maxSelectionCount;

    /// <summary>
    /// The separator between the node texts of a path, in filter results and (with <see cref="ShowFullPath"/>) in the
    /// summary and the chips.
    /// </summary>
    [DependencyProperty(DefaultValue = " / ", PropertyChangedCallback = nameof(OnChipsInputChanged))]
    private string pathSeparator;

    /// <summary>
    /// Whether the summary names and the chips show the full path of a node ("Denmark / Jutland / Aarhus") instead of
    /// its text.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnChipsInputChanged))]
    private bool showFullPath;

    /// <summary>
    /// The template for the text part of a pop-up row; its data context is the <see cref="CheckTreeNode"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// What the closed box shows for the ticked nodes: the names (<see cref="MultiSelectDisplayMode.Text"/>), a
    /// removable chip per key (<see cref="MultiSelectDisplayMode.Chips"/>) or only the count
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
    /// In <see cref="MultiSelectDisplayMode.Chips"/> mode, whether the chips wrap over several lines (no "+N").
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
    /// The summary when every leaf is ticked; <see langword="null"/> (the default) for the translated "All".
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
    /// The summary format when more than <see cref="MaxNamesInSummary"/> keys are ticked: <c>{0}</c> is the number of
    /// ticked leaves and <c>{1}</c> the number of leaves. <see langword="null"/> (the default) for the translated
    /// "{0} of {1} selected".
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
    private int keyCount;

    /// <summary>
    /// Occurs when the pop-up changes <see cref="SelectedKeys"/>: once per tick (a parent with all its descendants is
    /// one tick), chip removal, or loaded children that change the keys.
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
    /// Initializes a new instance of the <see cref="CascaderComboBox"/> class.
    /// </summary>
    public CascaderComboBox()
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

        PartColumns = new CascaderColumnView();
        PartColumnsHost.Child = PartColumns;

        ChipPanel = CreateChipPanel();

        SetCurrentValue(SelectedKeysProperty, new ObservableCollectionEx<string>());
        SetCurrentValue(NodesProperty, new ObservableCollection<CheckTreeNode>());

        // Options first, then SelectedKeys and Nodes last, so the first nodes are handled with the final options.
        Forward(CascaderColumnView.CheckedKeysStrategyProperty, CheckedKeysStrategyProperty);
        Forward(CascaderColumnView.IsCheckLinkedProperty, IsCheckLinkedProperty);
        Forward(CascaderColumnView.IsMultiSelectProperty, IsMultiSelectProperty);
        Forward(CascaderColumnView.ExpandTriggerProperty, ExpandTriggerProperty);
        Forward(CascaderColumnView.ColumnWidthProperty, ColumnWidthProperty);
        Forward(CascaderColumnView.IsFilterVisibleProperty, IsFilterVisibleProperty);
        Forward(CascaderColumnView.FilterTextProperty, FilterTextProperty, BindingMode.TwoWay);
        Forward(CascaderColumnView.MaxSelectionCountProperty, MaxSelectionCountProperty);
        Forward(CascaderColumnView.PathSeparatorProperty, PathSeparatorProperty);
        Forward(CascaderColumnView.ItemTemplateProperty, ItemTemplateProperty);
        Forward(CascaderColumnView.LoadChildrenAsyncProperty, LoadChildrenAsyncProperty);
        Forward(CascaderColumnView.SelectedKeysProperty, SelectedKeysProperty, BindingMode.TwoWay);
        Forward(CascaderColumnView.NodesProperty, NodesProperty);

        PartColumns.SelectedKeysChanged += OnColumnsSelectedKeysChanged;
        PartColumns.LeafPicked += OnColumnsLeafPicked;
        PartColumns.StateRefreshed += OnColumnsStateRefreshed;
        CultureManager.UiCultureChanged += OnUiCultureChanged;

        // Like ComboBox: while open, the control holds the mouse capture (sub-tree), so a click outside closes the
        // pop-up and a click on the box toggles it.
        Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideCapturedElement);
        AddHandler(Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCaptureWithin));

        UpdateSummary();
    }

    /// <summary>
    /// Gets or sets the maximum height of the pop-up; each column scrolls (virtualized) beyond it.
    /// </summary>
    public double MaxDropDownHeight
    {
        get => (double)GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the callback that loads the children of a node with <see cref="CheckTreeNode.HasUnloadedChildren"/>
    /// when the node becomes active; the next column shows a busy row meanwhile and activating another row cancels the
    /// load. Children of a ticked node inherit the tick.
    /// </summary>
    public Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>? LoadChildrenAsync
    {
        get => (Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>?)GetValue(LoadChildrenAsyncProperty);
        set => SetValue(LoadChildrenAsyncProperty, value);
    }

    /// <summary>
    /// Gets the text the closed box shows for the ticked nodes (also the UI Automation value).
    /// </summary>
    public string SummaryText
    {
        get => (string)GetValue(SummaryTextProperty);
        private set => SetValue(SummaryTextPropertyKey, value);
    }

    /// <summary>
    /// Gets the column view shown in the pop-up.
    /// </summary>
    internal CascaderColumnView PartColumns { get; }

    /// <summary>
    /// Gets how many times the summary was rebuilt (for change-storm tests).
    /// </summary>
    internal int SummaryUpdateCount { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the control is writing its own change into <c>SelectedKeys</c>; a wrapper that
    /// shares the collection validates once afterwards instead of once per notification.
    /// </summary>
    internal bool IsWritingSelectedKeys => PartColumns.IsWritingSelectedKeys;

    /// <summary>
    /// Defers the handling of changes to the tree (nodes added to or removed from <see cref="Nodes"/> or a
    /// <see cref="CheckTreeNode.Children"/> collection) until the returned scope is disposed; each changed parent is
    /// then applied once. Wrap the adding of many nodes in it:
    /// <code>using (tree.DeferRefresh()) { foreach (var node in nodes) { parent.Children.Add(node); } }</code>
    /// </summary>
    /// <returns>The scope; dispose it to apply the changes.</returns>
    public IDisposable DeferRefresh()
        => PartColumns.DeferRefresh();

    /// <summary>
    /// Returns the node with a key, or <see langword="null"/> when it is not in the tree.
    /// </summary>
    /// <param name="key">The node key.</param>
    /// <returns>The node, or <see langword="null"/>.</returns>
    public CheckTreeNode? FindNode(string key)
        => PartColumns.FindNode(key);

    /// <summary>
    /// Toggles a node as a click on its check box does. Raises <see cref="SelectedKeysChanged"/> once.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns><see langword="true"/> when <see cref="SelectedKeys"/> changed.</returns>
    public bool ToggleNode(CheckTreeNode node)
        => PartColumns.ToggleNode(node);

    /// <summary>
    /// Ticks every enabled node in one change.
    /// </summary>
    public void SelectAll()
        => PartColumns.SelectAll();

    /// <summary>
    /// Unticks every enabled node in one change.
    /// </summary>
    public void ClearSelection()
        => PartColumns.ClearSelection();

    /// <summary>
    /// Opens or closes the pop-up for a key press: <c>F4</c> and <c>Alt+Down</c> / <c>Alt+Up</c> toggle it;
    /// <c>Esc</c> clears the filter text first, then closes it; <c>Enter</c> closes it unless a row has the focus (on a
    /// row it activates or ticks); <c>Backspace</c> removes the last chip in chips mode.
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

        if (IsDropDownOpen && key == Key.Escape)
        {
            if (!string.IsNullOrEmpty(FilterText))
            {
                SetCurrentValue(FilterTextProperty, string.Empty);
                PartColumns.FlushPendingFilter();
                return true;
            }

            SetCurrentValue(IsDropDownOpenProperty, false);
            return true;
        }

        if (IsDropDownOpen && key == Key.Enter && !PartColumns.HasRowFocus)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
            return true;
        }

        if (key == Key.Back &&
            modifiers == ModifierKeys.None &&
            !IsDropDownOpen &&
            DisplayMode == MultiSelectDisplayMode.Chips)
        {
            return RemoveLastChip();
        }

        return false;
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new CascaderComboBoxAutomationPeer(this);

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
        => ((CascaderComboBox)d).UpdateSummary();

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CascaderComboBox)d;
        if (control.subscribedSelectedKeys is not null)
        {
            CollectionChangedEventManager.RemoveHandler(control.subscribedSelectedKeys, control.OnSelectedKeysCollectionChanged);
        }

        control.subscribedSelectedKeys = e.NewValue as ObservableCollection<string>;
        if (control.subscribedSelectedKeys is not null)
        {
            CollectionChangedEventManager.AddHandler(control.subscribedSelectedKeys, control.OnSelectedKeysCollectionChanged);
        }

        control.UpdateSummary();
    }

    /// <summary>
    /// Caps the pop-up at the width of the work area of the monitor the box is on (read on each open, so moving the
    /// window to another monitor is followed).
    /// </summary>
    internal void UpdatePopupMaxWidth()
    {
        var workArea = Atc.Wpf.Controls.Layouts.DockScreenHelper.GetWorkAreaAt(
            this,
            new Point(ActualWidth / 2, ActualHeight / 2));
        PartPopupBorder.MaxWidth = System.Math.Max(200, workArea.Width - 16);
    }

    private static void OnIsDropDownOpenChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CascaderComboBox)d;
        var isOpen = (bool)e.NewValue;

        if (isOpen)
        {
            control.UpdatePopupMaxWidth();
            Mouse.Capture(control, CaptureMode.SubTree);
            _ = control.Dispatcher.BeginInvoke(DispatcherPriority.Input, control.FocusPopupContent);
            control.DropDownOpened?.Invoke(control, EventArgs.Empty);
        }
        else
        {
            control.PartColumns.CancelPendingHover();
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

        if (UIElementAutomationPeer.FromElement(control) is CascaderComboBoxAutomationPeer peer)
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
        => Bind(PartColumns, innerProperty, outerProperty, mode);

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
            // (the arrow, the filter box, a scroll bar).
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

        PartColumns.UpdateLayout();
        PartColumns.FocusFirstElement();
    }

    private void OnColumnsLeafPicked(
        object? sender,
        EventArgs e)
        => SetCurrentValue(IsDropDownOpenProperty, false);

    private void OnColumnsSelectedKeysChanged(
        object? sender,
        ValueChangedEventArgs<IReadOnlyList<string>> e)
    {
        UpdateSummary();
        RaiseSelectedKeysChanged(e.OldValue, e.NewValue);
    }

    private void OnColumnsStateRefreshed(
        object? sender,
        EventArgs e)
        => UpdateSummary();

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
        // A change written by the pop-up is summarised once, after the write.
        if (PartColumns.IsWritingSelectedKeys && ReferenceEquals(PartColumns.SelectedKeys, sender))
        {
            return;
        }

        UpdateSummary();
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        chipsSignature = null;
        UpdateSummary();
    }

    private void UpdateSummary()
    {
        if (summaryTextBlock is null)
        {
            return;
        }

        SummaryUpdateCount++;

        var text = BuildSummaryText();
        var oldText = SummaryText;
        SummaryText = text;
        summaryTextBlock.Text = text;
        UpdateChips();
        UpdateWatermark();

        if (!string.Equals(oldText, text, StringComparison.Ordinal) &&
            UIElementAutomationPeer.FromElement(this) is CascaderComboBoxAutomationPeer peer)
        {
            peer.RaiseValueChanged(oldText, text);
        }
    }

    private string BuildSummaryText()
    {
        var keys = SelectedKeys;
        keyCount = keys?.Count ?? 0;
        if (keys is null || keyCount == 0 || PartColumns is null)
        {
            return NoneText ?? string.Empty;
        }

        var names = keys
            .Select(GetDisplayText)
            .ToList();

        if (DisplayMode == MultiSelectDisplayMode.Chips)
        {
            return string.Join(", ", names);
        }

        var (ticked, total) = PartColumns.State.CountLeaves();
        if (total > 0 && ticked == total)
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
            ticked,
            total);
    }

    private string GetDisplayText(string key)
    {
        if (PartColumns.FindNode(key) is not { } node)
        {
            return key;
        }

        return ShowFullPath
            ? PartColumns.GetFullPath(node, PathSeparator)
            : node.Text;
    }

    private void UpdateWatermark()
    {
        if (summaryTextBlock is null)
        {
            return;
        }

        var nothingTicked = keyCount == 0;
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

    private static bool ValidateMaxSelectionCount(object? value)
        => KeySelectionHelper.IsValidMaxSelectionCount(value);
}