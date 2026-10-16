namespace Atc.Wpf.Tests.DragDrop;

public sealed class DefaultDropHandlerTests
{
    [StaFact]
    public void DragOver_CompatibleObservableCollections_GrantsDragEffects()
    {
        // Arrange
        var source = new ObservableCollection<string> { "a", "b" };
        var target = new ObservableCollection<string> { "c" };
        var dropInfo = CreateDropInfo(source, "a", target, insertIndex: 0);

        // Act
        new DefaultDropHandler().DragOver(dropInfo);

        // Assert
        Assert.Equal(DragDropEffects.Move, dropInfo.Effects);
    }

    [StaFact]
    public void DragOver_NonGenericTarget_GrantsDragEffects()
    {
        // Arrange
        var source = new ObservableCollection<string> { "a" };
        var target = new System.Collections.ArrayList { 1, "x" };
        var dropInfo = CreateDropInfo(source, "a", target, insertIndex: 0);

        // Act
        new DefaultDropHandler().DragOver(dropInfo);

        // Assert
        Assert.Equal(DragDropEffects.Move, dropInfo.Effects);
    }

    [StaFact]
    public void DragOver_FixedSizeArray_DeniesDrop()
    {
        // Arrange
        var items = new[] { "a", "b", "c" };
        var dropInfo = CreateDropInfo(items, "a", items, insertIndex: 2);

        // Act
        new DefaultDropHandler().DragOver(dropInfo);

        // Assert
        Assert.Equal(DragDropEffects.None, dropInfo.Effects);
    }

    [StaFact]
    public void DragOver_ReadOnlyTarget_DeniesDrop()
    {
        // Arrange
        var source = new ObservableCollection<string> { "a" };
        var target = new ReadOnlyObservableCollection<string>(new ObservableCollection<string> { "b" });
        var dropInfo = CreateDropInfo(source, "a", target, insertIndex: 0);

        // Act
        new DefaultDropHandler().DragOver(dropInfo);

        // Assert
        Assert.Equal(DragDropEffects.None, dropInfo.Effects);
    }

    [StaFact]
    public void DragOver_ReadOnlySource_DeniesDrop()
    {
        // Arrange
        var source = new ReadOnlyCollection<string>(["a"]);
        var target = new ObservableCollection<string> { "b" };
        var dropInfo = CreateDropInfo(source, "a", target, insertIndex: 0);

        // Act
        new DefaultDropHandler().DragOver(dropInfo);

        // Assert
        Assert.Equal(DragDropEffects.None, dropInfo.Effects);
    }

    [StaFact]
    public void DragOver_IncompatibleItemType_DeniesDrop()
    {
        // Arrange
        var source = new ObservableCollection<FooItem> { new() };
        var target = new ObservableCollection<BarItem> { new() };
        var dropInfo = CreateDropInfo(source, source[0], target, insertIndex: 0);

        // Act
        new DefaultDropHandler().DragOver(dropInfo);

        // Assert
        Assert.Equal(DragDropEffects.None, dropInfo.Effects);
    }

    [StaFact]
    public void DragOver_DerivedItemIntoBaseTypedTarget_GrantsDragEffects()
    {
        // Arrange
        var source = new ObservableCollection<DerivedFooItem> { new() };
        var target = new ObservableCollection<FooItem> { new() };
        var dropInfo = CreateDropInfo(source, source[0], target, insertIndex: 1);

        // Act
        new DefaultDropHandler().DragOver(dropInfo);

        // Assert
        Assert.Equal(DragDropEffects.Move, dropInfo.Effects);
    }

    [StaFact]
    public void Drop_IncompatibleItemType_DoesNotThrowOrChangeCollections()
    {
        // Arrange
        var source = new ObservableCollection<FooItem> { new() };
        var target = new ObservableCollection<BarItem> { new() };
        var dropInfo = CreateDropInfo(source, source[0], target, insertIndex: 0);

        // Act
        new DefaultDropHandler().Drop(dropInfo);

        // Assert
        Assert.Single(source);
        Assert.Single(target);
    }

