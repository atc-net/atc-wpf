namespace Atc.Wpf.Controls.Media.W3cSvg.PaintServers;

internal sealed class SolidColorPaintServer : PaintServer
{
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
    }

    public Color Color { get; }

    public override Brush? GetBrush(
        double opacity,
        Svg svg,
        SvgRender svgRender,
        Rect bounds)
    {
        if (Brush is not null)
        {
            return Brush;
        }

        var a = (byte)(255 * opacity / 100);
        var c = Color;
        var newColor = Color.FromArgb(a, c.R, c.G, c.B);
        Brush = new SolidColorBrush(newColor);
        return Brush;
    }

    public override string ToString() => $"{nameof(Color)}: {Color}";
}