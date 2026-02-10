namespace Atc.Wpf.DragDrop;

/// <summary>
/// Adorner that renders a semi-transparent ghost of the dragged item during drag-and-drop.
/// </summary>
internal sealed class DragDropAdorner : Adorner
{
    private readonly VisualBrush visualBrush;
    private readonly Size renderSize;
    private Point currentPosition;

    public DragDropAdorner(
        UIElement adornedElement,
        UIElement draggedElement)
        : base(adornedElement)
    {
        renderSize = new Size(draggedElement.RenderSize.Width, draggedElement.RenderSize.Height);
        visualBrush = new VisualBrush(draggedElement)
        {
            Opacity = 0.6,
        };

        IsHitTestVisible = false;
    }

    public void UpdatePosition(Point position)
    {
        currentPosition = position;
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        var rect = new Rect(
            currentPosition.X - (renderSize.Width / 2),
            currentPosition.Y - (renderSize.Height / 2),
            renderSize.Width,
            renderSize.Height);

        drawingContext.DrawRectangle(visualBrush, pen: null, rect);
    }
}