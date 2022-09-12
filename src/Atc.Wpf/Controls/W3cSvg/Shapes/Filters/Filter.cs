namespace Atc.Wpf.Controls.W3cSvg.Shapes.Filters;

internal class Filter : Group
{
    public Filter(Svg svg, XmlNode node, Shape parent)
        : base(svg, node, parent)
    {
    }

    public BitmapEffect GetBitmapEffect()
    {
        var bitmapEffectGroup = new BitmapEffectGroup();
        foreach (var element in Elements.OfType<FilterBaseFe>())
        {
            bitmapEffectGroup.Children.Add(element.GetBitmapEffect());
        }

        return bitmapEffectGroup;
    }
}