namespace Atc.Wpf.Controls.W3cSvg.Shapes;

internal class Group : Shape
{
    private readonly List<Shape> elements = new List<Shape>();

    public Group(Svg svg, XmlNode node, Shape parent)
        : base(svg, node)
    {
        if (svg == null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        var clp = SvgXmlUtil.AttrValue(node, "clip-path", defaultValue: null);
        if (!string.IsNullOrEmpty(clp))
        {
            string id = ShapeUtil.ExtractBetween(clp, '(', ')');
            if (id.Length > 0 && id[0] == '#')
            {
                id = id.Substring(1);
            }

            svg.Shapes.TryGetValue(id, out var shape);
            this.Clip = shape as Clip;
        }

        this.Parent = parent;
        foreach (XmlNode childNode in node.ChildNodes)
        {
            var shape = AddToList(svg, this.elements, childNode, this);
            if (shape != null)
            {
                shape.Parent = this;
            }
        }
    }

    public IList<Shape> Elements => this.elements.AsReadOnly();

    public bool IsSwitch { get; init; }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    public static Shape? AddToList(Svg svg, List<Shape> list, XmlNode childNode, Shape? parent)
    {
        if (svg == null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (childNode == null)
        {
            throw new ArgumentNullException(nameof(childNode));
        }

        if (childNode.NodeType != XmlNodeType.Element)
        {
            return null;
        }

        Shape? retVal = null;
        var nodeName = GetNodeName(childNode);
        if (nodeName == null)
        {
            return null;
        }

        switch (nodeName)
        {
            case SvgTagConstants.Style:
                CssStyleParser.ParseStyle(svg, childNode.InnerText);
                break;
            case SvgTagConstants.ShapeRect:
                retVal = new RectangleShape(svg, childNode);
                break;
            case SvgTagConstants.Filter:
                retVal = new Filter(svg, childNode, parent);
                break;
            case SvgTagConstants.FeGaussianBlur:
                retVal = new FilterFeGaussianBlur(svg, childNode, parent);
                break;
            case SvgTagConstants.ShapeCircle:
                retVal = new CircleShape(svg, childNode);
                break;
            case SvgTagConstants.ShapeEllipse:
                retVal = new EllipseShape(svg, childNode);
                break;
            case SvgTagConstants.ShapeLine:
                retVal = new LineShape(svg, childNode);
                break;
            case SvgTagConstants.ShapePolyline:
                retVal = new PolylineShape(svg, childNode);
                break;
            case SvgTagConstants.ShapePolygon:
                retVal = new PolygonShape(svg, childNode);
                break;
            case SvgTagConstants.ShapePath:
                retVal = new PathShape(svg, childNode, parent);
                break;
            case SvgTagConstants.ClipPath:
                retVal = new Clip(svg, childNode, parent);
                break;
            case SvgTagConstants.ShapeGroup:
                retVal = new Group(svg, childNode, parent);
                break;
            case SvgTagConstants.Switch:
                retVal = new Group(svg, childNode, parent)
                {
                    IsSwitch = true,
                };
                break;
            case SvgTagConstants.ShapeUse:
                retVal = new UseShape(svg, childNode);
                break;
            case SvgTagConstants.ShapeImage:
                retVal = new ImageShape(svg, childNode);
                break;
            case SvgTagConstants.Animate:
                retVal = new Animate(svg, childNode, parent);
                break;
            case SvgTagConstants.AnimateColor:
                retVal = new AnimateColor(svg, childNode, parent);
                break;
            case SvgTagConstants.AnimateMotion:
                retVal = new AnimateMotion(svg, childNode, parent);
                break;
            case SvgTagConstants.AnimateTransform:
                retVal = new AnimateTransform(svg, childNode, parent);
                break;
            case SvgTagConstants.Text:
                retVal = new TextShape(svg, childNode, parent);
                break;
            case SvgTagConstants.LinearGradient:
                svg.PaintServers.Create(svg, childNode);
                return null;
            case SvgTagConstants.RadialGradient:
                svg.PaintServers.Create(svg, childNode);
                return null;
            case SvgTagConstants.Definitions:
                ReadDefs(svg, list, childNode);
                return null;
            case SvgTagConstants.Symbol:
                retVal = new Group(svg, childNode, parent);
                break;
        }

        if (retVal != null)
        {
            list.Add(retVal);
            if (retVal.Id.Length > 0)
            {
                svg.AddShape(retVal.Id, retVal);
            }
        }

        return nodeName == SvgTagConstants.Symbol
            ? null
            : retVal;
    }

    private static void ReadDefs(Svg svg, List<Shape> list, XmlNode node)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        list = new List<Shape>();
        foreach (XmlNode childNode in node.ChildNodes)
        {
            var nodeName = GetNodeName(childNode);
            if (nodeName == SvgTagConstants.LinearGradient ||
                nodeName == SvgTagConstants.RadialGradient ||
                nodeName == SvgTagConstants.Pattern)
            {
                svg.PaintServers.Create(svg, childNode);
                continue;
            }

            AddToList(svg, list, childNode, parent: null);
        }
    }

    private static string? GetNodeName(XmlNode node)
    {
        var nodeName = node.Name;
        if (nodeName.Contains(':', StringComparison.Ordinal))
        {
            var parts = nodeName.Split(':');
            var ns = node.GetNamespaceOfPrefix(parts[0]);
            return ns == SvgTagConstants.NsSvg
                ? parts[1]
                : null;
        }

        return node.NamespaceURI == SvgTagConstants.NsSvg || string.IsNullOrEmpty(node.NamespaceURI)
            ? nodeName
            : null;
    }
}