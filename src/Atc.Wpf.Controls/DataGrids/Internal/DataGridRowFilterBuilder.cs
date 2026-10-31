namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// Translates <see cref="DataGridFilter"/> filters into a <see cref="System.Data.DataView.RowFilter"/> expression, for grids on a
/// <see cref="System.Data.DataTable"/> (a <see cref="BindingListCollectionView"/>, which cannot filter with a predicate).
/// </summary>
/// <remarks>
/// A filter of one column depends on that column's value only, so it is translated exactly by testing each distinct
/// value of the column in the table and listing the ones that pass (or, when shorter, the ones that do not).
/// Values that appear in the table later are matched by that list until the filters are applied again.
/// </remarks>
internal static class DataGridRowFilterBuilder
{
    /// <summary>
    /// The expression that no row passes.
    /// </summary>
    public const string None = "1 = 0";

    /// <summary>
    /// Returns the data view a collection view filters through <see cref="BindingListCollectionView.CustomFilter"/>,
    /// or <see langword="null"/> when the view is not a custom-filterable view of a <see cref="System.Data.DataView"/>.
    /// </summary>
    public static System.Data.DataView? GetDataView(ICollectionView? view)
        => view is BindingListCollectionView { CanCustomFilter: true, SourceCollection: System.Data.DataView { Table: not null } dataView }
            ? dataView
            : null;

    /// <summary>
    /// Returns the column of the table that an accessor reads, when its values can be written in an expression.
    /// </summary>
    public static System.Data.DataColumn? GetColumn(
        System.Data.DataTable table,
        DataGridColumnValueAccessor accessor)
        => accessor.SinglePropertyName is { } name &&
           table.Columns.Contains(name) &&
           table.Columns[name] is { } column &&
           IsSupportedType(column.DataType)
            ? column
            : null;

    /// <summary>
    /// Builds the expression of the rows whose value in <paramref name="column"/> passes <paramref name="passes"/>,
    /// or returns <see langword="null"/> when every row passes.
    /// </summary>
    public static string? BuildColumnExpression(
        System.Data.DataColumn column,
        Func<object?, bool> passes)
    {
        ArgumentNullException.ThrowIfNull(column);
        ArgumentNullException.ThrowIfNull(passes);

        var passing = new List<object>();
        var failing = new List<object>();
        var seen = new HashSet<object>();
        var nullPasses = passes(null);
        foreach (System.Data.DataRow row in column.Table!.Rows)
        {
            if (row.RowState is System.Data.DataRowState.Deleted or System.Data.DataRowState.Detached)
            {
                continue;
            }

            var value = row[column];
            if (value == DBNull.Value || !seen.Add(value))
            {
                continue;
            }

            (passes(value) ? passing : failing).Add(value);
        }

        MoveTextTheTableTreatsAsEqual(column, passing, failing);

        var name = QuoteName(column.ColumnName);
        if (failing.Count == 0)
        {
            return nullPasses ? null : $"{name} IS NOT NULL";
        }

        if (passing.Count == 0)
        {
            return nullPasses ? $"{name} IS NULL" : None;
        }

        if (passing.Count <= failing.Count)
        {
            var list = $"{name} IN ({FormatList(passing)})";
            return nullPasses ? $"({name} IS NULL OR {list})" : list;
        }

        var notList = $"{name} NOT IN ({FormatList(failing)})";
        return nullPasses
            ? $"({name} IS NULL OR {notList})"
            : $"({name} IS NOT NULL AND {notList})";
    }

    /// <summary>
    /// Combines expressions with AND, leaving out the empty ones; <see langword="null"/> when none is left.
    /// </summary>
    public static string? And(IEnumerable<string?> expressions)
    {
        var parts = expressions
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
        return parts.Count switch
        {
            0 => null,
            1 => parts[0],
            _ => string.Join(" AND ", parts.Select(x => $"({x})")),
        };
    }

    /// <summary>
    /// A table that is not <see cref="System.Data.DataTable.CaseSensitive"/> compares text ignoring case, so a list cannot keep
    /// "Alice" and drop "alice": such a failing text passes with the passing one.
    /// </summary>
    private static void MoveTextTheTableTreatsAsEqual(
        System.Data.DataColumn column,
        List<object> passing,
        List<object> failing)
    {
        var table = column.Table!;
        if (column.DataType != typeof(string) || table.CaseSensitive || passing.Count == 0 || failing.Count == 0)
        {
            return;
        }

        var comparer = table.Locale.CompareInfo.GetStringComparer(
            CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth);
        var passingTexts = new HashSet<string>(passing.Cast<string>(), comparer);
        var equal = failing.FindAll(x => passingTexts.Contains((string)x));
        failing.RemoveAll(equal.Contains);
        passing.AddRange(equal);
    }

    private static bool IsSupportedType(Type type)
        => type == typeof(string) ||
           type == typeof(char) ||
           type == typeof(bool) ||
           type == typeof(DateTime) ||
           type == typeof(Guid) ||
           type == typeof(decimal) ||
           type == typeof(double) ||
           type == typeof(float) ||
           type == typeof(byte) ||
           type == typeof(sbyte) ||
           type == typeof(short) ||
           type == typeof(ushort) ||
           type == typeof(int) ||
           type == typeof(uint) ||
           type == typeof(long) ||
           type == typeof(ulong);

    private static string FormatList(List<object> values)
        => string.Join(", ", values.Select(FormatLiteral));

    private static string FormatLiteral(object value)
        => value switch
        {
            string text => Quote(text),
            char c => Quote(c.ToString()),
            bool b => b ? "true" : "false",
            DateTime date => "#" + date.ToString("yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture) + "#",
            Guid guid => $"CONVERT({Quote(guid.ToString("D"))}, 'System.Guid')",
            double d => FormatFloating(d),
            float f => FormatFloating(f),
            IFormattable formattable => formattable.ToString(format: null, CultureInfo.InvariantCulture),
            _ => Quote(value.ToString() ?? string.Empty),
        };

    private static string FormatFloating(double value)
        => $"CONVERT({Quote(value.ToString("R", CultureInfo.InvariantCulture))}, 'System.Double')";

    private static string Quote(string text)
        => "'" + text.Replace("'", "''", StringComparison.Ordinal) + "'";

    private static string QuoteName(string name)
        => "[" + name
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("]", "\\]", StringComparison.Ordinal) + "]";
}