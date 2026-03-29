namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// Identifies a zoom action that can be bound to a keyboard shortcut.
/// </summary>
public enum ZoomActionType
{
    /// <summary>Zoom in by one step.</summary>
    ZoomIn,

    /// <summary>Zoom out by one step.</summary>
    ZoomOut,

    /// <summary>Zoom to fill the viewport.</summary>
    ZoomFill,

    /// <summary>Zoom to fit content in the viewport.</summary>
    ZoomFit,

    /// <summary>Zoom to 100%.</summary>
    Zoom100Percent,

    /// <summary>Zoom to the next higher preset level.</summary>
    ZoomToNextPreset,

    /// <summary>Zoom to the next lower preset level.</summary>
    ZoomToPreviousPreset,

    /// <summary>Undo the last zoom/pan change.</summary>
    UndoZoom,

    /// <summary>Redo the last undone zoom/pan change.</summary>
    RedoZoom,
}