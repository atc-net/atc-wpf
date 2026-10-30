namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies how many chips of a <see cref="DataDisplay.ChipGroup"/> can be selected.
/// </summary>
[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "OK.")]
public enum ChipSelectionMode
{
    /// <summary>
    /// Any number of chips can be selected; each chip toggles on its own.
    /// </summary>
    Multiple,

    /// <summary>
    /// Exactly one chip is selected once a choice is made, as in a radio group; clicking the selected chip keeps it.
    /// </summary>
    Single,

    /// <summary>
    /// At most one chip is selected; clicking the selected chip clears the selection.
    /// </summary>
    SingleOrNone,
}