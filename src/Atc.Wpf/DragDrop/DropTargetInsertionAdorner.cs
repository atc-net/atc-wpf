namespace Atc.Wpf.DragDrop;

/// <summary>
/// Adorner that draws an insertion line at the drop position: horizontal across the element
/// by default, or any line set with <see cref="UpdateLine"/> (vertical for horizontal lists).
/// </summary>
internal sealed class DropTargetInsertionAdorner : Adorner
{
    private readonly Pen indicatorPen;
    private Point? lineStart;
    private Point? lineEnd;
    private double yPosition;

    public DropTargetInsertionAdorner(UIElement adornedElement)
        : base(adornedElement)
    {
        var brush = (adornedElement as FrameworkElement)?.TryFindResource("AtcApps.Brushes.Accent") as Brush
            ?? Brushes.DodgerBlue;

        indicatorPen = new Pen(brush, 2);
        indicatorPen.Freeze();

        IsHitTestVisible = false;
    }

    public void UpdatePosition(double y)
    {
        yPosition = y;
        lineStart = null;
        lineEnd = null;
        InvalidateVisual();
    }

    public void UpdateLine(
        Point start,
        Point end)
    {
        lineStart = start;
        lineEnd = end;
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        if (lineStart.HasValue && lineEnd.HasValue)
        {
            drawingContext.DrawLine(indicatorPen, lineStart.Value, lineEnd.Value);
            return;
        }

        drawingContext.DrawLine(
            indicatorPen,
            new Point(0, yPosition),
            new Point(AdornedElement.RenderSize.Width, yPosition));
    }
}