namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// An adorner that renders a zoom-adaptive grid overlay on a <see cref="ZoomBox"/>.
/// The grid density adjusts automatically based on the current zoom level —
/// coarse lines at low zoom, fine lines at high zoom.
/// </summary>
public sealed class ZoomGridOverlay : Adorner
{
    private readonly ZoomBox zoomBox;

    public ZoomGridOverlay(ZoomBox adornedElement)
        : base(adornedElement)
    {
        zoomBox = adornedElement;
        IsHitTestVisible = false;

        zoomBox.ContentZoomChanged += (_, _) => InvalidateVisual();
        zoomBox.ContentOffsetXChanged += (_, _) => InvalidateVisual();
        zoomBox.ContentOffsetYChanged += (_, _) => InvalidateVisual();
    }

    /// <summary>
    /// Gets or sets the base grid spacing in content coordinates.
    /// The visible spacing is multiplied or divided by 2/5/10 depending on zoom.
    /// Default is 50.
    /// </summary>
    public double BaseGridSpacing { get; set; } = 50.0;

    /// <summary>
    /// Gets or sets the brush used for minor grid lines.
    /// </summary>
    public Brush MinorLineBrush { get; set; } = new SolidColorBrush(Color.FromArgb(40, 128, 128, 128));

    /// <summary>
    /// Gets or sets the brush used for major grid lines (every 5th line).
    /// </summary>
    public Brush MajorLineBrush { get; set; } = new SolidColorBrush(Color.FromArgb(80, 128, 128, 128));

    /// <summary>
    /// Gets or sets the thickness of minor grid lines.
    /// </summary>
    public double MinorLineThickness { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets the thickness of major grid lines.
    /// </summary>
    public double MajorLineThickness { get; set; } = 1.0;

    protected override void OnRender(DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        var zoom = zoomBox.ViewportZoom;
        if (zoom <= 0)
        {
            return;
        }

        var spacing = CalculateAdaptiveSpacing(BaseGridSpacing, zoom);
        var screenSpacing = spacing * zoom;

        if (screenSpacing < 4)
        {
            return;
        }

        var offsetX = zoomBox.ContentOffsetX;
        var offsetY = zoomBox.ContentOffsetY;
        var width = ActualWidth;
        var height = ActualHeight;

        var minorPen = new Pen(MinorLineBrush, MinorLineThickness);
        var majorPen = new Pen(MajorLineBrush, MajorLineThickness);
        minorPen.Freeze();
        majorPen.Freeze();

        var startX = System.Math.Floor(offsetX / spacing) * spacing;
        var startY = System.Math.Floor(offsetY / spacing) * spacing;

        DrawGridLines(drawingContext, startX, spacing, offsetX, zoom, width, height, minorPen, majorPen, isVertical: true);
        DrawGridLines(drawingContext, startY, spacing, offsetY, zoom, height, width, minorPen, majorPen, isVertical: false);
    }

    private static void DrawGridLines(
        DrawingContext dc,
        double start,
        double spacing,
        double offset,
        double zoom,
        double extent,
        double crossExtent,
        Pen minorPen,
        Pen majorPen,
        bool isVertical)
    {
        var lineIndex = 0;
        var pos = start;
        var screenPos = (pos - offset) * zoom;

        while (screenPos <= extent)
        {
            if (screenPos >= 0)
            {
                var pen = lineIndex % 5 == 0 ? majorPen : minorPen;
                if (isVertical)
                {
                    dc.DrawLine(pen, new Point(screenPos, 0), new Point(screenPos, crossExtent));
                }
                else
                {
                    dc.DrawLine(pen, new Point(0, screenPos), new Point(crossExtent, screenPos));
                }
            }

            lineIndex++;
            pos += spacing;
            screenPos = (pos - offset) * zoom;
        }
    }

    private static double CalculateAdaptiveSpacing(
        double baseSpacing,
        double zoom)
    {
        var screenSpacing = baseSpacing * zoom;

        while (screenSpacing > 200)
        {
            baseSpacing /= 5;
            screenSpacing = baseSpacing * zoom;
        }

        while (screenSpacing < 20)
        {
            baseSpacing *= 5;
            screenSpacing = baseSpacing * zoom;
        }

        return baseSpacing;
    }

    /// <summary>
    /// Adds a grid overlay adorner to the specified <see cref="ZoomBox"/>.
    /// </summary>
    public static ZoomGridOverlay Attach(ZoomBox zoomBox)
    {
        ArgumentNullException.ThrowIfNull(zoomBox);

        var overlay = new ZoomGridOverlay(zoomBox);
        var adornerLayer = AdornerLayer.GetAdornerLayer(zoomBox);
        adornerLayer?.Add(overlay);
        return overlay;
    }

    /// <summary>
    /// Removes this grid overlay from its adorner layer.
    /// </summary>
    public void Detach()
    {
        var adornerLayer = AdornerLayer.GetAdornerLayer(zoomBox);
        adornerLayer?.Remove(this);
    }
}