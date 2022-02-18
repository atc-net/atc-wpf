namespace Atc.Wpf.Controls.W3cSvg.PaintServers;

internal class PatternPaintServer : PaintServer
{
    private readonly IList<Shape>? elements;
    private readonly IDictionary<string, PaintServer>? patternPaintServers;

    public PatternPaintServer(PaintServerManager owner, Svg svg, XmlNode node)
        : base(owner)
    {
        if (svg is null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        this.PatternUnits = SvgXmlUtil.AttrValue(node, "patternUnits", string.Empty);
        var transform = SvgXmlUtil.AttrValue(node, "patternTransform", string.Empty);
        if (!string.IsNullOrEmpty(transform))
        {
            this.PatternTransform = ShapeUtil.ParseTransform(transform.ToLower(GlobalizationConstants.EnglishCultureInfo));
        }

        var tempSVG = new Svg(node, svg.ExternalFileLoader);
        elements = tempSVG.Elements;

        patternPaintServers = svg.PaintServers.GetServers();
        this.X = SvgXmlUtil.AttrValue(node, "x", 0, svg.Size.Width);
        this.Y = SvgXmlUtil.AttrValue(node, "y", 0, svg.Size.Height);
        this.Width = SvgXmlUtil.AttrValue(node, "width", 1, svg.Size.Width);
        this.Height = SvgXmlUtil.AttrValue(node, "height", 1, svg.Size.Height);
    }

    public PatternPaintServer(PaintServerManager owner, Brush newBrush)
        : base(owner)
    {
        this.Brush = newBrush;
    }

    public double X { get; }

    public double Y { get; }

    public double Width { get; }

    public double Height { get; }

    public Transform? PatternTransform { get; }

    public string? PatternUnits { get; }

    public override Brush GetBrush(double opacity, Svg svg, SvgRender svgRender, Rect bounds)
    {
        if (svgRender is null)
        {
            throw new ArgumentNullException(nameof(svgRender));
        }

        if (this.Brush is not null)
        {
            return this.Brush;
        }

        if (patternPaintServers is not null &&
            svgRender.Svg is not null)
        {
            foreach (var (key, value) in patternPaintServers)
            {
                svgRender.Svg.PaintServers.AddServer(key, value);
            }
        }

        var brush = new DrawingBrush
        {
            Drawing = svgRender.LoadGroup(elements, viewBox: null, isSwitch: false),
            TileMode = TileMode.Tile,
            Transform = PatternTransform,
            Viewport = new Rect(X, Y, Width / bounds.Width, Height / bounds.Height),
            ViewportUnits = BrushMappingMode.RelativeToBoundingBox,
        };

        this.Brush = brush;
        return brush;
    }

    public override string ToString() => $"{nameof(elements)}: {elements}, {nameof(patternPaintServers)}: {patternPaintServers}, {nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(PatternTransform)}: {PatternTransform}, {nameof(PatternUnits)}: {PatternUnits}";
}