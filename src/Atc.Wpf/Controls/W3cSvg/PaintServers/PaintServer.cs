namespace Atc.Wpf.Controls.W3cSvg.PaintServers;

internal class PaintServer
{
    public PaintServer(PaintServerManager owner)
    {
        Owner = owner;
    }

    public PaintServer(PaintServerManager owner, Brush brush)
    {
        Owner = owner;
        Brush = brush;
    }

    public PaintServerManager Owner { get; }

    protected Brush? Brush { get; set; }

    public virtual Brush? GetBrush(double opacity, Svg svg, SvgRender svgRender, Rect bounds)
    {
        return Brush;
    }

    public Brush? GetBrush()
    {
        return Brush;
    }
}