namespace Atc.Wpf.Sample.SamplesWpfControls.DragDrop;

public class DragDropViewModel : ViewModelBase, IDropHandler
{
    public ObservableCollection<string> ReorderItems { get; } =
    [
        "Alpha",
        "Bravo",
        "Charlie",
        "Delta",
        "Echo",
        "Foxtrot",
    ];

    public ObservableCollection<string> SourceItems { get; } =
    [
        "Item 1",
        "Item 2",
        "Item 3",
        "Item 4",
        "Item 5",
    ];

    public ObservableCollection<string> TargetItems { get; } =
    [
        "Target A",
        "Target B",
    ];

    public ObservableCollection<string> DroppedFiles { get; } = [];

    public ObservableCollection<string> FilteredItems { get; } =
    [
        "Allowed - Apple",
        "Blocked - Banana",
        "Allowed - Cherry",
        "Blocked - Date",
        "Allowed - Elderberry",
    ];

    public ObservableCollection<string> AcceptedItems { get; } = [];

    public void DragOver(DropInfo dropInfo)
    {
        ArgumentNullException.ThrowIfNull(dropInfo);

        if (dropInfo.DragInfo?.SourceItem is string item &&
            item.StartsWith("Allowed", StringComparison.Ordinal))
        {
            dropInfo.Effects = DragDropEffects.Move;
        }
    }

    public void Drop(DropInfo dropInfo)
    {
        ArgumentNullException.ThrowIfNull(dropInfo);

        if (dropInfo.DragInfo?.SourceItem is not string item)
        {
            return;
        }

        if (dropInfo.DragInfo.SourceCollection is IList sourceList)
        {
            sourceList.Remove(item);
        }

        var index = System.Math.Min(dropInfo.InsertIndex, AcceptedItems.Count);
        AcceptedItems.Insert(index, item);
    }
}