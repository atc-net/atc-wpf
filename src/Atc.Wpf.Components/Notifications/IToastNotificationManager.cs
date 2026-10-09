namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// Shows toast notifications in <see cref="ToastNotificationArea"/> controls or on the desktop.
/// </summary>
public interface IToastNotificationManager
{
    /// <summary>
    /// Shows a toast notification with the given content.
    /// </summary>
    /// <param name="useDesktop"><see langword="true"/> to show the notification in a desktop overlay window; otherwise it is shown in the matching notification area.</param>
    /// <param name="content">The content to display.</param>
    /// <param name="areaName">The name of the <see cref="ToastNotificationArea"/> to show the notification in; when no area matches, all non-desktop areas are used.</param>
    /// <param name="expirationTime">The time before the notification closes automatically; defaults to 5 seconds.</param>
    /// <param name="onClick">An optional action invoked when the notification is clicked.</param>
    /// <param name="onClose">An optional action invoked when the notification is closed.</param>
    void Show(
        bool useDesktop,
        UserControl content,
        string areaName = "",
        TimeSpan? expirationTime = null,
        Action? onClick = null,
        Action? onClose = null);
}