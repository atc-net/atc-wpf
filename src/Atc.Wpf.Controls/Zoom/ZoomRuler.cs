namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// A ruler control that displays zoom-aware tick marks and position labels.
/// Bind to a <see cref="ZoomBox"/> to synchronize with its viewport.
/// </summary>
public sealed partial class ZoomRuler : FrameworkElement
{
    private static readonly Typeface LabelTypeface = new("Segoe UI");

    /// <summary>Identifies the <see cref="ZoomRuler.ZoomBox"/> dependency property.</summary>
    public static readonly DependencyProperty ZoomBoxProperty = DependencyProperty.Register(
        nameof(ZoomBox),
        typeof(ZoomBox),
        typeof(ZoomRuler),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            OnZoomBoxChanged));

    /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(ZoomRulerOrientation),
        typeof(ZoomRuler),
        new FrameworkPropertyMetadata(
            ZoomRulerOrientation.Horizontal,
            FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>Identifies the <see cref="TickBrush"/> dependency property.</summary>
    public static readonly DependencyProperty TickBrushProperty = DependencyProperty.Register(
        nameof(TickBrush),
        typeof(Brush),
        typeof(ZoomRuler),
        new FrameworkPropertyMetadata(
            Brushes.Gray,
            FrameworkPropertyMetadataOptions.AffectsRender,
            OnTickBrushChanged));

    /// <summary>Identifies the <see cref="LabelBrush"/> dependency property.</summary>
    public static readonly DependencyProperty LabelBrushProperty = DependencyProperty.Register(
        nameof(LabelBrush),
        typeof(Brush),
        typeof(ZoomRuler),
        new FrameworkPropertyMetadata(
            Brushes.Gray,
            FrameworkPropertyMetadataOptions.AffectsRender));

    [DependencyProperty(DefaultValue = 10.0, Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private double fontSize;

    // Rendered on every pan/zoom frame: the pen is rebuilt only when TickBrush changes.
    private Pen? cachedTickPen;

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

    /// <inheritdoc />
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

        var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
        if (Orientation == ZoomRulerOrientation.Horizontal)
        {
            RenderHorizontal(drawingContext, zoom, pixelsPerDip);
        }
        else
        {
            RenderVertical(drawingContext, zoom, pixelsPerDip);
        }
    }

    private void RenderHorizontal(
        DrawingContext dc,
        double zoom,
        double pixelsPerDip)
    {
        var offset = ZoomBox!.ContentOffsetX;
        var extent = ActualWidth;
        var height = ActualHeight;

        var tickPen = CreateTickPen();

        foreach (var (pos, screenPos, isMajor) in EnumerateTicks(offset, extent, zoom))
        {
            var tickHeight = isMajor ? height * 0.7 : height * 0.35;

            dc.DrawLine(
                tickPen,
                new Point(screenPos, height - tickHeight),
                new Point(screenPos, height));

            if (isMajor)
            {
                DrawLabel(dc, FormatLabel(pos, pixelsPerDip), new Point(screenPos + 2, 1));
            }
        }
    }

    private void RenderVertical(
        DrawingContext dc,
        double zoom,
        double pixelsPerDip)
    {
        var offset = ZoomBox!.ContentOffsetY;
        var extent = ActualHeight;
        var width = ActualWidth;

        var tickPen = CreateTickPen();

        foreach (var (pos, screenPos, isMajor) in EnumerateTicks(offset, extent, zoom))
        {
            var tickWidth = isMajor ? width * 0.7 : width * 0.35;

            dc.DrawLine(
                tickPen,
                new Point(width - tickWidth, screenPos),
                new Point(width, screenPos));

            if (isMajor)
            {
                var text = FormatLabel(pos, pixelsPerDip);
                var rotateTransform = new RotateTransform(-90, 2, screenPos + 2);
                dc.PushTransform(rotateTransform);
                DrawLabel(dc, text, new Point(2, screenPos + 2));
                dc.Pop();
            }
        }
    }

    /// <summary>
    /// Lists the ticks visible in a ruler of the given extent (in screen units).
    /// </summary>
    /// <remarks>
    /// Each tick position is computed from its integer index, not by adding the spacing up, so a fractional
    /// spacing does not drift and every 5th tick is reliably major.
    /// </remarks>
    internal static IEnumerable<(double Position, double ScreenPosition, bool IsMajor)> EnumerateTicks(
        double offset,
        double extent,
        double zoom)
    {
        var spacing = CalculateTickSpacing(zoom);

        var index = (long)System.Math.Floor(offset / spacing);
        var position = index * spacing;
        var screenPosition = (position - offset) * zoom;

        while (screenPosition <= extent)
        {
            if (screenPosition >= 0)
            {
                yield return (position, screenPosition, index % 5 == 0);
            }

            index++;
            position = index * spacing;
            screenPosition = (position - offset) * zoom;
        }
    }

    // In a right-to-left layout the ruler is mirrored, so flip each label back around its own center.
    private void DrawLabel(
        DrawingContext dc,
        FormattedText text,
        Point origin)
    {
        if (FlowDirection != FlowDirection.RightToLeft)
        {
            dc.DrawText(text, origin);
            return;
        }

        dc.PushTransform(new ScaleTransform(-1, 1, origin.X + (text.Width / 2), origin.Y));
        dc.DrawText(text, origin);
        dc.Pop();
    }

    private static void OnTickBrushChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ZoomRuler)d).cachedTickPen = null;

    private Pen CreateTickPen()
    {
        if (cachedTickPen is null)
        {
            var pen = new Pen(TickBrush, 0.5);
            pen.Freeze();
            cachedTickPen = pen;
        }

        return cachedTickPen;
    }

    private FormattedText FormatLabel(
        double value,
        double pixelsPerDip)
    {
        var label = System.Math.Abs(value) < 0.01
            ? "0"
            : value.ToString("F0", CultureInfo.InvariantCulture);

        return new FormattedText(
            label,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            LabelTypeface,
            FontSize,
            LabelBrush,
            pixelsPerDip);
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