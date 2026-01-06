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
    [DependencyProperty(
        Category = "Layout",
        Description = "The rows property",
        DefaultValue = null,
        PropertyChangedCallback = nameof(OnRowsChanged))]
    private string rows;

    [DependencyProperty(
        Category = "Layout",
        Description = "The columns property",
        DefaultValue = null,
        PropertyChangedCallback = nameof(OnColumnsChanged))]
    private string columns;

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
}