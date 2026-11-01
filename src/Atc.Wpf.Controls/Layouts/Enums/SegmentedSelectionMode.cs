namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies how many segments of a <see cref="DataDisplay.Segmented"/> can be selected.
/// </summary>
[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "OK.")]
public enum SegmentedSelectionMode
{
    /// <summary>
    /// Exactly one segment is selected once a choice is made; clicking the selected segment keeps it (default).
    /// </summary>
    Single,

    /// <summary>
    /// At most one segment is selected; clicking the selected segment clears the selection.
    /// </summary>
    SingleOrNone,

    /// <summary>
    /// Any number of segments can be selected; each segment toggles on its own.
    /// </summary>
    Multiple,
}