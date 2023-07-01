namespace Atc.Wpf.Controls.Media.W3cSvg.PaintServers;

internal sealed class InheritPaintServer : PaintServer
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