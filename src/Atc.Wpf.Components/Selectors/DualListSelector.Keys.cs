namespace Atc.Wpf.Components.Selectors;

/// <summary>
/// The <see cref="SelectedKeys"/> and <see cref="MaxSelectionCount"/> of a <see cref="DualListSelector"/>, named like
/// the other multi-select controls.
/// </summary>
public sealed partial class DualListSelector
{
    private ObservableCollection<string>? subscribedSelectedKeys;
    private bool isSyncingMaxCount;
    private bool isWritingKeys;
    private bool isApplyingKeys;
    private bool areKeysDirty;
    private bool isKeySyncPending;

    private static void OnMaxSelectedCountChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        OnStateChanged(d, e);
        ((DualListSelector)d).SyncMaxCount(MaxSelectionCountProperty, e.NewValue);
    }

    private static void OnMaxSelectionCountChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((DualListSelector)d).SyncMaxCount(MaxSelectedCountProperty, e.NewValue);

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var selector = (DualListSelector)d;
        if (selector.subscribedSelectedKeys is not null)
        {
            CollectionChangedEventManager.RemoveHandler(selector.subscribedSelectedKeys, selector.OnSelectedKeysCollectionChanged);
        }

        selector.subscribedSelectedKeys = e.NewValue as ObservableCollection<string>;
        if (selector.subscribedSelectedKeys is not null)
        {
            CollectionChangedEventManager.AddHandler(selector.subscribedSelectedKeys, selector.OnSelectedKeysCollectionChanged);
        }

        if (!selector.isWritingKeys)
        {
            selector.areKeysDirty = true;
            selector.ScheduleKeySync();
        }
    }

    private void SyncMaxCount(
        DependencyProperty other,
        object? value)
    {
        if (isSyncingMaxCount)
        {
            return;
        }

        isSyncingMaxCount = true;
        try
        {
            SetCurrentValue(other, value);
        }
        finally
        {
            isSyncingMaxCount = false;
        }
    }

    private void OnSelectedKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (isWritingKeys)
        {
            return;
        }

        areKeysDirty = true;
        ScheduleKeySync();
    }

    /// <summary>
    /// The selected list changed in place (a transfer, reorder or removal): write the keys now, unless a key change
    /// is still waiting to be applied.
    /// </summary>
    private void OnSelectedListChanged()
    {
        if (isApplyingKeys)
        {
            return;
        }

        if (areKeysDirty)
        {
            ScheduleKeySync();
            return;
        }

        WriteKeys();
    }

    /// <summary>
    /// Syncs the keys and the lists once the bindings of this pass have settled.
    /// </summary>
    private void ScheduleKeySync()
    {
        if (isKeySyncPending)
        {
            return;
        }

        isKeySyncPending = true;
        _ = Dispatcher.BeginInvoke(
            DispatcherPriority.DataBind,
            () =>
            {
                isKeySyncPending = false;
                if (areKeysDirty)
                {
                    areKeysDirty = false;
                    ApplyKeys();
                }

                WriteKeys();
            });
    }

    /// <summary>
    /// Moves the items with the keys to the selected list (in key order, after the ones already there) and the other
    /// keyed, unlocked items back.
    /// </summary>
    private void ApplyKeys()
    {
        var keys = SelectedKeys?.ToList() ?? [];
        var keySet = new HashSet<string>(keys, StringComparer.Ordinal);
        var available = AvailableItems;
        var selected = SelectedItems;
        if (available is null || selected is null)
        {
            return;
        }

        var toRelease = selected
            .Where(i => i.Identifier is not null && !keySet.Contains(i.Identifier) && CanLeaveSelected(i))
            .ToList();
        var byKey = available
            .Where(i => i.Identifier is not null)
            .GroupBy(i => i.Identifier!, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);
        var toSelect = keys
            .Distinct(StringComparer.Ordinal)
            .Select(k => byKey.GetValueOrDefault(k))
            .OfType<DualListSelectorItem>()
            .ToList();

        if (toRelease.Count == 0 && toSelect.Count == 0)
        {
            return;
        }

        isApplyingKeys = true;
        try
        {
            if (toRelease.Count > 0)
            {
                Transfer(toRelease, selectedListBox, selected, available, insertIndex: -1, clearFilters: false);
            }

            if (toSelect.Count > 0)
            {
                Transfer(toSelect, availableListBox, available, selected, insertIndex: -1, clearFilters: false);
            }
        }
        finally
        {
            isApplyingKeys = false;
        }
    }

    /// <summary>
    /// Writes the identifiers of the selected list to <see cref="SelectedKeys"/> in place, raising
    /// <see cref="SelectedKeysChanged"/> when they changed.
    /// </summary>
    private void WriteKeys()
    {
        var newKeys = SelectedItems?
            .Select(i => i.Identifier)
            .OfType<string>()
            .ToList() ?? [];
        var target = SelectedKeys;
        if (target is null && newKeys.Count == 0)
        {
            return;
        }

        if (target is not null && target.SequenceEqual(newKeys, StringComparer.Ordinal))
        {
            return;
        }

        var oldKeys = target?.ToList() ?? [];
        isWritingKeys = true;
        try
        {
            if (target is null)
            {
                SetCurrentValue(SelectedKeysProperty, new ObservableCollection<string>(newKeys));
            }
            else
            {
                target.Clear();
                foreach (var key in newKeys)
                {
                    target.Add(key);
                }
            }
        }
        finally
        {
            isWritingKeys = false;
        }

        SelectedKeysChanged?.Invoke(
            this,
            new ValueChangedEventArgs<IReadOnlyList<string>>(Name, oldKeys, newKeys));
    }
}