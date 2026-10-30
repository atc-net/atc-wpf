namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Specifies which keys of a tri-state check tree (<see cref="CheckTreeView"/>, <c>TreeSelectComboBox</c>) end
/// up in <c>SelectedKeys</c> when the check boxes are linked.
/// </summary>
public enum CheckedKeysStrategy
{
    /// <summary>Only ticked leaf nodes (nodes without loaded children).</summary>
    Leaves,

    /// <summary>A fully ticked branch collapses to its top-most fully ticked node.</summary>
    Parents,

    /// <summary>Every ticked node, parents and leaves.</summary>
    All,
}