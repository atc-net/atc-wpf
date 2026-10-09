namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// The default content of a toast notification: a type, a title and a message.
/// </summary>
/// <param name="type">The kind of notification.</param>
/// <param name="title">The notification title.</param>
/// <param name="message">The notification message text.</param>
public class ToastNotificationContent(
    ToastNotificationType type,
    string title,
    string message)
    : UserControl
{
    /// <summary>
    /// Gets the kind of notification.
    /// </summary>
    public ToastNotificationType Type { get; init; } = type;

    /// <summary>
    /// Gets the notification title.
    /// </summary>
    public string Title { get; init; } = title;

    /// <summary>
    /// Gets the notification message text.
    /// </summary>
    public string Message { get; init; } = message;

    /// <summary>
    /// Deconstructs the content into its type, title and message.
    /// </summary>
    /// <param name="type">Receives the <see cref="Type"/> value.</param>
    /// <param name="title">Receives the <see cref="Title"/> value.</param>
    /// <param name="message">Receives the <see cref="Message"/> value.</param>
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