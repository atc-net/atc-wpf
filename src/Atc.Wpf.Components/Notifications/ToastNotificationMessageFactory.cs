namespace Atc.Wpf.Components.Notifications;

public static class ToastNotificationMessageFactory
{
    public static ToastNotificationMessage CreateInformation(
        string area,
        string message)
        => new(ToastNotificationType.Information, area, message);

    public static ToastNotificationMessage CreateSuccess(
        string area,
        string message)
        => new(ToastNotificationType.Success, area, message);

    public static ToastNotificationMessage CreateWarning(
        string area,
        string message)
        => new(ToastNotificationType.Warning, area, message);

    public static ToastNotificationMessage CreateError(
        string area,
        string message)
        => new(ToastNotificationType.Error, area, message);
}