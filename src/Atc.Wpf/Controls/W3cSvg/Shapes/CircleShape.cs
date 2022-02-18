namespace Atc.Wpf.Controls.W3cSvg.Shapes;

internal class CircleShape : Shape
{
    public CircleShape(Svg svg, XmlNode node)
        : base(svg, node)
    {
        if (svg == null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        this.Cx = SvgXmlUtil.AttrValue(node, "cx", 0, svg.ViewBox?.Width ?? svg.Size.Width);
        this.Cy = SvgXmlUtil.AttrValue(node, "cy", 0, svg.ViewBox?.Height ?? svg.Size.Height);
        var dRef = svg.ViewBox.HasValue
            ? ShapeUtil.CalculateDRef(svg.ViewBox.Value.Width, svg.ViewBox.Value.Height)
            : ShapeUtil.CalculateDRef(svg.Size.Width, svg.Size.Height);
        this.R = SvgXmlUtil.AttrValue(node, "r", 0, dRef);
    }

    public double Cx { get; }

    public double Cy { get; }

    public double R { get; }

    public override string ToString() => $"{base.ToString()}, {nameof(Cx)}: {Cx}, {nameof(Cy)}: {Cy}, {nameof(R)}: {R}";
}