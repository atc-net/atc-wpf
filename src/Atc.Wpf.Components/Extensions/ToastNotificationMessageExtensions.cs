// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Components;

/// <summary>
/// Extension methods for <see cref="ToastNotificationMessage"/>.
/// </summary>
public static class ToastNotificationMessageExtensions
{
    /// <summary>
    /// Creates an <see cref="ApplicationEventEntry"/> from the toast notification message, using its title as area
    /// and mapping its type to a <see cref="LogCategoryType"/>.
    /// </summary>
    public static ApplicationEventEntry ToApplicationEventEntry(
        this ToastNotificationMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        return new ApplicationEventEntry(
            message.ToastNotificationType.ToLogCategoryType(),
            message.Title,
            message.Message);
    }
}