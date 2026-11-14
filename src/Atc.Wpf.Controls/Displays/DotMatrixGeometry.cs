namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// Describes the dot grid a <see cref="DotMatrixDisplayBase"/> draws: its size in dots and the gaps between dots and cells.
/// </summary>
/// <remarks>
/// All gaps are fractions of the dot size: a <see cref="DotGap"/> of 0.1 leaves a tenth of a dot between two dots.
/// A <see cref="CellWidth"/> or <see cref="CellHeight"/> of 0 means the grid has no cells in that direction,
/// as on a graphic display; otherwise <see cref="CellGapX"/> and <see cref="CellGapY"/> replace the dot gap between two cells.
/// </remarks>
/// <param name="DotColumns">The number of dot columns of the whole grid.</param>
/// <param name="DotRows">The number of dot rows of the whole grid.</param>
/// <param name="CellWidth">The width of a cell in dots, or 0 for no cells horizontally.</param>
/// <param name="CellHeight">The height of a cell in dots, or 0 for no cells vertically.</param>
/// <param name="DotGap">The gap between two neighbouring dots, as a fraction of the dot size.</param>
/// <param name="CellGapX">The horizontal gap between two cells, as a fraction of the dot size.</param>
/// <param name="CellGapY">The vertical gap between two cells, as a fraction of the dot size.</param>
public sealed record DotMatrixGeometry(
    int DotColumns,
    int DotRows,
    int CellWidth,
    int CellHeight,
    double DotGap,
    double CellGapX,
    double CellGapY);