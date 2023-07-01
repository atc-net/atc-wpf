namespace Atc.Wpf.Controls.Media.W3cSvg.Animation;

internal sealed class AnimateTransform : AnimationBase
{
    public AnimateTransform(Svg svg, XmlNode node, Shape parent)
        : base(svg, node, parent)
    {
        Type = Enum<AnimateTransformType>.Parse(SvgXmlUtil.AttrValue(node, "type", "translate")!);
        From = SvgXmlUtil.AttrValue(node, "from", defaultValue: null);
        To = SvgXmlUtil.AttrValue(node, "to", defaultValue: null);
        AttributeName = SvgXmlUtil.AttrValue(node, "attributeName", defaultValue: null);
        RepeatType = SvgXmlUtil.AttrValue(node, "repeatCount", "indefinite");
        Values = SvgXmlUtil.AttrValue(node, "values", defaultValue: null);
    }

    public string? AttributeName { get; }

    public AnimateTransformType Type { get; }

    public string? From { get; }

    public string? To { get; }

    public string? Values { get; }

    public string? RepeatType { get; }

    public override string ToString() => $"{base.ToString()}, {nameof(AttributeName)}: {AttributeName}, {nameof(Type)}: {Type}, {nameof(From)}: {From}, {nameof(To)}: {To}, {nameof(Values)}: {Values}, {nameof(RepeatType)}: {RepeatType}";
}