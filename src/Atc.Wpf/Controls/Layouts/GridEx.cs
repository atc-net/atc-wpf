namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// GridEx is a extension of the <see cref="Grid" />.
/// This GridEx supports easy definition of rows and columns.
/// </summary>
/// <example>
/// <![CDATA[
/// <!-- Example for https://www.wpf-tutorial.com/panels/grid-rows-and-columns
/// Gives a grid for "3*3" where cell [0,0] is double the size as cell [2,2].
/// -->
/// <Grid Rows="2*,1*,1*" Columns="2*,1*,1*">
/// <!-- grid content -->
/// </Grid>
///
/// <!-- Example for use of combination as:
/// - pixels
/// - auto (size to column content)
/// - * (star - size proportional to grid) -->
/// <atc:GridEx Rows="2*,Auto,*,66" Columns="2*,*,Auto">
/// <!-- grid content -->
/// </atc:GridEx>
/// ]]>
/// </example>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
public sealed partial class GridEx : Grid
{
    /// <summary>
    /// Gets or sets the row definitions as a comma-separated list of sizes (for example <c>2*,Auto,*,66</c>).
    /// </summary>
    [DependencyProperty(
        PropertyChangedCallback = nameof(OnRowsChanged),
        Category = "Layout",
        Description = "The rows property")]
    private string rows;

    /// <summary>
    /// Gets or sets the column definitions as a comma-separated list of sizes (for example <c>2*,*,Auto</c>).
    /// </summary>
    [DependencyProperty(
        PropertyChangedCallback = nameof(OnColumnsChanged),
        Category = "Layout",
        Description = "The columns property")]
    private string columns;

    /// <summary>
    /// Gets or sets the uniform spacing between grid cells, applied both horizontally and vertically.
    /// </summary>
    [DependencyProperty(
        DefaultValue = double.NaN,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure,
        Category = "Layout",
        Description = "Sets uniform spacing between grid cells (sets both horizontal and vertical)")]
    private double spacing;

    /// <summary>
    /// Gets or sets the horizontal spacing between grid columns.
    /// </summary>
    [DependencyProperty(
        DefaultValue = double.NaN,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure,
        Category = "Layout",
        Description = "Sets horizontal spacing between grid columns")]
    private double horizontalSpacing;

    /// <summary>
    /// Gets or sets the vertical spacing between grid rows.
    /// </summary>
    [DependencyProperty(
        DefaultValue = double.NaN,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure,
        Category = "Layout",
        Description = "Sets vertical spacing between grid rows")]
    private double verticalSpacing;

    /// <summary>
    /// Remembers the margin GridEx last applied to a child, so spacing changes can be re-applied
    /// without overwriting a margin the user set.
    /// </summary>
    [AttachedProperty(AccessModifier = PropertyAccessModifier.Private, GenerateClrAccessors = false)]
    private static Thickness? appliedSpacingMargin;

