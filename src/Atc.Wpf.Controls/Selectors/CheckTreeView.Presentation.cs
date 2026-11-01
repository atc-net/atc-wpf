// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <content>
/// The presentation: the shown children of each node (filter, "please wait" rows), the row containers, the filter and
/// lazy loading.
/// </content>
public partial class CheckTreeView
{
    private const string PathSeparator = " › ";

    private readonly ObservableCollectionEx<CheckTreeNode> shownRoots = [];
    private readonly Dictionary<CheckTreeNode, ObservableCollectionEx<CheckTreeNode>> shownChildren = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<CheckTreeNode, CheckTreeNode> placeholders = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<CheckTreeNode> placeholderSet = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<CheckTreeNode, bool> expansionBeforeFilter = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<CheckTreeNode> loadingNodes = new(ReferenceEqualityComparer.Instance);
    private Dictionary<CheckTreeNode, List<CheckTreeNode>>? filteredChildren;
    private HashSet<CheckTreeNode>? filterShownSet;
    private string? filterMatchesText;
    private DispatcherTimer? filterTimer;
    private CancellationTokenSource? loadCancellation;

    /// <summary>
    /// Gets the nodes shown at the top level (the roots, the filtered roots, or the flat filter results).
    /// </summary>
    internal IReadOnlyList<CheckTreeNode> ShownRoots => shownRoots;

    /// <summary>
    /// Gets a value indicating whether the filter shows its matches as a flat list.
    /// </summary>
    internal bool IsShowingFlatResults
        => filterMatchesText is not null && FilterMode == TreeFilterMode.FlatResults;

    /// <summary>
    /// Returns the children shown under a node (after the filter; the "please wait" row while they load).
    /// </summary>
    internal IReadOnlyList<CheckTreeNode> GetShownChildren(CheckTreeNode node)
    {
        if (!shownChildren.TryGetValue(node, out var shown))
        {
            shown = [];
            SyncShown(shown, ComputeShownChildren(node));
            shownChildren[node] = shown;
        }

        return shown;
    }

    /// <summary>
    /// Returns whether a node is a "please wait" row.
    /// </summary>
    internal bool IsPlaceholder(CheckTreeNode node)
        => placeholderSet.Contains(node);

    /// <summary>
    /// Returns whether a node is shown (always, without a filter).
    /// </summary>
    internal bool IsShown(CheckTreeNode node)
        => filterShownSet is null || filterShownSet.Contains(node);

