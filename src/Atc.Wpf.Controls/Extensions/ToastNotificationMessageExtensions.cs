// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Controls;

public static class ToastNotificationMessageExtensions
{
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