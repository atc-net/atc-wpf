// ReSharper disable once CheckNamespace
namespace System.Windows.Controls;

/// <summary>
/// Extension methods for <see cref="TreeViewItem"/>.
/// </summary>
public static class TreeViewItemExtensions
{
    /// <summary>
    /// Gets the nesting depth of the item, counted as the number of ancestor tree view items.
    /// </summary>
    public static int GetDepth(this TreeViewItem item)
        => item.CountAncestors<TreeView>(x => x is TreeViewItem);
}