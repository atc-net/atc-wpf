namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Specifies how a tri-state check tree (<see cref="CheckTreeView"/>, <c>TreeSelectComboBox</c>) shows the
/// nodes that match the filter text.
/// </summary>
public enum TreeFilterMode
{
    /// <summary>
    /// The tree keeps its shape: matching nodes (with everything under them) and their ancestors stay visible, and
    /// the ancestors are expanded.
    /// </summary>
    KeepAncestors,

    /// <summary>The matching nodes are listed flat, each with its path as a sub-line.</summary>
    FlatResults,
}