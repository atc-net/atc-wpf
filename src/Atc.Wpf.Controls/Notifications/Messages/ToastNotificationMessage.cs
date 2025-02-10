namespace Atc.Wpf.Controls.Notifications.Messages;

public sealed class ToastNotificationMessage : MessageBase
{
    public ToastNotificationMessage(
        ToastNotificationType toastNotificationType,
        string title,
        string message)
    {
        ToastNotificationType = toastNotificationType;
        Title = title;
        Message = message;
    }

    public ToastNotificationType ToastNotificationType { get; }

    public string Title { get; }

    public string Message { get; }

    public override string ToString()
        => $"{nameof(ToastNotificationType)}: {ToastNotificationType}, {nameof(Title)}: {Title}, {nameof(Message)}: {Message}";
}