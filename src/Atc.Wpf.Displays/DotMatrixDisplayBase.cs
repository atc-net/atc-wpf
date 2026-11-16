namespace Atc.Wpf.Displays;

/// <summary>
/// The base of the dot-matrix display controls: draws a grid of dots on a panel inside an optional bezel,
/// the way a character LCD/VFD or a small graphic display looks.
/// </summary>
/// <remarks>
/// The control draws in <see cref="OnRender"/> only, from frozen geometries it keeps until the grid, the size or the lit dots change:
/// one geometry for the faint unlit ("ghost") dots, one for the lit dots and, when <see cref="GlowRadius"/> is above 0,
/// one for a translucent glow around the lit dots. A subclass describes the grid with <see cref="GetGeometry"/>,
/// answers <see cref="IsDotLit"/> and calls <see cref="InvalidateDots"/> when the lit dots change.
/// </remarks>
public abstract partial class DotMatrixDisplayBase : FrameworkElement
{
    private const int MaxColorZones = 4;
    private const double MinBrightness = 0.25;
    private const double MaxBrightness = 1.0;
    private const double MaxDotCornerRadius = 0.5;
    private const double GlowOpacity = 0.25;
    private const double BezelCornerRadius = 1.0;
    private const double PanelCornerRadius = 0.5;

