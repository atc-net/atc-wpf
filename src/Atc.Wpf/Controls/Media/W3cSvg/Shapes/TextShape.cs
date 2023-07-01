namespace Atc.Wpf.Controls.Media.W3cSvg.Shapes;

internal sealed class TextShape : Shape
{
    private static Fill? defaultFill;
    private static Stroke? defaultStroke;

    [SuppressMessage("Major Code Smell", "S3010:Static fields should not be updated in constructors", Justification = "OK.")]
    public TextShape(Svg svg, XmlNode node, Shape parent)
        : base(svg, node, parent)
    {
        ArgumentNullException.ThrowIfNull(svg);
        ArgumentNullException.ThrowIfNull(node);

        X = SvgXmlUtil.AttrValue(node, "x", 0);
        Y = SvgXmlUtil.AttrValue(node, "y", 0);
        Text = node.InnerText;
        GetTextStyle();
        if (node.InnerXml.IndexOf("<", StringComparison.Ordinal) != -1)
        {
            TextSpan = ParseTSpan(svg, node.InnerXml);
        }

        defaultFill ??= new Fill
        {
            PaintServerKey = svg.PaintServers.Parse("black"),
        };

        defaultStroke ??= new Stroke
        {
            Width = 0.1,
        };
    }

    public double X { get; }

    public double Y { get; }

    public string Text { get; }

    public TSpan.Element? TextSpan { get; }

    public override Fill? Fill => base.Fill ?? defaultFill;

    public override Stroke? Stroke => base.Stroke ?? defaultStroke;

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    private TSpan.Element? ParseTSpan(Svg svg, string tSpanText)
    {
        try
        {
            return TSpan.Parse(svg, tSpanText, this);
        }
        catch
        {
            return null;
        }
    }

    internal sealed class TSpan
    {
        public sealed class Element : Shape
        {
            public override Transform? Transform => Parent?.Transform;

            public TextShapeElementType ElementType { get; }

            public List<KeyValueItem>? Attributes { get; set; }

            public List<Element>? Children { get; }

            public int StartIndex { get; init; }

            public string Text { get; init; }

            public Element? End { get; set; }

            public Element(Svg svg, Shape parent, string text)
                : base(svg, (XmlNode)null!, parent)
            {
                ElementType = TextShapeElementType.Text;
                Text = text;
            }

            public Element(Svg svg, Shape parent, TextShapeElementType elementType, IReadOnlyCollection<KeyValueItem>? attributes)
                : base(svg, attributes, parent)
            {
                ElementType = elementType;
                Text = string.Empty;
                Children = new List<Element>();
            }

            public override string ToString()
            {
                return Text;
            }
        }

        public static Element? Parse(Svg svg, string text, TextShape owner)
        {
            ArgumentNullException.ThrowIfNull(text);

            var curPos = 0;
            var root = new Element(svg, owner, TextShapeElementType.Tag, attributes: null)
            {
                Text = "<root>",
                StartIndex = 0,
            };

            return Parse(svg, text, ref curPos, parent: null, root);
        }

        public void Print(Element tag, string indent)
        {
            ArgumentNullException.ThrowIfNull(tag);

            if (tag.ElementType == TextShapeElementType.Text)
            {
                System.Console.WriteLine($"{indent} '{tag.Text}'");
            }

            if (tag.Children is null)
            {
                return;
            }

            indent += "   ";
            foreach (var c in tag.Children)
            {
                Print(c, indent);
            }
        }

        [SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "OK.")]
        [SuppressMessage("Major Code Smell", "S112:General exceptions should never be thrown", Justification = "OK.")]
        private static Element? NextTag(Svg svg, Element parent, string text, ref int curPos)
        {
            var start = text.IndexOf("<", curPos, StringComparison.Ordinal);
            if (start < 0)
            {
                return null;
            }

            var end = text.IndexOf(">", start + 1, StringComparison.Ordinal);
            if (end < 0)
            {
                throw new Exception("Start '<' with no end '>'");
            }

            end++;

            var tagText = text.Substring(start, end - start);
            if (tagText.IndexOf("<", 1, StringComparison.Ordinal) != -1)
            {
                throw new Exception("Start '<' within tag 'tag'");
            }

            var attrs = new List<KeyValueItem>();
            var attrStart = tagText.IndexOf("tspan", StringComparison.Ordinal);
            if (attrStart > 0)
            {
                attrStart += 5;
                while (attrStart < tagText.Length - 1)
                {
                    attrs.Add(ShapeUtil.ReadNextAttr(tagText, ref attrStart));
                }
            }

            var tag = new Element(svg, parent, TextShapeElementType.Tag, attrs)
            {
                StartIndex = start,
                Text = text.Substring(start, end - start),
            };

            if (tag.Text.IndexOf("<", 1, StringComparison.Ordinal) != -1)
            {
                throw new Exception("Start '<' within tag 'tag'");
            }

            curPos = end;
            return tag;
        }

        [SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "OK.")]
        [SuppressMessage("Major Code Smell", "S112:General exceptions should never be thrown", Justification = "OK.")]
        private static Element? Parse(Svg svg, string text, ref int curPos, Element? parent, Element? curTag)
        {
            var tag = curTag ?? NextTag(svg, parent!, text, ref curPos);

            while (curPos < text.Length)
            {
                var prevPos = curPos;
                var next = NextTag(svg, tag!, text, ref curPos);
                if (tag?.Children is not null && next is null && curPos < text.Length)
                {
                    var s = text.Substring(curPos, text.Length - curPos);
                    tag.Children.Add(new Element(svg, tag, s));
                    return tag;
                }

                if (next is null)
                {
                    throw new Exception("unexpected tag");
                }

                if (tag?.Children is not null && next.StartIndex - prevPos > 0)
                {
                    var diff = next.StartIndex - prevPos;
                    var s = text.Substring(prevPos, diff);
                    tag.Children.Add(new Element(svg, tag, s));
                }

                if (tag?.Children is not null && next.Text.StartsWith("<tspan", StringComparison.Ordinal))
                {
                    next = Parse(svg, text, ref curPos, tag, next);
                    if (next is not null)
                    {
                        tag.Children.Add(next);
                    }

                    continue;
                }

                if (tag is not null && next.Text.StartsWith("</tspan", StringComparison.Ordinal))
                {
                    tag.End = next;
                    return tag;
                }

                if (next.Text.StartsWith("<textPath", StringComparison.Ordinal))
                {
                    continue;
                }

                if (next.Text.StartsWith("</textPath", StringComparison.Ordinal))
                {
                    continue;
                }

                throw new Exception($"unexpected tag '{next.Text}'");
            }

            return tag;
        }
    }
}