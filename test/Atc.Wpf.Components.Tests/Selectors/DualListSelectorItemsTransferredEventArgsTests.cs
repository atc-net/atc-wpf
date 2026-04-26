namespace Atc.Wpf.Components.Tests.Selectors;

public sealed class DualListSelectorItemsTransferredEventArgsTests
{
    [Theory]
    [InlineData(DualListSelectorTransferDirection.ToSelected)]
    [InlineData(DualListSelectorTransferDirection.ToAvailable)]
    public void Constructor_exposes_items_and_direction_as_provided(
        DualListSelectorTransferDirection direction)
    {
        var transferred = new[]
        {
            new DualListSelectorItem { Name = "a" },
            new DualListSelectorItem { Name = "b" },
        };

        var args = new DualListSelectorItemsTransferredEventArgs(transferred, direction);

        Assert.Same(transferred, args.TransferredItems);
        Assert.Equal(2, args.TransferredItems.Count);
        Assert.Equal(direction, args.Direction);
    }

    [Fact]
    public void Empty_TransferredItems_is_allowed()
    {
        var args = new DualListSelectorItemsTransferredEventArgs(
            Array.Empty<DualListSelectorItem>(),
            DualListSelectorTransferDirection.ToSelected);

        Assert.Empty(args.TransferredItems);
    }
}