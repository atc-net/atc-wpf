namespace Atc.Wpf.Controls.Media.W3cSvg.PaintServers;

internal sealed class CurrentColorPaintServer : PaintServer
{
    public CurrentColorPaintServer(PaintServerManager owner)
        : base(owner)
    {
    }

    public override Brush? GetBrush(double opacity, Svg svg, SvgRender svgRender, Rect bounds)
    {
        return null;
    }
}