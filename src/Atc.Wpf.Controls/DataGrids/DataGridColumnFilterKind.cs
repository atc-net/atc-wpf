namespace Atc.Wpf.Controls.DataGrids;

/// <summary>
/// How a <see cref="DataGridColumnFilter"/> decides which rows pass.
/// </summary>
public enum DataGridColumnFilterKind
{
    /// <summary>
    /// The row passes when its formatted cell text is one of <see cref="DataGridColumnFilter.Values"/>.
    /// </summary>
    Values,

    /// <summary>
    /// The row passes when its formatted cell text matches <see cref="DataGridColumnFilter.Text"/>
    /// with <see cref="DataGridColumnFilter.TextOperator"/> (case-insensitive).
    /// </summary>
    Text,

    /// <summary>
    /// The row passes when its numeric cell value lies between <see cref="DataGridColumnFilter.Minimum"/>
    /// and <see cref="DataGridColumnFilter.Maximum"/> (both inclusive, either may be left out).
    /// </summary>
    NumberRange,

    /// <summary>
    /// The row passes when the date of its cell value lies between <see cref="DataGridColumnFilter.From"/>
    /// and <see cref="DataGridColumnFilter.To"/> (whole days, both inclusive, either may be left out).
    /// </summary>
    DateRange,
}