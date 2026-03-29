namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// A ruler control that displays zoom-aware tick marks and position labels.
/// Bind to a <see cref="ZoomBox"/> to synchronize with its viewport.
/// </summary>
public sealed partial class ZoomRuler : FrameworkElement
{
    public static readonly DependencyProperty ZoomBoxProperty = DependencyProperty.Register(
        nameof(ZoomBox),
        typeof(ZoomBox),
        typeof(ZoomRuler),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            OnZoomBoxChanged));

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(ZoomRulerOrientation),
        typeof(ZoomRuler),
        new FrameworkPropertyMetadata(
            ZoomRulerOrientation.Horizontal,
            FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty TickBrushProperty = DependencyProperty.Register(
        nameof(TickBrush),
        typeof(Brush),
        typeof(ZoomRuler),
        new FrameworkPropertyMetadata(
            Brushes.Gray,
            FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty LabelBrushProperty = DependencyProperty.Register(
        nameof(LabelBrush),
        typeof(Brush),
        typeof(ZoomRuler),
        new FrameworkPropertyMetadata(
            Brushes.Gray,
            FrameworkPropertyMetadataOptions.AffectsRender));

    [DependencyProperty(DefaultValue = 8.0)]
    private double fontSize;

    /// <summary>
    /// Gets or sets the <see cref="ZoomBox"/> this ruler tracks.
    /// </summary>
    public ZoomBox? ZoomBox
    {
        get => (ZoomBox?)GetValue(ZoomBoxProperty);
        set => SetValue(ZoomBoxProperty, value);
    }

    /// <summary>
    /// Gets or sets the ruler orientation.
    /// </summary>
    public ZoomRulerOrientation Orientation
    {
        get => (ZoomRulerOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush for tick marks.
    /// </summary>
    public Brush TickBrush
    {
        get => (Brush)GetValue(TickBrushProperty);
        set => SetValue(TickBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush for position labels.
    /// </summary>
    public Brush LabelBrush
    {
        get => (Brush)GetValue(LabelBrushProperty);
        set => SetValue(LabelBrushProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        base.OnRender(drawingContext);

        if (ZoomBox is null)
        {
            return;
        }

        var zoom = ZoomBox.ViewportZoom;
        if (zoom <= 0)
        {
            return;
        }

        if (Orientation == ZoomRulerOrientation.Horizontal)
        {
            RenderHorizontal(drawingContext, zoom);
        }
        else
        {
            RenderVertical(drawingContext, zoom);
        }
    }

    private void RenderHorizontal(
        DrawingContext dc,
        double zoom)
    {
        var offset = ZoomBox!.ContentOffsetX;
        var extent = ActualWidth;
        var height = ActualHeight;

        var spacing = CalculateTickSpacing(zoom);
        var tickPen = CreateTickPen();

        var start = System.Math.Floor(offset / spacing) * spacing;
        var pos = start;
        var screenPos = (pos - offset) * zoom;

        while (screenPos <= extent)
        {
            if (screenPos >= 0)
            {
                var isMajor = IsMajorTick(pos, spacing);
                var tickHeight = isMajor ? height * 0.7 : height * 0.35;

                dc.DrawLine(
                    tickPen,
                    new Point(screenPos, height - tickHeight),
                    new Point(screenPos, height));

                if (isMajor)
                {
                    DrawLabel(dc, pos, new Point(screenPos + 2, 1));
                }
            }

            pos += spacing;
            screenPos = (pos - offset) * zoom;
        }
    }

    private void RenderVertical(
        DrawingContext dc,
        double zoom)
    {
        var offset = ZoomBox!.ContentOffsetY;
        var extent = ActualHeight;
        var width = ActualWidth;

        var spacing = CalculateTickSpacing(zoom);
        var tickPen = CreateTickPen();

        var start = System.Math.Floor(offset / spacing) * spacing;
        var pos = start;
        var screenPos = (pos - offset) * zoom;

        while (screenPos <= extent)
        {
            if (screenPos >= 0)
            {
                var isMajor = IsMajorTick(pos, spacing);
                var tickWidth = isMajor ? width * 0.7 : width * 0.35;

                dc.DrawLine(
                    tickPen,
                    new Point(width - tickWidth, screenPos),
                    new Point(width, screenPos));

                if (isMajor)
                {
                    var text = FormatLabel(pos);
                    var rotateTransform = new RotateTransform(-90, 2, screenPos + 2);
                    dc.PushTransform(rotateTransform);
                    dc.DrawText(text, new Point(2, screenPos + 2));
                    dc.Pop();
                }
            }

            pos += spacing;
            screenPos = (pos - offset) * zoom;
        }
    }

    private Pen CreateTickPen()
    {
        var pen = new Pen(TickBrush, 0.5);
        pen.Freeze();
        return pen;
    }

    private void DrawLabel(
        DrawingContext dc,
        double value,
        Point position)
    {
        var text = FormatLabel(value);
        dc.DrawText(text, position);
    }

    private FormattedText FormatLabel(double value)
    {
        var label = System.Math.Abs(value) < 0.01
            ? "0"
            : value.ToString("F0", CultureInfo.InvariantCulture);

        return new FormattedText(
            label,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface("Segoe UI"),
            10.0,
            LabelBrush,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);
    }

    private static double CalculateTickSpacing(double zoom)
    {
        var baseSpacing = 10.0;
        var screenSpacing = baseSpacing * zoom;

        while (screenSpacing > 150)
        {
            baseSpacing /= 5;
            screenSpacing = baseSpacing * zoom;
        }

        while (screenSpacing < 15)
        {
            baseSpacing *= 5;
            screenSpacing = baseSpacing * zoom;
        }

        return baseSpacing;
    }

    private static bool IsMajorTick(
        double position,
        double spacing)
    {
        var majorSpacing = spacing * 5;
        return System.Math.Abs(position % majorSpacing) < spacing * 0.1;
    }

    private static void OnZoomBoxChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var ruler = (ZoomRuler)d;

        if (e.OldValue is ZoomBox oldBox)
        {
            oldBox.ContentOffsetXChanged -= ruler.OnZoomBoxChanged;
            oldBox.ContentOffsetYChanged -= ruler.OnZoomBoxChanged;
            oldBox.ContentZoomChanged -= ruler.OnZoomBoxChanged;
        }

        if (e.NewValue is ZoomBox newBox)
        {
            newBox.ContentOffsetXChanged += ruler.OnZoomBoxChanged;
            newBox.ContentOffsetYChanged += ruler.OnZoomBoxChanged;
            newBox.ContentZoomChanged += ruler.OnZoomBoxChanged;
        }

        ruler.InvalidateVisual();
    }

    private void OnZoomBoxChanged(
        object? sender,
        EventArgs e)
        => InvalidateVisual();
}