#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A row container of a <see cref="CheckTreeView"/>: a click on the row (not on the expander) or <c>Space</c> toggles the
/// node's check state.
/// </summary>
internal sealed class CheckTreeViewItem : TreeViewItem
{
    public CheckTreeViewItem(CheckTreeView owner)
    {
        OwnerView = owner;
        SetResourceReference(StyleProperty, typeof(TreeViewItem));
        CheckTreeViewTree.ApplyVirtualization(this);
    }

    /// <summary>
    /// Gets the check tree that owns this row.
    /// </summary>
    public CheckTreeView OwnerView { get; }

    /// <summary>
    /// Gets or sets the node shown by this row.
    /// </summary>
    public CheckTreeNode? Node { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this row is the "please wait" row of a node whose children are loading.
    /// </summary>
    public bool IsPlaceholder { get; set; }

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
        => new CheckTreeViewItemAutomationPeer(this);

    /// <inheritdoc />
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        var isRowClick = IsFromOwnRow(e.OriginalSource as DependencyObject);
        base.OnMouseLeftButtonDown(e);
        if (isRowClick && e.ClickCount == 1 && !IsPlaceholder && Node is { } node)
        {
            OwnerView.ToggleNode(node);
            e.Handled = true;
        }
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e.Key == Key.Space &&
            Keyboard.Modifiers == ModifierKeys.None &&
            !IsPlaceholder &&
            Node is { } node &&
            ReferenceEquals(e.OriginalSource, this))
        {
            OwnerView.ToggleNode(node);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Multiply &&
            !IsPlaceholder &&
            Node is { } expandNode &&
            ReferenceEquals(e.OriginalSource, this))
        {
            OwnerView.ExpandSubtree(expandNode);
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }

    private bool IsFromOwnRow(DependencyObject? source)
    {
        // Anywhere on the row counts, except the expander arrow and the rows of the children.
        while (source is not null)
        {
            if (ReferenceEquals(source, this))
            {
                return true;
            }

            if (source is CheckTreeViewItem or ItemsPresenter or (ToggleButton and not CheckTreeCheckBox))
            {
                return false;
            }

            source = source is Visual or System.Windows.Media.Media3D.Visual3D
                ? VisualTreeHelper.GetParent(source)
                : LogicalTreeHelper.GetParent(source);
        }

        return false;
    }
}