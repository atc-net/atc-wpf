namespace Atc.Wpf.Controls.Zoom.Messages;

public record ZoomCommandMessage(
    ZoomCommandMessageType CommandType,
    decimal Percentage = 100);