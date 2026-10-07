namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// An adorner that renders a zoom-adaptive grid overlay on a <see cref="ZoomBox"/>.
/// The grid density adjusts automatically based on the current zoom level —
/// coarse lines at low zoom, fine lines at high zoom.
/// </summary>
public sealed class ZoomGridOverlay : Adorner
{
    public static readonly DependencyProperty BaseGridSpacingProperty = DependencyProperty.Register(
        nameof(BaseGridSpacing),
        typeof(double),
        typeof(ZoomGridOverlay),
        new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsRender),
        IsValidGridSpacing);

    public static readonly DependencyProperty MinorLineBrushProperty = DependencyProperty.Register(
        nameof(MinorLineBrush),
        typeof(Brush),
        typeof(ZoomGridOverlay),
        new FrameworkPropertyMetadata(CreateFrozenBrush(40), FrameworkPropertyMetadataOptions.AffectsRender, OnPenPropertyChanged));

    public static readonly DependencyProperty MajorLineBrushProperty = DependencyProperty.Register(
        nameof(MajorLineBrush),
        typeof(Brush),
        typeof(ZoomGridOverlay),
        new FrameworkPropertyMetadata(CreateFrozenBrush(80), FrameworkPropertyMetadataOptions.AffectsRender, OnPenPropertyChanged));

    public static readonly DependencyProperty MinorLineThicknessProperty = DependencyProperty.Register(
        nameof(MinorLineThickness),
        typeof(double),
        typeof(ZoomGridOverlay),
        new FrameworkPropertyMetadata(0.5, FrameworkPropertyMetadataOptions.AffectsRender, OnPenPropertyChanged));

    public static readonly DependencyProperty MajorLineThicknessProperty = DependencyProperty.Register(
        nameof(MajorLineThickness),
        typeof(double),
        typeof(ZoomGridOverlay),
        new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, OnPenPropertyChanged));

    private readonly ZoomBox zoomBox;

    // Rendered on every pan/zoom frame: pens are rebuilt only when a brush or thickness changes.
    private Pen? minorPen;
    private Pen? majorPen;

    public ZoomGridOverlay(ZoomBox adornedElement)
        : base(adornedElement)
    {
        zoomBox = adornedElement;
        IsHitTestVisible = false;

        zoomBox.ContentZoomChanged += OnZoomBoxViewChanged;
        zoomBox.ContentOffsetXChanged += OnZoomBoxViewChanged;
        zoomBox.ContentOffsetYChanged += OnZoomBoxViewChanged;
    }

    /// <summary>
    /// Gets or sets the base grid spacing in content coordinates.
    /// The visible spacing is multiplied or divided by 2/5/10 depending on zoom.
    /// Default is 50. Must be a positive, finite number.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The value is zero, negative, NaN or infinite.</exception>
    public double BaseGridSpacing
    {
        get => (double)GetValue(BaseGridSpacingProperty);
        set
        {
            if (!IsValidGridSpacing(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "BaseGridSpacing must be a positive, finite number.");
            }

            SetValue(BaseGridSpacingProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the brush used for minor grid lines.
    /// </summary>
    public Brush MinorLineBrush
    {
        get => (Brush)GetValue(MinorLineBrushProperty);
        set => SetValue(MinorLineBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for major grid lines (every 5th line).
    /// </summary>
    public Brush MajorLineBrush
    {
        get => (Brush)GetValue(MajorLineBrushProperty);
        set => SetValue(MajorLineBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the thickness of minor grid lines.
    /// </summary>
    public double MinorLineThickness
    {
        get => (double)GetValue(MinorLineThicknessProperty);
        set => SetValue(MinorLineThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the thickness of major grid lines.
    /// </summary>
    public double MajorLineThickness
    {
        get => (double)GetValue(MajorLineThicknessProperty);
        set => SetValue(MajorLineThicknessProperty, value);
    }

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

        minorPen ??= CreateFrozenPen(MinorLineBrush, MinorLineThickness);
        majorPen ??= CreateFrozenPen(MajorLineBrush, MajorLineThickness);

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

    private static bool IsValidGridSpacing(object value)
        => value is double spacing && double.IsFinite(spacing) && spacing > 0;

    private static void OnPenPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var overlay = (ZoomGridOverlay)d;
        overlay.minorPen = null;
        overlay.majorPen = null;
    }

    private static Pen CreateFrozenPen(
        Brush brush,
        double thickness)
    {
        var pen = new Pen(brush, thickness);
        pen.Freeze();
        return pen;
    }

    private static SolidColorBrush CreateFrozenBrush(byte alpha)
    {
        var brush = new SolidColorBrush(Color.FromArgb(alpha, 128, 128, 128));
        brush.Freeze();
        return brush;
    }

    private void OnZoomBoxViewChanged(
        object? sender,
        EventArgs e)
        => InvalidateVisual();

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
        zoomBox.ContentZoomChanged -= OnZoomBoxViewChanged;
        zoomBox.ContentOffsetXChanged -= OnZoomBoxViewChanged;
        zoomBox.ContentOffsetYChanged -= OnZoomBoxViewChanged;

        var adornerLayer = AdornerLayer.GetAdornerLayer(zoomBox);
        adornerLayer?.Remove(this);
    }
}