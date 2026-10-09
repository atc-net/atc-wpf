// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Components;

/// <summary>
/// Extension methods for <see cref="ToastNotificationType"/>.
/// </summary>
public static class ToastNotificationTypeExtensions
{
    /// <summary>
    /// Maps a <see cref="ToastNotificationType"/> to the matching <see cref="LogCategoryType"/>
    /// (<see cref="ToastNotificationType.Success"/> maps to <see cref="LogCategoryType.Information"/>).
    /// </summary>
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