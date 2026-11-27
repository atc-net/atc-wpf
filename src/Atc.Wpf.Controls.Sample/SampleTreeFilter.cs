namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// Filters the items of a sample tree: by text (ignoring case) or, for a filter of two or more capitals only,
/// by the capitals of the text (<c>NW</c> matches <c>NiceWindow</c>).
/// </summary>
public static class SampleTreeFilter
{
    /// <summary>
    /// Determines whether a text matches a filter. A filter of two or more capitals only matches the capitals of
    /// the text (<c>TS</c> matches <c>LabelToggleSwitch</c>); any other filter matches a text containing it,
    /// ignoring case.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="filter">The filter.</param>
    /// <returns><see langword="true"/> when the text matches; <see langword="false"/> for an empty text or filter.</returns>
    public static bool Matches(
        string? text,
        string? filter)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(filter))
        {
            return false;
        }

        if (filter.Length >= 2 && filter.All(char.IsUpper))
        {
            var capitals = new string(text.Where(char.IsUpper).ToArray());
            return capitals.Contains(filter, StringComparison.Ordinal);
        }

        return text.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Shows the tree items whose header, tag or sample path match the filter, and the parents of those items
    /// (expanded), and hides the rest. An empty or white-space filter shows every item.
    /// </summary>
    /// <param name="items">The tree items.</param>
    /// <param name="filter">The filter.</param>
    /// <returns><see langword="true"/> when at least one item is shown.</returns>
    public static bool ApplyFilter(
        IEnumerable items,
        string? filter)
    {
        ArgumentNullException.ThrowIfNull(items);

        return ApplyTrimmedFilter(items, filter?.Trim() ?? string.Empty);
    }

    /// <summary>
    /// Counts the sample items (<see cref="SampleTreeViewItem"/>, not the category items) that match the filter.
    /// </summary>
    /// <param name="items">The tree items.</param>
    /// <param name="filter">The filter.</param>
    /// <returns>The number of matching samples; 0 for an empty or white-space filter.</returns>
    public static int CountMatches(
        IEnumerable items,
        string? filter)
    {
        ArgumentNullException.ThrowIfNull(items);

        var trimmed = filter?.Trim() ?? string.Empty;
        return trimmed.Length == 0
            ? 0
            : CountTrimmedMatches(items, trimmed);
    }

    /// <summary>
    /// Expands or collapses the tree items and all their sub-items.
    /// </summary>
    /// <param name="items">The tree items.</param>
    /// <param name="isExpanded"><see langword="true"/> to expand; <see langword="false"/> to collapse.</param>
    public static void SetExpanded(
        IEnumerable items,
        bool isExpanded)
    {
        ArgumentNullException.ThrowIfNull(items);

        foreach (var treeViewItem in items.OfType<TreeViewItem>())
        {
            treeViewItem.SetCurrentValue(TreeViewItem.IsExpandedProperty, isExpanded);
            SetExpanded(treeViewItem.Items, isExpanded);
        }
    }

    private static bool ApplyTrimmedFilter(
        IEnumerable items,
        string filter)
    {
        var showAny = false;
        foreach (var treeViewItem in items.OfType<TreeViewItem>())
        {
            var showBySubItems = ApplyTrimmedFilter(treeViewItem.Items, filter);
            var showByItem = filter.Length == 0 || IsMatch(treeViewItem, filter);

            treeViewItem.SetCurrentValue(
                UIElement.VisibilityProperty,
                showByItem || showBySubItems ? Visibility.Visible : Visibility.Collapsed);

            if (filter.Length > 0 && showBySubItems)
            {
                treeViewItem.SetCurrentValue(TreeViewItem.IsExpandedProperty, value: true);
            }

            showAny |= showByItem || showBySubItems;
        }

        return showAny;
    }

    private static int CountTrimmedMatches(
        IEnumerable items,
        string filter)
    {
        var count = 0;
        foreach (var treeViewItem in items.OfType<TreeViewItem>())
        {
            count += CountTrimmedMatches(treeViewItem.Items, filter);
            if (treeViewItem is SampleTreeViewItem && IsMatch(treeViewItem, filter))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsMatch(
        TreeViewItem treeViewItem,
        string filter)
        => Matches(treeViewItem.Header as string, filter) ||
           Matches(treeViewItem.Tag as string, filter) ||
           (treeViewItem is SampleTreeViewItem sampleItem && Matches(sampleItem.SamplePath, filter));
}