namespace Atc.Wpf.Controls.W3cSvg.Shapes;

internal class PolygonShape : Shape
{
    private static Fill? defaultFill;

    [SuppressMessage("Major Code Smell", "S3010:Static fields should not be updated in constructors", Justification = "OK.")]
    public PolygonShape(Svg svg, XmlNode node)
        : base(svg, node)
    {
        if (svg is null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        defaultFill ??= new Fill
        {
            PaintServerKey = svg.PaintServers.Parse("black"),
        };

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

        this.Points = list.ToArray();
    }

    public Point[] Points { get; }

    public override Fill? Fill => base.Fill ?? defaultFill;

    public override string ToString() => $"{base.ToString()}, {nameof(Points)}: {Points}, {nameof(Fill)}: {Fill}";
}