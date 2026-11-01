#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Shared selection logic for the multi-select controls that work on a key/value <c>Items</c> dictionary
/// and a <c>SelectedKeys</c> collection: stale-key cleanup, new-key ticking, batched in-place updates and filtering.
/// </summary>
internal static class KeySelectionHelper
{
    /// <summary>
    /// Above this many items the filter is applied after a short pause in typing instead of on each key press.
    /// </summary>
    public const int FilterDebounceThreshold = 2000;

    /// <summary>
    /// The pause in typing before the filter is applied for large item sets.
    /// </summary>
    public static readonly TimeSpan FilterDebounceDelay = TimeSpan.FromMilliseconds(150);

    /// <summary>
    /// An empty key set (for a <see cref="SelectedKeysCleanup"/> request that is not about items).
    /// </summary>
    public static readonly IReadOnlySet<string> EmptyKeySet = new HashSet<string>(StringComparer.Ordinal);

    /// <summary>
    /// Returns the selection after <c>Items</c> changed from <paramref name="oldItemKeys"/> to <paramref name="newItems"/>:
    /// keys of items that disappeared (present in <paramref name="oldItemKeys"/>, gone from <paramref name="newItems"/>)
    /// are dropped, while keys that were never among the items are kept (a restored selection survives an empty first
    /// <c>Items</c>) and, when <paramref name="selectNewItems"/> is set, keys that were not
    /// present before are ticked (in item order), but only while the selection holds fewer than
    /// <paramref name="maxSelectionCount"/> keys.
    /// </summary>
    public static IReadOnlyList<string> GetSelectionAfterItemsChanged(
        IReadOnlyCollection<string> currentSelection,
        IReadOnlySet<string> oldItemKeys,
        IReadOnlyDictionary<string, string>? newItems,
        bool selectNewItems,
        int? maxSelectionCount = null)
    {
        ArgumentNullException.ThrowIfNull(currentSelection);
        ArgumentNullException.ThrowIfNull(oldItemKeys);

        if (newItems is null)
        {
            return currentSelection.ToList();
        }

        var result = currentSelection
            .Where(x => newItems.ContainsKey(x) || !oldItemKeys.Contains(x))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (!selectNewItems)
        {
            return result;
        }

        var selected = new HashSet<string>(result, StringComparer.Ordinal);
        foreach (var key in newItems.Keys)
        {
            if (maxSelectionCount is { } max && result.Count >= max)
            {
                break;
            }

            if (!oldItemKeys.Contains(key) && selected.Add(key))
            {
                result.Add(key);
            }
        }

        return result;
    }

    /// <summary>
    /// Returns the selection with <paramref name="keysToAdd"/> ticked and <paramref name="keysToRemove"/> unticked,
    /// keeping the order of the keys already selected and appending new keys in the given order.
    /// </summary>
    public static IReadOnlyList<string> GetSelectionWith(
        IReadOnlyCollection<string> currentSelection,
        IEnumerable<string> keysToAdd,
        IEnumerable<string> keysToRemove)
    {
        ArgumentNullException.ThrowIfNull(currentSelection);
        ArgumentNullException.ThrowIfNull(keysToAdd);
        ArgumentNullException.ThrowIfNull(keysToRemove);

        var removeSet = new HashSet<string>(keysToRemove, StringComparer.Ordinal);
        var result = currentSelection
            .Where(x => !removeSet.Contains(x))
            .ToList();

        var selected = new HashSet<string>(result, StringComparer.Ordinal);
        foreach (var key in keysToAdd)
        {
            if (!removeSet.Contains(key) && selected.Add(key))
            {
                result.Add(key);
            }
        }

        return result;
    }

    /// <summary>
    /// Returns whether a value is allowed for a <c>MaxSelectionCount</c> property: <see langword="null"/> (no limit) or
    /// at least one. Used as the validate-value callback, so a lower value throws an <see cref="ArgumentException"/>.
    /// </summary>
    public static bool IsValidMaxSelectionCount(object? value)
        => value is null or int and >= 1;

