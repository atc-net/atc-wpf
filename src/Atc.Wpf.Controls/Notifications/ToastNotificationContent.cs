namespace Atc.Wpf.Controls.Notifications;

public record ToastNotificationContent(
    ToastNotificationType Type,
    string Title,
    string Message);