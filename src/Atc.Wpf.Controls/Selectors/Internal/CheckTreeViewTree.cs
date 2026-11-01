#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The virtualized <see cref="TreeView"/> inside a <see cref="CheckTreeView"/>. Its containers are
/// <see cref="CheckTreeViewItem"/>s; the owner prepares them (header, shown children, expansion).
/// </summary>
internal sealed class CheckTreeViewTree : TreeView
{
    private static readonly ItemsPanelTemplate VirtualizingPanelTemplate = CreateVirtualizingPanelTemplate();

    public CheckTreeViewTree(CheckTreeView owner)
    {
        OwnerView = owner;
        SetResourceReference(StyleProperty, typeof(TreeView));
        ApplyVirtualization(this);
    }

    /// <summary>
    /// Gets the check tree that owns this tree.
    /// </summary>
    public CheckTreeView OwnerView { get; }

    /// <summary>
    /// Turns on virtualization with recycling for a tree or a tree item.
    /// </summary>
    public static void ApplyVirtualization(ItemsControl itemsControl)
    {
        ArgumentNullException.ThrowIfNull(itemsControl);

        VirtualizingPanel.SetIsVirtualizing(itemsControl, value: true);
        VirtualizingPanel.SetVirtualizationMode(itemsControl, VirtualizationMode.Recycling);
        itemsControl.SetCurrentValue(ItemsPanelProperty, VirtualizingPanelTemplate);
    }

    /// <inheritdoc />
    protected override DependencyObject GetContainerForItemOverride()
        => new CheckTreeViewItem(OwnerView);

    /// <inheritdoc />
    protected override bool IsItemItsOwnContainerOverride(object item)
        => item is CheckTreeViewItem;

    /// <inheritdoc />
    protected override void PrepareContainerForItemOverride(
        DependencyObject element,
        object item)
    {
        base.PrepareContainerForItemOverride(element, item);
        if (element is CheckTreeViewItem container && item is CheckTreeNode node)
        {
            OwnerView.PrepareContainer(container, node);
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new CheckTreeViewTreeAutomationPeer(this);

    private static ItemsPanelTemplate CreateVirtualizingPanelTemplate()
    {
        var template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(VirtualizingStackPanel)));
        template.Seal();
        return template;
    }
}