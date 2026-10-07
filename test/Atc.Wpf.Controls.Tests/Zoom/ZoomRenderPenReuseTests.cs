namespace Atc.Wpf.Controls.Tests.Zoom;

/// <summary>
/// The zoom ruler and grid overlay re-render on every pan/zoom frame. Their pens are cached and only rebuilt
/// when a brush or thickness changes, so a frame does not allocate new pens.
/// </summary>
public sealed class ZoomRenderPenReuseTests
{
    private static readonly MethodInfo OnRenderMethod = typeof(UIElement).GetMethod(
        "OnRender",
        BindingFlags.Instance | BindingFlags.NonPublic)!;

    [StaFact]
    public void ZoomRuler_ConsecutiveRenders_ReuseTheTickPen()
    {
        var ruler = CreateRuler();

        var first = RenderPens(ruler);
        var second = RenderPens(ruler);

        Assert.NotEmpty(first);
        Assert.Same(first[0], second[0]);
    }

    [StaFact]
    public void ZoomRuler_TickBrushChanged_UsesANewPenWithThatBrush()
    {
        var ruler = CreateRuler();
        var before = RenderPens(ruler)[0];

        ruler.TickBrush = Brushes.OrangeRed;
        var after = RenderPens(ruler)[0];

        Assert.NotSame(before, after);
        Assert.Same(Brushes.OrangeRed, after.Brush);
    }

    [StaFact]
    public void ZoomGridOverlay_ConsecutiveRenders_ReuseTheLinePens()
    {
        var overlay = CreateOverlay();

        var first = RenderPens(overlay);
        var second = RenderPens(overlay);

        Assert.NotEmpty(first);
        Assert.Equal(first.Distinct().Count(), second.Distinct().Count());
        Assert.All(second, pen => Assert.Contains(pen, first));
    }

    [StaFact]
    public void ZoomGridOverlay_MinorLineThicknessChanged_UsesANewPenWithThatThickness()
    {
        var overlay = CreateOverlay();
        var before = RenderPens(overlay);

        overlay.MinorLineThickness = 2.5;
        var after = RenderPens(overlay);

        Assert.Contains(after, pen => System.Math.Abs(pen.Thickness - 2.5) < 0.0001);
        Assert.DoesNotContain(after, pen => before.Contains(pen) && System.Math.Abs(pen.Thickness - 0.5) < 0.0001);
    }

    private static ZoomRuler CreateRuler()
    {
        var ruler = new ZoomRuler { ZoomBox = new ZoomBox() };
        ruler.Measure(new Size(400, 30));
        ruler.Arrange(new Rect(0, 0, 400, 30));
        return ruler;
    }

    private static ZoomGridOverlay CreateOverlay()
    {
        var overlay = new ZoomGridOverlay(new ZoomBox());
        overlay.Measure(new Size(400, 400));
        overlay.Arrange(new Rect(0, 0, 400, 400));
        return overlay;
    }

    private static List<Pen> RenderPens(UIElement element)
    {
        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
            OnRenderMethod.Invoke(element, [dc]);
        }

        var pens = new List<Pen>();
        CollectPens(VisualTreeHelper.GetDrawing(visual), pens);
        return pens;
    }

    private static void CollectPens(
        Drawing? drawing,
        List<Pen> pens)
    {
        switch (drawing)
        {
            case DrawingGroup group:
                foreach (var child in group.Children)
                {
                    CollectPens(child, pens);
                }

                break;
            case GeometryDrawing { Pen: not null } geometryDrawing:
                pens.Add(geometryDrawing.Pen);
                break;
        }
    }
}