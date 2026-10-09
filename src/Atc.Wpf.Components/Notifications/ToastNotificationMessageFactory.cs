namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// Factory methods for creating <see cref="ToastNotificationMessage"/> instances.
/// </summary>
public static class ToastNotificationMessageFactory
{
    /// <summary>
    /// Creates an information toast message.
    /// </summary>
    public static ToastNotificationMessage CreateInformation(
        string title,
        string message)
        => new(ToastNotificationType.Information, title, message);

    /// <summary>
    /// Creates a success toast message.
    /// </summary>
    public static ToastNotificationMessage CreateSuccess(
        string title,
        string message)
        => new(ToastNotificationType.Success, title, message);

    /// <summary>
    /// Creates a warning toast message.
    /// </summary>
    public static ToastNotificationMessage CreateWarning(
        string title,
        string message)
        => new(ToastNotificationType.Warning, title, message);

    /// <summary>
    /// Creates an error toast message.
    /// </summary>
    public static ToastNotificationMessage CreateError(
        string title,
        string message)
        => new(ToastNotificationType.Error, title, message);
}