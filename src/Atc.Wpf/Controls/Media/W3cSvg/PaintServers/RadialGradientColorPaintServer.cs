namespace Atc.Wpf.Controls.Media.W3cSvg.PaintServers;

internal sealed class RadialGradientColorPaintServer : GradientColorPaintServer
{
    public RadialGradientColorPaintServer(PaintServerManager owner, XmlNode node)
        : base(owner, node)
    {
        Cx = SvgXmlUtil.AttrValue(node, "cx", 0.5);
        Cy = SvgXmlUtil.AttrValue(node, "cy", 0.5);
        Fx = SvgXmlUtil.AttrValue(node, "fx", Cx);
        Fy = SvgXmlUtil.AttrValue(node, "fy", Cy);
        R = SvgXmlUtil.AttrValue(node, "r", 0.5);
    }

    public RadialGradientColorPaintServer(PaintServerManager owner, Brush newBrush)
        : base(owner)
    {
        Brush = newBrush;
    }

    public double Cx { get; }

    public double Cy { get; }

    public double Fx { get; }

    public double Fy { get; }

    public double R { get; }

    public override Brush GetBrush(double opacity, Svg svg, SvgRender svgRender, Rect bounds)
    {
        if (Brush is not null)
        {
            return Brush;
        }

        var brush = new RadialGradientBrush();
        foreach (var stop in Stops)
        {
            brush.GradientStops.Add(stop);
        }

        brush.GradientOrigin = new Point(0.5, 0.5);
        brush.Center = new Point(0.5, 0.5);
        brush.RadiusX = 0.5;
        brush.RadiusY = 0.5;

        if (GradientUnits == SvgTagConstants.UserSpaceOnUse)
        {
            brush.Center = new Point(Cx, Cy);
            brush.GradientOrigin = new Point(Fx, Fy);
            brush.RadiusX = R;
            brush.RadiusY = R;
            brush.MappingMode = BrushMappingMode.Absolute;
        }
        else
        {
            const double scale = 1d / 100d;
            if (!double.IsNaN(Cx) && !double.IsNaN(Cy))
            {
                brush.Center = new Point(Cx, Cy);
            }

            if (!double.IsNaN(Fx) && !double.IsNaN(Fy))
            {
                brush.GradientOrigin = new Point(Fx * scale, Fy * scale);
            }

            if (!double.IsNaN(R))
            {
                brush.RadiusX = R;
                brush.RadiusY = R;
            }

            brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
        }

        if (Transform is not null)
        {
            brush.Transform = Transform;
        }

        Brush = brush;

        return brush;
    }

    public override string ToString() => $"{nameof(Cx)}: {Cx}, {nameof(Cy)}: {Cy}, {nameof(Fx)}: {Fx}, {nameof(Fy)}: {Fy}, {nameof(R)}: {R}";
}