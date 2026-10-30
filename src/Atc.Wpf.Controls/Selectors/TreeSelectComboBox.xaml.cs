// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A drop-down with a tri-state check tree. Closed, it looks like a <see cref="ComboBox"/> and shows a summary of the
/// ticked nodes (names, removable chips or a count); open, it shows a <see cref="CheckTreeView"/>. Ticking a node does
/// not close the pop-up.
/// </summary>
/// <remarks>
/// The pop-up tree is a <see cref="CheckTreeView"/>; the tree options are passed through to it. Ticks made in the pop-up
/// update <see cref="SelectedKeys"/> in place and raise <see cref="SelectedKeysChanged"/> once per action. Changes made to
/// <see cref="SelectedKeys"/> from outside (or a new instance) only update the ticks and the summary.
/// </remarks>
public partial class TreeSelectComboBox
{
    /// <summary>
    /// Identifies the <see cref="MaxDropDownHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxDropDownHeightProperty = ComboBox.MaxDropDownHeightProperty.AddOwner(
        typeof(TreeSelectComboBox));

    /// <summary>
    /// Identifies the <see cref="LoadChildrenAsync"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LoadChildrenAsyncProperty = CheckTreeView.LoadChildrenAsyncProperty.AddOwner(
        typeof(TreeSelectComboBox));

    private static readonly DependencyPropertyKey SummaryTextPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(SummaryText),
        typeof(string),
        typeof(TreeSelectComboBox),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="SummaryText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SummaryTextProperty = SummaryTextPropertyKey.DependencyProperty;

    private readonly TextBlock summaryTextBlock;

    /// <summary>
    /// The root nodes.
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
    /// Which ticked nodes <see cref="SelectedKeys"/> holds: only leaves (the default), the top-most fully ticked nodes,
    /// or every ticked node. Ignored when <see cref="IsCheckLinked"/> is <see langword="false"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = CheckedKeysStrategy.Leaves)]
    private CheckedKeysStrategy checkedKeysStrategy;

    /// <summary>
    /// Whether parent and child check boxes are linked; <see langword="false"/> makes every node independent.
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool isCheckLinked;

    /// <summary>
    /// Whether the pop-up shows the filter box; <see langword="null"/> (the default) shows it when the tree has more
    /// than 20 nodes.
    /// </summary>
    [DependencyProperty]
    private bool? isFilterVisible;

    /// <summary>
    /// The text in the pop-up's filter box.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private string filterText;

    /// <summary>
    /// How filter matches are shown: in the tree with their ancestors, or as a flat list with their path.
    /// </summary>
    [DependencyProperty(DefaultValue = TreeFilterMode.KeepAncestors)]
    private TreeFilterMode filterMode;

    /// <summary>
    /// Whether the pop-up shows the row with "Select all", the count and "Clear" above the tree.
    /// </summary>
    [DependencyProperty]
    private bool showSelectAllRow;

    /// <summary>
    /// The maximum number of keys in <see cref="SelectedKeys"/>; <see langword="null"/> for no limit.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnSummaryInputChanged), ValidateValueCallback = nameof(ValidateMaxSelectionCount))]
    private int? maxSelectionCount;

    /// <summary>
    /// The number of tree levels expanded when <see cref="Nodes"/> is set.
    /// </summary>
    [DependencyProperty(DefaultValue = 1)]
    private int expandDepth;

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
    /// Occurs when the pop-up tree changes <see cref="SelectedKeys"/>: once per tick (a parent with all its children is
    /// one tick), Select all, Clear, chip removal, or loaded children that change the keys.
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
    /// Initializes a new instance of the <see cref="TreeSelectComboBox"/> class.
    /// </summary>
    public TreeSelectComboBox()
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

        PartTree = new CheckTreeView();
        PartTreeHost.Child = PartTree;

        ChipPanel = CreateChipPanel();

        SetCurrentValue(SelectedKeysProperty, new ObservableCollectionEx<string>());
        SetCurrentValue(NodesProperty, new ObservableCollection<CheckTreeNode>());

        // Options first, then SelectedKeys and Nodes last, so the first nodes are handled with the final options.
        Forward(CheckTreeView.CheckedKeysStrategyProperty, CheckedKeysStrategyProperty);
        Forward(CheckTreeView.IsCheckLinkedProperty, IsCheckLinkedProperty);
        Forward(CheckTreeView.IsFilterVisibleProperty, IsFilterVisibleProperty);
        Forward(CheckTreeView.FilterTextProperty, FilterTextProperty, BindingMode.TwoWay);
        Forward(CheckTreeView.FilterModeProperty, FilterModeProperty);
        Forward(CheckTreeView.ShowSelectAllRowProperty, ShowSelectAllRowProperty);
        Forward(CheckTreeView.MaxSelectionCountProperty, MaxSelectionCountProperty);
        Forward(CheckTreeView.ExpandDepthProperty, ExpandDepthProperty);
        Forward(CheckTreeView.ItemTemplateProperty, ItemTemplateProperty);
        Forward(CheckTreeView.LoadChildrenAsyncProperty, LoadChildrenAsyncProperty);
        Forward(CheckTreeView.SelectedKeysProperty, SelectedKeysProperty, BindingMode.TwoWay);
        Forward(CheckTreeView.NodesProperty, NodesProperty);

        PartTree.SelectedKeysChanged += OnTreeSelectedKeysChanged;
        PartTree.StateRefreshed += OnTreeStateRefreshed;
        CultureManager.UiCultureChanged += OnUiCultureChanged;

        // Like ComboBox: while open, the control holds the mouse capture (sub-tree), so a click outside closes the
        // pop-up and a click on the box toggles it.
        Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideCapturedElement);
        AddHandler(Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCaptureWithin));

        UpdateSummary();
    }

    /// <summary>
    /// Gets or sets the maximum height of the pop-up; the tree scrolls (virtualized) beyond it.
    /// </summary>
    public double MaxDropDownHeight
    {
        get => (double)GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the callback that loads the children of a node with <see cref="CheckTreeNode.HasUnloadedChildren"/>
    /// on its first expand; children of a ticked node inherit the tick.
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
    /// Gets the check tree shown in the pop-up.
    /// </summary>
    internal CheckTreeView PartTree { get; }

    /// <summary>
    /// Gets how many times the summary was rebuilt (for change-storm tests).
    /// </summary>
    internal int SummaryUpdateCount { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the control is writing its own change into <c>SelectedKeys</c>; a wrapper that
    /// shares the collection validates once afterwards instead of once per notification.
    /// </summary>
    internal bool IsWritingSelectedKeys => PartTree.IsWritingSelectedKeys;

    /// <summary>
    /// Returns the node with a key, or <see langword="null"/> when it is not in the tree.
    /// </summary>
    /// <param name="key">The node key.</param>
    /// <returns>The node, or <see langword="null"/>.</returns>
    public CheckTreeNode? FindNode(string key)
        => PartTree.FindNode(key);

    /// <summary>
    /// Toggles a node as a click in the pop-up does. Raises <see cref="SelectedKeysChanged"/> once.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns><see langword="true"/> when <see cref="SelectedKeys"/> changed.</returns>
    public bool ToggleNode(CheckTreeNode node)
        => PartTree.ToggleNode(node);

    /// <summary>
    /// Ticks every shown, enabled node in one change.
    /// </summary>
    public void SelectAll()
        => PartTree.SelectAll();

    /// <summary>
    /// Unticks every enabled node in one change.
    /// </summary>
    public void ClearSelection()
        => PartTree.ClearSelection();

    /// <summary>
    /// Opens or closes the pop-up for a key press: <c>F4</c> and <c>Alt+Down</c> / <c>Alt+Up</c> toggle it,
    /// <c>Esc</c> and <c>Enter</c> close it; <c>Backspace</c> removes the last chip in chips mode.
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
        => new TreeSelectComboBoxAutomationPeer(this);

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
        => ((TreeSelectComboBox)d).UpdateSummary();

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (TreeSelectComboBox)d;
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

    private static void OnIsDropDownOpenChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (TreeSelectComboBox)d;
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

        if (UIElementAutomationPeer.FromElement(control) is TreeSelectComboBoxAutomationPeer peer)
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
        => Bind(PartTree, innerProperty, outerProperty, mode);

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
            // (the arrow, an expander, the filter box, a scroll bar).
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

        if (PartTree.PartFilter.IsVisible)
        {
            PartTree.PartFilter.Focus();
            return;
        }

        var tree = PartTree.PartTree;
        if (tree.Items.Count > 0)
        {
            tree.UpdateLayout();
            if (tree.ItemContainerGenerator.ContainerFromIndex(0) is TreeViewItem container)
            {
                container.Focus();
            }
        }
    }

    private void OnTreeSelectedKeysChanged(
        object? sender,
        ValueChangedEventArgs<IReadOnlyList<string>> e)
    {
        UpdateSummary();
        RaiseSelectedKeysChanged(e.OldValue, e.NewValue);
    }

    private void OnTreeStateRefreshed(
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
        if (PartTree.IsWritingSelectedKeys && ReferenceEquals(PartTree.SelectedKeys, sender))
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
            UIElementAutomationPeer.FromElement(this) is TreeSelectComboBoxAutomationPeer peer)
        {
            peer.RaiseValueChanged(oldText, text);
        }
    }

    private string BuildSummaryText()
    {
        var keys = SelectedKeys;
        keyCount = keys?.Count ?? 0;
        if (keys is null || keyCount == 0 || PartTree is null)
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

        var (ticked, total) = PartTree.State.CountLeaves();
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
        => PartTree.FindNode(key)?.Text ?? key;

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