namespace Atc.Wpf.Components.Tests.Selectors;

public sealed class DualListSelectorItemsReorderedEventArgsTests
{
    [Fact]
    public void Constructor_exposes_item_oldIndex_and_newIndex_as_provided()
    {
        var item = new DualListSelectorItem { Name = "moved" };

        var args = new DualListSelectorItemsReorderedEventArgs(item, oldIndex: 1, newIndex: 4);

        Assert.Same(item, args.Item);
        Assert.Equal(1, args.OldIndex);
        Assert.Equal(4, args.NewIndex);
    }

    [Fact]
    public void Indices_are_independent_per_instance()
    {
        var item = new DualListSelectorItem { Name = "x" };

        var first = new DualListSelectorItemsReorderedEventArgs(item, oldIndex: 0, newIndex: 1);
        var second = new DualListSelectorItemsReorderedEventArgs(item, oldIndex: 5, newIndex: 2);

        Assert.Equal((0, 1), (first.OldIndex, first.NewIndex));
        Assert.Equal((5, 2), (second.OldIndex, second.NewIndex));
    }
}