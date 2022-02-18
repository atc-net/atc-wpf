namespace Atc.Wpf.Controls.W3cSvg.Shapes;

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
        if (svg == null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (name.Contains(':', StringComparison.Ordinal))
        {
            name = name.Split(':')[1];
        }

        switch (name)
        {
            case SvgTagConstants.Href:
            {
                this.Href = value;
                if (this.Href.StartsWith('#'))
                {
                    this.Href = this.Href.Substring(1);
                }

                return;
            }

            case "x":
                this.X = SvgXmlUtil.GetDoubleValue(value, svg.Size.Width);
                return;
            case "y":
                this.Y = SvgXmlUtil.GetDoubleValue(value, svg.Size.Height);
                return;
            default:
                base.Parse(svg, name, value);
                break;
        }
    }

    [SuppressMessage("Design", "MA0076:Do not use implicit culture-sensitive ToString in interpolated strings", Justification = "OK.")]
    public override string ToString() => $"{base.ToString()}, {nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Href)}: {Href}";
}