namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// Factory methods for creating <see cref="ToastNotificationMessage"/> instances.
/// </summary>
public static class ToastNotificationMessageFactory
{
    /// <summary>
    /// Creates an information toast message; <c>area</c> is used as the title.
    /// </summary>
    public static ToastNotificationMessage CreateInformation(
        string area,
        string message)
        => new(ToastNotificationType.Information, area, message);

    /// <summary>
    /// Creates a success toast message; <c>area</c> is used as the title.
    /// </summary>
    public static ToastNotificationMessage CreateSuccess(
        string area,
        string message)
        => new(ToastNotificationType.Success, area, message);

    /// <summary>
    /// Creates a warning toast message; <c>area</c> is used as the title.
    /// </summary>
    public static ToastNotificationMessage CreateWarning(
        string area,
        string message)
        => new(ToastNotificationType.Warning, area, message);

    /// <summary>
    /// Creates an error toast message; <c>area</c> is used as the title.
    /// </summary>
    public static ToastNotificationMessage CreateError(
        string area,
        string message)
        => new(ToastNotificationType.Error, area, message);
}