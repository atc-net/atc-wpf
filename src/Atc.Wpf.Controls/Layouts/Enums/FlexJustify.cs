namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies how children are justified along the main axis in a FlexPanel.
/// </summary>
public enum FlexJustify
{
    /// <summary>
    /// Children are packed at the start of the main axis.
    /// </summary>
    Start,

    /// <summary>
    /// Children are packed at the end of the main axis.
    /// </summary>
    End,

    /// <summary>
    /// Children are centered along the main axis.
    /// </summary>
    Center,

    /// <summary>
    /// Children are evenly distributed with the first child at the start and last child at the end.
    /// </summary>
    SpaceBetween,

    /// <summary>
    /// Children are evenly distributed with equal space around each child.
    /// </summary>
    SpaceAround,

    /// <summary>
    /// Children are evenly distributed with equal space between each child.
    /// </summary>
    SpaceEvenly,
}