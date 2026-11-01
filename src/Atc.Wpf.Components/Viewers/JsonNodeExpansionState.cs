namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// Remembers which nodes of a <see cref="JsonViewer"/> tree are expanded, keyed by node rather than by tree item, and
/// converts a node to its expanded state for the tree items' <see cref="TreeViewItem.IsExpanded"/> binding.
/// </summary>
/// <remarks>
/// With a virtualizing tree the tree items are discarded when they scroll out of view and recreated when the tree is
/// re-templated, so the state cannot live on them: each tree item reads it from here when it gets its node.
/// </remarks>
internal sealed class JsonNodeExpansionState : IValueConverter
{
    private readonly HashSet<JsonNode> expanded = new(ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Returns whether <paramref name="node"/> is expanded.
    /// </summary>
    public bool IsExpanded(JsonNode node)
        => expanded.Contains(node);

    /// <summary>
    /// Records that <paramref name="node"/> was expanded or collapsed.
    /// </summary>
    public void Set(
        JsonNode node,
        bool isExpanded)
    {
        if (isExpanded)
        {
            expanded.Add(node);
        }
        else
        {
            expanded.Remove(node);
        }
    }

    /// <summary>
    /// Collapses every node.
    /// </summary>
    public void Clear()
        => expanded.Clear();

    /// <summary>
    /// Expands <paramref name="roots"/> and every node below them that has children.
    /// </summary>
    public void ExpandAll(IEnumerable<JsonNode> roots)
    {
        ArgumentNullException.ThrowIfNull(roots);

        var pending = new Stack<JsonNode>(roots);
        while (pending.Count > 0)
        {
            var node = pending.Pop();
            var hasChildren = false;
            foreach (var child in node.Children())
            {
                hasChildren = true;
                pending.Push(child);
            }

            if (hasChildren)
            {
                expanded.Add(node);
            }
        }
    }

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is JsonNode node && expanded.Contains(node);

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();
}