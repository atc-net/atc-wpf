namespace Atc.Wpf.Controls.DataGrids;

/// <summary>
/// How the text of a <see cref="DataGridColumnFilterKind.Text"/> filter is compared with the formatted cell text.
/// All comparisons ignore case.
/// </summary>
public enum DataGridTextFilterOperator
{
    /// <summary>
    /// The cell text contains the filter text.
    /// </summary>
    Contains,

    /// <summary>
    /// The cell text equals the filter text.
    /// </summary>
    IsEqualTo,

    /// <summary>
    /// The cell text starts with the filter text.
    /// </summary>
    StartsWith,
}