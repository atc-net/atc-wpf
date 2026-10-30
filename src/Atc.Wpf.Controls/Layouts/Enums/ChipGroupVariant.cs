namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies how the chips of a <see cref="DataDisplay.ChipGroup"/> show that they are selected.
/// </summary>
public enum ChipGroupVariant
{
    /// <summary>
    /// A selected chip is filled with the accent colour.
    /// </summary>
    Filled,

    /// <summary>
    /// Chips are outlined; a selected chip gets an accent border and a light accent tint.
    /// </summary>
    Outlined,
}