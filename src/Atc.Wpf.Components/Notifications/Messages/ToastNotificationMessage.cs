namespace Atc.Wpf.Components.Notifications.Messages;

/// <summary>
/// Messenger message that requests a toast notification.
/// </summary>
/// <param name="toastNotificationType">The kind of notification.</param>
/// <param name="title">The notification title.</param>
/// <param name="message">The notification message text.</param>
public sealed class ToastNotificationMessage(
    ToastNotificationType toastNotificationType,
    string title,
    string message)
    : MessageBase
{
    /// <summary>
    /// Gets the kind of notification.
    /// </summary>
    public ToastNotificationType ToastNotificationType { get; } = toastNotificationType;

    /// <summary>
    /// Gets the notification title.
    /// </summary>
    public string Title { get; } = title;

    /// <summary>
    /// Gets the notification message text.
    /// </summary>
    public string Message { get; } = message;

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(ToastNotificationType)}: {ToastNotificationType}, {nameof(Title)}: {Title}, {nameof(Message)}: {Message}";
}