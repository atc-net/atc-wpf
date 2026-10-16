namespace Atc.Wpf.Controls.Media.W3cSvg.Animation;

internal class AnimationBase : Shape
{
    public AnimationBase(
        Svg svg,
        XmlNode node,
        Shape parent)
        : base(svg, node, parent)
    {
        Duration = ParseDuration(SvgXmlUtil.AttrValue(node, "dur", defaultValue: null));
    }

    public TimeSpan Duration { get; init; }

    /// <summary>
    /// Reads a "dur" value in seconds ("2", "2s") or milliseconds ("500ms").
    /// A missing value, "indefinite" or anything else that is not such a number gives <see cref="TimeSpan.Zero"/>.
    /// </summary>
    private static TimeSpan ParseDuration(string? dur)
    {
        if (string.IsNullOrWhiteSpace(dur))
        {
            return TimeSpan.Zero;
        }

        dur = dur.Trim();
        var number = dur.AsSpan();
        var isMilliseconds = false;
        if (dur.EndsWith("ms", StringComparison.Ordinal))
        {
            number = number[..^2];
            isMilliseconds = true;
        }
        else if (dur.EndsWith('s'))
        {
            number = number[..^1];
        }

        if (!double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ||
            !double.IsFinite(value) ||
            value < 0)
        {
            return TimeSpan.Zero;
        }

        return isMilliseconds
            ? TimeSpan.FromMilliseconds(value)
            : TimeSpan.FromSeconds(value);
    }

    public override string ToString()
        => $"{base.ToString()}, {nameof(Duration)}: {Duration}";
}