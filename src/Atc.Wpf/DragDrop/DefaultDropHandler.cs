namespace Atc.Wpf.DragDrop;

/// <summary>
/// Default implementation of <see cref="IDropHandler"/> that supports
/// reordering within a list, transferring between lists, and file drops.
/// </summary>
public class DefaultDropHandler : IDropHandler
{
    private static readonly ConcurrentDictionary<Type, MethodInfo?> ObservableCollectionMoveMethods = new();

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
            dropInfo.TargetCollection is not IList targetCollection)
        {
            return;
        }

        if (!CanMoveItem(
                dropInfo.DragInfo.SourceCollection as IList,
                targetCollection,
                dropInfo.DragInfo.SourceItem))
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

        var sourceCollection = dropInfo.DragInfo.SourceCollection as IList;
        var targetCollection = dropInfo.TargetCollection as IList;
        var item = dropInfo.DragInfo.SourceItem;

        if (sourceCollection is null ||
            targetCollection is null ||
            !CanMoveItem(sourceCollection, targetCollection, item))
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

    /// <summary>
    /// Determines whether <paramref name="item"/> can be removed from <paramref name="sourceCollection"/>
    /// and inserted into <paramref name="targetCollection"/> without the list operations throwing:
    /// both lists must be mutable and not fixed-size, and the item must be compatible
    /// with the element type of a generic target list.
    /// </summary>
    internal static bool CanMoveItem(
        IList? sourceCollection,
        IList targetCollection,
        object item)
    {
        ArgumentNullException.ThrowIfNull(targetCollection);
        ArgumentNullException.ThrowIfNull(item);

        if (sourceCollection is null ||
            sourceCollection.IsReadOnly ||
            sourceCollection.IsFixedSize ||
            targetCollection.IsReadOnly ||
            targetCollection.IsFixedSize)
        {
            return false;
        }

        if (ReferenceEquals(sourceCollection, targetCollection))
        {
            return true;
        }

        var elementType = GetElementType(targetCollection.GetType());
        return elementType is null || elementType.IsInstanceOfType(item);
    }

    private static Type? GetElementType(Type collectionType)
    {
        var listInterface = FindGenericInterface(collectionType, typeof(IList<>))
                            ?? FindGenericInterface(collectionType, typeof(ICollection<>));

        return listInterface?.GetGenericArguments()[0];
    }

    private static Type? FindGenericInterface(
        Type type,
        Type openGenericInterface)
    {
        if (type.IsInterface &&
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == openGenericInterface)
        {
            return type;
        }

        return Array.Find(
            type.GetInterfaces(),
            x => x.IsGenericType && x.GetGenericTypeDefinition() == openGenericInterface);
    }

    private static void HandleReorder(
        IList collection,
        object item,
        int insertIndex)
    {
        var oldIndex = collection.IndexOf(item);
        if (oldIndex < 0)
        {
            return;
        }

        var targetIndex = insertIndex > oldIndex ? insertIndex - 1 : insertIndex;
        targetIndex = System.Math.Clamp(targetIndex, 0, collection.Count - 1);
        if (oldIndex == targetIndex)
        {
            return;
        }

        if (TryMoveInObservableCollection(collection, oldIndex, targetIndex))
        {
            return;
        }

        collection.RemoveAt(oldIndex);
        collection.Insert(targetIndex, item);
    }

    private static bool TryMoveInObservableCollection(
        IList collection,
        int oldIndex,
        int newIndex)
    {
        var moveMethod = ObservableCollectionMoveMethods.GetOrAdd(
            collection.GetType(),
            FindObservableCollectionMoveMethod);

        if (moveMethod is null)
        {
            return false;
        }

        moveMethod.Invoke(collection, [oldIndex, newIndex]);
        return true;
    }

    private static MethodInfo? FindObservableCollectionMoveMethod(
        Type collectionType)
    {
        for (var type = collectionType; type is not null; type = type.BaseType)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(ObservableCollection<>))
            {
                return type.GetMethod(
                    nameof(ObservableCollection<object>.Move),
                    BindingFlags.Public | BindingFlags.Instance,
                    [typeof(int), typeof(int)]);
            }
        }

        return null;
    }

    private static void HandleTransfer(
        IList sourceCollection,
        IList targetCollection,
        object item,
        int insertIndex)
    {
        sourceCollection.Remove(item);
        var safeIndex = System.Math.Clamp(insertIndex, 0, targetCollection.Count);
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