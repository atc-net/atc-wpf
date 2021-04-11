using System.Xml;
using Atc.Wpf.Controls.W3cSvg.Shapes;

namespace Atc.Wpf.Controls.W3cSvg.Animation
{
    internal class AnimateColor : AnimationBase
    {
        public AnimateColor(Svg svg, XmlNode node, Shape parent)
            : base(svg, node, parent)
        {
        }
    }
}