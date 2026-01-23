namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies the direction in which children are laid out in a FlexPanel.
/// </summary>
public enum FlexDirection
{
    /// <summary>
    /// Children are laid out horizontally from left to right (main axis is horizontal).
    /// </summary>
    Row,

    /// <summary>
    /// Children are laid out vertically from top to bottom (main axis is vertical).
    /// </summary>
    Column,

    /// <summary>
    /// Children are laid out horizontally from right to left (main axis is horizontal, reversed).
    /// </summary>
    RowReverse,

    /// <summary>
    /// Children are laid out vertically from bottom to top (main axis is vertical, reversed).
    /// </summary>
    ColumnReverse,
}