namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Specifies what the closed <see cref="MultiSelectComboBox"/> shows for the ticked items.
/// </summary>
public enum MultiSelectDisplayMode
{
    /// <summary>The names ("Orders, Payments"), or the count beyond <see cref="MultiSelectComboBox.MaxNamesInSummary"/>.</summary>
    Text,

    /// <summary>A removable chip per ticked item, with a "+N" chip for the ones that do not fit.</summary>
    Chips,

    /// <summary>Only the count ("3 of 5 selected"), or the all / none text at the ends.</summary>
    Count,
}