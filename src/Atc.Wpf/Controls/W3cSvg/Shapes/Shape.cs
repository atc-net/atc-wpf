// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable VirtualMemberNeverOverridden.Global
namespace Atc.Wpf.Controls.W3cSvg.Shapes;

[SuppressMessage("Critical Code Smell", "S1699:Constructors should only call non-overridable methods", Justification = "OK.")]
[SuppressMessage("Usage", "CA2214:Do not call overridable methods in constructors", Justification = "OK.")]
[SuppressMessage("Design", "MA0056:Do not call overridable members in constructor", Justification = "OK.")]
internal class Shape
{
    private Fill? fill;
    private Stroke? stroke;
    private TextStyle? textStyle;
    private string? localStyle;
    private double opacity;

    public Shape(Svg svg, XmlNode? node = null, Shape? parent = null)
    {
        if (node is null)
        {
            Id = "<null>";
        }
        else
        {
            var attrValue = SvgXmlUtil.AttrValue(node, "id");
            Id = attrValue ?? "<null>";
        }

        Opacity = 1;
        Parent = parent;
        ParseAtStart(svg, node);
        if (node?.Attributes is not null)
        {
            foreach (XmlAttribute? attr in node.Attributes)
            {
                if (attr is not null)
                {
                    Parse(svg, attr.Name, attr.Value);
                }
            }
        }

        ParseLocalStyle(svg);
    }

    public Shape(Svg svg, IReadOnlyCollection<KeyValueItem>? attrs, Shape parent)
    {
        Id = "<null>";

        Opacity = 1;
        Parent = parent;
        if (attrs is not null)
        {
            foreach (KeyValueItem attr in attrs)
            {
                Parse(svg, attr.Key, attr.Value);
            }
        }

        ParseLocalStyle(svg);
    }

    internal Clip? Clip { get; set; }

    internal Geometry? GeometryElement { get; set; }

    internal virtual Filter? Filter { get; private set; }

    internal Shape? RealParent { get; set; }

    public string Id { get; }

    public string? PaintServerKey { get; set; }

    public string? RequiredExtensions { get; set; }

    public string? RequiredFeatures { get; set; }

    public VisibilityType Visibility { get; set; }

    public bool Display { get; set; } = true;

    public virtual Stroke? Stroke
    {
        get
        {
            if (stroke is not null)
            {
                return stroke;
            }

            var parent = Parent;
            while (parent is not null)
            {
                if (parent.Stroke is not null)
                {
                    return parent.Stroke;
                }

                parent = parent.Parent;
            }

            return null;
        }
    }

    public virtual Fill? Fill
    {
        get
        {
            if (fill is not null)
            {
                return fill;
            }

            var parent = Parent;
            while (parent is not null)
            {
                if (parent.Fill is not null)
                {
                    return parent.Fill;
                }

                parent = parent.Parent;
            }

            return null;
        }
    }

    public virtual TextStyle? TextStyle
    {
        get
        {
            if (textStyle is not null)
            {
                return textStyle;
            }

            var parent = Parent;
            while (parent is not null)
            {
                if (parent.textStyle is not null)
                {
                    return parent.textStyle;
                }

                parent = parent.Parent;
            }

            return null;
        }
    }

    public double Opacity
    {
        get => Visibility == VisibilityType.Visible ? opacity : 0;
        set => opacity = value;
    }

    public virtual Transform? Transform { get; private set; }

    public Shape? Parent { get; set; }

