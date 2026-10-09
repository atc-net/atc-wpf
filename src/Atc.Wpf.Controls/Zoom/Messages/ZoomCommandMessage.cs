namespace Atc.Wpf.Controls.Zoom.Messages;

/// <summary>A message that asks a <see cref="ZoomBox"/> to perform a zoom action.</summary>
/// <param name="CommandType">The zoom action to perform.</param>
/// <param name="Percentage">The zoom percentage used by <see cref="ZoomCommandMessageType.ZoomToPercentage"/>.</param>
public record ZoomCommandMessage(
    ZoomCommandMessageType CommandType,
    decimal Percentage = 100);