#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Runs the stale-key cleanup of a multi-select control after its items changed.
/// </summary>
/// <remarks>
/// When <c>SelectedKeys</c> is data-bound the cleanup is posted once at <see cref="DispatcherPriority.DataBind"/>
/// instead of running at once. A DataContext swap (or a wrapper that forwards its own bindings) can update the
/// items before <c>SelectedKeys</c>; running at once would then drop or add keys in the previous view model's
/// collection. Posted, it runs on whatever <c>SelectedKeys</c> the bindings have settled on. Several item changes
/// before it runs are coalesced: the item keys from before the first change are kept.
/// <para>
/// The <c>SelectedKeys</c> instance that belonged to the old items is recorded as well. When the instance differs by the
/// time the cleanup runs (a DataContext swap, in either binding order, or a chain of swaps), the new keys were never
/// shown against the old items, so "tick new items" is skipped: only the keys of items that disappeared are dropped.
/// A posted cleanup does not run while the control is unloaded: it is cancelled on <c>Unloaded</c> and posted again on
/// the next <c>Loaded</c>, so it runs against the bindings of the reloaded control.
/// </para>
/// </remarks>
internal sealed class SelectedKeysCleanup
{
    private readonly DependencyObject owner;
    private readonly DependencyProperty selectedKeysProperty;
    private readonly Action<IReadOnlySet<string>, bool> cleanup;
    private IReadOnlySet<string>? pendingOldItemKeys;
    private object? pendingSelection;
    private DispatcherOperation? operation;
    private DispatcherOperation? baselineOperation;
    private bool hasBaseline;
    private bool isUnloaded;
    private object? baselineSelection;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectedKeysCleanup"/> class.
    /// </summary>
    /// <param name="owner">The control.</param>
    /// <param name="selectedKeysProperty">The control's <c>SelectedKeys</c> property.</param>
    /// <param name="cleanup">
    /// Cleans the current selection, given the item keys from before the change and whether new items may be ticked
    /// (<see langword="false"/> when <c>SelectedKeys</c> was replaced since the old items were shown).
    /// </param>
    public SelectedKeysCleanup(
        DependencyObject owner,
        DependencyProperty selectedKeysProperty,
        Action<IReadOnlySet<string>, bool> cleanup)
    {
        this.owner = owner;
        this.selectedKeysProperty = selectedKeysProperty;
        this.cleanup = cleanup;
        if (owner is FrameworkElement element)
        {
            element.Unloaded += (_, _) => OnUnloaded();
            element.Loaded += (_, _) => OnLoaded();
        }
    }

    /// <summary>
    /// Returns whether a property gets its value from a binding to something other than an unbound property of
    /// another element: a control that only forwards its own (unbound) property to an inner control still cleans up
    /// at once.
    /// </summary>
    private static bool IsBoundToOutsideSource(
        DependencyObject target,
        DependencyProperty property,
        int depth)
    {
        var expression = BindingOperations.GetBindingExpressionBase(target, property);
        if (expression is null)
        {
            return false;
        }

        var binding = expression as BindingExpression;
        if (depth < 8 &&
            binding?.ResolvedSource is DependencyObject source &&
            binding.ResolvedSourcePropertyName is { } name &&
            DependencyPropertyDescriptor.FromName(name, source.GetType(), source.GetType()) is { } descriptor)
        {
            return IsBoundToOutsideSource(source, descriptor.DependencyProperty, depth + 1);
        }

        return true;
    }

    /// <summary>
    /// Gets a value indicating whether a cleanup is waiting for the bindings to settle.
    /// </summary>
    public bool IsPending => pendingOldItemKeys is not null;

    /// <summary>
    /// Runs the cleanup now, or posts it when <c>SelectedKeys</c> is data-bound (or a cleanup is already waiting).
    /// </summary>
    /// <param name="oldItemKeys">A snapshot of the item keys from before the change.</param>
    public void Request(IReadOnlySet<string> oldItemKeys)
    {
        ArgumentNullException.ThrowIfNull(oldItemKeys);

        if (pendingOldItemKeys is null && !IsBoundToOutsideSource(owner, selectedKeysProperty, depth: 0))
        {
            cleanup(oldItemKeys, true);
            SetBaseline(owner.GetValue(selectedKeysProperty));
            return;
        }

        if (pendingOldItemKeys is null)
        {
            pendingOldItemKeys = oldItemKeys;
            pendingSelection = hasBaseline ? baselineSelection : owner.GetValue(selectedKeysProperty);
        }

        if (!isUnloaded)
        {
            operation ??= owner.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, Run);
        }
    }

    /// <summary>
    /// Tells the cleanup that <c>SelectedKeys</c> was replaced from outside. The new instance becomes the one that
    /// belongs to the current items after the bindings have settled, so an items change in the same binding pass
    /// (a DataContext swap that updates <c>SelectedKeys</c> first) still counts as a swap.
    /// </summary>
    public void NotifySelectedKeysReplaced()
    {
        if (!hasBaseline || baselineOperation is not null)
        {
            return;
        }

        baselineOperation = owner.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            () =>
            {
                baselineOperation = null;
                SetBaseline(owner.GetValue(selectedKeysProperty));
            });
    }

    private void SetBaseline(object? selection)
    {
        hasBaseline = true;
        baselineSelection = selection;
    }

    private void OnUnloaded()
    {
        isUnloaded = true;
        operation?.Abort();
        operation = null;
    }

    private void OnLoaded()
    {
        isUnloaded = false;
        if (pendingOldItemKeys is not null)
        {
            operation ??= owner.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, Run);
        }
    }

    private void Run()
    {
        operation = null;
        if (isUnloaded)
        {
            // Kept pending: posted again when the control is loaded.
            return;
        }

        var oldItemKeys = pendingOldItemKeys;
        var selection = pendingSelection;
        pendingOldItemKeys = null;
        pendingSelection = null;
        if (oldItemKeys is null)
        {
            return;
        }

        // With no old items there is nothing to swap from: the first items may be ticked into whatever selection is bound.
        var current = owner.GetValue(selectedKeysProperty);
        cleanup(oldItemKeys, oldItemKeys.Count == 0 || ReferenceEquals(selection, current));
        SetBaseline(owner.GetValue(selectedKeysProperty));
    }
}