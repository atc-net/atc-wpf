// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Atc.Wpf.Controls.W3cSvg.Shapes.Filters;

internal class FilterFeGaussianBlur : FilterBaseFe
{
    public FilterFeGaussianBlur(Svg svg, XmlNode node, Shape parent)
        : base(svg, node, parent)
    {
        StdDeviationX = StdDeviationY = SvgXmlUtil.AttrValue(node, "stdDeviation", 0);
    }

    public string? In { get; set; }

    public double StdDeviationX { get; }

    public double StdDeviationY { get; }

    public override BitmapEffect GetBitmapEffect() =>
        new BlurBitmapEffect
        {
            Radius = StdDeviationX,
        };

    public override string ToString() => $"{base.ToString()}, {nameof(In)}: {In}, {nameof(StdDeviationX)}: {StdDeviationX}, {nameof(StdDeviationY)}: {StdDeviationY}";
}