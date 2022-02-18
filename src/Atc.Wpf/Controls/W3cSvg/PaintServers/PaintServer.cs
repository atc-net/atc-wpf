namespace Atc.Wpf.Controls.W3cSvg.PaintServers;

internal class PaintServer
{
    public PaintServer(PaintServerManager owner)
    {
        this.Owner = owner;
    }

    public PaintServer(PaintServerManager owner, Brush brush)
    {
        this.Owner = owner;
        this.Brush = brush;
    }

    public PaintServerManager Owner { get; }

    protected Brush? Brush { get; set; }

    public virtual Brush? GetBrush(double opacity, Svg svg, SvgRender svgRender, Rect bounds)
    {
        return this.Brush;
    }

    public Brush? GetBrush()
    {
        return this.Brush;
    }
}