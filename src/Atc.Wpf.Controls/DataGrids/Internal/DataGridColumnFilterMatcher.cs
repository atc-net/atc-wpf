namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// Tests row items against one <see cref="DataGridColumnFilter"/> through the column's value accessor.
/// </summary>
internal sealed class DataGridColumnFilterMatcher
{
    private readonly DataGridColumnFilter filter;
    private readonly HashSet<string>? values;

    public DataGridColumnFilterMatcher(
        DataGridColumn column,
        DataGridColumnFilter filter,
        DataGridColumnValueAccessor accessor)
    {
        Column = column;
        this.filter = filter;
        Accessor = accessor;
        if (filter.Kind == DataGridColumnFilterKind.Values)
        {
            values = new HashSet<string>(filter.Values, StringComparer.Ordinal);
        }
    }

    /// <summary>
    /// Gets the column the filter belongs to.
    /// </summary>
    public DataGridColumn Column { get; }

    /// <summary>
    /// Gets the accessor of the column.
    /// </summary>
    public DataGridColumnValueAccessor Accessor { get; }

    /// <summary>
    /// Returns whether a filter restricts anything (an empty text or an open range does not).
    /// </summary>
    public static bool IsActive(DataGridColumnFilter? filter)
        => filter?.Kind switch
        {
            DataGridColumnFilterKind.Values => true,
            DataGridColumnFilterKind.Text => !string.IsNullOrEmpty(filter.Text),
            DataGridColumnFilterKind.NumberRange => filter.Minimum.HasValue || filter.Maximum.HasValue,
            DataGridColumnFilterKind.DateRange => filter.From.HasValue || filter.To.HasValue,
            _ => false,
        };

    /// <summary>
    /// Returns whether the item passes the filter.
    /// </summary>
    public bool Matches(object? item)
        => filter.Kind switch
        {
            DataGridColumnFilterKind.Values => values!.Contains(Accessor.GetText(item)),
            DataGridColumnFilterKind.Text => MatchesText(Accessor.GetText(item)),
            DataGridColumnFilterKind.NumberRange => MatchesNumber(Accessor.GetValue(item)),
            DataGridColumnFilterKind.DateRange => MatchesDate(Accessor.GetValue(item)),
            _ => true,
        };

    /// <summary>
    /// Converts a raw cell value to a number, or <see langword="null"/> when it is not numeric.
    /// </summary>
    public static double? ToNumber(object? value)
        => value switch
        {
            null => null,
            double d => d,
            float f => f,
            decimal m => (double)m,
            int i => i,
            long l => l,
            short s => s,
            byte b => b,
            uint ui => ui,
            ulong ul => ul,
            ushort us => us,
            sbyte sb => sb,
            _ => null,
        };

    /// <summary>
    /// Converts a raw cell value to a date, or <see langword="null"/> when it is not a date.
    /// </summary>
    public static DateTime? ToDate(object? value)
        => value switch
        {
            DateTime dt => dt,
            DateTimeOffset dto => dto.DateTime,
            DateOnly d => d.ToDateTime(TimeOnly.MinValue),
            _ => null,
        };

    private bool MatchesText(string text)
    {
        var search = filter.Text ?? string.Empty;
        return filter.TextOperator switch
        {
            DataGridTextFilterOperator.IsEqualTo => string.Equals(text, search, StringComparison.OrdinalIgnoreCase),
            DataGridTextFilterOperator.StartsWith => text.StartsWith(search, StringComparison.OrdinalIgnoreCase),
            _ => text.Contains(search, StringComparison.OrdinalIgnoreCase),
        };
    }

    private bool MatchesNumber(object? value)
    {
        if (ToNumber(value) is not { } number)
        {
            return false;
        }

        return (!filter.Minimum.HasValue || number >= filter.Minimum.Value) &&
               (!filter.Maximum.HasValue || number <= filter.Maximum.Value);
    }

    private bool MatchesDate(object? value)
    {
        if (ToDate(value) is not { } date)
        {
            return false;
        }

        var day = date.Date;
        return (!filter.From.HasValue || day >= filter.From.Value.Date) &&
               (!filter.To.HasValue || day <= filter.To.Value.Date);
    }
}