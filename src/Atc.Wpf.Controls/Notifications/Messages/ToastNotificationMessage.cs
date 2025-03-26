namespace Atc.Wpf.Controls.Notifications.Messages;

public sealed class ToastNotificationMessage(
    ToastNotificationType toastNotificationType,
    string title,
    string message)
    : MessageBase
{
    public ToastNotificationType ToastNotificationType { get; } = toastNotificationType;

    public string Title { get; } = title;

    public string Message { get; } = message;

    public override string ToString()
        => $"{nameof(ToastNotificationType)}: {ToastNotificationType}, {nameof(Title)}: {Title}, {nameof(Message)}: {Message}";
}