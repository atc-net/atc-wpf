namespace Atc.Wpf.Components.Notifications;

public class ToastNotificationContent(
    ToastNotificationType type,
    string title,
    string message)
    : UserControl
{
    public ToastNotificationType Type { get; init; } = type;

    public string Title { get; init; } = title;

    public string Message { get; init; } = message;

    public void Deconstruct(
        out ToastNotificationType type,
        out string title,
        out string message)
    {
        type = Type;
        title = Title;
        message = Message;
    }
}