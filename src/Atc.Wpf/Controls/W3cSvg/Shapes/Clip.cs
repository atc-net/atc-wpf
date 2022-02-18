namespace Atc.Wpf.Controls.W3cSvg.Shapes;

internal class Clip : Group
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
            if (this.clpGeo != null)
            {
                return this.clpGeo;
            }

            var retVal = GetGeometryForShape(this.Elements[0]);
            if (this.Elements.Count > 1)
            {
                retVal = this.Elements.Aggregate(retVal, (current, element) => new CombinedGeometry(current, GetGeometryForShape(element)));
            }

            this.clpGeo = retVal;
            return this.clpGeo;
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