#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The automation peer of the tree inside a <see cref="CheckTreeView"/>: its items are tree items with a three-state
/// toggle.
/// </summary>
internal sealed class CheckTreeViewTreeAutomationPeer : TreeViewAutomationPeer
{
    public CheckTreeViewTreeAutomationPeer(CheckTreeViewTree owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        => new CheckTreeNodeAutomationPeer(item, this, parentDataItemAutomationPeer: null);

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(CheckTreeView);
}