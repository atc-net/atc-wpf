namespace Atc.Wpf.Controls.Zoom.Messages;

/// <summary>A message sent by a <see cref="ZoomBox"/> to report its current zoom level.</summary>
/// <param name="Percentage">The current zoom level, in percent.</param>
public record ZoomInformationMessage(
    double Percentage);