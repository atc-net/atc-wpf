using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Atc.Wpf.Controls.W3cSvg.PaintServers
{
    internal class LinearGradientColorPaintServer : GradientColorPaintServer
    {
        public LinearGradientColorPaintServer(PaintServerManager owner, XmlNode node)
            : base(owner, node)
        {
            this.X1 = SvgXmlUtil.AttrValue(node, "x1", double.NaN);
            this.Y1 = SvgXmlUtil.AttrValue(node, "y1", double.NaN);
            this.X2 = SvgXmlUtil.AttrValue(node, "x2", double.NaN);
            this.Y2 = SvgXmlUtil.AttrValue(node, "y2", double.NaN);
        }

        public LinearGradientColorPaintServer(PaintServerManager owner, Brush newBrush)
            : base(owner)
        {
            Brush = newBrush;
        }

        public double X1 { get; private set; }

        public double Y1 { get; private set; }

        public double X2 { get; private set; }

        public double Y2 { get; private set; }

        public override Brush GetBrush(double opacity, Svg svg, SvgRender svgRender, Rect bounds)
        {
            if (this.Brush != null)
            {
                return this.Brush;
            }

            var brush = new LinearGradientBrush();
            foreach (GradientStop stop in this.Stops)
            {
                brush.GradientStops.Add(stop);
            }

            brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(1, 0);

            if (this.GradientUnits == SvgTagConstants.UserSpaceOnUse)
            {
                var tr = this.Transform;
                if (tr != null)
                {
                    brush.StartPoint = tr.Transform(new Point(this.X1, this.Y1));
                    brush.EndPoint = tr.Transform(new Point(this.X2, this.Y2));
                }
                else
                {
                    brush.StartPoint = new Point(this.X1, this.Y1);
                    brush.EndPoint = new Point(this.X2, this.Y2);
                }

                this.Transform = null;
                brush.MappingMode = BrushMappingMode.Absolute;
            }
            else
            {
                this.Normalize();
                if (!double.IsNaN(this.X1))
                {
                    brush.StartPoint = new Point(this.X1, this.Y1);
                }

                if (!double.IsNaN(this.X2))
                {
                    brush.EndPoint = new Point(this.X2, this.Y2);
                }
            }

            this.Brush = brush;

            return brush;
        }

        private void Normalize()
        {
            if (!double.IsNaN(this.X1) && !double.IsNaN(this.X2))
            {
                double min = this.X1;
                if (this.X2 < this.X1)
                {
                    min = this.X2;
                }

                this.X1 -= min;
                this.X2 -= min;
                double scale = this.X1;
                if (this.X2 > this.X1)
                {
                    scale = this.X2;
                }

                if (scale != 0)
                {
                    this.X1 /= scale;
                    this.X2 /= scale;
                }
            }

            if (!double.IsNaN(this.Y1) && !double.IsNaN(this.Y2))
            {
                double min = this.Y1;
                if (this.Y2 < this.Y1)
                {
                    min = this.Y2;
                }

                this.Y1 -= min;
                this.Y2 -= min;
                double scale = this.Y1;
                if (this.Y2 > this.Y1)
                {
                    scale = this.Y2;
                }

                if (scale != 0)
                {
                    this.Y1 /= scale;
                    this.Y2 /= scale;
                }
            }
        }

        [SuppressMessage("Design", "MA0076:Do not use implicit culture-sensitive ToString in interpolated strings", Justification = "OK.")]
        public override string ToString() => $"{nameof(X1)}: {X1}, {nameof(Y1)}: {Y1}, {nameof(X2)}: {X2}, {nameof(Y2)}: {Y2}";
    }
}