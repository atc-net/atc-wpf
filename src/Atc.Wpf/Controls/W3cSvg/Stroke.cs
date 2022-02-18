namespace Atc.Wpf.Controls.W3cSvg;

internal class Stroke
{
    public Stroke()
    {
        this.Width = 1;
        this.LineCap = StrokeLineCapType.Butt;
        this.LineJoin = StrokeLineJoinType.Miter;
        this.Opacity = 100;
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
        if (svg == null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        if (shape == null)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        var paintServer = svg.PaintServers.GetServer(PaintServerKey);
        if (paintServer == null)
        {
            return null;
        }

        if (paintServer is CurrentColorPaintServer)
        {
            var shapePaintServer = svg.PaintServers.GetServer(shape.PaintServerKey);
            if (shapePaintServer != null)
            {
                return shapePaintServer.GetBrush(this.Opacity * elementOpacity, svg, svgRender, bounds);
            }
        }

        if (paintServer is not InheritPaintServer)
        {
            return paintServer.GetBrush(this.Opacity * elementOpacity, svg, svgRender, bounds);
        }

        var p = shape.RealParent ?? shape.Parent;
        while (p != null)
        {
            if (p.Stroke != null)
            {
                var checkPaintServer = svg.PaintServers.GetServer(p.Stroke.PaintServerKey);
                if (checkPaintServer is not null &&
                    !(checkPaintServer is InheritPaintServer))
                {
                    return checkPaintServer.GetBrush(this.Opacity * elementOpacity, svg, svgRender, bounds);
                }
            }

            p = p.RealParent ?? p.Parent;
        }

        return null;
    }
}