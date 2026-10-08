namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// Decodes the shell callback message of a notification area icon that uses
/// <c>NOTIFYICON_VERSION_4</c>, where the low word of <c>lParam</c> holds the notification.
/// </summary>
internal static class TrayIconMessageDecoder
{
    // The shell truncates szTip to 127 characters plus the terminating null.
    internal const int MaxToolTipLength = 127;

    private const int NinSelect = 0x0400;
    private const int NinKeySelect = 0x0401;
    private const int WmLeftButtonDoubleClick = 0x0203;
    private const int WmContextMenu = 0x007B;

    public static TrayIconAction Decode(long lParam)
        => (lParam & 0xFFFF) switch
        {
            NinSelect or NinKeySelect => TrayIconAction.Click,
            WmLeftButtonDoubleClick => TrayIconAction.DoubleClick,
            WmContextMenu => TrayIconAction.ContextMenu,
            _ => TrayIconAction.None,
        };

    public static string TruncateToolTip(string toolTip)
        => toolTip.Length <= MaxToolTipLength
            ? toolTip
            : toolTip[..MaxToolTipLength];
}