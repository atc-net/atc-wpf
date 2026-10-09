namespace Atc.Wpf.Controls.Zoom;

/// <summary>Specifies how a <see cref="ZoomBox"/> positions its content when it is first loaded.</summary>
public enum ZoomInitialPositionType
{
    /// <summary>No initial positioning.</summary>
    Default,

    /// <summary>Zoom so that the content fits the screen.</summary>
    FitScreen,

    /// <summary>Zoom so that the content fills the screen.</summary>
    FillScreen,

    /// <summary>Zoom to 100% with the content centered.</summary>
    OneHundredPercentCentered,
}