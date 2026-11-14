namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// Computes where a <see cref="DotMatrixDisplayBase"/> draws its bezel, panel and dots for a render size.
/// </summary>
/// <remarks>
/// The module is laid out in units of one dot size: the dots with their gaps, a panel margin around them
/// and an optional bezel around the panel. The module is then scaled to the render size as <see cref="Stretch"/> says and centred.
/// </remarks>
internal sealed class DotMatrixLayout
{
    /// <summary>The distance between two dot centres, in DIP, at the natural size.</summary>
    internal const double NaturalDotPitch = 3.0;

    /// <summary>The margin of panel around the dots, in dot sizes.</summary>
    internal const double PanelMargin = 2.0;

    /// <summary>The width of the bezel around the panel, in dot sizes.</summary>
    internal const double BezelWidth = 3.0;

    private readonly double[] dotX;
    private readonly double[] dotY;

    private DotMatrixLayout(
        double[] dotX,
        double[] dotY,
        double scaleX,
        double scaleY,
        Rect moduleRect,
        Rect panelRect)
    {
        this.dotX = dotX;
        this.dotY = dotY;
        ScaleX = scaleX;
        ScaleY = scaleY;
        ModuleRect = moduleRect;
        PanelRect = panelRect;
    }

    /// <summary>Gets the horizontal size of one dot, in DIP.</summary>
    public double ScaleX { get; }

    /// <summary>Gets the vertical size of one dot, in DIP.</summary>
    public double ScaleY { get; }

    /// <summary>Gets the bounds of the whole module, bezel included.</summary>
    public Rect ModuleRect { get; }

    /// <summary>Gets the bounds of the panel (the glass inside the bezel).</summary>
    public Rect PanelRect { get; }

    /// <summary>Gets the number of dot columns.</summary>
    public int DotColumns => dotX.Length;

    /// <summary>Gets the number of dot rows.</summary>
    public int DotRows => dotY.Length;

    /// <summary>Gets a value indicating whether there is nothing to draw.</summary>
    public bool IsEmpty
        => DotColumns == 0 ||
           DotRows == 0 ||
           ScaleX <= 0 ||
           ScaleY <= 0;

    /// <summary>
    /// Gets the size the module takes at its natural dot pitch of <see cref="NaturalDotPitch"/> DIP.
    /// </summary>
    /// <param name="geometry">The dot grid.</param>
    /// <param name="showBezel">Whether the bezel is drawn.</param>
    /// <returns>The natural size, or an empty size when the grid has no dots.</returns>
    public static Size GetNaturalSize(
        DotMatrixGeometry geometry,
        bool showBezel)
    {
        ArgumentNullException.ThrowIfNull(geometry);

        var units = GetModuleUnits(geometry, showBezel);
        if (units.Width <= 0 || units.Height <= 0)
        {
            return default;
        }

        var scale = GetNaturalScale(geometry);
        return new Size(units.Width * scale, units.Height * scale);
    }

    /// <summary>
    /// Gets the size the module wants for the available size, like an <see cref="Image"/> with the same <see cref="Stretch"/>.
    /// </summary>
    /// <param name="geometry">The dot grid.</param>
    /// <param name="showBezel">Whether the bezel is drawn.</param>
    /// <param name="availableSize">The available size; either dimension may be infinite.</param>
    /// <param name="stretch">How the module fills the available size.</param>
    /// <returns>The desired size.</returns>
    public static Size Measure(
        DotMatrixGeometry geometry,
        bool showBezel,
        Size availableSize,
        Stretch stretch)
    {
        var natural = GetNaturalSize(geometry, showBezel);
        if (natural.Width <= 0 || natural.Height <= 0)
        {
            return default;
        }

        var (scaleX, scaleY) = ComputeScale(availableSize, natural, stretch);
        return new Size(natural.Width * scaleX, natural.Height * scaleY);
    }

