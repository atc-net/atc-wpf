namespace Atc.Wpf.Controls.Media.W3cSvg.PaintServers;

internal sealed class SolidColorPaintServer : PaintServer
{
    // Paint servers are shared by every shape of the document, so the brush is cached per resulting alpha.
    private readonly Dictionary<byte, Brush> brushesByAlpha = new();
    private readonly bool hasFixedBrush;

    public SolidColorPaintServer(
        PaintServerManager owner,
        Color color)
        : base(owner)
    {
        Color = color;
    }

    public SolidColorPaintServer(
        PaintServerManager owner,
        Brush brush)
        : base(owner)
    {
        Brush = brush;
        hasFixedBrush = true;
    }

    public Color Color { get; }

    public override Brush? GetBrush(
        double opacity,
        Svg svg,
        SvgRender svgRender,
        Rect bounds)
    {
        if (hasFixedBrush)
        {
            return Brush;
        }

        var alpha = (byte)System.Math.Clamp(Color.A * opacity / 100, 0, 255);
        if (brushesByAlpha.TryGetValue(alpha, out var cachedBrush))
        {
            return cachedBrush;
        }

        // The brush is shared by every shape in this color, so it is frozen: WPF tracks each parent of an
        // unfrozen brush, which gets slower with every shape that uses it.
        var brush = new SolidColorBrush(Color.FromArgb(alpha, Color.R, Color.G, Color.B));
        brush.Freeze();
        brushesByAlpha[alpha] = brush;

        // The first brush is the one exposed through the custom brushes of the image, as an editable copy.
        Brush ??= brush.Clone();
        return brush;
    }

    public override string ToString() => $"{nameof(Color)}: {Color}";
}