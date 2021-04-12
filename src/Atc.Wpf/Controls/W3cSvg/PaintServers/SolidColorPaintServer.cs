using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;

namespace Atc.Wpf.Controls.W3cSvg.PaintServers
{
    internal class SolidColorPaintServer : PaintServer
    {
        public SolidColorPaintServer(PaintServerManager owner, Color color)
            : base(owner)
        {
            this.Color = color;
        }

        public SolidColorPaintServer(PaintServerManager owner, Brush brush)
            : base(owner)
        {
            this.Brush = brush;
        }

        public Color Color { get; }

        public override Brush? GetBrush(double opacity, Svg svg, SvgRender svgRender, Rect bounds)
        {
            if (this.Brush != null)
            {
                return this.Brush;
            }

            byte a = (byte)(255 * opacity / 100);
            var c = this.Color;
            var newColor = Color.FromArgb(a, c.R, c.G, c.B);
            this.Brush = new SolidColorBrush(newColor);
            return this.Brush;
        }

        [SuppressMessage("Design", "MA0076:Do not use implicit culture-sensitive ToString in interpolated strings", Justification = "OK.")]
        public override string ToString() => $"{nameof(Color)}: {Color}";
    }
}