    /// <summary>
    /// Returns the path of a node: the texts of its ancestors.
    /// </summary>
    internal string GetPathText(CheckTreeNode node)
    {
        var parts = new List<string>();
        var parent = state.GetParent(node);
        while (parent is not null)
        {
            parts.Add(parent.Text);
            parent = state.GetParent(parent);
        }

        parts.Reverse();
        return string.Join(PathSeparator, parts);
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
    /// Starts loading the children of a node with unloaded children (as its first expand does).
    /// </summary>
    /// <returns>A task that completes when the children are in the tree (or the load failed or was cancelled).</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "A failing load leaves the node unloaded (it loads again on the next expand) instead of crashing the UI.")]
    internal async Task LoadChildrenOfAsync(CheckTreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (LoadChildrenAsync is not { } loader ||
            !node.HasUnloadedChildren ||
            node.Children.Count > 0 ||
            !state.Contains(node) ||
            !loadingNodes.Add(node))
        {
            return;
        }

        loadCancellation ??= new CancellationTokenSource();
        var cancellation = loadCancellation;
        try
        {
            var children = await loader(node, cancellation.Token).ConfigureAwait(true) ?? [];
            if (cancellation.IsCancellationRequested || !state.Contains(node))
            {
                return;
            }

            using (state.DeferStructureChanges())
            {
                node.HasUnloadedChildren = false;
                foreach (var child in children)
                {
                    node.Children.Add(child);
                }
            }

            if (children.Count == 0 && shownChildren.TryGetValue(node, out var shown))
            {
                SyncShown(shown, ComputeShownChildren(node));
            }
        }
        catch (OperationCanceledException)
        {
            // Cancelled: the node stays unloaded.
        }
        catch (Exception)
        {
            node.IsExpanded = false;
        }
        finally
        {
            loadingNodes.Remove(node);
        }
    }

    /// <summary>
    /// Prepares a row container for a node: its header, shown children and expansion.
    /// </summary>
    internal void PrepareContainer(
        CheckTreeViewItem container,
        CheckTreeNode node)
    {
        var isPlaceholder = IsPlaceholder(node);
        container.Node = node;
        container.IsPlaceholder = isPlaceholder;
        container.HeaderTemplate = (DataTemplate)Resources[isPlaceholder ? "CheckTreeBusyRowTemplate" : "CheckTreeRowTemplate"];
        container.SetBinding(
            AutomationProperties.NameProperty,
            new Binding(nameof(CheckTreeNode.Text)) { Source = node, Mode = BindingMode.OneWay });

        if (isPlaceholder || IsShowingFlatResults)
        {
            BindingOperations.ClearBinding(container, TreeViewItem.IsExpandedProperty);
            container.SetCurrentValue(TreeViewItem.IsExpandedProperty, false);
            container.ItemsSource = null;
            container.Tag = isPlaceholder ? null : GetPathText(node);
            return;
        }

        container.Tag = null;
        container.ItemsSource = GetShownChildren(node);
        container.SetBinding(
            TreeViewItem.IsExpandedProperty,
            new Binding(nameof(CheckTreeNode.IsExpanded)) { Source = node, Mode = BindingMode.TwoWay });
    }

    private static void OnLoadChildrenAsyncChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((CheckTreeView)d).RebuildShown();

    private static void OnFilterModeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CheckTreeView)d;
        if (control.filterMatchesText is not null)
        {
            control.ApplyFilter();
        }
    }

    private static void OnFilterTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (CheckTreeView)d;
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

    private void OnFilterTimerTick(
        object? sender,
        EventArgs e)
        => FlushPendingFilter();

    private Func<CheckTreeNode, bool>? GetShownPredicate()
        => filterShownSet is { } shownSet
            ? shownSet.Contains
            : null;

    private IList<CheckTreeNode> ComputeShownChildren(CheckTreeNode node)
    {
        if (IsShowingFlatResults)
        {
            return [];
        }

        if (filteredChildren is not null)
        {
            return filteredChildren.TryGetValue(node, out var kept)
                ? kept
                : [];
        }

        if (node.Children.Count == 0 &&
            node.HasUnloadedChildren &&
            LoadChildrenAsync is not null)
        {
            return [GetPlaceholder(node)];
        }

        return node.Children;
    }

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

    /// <summary>
    /// Recomputes every shown-children list (after the roots, the filter or the loader changed).
    /// </summary>
    private void RebuildShown()
    {
        if (filterMatchesText is not null || !string.IsNullOrWhiteSpace(FilterText))
        {
            ApplyFilter();
            return;
        }

        foreach (var node in shownChildren.Keys.ToList())
        {
            if (!state.Contains(node))
            {
                shownChildren.Remove(node);
            }
        }

        foreach (var (node, shown) in shownChildren)
        {
            SyncShown(shown, ComputeShownChildren(node));
        }

        SyncShown(shownRoots, (IList<CheckTreeNode>?)Nodes ?? []);
    }

    private void ApplyFilter()
    {
        var text = string.IsNullOrWhiteSpace(FilterText)
            ? null
            : FilterText.Trim();

        if (text is null)
        {
            filterMatchesText = null;
            filteredChildren = null;
            filterShownSet = null;
            RestoreExpansion();
            RebuildShown();
            RefreshChrome();
            return;
        }

        var result = new TreeFilterResult(text);
        foreach (var root in (IEnumerable<CheckTreeNode>?)Nodes ?? [])
        {
            if (result.Visit(root, ancestorMatched: false))
            {
                result.ShownRoots.Add(root);
            }
        }

        filterMatchesText = text;
        filteredChildren = result.KeptChildren;
        filterShownSet = result.ShownSet;

        if (FilterMode == TreeFilterMode.KeepAncestors)
        {
            foreach (var node in result.AncestorsToExpand)
            {
                if (!node.IsExpanded)
                {
                    expansionBeforeFilter.TryAdd(node, value: false);
                    node.IsExpanded = true;
                }
            }
        }

        foreach (var (node, shown) in shownChildren)
        {
            SyncShown(shown, ComputeShownChildren(node));
        }

        SyncShown(shownRoots, FilterMode == TreeFilterMode.FlatResults ? result.Matches : result.ShownRoots);
        RefreshChrome();
    }

    private void RestoreExpansion()
    {
        foreach (var (node, wasExpanded) in expansionBeforeFilter)
        {
            node.IsExpanded = wasExpanded;
        }

        expansionBeforeFilter.Clear();
    }

    private void ExpandToDepth()
    {
        if (ExpandDepth <= 0 || Nodes is null)
        {
            return;
        }

        foreach (var root in Nodes)
        {
            Expand(root, 1);
        }

        void Expand(
            CheckTreeNode node,
            int level)
        {
            if (node.Children.Count == 0)
            {
                return;
            }

            node.IsExpanded = true;
            if (level >= ExpandDepth)
            {
                return;
            }

            foreach (var child in node.Children)
            {
                Expand(child, level + 1);
            }
        }
    }

    private void OnItemExpanded(
        object sender,
        RoutedEventArgs e)
    {
        if (e.OriginalSource is CheckTreeViewItem { IsPlaceholder: false, Node: { HasUnloadedChildren: true } node })
        {
            _ = LoadChildrenOfAsync(node);
        }
    }

    private void CancelLoads()
    {
        if (loadCancellation is null)
        {
            return;
        }

        loadCancellation.Cancel();
        loadCancellation.Dispose();
        loadCancellation = null;
        loadingNodes.Clear();
    }
}