using System.Xml;
using Atc.Wpf.Controls.W3cSvg.Shapes;

namespace Atc.Wpf.Controls.W3cSvg.Animation
{
    internal class AnimateTransform : AnimationBase
    {
        public AnimateTransform(Svg svg, XmlNode node, Shape parent)
            : base(svg, node, parent)
        {
            this.Type = Enum<AnimateTransformType>.Parse(SvgXmlUtil.AttrValue(node, "type", "translate"));
            this.From = SvgXmlUtil.AttrValue(node, "from", defaultValue: null);
            this.To = SvgXmlUtil.AttrValue(node, "to", defaultValue: null);
            this.AttributeName = SvgXmlUtil.AttrValue(node, "attributeName", defaultValue: null);
            this.RepeatType = SvgXmlUtil.AttrValue(node, "repeatCount", "indefinite");
            this.Values = SvgXmlUtil.AttrValue(node, "values", defaultValue: null);
        }

        public string? AttributeName { get; }

        public AnimateTransformType Type { get; }

        public string? From { get; }

        public string? To { get; }

        public string? Values { get; }

        public string? RepeatType { get; }

        public override string ToString() => $"{base.ToString()}, {nameof(AttributeName)}: {AttributeName}, {nameof(Type)}: {Type}, {nameof(From)}: {From}, {nameof(To)}: {To}, {nameof(Values)}: {Values}, {nameof(RepeatType)}: {RepeatType}";
    }
}