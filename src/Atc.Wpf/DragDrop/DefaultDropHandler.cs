namespace Atc.Wpf.DragDrop;

/// <summary>
/// Default implementation of <see cref="IDropHandler"/> that supports
/// reordering within a list, transferring between lists, and file drops.
/// </summary>
public class DefaultDropHandler : IDropHandler
{
    /// <inheritdoc />
    public virtual void DragOver(DropInfo dropInfo)
    {
        ArgumentNullException.ThrowIfNull(dropInfo);

        if (dropInfo.IsFileDrop)
        {
            dropInfo.Effects = DragDropEffects.Copy;
            return;
        }

        if (dropInfo.DragInfo is null ||
            dropInfo.TargetCollection is not System.Collections.IList)
        {
            return;
        }

        dropInfo.Effects = dropInfo.DragInfo.Effects;
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - Drop logic with reorder and transfer.")]
    public virtual void Drop(DropInfo dropInfo)
    {
        ArgumentNullException.ThrowIfNull(dropInfo);

        if (dropInfo.IsFileDrop)
        {
            HandleFileDrop(dropInfo);
            return;
        }

        if (dropInfo.DragInfo is null)
        {
            return;
        }

        var sourceCollection = dropInfo.DragInfo.SourceCollection as System.Collections.IList;
        var targetCollection = dropInfo.TargetCollection as System.Collections.IList;
        var item = dropInfo.DragInfo.SourceItem;

        if (sourceCollection is null || targetCollection is null)
        {
            return;
        }

        if (ReferenceEquals(sourceCollection, targetCollection))
        {
            HandleReorder(sourceCollection, item, dropInfo.InsertIndex);
        }
        else
        {
            HandleTransfer(sourceCollection, targetCollection, item, dropInfo.InsertIndex);
        }
    }

    private static void HandleReorder(
        System.Collections.IList collection,
        object item,
        int insertIndex)
    {
        var oldIndex = collection.IndexOf(item);
        if (oldIndex < 0)
        {
            return;
        }

        if (collection is ObservableCollection<object> observableCollection)
        {
            var targetIndex = insertIndex > oldIndex ? insertIndex - 1 : insertIndex;
            if (oldIndex != targetIndex)
            {
                observableCollection.Move(oldIndex, targetIndex);
            }

            return;
        }

        collection.RemoveAt(oldIndex);
        var adjustedIndex = insertIndex > oldIndex ? insertIndex - 1 : insertIndex;
        adjustedIndex = System.Math.Min(adjustedIndex, collection.Count);
        collection.Insert(adjustedIndex, item);
    }

    private static void HandleTransfer(
        System.Collections.IList sourceCollection,
        System.Collections.IList targetCollection,
        object item,
        int insertIndex)
    {
        sourceCollection.Remove(item);
        var safeIndex = System.Math.Min(insertIndex, targetCollection.Count);
        targetCollection.Insert(safeIndex, item);
    }

    /// <summary>
    /// Override this method to handle file drops.
    /// The default implementation does nothing.
    /// </summary>
    /// <param name="dropInfo">Drop information containing file paths.</param>
    protected virtual void HandleFileDrop(DropInfo dropInfo)
    {
        // No-op by default. Override in derived class to handle file drops.
    }
}