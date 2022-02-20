namespace Atc.Wpf.Controls.W3cSvg.PaintServers;

internal class InheritPaintServer : PaintServer
{
    public InheritPaintServer(PaintServerManager owner)
        : base(owner)
    {
    }

    public override Brush? GetBrush(double opacity, Svg svg, SvgRender svgRender, Rect bounds)
    {
        return null;
    }
}