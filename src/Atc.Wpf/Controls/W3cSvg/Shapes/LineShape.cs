using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Xml;

namespace Atc.Wpf.Controls.W3cSvg.Shapes
{
    internal class LineShape : Shape
    {
        public LineShape(Svg svg, XmlNode node)
            : base(svg, node)
        {
            if (svg == null)
            {
                throw new ArgumentNullException(nameof(svg));
            }

            double x1 = SvgXmlUtil.AttrValue(node, "x1", 0, svg.Size.Width);
            double y1 = SvgXmlUtil.AttrValue(node, "y1", 0, svg.Size.Height);
            double x2 = SvgXmlUtil.AttrValue(node, "x2", 0, svg.Size.Width);
            double y2 = SvgXmlUtil.AttrValue(node, "y2", 0, svg.Size.Height);
            this.P1 = new Point(x1, y1);
            this.P2 = new Point(x2, y2);
        }

        public Point P1 { get; }

        public Point P2 { get; }

        [SuppressMessage("Design", "MA0076:Do not use implicit culture-sensitive ToString in interpolated strings", Justification = "OK.")]
        public override string ToString() => $"{base.ToString()}, {nameof(P1)}: ({P1}), {nameof(P2)}: ({P2})";
    }
}
