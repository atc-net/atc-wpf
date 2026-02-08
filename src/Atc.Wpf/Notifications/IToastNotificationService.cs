namespace Atc.Wpf.Notifications;

/// <summary>
/// Service interface for showing toast notifications in an MVVM-friendly way.
/// </summary>
public interface IToastNotificationService
{
    /// <summary>
    /// Shows an information toast notification.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="areaName">Optional area name for targeting a specific ToastNotificationArea.</param>
    /// <param name="expirationTime">Optional duration before auto-dismiss. Defaults to 5 seconds.</param>
    /// <param name="useDesktop">If true, shows notification on the desktop overlay.</param>
    /// <param name="onClick">Optional action invoked when the notification is clicked.</param>
    /// <param name="onClose">Optional action invoked when the notification closes.</param>
    void ShowInformation(
        string title,
        string message,
        string areaName = "",
        TimeSpan? expirationTime = null,
        bool useDesktop = false,
        Action? onClick = null,
        Action? onClose = null);

    /// <summary>
    /// Shows a success toast notification.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="areaName">Optional area name for targeting a specific ToastNotificationArea.</param>
    /// <param name="expirationTime">Optional duration before auto-dismiss. Defaults to 5 seconds.</param>
    /// <param name="useDesktop">If true, shows notification on the desktop overlay.</param>
    /// <param name="onClick">Optional action invoked when the notification is clicked.</param>
    /// <param name="onClose">Optional action invoked when the notification closes.</param>
    void ShowSuccess(
        string title,
        string message,
        string areaName = "",
        TimeSpan? expirationTime = null,
        bool useDesktop = false,
        Action? onClick = null,
        Action? onClose = null);

    /// <summary>
    /// Shows a warning toast notification.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="areaName">Optional area name for targeting a specific ToastNotificationArea.</param>
    /// <param name="expirationTime">Optional duration before auto-dismiss. Defaults to 5 seconds.</param>
    /// <param name="useDesktop">If true, shows notification on the desktop overlay.</param>
    /// <param name="onClick">Optional action invoked when the notification is clicked.</param>
    /// <param name="onClose">Optional action invoked when the notification closes.</param>
    void ShowWarning(
        string title,
        string message,
        string areaName = "",
        TimeSpan? expirationTime = null,
        bool useDesktop = false,
        Action? onClick = null,
        Action? onClose = null);

    /// <summary>
    /// Shows an error toast notification.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="areaName">Optional area name for targeting a specific ToastNotificationArea.</param>
    /// <param name="expirationTime">Optional duration before auto-dismiss. Defaults to 5 seconds.</param>
    /// <param name="useDesktop">If true, shows notification on the desktop overlay.</param>
    /// <param name="onClick">Optional action invoked when the notification is clicked.</param>
    /// <param name="onClose">Optional action invoked when the notification closes.</param>
    void ShowError(
        string title,
        string message,
        string areaName = "",
        TimeSpan? expirationTime = null,
        bool useDesktop = false,
        Action? onClick = null,
        Action? onClose = null);
}