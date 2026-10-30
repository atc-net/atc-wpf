#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The automation peer of a <see cref="CheckTreeViewItem"/>: its child items are tree items with a three-state toggle.
/// </summary>
internal sealed class CheckTreeViewItemAutomationPeer : TreeViewItemAutomationPeer
{
    public CheckTreeViewItemAutomationPeer(CheckTreeViewItem owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    public override object GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Toggle &&
           Owner is CheckTreeViewItem { IsPlaceholder: false, Node: not null }
            ? new CheckTreeNodeToggleProvider((CheckTreeViewItem)Owner)
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        => new CheckTreeNodeAutomationPeer(item, this, EventsSource as TreeViewDataItemAutomationPeer);
}