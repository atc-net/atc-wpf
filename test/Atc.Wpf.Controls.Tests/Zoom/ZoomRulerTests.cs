namespace Atc.Wpf.Controls.Tests.Zoom;

public sealed class ZoomRulerTests
{
    private static readonly MethodInfo OnRenderMethod = typeof(UIElement).GetMethod(
        "OnRender",
        BindingFlags.Instance | BindingFlags.NonPublic)!;

    // A right-to-left ruler is mirrored, so its labels are drawn flipped back to stay readable.
    [StaTheory]
    [InlineData(ZoomRulerOrientation.Horizontal, FlowDirection.LeftToRight, false)]
    [InlineData(ZoomRulerOrientation.Horizontal, FlowDirection.RightToLeft, true)]
    [InlineData(ZoomRulerOrientation.Vertical, FlowDirection.LeftToRight, false)]
    [InlineData(ZoomRulerOrientation.Vertical, FlowDirection.RightToLeft, true)]
    public void Labels_AreFlippedBackInRightToLeft(
        ZoomRulerOrientation orientation,
        FlowDirection flowDirection,
        bool expectedFlipped)
    {
        var ruler = new ZoomRuler { ZoomBox = new ZoomBox(), Orientation = orientation, FlowDirection = flowDirection };
        ruler.Measure(new Size(400, 400));
        ruler.Arrange(new Rect(0, 0, 400, 400));

        var labelsFlipped = RenderLabelsFlipped(ruler);

        Assert.NotEmpty(labelsFlipped);
        Assert.All(labelsFlipped, flipped => Assert.Equal(expectedFlipped, flipped));
    }

    [Fact]
    public void EnumerateTicks_FractionalSpacing_MakesEveryFifthTickMajor()
    {
        // Zoom 1000 gives a tick spacing of 0.08, which does not add up exactly in binary floating point.
        var ticks = ZoomRuler.EnumerateTicks(offset: 0, extent: 100_000, zoom: 1000).ToList();

        Assert.Equal(1251, ticks.Count);
        Assert.Equal(251, ticks.Count(t => t.IsMajor));
        Assert.All(
            ticks.Where((_, i) => i % 5 == 0),
            t => Assert.True(t.IsMajor, $"The tick at {t.Position} should be major."));
    }

    [StaFact]
    public void Labels_UseTheFontSize()
    {
        var ruler = new ZoomRuler { ZoomBox = new ZoomBox(), FontSize = 20 };
        ruler.Measure(new Size(400, 30));
        ruler.Arrange(new Rect(0, 0, 400, 30));

        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
            OnRenderMethod.Invoke(ruler, [dc]);
        }

        var emSizes = new List<double>();
        CollectEmSizes(VisualTreeHelper.GetDrawing(visual), emSizes);

        Assert.NotEmpty(emSizes);
        Assert.All(emSizes, size => Assert.Equal(20, size, 6));
    }

    private static void CollectEmSizes(
        Drawing? drawing,
        List<double> emSizes)
    {
        switch (drawing)
        {
            case DrawingGroup group:
                foreach (var child in group.Children)
                {
                    CollectEmSizes(child, emSizes);
                }

                break;
            case GlyphRunDrawing glyphRunDrawing:
                emSizes.Add(glyphRunDrawing.GlyphRun.FontRenderingEmSize);
                break;
        }
    }

    private static List<bool> RenderLabelsFlipped(UIElement element)
    {
        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
            OnRenderMethod.Invoke(element, [dc]);
        }

        var flipped = new List<bool>();
        CollectLabels(VisualTreeHelper.GetDrawing(visual), Matrix.Identity, flipped);
        return flipped;
    }

    private static void CollectLabels(
        Drawing? drawing,
        Matrix transform,
        List<bool> flipped)
    {
        switch (drawing)
        {
            case DrawingGroup group:
                var groupTransform = group.Transform is null
                    ? transform
                    : Matrix.Multiply(group.Transform.Value, transform);
                foreach (var child in group.Children)
                {
                    CollectLabels(child, groupTransform, flipped);
                }

                break;
            case GlyphRunDrawing:
                // A negative determinant means the label is mirrored, whatever its rotation.
                flipped.Add(transform.Determinant < 0);
                break;
        }
    }
}