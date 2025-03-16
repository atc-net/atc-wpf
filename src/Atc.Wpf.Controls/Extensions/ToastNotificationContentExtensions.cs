// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Controls;

public static class ToastNotificationContentExtensions
{
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