using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Atc.Wpf.Controls.W3cSvg.Shapes
{
    internal class RectangleShape : Shape
    {
        private static Fill? defaultFill;

        [SuppressMessage("Major Code Smell", "S3010:Static fields should not be updated in constructors", Justification = "OK.")]
        public RectangleShape(Svg svg, XmlNode node)
            : base(svg, node)
        {
            if (svg == null)
            {
                throw new ArgumentNullException(nameof(svg));
            }

            defaultFill ??= new Fill
            {
                PaintServerKey = svg.PaintServers.Parse("black"),
            };
        }

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Rx { get; set; }

        public double Ry { get; set; }

        public override Fill? Fill => base.Fill ?? defaultFill;

        protected override void Parse(Svg svg, string name, string value)
        {
            if (svg == null)
            {
                throw new ArgumentNullException(nameof(svg));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Contains(':', StringComparison.Ordinal))
            {
                name = name.Split(':')[1];
            }

            switch (name)
            {
                case "x":
                    this.X = SvgXmlUtil.GetDoubleValue(value, svg.Size.Width);
                    return;
                case "y":
                    this.Y = SvgXmlUtil.GetDoubleValue(value, svg.Size.Height);
                    return;
                case "width":
                    this.Width = SvgXmlUtil.GetDoubleValue(value, svg.Size.Width);
                    return;
                case "height":
                    this.Height = SvgXmlUtil.GetDoubleValue(value, svg.Size.Height);
                    return;
                case "rx":
                    this.Rx = SvgXmlUtil.GetDoubleValue(value, svg.Size.Width);
                    return;
                case "ry":
                    this.Ry = SvgXmlUtil.GetDoubleValue(value, svg.Size.Height);
                    return;
                default:
                    base.Parse(svg, name, value);
                    break;
            }
        }

        [SuppressMessage("Design", "MA0076:Do not use implicit culture-sensitive ToString in interpolated strings", Justification = "OK.")]
        public override string ToString() => $"{base.ToString()}, {nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(Rx)}: {Rx}, {nameof(Ry)}: {Ry}, {nameof(Fill)}: {Fill}";
    }
}