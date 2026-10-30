#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The result of filtering a check tree: a node is kept when its text matches, when an ancestor matches (everything
/// under a match is kept) or when a descendant matches (then it is an ancestor to expand).
/// </summary>
internal sealed class TreeFilterResult(string filterText)
{
    /// <summary>
    /// Gets the kept children of every kept node.
    /// </summary>
    public Dictionary<CheckTreeNode, List<CheckTreeNode>> KeptChildren { get; } = new(ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Gets every kept node.
    /// </summary>
    public HashSet<CheckTreeNode> ShownSet { get; } = new(ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Gets the nodes whose own text matches, in tree order.
    /// </summary>
    public List<CheckTreeNode> Matches { get; } = [];

    /// <summary>
    /// Gets the kept nodes that are kept only for a matching descendant.
    /// </summary>
    public List<CheckTreeNode> AncestorsToExpand { get; } = [];

    /// <summary>
    /// Gets the kept roots.
    /// </summary>
    public List<CheckTreeNode> ShownRoots { get; } = [];

    /// <summary>
    /// Filters a node and its sub-tree.
    /// </summary>
    /// <returns><see langword="true"/> when the node is kept.</returns>
    public bool Visit(
        CheckTreeNode node,
        bool ancestorMatched)
    {
        ArgumentNullException.ThrowIfNull(node);

        var isMatch = KeySelectionHelper.MatchesFilter(node.Text, filterText);
        if (isMatch)
        {
            Matches.Add(node);
        }

        var keepAll = ancestorMatched || isMatch;
        var keptChildren = new List<CheckTreeNode>();
        foreach (var child in node.Children)
        {
            if (Visit(child, keepAll))
            {
                keptChildren.Add(child);
            }
        }

        if (!keepAll && keptChildren.Count == 0)
        {
            return false;
        }

        KeptChildren[node] = keptChildren;
        ShownSet.Add(node);
        if (!keepAll)
        {
            AncestorsToExpand.Add(node);
        }

        return true;
    }
}