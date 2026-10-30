// ReSharper disable InvertIf
#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The column view of a <see cref="CascaderComboBox"/>: one column per level of a tri-state check tree, side by side.
/// The first column shows the roots; each active row (one per column) shows its children in the next column. Typing in
/// the filter box replaces the columns with one flat list of matches from every level, each with its full path.
/// </summary>
/// <remarks>
/// <para>
/// The check states are kept by a <see cref="CheckTreeState"/> from <see cref="SelectedKeys"/>, exactly as in
/// <see cref="CheckTreeView"/>; only the presentation differs. Ticks made in the view update <see cref="SelectedKeys"/>
/// in place and raise <see cref="SelectedKeysChanged"/> once per action; keys set from code raise no event.
/// </para>
/// <para>
/// The view is a separate element (not part of the drop-down) so it could be shown inline or reused for a single-select
/// cascader later.
/// </para>
/// </remarks>
internal partial class CascaderColumnView
{
    /// <summary>
    /// Identifies the <see cref="LoadChildrenAsync"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LoadChildrenAsyncProperty = DependencyProperty.Register(
        nameof(LoadChildrenAsync),
        typeof(Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>),
        typeof(CascaderColumnView),
        new PropertyMetadata(defaultValue: null, OnLoadChildrenAsyncChanged));

    private static readonly DependencyPropertyKey IsAtSelectionLimitPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsAtSelectionLimit),
        typeof(bool),
        typeof(CascaderColumnView),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <see cref="IsAtSelectionLimit"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsAtSelectionLimitProperty = IsAtSelectionLimitPropertyKey.DependencyProperty;

    private const int AutoFilterNodeCount = 20;

    private static readonly TimeSpan HoverDelay = TimeSpan.FromMilliseconds(150);

    private readonly CheckTreeState state = new();
    private readonly List<CheckTreeNode> activePath = [];
    private readonly List<ObservableCollectionEx<CheckTreeNode>> columnItems = [];
    private readonly List<CascaderColumnList> columnLists = [];
    private readonly ObservableCollectionEx<CheckTreeNode> filterResults = [];
    private readonly Dictionary<CheckTreeNode, CheckTreeNode> placeholders = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<CheckTreeNode> placeholderSet = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<CascaderColumnItem> realizedContainers = [];
    private readonly CascaderColumnList filterList;

    /// <summary>
    /// The root nodes (the first column).
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnNodesChanged))]
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
    /// Which ticked nodes <see cref="SelectedKeys"/> holds; the top-most fully ticked nodes by default.
    /// </summary>
    [DependencyProperty(
        DefaultValue = CheckedKeysStrategy.Parents,
        PropertyChangedCallback = nameof(OnCheckedKeysStrategyChanged))]
    private CheckedKeysStrategy checkedKeysStrategy;

    /// <summary>
    /// Whether parent and child check boxes are linked.
    /// </summary>
    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnIsCheckLinkedChanged))]
    private bool isCheckLinked;

    /// <summary>
    /// Whether the filter box is shown; <see langword="null"/> shows it when the tree has more than 20 nodes.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnChromeChanged))]
    private bool? isFilterVisible;

    /// <summary>
    /// The text in the filter box; when not empty, the columns are replaced by the flat list of matches.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnFilterTextChanged))]
    private string filterText;

    /// <summary>
    /// The maximum number of keys in <see cref="SelectedKeys"/>; <see langword="null"/> for no limit.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnChromeChanged))]
    private int? maxSelectionCount;

    /// <summary>
    /// What makes a row active: a click, or also resting the mouse on it.
    /// </summary>
    [DependencyProperty(DefaultValue = CascaderExpandTrigger.Click, PropertyChangedCallback = nameof(OnExpandTriggerChanged))]
    private CascaderExpandTrigger expandTrigger;

    /// <summary>
    /// The width of each column.
    /// </summary>
    [DependencyProperty(DefaultValue = 180.0)]
    private double columnWidth;

    /// <summary>
    /// The separator between the node texts of a path.
    /// </summary>
    [DependencyProperty(DefaultValue = " / ", PropertyChangedCallback = nameof(OnPathSeparatorChanged))]
    private string pathSeparator;

    /// <summary>
    /// The template for the text part of a row; its data context is the <see cref="CheckTreeNode"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? itemTemplate;

    private ObservableCollection<string>? subscribedSelectedKeys;
    private bool isWritingSelectedKeys;
    private string? filterMatchesText;
    private DispatcherTimer? filterTimer;
    private DispatcherTimer? hoverTimer;
    private CheckTreeNode? pendingHoverNode;
    private CancellationTokenSource? loadCancellation;
    private CheckTreeNode? loadingNode;
    private CheckTreeNode? focusAfterLoad;
    private bool isScrollPending;

    /// <summary>
    /// Occurs when the view changes <see cref="SelectedKeys"/>: once per user action, or when loaded or added children
    /// change the keys.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Occurs after the check states, the keys or the tree changed and the chrome was refreshed.
    /// </summary>
    public event EventHandler? StateRefreshed;

    /// <summary>
    /// Initializes a new instance of the <see cref="CascaderColumnView"/> class.
    /// </summary>
    public CascaderColumnView()
    {
        InitializeComponent();

        // The generated callbacks do not run for the default value, so the engine gets the default strategy here.
        state.Strategy = CheckedKeysStrategy.Parents;
        state.StructureChanged = OnStructureChanged;

        filterList = CreateList(-1, filterResults);
        filterList.SetBinding(
            MinWidthProperty,
            new Binding
            {
                Path = new PropertyPath(ColumnWidthProperty),
                Source = this,
                Converter = DoubleWidthConverter.Instance,
            });
        filterList.BorderThickness = new Thickness(0);
        PartFilterResultsHost.Child = filterList;

        PreviewKeyDown += OnPreviewKeyDown;
        Unloaded += OnUnloaded;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
        ApplyTexts();

        SetCurrentValue(SelectedKeysProperty, new ObservableCollectionEx<string>());
        SetCurrentValue(NodesProperty, new ObservableCollection<CheckTreeNode>());
        RefreshChrome();
    }

    /// <summary>
    /// Gets or sets the callback that loads the children of a node with <see cref="CheckTreeNode.HasUnloadedChildren"/>
    /// when the node becomes active.
    /// </summary>
    public Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>? LoadChildrenAsync
    {
        get => (Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>?)GetValue(LoadChildrenAsyncProperty);
        set => SetValue(LoadChildrenAsyncProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="SelectedKeys"/> holds <see cref="MaxSelectionCount"/> keys (or more).
    /// </summary>
    public bool IsAtSelectionLimit
    {
        get => (bool)GetValue(IsAtSelectionLimitProperty);
        private set => SetValue(IsAtSelectionLimitPropertyKey, BooleanBoxes.Box(value));
    }

    /// <summary>
    /// Gets the check-state engine.
    /// </summary>
    internal CheckTreeState State => state;

    /// <summary>
    /// Gets a value indicating whether the view is writing its own change into <c>SelectedKeys</c>; the drop-down that
    /// shares the collection skips the notifications of that write and updates once afterwards.
    /// </summary>
    internal bool IsWritingSelectedKeys => isWritingSelectedKeys;

    /// <summary>
    /// Gets the nodes shown in each column (the roots first; a "please wait" row while a column loads).
    /// </summary>
    internal IReadOnlyList<IReadOnlyList<CheckTreeNode>> Columns => columnItems;

    /// <summary>
    /// Gets the list elements of the columns.
    /// </summary>
    internal IReadOnlyList<CascaderColumnList> ColumnLists => columnLists;

    /// <summary>
    /// Gets the list element of the filter results.
    /// </summary>
    internal CascaderColumnList FilterList => filterList;

    /// <summary>
    /// Gets the active row of each column, from the first column on.
    /// </summary>
    internal IReadOnlyList<CheckTreeNode> ActivePath => activePath;

    /// <summary>
    /// Gets the filter matches (from every level, in tree order).
    /// </summary>
    internal IReadOnlyList<CheckTreeNode> FilterResults => filterResults;

    /// <summary>
    /// Gets a value indicating whether the flat list of filter results replaces the columns.
    /// </summary>
    internal bool IsShowingFilterResults => filterMatchesText is not null;

    /// <summary>
    /// Gets a value indicating whether the filter box is shown.
    /// </summary>
    internal bool IsFilterShown
        => IsFilterVisible ?? state.NodeCount > AutoFilterNodeCount;

    /// <summary>
    /// Gets the row the keyboard acts on.
    /// </summary>
    internal CheckTreeNode? FocusedNode { get; private set; }

    /// <summary>
    /// Gets the column of <see cref="FocusedNode"/> (-1 for the filter results).
    /// </summary>
    internal int FocusedColumn { get; private set; }

    /// <summary>
    /// Gets a value indicating whether a hover activation waits for its delay.
    /// </summary>
    internal bool IsHoverPending => pendingHoverNode is not null;

    /// <summary>
    /// Gets a value indicating whether a row of the view has the keyboard focus.
    /// </summary>
    internal bool HasRowFocus
        => Keyboard.FocusedElement is CascaderColumnItem item && IsAncestorOf(item);

    /// <summary>
    /// Returns whether a node has children, loaded or not.
    /// </summary>
    internal static bool HasChildren(CheckTreeNode node)
        => node.Children.Count > 0 || node.HasUnloadedChildren;

    /// <summary>
    /// Returns the node with a key, or <see langword="null"/>.
    /// </summary>
    internal CheckTreeNode? FindNode(string key)
        => state.Find(key);

    /// <summary>
    /// Returns whether a node is a "please wait" row.
    /// </summary>
    internal bool IsPlaceholder(CheckTreeNode node)
        => placeholderSet.Contains(node);

    /// <summary>
    /// Returns whether a node is active (its children are shown in the next column).
    /// </summary>
    internal bool IsActive(CheckTreeNode node)
        => activePath.Contains(node, ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Returns the full path of a node: the texts of its ancestors and its own, joined by <see cref="PathSeparator"/>.
    /// </summary>
    internal string GetFullPath(CheckTreeNode node)
        => GetFullPath(node, PathSeparator);

    /// <summary>
    /// Returns the full path of a node with a given separator.
    /// </summary>
    internal string GetFullPath(
        CheckTreeNode node,
        string? separator)
    {
        var parts = new List<string> { node.Text };
        var parent = state.GetParent(node);
        while (parent is not null)
        {
            parts.Add(parent.Text);
            parent = state.GetParent(parent);
        }

        parts.Reverse();
        return string.Join(separator ?? string.Empty, parts);
    }

    /// <summary>
    /// Toggles a node as a click on its check box does. Raises <see cref="SelectedKeysChanged"/> once.
    /// </summary>
    internal bool ToggleNode(CheckTreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        return RunUserAction(() => state.Toggle(node));
    }

    /// <summary>
    /// Ticks every enabled node in one change.
    /// </summary>
    internal void SelectAll()
        => RunUserAction(() => state.SetAll(value: true));

    /// <summary>
    /// Unticks every enabled node in one change.
    /// </summary>
    internal void ClearSelection()
        => RunUserAction(() => state.SetAll(value: false));

    /// <summary>
    /// Removes a key as the user would: a node in the tree is unticked (with its children when linked); a key that is
    /// not in the tree is dropped. Raises <see cref="SelectedKeysChanged"/> once.
    /// </summary>
    internal bool RemoveKey(string key)
    {
        if (state.Find(key) is { } node)
        {
            return RunUserAction(() => state.SetChecked(node, value: false));
        }

        if (SelectedKeys is null || !SelectedKeys.Contains(key, StringComparer.Ordinal))
        {
            return false;
        }

        var newKeys = SelectedKeys.Where(x => !string.Equals(x, key, StringComparison.Ordinal)).ToList();
        state.ApplyKeys(newKeys);
        Commit(newKeys);
        return true;
    }

    /// <summary>
    /// Makes a row active: the deeper columns are dropped and its children are shown in the next column (loaded first
    /// when they are not loaded yet; a pending load of another row is cancelled). For a leaf, only the deeper columns
    /// are dropped.
    /// </summary>
    internal void Activate(CheckTreeNode node)
        => _ = ActivateAsync(node);

    /// <summary>
    /// Makes a row active (see <see cref="Activate"/>).
    /// </summary>
    /// <returns>A task that completes when the children are loaded (or the load failed or was cancelled).</returns>
    internal Task ActivateAsync(CheckTreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (!state.Contains(node) || IsActive(node))
        {
            return Task.CompletedTask;
        }

        CancelPendingHover();
        var path = GetAncestors(node);
        var canActivate = CanActivate(node);
        if (canActivate)
        {
            path.Add(node);
        }

        if (loadingNode is not null && !ReferenceEquals(loadingNode, node))
        {
            CancelLoad();
        }

        activePath.Clear();
        activePath.AddRange(path);
        RebuildColumns();

        return canActivate && node.Children.Count == 0
            ? LoadChildrenOfAsync(node)
            : Task.CompletedTask;
    }

    /// <summary>
    /// Makes a row inactive: its column becomes the last one.
    /// </summary>
    internal void Deactivate(CheckTreeNode node)
    {
        var index = activePath.FindIndex(x => ReferenceEquals(x, node));
        if (index < 0)
        {
            return;
        }

        if (loadingNode is not null && activePath.Skip(index).Contains(loadingNode, ReferenceEqualityComparer.Instance))
        {
            CancelLoad();
        }

        activePath.RemoveRange(index, activePath.Count - index);
        RebuildColumns();
    }

    /// <summary>
    /// Sets the row the keyboard acts on and moves the keyboard focus to it when it is shown.
    /// </summary>
    internal void FocusNode(CheckTreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var column = IsShowingFilterResults && filterResults.Contains(node)
            ? -1
            : state.GetDepth(node);
        SetFocus(column, node);
    }

    /// <summary>
    /// Moves the keyboard focus into the view: the filter box when shown, otherwise the deepest active row (or the
    /// first row of the first column).
    /// </summary>
    internal void FocusFirstElement()
    {
        if (PartFilter.IsVisible)
        {
            PartFilter.Focus();
            return;
        }

        FocusFirstRow();
    }

    /// <summary>
    /// Handles a key on the focused row: <c>↑</c> / <c>↓</c> move within the column, <c>→</c> makes the row active and
    /// moves into the next column, <c>←</c> goes back to the previous column, <c>Space</c> toggles, <c>Enter</c> acts
    /// as <c>→</c> on a row with children and toggles a leaf.
    /// </summary>
    /// <returns><see langword="true"/> when the key was used.</returns>
    internal bool HandleKey(Key key)
    {
        if (FocusedNode is not { } node || !state.Contains(node) || GetItems(FocusedColumn) is not { } items)
        {
            return false;
        }

        if (FlowDirection == FlowDirection.RightToLeft)
        {
            key = key switch
            {
                Key.Left => Key.Right,
                Key.Right => Key.Left,
                _ => key,
            };
        }

        switch (key)
        {
            case Key.Up:
            case Key.Down:
                MoveWithinColumn(items, node, key == Key.Down ? 1 : -1);
                return true;
            case Key.Space:
                ToggleNode(node);
                return true;
            case Key.Enter:
                if (FocusedColumn >= 0 && CanActivate(node))
                {
                    MoveIntoNextColumn(node);
                }
                else
                {
                    ToggleNode(node);
                }

                return true;
            case Key.Right:
                if (FocusedColumn >= 0 && CanActivate(node))
                {
                    MoveIntoNextColumn(node);
                    return true;
                }

                return false;
            case Key.Left:
                if (FocusedColumn > 0 && FocusedColumn - 1 < activePath.Count)
                {
                    SetFocus(FocusedColumn - 1, activePath[FocusedColumn - 1]);
                    return true;
                }

                return false;
            default:
                return false;
        }
    }

    /// <summary>
    /// Applies a pending (debounced) filter now.
    /// </summary>
    internal void FlushPendingFilter()
    {
        if (filterTimer is not { IsEnabled: true })
        {
            return;
        }

        filterTimer.Stop();
        ApplyFilter();
    }

    /// <summary>
    /// Reports that the mouse rests on a row: with <see cref="CascaderExpandTrigger.Hover"/>, a row with children
    /// becomes active after a short delay (so a diagonal mouse move to the next column does not activate the rows it
    /// crosses).
    /// </summary>
    internal void OnRowHovered(CheckTreeNode node)
    {
        if (ExpandTrigger != CascaderExpandTrigger.Hover || IsShowingFilterResults || IsPlaceholder(node))
        {
            return;
        }

        if (!CanActivate(node) || IsActive(node))
        {
            CancelPendingHover();
            return;
        }

        pendingHoverNode = node;
        if (hoverTimer is null)
        {
            hoverTimer = new DispatcherTimer(DispatcherPriority.Input, Dispatcher)
            {
                Interval = HoverDelay,
            };
            hoverTimer.Tick += OnHoverTimerTick;
        }

        hoverTimer.Stop();
        hoverTimer.Start();
    }

    /// <summary>
    /// Reports that the mouse left a row: its pending hover activation is dropped.
    /// </summary>
    internal void OnRowHoverLeft(CheckTreeNode node)
    {
        if (ReferenceEquals(pendingHoverNode, node))
        {
            CancelPendingHover();
        }
    }

    /// <summary>
    /// Runs the pending hover activation now.
    /// </summary>
    internal void RunPendingHoverActivation()
    {
        hoverTimer?.Stop();
        var node = pendingHoverNode;
        pendingHoverNode = null;
        if (node is not null && state.Contains(node))
        {
            Activate(node);
        }
    }

    /// <summary>
    /// Drops a pending hover activation.
    /// </summary>
    internal void CancelPendingHover()
    {
        hoverTimer?.Stop();
        pendingHoverNode = null;
    }

    /// <summary>
    /// Handles a click on a row: a filter result, a click on the check box or a click on a leaf ticks the node; a click
    /// on the rest of a row with children makes it active.
    /// </summary>
    internal void OnRowClicked(
        CascaderColumnList list,
        CheckTreeNode node,
        bool isCheckArea)
    {
        if (list.IsFilterResults || isCheckArea || !CanActivate(node))
        {
            ToggleNode(node);
            return;
        }

        Activate(node);
    }

    /// <summary>
    /// Records the row that got the keyboard focus.
    /// </summary>
    internal void OnRowFocused(
        CascaderColumnList list,
        CheckTreeNode node)
    {
        FocusedColumn = list.ColumnIndex;
        FocusedNode = node;
    }

    /// <summary>
    /// Prepares a row container for a node.
    /// </summary>
    internal void PrepareContainer(
        CascaderColumnList list,
        CascaderColumnItem container,
        CheckTreeNode node)
    {
        var isPlaceholder = IsPlaceholder(node);
        container.Node = node;
        container.IsPlaceholder = isPlaceholder;
        container.ContentTemplate = (DataTemplate)Resources[isPlaceholder ? "CascaderBusyRowTemplate" : "CascaderRowTemplate"];
        container.IsFilterResult = list.IsFilterResults;
        container.FilterContent = list.IsFilterResults && !isPlaceholder
            ? CreateHighlightedPath(node)
            : null;
        container.IsActive = !list.IsFilterResults && IsActive(node);
        container.SetBinding(
            AutomationProperties.NameProperty,
            new Binding(nameof(CheckTreeNode.Text)) { Source = node, Mode = BindingMode.OneWay });
        realizedContainers.Add(container);
    }

    /// <summary>
    /// Forgets a row container that is recycled.
    /// </summary>
    internal void ClearContainer(CascaderColumnItem container)
        => realizedContainers.Remove(container);

    /// <summary>
    /// Starts loading the children of a node with unloaded children.
    /// </summary>
    /// <returns>A task that completes when the children are in the tree (or the load failed or was cancelled).</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A failing load leaves the node unloaded (it loads again on the next activation) instead of crashing the UI.")]
    internal async Task LoadChildrenOfAsync(CheckTreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (LoadChildrenAsync is not { } loader ||
            !node.HasUnloadedChildren ||
            node.Children.Count > 0 ||
            !state.Contains(node) ||
            ReferenceEquals(loadingNode, node))
        {
            return;
        }

        CancelLoad();
        var cancellation = new CancellationTokenSource();
        loadCancellation = cancellation;
        loadingNode = node;
        try
        {
            var children = await loader(node, cancellation.Token).ConfigureAwait(true) ?? [];
            if (cancellation.IsCancellationRequested || !state.Contains(node))
            {
                return;
            }

            AddLoadedChildren(node, children);
        }
        catch (OperationCanceledException)
        {
            // Cancelled: the node stays unloaded.
        }
        catch (Exception)
        {
            if (!cancellation.IsCancellationRequested)
            {
                DropFromActivePath(node);
            }
        }
        finally
        {
            if (ReferenceEquals(loadCancellation, cancellation))
            {
                loadCancellation = null;
                loadingNode = null;
            }

            if (ReferenceEquals(focusAfterLoad, node))
            {
                focusAfterLoad = null;
            }

            cancellation.Dispose();
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new FrameworkElementAutomationPeer(this);

    private void AddLoadedChildren(
        CheckTreeNode node,
        IReadOnlyList<CheckTreeNode> children)
    {
        using (state.DeferStructureChanges())
        {
            node.HasUnloadedChildren = false;
            foreach (var child in children)
            {
                node.Children.Add(child);
            }
        }

        if (children.Count == 0)
        {
            DropFromActivePath(node);
        }

        if (!ReferenceEquals(focusAfterLoad, node))
        {
            return;
        }

        focusAfterLoad = null;
        var column = activePath.FindIndex(x => ReferenceEquals(x, node)) + 1;
        if (column > 0 && column < columnItems.Count && columnItems[column].Count > 0)
        {
            SetFocus(column, columnItems[column][0]);
        }
    }

    private static void OnNodesChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CascaderColumnView)d;
        control.CancelLoad();
        control.CancelPendingHover();
        control.state.SetRoots(e.NewValue as ObservableCollection<CheckTreeNode>);
        control.activePath.Clear();
        control.FocusedNode = null;
        control.FocusedColumn = 0;
        control.RebuildColumns();
        if (control.IsShowingFilterResults)
        {
            control.ApplyFilter();
        }

        control.RefreshChrome();
    }

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CascaderColumnView)d;
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
            control.ApplySelectedKeys();
        }
    }

    private static void OnCheckedKeysStrategyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CascaderColumnView)d).state.Strategy = (CheckedKeysStrategy)e.NewValue;

    private static void OnIsCheckLinkedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CascaderColumnView)d;
        control.state.IsCheckLinked = (bool)e.NewValue;
        control.RefreshChrome();
    }

    private static void OnChromeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CascaderColumnView)d).RefreshChrome();

    private static void OnExpandTriggerChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CascaderColumnView)d).CancelPendingHover();

    private static void OnPathSeparatorChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CascaderColumnView)d;
        if (control.IsShowingFilterResults)
        {
            control.RefreshFilterContents();
        }
    }

    private static void OnLoadChildrenAsyncChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CascaderColumnView)d).RebuildColumns();

    private static void OnFilterTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CascaderColumnView)d;
        if (control.state.NodeCount <= KeySelectionHelper.FilterDebounceThreshold)
        {
            control.filterTimer?.Stop();
            control.ApplyFilter();
            return;
        }

        if (control.filterTimer is null)
        {
            control.filterTimer = new DispatcherTimer(DispatcherPriority.Input, control.Dispatcher)
            {
                Interval = KeySelectionHelper.FilterDebounceDelay,
            };
            control.filterTimer.Tick += control.OnFilterTimerTick;
        }

        control.filterTimer.Stop();
        control.filterTimer.Start();
    }

    private static void SyncShown(
        ObservableCollectionEx<CheckTreeNode> shown,
        IList<CheckTreeNode> desired)
    {
        if (shown.Count == desired.Count)
        {
            var isSame = true;
            for (var i = 0; i < desired.Count; i++)
            {
                if (!ReferenceEquals(shown[i], desired[i]))
                {
                    isSame = false;
                    break;
                }
            }

            if (isSame)
            {
                return;
            }
        }

        shown.SuppressOnChangedNotification = true;
        try
        {
            shown.Clear();
            foreach (var node in desired)
            {
                shown.Add(node);
            }
        }
        finally
        {
            shown.SuppressOnChangedNotification = false;
        }
    }

    private static int IndexOf(
        IReadOnlyList<CheckTreeNode> items,
        CheckTreeNode node)
    {
        for (var i = 0; i < items.Count; i++)
        {
            if (ReferenceEquals(items[i], node))
            {
                return i;
            }
        }

        return -1;
    }

    private bool CanActivate(CheckTreeNode node)
        => node.Children.Count > 0 ||
           (node.HasUnloadedChildren && LoadChildrenAsync is not null);

    private List<CheckTreeNode> GetAncestors(CheckTreeNode node)
    {
        var ancestors = new List<CheckTreeNode>();
        var parent = state.GetParent(node);
        while (parent is not null)
        {
            ancestors.Add(parent);
            parent = state.GetParent(parent);
        }

        ancestors.Reverse();
        return ancestors;
    }

    private IReadOnlyList<CheckTreeNode>? GetItems(int column)
        => column < 0
            ? filterResults
            : column < columnItems.Count
                ? columnItems[column]
                : null;

    private CascaderColumnList? GetList(int column)
        => column < 0
            ? filterList
            : column < columnLists.Count
                ? columnLists[column]
                : null;

    private void SetFocus(
        int column,
        CheckTreeNode node)
    {
        FocusedColumn = column;
        FocusedNode = node;
        if (GetList(column) is not { IsVisible: true } list ||
            GetItems(column) is not { } items)
        {
            return;
        }

        var index = IndexOf(items, node);
        if (index >= 0 && list.RealizeContainer(index) is { } container)
        {
            container.Focus();
        }
    }

    private void FocusFirstRow()
    {
        var column = IsShowingFilterResults ? -1 : 0;
        if (GetItems(column) is not { Count: > 0 } items)
        {
            return;
        }

        // With an active path, the deepest active row gets the focus, so the newest columns stay in view.
        if (column == 0 && activePath.Count > 0)
        {
            SetFocus(activePath.Count - 1, activePath[^1]);
            return;
        }

        SetFocus(column, items[0]);
    }

    private void MoveWithinColumn(
        IReadOnlyList<CheckTreeNode> items,
        CheckTreeNode node,
        int delta)
    {
        var index = IndexOf(items, node);
        var next = index + delta;
        while (next >= 0 && next < items.Count && IsPlaceholder(items[next]))
        {
            next += delta;
        }

        if (next < 0 || next >= items.Count)
        {
            return;
        }

        SetFocus(FocusedColumn, items[next]);
    }

    private void MoveIntoNextColumn(CheckTreeNode node)
    {
        var column = FocusedColumn;
        var load = ActivateAsync(node);
        var next = column + 1;
        if (next < columnItems.Count &&
            columnItems[next].FirstOrDefault(x => !IsPlaceholder(x)) is { } first)
        {
            SetFocus(next, first);
            return;
        }

        if (!load.IsCompleted)
        {
            focusAfterLoad = node;
        }
    }

    private void DropFromActivePath(CheckTreeNode node)
    {
        var index = activePath.FindIndex(x => ReferenceEquals(x, node));
        if (index >= 0)
        {
            activePath.RemoveRange(index, activePath.Count - index);
        }

        RebuildColumns();
    }

    /// <summary>
    /// Recomputes the columns from the roots and the active path.
    /// </summary>
    private void RebuildColumns()
    {
        if (PartColumnsPanel is null)
        {
            return;
        }

        var desired = new List<IList<CheckTreeNode>> { (IList<CheckTreeNode>?)Nodes ?? [] };
        foreach (var node in activePath)
        {
            desired.Add(ComputeChildren(node));
        }

        while (columnItems.Count > desired.Count)
        {
            var last = columnItems.Count - 1;
            PartColumnsPanel.Children.Remove(columnLists[last]);
            columnLists[last].ItemsSource = null;
            columnLists.RemoveAt(last);
            columnItems.RemoveAt(last);
        }

        for (var i = 0; i < desired.Count; i++)
        {
            if (i >= columnItems.Count)
            {
                var items = new ObservableCollectionEx<CheckTreeNode>();
                SyncShown(items, desired[i]);
                columnItems.Add(items);
                var list = CreateList(i, items);
                columnLists.Add(list);
                PartColumnsPanel.Children.Add(list);
            }
            else
            {
                SyncShown(columnItems[i], desired[i]);
            }
        }

        RefreshActiveFlags();
        ScrollNewestColumnIntoView();
    }

    private IList<CheckTreeNode> ComputeChildren(CheckTreeNode node)
        => node.Children.Count == 0 && node.HasUnloadedChildren && LoadChildrenAsync is not null
            ? [GetPlaceholder(node)]
            : node.Children;

    private CheckTreeNode GetPlaceholder(CheckTreeNode node)
    {
        if (!placeholders.TryGetValue(node, out var placeholder))
        {
            placeholder = new CheckTreeNode(string.Empty, Atc.Wpf.Resources.Miscellaneous.PleaseWait3Dots)
            {
                IsEnabled = false,
            };
            placeholders[node] = placeholder;
            placeholderSet.Add(placeholder);
        }

        return placeholder;
    }

    private CascaderColumnList CreateList(
        int column,
        ObservableCollectionEx<CheckTreeNode> items)
    {
        var list = new CascaderColumnList(this, column)
        {
            Template = (ControlTemplate)Resources["CascaderColumnListTemplate"],
            ItemContainerStyle = (Style)Resources["CascaderColumnItemStyle"],
            ItemsSource = items,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0, 0, 1, 0),
        };
        list.SetResourceReference(BorderBrushProperty, "AtcApps.Brushes.ComboBox.PopupBorder");
        if (column >= 0)
        {
            list.SetBinding(
                WidthProperty,
                new Binding
                {
                    Path = new PropertyPath(ColumnWidthProperty),
                    Source = this,
                });
        }

        return list;
    }

    private void RefreshActiveFlags()
    {
        foreach (var container in realizedContainers)
        {
            container.IsActive = !container.IsFilterResult &&
                                 container.Node is { } node &&
                                 IsActive(node);
        }
    }

    private void ScrollNewestColumnIntoView()
    {
        if (isScrollPending || !PartColumnsScroller.IsVisible)
        {
            return;
        }

        isScrollPending = true;
        _ = Dispatcher.BeginInvoke(
            DispatcherPriority.Loaded,
            () =>
            {
                isScrollPending = false;

                // The scroll is applied in the next layout pass; make sure there is one.
                PartColumnsScroller.ScrollToRightEnd();
                PartColumnsScroller.InvalidateArrange();
            });
    }

    private void OnStructureChanged(CheckTreeNode? parent)
    {
        // Drop the part of the active path that left the tree or has no children any more.
        for (var i = 0; i < activePath.Count; i++)
        {
            if (!state.Contains(activePath[i]) || !CanActivate(activePath[i]))
            {
                activePath.RemoveRange(i, activePath.Count - i);
                break;
            }
        }

        RebuildColumns();
        if (IsShowingFilterResults)
        {
            ApplyFilter();
        }

        // Loaded or added children may change the keys (a ticked parent passes its tick on).
        var newKeys = state.GetKeys();
        if (!KeySelectionHelper.SequenceEquals(GetCurrentKeys(), newKeys))
        {
            state.CommitKeys();
            Commit(newKeys);
        }
        else
        {
            RefreshChrome();
        }
    }

    private void OnFilterTimerTick(
        object? sender,
        EventArgs e)
        => FlushPendingFilter();

    private void OnHoverTimerTick(
        object? sender,
        EventArgs e)
        => RunPendingHoverActivation();

    private void ApplyFilter()
    {
        var text = string.IsNullOrWhiteSpace(FilterText)
            ? null
            : FilterText.Trim();

        if (text is null)
        {
            filterMatchesText = null;
            SyncShown(filterResults, []);
            RefreshChrome();
            return;
        }

        var result = new TreeFilterResult(text);
        foreach (var root in (IEnumerable<CheckTreeNode>?)Nodes ?? [])
        {
            result.Visit(root, ancestorMatched: false);
        }

        filterMatchesText = text;
        SyncShown(filterResults, result.Matches);
        RefreshFilterContents();
        RefreshChrome();
    }

    private void RefreshFilterContents()
    {
        foreach (var container in realizedContainers)
        {
            if (container.IsFilterResult && !container.IsPlaceholder && container.Node is { } node)
            {
                container.FilterContent = CreateHighlightedPath(node);
            }
        }
    }

    /// <summary>
    /// Returns the full path of a filter result with the match in the node's own text highlighted.
    /// </summary>
    private TextBlock CreateHighlightedPath(CheckTreeNode node)
    {
        var textBlock = new TextBlock
        {
            TextTrimming = TextTrimming.CharacterEllipsis,
            VerticalAlignment = VerticalAlignment.Center,
        };

        var fullPath = GetFullPath(node);
        var prefixLength = fullPath.Length - node.Text.Length;
        if (prefixLength > 0)
        {
            var prefix = new Run(fullPath[..prefixLength]);
            prefix.SetResourceReference(TextElement.ForegroundProperty, "AtcApps.Brushes.Gray");
            textBlock.Inlines.Add(prefix);
        }

        var text = node.Text;
        var match = filterMatchesText is null
            ? -1
            : text.IndexOf(filterMatchesText, StringComparison.CurrentCultureIgnoreCase);
        if (match < 0)
        {
            textBlock.Inlines.Add(new Run(text));
            return textBlock;
        }

        if (match > 0)
        {
            textBlock.Inlines.Add(new Run(text[..match]));
        }

        var highlight = new Run(text.Substring(match, filterMatchesText!.Length))
        {
            FontWeight = FontWeights.Bold,
        };
        highlight.SetResourceReference(TextElement.ForegroundProperty, "AtcApps.Brushes.Accent");
        textBlock.Inlines.Add(highlight);

        var end = match + filterMatchesText.Length;
        if (end < text.Length)
        {
            textBlock.Inlines.Add(new Run(text[end..]));
        }

        return textBlock;
    }

    private void OnSelectedKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (!isWritingSelectedKeys)
        {
            ApplySelectedKeys();
        }
    }

    private void ApplySelectedKeys()
    {
        state.ApplyKeys((IEnumerable<string>?)SelectedKeys ?? []);
        RefreshChrome();
    }

    /// <summary>
    /// Runs a user action on the engine and turns its result into one <see cref="SelectedKeys"/> change (one event),
    /// or refuses it when it would go beyond <see cref="MaxSelectionCount"/>.
    /// </summary>
    private bool RunUserAction(Func<bool> action)
    {
        var oldKeys = GetCurrentKeys();
        if (!action())
        {
            return false;
        }

        var newKeys = state.GetKeys();
        if (MaxSelectionCount is { } max &&
            newKeys.Count > max &&
            newKeys.Count > oldKeys.Count)
        {
            state.ApplyKeys(oldKeys);
            return false;
        }

        state.CommitKeys();
        return Commit(newKeys);
    }

    private bool Commit(IReadOnlyList<string> newKeys)
    {
        var oldKeys = GetCurrentKeys();
        if (KeySelectionHelper.SequenceEquals(oldKeys, newKeys))
        {
            RefreshChrome();
            return false;
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

        // SelectedKeysChanged follows: no separate StateRefreshed, so the drop-down updates its summary once.
        RefreshChrome(raiseStateRefreshed: false);
        SelectedKeysChanged?.Invoke(
            this,
            new ValueChangedEventArgs<IReadOnlyList<string>>(
                Name,
                oldKeys,
                SelectedKeys.ToList()));
        return true;
    }

    private IReadOnlyList<string> GetCurrentKeys()
        => SelectedKeys is null
            ? []
            : SelectedKeys.ToList();

    private void RefreshChrome(bool raiseStateRefreshed = true)
    {
        if (PartFilter is null)
        {
            return;
        }

        var keyCount = SelectedKeys?.Count ?? 0;
        IsAtSelectionLimit = MaxSelectionCount is { } max && keyCount >= max;

        PartFilter.Visibility = IsFilterShown ? Visibility.Visible : Visibility.Collapsed;
        var isFiltering = IsShowingFilterResults;
        PartColumnsScroller.Visibility = isFiltering ? Visibility.Collapsed : Visibility.Visible;
        PartFilterResultsHost.Visibility = isFiltering ? Visibility.Visible : Visibility.Collapsed;
        PartNoMatches.Visibility = isFiltering && filterResults.Count == 0 && state.NodeCount > 0
            ? Visibility.Visible
            : Visibility.Collapsed;

        if (raiseStateRefreshed)
        {
            StateRefreshed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void CancelLoad()
    {
        if (loadCancellation is null)
        {
            return;
        }

        loadCancellation.Cancel();
        loadCancellation = null;
        loadingNode = null;
        focusAfterLoad = null;
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        ApplyTexts();
        foreach (var placeholder in placeholders.Values)
        {
            placeholder.Text = Atc.Wpf.Resources.Miscellaneous.PleaseWait3Dots;
        }
    }

    private void ApplyTexts()
    {
        PartNoMatches.Text = Atc.Wpf.Resources.Miscellaneous.NoMatches;
        TextBoxHelper.SetWatermark(PartFilter, Atc.Wpf.Resources.Word.Filter);
        AutomationProperties.SetName(PartFilter, Atc.Wpf.Resources.Word.Filter);
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        CancelPendingHover();
        filterTimer?.Stop();
    }

    private void OnPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (Keyboard.Modifiers != ModifierKeys.None)
        {
            return;
        }

        if (e.OriginalSource is TextBox)
        {
            if (e.Key == Key.Down)
            {
                FlushPendingFilter();
                FocusFirstRow();
                e.Handled = HasRowFocus;
            }

            return;
        }

        if (e.OriginalSource is CascaderColumnItem && HandleKey(e.Key))
        {
            e.Handled = true;
        }
    }

    /// <summary>
    /// Doubles a column width (the list of filter results is two columns wide).
    /// </summary>
    private sealed class DoubleWidthConverter : IValueConverter
    {
        public static readonly DoubleWidthConverter Instance = new();

        public object Convert(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture)
            => value is double width ? width * 2 : DependencyProperty.UnsetValue;

        public object ConvertBack(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture)
            => throw new NotSupportedException();
    }
}