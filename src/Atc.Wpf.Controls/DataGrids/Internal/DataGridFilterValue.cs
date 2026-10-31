namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// One distinct value of a column in the filter pop-up.
/// </summary>
/// <param name="Text">The formatted cell text (empty for blank cells); this is what a values filter stores.</param>
/// <param name="DisplayText">The text shown in the list ("(Blanks)" for blank cells).</param>
internal sealed record DataGridFilterValue(
    string Text,
    string DisplayText);