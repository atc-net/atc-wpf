namespace Atc.Wpf.Controls.Media.W3cSvg.Shapes.Filters;

internal abstract class FilterBaseFe : Shape
{
    protected FilterBaseFe(
        Svg svg,
        XmlNode node,
        Shape parent)
        : base(svg, node, parent)
    {
    }

    public abstract BitmapEffect GetBitmapEffect();
}