    [StaFact]
    public void Drop_ReorderTypedObservableCollection_UsesMove()
    {
        // Arrange
        var items = new ObservableCollection<string> { "a", "b", "c" };
        var actions = new List<NotifyCollectionChangedAction>();
        items.CollectionChanged += (_, e) => actions.Add(e.Action);
        var dropInfo = CreateDropInfo(items, "a", items, insertIndex: 3);

        // Act
        new DefaultDropHandler().Drop(dropInfo);

        // Assert
        Assert.Equal(["b", "c", "a"], items, StringComparer.Ordinal);
        Assert.Equal([NotifyCollectionChangedAction.Move], actions);
    }

    [StaFact]
    public void Drop_ReorderTowardsStart_UsesMove()
    {
        // Arrange
        var items = new ObservableCollection<string> { "a", "b", "c" };
        var actions = new List<NotifyCollectionChangedAction>();
        items.CollectionChanged += (_, e) => actions.Add(e.Action);
        var dropInfo = CreateDropInfo(items, "c", items, insertIndex: 0);

        // Act
        new DefaultDropHandler().Drop(dropInfo);

        // Assert
        Assert.Equal(["c", "a", "b"], items, StringComparer.Ordinal);
        Assert.Equal([NotifyCollectionChangedAction.Move], actions);
    }

    [StaFact]
    public void Drop_ReorderToSamePosition_DoesNotRaiseEvents()
    {
        // Arrange
        var items = new ObservableCollection<string> { "a", "b", "c" };
        var actions = new List<NotifyCollectionChangedAction>();
        items.CollectionChanged += (_, e) => actions.Add(e.Action);
        var dropInfo = CreateDropInfo(items, "b", items, insertIndex: 2);

        // Act
        new DefaultDropHandler().Drop(dropInfo);

        // Assert
        Assert.Equal(["a", "b", "c"], items, StringComparer.Ordinal);
        Assert.Empty(actions);
    }

    [StaFact]
    public void Drop_ReorderPlainList_RemovesAndInserts()
    {
        // Arrange
        var items = new List<string> { "a", "b", "c" };
        var dropInfo = CreateDropInfo(items, "a", items, insertIndex: 2);

        // Act
        new DefaultDropHandler().Drop(dropInfo);

        // Assert
        Assert.Equal(["b", "a", "c"], items, StringComparer.Ordinal);
    }

    [StaFact]
    public void Drop_Transfer_MovesItemBetweenCollections()
    {
        // Arrange
        var source = new ObservableCollection<string> { "a", "b" };
        var target = new ObservableCollection<string> { "c", "d" };
        var dropInfo = CreateDropInfo(source, "b", target, insertIndex: 1);

        // Act
        new DefaultDropHandler().Drop(dropInfo);

        // Assert
        Assert.Equal(["a"], source, StringComparer.Ordinal);
        Assert.Equal(["c", "b", "d"], target, StringComparer.Ordinal);
    }

    private static DropInfo CreateDropInfo(
        System.Collections.IEnumerable sourceCollection,
        object item,
        System.Collections.IEnumerable targetCollection,
        int insertIndex)
    {
        var sourceControl = new ListBox { ItemsSource = sourceCollection };
        var targetControl = ReferenceEquals(sourceCollection, targetCollection)
            ? sourceControl
            : new ListBox { ItemsSource = targetCollection };

        var dragInfo = new DragInfo
        {
            SourceItem = item,
            SourceCollection = sourceCollection,
            VisualSource = sourceControl,
            SourceIndex = 0,
        };

        return new DropInfo
        {
            Data = item,
            TargetCollection = targetCollection,
            VisualTarget = targetControl,
            InsertIndex = insertIndex,
            DragInfo = dragInfo,
        };
    }

    private class FooItem;

    private sealed class DerivedFooItem : FooItem;

    private sealed class BarItem;
}