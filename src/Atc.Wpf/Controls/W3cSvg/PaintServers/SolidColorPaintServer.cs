namespace Atc.Wpf.Controls.W3cSvg.PaintServers;

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

    public override string ToString() => $"{nameof(Color)}: {Color}";
}