namespace Atc.Wpf.Tests.DragDrop;

public sealed class DragDropManagerTests
{
    private static readonly FieldInfo DragStartPointField = typeof(DragDropManager)
        .GetField("dragStartPoint", BindingFlags.NonPublic | BindingFlags.Static)!;

    [StaFact]
    public void BeginDrag_CancelledByDragHandler_ClearsPendingDragStart()
    {
        // Arrange
        var items = new ObservableCollection<string> { "a", "b" };
        var listBox = new ListBox { ItemsSource = items };
        var dragHandler = new CancellingDragHandler();
        DragDropAttach.SetDragHandler(listBox, dragHandler);
        DragStartPointField.SetValue(obj: null, new Point(1, 1));

        // Act
        DragDropManager.BeginDrag(listBox, "a");

        // Assert
        Assert.Equal(1, dragHandler.StartDragCount);
        Assert.Null(DragStartPointField.GetValue(obj: null));
    }

    private sealed class CancellingDragHandler : IDragHandler
    {
        public int StartDragCount { get; private set; }

        public void StartDrag(DragInfo dragInfo)
        {
            StartDragCount++;
            dragInfo.Effects = DragDropEffects.None;
        }
    }
}