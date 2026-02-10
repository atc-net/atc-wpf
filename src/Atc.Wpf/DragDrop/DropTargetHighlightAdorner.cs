namespace Atc.Wpf.DragDrop;

/// <summary>
/// Adorner that draws a highlight border around the drop target element.
/// </summary>
internal sealed class DropTargetHighlightAdorner : Adorner
{
    private readonly Pen borderPen;

    public DropTargetHighlightAdorner(UIElement adornedElement)
        : base(adornedElement)
    {
        var brush = (adornedElement as FrameworkElement)?.TryFindResource("AtcApps.Brushes.Accent") as Brush
            ?? Brushes.DodgerBlue;

        borderPen = new Pen(brush, 2);
        borderPen.Freeze();

        IsHitTestVisible = false;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        var rect = new Rect(AdornedElement.RenderSize);
        drawingContext.DrawRectangle(brush: null, borderPen, rect);
    }
}