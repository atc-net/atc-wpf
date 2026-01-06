namespace Atc.Wpf.Controls.Media.W3cSvg.Shapes;

internal sealed class RectangleShape : Shape
{
    private static Fill? defaultFill;

    [SuppressMessage("Major Code Smell", "S3010:Static fields should not be updated in constructors", Justification = "OK.")]
    public RectangleShape(
        Svg svg,
        XmlNode node)
        : base(
            svg,
            node)
    {
        ArgumentNullException.ThrowIfNull(svg);

        defaultFill ??= new Fill
        {
            PaintServerKey = svg.PaintServers.Parse("black"),
        };
    }

    public double X { get; set; }

    public double Y { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    public double Rx { get; set; }

    public double Ry { get; set; }

    public override Fill? Fill => base.Fill ?? defaultFill;

    protected override void Parse(
        Svg svg,
        string name,
        string value)
    {
        ArgumentNullException.ThrowIfNull(svg);
        ArgumentNullException.ThrowIfNull(name);

        if (name.Contains(':', StringComparison.Ordinal))
        {
            name = name.Split(':')[1];
        }

        switch (name)
        {
            case "x":
                X = SvgXmlUtil.GetDoubleValue(value, svg.Size.Width);
                return;
            case "y":
                Y = SvgXmlUtil.GetDoubleValue(value, svg.Size.Height);
                return;
            case "width":
                Width = SvgXmlUtil.GetDoubleValue(value, svg.Size.Width);
                return;
            case "height":
                Height = SvgXmlUtil.GetDoubleValue(value, svg.Size.Height);
                return;
            case "rx":
                Rx = SvgXmlUtil.GetDoubleValue(value, svg.Size.Width);
                return;
            case "ry":
                Ry = SvgXmlUtil.GetDoubleValue(value, svg.Size.Height);
                return;
            default:
                base.Parse(
                    svg,
                    name,
                    value);
                break;
        }
    }

    public override string ToString()
        => $"{base.ToString()}, {nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(Rx)}: {Rx}, {nameof(Ry)}: {Ry}";
}