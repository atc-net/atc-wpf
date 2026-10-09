namespace Atc.Wpf.Components.Selectors;

/// <summary>
/// Provides data for the <see cref="DualListSelector.ItemsTransferred"/> event.
/// </summary>
public sealed class DualListSelectorItemsTransferredEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DualListSelectorItemsTransferredEventArgs"/> class.
    /// </summary>
    /// <param name="transferredItems">The items that were transferred.</param>
    /// <param name="direction">The direction in which the items were transferred.</param>
    public DualListSelectorItemsTransferredEventArgs(
        IReadOnlyList<DualListSelectorItem> transferredItems,
        DualListSelectorTransferDirection direction)
    {
        TransferredItems = transferredItems;
        Direction = direction;
    }

    /// <summary>
    /// Gets the items that were transferred.
    /// </summary>
    public IReadOnlyList<DualListSelectorItem> TransferredItems { get; }

    /// <summary>
    /// Gets the direction of the transfer.
    /// </summary>
    public DualListSelectorTransferDirection Direction { get; }
}