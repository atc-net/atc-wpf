using System.Collections.Generic;
using System.Windows;
using System.Xml;

namespace Atc.Wpf.Controls.W3cSvg.Shapes
{
    internal class PolylineShape : Shape
    {
        public PolylineShape(Svg svg, XmlNode node)
            : base(svg, node)
        {
            var points = SvgXmlUtil.AttrValue(node, SvgTagConstants.Points, string.Empty);
            var split = new StringSplitter(points);
            var list = new List<Point>();
            while (split.More)
            {
                list.Add(split.ReadNextPoint());
            }

            this.Points = list.ToArray();
        }

        public Point[] Points { get; }

        public override string ToString() => $"{base.ToString()}, {nameof(Points)}: {Points}";
    }
}