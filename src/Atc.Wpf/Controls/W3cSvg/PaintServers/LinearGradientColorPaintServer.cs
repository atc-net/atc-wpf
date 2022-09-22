namespace Atc.Wpf.Controls.W3cSvg.PaintServers;

internal class LinearGradientColorPaintServer : GradientColorPaintServer
{
    public LinearGradientColorPaintServer(PaintServerManager owner, XmlNode node)
        : base(owner, node)
    {
        X1 = SvgXmlUtil.AttrValue(node, "x1", double.NaN);
        Y1 = SvgXmlUtil.AttrValue(node, "y1", double.NaN);
        X2 = SvgXmlUtil.AttrValue(node, "x2", double.NaN);
        Y2 = SvgXmlUtil.AttrValue(node, "y2", double.NaN);
    }

    public LinearGradientColorPaintServer(PaintServerManager owner, Brush newBrush)
        : base(owner)
    {
        Brush = newBrush;
    }

    public double X1 { get; private set; }

    public double Y1 { get; private set; }

    public double X2 { get; private set; }

    public double Y2 { get; private set; }

    public override Brush GetBrush(double opacity, Svg svg, SvgRender svgRender, Rect bounds)
    {
        if (Brush is not null)
        {
            return Brush;
        }

        var brush = new LinearGradientBrush();
        foreach (var stop in Stops)
        {
            brush.GradientStops.Add(stop);
        }

        brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
        brush.StartPoint = new Point(0, 0);
        brush.EndPoint = new Point(1, 0);

        if (GradientUnits == SvgTagConstants.UserSpaceOnUse)
        {
            var tr = Transform;
            if (tr is not null)
            {
                brush.StartPoint = tr.Transform(new Point(X1, Y1));
                brush.EndPoint = tr.Transform(new Point(X2, Y2));
            }
            else
            {
                brush.StartPoint = new Point(X1, Y1);
                brush.EndPoint = new Point(X2, Y2);
            }

            Transform = null;
            brush.MappingMode = BrushMappingMode.Absolute;
        }
        else
        {
            Normalize();
            if (!double.IsNaN(X1))
            {
                brush.StartPoint = new Point(X1, Y1);
            }

            if (!double.IsNaN(X2))
            {
                brush.EndPoint = new Point(X2, Y2);
            }
        }

        Brush = brush;

        return brush;
    }

    private void Normalize()
    {
        if (!double.IsNaN(X1) && !double.IsNaN(X2))
        {
            var min = X1;
            if (X2 < X1)
            {
                min = X2;
            }

            X1 -= min;
            X2 -= min;
            var scale = X1;
            if (X2 > X1)
            {
                scale = X2;
            }

            if (scale != 0)
            {
                X1 /= scale;
                X2 /= scale;
            }
        }

        if (!double.IsNaN(Y1) && !double.IsNaN(Y2))
        {
            var min = Y1;
            if (Y2 < Y1)
            {
                min = Y2;
            }

            Y1 -= min;
            Y2 -= min;
            var scale = Y1;
            if (Y2 > Y1)
            {
                scale = Y2;
            }

            if (scale != 0)
            {
                Y1 /= scale;
                Y2 /= scale;
            }
        }
    }

    public override string ToString() => $"{nameof(X1)}: {X1}, {nameof(Y1)}: {Y1}, {nameof(X2)}: {X2}, {nameof(Y2)}: {Y2}";
}