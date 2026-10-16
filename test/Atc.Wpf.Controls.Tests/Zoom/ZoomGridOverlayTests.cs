namespace Atc.Wpf.Controls.Tests.Zoom;

public sealed class ZoomGridOverlayTests
{
    [StaTheory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void BaseGridSpacing_InvalidValue_Throws(double value)
    {
        // A non-positive or non-finite spacing makes the adaptive-spacing loop in OnRender never terminate.
        var overlay = new ZoomGridOverlay(new ZoomBox());

        Assert.Throws<ArgumentOutOfRangeException>(() => overlay.BaseGridSpacing = value);
        Assert.Equal(50.0, overlay.BaseGridSpacing);
    }

    [StaFact]
    public void BaseGridSpacing_PositiveValue_IsAccepted()
    {
        var overlay = new ZoomGridOverlay(new ZoomBox());

        overlay.BaseGridSpacing = 0.25;

        Assert.Equal(0.25, overlay.BaseGridSpacing);
    }

    [StaFact]
    public void BaseGridSpacing_InvalidValueThroughBinding_IsRejected()
    {
        var overlay = new ZoomGridOverlay(new ZoomBox());

        BindingOperations.SetBinding(overlay, ZoomGridOverlay.BaseGridSpacingProperty, new Binding { Source = 0.0 });

        Assert.Equal(50.0, overlay.BaseGridSpacing);
    }

    [StaTheory]
    [InlineData(nameof(ZoomGridOverlay.MinorLineBrush))]
    [InlineData(nameof(ZoomGridOverlay.MajorLineBrush))]
    [InlineData(nameof(ZoomGridOverlay.MinorLineThickness))]
    [InlineData(nameof(ZoomGridOverlay.MajorLineThickness))]
    [InlineData(nameof(ZoomGridOverlay.BaseGridSpacing))]
    public void StylingProperty_IsADependencyPropertyThatAffectsRender(
        string propertyName)
    {
        // Bindable and themable (e.g. via SetResourceReference), and a change repaints the grid.
        var descriptor = DependencyPropertyDescriptor.FromName(propertyName, typeof(ZoomGridOverlay), typeof(ZoomGridOverlay));

        Assert.NotNull(descriptor);
        var metadata = Assert.IsType<FrameworkPropertyMetadata>(descriptor.DependencyProperty.GetMetadata(typeof(ZoomGridOverlay)));
        Assert.True(metadata.AffectsRender);
    }

    [StaFact]
    public void MajorLineBrush_CanBeBound()
    {
        var overlay = new ZoomGridOverlay(new ZoomBox());
        var brush = new SolidColorBrush(Colors.OrangeRed);

        BindingOperations.SetBinding(overlay, ZoomGridOverlay.MajorLineBrushProperty, new Binding { Source = brush });

        Assert.Same(brush, overlay.MajorLineBrush);
    }

    [StaFact]
    public void Render_PannedByOneMinorCell_KeepsMajorLinesAtMultiplesOfFiveCells()
    {
        // With the view panned one cell (50) to the right, the line at content x = 250 (every 5th cell)
        // is still a major line and the first drawn line, at content x = 50, is a minor one.
        var zoomBox = ZoomBoxFactory.CreateTemplated(new Canvas { Width = 800, Height = 600 }, 400, 400);
        zoomBox.ContentOffsetX = 50;
        var zoom = zoomBox.ViewportZoom;
        Assert.InRange(zoom, 0.4, 4); // the base spacing of 50 is used as is
        var overlay = new ZoomGridOverlay(zoomBox);
        overlay.Measure(new Size(400, 400));
        overlay.Arrange(new Rect(0, 0, 400, 400));

        var verticalLines = RenderLines(overlay)
            .Where(x => IsClose(x.Line.StartPoint.X, x.Line.EndPoint.X))
            .ToList();

        var atContent250 = Assert.Single(verticalLines, x => IsClose(x.Line.StartPoint.X, (250 - 50) * zoom));
        var atContent50 = Assert.Single(verticalLines, x => IsClose(x.Line.StartPoint.X, 0));
        Assert.Equal(overlay.MajorLineThickness, atContent250.Pen.Thickness);
        Assert.Equal(overlay.MinorLineThickness, atContent50.Pen.Thickness);
    }

    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Forcing a collection is how the leak is detected.")]
    [StaFact]
    public void Detach_ReleasesTheOverlayWhileTheZoomBoxLives()
    {
        var zoomBox = new ZoomBox();

        var reference = CreateAndDetach(zoomBox);
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.False(reference.IsAlive);
        GC.KeepAlive(zoomBox);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateAndDetach(ZoomBox zoomBox)
    {
        var overlay = new ZoomGridOverlay(zoomBox);
        overlay.Detach();
        return new WeakReference(overlay);
    }

    private static bool IsClose(
        double a,
        double b)
        => System.Math.Abs(a - b) < 0.001;

    private static List<(LineGeometry Line, Pen Pen)> RenderLines(
        UIElement element)
    {
        var onRender = typeof(UIElement).GetMethod("OnRender", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
            onRender.Invoke(element, [dc]);
        }

        var lines = new List<(LineGeometry Line, Pen Pen)>();
        CollectLines(VisualTreeHelper.GetDrawing(visual), lines);
        return lines;
    }

    private static void CollectLines(
        Drawing? drawing,
        List<(LineGeometry Line, Pen Pen)> lines)
    {
        switch (drawing)
        {
            case DrawingGroup group:
                foreach (var child in group.Children)
                {
                    CollectLines(child, lines);
                }

                break;
            case GeometryDrawing { Pen: not null, Geometry: LineGeometry line } geometryDrawing:
                lines.Add((line, geometryDrawing.Pen));
                break;
        }
    }
}