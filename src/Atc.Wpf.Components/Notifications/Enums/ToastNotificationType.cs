// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Notifications;

/// <summary>
/// Specifies the kind of a toast notification.
/// </summary>
public enum ToastNotificationType
{
    /// <summary>An informational notification.</summary>
    Information,

    /// <summary>A notification that an operation succeeded.</summary>
    Success,

    /// <summary>A warning notification.</summary>
    Warning,

    /// <summary>An error notification.</summary>
    Error,
}