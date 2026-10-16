namespace Atc.Wpf.Tests.Controls.Media.W3cSvg;

public sealed class SvgRenderTests
{
    private const string SvgOpen = """<svg viewBox="0 0 10 10" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">""";

    [StaTheory]
    [InlineData("rgb(255,0,0)", 255, 255, 0, 0)]
    [InlineData("rgb(10, 10, 10)", 255, 10, 10, 10)]
    [InlineData("rgb(100%, 0%, 0%)", 255, 255, 0, 0)]
    [InlineData("rgba(0,0,255,1)", 255, 0, 0, 255)]
    [InlineData("rgba(0,0,255,0)", 0, 0, 0, 255)]
    public void RgbFill_IsRenderedAsThatColor(
        string fill,
        byte a,
        byte r,
        byte g,
        byte b)
    {
        var drawing = Render($"""{SvgOpen}<rect width="10" height="10" fill="{fill}" /></svg>""");

        var brush = Assert.IsType<SolidColorBrush>(GeometryDrawings(drawing).Single().Brush);
        Assert.Equal(Color.FromArgb(a, r, g, b), brush.Color);
    }

    [StaFact]
    public void RgbaFill_HalfTransparent_HasHalfAlpha()
    {
        var drawing = Render($"""{SvgOpen}<rect width="10" height="10" fill="rgba(255,0,0,0.5)" /></svg>""");

        var brush = Assert.IsType<SolidColorBrush>(GeometryDrawings(drawing).Single().Brush);
        Assert.InRange(brush.Color.A, 127, 128);
        Assert.Equal((255, 0, 0), (brush.Color.R, brush.Color.G, brush.Color.B));
    }

    [StaFact]
    public void GradientStop_WithoutStopColor_IsBlack()
    {
        var drawing = Render(
            $$"""
            {{SvgOpen}}
              <defs>
                <linearGradient id="g"><stop offset="0" /><stop offset="1" stop-color="red" /></linearGradient>
              </defs>
              <rect width="10" height="10" fill="url(#g)" />
            </svg>
            """);

        var brush = Assert.IsType<LinearGradientBrush>(GeometryDrawings(drawing).Single().Brush);
        Assert.Equal(2, brush.GradientStops.Count);
        Assert.Equal(Colors.Black, brush.GradientStops[0].Color);
        Assert.Equal(Colors.Red, brush.GradientStops[1].Color);
    }

    [StaFact]
    public void GradientStop_WithInvalidStopColor_IsBlack()
    {
        var drawing = Render(
            $$"""
            {{SvgOpen}}
              <defs>
                <linearGradient id="g"><stop offset="0" stop-color="#12" /><stop offset="1" stop-color="red" /></linearGradient>
              </defs>
              <rect width="10" height="10" fill="url(#g)" />
            </svg>
            """);

        var brush = Assert.IsType<LinearGradientBrush>(GeometryDrawings(drawing).Single().Brush);
        Assert.Equal(Colors.Black, brush.GradientStops[0].Color);
    }

    [StaFact]
    public void SameFillColor_DifferentFillOpacity_EachShapeGetsItsOwnOpacity()
    {
        var drawing = Render(
            $$"""
            {{SvgOpen}}
              <rect width="5" height="10" fill="red" fill-opacity="0.2" />
              <rect x="5" width="5" height="10" fill="red" />
              <rect x="5" width="5" height="10" fill="red" fill-opacity="0.2" />
            </svg>
            """);

        var alphas = GeometryDrawings(drawing)
            .Select(x => ((SolidColorBrush)x.Brush).Color.A)
            .ToArray();
        Assert.Equal(new byte[] { 51, 255, 51 }, alphas);
    }

    private static DrawingGroup Render(string svg)
        => new SvgRender().LoadXmlDrawing(svg);

    private static IEnumerable<GeometryDrawing> GeometryDrawings(
        DrawingGroup group)
    {
        foreach (var child in group.Children)
        {
            switch (child)
            {
                case GeometryDrawing geometryDrawing:
                    yield return geometryDrawing;
                    break;
                case DrawingGroup childGroup:
                    foreach (var nested in GeometryDrawings(childGroup))
                    {
                        yield return nested;
                    }

                    break;
            }
        }
    }

    private static IEnumerable<DrawingGroup> DrawingGroups(DrawingGroup group)
    {
        foreach (var child in group.Children.OfType<DrawingGroup>())
        {
            yield return child;
            foreach (var nested in DrawingGroups(child))
            {
                yield return nested;
            }
        }
    }
}