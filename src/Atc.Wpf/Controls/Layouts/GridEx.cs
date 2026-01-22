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
public sealed class GridEx : Grid
{
    public static readonly DependencyProperty RowsProperty = DependencyProperty.Register(
        nameof(Rows),
        typeof(string),
        typeof(GridEx),
        new PropertyMetadata(default(string), OnRowsChanged));

    public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
        nameof(Columns),
        typeof(string),
        typeof(GridEx),
        new PropertyMetadata(default(string), OnColumnsChanged));

    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
        nameof(Spacing),
        typeof(double),
        typeof(GridEx),
        new FrameworkPropertyMetadata(
            double.NaN,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty HorizontalSpacingProperty = DependencyProperty.Register(
        nameof(HorizontalSpacing),
        typeof(double),
        typeof(GridEx),
        new FrameworkPropertyMetadata(
            double.NaN,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty VerticalSpacingProperty = DependencyProperty.Register(
        nameof(VerticalSpacing),
        typeof(double),
        typeof(GridEx),
        new FrameworkPropertyMetadata(
            double.NaN,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

    [Category("Layout")]
    [Description("The rows property")]
    public string Rows
    {
        get => (string)GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    [Category("Layout")]
    [Description("The columns property")]
    public string Columns
    {
        get => (string)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    [Category("Layout")]
    [Description("Sets uniform spacing between grid cells (sets both horizontal and vertical)")]
    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    [Category("Layout")]
    [Description("Sets horizontal spacing between grid columns")]
    public double HorizontalSpacing
    {
        get => (double)GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    [Category("Layout")]
    [Description("Sets vertical spacing between grid rows")]
    public double VerticalSpacing
    {
        get => (double)GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

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

        // Split the string by comma.
        var sa = lengths.Split(',');

        // Create a grid length converter.
        var gridLengthConverter = new GridLengthConverter();

        // Use the grid length converter to set each length.
        gridLengths.AddRange(sa.Select(length =>
        {
            var convertFromString = gridLengthConverter.ConvertFromString(length);
            if (convertFromString is GridLength gridLength)
            {
                return gridLength;
            }

            return GridLength.Auto;
        }));

        return gridLengths;
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
    private void ApplySpacingToChildren()
    {
        var (hSpacing, vSpacing) = GetEffectiveSpacing();
        if (hSpacing <= 0 && vSpacing <= 0)
        {
            return;
        }

        var colCount = ColumnDefinitions.Count;
        var rowCount = RowDefinitions.Count;

        foreach (UIElement child in Children)
        {
            if (child is not FrameworkElement fe)
            {
                continue;
            }

            var col = GetColumn(child);
            var row = GetRow(child);
            var colSpan = GetColumnSpan(child);
            var rowSpan = GetRowSpan(child);

            var isLastColumn = colCount == 0 || col + colSpan >= colCount;
            var isLastRow = rowCount == 0 || row + rowSpan >= rowCount;

            var spacingMargin = new Thickness(
                0,
                0,
                isLastColumn ? 0 : hSpacing,
                isLastRow ? 0 : vSpacing);

            fe.SetIfDefault(MarginProperty, spacingMargin);
        }
    }
}