#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A row container of a <see cref="CheckTreeView"/>: a click on the row (not on the expander) or <c>Space</c> toggles the
/// node's check state.
/// </summary>
internal sealed class CheckTreeViewItem : TreeViewItem
{
    private bool? lastIsChecked;

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
    /// Gets or sets the node shown by this row. While a node is shown, its check-state changes are raised as UI
    /// Automation toggle-state changes of the row.
    /// </summary>
    public CheckTreeNode? Node
    {
        get;
        set
        {
            if (ReferenceEquals(field, value))
            {
                return;
            }

            if (field is not null)
            {
                PropertyChangedEventManager.RemoveHandler(field, OnNodePropertyChanged, nameof(CheckTreeNode.IsChecked));
            }

            field = value;
            lastIsChecked = value?.IsChecked;
            if (value is not null)
            {
                PropertyChangedEventManager.AddHandler(value, OnNodePropertyChanged, nameof(CheckTreeNode.IsChecked));
            }
        }
    }

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

    private void OnNodePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (sender is not CheckTreeNode node || !ReferenceEquals(node, Node))
        {
            return;
        }

        var oldValue = lastIsChecked;
        lastIsChecked = node.IsChecked;
        if (IsPlaceholder ||
            oldValue == node.IsChecked ||
            !AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
        {
            return;
        }

        FindNodePeer(node)?.RaiseToggleStateChanged(oldValue, node.IsChecked);
    }

    /// <summary>
    /// Returns the data-item peer of the row's node (the one UI Automation clients see), or <see langword="null"/> when
    /// no peer exists yet.
    /// </summary>
    private CheckTreeNodeAutomationPeer? FindNodePeer(CheckTreeNode node)
    {
        if (UIElementAutomationPeer.FromElement(this)?.EventsSource is CheckTreeNodeAutomationPeer eventsSource)
        {
            return eventsSource;
        }

        return ItemsControl.ItemsControlFromItemContainer(this) is { } parent &&
               UIElementAutomationPeer.FromElement(parent) is ItemsControlAutomationPeer parentPeer
            ? parentPeer.GetChildren()?.OfType<CheckTreeNodeAutomationPeer>().FirstOrDefault(x => ReferenceEquals(x.Item, node))
            : null;
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