    /// <summary>
    /// Returns whether two selections hold the same keys in the same order.
    /// </summary>
    public static bool SequenceEquals(
        IReadOnlyList<string> first,
        IReadOnlyList<string> second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        if (first.Count != second.Count)
        {
            return false;
        }

        for (var i = 0; i < first.Count; i++)
        {
            if (!string.Equals(first[i], second[i], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns whether two selections hold the same keys, in any order. The family compares selections this way:
    /// <see cref="ApplyInPlace"/> keeps the order in which keys were ticked, so a selection that differs only in
    /// order is not a change.
    /// </summary>
    public static bool SetEquals(
        IReadOnlyCollection<string> first,
        IReadOnlyCollection<string> second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        var firstSet = new HashSet<string>(first, StringComparer.Ordinal);
        return firstSet.SetEquals(second);
    }

    /// <summary>
    /// Updates <paramref name="target"/> in place so it holds <paramref name="newKeys"/>: keys that stay keep their
    /// place and new keys are appended in the given order (duplicates skipped), so the collection keeps the order in
    /// which keys were ticked. Nothing is raised when the keys do not change. An
    /// <see cref="ObservableCollectionEx{T}"/> raises a single reset notification; any other collection gets the
    /// smallest set of removes and adds (or a clear and refill when most keys go).
    /// </summary>
    /// <returns><see langword="true"/> when <paramref name="target"/> was changed.</returns>
    public static bool ApplyInPlace(
        ObservableCollection<string> target,
        IReadOnlyList<string> newKeys)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(newKeys);

        var newSet = new HashSet<string>(newKeys, StringComparer.Ordinal);
        var kept = target
            .Where(newSet.Contains)
            .Distinct(StringComparer.Ordinal)
            .ToList();
        var keptSet = new HashSet<string>(kept, StringComparer.Ordinal);
        var added = newKeys
            .Where(keptSet.Add)
            .ToList();

        if (added.Count == 0 && kept.Count == target.Count)
        {
            return false;
        }

        if (target is ObservableCollectionEx<string> rangeCollection)
        {
            ResetTo(rangeCollection, kept.Concat(added));
            return true;
        }

        var removeCount = target.Count - kept.Count;
        if (removeCount > 0 && removeCount > target.Count / 2)
        {
            target.Clear();
            foreach (var key in kept.Concat(added))
            {
                target.Add(key);
            }

            return true;
        }

        for (var i = target.Count - 1; i >= 0; i--)
        {
            if (!newSet.Contains(target[i]))
            {
                target.RemoveAt(i);
            }
        }

        foreach (var key in added)
        {
            target.Add(key);
        }

        return true;
    }

    /// <summary>
    /// Returns whether a display value matches the filter text: a case-insensitive "contains" match;
    /// an empty filter matches everything.
    /// </summary>
    public static bool MatchesFilter(
        string? displayValue,
        string? filterText)
    {
        if (string.IsNullOrWhiteSpace(filterText))
        {
            return true;
        }

        return (displayValue ?? string.Empty).Contains(filterText.Trim(), StringComparison.CurrentCultureIgnoreCase);
    }

    /// <summary>
    /// Returns the tri-state check value for a set of shown, enabled items: <see langword="false"/> when none is
    /// ticked (or there are none), <see langword="true"/> when all are, otherwise <see langword="null"/>.
    /// </summary>
    public static bool? GetAggregateState(
        int selectedCount,
        int totalCount)
    {
        if (totalCount == 0 || selectedCount == 0)
        {
            return false;
        }

        return selectedCount >= totalCount
            ? true
            : null;
    }

    private static void ResetTo(
        ObservableCollectionEx<string> target,
        IEnumerable<string> keys)
    {
        target.SuppressOnChangedNotification = true;
        try
        {
            target.Clear();
            foreach (var key in keys)
            {
                target.Add(key);
            }
        }
        finally
        {
            target.SuppressOnChangedNotification = false;
        }
    }
}