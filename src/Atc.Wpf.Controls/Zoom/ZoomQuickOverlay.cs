namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// An adorner that shows a zoomed-out overview of the ZoomBox content
/// with a draggable viewport rectangle. Used for the "Quick Zoom" tool —
/// activate with a key press to see the full canvas and select an area to zoom to.
/// </summary>
public sealed class ZoomQuickOverlay : Adorner
{
    private readonly ZoomBox zoomBox;
    private readonly Brush overlayBrush;
    private readonly Brush viewportBrush;
    private readonly Pen viewportPen;
    private bool isDragging;
    private Point dragStart;
    private Rect? selectionRect;

    private ZoomQuickOverlay(ZoomBox adornedElement)
        : base(adornedElement)
    {
        zoomBox = adornedElement;
        IsHitTestVisible = true;
        Focusable = false;

        overlayBrush = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0));
        overlayBrush.Freeze();

        viewportBrush = new SolidColorBrush(Color.FromArgb(60, 100, 150, 255));
        viewportBrush.Freeze();

        viewportPen = new Pen(new SolidColorBrush(Color.FromArgb(200, 100, 150, 255)), 2);
        viewportPen.Freeze();
    }

    /// <summary>
    /// Shows the quick overview overlay on the specified ZoomBox.
    /// The user can click or drag to select an area, then the overlay closes
    /// and the ZoomBox animates to the selected region.
    /// </summary>
    /// <returns>The overlay instance. Call <see cref="Dismiss"/> to close without zooming.</returns>
    public static ZoomQuickOverlay? Show(ZoomBox zoomBox)
    {
        ArgumentNullException.ThrowIfNull(zoomBox);

        var adornerLayer = AdornerLayer.GetAdornerLayer(zoomBox);
        if (adornerLayer is null)
        {
            return null;
        }

        var overlay = new ZoomQuickOverlay(zoomBox);
        adornerLayer.Add(overlay);
        return overlay;
    }

    /// <summary>
    /// Dismisses the overlay without performing any zoom action.
    /// </summary>
    public void Dismiss()
    {
        var adornerLayer = AdornerLayer.GetAdornerLayer(zoomBox);
        adornerLayer?.Remove(this);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        var width = ActualWidth;
        var height = ActualHeight;

        // Draw semi-transparent overlay
        drawingContext.DrawRectangle(overlayBrush, pen: null, new Rect(0, 0, width, height));

        // Draw current viewport indicator
        if (zoomBox.ViewportZoom > 0 && width > 0 && height > 0)
        {
            var contentWidth = zoomBox.ContentViewportWidth * zoomBox.ViewportZoom / GetFitScale();
            var contentHeight = zoomBox.ContentViewportHeight * zoomBox.ViewportZoom / GetFitScale();
            var offsetX = zoomBox.ContentOffsetX * GetFitScale();
            var offsetY = zoomBox.ContentOffsetY * GetFitScale();

            var vpRect = new Rect(offsetX, offsetY, contentWidth, contentHeight);
            vpRect.Intersect(new Rect(0, 0, width, height));

            if (vpRect is { Width: > 0, Height: > 0 })
            {
                drawingContext.DrawRectangle(viewportBrush, viewportPen, vpRect);
            }
        }

        // Draw selection rectangle if dragging
        if (selectionRect is { Width: > 0, Height: > 0 } sel)
        {
            var selPen = new Pen(Brushes.White, 1) { DashStyle = DashStyles.Dash };
            selPen.Freeze();
            drawingContext.DrawRectangle(
                new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)),
                selPen,
                sel);
        }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        isDragging = true;
        dragStart = e.GetPosition(this);
        selectionRect = null;
        CaptureMouse();
        e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (!isDragging)
        {
            return;
        }

        var current = e.GetPosition(this);
        selectionRect = new Rect(dragStart, current);
        InvalidateVisual();
        e.Handled = true;
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        ReleaseMouseCapture();
        isDragging = false;

        var clickPoint = e.GetPosition(this);
        var fitScale = GetFitScale();

        if (selectionRect is { Width: > 5, Height: > 5 } sel)
        {
            // Drag selection → zoom to selected area
            var contentRect = new Rect(
                sel.X / fitScale,
                sel.Y / fitScale,
                sel.Width / fitScale,
                sel.Height / fitScale);

            Dismiss();
            zoomBox.SaveZoom();
            zoomBox.AnimatedZoomTo(contentRect);
        }
        else
        {
            // Click → center on point
            var contentPoint = new Point(
                clickPoint.X / fitScale,
                clickPoint.Y / fitScale);

            Dismiss();
            zoomBox.SaveZoom();
            zoomBox.AnimatedSnapTo(contentPoint);
        }

        e.Handled = true;
    }

    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        Dismiss();
        e.Handled = true;
    }

    private double GetFitScale()
    {
        if (zoomBox.Content is not FrameworkElement content ||
            content.ActualWidth <= 0 ||
            content.ActualHeight <= 0)
        {
            return 1.0;
        }

        var scaleX = ActualWidth / content.ActualWidth;
        var scaleY = ActualHeight / content.ActualHeight;
        return System.Math.Min(scaleX, scaleY);
    }
}