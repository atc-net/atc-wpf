namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// The user action on a <see cref="TrayIcon"/>, decoded from the shell callback message.
/// </summary>
internal enum TrayIconAction
{
    None,
    Click,
    DoubleClick,
    ContextMenu,
}