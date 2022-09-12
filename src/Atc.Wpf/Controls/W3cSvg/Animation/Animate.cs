// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Controls.W3cSvg.Animation;

internal class Animate : AnimationBase
{
    public Animate(Svg svg, XmlNode node, Shape parent)
        : base(svg, node, parent)
    {
        From = SvgXmlUtil.AttrValue(node, "from", defaultValue: null);
        To = SvgXmlUtil.AttrValue(node, "to", defaultValue: null);
        AttributeName = SvgXmlUtil.AttrValue(node, "attributeName", defaultValue: null);
        RepeatType = SvgXmlUtil.AttrValue(node, "repeatCount", "indefinite");
        Values = SvgXmlUtil.AttrValue(node, "values", defaultValue: null);
        Href = SvgXmlUtil.AttrValue(node, "xlink:href", string.Empty);
        if (Href is not null && Href.StartsWith('#'))
        {
            Href = Href.Substring(1);
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