    /// <summary>
    /// Lays out the module in a render size.
    /// </summary>
    /// <param name="geometry">The dot grid.</param>
    /// <param name="showBezel">Whether the bezel is drawn.</param>
    /// <param name="renderSize">The size of the element.</param>
    /// <param name="stretch">How the module fills the render size.</param>
    /// <returns>The layout.</returns>
    public static DotMatrixLayout Create(
        DotMatrixGeometry geometry,
        bool showBezel,
        Size renderSize,
        Stretch stretch)
    {
        ArgumentNullException.ThrowIfNull(geometry);

        var units = GetModuleUnits(geometry, showBezel);
        if (units.Width <= 0 ||
            units.Height <= 0 ||
            renderSize.Width <= 0 ||
            renderSize.Height <= 0)
        {
            return new DotMatrixLayout([], [], 0, 0, Rect.Empty, Rect.Empty);
        }

        var (scaleX, scaleY) = stretch == Stretch.None
            ? (GetNaturalScale(geometry), GetNaturalScale(geometry))
            : ComputeScale(renderSize, units, stretch);

        var moduleWidth = units.Width * scaleX;
        var moduleHeight = units.Height * scaleY;
        var moduleRect = new Rect(
            (renderSize.Width - moduleWidth) / 2,
            (renderSize.Height - moduleHeight) / 2,
            moduleWidth,
            moduleHeight);

        var bezel = showBezel ? BezelWidth : 0;
        var panelRect = new Rect(
            moduleRect.X + (bezel * scaleX),
            moduleRect.Y + (bezel * scaleY),
            moduleWidth - (2 * bezel * scaleX),
            moduleHeight - (2 * bezel * scaleY));

        var dotGap = NonNegative(geometry.DotGap);
        var columnPositions = ToElementPositions(
            GetDotPositions(geometry.DotColumns, geometry.CellWidth, dotGap, geometry.CellGapX),
            panelRect.X + (PanelMargin * scaleX),
            scaleX);
        var rowPositions = ToElementPositions(
            GetDotPositions(geometry.DotRows, geometry.CellHeight, dotGap, geometry.CellGapY),
            panelRect.Y + (PanelMargin * scaleY),
            scaleY);

        return new DotMatrixLayout(columnPositions, rowPositions, scaleX, scaleY, moduleRect, panelRect);
    }

    /// <summary>
    /// Gets the rectangle of one dot.
    /// </summary>
    /// <param name="x">The dot column.</param>
    /// <param name="y">The dot row.</param>
    /// <returns>The dot rectangle in element coordinates.</returns>
    public Rect GetDotRect(
        int x,
        int y)
        => new(dotX[x], dotY[y], ScaleX, ScaleY);

    private static (double ScaleX, double ScaleY) ComputeScale(
        Size availableSize,
        Size contentSize,
        Stretch stretch)
    {
        if (stretch == Stretch.None)
        {
            return (1, 1);
        }

        var isWidthInfinite = double.IsPositiveInfinity(availableSize.Width);
        var isHeightInfinite = double.IsPositiveInfinity(availableSize.Height);
        if (isWidthInfinite && isHeightInfinite)
        {
            return (1, 1);
        }

        var scaleX = isWidthInfinite ? 0 : availableSize.Width / contentSize.Width;
        var scaleY = isHeightInfinite ? 0 : availableSize.Height / contentSize.Height;

        if (isWidthInfinite)
        {
            scaleX = scaleY;
        }
        else if (isHeightInfinite)
        {
            scaleY = scaleX;
        }
        else
        {
            switch (stretch)
            {
                case Stretch.Uniform:
                    scaleX = System.Math.Min(scaleX, scaleY);
                    scaleY = scaleX;
                    break;
                case Stretch.UniformToFill:
                    scaleX = System.Math.Max(scaleX, scaleY);
                    scaleY = scaleX;
                    break;
            }
        }

        return (scaleX, scaleY);
    }

    private static Size GetModuleUnits(
        DotMatrixGeometry geometry,
        bool showBezel)
    {
        var columns = System.Math.Max(0, geometry.DotColumns);
        var rows = System.Math.Max(0, geometry.DotRows);
        if (columns == 0 || rows == 0)
        {
            return default;
        }

        var dotGap = NonNegative(geometry.DotGap);
        var dotsWidth = GetDotPositions(columns, geometry.CellWidth, dotGap, geometry.CellGapX)[^1] + 1;
        var dotsHeight = GetDotPositions(rows, geometry.CellHeight, dotGap, geometry.CellGapY)[^1] + 1;
        var frame = 2 * (PanelMargin + (showBezel ? BezelWidth : 0));

        return new Size(dotsWidth + frame, dotsHeight + frame);
    }

    private static double[] ToElementPositions(
        double[] unitPositions,
        double origin,
        double scale)
    {
        var positions = new double[unitPositions.Length];
        for (var i = 0; i < positions.Length; i++)
        {
            positions[i] = origin + (unitPositions[i] * scale);
        }

        return positions;
    }

    private static double[] GetDotPositions(
        int count,
        int cellSize,
        double dotGap,
        double cellGap)
    {
        var gapAfterCell = NonNegative(cellGap);
        var positions = new double[System.Math.Max(0, count)];
        var position = 0.0;
        for (var i = 0; i < positions.Length; i++)
        {
            positions[i] = position;
            var isCellEnd = cellSize > 0 && (i + 1) % cellSize == 0;
            position += 1 + (isCellEnd ? gapAfterCell : dotGap);
        }

        return positions;
    }

    private static double GetNaturalScale(DotMatrixGeometry geometry)
        => NaturalDotPitch / (1 + NonNegative(geometry.DotGap));

    private static double NonNegative(double value)
        => double.IsFinite(value) && value > 0
            ? value
            : 0;
}