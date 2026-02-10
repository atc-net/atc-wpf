// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies the severity level of an <see cref="DataDisplay.Alert"/> control.
/// </summary>
public enum AlertSeverity
{
    /// <summary>
    /// Informational message.
    /// </summary>
    Info,

    /// <summary>
    /// Success or confirmation message.
    /// </summary>
    Success,

    /// <summary>
    /// Warning message requiring attention.
    /// </summary>
    Warning,

    /// <summary>
    /// Error or critical message.
    /// </summary>
    Error,
}