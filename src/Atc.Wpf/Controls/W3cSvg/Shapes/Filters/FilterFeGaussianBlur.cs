using System.Diagnostics.CodeAnalysis;
using System.Windows.Media.Effects;
using System.Xml;

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Atc.Wpf.Controls.W3cSvg.Shapes.Filters
{
    internal class FilterFeGaussianBlur : FilterBaseFe
    {
        public FilterFeGaussianBlur(Svg svg, XmlNode node, Shape parent)
            : base(svg, node, parent)
        {
            this.StdDeviationX = this.StdDeviationY = SvgXmlUtil.AttrValue(node, "stdDeviation", 0);
        }

        public string? In { get; set; }

        public double StdDeviationX { get; }

        public double StdDeviationY { get; }

        public override BitmapEffect GetBitmapEffect() =>
            new BlurBitmapEffect
            {
                Radius = this.StdDeviationX,
            };

        [SuppressMessage("Design", "MA0076:Do not use implicit culture-sensitive ToString in interpolated strings", Justification = "OK.")]
        public override string ToString() => $"{base.ToString()}, {nameof(In)}: {In}, {nameof(StdDeviationX)}: {StdDeviationX}, {nameof(StdDeviationY)}: {StdDeviationY}";
    }
}