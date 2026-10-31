namespace Atc.Wpf.Controls.DataGrids;

/// <summary>
/// The filter of one <see cref="DataGrid"/> column, identified by its column key
/// (see <see cref="DataGridFilter.ResolveColumnKey"/>). Filters of several columns combine with AND.
/// </summary>
/// <remarks>
/// A <see cref="DataGridColumnFilterKind.Values"/> filter lists the formatted cell texts that pass (the texts shown
/// in the grid, so they depend on the column's <c>StringFormat</c> and the grid's language); an empty text stands
/// for blank cells (<see langword="null"/> or empty).
/// </remarks>
public sealed class DataGridColumnFilter
{
    /// <summary>
    /// Gets the key of the column the filter belongs to.
    /// </summary>
    public string ColumnKey { get; init; } = string.Empty;

    /// <summary>
    /// Gets how the filter decides which rows pass.
    /// </summary>
    public DataGridColumnFilterKind Kind { get; init; }

    /// <summary>
    /// Gets the formatted cell texts that pass a <see cref="DataGridColumnFilterKind.Values"/> filter;
    /// an empty string stands for blank cells.
    /// </summary>
    public IReadOnlyList<string> Values { get; init; } = [];

    /// <summary>
    /// Gets how <see cref="Text"/> is compared in a <see cref="DataGridColumnFilterKind.Text"/> filter.
    /// </summary>
    public DataGridTextFilterOperator TextOperator { get; init; }

    /// <summary>
    /// Gets the text of a <see cref="DataGridColumnFilterKind.Text"/> filter.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    /// Gets the inclusive lower bound of a <see cref="DataGridColumnFilterKind.NumberRange"/> filter, or
    /// <see langword="null"/> for none.
    /// </summary>
    public double? Minimum { get; init; }

    /// <summary>
    /// Gets the inclusive upper bound of a <see cref="DataGridColumnFilterKind.NumberRange"/> filter, or
    /// <see langword="null"/> for none.
    /// </summary>
    public double? Maximum { get; init; }

    /// <summary>
    /// Gets the first day of a <see cref="DataGridColumnFilterKind.DateRange"/> filter, or <see langword="null"/>
    /// for none.
    /// </summary>
    public DateTime? From { get; init; }

    /// <summary>
    /// Gets the last day (inclusive) of a <see cref="DataGridColumnFilterKind.DateRange"/> filter, or
    /// <see langword="null"/> for none.
    /// </summary>
    public DateTime? To { get; init; }

    /// <summary>
    /// Creates a filter that lets the rows with one of the given formatted cell texts pass.
    /// </summary>
    /// <param name="columnKey">The column key.</param>
    /// <param name="values">The formatted cell texts that pass; an empty string stands for blank cells.</param>
    /// <returns>The filter.</returns>
    public static DataGridColumnFilter ForValues(
        string columnKey,
        IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new DataGridColumnFilter
        {
            ColumnKey = columnKey,
            Kind = DataGridColumnFilterKind.Values,
            Values = values.ToList(),
        };
    }

    /// <summary>
    /// Creates a filter that compares the formatted cell text with a text (case-insensitive).
    /// </summary>
    /// <param name="columnKey">The column key.</param>
    /// <param name="textOperator">How the text is compared.</param>
    /// <param name="text">The text.</param>
    /// <returns>The filter.</returns>
    public static DataGridColumnFilter ForText(
        string columnKey,
        DataGridTextFilterOperator textOperator,
        string text)
        => new()
        {
            ColumnKey = columnKey,
            Kind = DataGridColumnFilterKind.Text,
            TextOperator = textOperator,
            Text = text,
        };

    /// <summary>
    /// Creates a filter that lets the rows with a numeric cell value in a range pass.
    /// </summary>
    /// <param name="columnKey">The column key.</param>
    /// <param name="minimum">The inclusive lower bound, or <see langword="null"/>.</param>
    /// <param name="maximum">The inclusive upper bound, or <see langword="null"/>.</param>
    /// <returns>The filter.</returns>
    public static DataGridColumnFilter ForNumberRange(
        string columnKey,
        double? minimum,
        double? maximum)
        => new()
        {
            ColumnKey = columnKey,
            Kind = DataGridColumnFilterKind.NumberRange,
            Minimum = minimum,
            Maximum = maximum,
        };

    /// <summary>
    /// Creates a filter that lets the rows with a date cell value in a range of days pass.
    /// </summary>
    /// <param name="columnKey">The column key.</param>
    /// <param name="from">The first day, or <see langword="null"/>.</param>
    /// <param name="to">The last day (inclusive), or <see langword="null"/>.</param>
    /// <returns>The filter.</returns>
    public static DataGridColumnFilter ForDateRange(
        string columnKey,
        DateTime? from,
        DateTime? to)
        => new()
        {
            ColumnKey = columnKey,
            Kind = DataGridColumnFilterKind.DateRange,
            From = from,
            To = to,
        };
}