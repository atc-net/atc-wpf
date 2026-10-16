namespace Atc.Wpf.Components.Tests.Selectors;

public sealed class DualListSelectorReorderTests
{
    // Selected = [A, B, C]; the insert index comes from DragDropHelper.GetInsertIndex ("insert before item i").
    [Theory]
    [InlineData(0, 2, 3, 1)] // drag A onto the upper half of C: [B, A, C]
    [InlineData(0, 3, 3, 2)] // drag A below the last item: [B, C, A]
    [InlineData(2, 0, 3, 0)] // drag C onto the upper half of A: [C, A, B]
    [InlineData(2, 1, 3, 1)] // drag C onto the upper half of B: [A, C, B]
    [InlineData(1, 1, 3, 1)] // drop on its own slot: no move
    [InlineData(1, 2, 3, 1)] // drop just below itself: no move
    public void GetReorderTargetIndex_MapsTheInsertIndexToTheMoveTarget(
        int oldIndex,
        int insertIndex,
        int count,
        int expected)
        => Assert.Equal(expected, DualListSelector.GetReorderTargetIndex(oldIndex, insertIndex, count));

    [Fact]
    public void GetReorderTargetIndex_MatchesAnActualMove()
    {
        var items = new ObservableCollection<string> { "A", "B", "C" };

        // Insert index 2 means "before C", so A must end up between B and C.
        items.Move(0, DualListSelector.GetReorderTargetIndex(oldIndex: 0, insertIndex: 2, count: items.Count));

        Assert.Equal(["B", "A", "C"], items);
    }

    [Fact]
    public void GetReorderTargetIndex_EmptyList_StaysAtZero()
        => Assert.Equal(0, DualListSelector.GetReorderTargetIndex(oldIndex: 0, insertIndex: 0, count: 0));

    [StaFact]
    public void TwoSelectors_DoNotShareTheirDefaultCollections()
    {
        var first = new DualListSelector();
        var second = new DualListSelector();

        Assert.NotSame(first.AvailableItems, second.AvailableItems);
        Assert.NotSame(first.SelectedItems, second.SelectedItems);
    }
}