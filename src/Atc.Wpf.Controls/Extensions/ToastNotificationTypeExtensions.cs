// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Controls;

public static class ToastNotificationTypeExtensions
{
    public static LogCategoryType ToLogCategoryType(
        this ToastNotificationType toastNotificationType)
        => toastNotificationType switch
        {
            ToastNotificationType.Information or ToastNotificationType.Success => LogCategoryType.Information,
            ToastNotificationType.Warning => LogCategoryType.Warning,
            ToastNotificationType.Error => LogCategoryType.Error,
            _ => throw new SwitchCaseDefaultException(toastNotificationType),
        };
}