    /// <summary>
    /// Measures the children of a <see cref="Grid" /> in anticipation of arranging them during the ArrangeOverride pass.
    /// </summary>
    /// <param name="constraint">Indicates an upper limit size that should not be exceeded.</param>
    /// <returns>
    ///   <see cref="Size" /> that represents the required size to arrange child content.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
        ApplySpacingToChildren();
        return base.MeasureOverride(constraint);
    }

    /// <summary>
    /// Called when the rows property is changed.
    /// </summary>
    /// <param name="d">The dependency object.</param>
    /// <param name="e">The <see ref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
    private static void OnRowsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        // Get the gridEx.
        if (d is not GridEx gridEx)
        {
            return;
        }

        // Clear any current rows definitions.
        gridEx.RowDefinitions.Clear();

        // Add each row from the row lengths definition.
        foreach (var rowLength in StringLengthsToGridLengths(gridEx.Rows))
        {
            gridEx.RowDefinitions.Add(
                new RowDefinition
                {
                    Height = rowLength,
                });
        }
    }

    /// <summary>
    /// Called when the columns property is changed.
    /// </summary>
    /// <param name="d">The dependency object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
    private static void OnColumnsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        // Get the gridEx.
        if (d is not GridEx gridEx)
        {
            return;
        }

        // Clear any current rows definitions.
        gridEx.ColumnDefinitions.Clear();

        // Add each column from the column lengths definition.
        foreach (var columnLength in StringLengthsToGridLengths(gridEx.Columns))
        {
            gridEx.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = columnLength,
                });
        }
    }

    private static readonly GridLengthConverter SharedGridLengthConverter = new();

    /// <summary>
    /// Turns a string of lengths, such as "3*,Auto,2000" into a set of gridLength.
    /// </summary>
    /// <param name="lengths">The string of lengths, separated by commas.</param>
    /// <returns>A list of GridLengths.</returns>
    private static IEnumerable<GridLength> StringLengthsToGridLengths(
        string lengths)
    {
        // Create the list of GridLengths.
        var gridLengths = new List<GridLength>();

        // If the string is null or empty, this is all we can do.
        if (string.IsNullOrEmpty(lengths))
        {
            return gridLengths;
        }

        // Split the string by comma, ignoring surrounding spaces and empty tokens such as a trailing comma.
        var tokens = lengths.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        // Use the cached grid length converter to set each length.
        gridLengths.AddRange(tokens.Select(ParseGridLength));

        return gridLengths;
    }

    /// <summary>
    /// Parses a single length token such as "3*", "Auto" or "1.5*" using the invariant culture,
    /// falling back to <see cref="GridLength.Auto"/> for tokens that cannot be parsed.
    /// </summary>
    private static GridLength ParseGridLength(string token)
    {
        try
        {
            return SharedGridLengthConverter.ConvertFromInvariantString(token) is GridLength gridLength
                ? gridLength
                : GridLength.Auto;
        }
        catch (Exception ex) when (ex is FormatException or OverflowException or NotSupportedException or ArgumentException)
        {
            return GridLength.Auto;
        }
    }

    /// <summary>
    /// Gets the effective horizontal and vertical spacing values.
    /// Individual spacing properties (HorizontalSpacing, VerticalSpacing) take precedence over unified Spacing.
    /// </summary>
    /// <returns>A tuple containing the effective horizontal and vertical spacing.</returns>
    private (double Horizontal, double Vertical) GetEffectiveSpacing()
    {
        var horizontal = HorizontalSpacing;
        var vertical = VerticalSpacing;

        if (double.IsNaN(horizontal))
        {
            horizontal = double.IsNaN(Spacing) ? 0 : Spacing;
        }

        if (double.IsNaN(vertical))
        {
            vertical = double.IsNaN(Spacing) ? 0 : Spacing;
        }

        return (horizontal, vertical);
    }

    /// <summary>
    /// Applies spacing margins to all children based on their grid position.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each gap is split over the cells on both sides of it, in proportion to their position:
    /// a cell starting at track <c>i</c> of <c>n</c> and spanning <c>k</c> tracks gets a leading margin
    /// of <c>spacing * i / n</c> and a trailing margin of <c>spacing * (n - i - k) / n</c>. So every gap
    /// between neighbouring cells is exactly the spacing, the outer edges get none, and the content of
    /// equally sized (for example <c>*,*</c>) tracks gets equal sizes. With weighted star tracks
    /// (<c>2*,*</c>) the content keeps the gap but is only approximately proportional.
    /// </para>
    /// <para>
    /// The margin GridEx applies is remembered on each child, so a later change of spacing,
    /// rows, columns or cell position re-applies it. A margin the child got from anywhere else
    /// (local value, style, binding) is never overwritten, so such a child gets no spacing.
    /// </para>
    /// </remarks>
    private void ApplySpacingToChildren()
    {
        var (hSpacing, vSpacing) = GetEffectiveSpacing();
        var hasSpacing = hSpacing > 0 || vSpacing > 0;

        var colCount = ColumnDefinitions.Count;
        var rowCount = RowDefinitions.Count;

        foreach (UIElement child in Children)
        {
            if (child is not FrameworkElement fe)
            {
                continue;
            }

            if (!hasSpacing)
            {
                ApplySpacingMargin(fe, margin: null);
                continue;
            }

            var (left, right) = SplitGap(hSpacing, GetColumn(child), GetColumnSpan(child), colCount);
            var (top, bottom) = SplitGap(vSpacing, GetRow(child), GetRowSpan(child), rowCount);

            ApplySpacingMargin(fe, new Thickness(left, top, right, bottom));
        }
    }

    /// <summary>
    /// Gets the leading and trailing margin of a cell starting at track <paramref name="index"/> and
    /// spanning <paramref name="span"/> of <paramref name="trackCount"/> tracks, so each gap of
    /// <paramref name="spacing"/> is shared by the cells on both sides of it.
    /// </summary>
    internal static (double Leading, double Trailing) SplitGap(
        double spacing,
        int index,
        int span,
        int trackCount)
    {
        if (spacing <= 0 || trackCount <= 1)
        {
            return (0, 0);
        }

        var first = System.Math.Clamp(index, 0, trackCount - 1);
        var end = System.Math.Clamp(first + System.Math.Max(span, 1), first + 1, trackCount);

        return (
            spacing * first / trackCount,
            spacing * (trackCount - end) / trackCount);
    }

    /// <summary>
    /// Applies (or, when <paramref name="margin"/> is <see langword="null"/>, removes) the spacing margin
    /// of a child, unless the child's margin was set by someone other than GridEx.
    /// </summary>
    private static void ApplySpacingMargin(
        FrameworkElement fe,
        Thickness? margin)
    {
        var appliedMargin = (Thickness?)fe.GetValue(AppliedSpacingMarginProperty);
        var ownsMargin = appliedMargin.HasValue && fe.Margin == appliedMargin.Value;

        if (margin is null)
        {
            if (ownsMargin)
            {
                fe.ClearValue(MarginProperty);
            }

            fe.ClearValue(AppliedSpacingMarginProperty);
            return;
        }

        if (!ownsMargin &&
            DependencyPropertyHelper.GetValueSource(fe, MarginProperty).BaseValueSource is not BaseValueSource.Default)
        {
            // The margin was set by the user (or a style/binding) - leave it alone.
            fe.ClearValue(AppliedSpacingMarginProperty);
            return;
        }

        if (fe.Margin != margin.Value)
        {
            fe.SetValue(MarginProperty, margin.Value);
        }

        fe.SetValue(AppliedSpacingMarginProperty, margin.Value);
    }
}