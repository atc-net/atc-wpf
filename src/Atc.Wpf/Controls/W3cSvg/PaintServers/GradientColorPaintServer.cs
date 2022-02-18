// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Controls.W3cSvg.PaintServers;

internal abstract class GradientColorPaintServer : PaintServer
{
    private readonly List<GradientStop> gradientStop = new List<GradientStop>();

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - for now.")]
    protected GradientColorPaintServer(PaintServerManager owner, XmlNode node)
        : base(owner)
    {
        if (owner == null)
        {
            throw new ArgumentNullException(nameof(owner));
        }

        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        this.GradientUnits = SvgXmlUtil.AttrValue(node, "gradientUnits", string.Empty);
        var transform = SvgXmlUtil.AttrValue(node, "gradientTransform", string.Empty);
        if (!string.IsNullOrEmpty(transform))
        {
            this.Transform = ShapeUtil.ParseTransform(transform.ToLower(GlobalizationConstants.EnglishCultureInfo));
        }

        var refId = SvgXmlUtil.AttrValue(node, "xlink:href", string.Empty);
        if (node.ChildNodes.Count == 0 && !string.IsNullOrEmpty(refId))
        {
            var paintServerKey = owner.Parse(refId.Substring(1));
            if (owner.GetServer(paintServerKey) is not GradientColorPaintServer refCol)
            {
                return;
            }

            this.gradientStop = new List<GradientStop>(refCol.gradientStop);
        }

        foreach (XmlNode childNode in node.ChildNodes)
        {
            if (childNode.Name != "stop")
            {
                continue;
            }

            var styleAttr = new List<KeyValueItem>();
            var fullStyle = SvgXmlUtil.AttrValue(childNode, SvgTagConstants.Style, string.Empty);
            if (!string.IsNullOrEmpty(fullStyle))
            {
                styleAttr.AddRange(SvgXmlUtil.SplitStyle(fullStyle).Select(x => new KeyValueItem(x.Key, x.Value)));
            }

            foreach (var attr in styleAttr)
            {
                childNode.Attributes?.Append(new TempXmlAttribute(childNode, attr.Key, attr.Value));
            }

            var offset = SvgXmlUtil.AttrValue(childNode, "offset", 0);
            var stopColor = SvgXmlUtil.AttrValue(childNode, "stop-color", "#0");
            if (stopColor is null)
            {
                continue;
            }

            var stopOpacity = SvgXmlUtil.AttrValue(childNode, "stop-opacity", 1);

            var color = stopColor.StartsWith('#')
                ? ColorUtil.GetColorFromHex(stopColor)
                : PaintServerManager.KnownColor(stopColor);

            if (!stopOpacity.Equals(1))
            {
                color = Color.FromArgb((byte)(stopOpacity * 255), color.R, color.G, color.B);
            }

            if (offset > 1)
            {
                offset /= 100;
            }

            this.gradientStop.Add(new GradientStop(color, offset));
        }
    }

    protected GradientColorPaintServer(PaintServerManager owner)
        : base(owner)
    {
    }

    public IEnumerable<GradientStop> Stops => this.gradientStop.AsReadOnly();

    public Transform? Transform { get; protected set; }

    public string? GradientUnits { get; }

    public sealed class TempXmlAttribute : XmlAttribute
    {
        public TempXmlAttribute(XmlNode owner, string name, string value)
            : base(string.Empty, name, string.Empty, owner.OwnerDocument)
        {
            this.Value = value;
        }
    }
}