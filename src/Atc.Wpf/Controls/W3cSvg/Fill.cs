using System;
using System.Windows;
using System.Windows.Media;
using Atc.Wpf.Controls.W3cSvg.PaintServers;
using Atc.Wpf.Controls.W3cSvg.Shapes;

namespace Atc.Wpf.Controls.W3cSvg
{
    internal class Fill
    {
        public Fill()
        {
            this.FillRule = FillRuleType.NonZero;
            this.Opacity = 100;
        }

        public FillRuleType FillRule { get; set; }

        public string? PaintServerKey { get; set; }

        public double Opacity { get; set; }

        public Brush? FillBrush(Svg svg, SvgRender svgRender, Shape shape, double elementOpacity, Rect bounds)
        {
            if (svg == null)
            {
                throw new ArgumentNullException(nameof(svg));
            }

            if (shape == null)
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var paintServer = svg.PaintServers.GetServer(PaintServerKey);
            if (paintServer == null)
            {
                return null;
            }

            if (paintServer is CurrentColorPaintServer)
            {
                var shapePaintServer = svg.PaintServers.GetServer(shape.PaintServerKey);
                if (shapePaintServer != null)
                {
                    return shapePaintServer.GetBrush(this.Opacity * elementOpacity, svg, svgRender, bounds);
                }
            }

            if (paintServer is not InheritPaintServer)
            {
                return paintServer.GetBrush(this.Opacity * elementOpacity, svg, svgRender, bounds);
            }

            var p = shape.RealParent;
            while (p != null)
            {
                if (p.Fill != null)
                {
                    var checkPaintServer = svg.PaintServers.GetServer(p.Fill.PaintServerKey);
                    if (checkPaintServer is not null && !(checkPaintServer is InheritPaintServer))
                    {
                        return checkPaintServer.GetBrush(this.Opacity * elementOpacity, svg, svgRender, bounds);
                    }
                }

                p = p.RealParent;
            }

            return null;
        }
    }
}