    protected virtual void ParseAtStart(Svg svg, XmlNode? node)
    {
        if (svg is null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        if (node is null)
        {
            return;
        }

        var name = node.Name;
        if (name.Contains(':', StringComparison.Ordinal))
        {
            name = name.Split(':')[1];
        }

        if (svg.StyleItems.TryGetValue(name, out var attributes))
        {
            foreach (var xmlAttribute in attributes)
            {
                Parse(svg, xmlAttribute.Key, xmlAttribute.Value);
            }
        }

        if (!string.IsNullOrEmpty(Id) &&
            svg.StyleItems.TryGetValue("#" + Id, out attributes))
        {
            foreach (var xmlAttribute in attributes)
            {
                Parse(svg, xmlAttribute.Key, xmlAttribute.Value);
            }
        }
    }

    protected virtual void ParseLocalStyle(Svg svg)
    {
        if (string.IsNullOrEmpty(localStyle))
        {
            return;
        }

        foreach (KeyValueItem item in SvgXmlUtil.SplitStyle(localStyle))
        {
            Parse(svg, item.Key, item.Value);
        }
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - for now.")]
    protected virtual void Parse(Svg svg, string name, string value)
    {
        if (svg is null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        if (name is null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (name.Contains(':', StringComparison.Ordinal))
        {
            name = name.Split(':')[1];
        }

        switch (name)
        {
            case SvgTagConstants.Display when value == "none":
                Display = false;
                break;
            case SvgTagConstants.Class:
            {
                var classes = value.Split(' ');
                foreach (var @class in classes)
                {
                    if (!svg.StyleItems.TryGetValue("." + @class, out var attributes))
                    {
                        continue;
                    }

                    foreach (var xmlAttribute in attributes)
                    {
                        Parse(svg, xmlAttribute.Key, xmlAttribute.Value);
                    }
                }

                return;
            }
        }

        switch (name)
        {
            case SvgTagConstants.Transform:
                Transform = ShapeUtil.ParseTransform(value.ToLower(GlobalizationConstants.EnglishCultureInfo));
                return;
            case SvgTagConstants.Visibility:
                Visibility = value == "hidden"
                    ? VisibilityType.Hidden
                    : VisibilityType.Visible;
                break;
            case SvgTagConstants.Stroke:
                GetStroke().PaintServerKey = svg.PaintServers.Parse(value);
                return;
            case SvgTagConstants.StrokeWidth:
                GetStroke().Width = SvgXmlUtil.ParseDouble(value);
                return;
            case SvgTagConstants.StrokeOpacity:
                GetStroke().Opacity = SvgXmlUtil.ParseDouble(value) * 100;
                return;
            case SvgTagConstants.StrokeDashArray when value == "none":
                GetStroke().StrokeArray = null;
                return;
            case SvgTagConstants.StrokeDashArray:
            {
                var stringSplitter = new StringSplitter(value);
                var doubles = new List<double>();
                while (stringSplitter.More)
                {
                    doubles.Add(stringSplitter.ReadNextValue());
                }

                GetStroke().StrokeArray = doubles.ToArray();
                return;
            }

            case SvgTagConstants.RequiredFeatures:
                RequiredFeatures = value.Trim();
                return;
            case SvgTagConstants.RequiredExtensions:
                RequiredExtensions = value.Trim();
                return;
            case SvgTagConstants.StrokeLineCap:
                GetStroke().LineCap = Enum<StrokeLineCapType>.Parse(value);
                return;
            case SvgTagConstants.StrokeLineJoin:
                GetStroke().LineJoin = Enum<StrokeLineJoinType>.Parse(value);
                return;
            case SvgTagConstants.FilterProperty when value.StartsWith("url", StringComparison.Ordinal):
            {
                string id = ShapeUtil.ExtractBetween(value, '(', ')');
                if (id.Length > 0 && id[0] == '#')
                {
                    id = id.Substring(1);
                }

                svg.Shapes.TryGetValue(id, out var shape);
                Filter = shape as Filter;
                return;
            }

            case SvgTagConstants.FilterProperty:
                return;
            case SvgTagConstants.ClipPathProperty when value.StartsWith("url", StringComparison.Ordinal):
            {
                string id = ShapeUtil.ExtractBetween(value, '(', ')');
                if (id.Length > 0 && id[0] == '#')
                {
                    id = id.Substring(1);
                }

                svg.Shapes.TryGetValue(id, out var shape);
                Clip = shape as Clip;
                return;
            }

            case SvgTagConstants.ClipPathProperty:
                return;
            case SvgTagConstants.Fill:
                GetFill().PaintServerKey = svg.PaintServers.Parse(value);
                return;
            case SvgTagConstants.Color:
                PaintServerKey = svg.PaintServers.Parse(value);
                return;
            case SvgTagConstants.FillOpacity:
                GetFill().Opacity = SvgXmlUtil.ParseDouble(value) * 100;
                return;
            case SvgTagConstants.FillRule:
                GetFill().FillRule = Enum<FillRuleType>.Parse(value);
                return;
            case SvgTagConstants.Opacity:
                Opacity = SvgXmlUtil.ParseDouble(value);
                return;
            case SvgTagConstants.Style:
                localStyle = value;
                break;
            case SvgTagConstants.FontFamily:
                GetTextStyle().FontFamily = value;
                return;
            case SvgTagConstants.FontSize:
                GetTextStyle().FontSize = SvgXmlUtil.AttrValue(new KeyValueItem(name, value));
                return;
            case SvgTagConstants.FontWeight:
                var convertFontWeight = new FontWeightConverter().ConvertFromString(value);
                if (convertFontWeight is not null)
                {
                    GetTextStyle().FontWeight = (FontWeight)convertFontWeight;
                }

                return;
            case SvgTagConstants.FontStyle:
                var convertFontStyle = new FontStyleConverter().ConvertFromString(value);
                if (convertFontStyle is not null)
                {
                    GetTextStyle().FontStyle = (FontStyle)convertFontStyle;
                }

                return;
            case SvgTagConstants.TextDecoration:
            {
                TextDecoration t = new TextDecoration();
                switch (value)
                {
                    case "none":
                        return;
                    case "underline":
                        t.Location = TextDecorationLocation.Underline;
                        break;
                    case "overline":
                        t.Location = TextDecorationLocation.OverLine;
                        break;
                    case "line-through":
                        t.Location = TextDecorationLocation.Strikethrough;
                        break;
                }

                var tt = new TextDecorationCollection
                {
                    t,
                };

                GetTextStyle().TextDecoration = tt;
                return;
            }

            case SvgTagConstants.TextAnchor:
            {
                GetTextStyle().TextAlignment = value switch
                {
                    "start" => TextAlignment.Left,
                    "middle" => TextAlignment.Center,
                    "end" => TextAlignment.Right,
                    _ => GetTextStyle().TextAlignment
                };

                return;
            }

            case "word-spacing":
                GetTextStyle().WordSpacing = SvgXmlUtil.AttrValue(new KeyValueItem(name, value));
                return;
            case "letter-spacing":
                GetTextStyle().LetterSpacing = SvgXmlUtil.AttrValue(new KeyValueItem(name, value));
                return;
            case "baseline-shift":
                GetTextStyle().BaseLineShift = value;
                return;
        }
    }

    protected Fill GetFill() => fill ??= new Fill();

    protected TextStyle GetTextStyle() => textStyle ??= new TextStyle(this);

    public override string ToString()
    {
        return GetType().Name + " (" + Id + ")";
    }

    private Stroke GetStroke() => stroke ??= new Stroke();
}