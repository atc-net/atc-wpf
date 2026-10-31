// ReSharper disable InvertIf
#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The tri-state check-state engine of a check tree, independent of how the tree is shown (an indented tree, columns,
/// a flat list). It keeps <see cref="CheckTreeNode.IsChecked"/> of every node in sync with a set of keys:
/// <list type="bullet">
///   <item><description>keys to states: <see cref="ApplyKeys"/> (one pass over the tree);</description></item>
///   <item><description>user actions: <see cref="Toggle"/>, <see cref="SetChecked"/>, <see cref="SetAll"/> change only the
///   affected sub-tree and walk up the ancestors (O(affected nodes));</description></item>
///   <item><description>states to keys: <see cref="GetKeys"/> per <see cref="Strategy"/>.</description></item>
/// </list>
/// </summary>
/// <remarks>
/// <para>
/// Linked (<see cref="IsCheckLinked"/>): a parent is ticked when all its children are, unticked when none is (and none
/// is partial), otherwise partial (<see langword="null"/>). Disabled children count for the parent's state but are never
/// changed by a user action. A parent key in the keys ticks the whole sub-tree under it, whatever the strategy, so a
/// child added later under a ticked parent (or loaded lazily) comes up ticked.
/// </para>
/// <para>
/// Keys of nodes that are not in the tree (e.g. children that are not loaded yet) are kept in <see cref="GetKeys"/>, so a
/// saved selection survives until those nodes appear. Select all and Clear (over the whole tree) drop them. Unticking a
/// node whose children are not loaded yet marks it: when its children load, the kept keys of the loaded nodes under it
/// are dropped instead of ticking them again.
/// </para>
/// <para>
/// The engine tracks <see cref="CheckTreeNode.Children"/> changes: the changed parent's sub-tree is re-applied from the
/// current keys and <see cref="StructureChanged"/> is raised.
/// </para>
/// </remarks>
internal sealed class CheckTreeState
{
    private readonly Dictionary<CheckTreeNode, NodeInfo> infos = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<string, CheckTreeNode> nodesByKey = new(StringComparer.Ordinal);
    private readonly HashSet<CheckTreeNode> deferredParents = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<object, CheckTreeNode> childrenOwners = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<CheckTreeNode> prunedNodes = new(ReferenceEqualityComparer.Instance);
    private ObservableCollection<CheckTreeNode>? roots;
    private List<string> keys = [];
    private HashSet<string> keySet = new(StringComparer.Ordinal);
    private int deferDepth;
    private bool isRootsDirty;

