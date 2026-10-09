// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Components;

/// <summary>
/// Extension methods for creating <see cref="ToastNotificationContent"/> instances.
/// </summary>
public static class ToastNotificationContentExtensions
{
    /// <summary>
    /// Creates a <see cref="ToastNotificationContent"/> with the type, title and message of the given toast notification message.
    /// </summary>
    public static ToastNotificationContent ToToastNotificationContent(
        this ToastNotificationMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        return new ToastNotificationContent(
            message.ToastNotificationType,
            message.Title,
            message.Message);
    }
}