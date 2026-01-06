namespace Atc.Wpf.Controls.Media.W3cSvg.Animation;

internal class AnimationBase : Shape
{
    public AnimationBase(
        Svg svg,
        XmlNode node,
        Shape parent)
        : base(svg, node, parent)
    {
        var dur = SvgXmlUtil.AttrValue(node, "dur", string.Empty);
        if (dur is null)
        {
            return;
        }

        if (dur.EndsWith("ms", StringComparison.Ordinal))
        {
            Duration = TimeSpan.FromMilliseconds(double.Parse(dur.AsSpan(0, dur.Length - 2), GlobalizationConstants.EnglishCultureInfo));
        }
        else if (dur.EndsWith('s'))
        {
            Duration = TimeSpan.FromSeconds(double.Parse(dur.AsSpan(0, dur.Length - 1), GlobalizationConstants.EnglishCultureInfo));
        }
        else
        {
            Duration = TimeSpan.FromSeconds(double.Parse(dur, GlobalizationConstants.EnglishCultureInfo));
        }
    }

    public TimeSpan Duration { get; init; }

    public override string ToString()
        => $"{base.ToString()}, {nameof(Duration)}: {Duration}";
}