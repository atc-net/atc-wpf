namespace Atc.Wpf.Components.Selectors;

/// <summary>
/// Specifies how the rows of a <see cref="DualListSelector"/> show that they are picked for a transfer.
/// </summary>
public enum DualListItemSelectionStyle
{
    /// <summary>
    /// Picked rows are highlighted; <c>Ctrl</c>/<c>Shift</c>-click picks several (the classic list box look).
    /// </summary>
    Highlight,

    /// <summary>
    /// Each row has a check box, and each list header has a tri-state "Select all" check box with a "checked / total" count.
    /// A plain click toggles a row.
    /// </summary>
    CheckBox,
}