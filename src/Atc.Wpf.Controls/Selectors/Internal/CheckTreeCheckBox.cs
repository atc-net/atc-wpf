#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The check box of a <see cref="CheckTreeView"/> row. It only shows the node's state: the row handles clicks and keys,
/// and UI Automation exposes the toggle on the tree item, so the check box has no automation peer of its own.
/// </summary>
internal sealed class CheckTreeCheckBox : CheckBox
{
    public CheckTreeCheckBox()
    {
        SetResourceReference(StyleProperty, typeof(CheckBox));
        IsThreeState = true;
        Focusable = false;
        IsHitTestVisible = false;
        IsTabStop = false;
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => null!;

    /// <inheritdoc />
    protected override void OnToggle()
    {
        // The state is kept by the owning tree.
    }
}