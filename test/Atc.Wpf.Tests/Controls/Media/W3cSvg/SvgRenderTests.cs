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

    [StaTheory]
    [InlineData("", 0)]
    [InlineData("indefinite", 0)]
    [InlineData("0:00:05", 0)]
    [InlineData("500ms", 500)]
    [InlineData("2s", 2000)]
    [InlineData("2", 2000)]
    public void AnimateDuration_IsParsedOrFallsBackToZero(
        string dur,
        int expectedMilliseconds)
    {
        var durAttribute = string.IsNullOrEmpty(dur)
            ? string.Empty
            : $"""dur="{dur}" """;
        var svgRender = new SvgRender();

        svgRender.LoadXmlDrawing(
            $$"""
            {{SvgOpen}}
              <g>
                <circle id="c" cx="5" cy="5" r="2" />
                <animate xlink:href="#c" attributeName="r" values="2;4;2" {{durAttribute}}repeatCount="indefinite" />
              </g>
            </svg>
            """);

        var group = Assert.IsType<Group>(svgRender.Svg!.Elements.Single());
        var animate = Assert.Single(group.Elements.OfType<Animate>());
        Assert.Equal(TimeSpan.FromMilliseconds(expectedMilliseconds), animate.Duration);
    }

    [StaTheory]
    [InlineData("0 0 24 24")]
    [InlineData("0,0,24,24")]
    [InlineData("0, 0, 24, 24")]
    [InlineData("0 0  24 24")]
    [InlineData("0\t0\n24\r\n24")]
    public void ViewBox_SeparatedByWhitespaceOrComma_IsParsed(string viewBox)
    {
        var svgRender = new SvgRender();

        svgRender.LoadXmlDrawing($"""<svg viewBox="{viewBox}" xmlns="http://www.w3.org/2000/svg"><rect width="10" height="10" /></svg>""");

        Assert.Equal(new Rect(0, 0, 24, 24), svgRender.Svg!.ViewBox);
    }

    [StaTheory]
    [InlineData("0 0 24")]
    [InlineData("0 0 24 24 1")]
    [InlineData("a b c d")]
    public void ViewBox_NotFourNumbers_IsIgnored(string viewBox)
    {
        var svgRender = new SvgRender();

        svgRender.LoadXmlDrawing($"""<svg viewBox="{viewBox}" xmlns="http://www.w3.org/2000/svg"><rect width="10" height="10" /></svg>""");

        Assert.Null(svgRender.Svg!.ViewBox);
    }

    [StaFact]
    public void RadialGradient_FocalPoint_IsNotScaled()
    {
        var drawing = Render(
            $$"""
            {{SvgOpen}}
              <defs><radialGradient id="g" fx="0.3" fy="0.7"><stop offset="0" stop-color="red" /><stop offset="1" stop-color="blue" /></radialGradient></defs>
              <rect width="10" height="10" fill="url(#g)" />
            </svg>
            """);

        var brush = Assert.IsType<RadialGradientBrush>(GeometryDrawings(drawing).Single().Brush);
        Assert.Equal(new Point(0.3, 0.7), brush.GradientOrigin);
        Assert.Equal(new Point(0.5, 0.5), brush.Center);
    }

    [StaFact]
    public void RadialGradient_DefaultFocalPoint_IsTheCenter()
    {
        var drawing = Render(
            $$"""
            {{SvgOpen}}
              <defs><radialGradient id="g" cx="0.4" cy="0.6"><stop offset="0" stop-color="red" /><stop offset="1" stop-color="blue" /></radialGradient></defs>
              <rect width="10" height="10" fill="url(#g)" />
            </svg>
            """);

        var brush = Assert.IsType<RadialGradientBrush>(GeometryDrawings(drawing).Single().Brush);
        Assert.Equal(new Point(0.4, 0.6), brush.GradientOrigin);
        Assert.Equal(new Point(0.4, 0.6), brush.Center);
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