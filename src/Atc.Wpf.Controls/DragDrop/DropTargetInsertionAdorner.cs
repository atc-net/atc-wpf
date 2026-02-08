namespace Atc.Wpf.Controls.DragDrop;

/// <summary>
/// Adorner that draws a horizontal insertion line at the drop position.
/// </summary>
internal sealed class DropTargetInsertionAdorner : Adorner
{
    private readonly Pen indicatorPen;
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
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        drawingContext.DrawLine(
            indicatorPen,
            new Point(0, yPosition),
            new Point(AdornedElement.RenderSize.Width, yPosition));
    }
}