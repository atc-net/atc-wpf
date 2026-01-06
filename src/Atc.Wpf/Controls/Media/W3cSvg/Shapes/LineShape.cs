namespace Atc.Wpf.Controls.Media.W3cSvg.Shapes;

internal sealed class LineShape : Shape
{
    public LineShape(
        Svg svg,
        XmlNode node)
        : base(svg, node)
    {
        ArgumentNullException.ThrowIfNull(svg);

        var x1 = SvgXmlUtil.AttrValue(node, "x1", 0, svg.Size.Width);
        var y1 = SvgXmlUtil.AttrValue(node, "y1", 0, svg.Size.Height);
        var x2 = SvgXmlUtil.AttrValue(node, "x2", 0, svg.Size.Width);
        var y2 = SvgXmlUtil.AttrValue(node, "y2", 0, svg.Size.Height);
        P1 = new Point(x1, y1);
        P2 = new Point(x2, y2);
    }

    public Point P1 { get; }

    public Point P2 { get; }

    public override string ToString()
        => $"{base.ToString()}, {nameof(P1)}: ({P1}), {nameof(P2)}: ({P2})";
}