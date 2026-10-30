#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The click area around the check box of a <see cref="CascaderColumnView"/> row: a click inside it ticks the row,
/// a click on the rest of a row with children makes the row active.
/// </summary>
internal sealed class CascaderCheckArea : Border
{
    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => null!;
}