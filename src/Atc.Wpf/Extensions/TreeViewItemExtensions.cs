// ReSharper disable once CheckNamespace
namespace System.Windows.Controls;

public static class TreeViewItemExtensions
{
    public static int GetDepth(this TreeViewItem item)
        => item.CountAncestors<TreeView>(x => x is TreeViewItem);
}