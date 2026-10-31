// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// An inline tree with tri-state check boxes: ticking a parent ticks everything under it, and a parent shows a partial
/// mark when only some of its children are ticked. Optional filter box, Select all / Clear row, selection limit and lazy
/// loading of children.
/// </summary>
/// <remarks>
/// <para>
/// The check state of every <see cref="CheckTreeNode"/> is kept by the control from <see cref="SelectedKeys"/>, so a view
/// model only binds keys. Which keys <see cref="SelectedKeys"/> holds depends on <see cref="CheckedKeysStrategy"/>.
/// </para>
/// <para>
/// Ticks made in the control update <see cref="SelectedKeys"/> in place and raise <see cref="SelectedKeysChanged"/> once
/// per action (ticking a parent with 500 children is one event). Setting <see cref="SelectedKeys"/> from code recomputes
/// every check state in one pass and raises no event. Keys of nodes that are not in the tree (for example children that
/// are not loaded yet) are kept.
/// </para>
/// </remarks>
public partial class CheckTreeView
{
    /// <summary>
    /// Identifies the <see cref="LoadChildrenAsync"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LoadChildrenAsyncProperty = DependencyProperty.Register(
        nameof(LoadChildrenAsync),
        typeof(Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>>),
        typeof(CheckTreeView),
        new PropertyMetadata(defaultValue: null, OnLoadChildrenAsyncChanged));

    private static readonly DependencyPropertyKey IsAtSelectionLimitPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(IsAtSelectionLimit),
        typeof(bool),
        typeof(CheckTreeView),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <see cref="IsAtSelectionLimit"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsAtSelectionLimitProperty = IsAtSelectionLimitPropertyKey.DependencyProperty;

    private const int AutoFilterNodeCount = 20;

    private readonly CheckTreeState state = new();

    /// <summary>
    /// The root nodes.
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
    /// Which ticked nodes <see cref="SelectedKeys"/> holds: only leaves (<see cref="Selectors.CheckedKeysStrategy.Leaves"/>,
    /// the default), the top-most fully ticked nodes (<see cref="Selectors.CheckedKeysStrategy.Parents"/>) or every ticked
    /// node (<see cref="Selectors.CheckedKeysStrategy.All"/>). Ignored when <see cref="IsCheckLinked"/> is
    /// <see langword="false"/>.
    /// </summary>
    [DependencyProperty(
        DefaultValue = CheckedKeysStrategy.Leaves,
        PropertyChangedCallback = nameof(OnCheckedKeysStrategyChanged))]
    private CheckedKeysStrategy checkedKeysStrategy;

    /// <summary>
    /// Whether parent and child check boxes are linked (ticking a parent ticks its children, a parent shows a partial
    /// state); <see langword="false"/> makes every node independent.
    /// </summary>
    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnIsCheckLinkedChanged))]
    private bool isCheckLinked;

    /// <summary>
    /// Whether the filter box is shown; <see langword="null"/> (the default) shows it when the tree has more than 20 nodes.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnChromeChanged))]
    private bool? isFilterVisible;

    /// <summary>
    /// The text in the filter box; only nodes whose text contains it (with everything under them, and their ancestors)
    /// are shown.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnFilterTextChanged))]
    private string filterText;

    /// <summary>
    /// How filter matches are shown: in the tree with their ancestors expanded
    /// (<see cref="TreeFilterMode.KeepAncestors"/>, the default), or as a flat list with their path as a sub-line
    /// (<see cref="TreeFilterMode.FlatResults"/>).
    /// </summary>
    [DependencyProperty(DefaultValue = TreeFilterMode.KeepAncestors, PropertyChangedCallback = nameof(OnFilterModeChanged))]
    private TreeFilterMode filterMode;

    /// <summary>
    /// Whether the row with the tri-state "Select all" check box, the count and "Clear" is shown above the tree.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnChromeChanged))]
    private bool showSelectAllRow;

    /// <summary>
    /// The maximum number of keys in <see cref="SelectedKeys"/>; <see langword="null"/> for no limit. A tick that would
    /// go beyond it is refused; at the limit the unticked nodes are disabled.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnChromeChanged), ValidateValueCallback = nameof(ValidateMaxSelectionCount))]
    private int? maxSelectionCount;

    /// <summary>
    /// The number of levels expanded when <see cref="Nodes"/> is set (1: the roots are expanded).
    /// </summary>
    [DependencyProperty(DefaultValue = 1)]
    private int expandDepth;

    /// <summary>
    /// The template for the text part of a row; its data context is the <see cref="CheckTreeNode"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? itemTemplate;

    private ObservableCollection<CheckTreeNode>? subscribedNodes;
    private ObservableCollection<string>? subscribedSelectedKeys;
    private bool isWritingSelectedKeys;
    private bool isSyncingSelectAll;
    private bool? selectAllState = false;

    /// <summary>
    /// Occurs when the control changes <see cref="SelectedKeys"/>: once per tick (a parent with all its children is one
    /// tick), Select all, Clear, or loaded children that change the keys.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Occurs after the check states, the keys or the tree changed and the chrome (count, Select all) was refreshed.
    /// </summary>
    internal event EventHandler? StateRefreshed;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckTreeView"/> class.
    /// </summary>
    public CheckTreeView()
    {
        InitializeComponent();

        PartTree = new CheckTreeViewTree(this)
        {
            BorderThickness = new Thickness(0),
            ItemsSource = shownRoots,
        };
        PartTree.SetValue(AutomationProperties.AutomationIdProperty, "Tree");
        ScrollViewer.SetCanContentScroll(PartTree, true);
        ScrollViewer.SetHorizontalScrollBarVisibility(PartTree, ScrollBarVisibility.Disabled);
        PartTree.AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(OnItemExpanded));
        PartTreeHost.Child = PartTree;

        state.StructureChanged = OnStructureChanged;
        PreviewKeyDown += OnPreviewKeyDown;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
        ApplyTexts();

        SetCurrentValue(SelectedKeysProperty, new ObservableCollectionEx<string>());
        SetCurrentValue(NodesProperty, new ObservableCollection<CheckTreeNode>());
        RefreshChrome();
    }

    /// <summary>
    /// Gets or sets the callback that loads the children of a node with <see cref="CheckTreeNode.HasUnloadedChildren"/>
    /// on its first expand; a "please wait" row is shown meanwhile. Children of a ticked node inherit the tick.
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
    /// Gets the tree.
    /// </summary>
    internal TreeView PartTree { get; }

    /// <summary>
    /// Gets how many times <see cref="SelectedKeys"/> was applied to the check states (for change-storm tests).
    /// </summary>
    internal int KeysApplyCount { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the control is writing its own change into <c>SelectedKeys</c>; a wrapper that
    /// shares the collection skips the notifications of that write and updates once afterwards.
    /// </summary>
    internal bool IsWritingSelectedKeys => isWritingSelectedKeys;

    /// <summary>
    /// Gets the check-state engine.
    /// </summary>
    internal CheckTreeState State => state;

    /// <summary>
    /// Returns the node with a key, or <see langword="null"/> when it is not in the tree.
    /// </summary>
    /// <param name="key">The node key.</param>
    /// <returns>The node, or <see langword="null"/>.</returns>
    public CheckTreeNode? FindNode(string key)
        => state.Find(key);

    /// <summary>
    /// Toggles a node as a click does: a node whose enabled (and shown) children are all ticked is unticked with them,
    /// any other node is ticked with them. Raises <see cref="SelectedKeysChanged"/> once.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns><see langword="true"/> when <see cref="SelectedKeys"/> changed.</returns>
    public bool ToggleNode(CheckTreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        return RunUserAction(() => state.Toggle(node, GetShownPredicate()));
    }

    /// <summary>
    /// Ticks or unticks a node (with its enabled, shown children when linked). Raises <see cref="SelectedKeysChanged"/> once.
    /// </summary>
    /// <param name="key">The node key.</param>
    /// <param name="isChecked">Whether to tick or untick it.</param>
    /// <returns><see langword="true"/> when <see cref="SelectedKeys"/> changed.</returns>
    public bool SetNodeChecked(
        string key,
        bool isChecked)
    {
        if (state.Find(key) is not { } node)
        {
            return false;
        }

        return RunUserAction(() => state.SetChecked(node, isChecked, GetShownPredicate()));
    }

    /// <summary>
    /// Ticks every shown, enabled node in one change. Does nothing when the selection limit has no room for them.
    /// </summary>
    public void SelectAll()
        => RunUserAction(() => state.SetAll(value: true, GetShownPredicate()));

    /// <summary>
    /// Unticks every enabled node (shown or not) in one change. Disabled nodes keep their state.
    /// </summary>
    public void ClearSelection()
        => RunUserAction(() => state.SetAll(value: false));

    /// <summary>
    /// Expands every loaded node.
    /// </summary>
    public void ExpandAll()
    {
        foreach (var node in state.EnumerateNodes())
        {
            if (node.Children.Count > 0)
            {
                node.IsExpanded = true;
            }
        }
    }

    /// <summary>
    /// Expands a node and every loaded node under it (what <c>*</c> on a row does); nodes with unloaded children start
    /// loading.
    /// </summary>
    /// <param name="node">The node.</param>
    public void ExpandSubtree(CheckTreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var stack = new Stack<CheckTreeNode>();
        stack.Push(node);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current.Children.Count > 0 || current.HasUnloadedChildren)
            {
                current.IsExpanded = true;
            }

            foreach (var child in current.Children)
            {
                stack.Push(child);
            }
        }
    }

    /// <summary>
    /// Collapses every node.
    /// </summary>
    public void CollapseAll()
    {
        foreach (var node in state.EnumerateNodes())
        {
            node.IsExpanded = false;
        }
    }

    /// <summary>
    /// Removes a key from <see cref="SelectedKeys"/> as the user would: a node in the tree is unticked (with its
    /// children when linked); a key that is not in the tree is dropped. Raises <see cref="SelectedKeysChanged"/> once.
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

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new FrameworkElementAutomationPeer(this);

    private static void OnNodesChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CheckTreeView)d;
        control.subscribedNodes = e.NewValue as ObservableCollection<CheckTreeNode>;
        control.CancelLoads();
        control.state.SetRoots(control.subscribedNodes);
        control.ExpandToDepth();
        control.RebuildShown();
        control.RefreshChrome();
    }

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CheckTreeView)d;
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
        => ((CheckTreeView)d).state.Strategy = (CheckedKeysStrategy)e.NewValue;

    private static void OnIsCheckLinkedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CheckTreeView)d;
        control.state.IsCheckLinked = (bool)e.NewValue;
        control.RefreshChrome();
    }

    private static void OnChromeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CheckTreeView)d).RefreshChrome();

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
        KeysApplyCount++;
        state.ApplyKeys((IEnumerable<string>?)SelectedKeys ?? []);
        RefreshChrome();
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

        RefreshChrome();
    }

    private void ApplyTexts()
    {
        PartSelectAll.Content = Atc.Wpf.Resources.Miscellaneous.SelectAll;
        PartClear.Content = Atc.Wpf.Resources.Miscellaneous.Clear;
        PartNoMatches.Text = Atc.Wpf.Resources.Miscellaneous.NoMatches;
        TextBoxHelper.SetWatermark(PartFilter, Atc.Wpf.Resources.Word.Filter);
        AutomationProperties.SetName(PartFilter, Atc.Wpf.Resources.Word.Filter);
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

    private void OnStructureChanged(CheckTreeNode? parent)
    {
        if (filterMatchesText is not null)
        {
            ApplyFilter();
        }
        else if (parent is null)
        {
            SyncShown(shownRoots, (IList<CheckTreeNode>?)Nodes ?? []);
        }
        else if (shownChildren.TryGetValue(parent, out var shown))
        {
            SyncShown(shown, ComputeShownChildren(parent));
        }

        // Loaded or added children may change the keys (a ticked parent passes its tick on).
        var newKeys = state.GetKeys();
        if (!KeySelectionHelper.SetEquals(GetCurrentKeys(), newKeys))
        {
            state.CommitKeys();
            Commit(newKeys);
        }
        else
        {
            RefreshChrome();
        }
    }

    private bool Commit(IReadOnlyList<string> newKeys)
    {
        var oldKeys = GetCurrentKeys();
        if (KeySelectionHelper.SetEquals(oldKeys, newKeys))
        {
            RefreshChrome();
            return false;
        }

        bool isChanged;
        isWritingSelectedKeys = true;
        try
        {
            var target = SelectedKeys;
            if (target is null)
            {
                target = new ObservableCollectionEx<string>();
                SetCurrentValue(SelectedKeysProperty, target);
            }

            isChanged = KeySelectionHelper.ApplyInPlace(target, newKeys);
        }
        finally
        {
            isWritingSelectedKeys = false;
        }

        if (!isChanged)
        {
            RefreshChrome();
            return false;
        }

        // SelectedKeysChanged follows: no separate StateRefreshed, so a wrapper updates its summary once.
        if (PartTree is not null)
        {
            RefreshChromeParts();
        }

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

    private void RefreshChrome()
    {
        if (PartTree is null)
        {
            return;
        }

        RefreshChromeParts();
        StateRefreshed?.Invoke(this, EventArgs.Empty);
    }

    private void RefreshChromeParts()
    {
        var keyCount = SelectedKeys?.Count ?? 0;
        IsAtSelectionLimit = MaxSelectionCount is { } max && keyCount >= max;

        PartSelectAllRow.Visibility = ShowSelectAllRow ? Visibility.Visible : Visibility.Collapsed;
        var showFilter = IsFilterVisible ?? state.NodeCount > AutoFilterNodeCount;
        PartFilter.Visibility = showFilter ? Visibility.Visible : Visibility.Collapsed;

        PartNoMatches.Visibility = filterMatchesText is not null && shownRoots.Count == 0 && state.NodeCount > 0
            ? Visibility.Visible
            : Visibility.Collapsed;

        if (!ShowSelectAllRow)
        {
            return;
        }

        var (ticked, total) = state.CountLeaves();
        PartCount.Text = MaxSelectionCount is { } limit
            ? string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.CountOfMax2, keyCount, limit)
            : string.Format(CultureInfo.CurrentCulture, Atc.Wpf.Resources.Miscellaneous.CountOf2, ticked, total);

        selectAllState = state.GetAllState(GetShownPredicate());
        isSyncingSelectAll = true;
        try
        {
            PartSelectAll.SetCurrentValue(ToggleButton.IsCheckedProperty, selectAllState);
            PartSelectAll.SetCurrentValue(IsEnabledProperty, state.NodeCount > 0);
        }
        finally
        {
            isSyncingSelectAll = false;
        }
    }

    private void OnSelectAllIsCheckedChanged(
        object sender,
        RoutedEventArgs e)
    {
        if (isSyncingSelectAll)
        {
            return;
        }

        if (selectAllState == true)
        {
            RunUserAction(() => state.SetAll(value: false, GetShownPredicate()));
        }
        else
        {
            SelectAll();
        }

        // Push the real state back: the check box may have moved to a state the tree does not have.
        RefreshChrome();
    }

    private void OnClearClick(
        object sender,
        RoutedEventArgs e)
        => ClearSelection();

    private void OnPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key == Key.A &&
            Keyboard.Modifiers.HasFlag(ModifierKeys.Control) &&
            e.OriginalSource is not TextBox)
        {
            SelectAll();
            e.Handled = true;
        }
    }

    private void OnFilterPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key != Key.Down || shownRoots.Count == 0)
        {
            return;
        }

        PartTree.UpdateLayout();
        if (PartTree.ItemContainerGenerator.ContainerFromIndex(0) is TreeViewItem container)
        {
            container.Focus();
            e.Handled = true;
        }
    }

    private static bool ValidateMaxSelectionCount(object? value)
        => KeySelectionHelper.IsValidMaxSelectionCount(value);
}