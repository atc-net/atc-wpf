namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Specifies the placement of a <see cref="Popover"/> relative to its anchor element.
/// </summary>
public enum PopoverPlacement
{
    /// <summary>Above the anchor, aligned to its start edge.</summary>
    TopStart,

    /// <summary>Above the anchor, centered.</summary>
    Top,

    /// <summary>Above the anchor, aligned to its end edge.</summary>
    TopEnd,

    /// <summary>Below the anchor, aligned to its start edge.</summary>
    BottomStart,

    /// <summary>Below the anchor, centered.</summary>
    Bottom,

    /// <summary>Below the anchor, aligned to its end edge.</summary>
    BottomEnd,

    /// <summary>Left of the anchor, aligned to its top edge.</summary>
    LeftStart,

    /// <summary>Left of the anchor, centered.</summary>
    Left,

    /// <summary>Left of the anchor, aligned to its bottom edge.</summary>
    LeftEnd,

    /// <summary>Right of the anchor, aligned to its top edge.</summary>
    RightStart,

    /// <summary>Right of the anchor, centered.</summary>
    Right,

    /// <summary>Right of the anchor, aligned to its bottom edge.</summary>
    RightEnd,
}