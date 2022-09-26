namespace Atc.Wpf.Controls.Media.W3cSvg.Shapes;

internal class UseShape : Shape
{
    public UseShape(Svg svg, XmlNode node)
        : base(svg, node)
    {
    }

    public double X { get; set; }

    public double Y { get; set; }

    public string? Href { get; set; }

    protected override void Parse(Svg svg, string name, string value)
    {
        ArgumentNullException.ThrowIfNull(svg);
        ArgumentNullException.ThrowIfNull(name);

        if (name.Contains(':', StringComparison.Ordinal))
        {
            name = name.Split(':')[1];
        }

        switch (name)
        {
            case SvgTagConstants.Href:
            {
                Href = value;
                if (Href.StartsWith('#'))
                {
                    Href = Href.Substring(1);
                }

                return;
            }

            case "x":
                X = SvgXmlUtil.GetDoubleValue(value, svg.Size.Width);
                return;
            case "y":
                Y = SvgXmlUtil.GetDoubleValue(value, svg.Size.Height);
                return;
            default:
                base.Parse(svg, name, value);
                break;
        }
    }

    public override string ToString() => $"{base.ToString()}, {nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Href)}: {Href}";
}