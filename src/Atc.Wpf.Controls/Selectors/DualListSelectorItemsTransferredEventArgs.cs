namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Provides data for the <see cref="DualListSelector.ItemsTransferred"/> event.
/// </summary>
public sealed class DualListSelectorItemsTransferredEventArgs : EventArgs
{
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