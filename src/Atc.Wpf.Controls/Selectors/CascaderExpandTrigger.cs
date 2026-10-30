namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Specifies what makes a row of a <see cref="CascaderComboBox"/> column active, so its children are shown in the
/// next column.
/// </summary>
public enum CascaderExpandTrigger
{
    /// <summary>A click on the row text (or <c>→</c> / <c>Enter</c>) makes the row active.</summary>
    Click,

    /// <summary>Resting the mouse on the row for a moment also makes it active; a click still does.</summary>
    Hover,
}