    /// <summary>
    /// Gets or sets the callback run after the tree structure changed (nodes added to or removed from the roots or a
    /// <see cref="CheckTreeNode.Children"/> collection) and the states were re-applied. The argument is the changed
    /// parent, or <see langword="null"/> for the roots.
    /// </summary>
    public Action<CheckTreeNode?>? StructureChanged { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether parent and child check boxes are linked. Changing it re-applies the keys.
    /// </summary>
    public bool IsCheckLinked
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            ApplyKeys(keys);
        }
    }

        = true;

    /// <summary>
    /// Gets or sets which ticked nodes <see cref="GetKeys"/> returns when the check boxes are linked.
    /// </summary>
    public CheckedKeysStrategy Strategy { get; set; } = CheckedKeysStrategy.Leaves;

    /// <summary>
    /// Gets the number of node state writes so far (a measure of how many nodes an action touched).
    /// </summary>
    public int StateWriteCount { get; private set; }

    /// <summary>
    /// Gets the root nodes.
    /// </summary>
    public IReadOnlyList<CheckTreeNode> Roots
        => (IReadOnlyList<CheckTreeNode>?)roots ?? [];

    /// <summary>
    /// Gets the number of nodes in the tree.
    /// </summary>
    public int NodeCount => nodesByKey.Count;

    /// <summary>
    /// Sets the root nodes: indexes the tree, tracks its changes and applies the current keys.
    /// </summary>
    public void SetRoots(ObservableCollection<CheckTreeNode>? newRoots)
    {
        if (roots is not null)
        {
            CollectionChangedEventManager.RemoveHandler(roots, OnRootsCollectionChanged);
        }

        UnindexAll();
        prunedNodes.Clear();
        roots = newRoots;
        if (roots is not null)
        {
            CollectionChangedEventManager.AddHandler(roots, OnRootsCollectionChanged);
            foreach (var root in roots)
            {
                Index(root, parent: null);
            }
        }

        ApplyKeys(keys);
    }

    /// <summary>
    /// Returns the node with a key, or <see langword="null"/>.
    /// </summary>
    public CheckTreeNode? Find(string key)
        => nodesByKey.GetValueOrDefault(key);

    /// <summary>
    /// Returns the parent of a node, or <see langword="null"/> for a root (or a node not in the tree).
    /// </summary>
    public CheckTreeNode? GetParent(CheckTreeNode node)
        => infos.TryGetValue(node, out var info) ? info.Parent : null;

    /// <summary>
    /// Returns whether a node is in the tree.
    /// </summary>
    public bool Contains(CheckTreeNode node)
        => infos.ContainsKey(node);

    /// <summary>
    /// Returns the depth of a node (0 for a root).
    /// </summary>
    public int GetDepth(CheckTreeNode node)
    {
        var depth = 0;
        var parent = GetParent(node);
        while (parent is not null)
        {
            depth++;
            parent = GetParent(parent);
        }

        return depth;
    }

    /// <summary>
    /// Returns every node in tree (pre-)order.
    /// </summary>
    public IEnumerable<CheckTreeNode> EnumerateNodes()
    {
        if (roots is null)
        {
            yield break;
        }

        var stack = new Stack<CheckTreeNode>();
        for (var i = roots.Count - 1; i >= 0; i--)
        {
            stack.Push(roots[i]);
        }

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            yield return node;
            for (var i = node.Children.Count - 1; i >= 0; i--)
            {
                stack.Push(node.Children[i]);
            }
        }
    }

    /// <summary>
    /// Sets the keys and recomputes every state in one pass.
    /// </summary>
    public void ApplyKeys(IEnumerable<string> newKeys)
    {
        ArgumentNullException.ThrowIfNull(newKeys);

        keys = newKeys
            .Where(x => x is not null)
            .Distinct(StringComparer.Ordinal)
            .ToList();
        keySet = new HashSet<string>(keys, StringComparer.Ordinal);

        if (roots is null)
        {
            return;
        }

        foreach (var root in roots)
        {
            ApplyRecursive(root, inherited: false);
        }
    }

    /// <summary>
    /// Sets the keys from outside the engine (the bound <c>SelectedKeys</c>): forgets which unloaded nodes the user
    /// unticked, since the new keys say what is ticked, then applies them.
    /// </summary>
    public void ReplaceKeys(IEnumerable<string> newKeys)
    {
        prunedNodes.Clear();
        ApplyKeys(newKeys);
    }

    /// <summary>
    /// Returns the keys for the current states: per <see cref="Strategy"/> when linked, every ticked node otherwise; in
    /// tree order, followed by the kept keys of nodes that are not in the tree.
    /// </summary>
    public IReadOnlyList<string> GetKeys()
    {
        var result = new List<string>();
        if (roots is not null)
        {
            foreach (var root in roots)
            {
                CollectKeys(root, result);
            }
        }

        foreach (var key in keys)
        {
            if (!nodesByKey.ContainsKey(key))
            {
                result.Add(key);
            }
        }

        return result;
    }

    /// <summary>
    /// Stores <see cref="GetKeys"/> as the current keys (after a user action) and returns them.
    /// </summary>
    public IReadOnlyList<string> CommitKeys()
    {
        var result = GetKeys();
        keys = result.ToList();
        keySet = new HashSet<string>(keys, StringComparer.Ordinal);
        return result;
    }

    /// <summary>
    /// Toggles a node as a click does: linked, a node whose enabled (and shown) descendants are all ticked is unticked with
    /// them, any other node is ticked with them. Does nothing for a disabled node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="isShown">Which nodes are shown (filter); <see langword="null"/> for all.</param>
    /// <returns><see langword="true"/> when a state changed.</returns>
    public bool Toggle(
        CheckTreeNode node,
        Func<CheckTreeNode, bool>? isShown = null)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (!node.IsEnabled || !infos.ContainsKey(node))
        {
            return false;
        }

        var tick = IsCheckLinked
            ? !AreToggleableTicked(node, isShown)
            : node.IsChecked != true;

        return SetChecked(node, tick, isShown);
    }

    /// <summary>
    /// Ticks or unticks a node (linked: with its enabled, shown descendants) and updates its ancestors.
    /// Does nothing for a disabled node.
    /// </summary>
    /// <returns><see langword="true"/> when a state changed.</returns>
    public bool SetChecked(
        CheckTreeNode node,
        bool value,
        Func<CheckTreeNode, bool>? isShown = null)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (!node.IsEnabled || !infos.ContainsKey(node))
        {
            return false;
        }

        var before = StateWriteCount;
        if (!IsCheckLinked)
        {
            WriteState(node, value);
            return StateWriteCount != before;
        }

        MarkUnloaded(node, value, isShown);
        SetSubtree(node, value, isShown);
        Propagate(GetParent(node));
        return StateWriteCount != before;
    }

    /// <summary>
    /// Ticks or unticks every enabled (and shown) node, as Select all / Clear do.
    /// </summary>
    /// <returns><see langword="true"/> when a state changed.</returns>
    public bool SetAll(
        bool value,
        Func<CheckTreeNode, bool>? isShown = null)
    {
        if (roots is null)
        {
            return false;
        }

        var before = StateWriteCount;
        var isKeptKeyDropped = false;
        if (isShown is null)
        {
            // Over the whole tree, Select all and Clear also settle the kept keys of nodes that are not loaded: Clear
            // unticks them, and Select all ticks their loaded ancestors, which pass the tick on when they load.
            isKeptKeyDropped = keys.RemoveAll(x => !nodesByKey.ContainsKey(x)) > 0;
            if (isKeptKeyDropped)
            {
                keySet = new HashSet<string>(keys, StringComparer.Ordinal);
            }

            prunedNodes.Clear();
        }

        if (!IsCheckLinked)
        {
            foreach (var node in EnumerateNodes())
            {
                if (node.IsEnabled && (isShown is null || isShown(node)))
                {
                    WriteState(node, value);
                }
            }

            return isKeptKeyDropped || StateWriteCount != before;
        }

        foreach (var root in roots)
        {
            if (root.IsEnabled && (isShown is null || isShown(root)))
            {
                SetSubtree(root, value, isShown);
            }
        }

        return isKeptKeyDropped || StateWriteCount != before;
    }

    /// <summary>
    /// Returns the aggregate state of the enabled (and shown) nodes, for a "Select all" check box.
    /// </summary>
    public bool? GetAllState(Func<CheckTreeNode, bool>? isShown = null)
    {
        var ticked = 0;
        var total = 0;
        foreach (var node in EnumerateNodes())
        {
            if (!node.IsEnabled || (isShown is not null && !isShown(node)) || (IsCheckLinked && HasChildren(node)))
            {
                continue;
            }

            total++;
            if (node.IsChecked == true)
            {
                ticked++;
            }
        }

        return KeySelectionHelper.GetAggregateState(ticked, total);
    }

    /// <summary>
    /// Returns the number of ticked leaves (nodes without loaded children; every node when not linked) and the number
    /// of leaves.
    /// </summary>
    public (int Ticked, int Total) CountLeaves()
    {
        var ticked = 0;
        var total = 0;
        foreach (var node in EnumerateNodes())
        {
            if (IsCheckLinked && HasChildren(node))
            {
                continue;
            }

            total++;
            if (node.IsChecked == true)
            {
                ticked++;
            }
        }

        return (ticked, total);
    }

    /// <summary>
    /// Defers the handling of structure changes until the returned scope is disposed; each changed parent is then
    /// re-applied once.
    /// </summary>
    public IDisposable DeferStructureChanges()
    {
        deferDepth++;
        return new DeferScope(this);
    }

    /// <summary>
    /// Stops tracking the tree (unsubscribes from every collection).
    /// </summary>
    public void Detach()
    {
        if (roots is not null)
        {
            CollectionChangedEventManager.RemoveHandler(roots, OnRootsCollectionChanged);
            roots = null;
        }

        UnindexAll();
        prunedNodes.Clear();
    }

    private static bool HasChildren(CheckTreeNode node)
        => node.Children.Count > 0;

    private static bool IsStateOf(
        bool? state,
        bool? value)
        => state == value;

    private void ApplyRecursive(
        CheckTreeNode node,
        bool inherited)
    {
        var info = infos[node];
        var isOn = keySet.Contains(node.Key);
        if (!IsCheckLinked)
        {
            info.CheckedChildren = 0;
            info.PartialChildren = 0;
            foreach (var child in node.Children)
            {
                ApplyRecursive(child, inherited: false);
                Count(info, child.IsChecked, +1);
            }

            WriteRaw(node, isOn);
            return;
        }

        isOn |= inherited;
        if (!HasChildren(node))
        {
            info.CheckedChildren = 0;
            info.PartialChildren = 0;
            WriteRaw(node, isOn);
            return;
        }

        info.CheckedChildren = 0;
        info.PartialChildren = 0;
        foreach (var child in node.Children)
        {
            ApplyRecursive(child, isOn);
            Count(info, child.IsChecked, +1);
        }

        WriteRaw(node, StateFromCounters(node, info));
    }

    private void CollectKeys(
        CheckTreeNode node,
        List<string> result)
    {
        if (!IsCheckLinked)
        {
            if (node.IsChecked == true)
            {
                result.Add(node.Key);
            }

            foreach (var child in node.Children)
            {
                CollectKeys(child, result);
            }

            return;
        }

        if (node.IsChecked == false)
        {
            return;
        }

        var hasChildren = HasChildren(node);
        if (node.IsChecked == true)
        {
            switch (Strategy)
            {
                case CheckedKeysStrategy.Parents:
                    result.Add(node.Key);
                    return;
                case CheckedKeysStrategy.All:
                    result.Add(node.Key);
                    break;
                default:
                    if (!hasChildren)
                    {
                        result.Add(node.Key);
                        return;
                    }

                    break;
            }
        }

        foreach (var child in node.Children)
        {
            CollectKeys(child, result);
        }
    }

    /// <summary>
    /// Records (on untick) or forgets (on tick) the nodes in a sub-tree whose children are not loaded yet.
    /// </summary>
    private void MarkUnloaded(
        CheckTreeNode node,
        bool value,
        Func<CheckTreeNode, bool>? isShown)
    {
        var stack = new Stack<CheckTreeNode>();
        stack.Push(node);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current.HasUnloadedChildren)
            {
                if (value)
                {
                    prunedNodes.Remove(current);
                }
                else
                {
                    prunedNodes.Add(current);
                }
            }

            foreach (var child in current.Children)
            {
                if (child.IsEnabled && (isShown is null || isShown(child)))
                {
                    stack.Push(child);
                }
            }
        }
    }

    /// <summary>
    /// Drops the kept keys of the nodes now loaded under a node the user unticked before they loaded; nodes under it
    /// that still have unloaded children inherit the mark.
    /// </summary>
    private void DropKeysUnderPrunedNode(CheckTreeNode parent)
    {
        if (!prunedNodes.Remove(parent))
        {
            return;
        }

        var isChanged = false;
        var stack = new Stack<CheckTreeNode>(parent.Children);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            isChanged |= keySet.Remove(current.Key);
            if (current.HasUnloadedChildren)
            {
                prunedNodes.Add(current);
            }

            foreach (var child in current.Children)
            {
                stack.Push(child);
            }
        }

        if (isChanged)
        {
            keys = keys.Where(keySet.Contains).ToList();
        }
    }

    private bool AreToggleableTicked(
        CheckTreeNode node,
        Func<CheckTreeNode, bool>? isShown)
    {
        if (!HasChildren(node))
        {
            return node.IsChecked == true;
        }

        foreach (var child in node.Children)
        {
            if (child.IsEnabled &&
                (isShown is null || isShown(child)) &&
                !AreToggleableTicked(child, isShown))
            {
                return false;
            }
        }

        return true;
    }

    private void SetSubtree(
        CheckTreeNode node,
        bool value,
        Func<CheckTreeNode, bool>? isShown)
    {
        if (!HasChildren(node))
        {
            WriteState(node, value);
            return;
        }

        foreach (var child in node.Children)
        {
            if (child.IsEnabled && (isShown is null || isShown(child)))
            {
                SetSubtree(child, value, isShown);
            }
        }

        WriteState(node, StateFromCounters(node, infos[node]));
    }

    private void Propagate(CheckTreeNode? node)
    {
        while (node is not null)
        {
            var newState = StateFromCounters(node, infos[node]);
            if (IsStateOf(node.IsChecked, newState))
            {
                return;
            }

            WriteState(node, newState);
            node = GetParent(node);
        }
    }

    private static bool? StateFromCounters(
        CheckTreeNode node,
        NodeInfo info)
    {
        var count = node.Children.Count;
        if (count > 0 && info.CheckedChildren >= count)
        {
            return true;
        }

        return info.CheckedChildren == 0 && info.PartialChildren == 0
            ? false
            : null;
    }

    private static void Count(
        NodeInfo info,
        bool? state,
        int delta)
    {
        if (state == true)
        {
            info.CheckedChildren += delta;
        }
        else if (state is null)
        {
            info.PartialChildren += delta;
        }
    }

    /// <summary>
    /// Writes a state and keeps the parent's counters in step (the parent's own state is not recomputed).
    /// </summary>
    private void WriteState(
        CheckTreeNode node,
        bool? value)
    {
        var old = node.IsChecked;
        if (IsStateOf(old, value))
        {
            return;
        }

        WriteRaw(node, value);
        if (GetParent(node) is { } parent)
        {
            var parentInfo = infos[parent];
            Count(parentInfo, old, -1);
            Count(parentInfo, value, +1);
        }
    }

    /// <summary>
    /// Writes a state without touching the parent's counters.
    /// </summary>
    private void WriteRaw(
        CheckTreeNode node,
        bool? value)
    {
        if (IsStateOf(node.IsChecked, value))
        {
            return;
        }

        node.IsChecked = value;
        StateWriteCount++;
    }

    private void Index(
        CheckTreeNode node,
        CheckTreeNode? parent)
    {
        if (infos.ContainsKey(node))
        {
            throw new InvalidOperationException($"The node '{node.Key}' is in the tree more than once.");
        }

        if (nodesByKey.TryGetValue(node.Key, out var existing) && !ReferenceEquals(existing, node))
        {
            // Keys identify the nodes in SelectedKeys: two nodes with one key could not be ticked independently.
            throw new InvalidOperationException(
                $"Another node with the key '{node.Key}' is already in the tree; every node needs a unique key.");
        }

        var info = new NodeInfo
        {
            Parent = parent,
            IndexedChildren = node.Children.ToList(),
        };
        infos[node] = info;
        nodesByKey[node.Key] = node;
        childrenOwners[node.Children] = node;
        CollectionChangedEventManager.AddHandler(node.Children, OnChildrenCollectionChanged);
        foreach (var child in info.IndexedChildren)
        {
            Index(child, node);
        }
    }

    private void Unindex(CheckTreeNode node)
    {
        if (!infos.Remove(node, out var info))
        {
            return;
        }

        if (nodesByKey.TryGetValue(node.Key, out var indexed) && ReferenceEquals(indexed, node))
        {
            nodesByKey.Remove(node.Key);
        }

        childrenOwners.Remove(node.Children);
        CollectionChangedEventManager.RemoveHandler(node.Children, OnChildrenCollectionChanged);
        foreach (var child in info.IndexedChildren)
        {
            Unindex(child);
        }
    }

    private void UnindexAll()
    {
        foreach (var node in infos.Keys.ToList())
        {
            CollectionChangedEventManager.RemoveHandler(node.Children, OnChildrenCollectionChanged);
        }

        infos.Clear();
        nodesByKey.Clear();
        childrenOwners.Clear();
        deferredParents.Clear();
    }

    private void OnRootsCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (deferDepth > 0)
        {
            isRootsDirty = true;
            return;
        }

        RefreshRoots();
    }

    private void RefreshRoots()
    {
        var currentRoots = roots;
        UnindexAll();
        if (currentRoots is not null)
        {
            foreach (var root in currentRoots)
            {
                Index(root, parent: null);
            }
        }

        prunedNodes.RemoveWhere(x => !infos.ContainsKey(x));
        ApplyKeys(keys);
        StructureChanged?.Invoke(null);
    }

    private void OnChildrenCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (sender is null || !childrenOwners.TryGetValue(sender, out var parent))
        {
            return;
        }

        if (deferDepth > 0)
        {
            deferredParents.Add(parent);
            return;
        }

        RefreshParent(parent);
    }

    private void RefreshParent(CheckTreeNode parent)
    {
        if (!infos.TryGetValue(parent, out var info))
        {
            return;
        }

        foreach (var child in info.IndexedChildren)
        {
            Unindex(child);
        }

        info.IndexedChildren = parent.Children.ToList();
        foreach (var child in info.IndexedChildren)
        {
            Index(child, parent);
        }

        DropKeysUnderPrunedNode(parent);
        prunedNodes.RemoveWhere(x => !infos.ContainsKey(x));

        var oldState = parent.IsChecked;
        ApplyRecursive(parent, IsAnyAncestorKeyed(parent));
        if (info.Parent is { } grandParent && !IsStateOf(oldState, parent.IsChecked))
        {
            var grandInfo = infos[grandParent];
            Count(grandInfo, oldState, -1);
            Count(grandInfo, parent.IsChecked, +1);
            if (IsCheckLinked)
            {
                Propagate(grandParent);
            }
        }

        StructureChanged?.Invoke(parent);
    }

    private bool IsAnyAncestorKeyed(CheckTreeNode node)
    {
        if (!IsCheckLinked)
        {
            return false;
        }

        var parent = GetParent(node);
        while (parent is not null)
        {
            if (keySet.Contains(parent.Key))
            {
                return true;
            }

            parent = GetParent(parent);
        }

        return false;
    }

    private void EndDefer()
    {
        deferDepth--;
        if (deferDepth > 0)
        {
            return;
        }

        if (isRootsDirty)
        {
            isRootsDirty = false;
            deferredParents.Clear();
            RefreshRoots();
            return;
        }

        var parents = deferredParents.ToList();
        deferredParents.Clear();
        foreach (var parent in parents)
        {
            RefreshParent(parent);
        }
    }

    private sealed class NodeInfo
    {
        public CheckTreeNode? Parent { get; init; }

        public List<CheckTreeNode> IndexedChildren { get; set; } = [];

        public int CheckedChildren { get; set; }

        public int PartialChildren { get; set; }
    }

    private sealed class DeferScope(CheckTreeState owner) : IDisposable
    {
        private bool isDisposed;

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            owner.EndDefer();
        }
    }
}