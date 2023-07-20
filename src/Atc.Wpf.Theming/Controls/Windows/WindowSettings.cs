// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Theming.Controls.Windows;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "OK.")]
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK.")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "OK.")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "OK.")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "OK.")]
[SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "OK.")]
public class WindowPlacementSetting
{
    public uint showCmd;
    public Point minPosition;
    public Point maxPosition;
    public Rect normalPosition;

    internal WINDOWPLACEMENT ToWindowPlacement()
    {
        return new WINDOWPLACEMENT
        {
            length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>(),
            showCmd = (SHOW_WINDOW_CMD)showCmd,
            ptMinPosition = new POINT { x = (int)minPosition.X, y = (int)minPosition.Y },
            ptMaxPosition = new POINT { x = (int)maxPosition.X, y = (int)maxPosition.Y },
            rcNormalPosition = new RECT
            {
                left = (int)normalPosition.X,
                top = (int)normalPosition.Y,
                right = (int)normalPosition.Right,
                bottom = (int)normalPosition.Bottom,
            },
        };
    }

    internal static WindowPlacementSetting FromWindowPlacement(WINDOWPLACEMENT windowPlacement)
    {
        return new WindowPlacementSetting
        {
            showCmd = (uint)windowPlacement.showCmd,
            minPosition = new Point(windowPlacement.ptMinPosition.x, windowPlacement.ptMinPosition.y),
            maxPosition = new Point(windowPlacement.ptMaxPosition.x, windowPlacement.ptMaxPosition.y),
            normalPosition = new Rect(
                windowPlacement.rcNormalPosition.left,
                windowPlacement.rcNormalPosition.top,
                windowPlacement.rcNormalPosition.GetWidth(),
                windowPlacement.rcNormalPosition.GetHeight()),
        };
    }
}