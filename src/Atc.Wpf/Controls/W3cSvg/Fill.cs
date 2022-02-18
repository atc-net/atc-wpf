namespace Atc.Wpf.Controls.W3cSvg;

internal class Fill
{
    public Fill()
    {
        this.FillRule = FillRuleType.NonZero;
        this.Opacity = 100;
    }

    public FillRuleType FillRule { get; set; }

    public string? PaintServerKey { get; set; }

    public double Opacity { get; set; }

    public Brush? FillBrush(Svg svg, SvgRender svgRender, Shape shape, double elementOpacity, Rect bounds)
    {
        if (svg is null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        if (shape is null)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        var paintServer = svg.PaintServers.GetServer(PaintServerKey);
        if (paintServer is null)
        {
            return null;
        }

        if (paintServer is CurrentColorPaintServer)
        {
            var shapePaintServer = svg.PaintServers.GetServer(shape.PaintServerKey);
            if (shapePaintServer is not null)
            {
                return shapePaintServer.GetBrush(this.Opacity * elementOpacity, svg, svgRender, bounds);
            }
        }

        if (paintServer is not InheritPaintServer)
        {
            return paintServer.GetBrush(this.Opacity * elementOpacity, svg, svgRender, bounds);
        }

        var p = shape.RealParent;
        while (p is not null)
        {
            if (p.Fill is not null)
            {
                var checkPaintServer = svg.PaintServers.GetServer(p.Fill.PaintServerKey);
                if (checkPaintServer is not null && !(checkPaintServer is InheritPaintServer))
                {
                    return checkPaintServer.GetBrush(this.Opacity * elementOpacity, svg, svgRender, bounds);
                }
            }

            p = p.RealParent;
        }

        return null;
    }
}