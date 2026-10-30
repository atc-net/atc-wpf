#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The automation peer of a node row in a <see cref="CheckTreeView"/>: a tree item with expand/collapse and a
/// three-state toggle (on, off, indeterminate) whose name is the node text.
/// </summary>
internal sealed class CheckTreeNodeAutomationPeer : TreeViewDataItemAutomationPeer, IToggleProvider
{
    private readonly ItemsControlAutomationPeer ownerPeer;

    public CheckTreeNodeAutomationPeer(
        object item,
        ItemsControlAutomationPeer itemsControlAutomationPeer,
        TreeViewDataItemAutomationPeer? parentDataItemAutomationPeer)
        : base(item, itemsControlAutomationPeer, parentDataItemAutomationPeer)
        => ownerPeer = itemsControlAutomationPeer;

    /// <inheritdoc />
    public ToggleState ToggleState
        => Node?.IsChecked switch
        {
            true => ToggleState.On,
            false => ToggleState.Off,
            _ => ToggleState.Indeterminate,
        };

    private CheckTreeNode? Node => Item as CheckTreeNode;

    private CheckTreeView? View
        => ownerPeer.Owner switch
        {
            CheckTreeViewTree tree => tree.OwnerView,
            CheckTreeViewItem treeItem => treeItem.OwnerView,
            _ => null,
        };

    /// <inheritdoc />
    public override object GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Toggle && Node is { } node && View is { } view && !view.IsPlaceholder(node)
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public void Toggle()
    {
        if (Node is not { } node || View is not { } view)
        {
            return;
        }

        if (!node.IsEnabled || !view.IsEnabled)
        {
            throw new ElementNotEnabledException();
        }

        view.ToggleNode(node);
    }

    /// <summary>
    /// Raises the toggle state change of the row.
    /// </summary>
    internal void RaiseToggleStateChanged(
        bool? oldValue,
        bool? newValue)
        => RaisePropertyChangedEvent(
            TogglePatternIdentifiers.ToggleStateProperty,
            ToToggleState(oldValue),
            ToToggleState(newValue));

    /// <inheritdoc />
    protected override string GetNameCore()
    {
        var name = base.GetNameCore();
        return string.IsNullOrEmpty(name)
            ? Node?.Text ?? string.Empty
            : name;
    }

    private static ToggleState ToToggleState(bool? value)
        => value switch
        {
            true => ToggleState.On,
            false => ToggleState.Off,
            _ => ToggleState.Indeterminate,
        };
}