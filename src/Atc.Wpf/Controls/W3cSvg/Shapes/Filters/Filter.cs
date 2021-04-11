using System.Linq;
using System.Windows.Media.Effects;
using System.Xml;

namespace Atc.Wpf.Controls.W3cSvg.Shapes.Filters
{
    internal class Filter : Group
    {
        public Filter(Svg svg, XmlNode node, Shape parent)
            : base(svg, node, parent)
        {
        }

        public BitmapEffect GetBitmapEffect()
        {
            var bitmapEffectGroup = new BitmapEffectGroup();
            foreach (var element in this.Elements.OfType<FilterBaseFe>())
            {
                bitmapEffectGroup.Children.Add(element.GetBitmapEffect());
            }

            return bitmapEffectGroup;
        }
    }
}