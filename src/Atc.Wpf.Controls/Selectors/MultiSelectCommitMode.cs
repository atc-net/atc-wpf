namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Specifies when ticks made in the pop-up of a <see cref="MultiSelectComboBox"/> reach its selection.
/// </summary>
public enum MultiSelectCommitMode
{
    /// <summary>Each tick updates the selection right away.</summary>
    Immediate,

    /// <summary>Ticks are a draft; the pop-up shows OK and Cancel and only OK updates the selection.</summary>
    Deferred,
}