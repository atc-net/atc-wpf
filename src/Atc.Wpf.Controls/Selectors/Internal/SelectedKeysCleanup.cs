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
/// </remarks>
internal sealed class SelectedKeysCleanup
{
    private readonly DependencyObject owner;
    private readonly DependencyProperty selectedKeysProperty;
    private readonly Action<IReadOnlySet<string>> cleanup;
    private IReadOnlySet<string>? pendingOldItemKeys;
    private DispatcherOperation? operation;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectedKeysCleanup"/> class.
    /// </summary>
    /// <param name="owner">The control.</param>
    /// <param name="selectedKeysProperty">The control's <c>SelectedKeys</c> property.</param>
    /// <param name="cleanup">Cleans the current selection, given the item keys from before the change.</param>
    public SelectedKeysCleanup(
        DependencyObject owner,
        DependencyProperty selectedKeysProperty,
        Action<IReadOnlySet<string>> cleanup)
    {
        this.owner = owner;
        this.selectedKeysProperty = selectedKeysProperty;
        this.cleanup = cleanup;
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
    public bool IsPending => operation is not null;

    /// <summary>
    /// Runs the cleanup now, or posts it when <c>SelectedKeys</c> is data-bound (or a cleanup is already waiting).
    /// </summary>
    /// <param name="oldItemKeys">A snapshot of the item keys from before the change.</param>
    public void Request(IReadOnlySet<string> oldItemKeys)
    {
        ArgumentNullException.ThrowIfNull(oldItemKeys);

        if (operation is null && !IsBoundToOutsideSource(owner, selectedKeysProperty, depth: 0))
        {
            cleanup(oldItemKeys);
            return;
        }

        pendingOldItemKeys ??= oldItemKeys;
        operation ??= owner.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, Run);
    }

    private void Run()
    {
        var oldItemKeys = pendingOldItemKeys;
        pendingOldItemKeys = null;
        operation = null;
        if (oldItemKeys is not null)
        {
            cleanup(oldItemKeys);
        }
    }
}