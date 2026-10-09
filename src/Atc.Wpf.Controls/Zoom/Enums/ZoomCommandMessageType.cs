namespace Atc.Wpf.Controls.Zoom;

/// <summary>Specifies the zoom action requested by a <see cref="Messages.ZoomCommandMessage"/>.</summary>
public enum ZoomCommandMessageType
{
    /// <summary>Zoom so that the content fills the viewport.</summary>
    ZoomToFill,

    /// <summary>Zoom so that the content fits inside the viewport.</summary>
    ZoomToFit,

    /// <summary>Zoom to the percentage given in the message.</summary>
    ZoomToPercentage,
}