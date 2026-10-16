namespace Atc.Wpf.Controls.Media.W3cSvg.Shapes;

internal sealed class Clip : Group
{
    public Clip(
        Svg svg,
        XmlNode node,
        Shape parent)
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

            if (Elements.Count == 0)
            {
                clpGeo = Geometry.Empty;
                return clpGeo;
            }

            var retVal = GetGeometryForShape(Elements[0]);
            if (Elements.Count > 1)
            {
                retVal = Elements.Skip(1).Aggregate(retVal, (current, element) => new CombinedGeometry(current, GetGeometryForShape(element)));
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
                if (r.Width <= 0 || r.Height <= 0)
                {
                    return Geometry.Empty;
                }

                var g = new RectangleGeometry(new Rect(r.X, r.Y, r.Width, r.Height), r.Rx, r.Ry);
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