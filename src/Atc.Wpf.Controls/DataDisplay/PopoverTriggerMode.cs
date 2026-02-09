namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Specifies how a <see cref="Popover"/> is triggered to open.
/// </summary>
public enum PopoverTriggerMode
{
    /// <summary>
    /// The popover opens/closes when the anchor is clicked.
    /// </summary>
    Click,

    /// <summary>
    /// The popover opens on mouse hover and closes when the mouse leaves.
    /// </summary>
    Hover,

    /// <summary>
    /// The popover is only controlled programmatically via the <see cref="Popover.IsOpen"/> property.
    /// </summary>
    Manual,
}