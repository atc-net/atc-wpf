namespace Atc.Wpf.Controls.Media.W3cSvg.Shapes;

internal sealed class Clip : Group
{
    public Clip(Svg svg, XmlNode node, Shape parent)
        : base(svg, node, parent)
    {
    }

    private Geometry? clpGeo;

    public Geometry? ClipGeometry
    {
        get
        {
            if (clpGeo is not null)
            {
                return clpGeo;
            }

            var retVal = GetGeometryForShape(Elements[0]);
            if (Elements.Count > 1)
            {
                retVal = Elements.Aggregate(retVal, (current, element) => new CombinedGeometry(current, GetGeometryForShape(element)));
            }

            clpGeo = retVal;
            return clpGeo;
        }
    }

    private static Geometry? GetGeometryForShape(Shape shape)
    {
        switch (shape)
        {
            case RectangleShape rectangle:
            {
                var r = rectangle;
                var g = new RectangleGeometry(new Rect(r.Rx, r.Ry, r.Width, r.Height));
                r.GeometryElement = g;
                return g;
            }

            case CircleShape circle:
            {
                var c = circle;
                var g = new EllipseGeometry(new Point(c.Cx, c.Cy), c.R, c.R);
                c.GeometryElement = g;
                return g;
            }

            default:
                return null;
        }
    }
}