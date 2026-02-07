// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies the content placement mode for a <see cref="DataDisplay.Timeline"/> control.
/// </summary>
public enum TimelineMode
{
    /// <summary>
    /// Content is placed on the left side of the timeline axis.
    /// </summary>
    Left,

    /// <summary>
    /// Content is placed on the right side of the timeline axis.
    /// </summary>
    Right,

    /// <summary>
    /// Content alternates between left and right sides.
    /// </summary>
    Alternate,
}