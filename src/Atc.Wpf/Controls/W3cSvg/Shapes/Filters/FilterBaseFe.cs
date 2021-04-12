using System.Windows.Media.Effects;
using System.Xml;

namespace Atc.Wpf.Controls.W3cSvg.Shapes.Filters
{
    internal abstract class FilterBaseFe : Shape
    {
        protected FilterBaseFe(Svg svg, XmlNode node, Shape parent)
            : base(svg, node, parent)
        {
        }

        public abstract BitmapEffect GetBitmapEffect();
    }
}