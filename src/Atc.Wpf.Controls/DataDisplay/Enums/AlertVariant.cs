// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies the visual variant of an <see cref="DataDisplay.Alert"/> control.
/// </summary>
public enum AlertVariant
{
    /// <summary>
    /// Solid background with severity color.
    /// </summary>
    Filled,

    /// <summary>
    /// Transparent background with severity-colored border and text.
    /// </summary>
    Outlined,

    /// <summary>
    /// No border or background, severity-colored text only.
    /// </summary>
    Text,
}