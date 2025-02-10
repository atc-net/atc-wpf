// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Theming.Controls.Windows;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "OK.")]
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK.")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "OK.")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "OK.")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "OK.")]
[SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "OK.")]
public sealed class WindowPlacementSetting
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
            ptMinPosition = new System.Drawing.Point { X = (int)minPosition.X, Y = (int)minPosition.Y },
            ptMaxPosition = new System.Drawing.Point { X = (int)maxPosition.X, Y = (int)maxPosition.Y },
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
            minPosition = new Point(windowPlacement.ptMinPosition.X, windowPlacement.ptMinPosition.Y),
            maxPosition = new Point(windowPlacement.ptMaxPosition.X, windowPlacement.ptMaxPosition.Y),
            normalPosition = new Rect(
                windowPlacement.rcNormalPosition.left,
                windowPlacement.rcNormalPosition.top,
                windowPlacement.rcNormalPosition.GetWidth(),
                windowPlacement.rcNormalPosition.GetHeight()),
        };
    }
}