namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Defines a flexible grid area that consists of columns and rows.
/// Depending on the orientation, either the rows or the columns are auto-generated,
/// and the children's position is set according to their index.
///
/// Partially based on work at http://rachel53461.wordpress.com/2011/09/17/wpf-grids-rowcolumn-count-properties.
/// </summary>
public sealed class AutoGrid : Grid
{
    /// <summary>
    /// The child horizontal alignment property
    /// </summary>
    public static readonly DependencyProperty ChildHorizontalAlignmentProperty = DependencyProperty.Register(
        nameof(ChildHorizontalAlignment),
        typeof(HorizontalAlignment?),
        typeof(AutoGrid),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            OnChildHorizontalAlignmentChanged));

    /// <summary>
    /// The child margin property
    /// </summary>
    public static readonly DependencyProperty ChildMarginProperty = DependencyProperty.Register(
        nameof(ChildMargin),
        typeof(Thickness?),
        typeof(AutoGrid),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            OnChildMarginChanged));

    /// <summary>
    /// The child vertical alignment property
    /// </summary>
    public static readonly DependencyProperty ChildVerticalAlignmentProperty = DependencyProperty.Register(
        nameof(ChildVerticalAlignment),
        typeof(VerticalAlignment?),
        typeof(AutoGrid),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            OnChildVerticalAlignmentChanged));

    /// <summary>
    /// The column count property
    /// </summary>
    public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.RegisterAttached(
        nameof(ColumnCount),
        typeof(int),
        typeof(AutoGrid),
        new FrameworkPropertyMetadata(
            1,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            ColumnCountChanged));

    /// <summary>
    /// The columns property
    /// </summary>
    public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached(
        nameof(Columns),
        typeof(string),
        typeof(AutoGrid),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            ColumnsChanged));

    /// <summary>
    /// The column width property
    /// </summary>
    public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.RegisterAttached(
        nameof(ColumnWidth),
        typeof(GridLength),
        typeof(AutoGrid),
        new FrameworkPropertyMetadata(
            GridLength.Auto,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            FixedColumnWidthChanged));

    /// <summary>
    /// The is automatic indexing property
    /// </summary>
    public static readonly DependencyProperty IsAutoIndexingProperty = DependencyProperty.Register(
        nameof(IsAutoIndexing),
        typeof(bool),
        typeof(AutoGrid),
        new FrameworkPropertyMetadata(
            defaultValue: true,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// The orientation property
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(AutoGrid),
        new FrameworkPropertyMetadata(
            Orientation.Horizontal,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// The row count property
    /// </summary>
    public static readonly DependencyProperty RowCountProperty = DependencyProperty.RegisterAttached(
        nameof(RowCount),
        typeof(int),
        typeof(AutoGrid),
        new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure, RowCountChanged));

    /// <summary>
    /// The row height property
    /// </summary>
    public static readonly DependencyProperty RowHeightProperty = DependencyProperty.RegisterAttached(
        nameof(RowHeight),
        typeof(GridLength),
        typeof(AutoGrid),
        new FrameworkPropertyMetadata(
            GridLength.Auto,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            FixedRowHeightChanged));

    /// <summary>
    /// The rows property
    /// </summary>
    public static readonly DependencyProperty RowsProperty = DependencyProperty.RegisterAttached(
        nameof(Rows),
        typeof(string),
        typeof(AutoGrid),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            RowsChanged));

    /// <summary>
    /// Gets or sets the child horizontal alignment.
    /// </summary>
    /// <value>The child horizontal alignment.</value>
    [Category("Layout")]
    [Description("Presets the horizontal alignment of all child controls")]
    public HorizontalAlignment? ChildHorizontalAlignment
    {
        get => (HorizontalAlignment?)GetValue(ChildHorizontalAlignmentProperty);
        set => SetValue(ChildHorizontalAlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets the child margin.
    /// </summary>
    /// <value>The child margin.</value>
    [Category("Layout")]
    [Description("Presets the margin of all child controls")]
    public Thickness? ChildMargin
    {
        get => (Thickness?)GetValue(ChildMarginProperty);
        set => SetValue(ChildMarginProperty, value);
    }

    /// <summary>
    /// Gets or sets the child vertical alignment.
    /// </summary>
    /// <value>The child vertical alignment.</value>
    [Category("Layout")]
    [Description("Presets the vertical alignment of all child controls")]
    public VerticalAlignment? ChildVerticalAlignment
    {
        get => (VerticalAlignment?)GetValue(ChildVerticalAlignmentProperty);
        set => SetValue(ChildVerticalAlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets the column count.
    /// </summary>
    [Category("Layout")]
    [Description("Defines a set number of columns")]
    public int ColumnCount
    {
        get => (int)GetValue(ColumnCountProperty);
        set => SetValue(ColumnCountProperty, value);
    }

    /// <summary>
    /// Gets or sets the columns.
    /// </summary>
    [Category("Layout")]
    [Description("Defines all columns using comma separated grid length notation")]
    public string Columns
    {
        get => (string)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    /// <summary>
    /// Gets or sets the fixed column width.
    /// </summary>
    [Category("Layout")]
    [Description("Presets the width of all columns set using the ColumnCount property")]
    public GridLength ColumnWidth
    {
        get => (GridLength)GetValue(ColumnWidthProperty);
        set => SetValue(ColumnWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the children are automatically indexed.
    /// <remarks>
    /// The default is <see langword="true" />.
    /// Note that if children are already indexed, setting this property to <see langword="false" /> will not remove their indices.
    /// </remarks>
    /// </summary>
    [Category("Layout")]
    [Description("Set to false to disable the auto layout functionality")]
    public bool IsAutoIndexing
    {
        get => (bool)GetValue(IsAutoIndexingProperty);
        set => SetValue(IsAutoIndexingProperty, value);
    }

    /// <summary>
    /// Gets or sets the orientation.
    /// <remarks>The default is Vertical.</remarks>
    /// </summary>
    /// <value>The orientation.</value>
    [Category("Layout")]
    [Description("Defines the directionality of the autolayout. Use vertical for a column first layout, horizontal for a row first layout.")]
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of rows.
    /// </summary>
    [Category("Layout")]
    [Description("Defines a set number of rows")]
    public int RowCount
    {
        get => (int)GetValue(RowCountProperty);
        set => SetValue(RowCountProperty, value);
    }

    /// <summary>
    /// Gets or sets the fixed row height.
    /// </summary>
    [Category("Layout")]
    [Description("Presets the height of all rows set using the RowCount property")]
    public GridLength RowHeight
    {
        get => (GridLength)GetValue(RowHeightProperty);
        set => SetValue(RowHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the rows.
    /// </summary>
    [Category("Layout")]
    [Description("Defines all rows using comma separated grid length notation")]
    public string Rows
    {
        get => (string)GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    /// <summary>
    /// Handles the column count changed event.
    /// </summary>
    /// <param name="d">The object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    public static void ColumnCountChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if ((int)e.NewValue < 0)
        {
            return;
        }

        if (d is not AutoGrid grid)
        {
            return;
        }

        // Look for an existing column definition for the height
        var width = GridLength.Auto;
        if (grid.ColumnDefinitions.Count > 0)
        {
            width = grid.ColumnDefinitions[0].Width;
        }

        // Clear and rebuild
        grid.ColumnDefinitions.Clear();
        for (var i = 0; i < (int)e.NewValue; i++)
        {
            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = width,
                });
        }
    }

    /// <summary>
    /// Handle the columns changed event.
    /// </summary>
    /// <param name="d">The object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    public static void ColumnsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (string.Empty.Equals((string)e.NewValue, StringComparison.Ordinal))
        {
            return;
        }

        if (d is not AutoGrid grid)
        {
            return;
        }

        grid.ColumnDefinitions.Clear();

        var columnDefinitions = Parse((string)e.NewValue);
        foreach (var columnDefinition in columnDefinitions)
        {
            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = columnDefinition,
                });
        }
    }

    /// <summary>
    /// Handle the fixed column width changed event.
    /// </summary>
    /// <param name="d">The object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    public static void FixedColumnWidthChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not AutoGrid grid)
        {
            return;
        }

        // Add a default column if missing
        if (grid.ColumnDefinitions.Count == 0)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        // Set all existing columns to this width
        foreach (var columnDefinition in grid.ColumnDefinitions)
        {
            columnDefinition.Width = (GridLength)e.NewValue;
        }
    }

    /// <summary>
    /// Handle the fixed row height changed event.
    /// </summary>
    /// <param name="d">The object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    public static void FixedRowHeightChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not AutoGrid grid)
        {
            return;
        }

        // Add a default row if missing
        if (grid.RowDefinitions.Count == 0)
        {
            grid.RowDefinitions.Add(new RowDefinition());
        }

        // Set all existing rows to this height
        foreach (var rowDefinition in grid.RowDefinitions)
        {
            rowDefinition.Height = (GridLength)e.NewValue;
        }
    }

    /// <summary>
    /// Parse an array of grid lengths from comma delimited text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>Return the GridLength array.</returns>
    public static GridLength[] Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var tokens = text.Split(',');
        var definitions = new GridLength[tokens.Length];
        for (var i = 0; i < tokens.Length; i++)
        {
            var str = tokens[i];
            double value;

            // Ratio
            if (str.Contains('*', StringComparison.Ordinal))
            {
                var s = str.Replace("*", string.Empty, StringComparison.Ordinal);
                if (!double.TryParse(s, NumberStyles.None, GlobalizationConstants.EnglishCultureInfo, out value))
                {
                    value = 1.0;
                }

                definitions[i] = new GridLength(value, GridUnitType.Star);
                continue;
            }

            // Pixels
            if (double.TryParse(str, NumberStyles.None, GlobalizationConstants.EnglishCultureInfo, out value))
            {
                definitions[i] = new GridLength(value);
                continue;
            }

            // Auto
            definitions[i] = GridLength.Auto;
        }

        return definitions;
    }

    /// <summary>
    /// Handles the row count changed event.
    /// </summary>
    /// <param name="d">The object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    public static void RowCountChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if ((int)e.NewValue < 0)
        {
            return;
        }

        if (d is not AutoGrid grid)
        {
            return;
        }

        // Look for an existing row to get the height
        var height = GridLength.Auto;
        if (grid.RowDefinitions.Count > 0)
        {
            height = grid.RowDefinitions[0].Height;
        }

        // Clear and rebuild
        grid.RowDefinitions.Clear();
        for (var i = 0; i < (int)e.NewValue; i++)
        {
            grid.RowDefinitions.Add(
                new RowDefinition
                {
                    Height = height,
                });
        }
    }

    /// <summary>
    /// Handle the rows changed event.
    /// </summary>
    /// <param name="d">The object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    public static void RowsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (string.Empty.Equals((string)e.NewValue, StringComparison.Ordinal))
        {
            return;
        }

        if (d is not AutoGrid grid)
        {
            return;
        }

        grid.RowDefinitions.Clear();

        var rowDefinitions = Parse((string)e.NewValue);
        foreach (var rowDefinition in rowDefinitions)
        {
            grid.RowDefinitions.Add(
                new RowDefinition
                {
                    Height = rowDefinition,
                });
        }
    }

    /// <summary>
    /// Measures the children of a <see cref="Grid" /> in anticipation of arranging them during the <see cref="Grid.ArrangeOverride(Size)" /> pass.
    /// </summary>
    /// <param name="constraint">Indicates an upper limit size that should not be exceeded.</param>
    /// <returns>
    ///   <see cref="Size" /> that represents the required size to arrange child content.
    /// </returns>
    protected override Size MeasureOverride(
        Size constraint)
    {
        PerformLayout();
        return base.MeasureOverride(constraint);
    }

    /// <summary>
    /// Called when [child horizontal alignment changed].
    /// </summary>
    private static void OnChildHorizontalAlignmentChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not AutoGrid grid)
        {
            return;
        }

        foreach (UIElement child in grid.Children)
        {
            child.SetValue(HorizontalAlignmentProperty, grid.ChildHorizontalAlignment ?? DependencyProperty.UnsetValue);
        }
    }

    /// <summary>
    /// Called when [child layout changed].
    /// </summary>
    private static void OnChildMarginChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not AutoGrid grid)
        {
            return;
        }

        foreach (UIElement child in grid.Children)
        {
            child.SetValue(MarginProperty, grid.ChildMargin ?? DependencyProperty.UnsetValue);
        }
    }

    /// <summary>
    /// Called when [child vertical alignment changed].
    /// </summary>
    private static void OnChildVerticalAlignmentChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not AutoGrid grid)
        {
            return;
        }

        foreach (UIElement child in grid.Children)
        {
            child.SetValue(VerticalAlignmentProperty, grid.ChildVerticalAlignment ?? DependencyProperty.UnsetValue);
        }
    }

    /// <summary>
    /// Clamp a value to its maximum.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="max">The maximum.</param>
    /// <returns>Return the biggest integer.</returns>
    private static int Clamp(
        int value,
        int max)
    {
        return value > max
            ? max
            : value;
    }

    /// <summary>
    /// Apply child margins and layout effects such as alignment.
    /// </summary>
    private void ApplyChildLayout(
        DependencyObject d)
    {
        if (ChildMargin is not null)
        {
            _ = d.SetIfDefault(MarginProperty, ChildMargin.Value);
        }

        if (ChildHorizontalAlignment is not null)
        {
            _ = d.SetIfDefault(HorizontalAlignmentProperty, ChildHorizontalAlignment.Value);
        }

        if (ChildVerticalAlignment is not null)
        {
            _ = d.SetIfDefault(VerticalAlignmentProperty, ChildVerticalAlignment.Value);
        }
    }

    /// <summary>
    /// Perform the grid layout of row and column indexes.
    /// </summary>
    [SuppressMessage("Performance", "CA1814:Prefer jagged arrays over multidimensional", Justification = "OK.")]
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private void PerformLayout()
    {
        var fillRowFirst = Orientation == Orientation.Horizontal;
        var rowCount = RowDefinitions.Count;
        var colCount = ColumnDefinitions.Count;

        if (rowCount == 0 || colCount == 0)
        {
            return;
        }

        var position = 0;
        var skip = new bool[rowCount, colCount];
        foreach (UIElement child in Children)
        {
            var childIsCollapsed = child.Visibility == Visibility.Collapsed;
            if (IsAutoIndexing && !childIsCollapsed)
            {
                if (fillRowFirst)
                {
                    var row = Clamp(position / colCount, rowCount - 1);
                    var col = Clamp(position % colCount, colCount - 1);
                    if (skip[row, col])
                    {
                        position++;
                        row = position / colCount;
                        col = position % colCount;
                    }

                    SetRow(child, row);
                    SetColumn(child, col);
                    position += GetColumnSpan(child);

                    var offset = GetRowSpan(child) - 1;
                    while (offset > 0)
                    {
                        skip[row + offset--, col] = true;
                    }
                }
                else
                {
                    var row = Clamp(position % rowCount, rowCount - 1);
                    var col = Clamp(position / rowCount, colCount - 1);
                    if (skip[row, col])
                    {
                        position++;
                        row = position % rowCount;
                        col = position / rowCount;
                    }

                    SetRow(child, row);
                    SetColumn(child, col);
                    position += GetRowSpan(child);

                    var offset = GetColumnSpan(child) - 1;
                    while (offset > 0)
                    {
                        skip[row, col + offset--] = true;
                    }
                }
            }

            ApplyChildLayout(child);
        }
    }
}