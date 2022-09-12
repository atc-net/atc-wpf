namespace Atc.Wpf.Controls.W3cSvg.Shapes;

internal class PolylineShape : Shape
{
    public PolylineShape(Svg svg, XmlNode node)
        : base(svg, node)
    {
        var list = new List<Point>();
        var points = SvgXmlUtil.AttrValue(node, SvgTagConstants.Points, string.Empty);
        if (points is not null)
        {
            var split = new StringSplitter(points);
            while (split.More)
            {
                list.Add(split.ReadNextPoint());
            }
        }

        Points = list.ToArray();
    }

    public Point[] Points { get; }

    public override string ToString() => $"{base.ToString()}, {nameof(Points)}: {Points}";
}