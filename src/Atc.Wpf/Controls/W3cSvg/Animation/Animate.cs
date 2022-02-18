// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Controls.W3cSvg.Animation;

internal class Animate : AnimationBase
{
    public Animate(Svg svg, XmlNode node, Shape parent)
        : base(svg, node, parent)
    {
        this.From = SvgXmlUtil.AttrValue(node, "from", defaultValue: null);
        this.To = SvgXmlUtil.AttrValue(node, "to", defaultValue: null);
        this.AttributeName = SvgXmlUtil.AttrValue(node, "attributeName", defaultValue: null);
        this.RepeatType = SvgXmlUtil.AttrValue(node, "repeatCount", "indefinite");
        this.Values = SvgXmlUtil.AttrValue(node, "values", defaultValue: null);
        this.Href = SvgXmlUtil.AttrValue(node, "xlink:href", string.Empty);
        if (this.Href is not null && this.Href.StartsWith('#'))
        {
            this.Href = this.Href.Substring(1);
        }
    }

    public string? AttributeName { get; }

    public string? From { get; }

    public string? To { get; }

    public string? RepeatType { get; }

    public string? Values { get; }

    public string? Href { get; }

    public override string ToString() => $"{base.ToString()}, {nameof(AttributeName)}: {AttributeName}, {nameof(From)}: {From}, {nameof(To)}: {To}, {nameof(RepeatType)}: {RepeatType}, {nameof(Values)}: {Values}, {nameof(Href)}: {Href}";
}