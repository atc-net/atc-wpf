namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// The kind of the raw values of a column, which decides whether the pop-up offers a range filter.
/// </summary>
internal enum DataGridFilterValueKind
{
    /// <summary>Text or mixed values: no range filter.</summary>
    Text,

    /// <summary>All values are numbers: a number range filter.</summary>
    Number,

    /// <summary>All values are dates: a date range filter.</summary>
    Date,
}