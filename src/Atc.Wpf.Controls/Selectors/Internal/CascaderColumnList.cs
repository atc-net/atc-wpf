#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A column of a <see cref="CascaderColumnView"/> (or its flat list of filter results): a virtualized list of
/// <see cref="CascaderColumnItem"/> rows. Activation, ticks and keyboard moves are handled by the owner, not by the list.
/// </summary>
internal sealed class CascaderColumnList : ItemsControl
{
    public CascaderColumnList(
        CascaderColumnView owner,
        int columnIndex)
    {
        OwnerView = owner;
        ColumnIndex = columnIndex;
        Focusable = false;
        IsTabStop = false;
        CheckTreeViewTree.ApplyVirtualization(this);
        VirtualizingPanel.SetScrollUnit(this, ScrollUnit.Item);
        ScrollViewer.SetCanContentScroll(this, true);
    }

    /// <summary>
    /// Gets the column view that owns this list.
    /// </summary>
    public CascaderColumnView OwnerView { get; }

    /// <summary>
    /// Gets the index of the column (0 for the roots); -1 for the list of filter results.
    /// </summary>
    public int ColumnIndex { get; }

    /// <summary>
    /// Gets a value indicating whether this is the list of filter results.
    /// </summary>
    public bool IsFilterResults => ColumnIndex < 0;

    /// <summary>
    /// Brings a row into view and returns its container (or <see langword="null"/> when it cannot be realized).
    /// </summary>
    public CascaderColumnItem? RealizeContainer(int index)
    {
        if (index < 0 || index >= Items.Count)
        {
            return null;
        }

        if (ItemContainerGenerator.ContainerFromIndex(index) is CascaderColumnItem existing)
        {
            existing.BringIntoView();
            return existing;
        }

        ApplyTemplate();
        UpdateLayout();
        if (FindItemsHost(this) is VirtualizingStackPanel panel)
        {
            panel.BringIndexIntoViewPublic(index);
        }

        return ItemContainerGenerator.ContainerFromIndex(index) as CascaderColumnItem;
    }

    /// <inheritdoc />
    protected override DependencyObject GetContainerForItemOverride()
        => new CascaderColumnItem(this);

    /// <inheritdoc />
    protected override bool IsItemItsOwnContainerOverride(object item)
        => item is CascaderColumnItem;

    /// <inheritdoc />
    protected override void PrepareContainerForItemOverride(
        DependencyObject element,
        object item)
    {
        base.PrepareContainerForItemOverride(element, item);
        if (element is CascaderColumnItem container && item is CheckTreeNode node)
        {
            OwnerView.PrepareContainer(this, container, node);
        }
    }

    /// <inheritdoc />
    protected override void ClearContainerForItemOverride(
        DependencyObject element,
        object item)
    {
        if (element is CascaderColumnItem container)
        {
            OwnerView.ClearContainer(container);
            container.Node = null;
            container.IsPlaceholder = false;
            container.IsActive = false;
            container.FilterContent = null;
        }

        base.ClearContainerForItemOverride(element, item);
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new CascaderColumnListAutomationPeer(this);

    private static Panel? FindItemsHost(DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is Panel { IsItemsHost: true } panel)
            {
                return panel;
            }

            if (FindItemsHost(child) is { } found)
            {
                return found;
            }
        }

        return null;
    }
}