#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The per-row state of a <see cref="MultiColumnComboBox"/> pop-up grid, set on each realized
/// <see cref="DataGridRow"/> (the rows show the caller's own objects, so the state cannot live on the items).
/// </summary>
internal static partial class MultiColumnRowState
{
    // Whether the row's item is ticked.
    [AttachedProperty(DefaultValue = false, NullElementHandling = NullElementHandling.None)]
    private static bool isChecked;

    // Whether the row's item can be ticked or unticked (not disabled and not blocked by the limit).
    [AttachedProperty(DefaultValue = true, NullElementHandling = NullElementHandling.None)]
    private static bool canToggle;

    // The display value of the row's item (the automation name of its check box).
    [AttachedProperty(DefaultValue = "", NullElementHandling = NullElementHandling.None)]
    private static string displayText;
}