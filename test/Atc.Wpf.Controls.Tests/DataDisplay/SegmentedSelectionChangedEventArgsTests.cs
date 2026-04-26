namespace Atc.Wpf.Controls.Tests.DataDisplay;

public sealed class SegmentedSelectionChangedEventArgsTests
{
    [Fact]
    public void Constructor_exposes_indices_and_items_with_null_items_allowed()
    {
        var args = new SegmentedSelectionChangedEventArgs(
            oldIndex: -1,
            newIndex: 0,
            oldItem: null,
            newItem: null);

        Assert.Equal(-1, args.OldIndex);
        Assert.Equal(0, args.NewIndex);
        Assert.Null(args.OldItem);
        Assert.Null(args.NewItem);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(3, -1)]
    public void Indices_are_independent_per_instance(
        int oldIndex,
        int newIndex)
    {
        var args = new SegmentedSelectionChangedEventArgs(
            oldIndex,
            newIndex,
            oldItem: null,
            newItem: null);

        Assert.Equal(oldIndex, args.OldIndex);
        Assert.Equal(newIndex, args.NewIndex);
    }
}