    /// <summary>The colour of the panel (the glass behind the dots).</summary>
    [DependencyProperty(
        DefaultValue = nameof(Brushes.YellowGreen),
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private Brush? panelBrush;

    /// <summary>The colour of a lit dot.</summary>
    [DependencyProperty(
        DefaultValue = nameof(Brushes.Black),
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private Brush? litDotBrush;

    /// <summary>The colour of an unlit dot, drawn at <see cref="GhostOpacity"/>; null uses <see cref="LitDotBrush"/>.</summary>
    [DependencyProperty(Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private Brush? unlitDotBrush;

    /// <summary>The opacity of the faint unlit ("ghost") dots, 0 to 1.</summary>
    [DependencyProperty(
        DefaultValue = 0.08,
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private double ghostOpacity;

    /// <summary>The reach of the glow around lit dots as a fraction of the dot size; 0 draws no glow.</summary>
    [DependencyProperty(
        DefaultValue = 0.0,
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private double glowRadius;

    /// <summary>The corner radius of a dot as a fraction of the dot size, 0 (square) to 0.5 (round).</summary>
    [DependencyProperty(
        DefaultValue = 0.0,
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private double dotCornerRadius;

    /// <summary>The intensity of the lit dots, 0.25 to 1.0.</summary>
    [DependencyProperty(
        DefaultValue = 1.0,
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        CoerceValueCallback = nameof(CoerceBrightness))]
    private double brightness;

    /// <summary>Whether a bezel (a dark frame) is drawn around the panel.</summary>
    [DependencyProperty(
        DefaultValue = true,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)]
    private bool showBezel;

    /// <summary>The colour of the bezel.</summary>
    [DependencyProperty(
        DefaultValue = nameof(Brushes.Black),
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private Brush? bezelBrush;

    /// <summary>How the module fills the size of the control.</summary>
    [DependencyProperty(
        DefaultValue = Stretch.Uniform,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender)]
    private Stretch stretch;

    private int isRedrawPending;
    private int isDotsDirty = 1;
    private LayoutKey cachedKey;
    private DotMatrixLayout? layout;
    private StreamGeometry? unlitDots;
    private StreamGeometry[]? litDots;
    private StreamGeometry?[]? glowDots;

    /// <summary>
    /// Gets the dot grid to draw.
    /// </summary>
    /// <remarks>
    /// The result is compared with the last one on every render, so a subclass whose grid changes
    /// calls <see cref="UIElement.InvalidateMeasure"/> and <see cref="InvalidateDots"/>.
    /// </remarks>
    /// <returns>The dot grid.</returns>
    protected abstract DotMatrixGeometry GetGeometry();

    /// <summary>
    /// Gets a value indicating whether a dot is lit. Called on the UI thread while the dots are redrawn.
    /// </summary>
    /// <param name="x">The dot column, 0 to <see cref="DotMatrixGeometry.DotColumns"/> - 1.</param>
    /// <param name="y">The dot row, 0 to <see cref="DotMatrixGeometry.DotRows"/> - 1.</param>
    /// <returns><see langword="true"/> when the dot is lit.</returns>
    protected abstract bool IsDotLit(
        int x,
        int y);

    /// <summary>
    /// Gets the number of colour zones the lit dots are split into, 1 to 4 (a value outside the range is coerced).
    /// Call <see cref="InvalidateDots"/> when it changes.
    /// </summary>
    /// <remarks>
    /// A two-colour panel (for example a yellow-blue OLED) uses two zones. With one zone every lit dot uses <see cref="LitDotBrush"/>.
    /// </remarks>
    protected virtual int ColorZoneCount
        => 1;

    /// <summary>
    /// Gets the colour zone of a lit dot. Called on the UI thread while the dots are redrawn, for lit dots only.
    /// </summary>
    /// <param name="x">The dot column.</param>
    /// <param name="y">The dot row.</param>
    /// <returns>The zone, 0 to <see cref="ColorZoneCount"/> - 1; a result outside the range counts as zone 0.</returns>
    protected virtual int GetDotColorZone(
        int x,
        int y)
        => 0;

    /// <summary>
    /// Gets the brush of the lit dots of a colour zone.
    /// </summary>
    /// <param name="zone">The zone, 0 to <see cref="ColorZoneCount"/> - 1.</param>
    /// <returns>The brush; <see langword="null"/> draws no dots for the zone. Zone 0 is <see cref="LitDotBrush"/> unless overridden.</returns>
    protected virtual Brush? GetZoneLitDotBrush(int zone)
        => LitDotBrush;

    /// <summary>
    /// Tells the control that the lit dots have changed, so it asks <see cref="IsDotLit"/> again and redraws.
    /// </summary>
    /// <remarks>
    /// Safe to call from any thread. Calls are coalesced: at most one redraw is pending at a time,
    /// and it runs on the UI thread at <see cref="DispatcherPriority.Render"/>.
    /// </remarks>
    protected void InvalidateDots()
    {
        Interlocked.Exchange(ref isDotsDirty, 1);
        if (Interlocked.CompareExchange(ref isRedrawPending, 1, 0) != 0)
        {
            return;
        }

        var dispatcher = Dispatcher;
        if (dispatcher.HasShutdownStarted)
        {
            Interlocked.Exchange(ref isRedrawPending, 0);
            return;
        }

        _ = dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(OnRedrawRequested));
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
        => DotMatrixLayout.Measure(GetGeometry(), ShowBezel, availableSize, Stretch);

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        base.OnRender(drawingContext);

        var isLayoutChanged = EnsureLayout();
        var isDotsChanged = Interlocked.Exchange(ref isDotsDirty, 0) == 1;
        if (layout is null || layout.IsEmpty)
        {
            return;
        }

        if (isLayoutChanged || isDotsChanged || litDots is null)
        {
            BuildDots(layout);
        }

        var bounds = new Rect(RenderSize);
        var isClipped = !bounds.Contains(layout.ModuleRect);
        if (isClipped)
        {
            drawingContext.PushClip(new RectangleGeometry(bounds));
        }

        DrawPanel(drawingContext, layout);
        DrawDots(drawingContext);

        if (isClipped)
        {
            drawingContext.Pop();
        }
    }

    private static object CoerceBrightness(
        DependencyObject d,
        object value)
    {
        var brightnessValue = (double)value;
        return double.IsNaN(brightnessValue)
            ? MaxBrightness
            : System.Math.Clamp(brightnessValue, MinBrightness, MaxBrightness);
    }

    private static double ClampFraction(
        double value,
        double max)
        => double.IsFinite(value)
            ? System.Math.Clamp(value, 0, max)
            : 0;

    private static StreamGeometry CreateGeometry()
        => new() { FillRule = FillRule.Nonzero };

    private static void AddDot(
        StreamGeometryContext context,
        Rect rect,
        double radiusX,
        double radiusY)
    {
        if (radiusX <= 0 || radiusY <= 0)
        {
            context.BeginFigure(rect.TopLeft, isFilled: true, isClosed: true);
            context.LineTo(rect.TopRight, isStroked: false, isSmoothJoin: false);
            context.LineTo(rect.BottomRight, isStroked: false, isSmoothJoin: false);
            context.LineTo(rect.BottomLeft, isStroked: false, isSmoothJoin: false);
            return;
        }

        var radius = new Size(radiusX, radiusY);
        context.BeginFigure(new Point(rect.Left + radiusX, rect.Top), isFilled: true, isClosed: true);
        context.LineTo(new Point(rect.Right - radiusX, rect.Top), isStroked: false, isSmoothJoin: false);
        AddCorner(context, new Point(rect.Right, rect.Top + radiusY), radius);
        context.LineTo(new Point(rect.Right, rect.Bottom - radiusY), isStroked: false, isSmoothJoin: false);
        AddCorner(context, new Point(rect.Right - radiusX, rect.Bottom), radius);
        context.LineTo(new Point(rect.Left + radiusX, rect.Bottom), isStroked: false, isSmoothJoin: false);
        AddCorner(context, new Point(rect.Left, rect.Bottom - radiusY), radius);
        context.LineTo(new Point(rect.Left, rect.Top + radiusY), isStroked: false, isSmoothJoin: false);
        AddCorner(context, new Point(rect.Left + radiusX, rect.Top), radius);
    }

    private static void AddCorner(
        StreamGeometryContext context,
        Point end,
        Size radius)
        => context.ArcTo(
            end,
            radius,
            rotationAngle: 0,
            isLargeArc: false,
            SweepDirection.Clockwise,
            isStroked: false,
            isSmoothJoin: false);

    private void OnRedrawRequested()
    {
        Interlocked.Exchange(ref isRedrawPending, 0);
        InvalidateVisual();
    }

    private bool EnsureLayout()
    {
        var key = new LayoutKey(
            GetGeometry(),
            RenderSize,
            Stretch,
            ShowBezel,
            ClampFraction(DotCornerRadius, MaxDotCornerRadius),
            ClampFraction(GlowRadius, double.MaxValue));

        if (layout is not null && key == cachedKey)
        {
            return false;
        }

        cachedKey = key;
        layout = key.Geometry is null
            ? null
            : DotMatrixLayout.Create(key.Geometry, key.ShowBezel, key.Size, key.Stretch);

        return true;
    }

    private void BuildDots(DotMatrixLayout dotLayout)
    {
        var zoneCount = GetZoneCount();
        var unlit = CreateGeometry();
        var lit = new StreamGeometry[zoneCount];
        var glow = new StreamGeometry?[zoneCount];
        for (var zone = 0; zone < zoneCount; zone++)
        {
            lit[zone] = CreateGeometry();
            glow[zone] = cachedKey.GlowRadius > 0 ? CreateGeometry() : null;
        }

        FillDots(dotLayout, unlit, lit, glow);

        unlit.Freeze();
        foreach (var geometry in lit)
        {
            geometry.Freeze();
        }

        foreach (var geometry in glow)
        {
            geometry?.Freeze();
        }

        unlitDots = unlit;
        litDots = lit;
        glowDots = glow;
    }

    private void FillDots(
        DotMatrixLayout dotLayout,
        StreamGeometry unlit,
        StreamGeometry[] lit,
        StreamGeometry?[] glow)
    {
        var zoneCount = lit.Length;
        var radiusX = cachedKey.CornerRadius * dotLayout.ScaleX;
        var radiusY = cachedKey.CornerRadius * dotLayout.ScaleY;
        var glowX = cachedKey.GlowRadius * dotLayout.ScaleX;
        var glowY = cachedKey.GlowRadius * dotLayout.ScaleY;

        var litContexts = new StreamGeometryContext[zoneCount];
        var glowContexts = new StreamGeometryContext?[zoneCount];
        using var unlitContext = unlit.Open();
        try
        {
            for (var zone = 0; zone < zoneCount; zone++)
            {
                litContexts[zone] = lit[zone].Open();
                glowContexts[zone] = glow[zone]?.Open();
            }

            for (var y = 0; y < dotLayout.DotRows; y++)
            {
                for (var x = 0; x < dotLayout.DotColumns; x++)
                {
                    var rect = dotLayout.GetDotRect(x, y);
                    if (!IsDotLit(x, y))
                    {
                        AddDot(unlitContext, rect, radiusX, radiusY);
                        continue;
                    }

                    var zone = zoneCount == 1 ? 0 : GetDotColorZone(x, y);
                    if (zone < 0 || zone >= zoneCount)
                    {
                        zone = 0;
                    }

                    AddDot(litContexts[zone], rect, radiusX, radiusY);
                    if (glowContexts[zone] is { } glowContext)
                    {
                        rect.Inflate(glowX, glowY);
                        AddDot(glowContext, rect, rect.Width / 2, rect.Height / 2);
                    }
                }
            }
        }
        finally
        {
            for (var zone = 0; zone < zoneCount; zone++)
            {
                litContexts[zone]?.Close();
                glowContexts[zone]?.Close();
            }
        }
    }

    private int GetZoneCount()
        => System.Math.Clamp(ColorZoneCount, 1, MaxColorZones);

    private void DrawPanel(
        DrawingContext drawingContext,
        DotMatrixLayout dotLayout)
    {
        var scale = System.Math.Min(dotLayout.ScaleX, dotLayout.ScaleY);

        if (cachedKey.ShowBezel && BezelBrush is { } bezel)
        {
            drawingContext.DrawRoundedRectangle(
                bezel,
                pen: null,
                dotLayout.ModuleRect,
                BezelCornerRadius * scale,
                BezelCornerRadius * scale);
        }

        if (PanelBrush is { } panel)
        {
            drawingContext.DrawRoundedRectangle(
                panel,
                pen: null,
                dotLayout.PanelRect,
                PanelCornerRadius * scale,
                PanelCornerRadius * scale);
        }
    }

    private void DrawDots(DrawingContext drawingContext)
    {
        var ghostBrush = UnlitDotBrush ?? LitDotBrush;
        var ghostOpacityValue = ClampFraction(GhostOpacity, 1);

        if (ghostBrush is not null && ghostOpacityValue > 0 && unlitDots is not null)
        {
            drawingContext.PushOpacity(ghostOpacityValue);
            drawingContext.DrawGeometry(ghostBrush, pen: null, unlitDots);
            drawingContext.Pop();
        }

        if (litDots is null || glowDots is null)
        {
            return;
        }

        var brightnessValue = Brightness;
        var brushes = new Brush?[litDots.Length];
        for (var zone = 0; zone < brushes.Length; zone++)
        {
            brushes[zone] = GetZoneLitDotBrush(zone);
        }

        for (var zone = 0; zone < brushes.Length; zone++)
        {
            if (brushes[zone] is { } brush && glowDots[zone] is { } glow)
            {
                drawingContext.PushOpacity(GlowOpacity * brightnessValue);
                drawingContext.DrawGeometry(brush, pen: null, glow);
                drawingContext.Pop();
            }
        }

        for (var zone = 0; zone < brushes.Length; zone++)
        {
            if (brushes[zone] is { } brush)
            {
                drawingContext.PushOpacity(brightnessValue);
                drawingContext.DrawGeometry(brush, pen: null, litDots[zone]);
                drawingContext.Pop();
            }
        }
    }

    private readonly record struct LayoutKey(
        DotMatrixGeometry? Geometry,
        Size Size,
        Stretch Stretch,
        bool ShowBezel,
        double CornerRadius,
        double GlowRadius);
}