namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// Default implementation of <see cref="IToastNotificationService"/> that wraps
/// the existing <see cref="ToastNotificationManager"/> for MVVM-friendly notification display.
/// </summary>
public class ToastNotificationService : IToastNotificationService
{
    private readonly ToastNotificationManager notificationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToastNotificationService"/> class.
    /// </summary>
    public ToastNotificationService()
        : this(dispatcher: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToastNotificationService"/> class.
    /// </summary>
    /// <param name="dispatcher">Optional dispatcher for thread marshalling.</param>
    public ToastNotificationService(Dispatcher? dispatcher)
    {
        notificationManager = new ToastNotificationManager(dispatcher);
    }

    /// <inheritdoc />
    public void ShowInformation(
        string title,
        string message,
        string areaName = "",
        TimeSpan? expirationTime = null,
        bool useDesktop = false,
        Action? onClick = null,
        Action? onClose = null)
        => Show(
            ToastNotificationType.Information,
            title,
            message,
            areaName,
            expirationTime,
            useDesktop,
            onClick,
            onClose);

    /// <inheritdoc />
    public void ShowSuccess(
        string title,
        string message,
        string areaName = "",
        TimeSpan? expirationTime = null,
        bool useDesktop = false,
        Action? onClick = null,
        Action? onClose = null)
        => Show(
            ToastNotificationType.Success,
            title,
            message,
            areaName,
            expirationTime,
            useDesktop,
            onClick,
            onClose);

    /// <inheritdoc />
    public void ShowWarning(
        string title,
        string message,
        string areaName = "",
        TimeSpan? expirationTime = null,
        bool useDesktop = false,
        Action? onClick = null,
        Action? onClose = null)
        => Show(
            ToastNotificationType.Warning,
            title,
            message,
            areaName,
            expirationTime,
            useDesktop,
            onClick,
            onClose);

    /// <inheritdoc />
    public void ShowError(
        string title,
        string message,
        string areaName = "",
        TimeSpan? expirationTime = null,
        bool useDesktop = false,
        Action? onClick = null,
        Action? onClose = null)
        => Show(
            ToastNotificationType.Error,
            title,
            message,
            areaName,
            expirationTime,
            useDesktop,
            onClick,
            onClose);

    private void Show(
        ToastNotificationType type,
        string title,
        string message,
        string areaName,
        TimeSpan? expirationTime,
        bool useDesktop,
        Action? onClick,
        Action? onClose)
    {
        var content = new ToastNotificationContent(type, title, message);
        notificationManager.Show(
            useDesktop,
            content,
            areaName,
            expirationTime,
            onClick,
            onClose);
    }
}