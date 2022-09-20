namespace Atc.Wpf.Controls.W3cSvg;

internal class Stroke
{
    public Stroke()
    {
        Width = 1;
        LineCap = StrokeLineCapType.Butt;
        LineJoin = StrokeLineJoinType.Miter;
        Opacity = 100;
    }

    public string? PaintServerKey { get; set; }

    public double Width { get; set; }

    public double Opacity { get; set; }

    public StrokeLineCapType LineCap { get; set; }

    public StrokeLineJoinType LineJoin { get; set; }

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1011:Closing square brackets should be spaced correctly", Justification = "OK.")]
    public double[]? StrokeArray { get; set; }

    public Brush? StrokeBrush(Svg svg, SvgRender svgRender, Shape shape, double elementOpacity, Rect bounds)
    {
        ArgumentNullException.ThrowIfNull(svg);
        ArgumentNullException.ThrowIfNull(shape);

        if (PaintServerKey is null)
        {
            return null;
        }

        var paintServer = svg.PaintServers.GetServer(PaintServerKey);
        if (paintServer is null)
        {
            return null;
        }

        if (paintServer is CurrentColorPaintServer && shape.PaintServerKey is not null)
        {
            var shapePaintServer = svg.PaintServers.GetServer(shape.PaintServerKey);
            if (shapePaintServer is not null)
            {
                return shapePaintServer.GetBrush(Opacity * elementOpacity, svg, svgRender, bounds);
            }
        }

        if (paintServer is not InheritPaintServer)
        {
            return paintServer.GetBrush(Opacity * elementOpacity, svg, svgRender, bounds);
        }

        var p = shape.RealParent ?? shape.Parent;
        while (p is not null)
        {
            if (p.Stroke?.PaintServerKey is not null)
            {
                var checkPaintServer = svg.PaintServers.GetServer(p.Stroke.PaintServerKey);
                if (checkPaintServer is not null &&
                    checkPaintServer is not InheritPaintServer)
                {
                    return checkPaintServer.GetBrush(Opacity * elementOpacity, svg, svgRender, bounds);
                }
            }

            p = p.RealParent ?? p.Parent;
        }

        return null;
    }
}