namespace Atc.Wpf.Controls.W3cSvg.PaintServers;

internal class RadialGradientColorPaintServer : GradientColorPaintServer
{
    public RadialGradientColorPaintServer(PaintServerManager owner, XmlNode node)
        : base(owner, node)
    {
        this.Cx = SvgXmlUtil.AttrValue(node, "cx", 0.5);
        this.Cy = SvgXmlUtil.AttrValue(node, "cy", 0.5);
        this.Fx = SvgXmlUtil.AttrValue(node, "fx", this.Cx);
        this.Fy = SvgXmlUtil.AttrValue(node, "fy", this.Cy);
        this.R = SvgXmlUtil.AttrValue(node, "r", 0.5);
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
        if (this.Brush != null)
        {
            return this.Brush;
        }

        var brush = new RadialGradientBrush();
        foreach (GradientStop stop in this.Stops)
        {
            brush.GradientStops.Add(stop);
        }

        brush.GradientOrigin = new Point(0.5, 0.5);
        brush.Center = new Point(0.5, 0.5);
        brush.RadiusX = 0.5;
        brush.RadiusY = 0.5;

        if (this.GradientUnits == SvgTagConstants.UserSpaceOnUse)
        {
            brush.Center = new Point(this.Cx, this.Cy);
            brush.GradientOrigin = new Point(this.Fx, this.Fy);
            brush.RadiusX = this.R;
            brush.RadiusY = this.R;
            brush.MappingMode = BrushMappingMode.Absolute;
        }
        else
        {
            const double scale = 1d / 100d;
            if (!double.IsNaN(this.Cx) && !double.IsNaN(this.Cy))
            {
                brush.Center = new Point(this.Cx, this.Cy);
            }

            if (!double.IsNaN(this.Fx) && !double.IsNaN(this.Fy))
            {
                brush.GradientOrigin = new Point(this.Fx * scale, this.Fy * scale);
            }

            if (!double.IsNaN(this.R))
            {
                brush.RadiusX = this.R;
                brush.RadiusY = this.R;
            }

            brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
        }

        if (this.Transform != null)
        {
            brush.Transform = this.Transform;
        }

        this.Brush = brush;

        return brush;
    }

    [SuppressMessage("Design", "MA0076:Do not use implicit culture-sensitive ToString in interpolated strings", Justification = "OK.")]
    public override string ToString() => $"{nameof(Cx)}: {Cx}, {nameof(Cy)}: {Cy}, {nameof(Fx)}: {Fx}, {nameof(Fy)}: {Fy}, {nameof(R)}: {R}";
}