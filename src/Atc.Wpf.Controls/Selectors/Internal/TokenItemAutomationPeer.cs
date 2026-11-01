#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Exposes one token of a <see cref="TokenBox"/> as a list item named by its display value. The default chip's child is
/// its "Remove {name}" button (when the token can be removed); a custom token template's elements are its children.
/// </summary>
internal sealed class TokenItemAutomationPeer : FrameworkElementAutomationPeer
{
    public TokenItemAutomationPeer(FrameworkElement owner)
        : base(owner)
    {
    }

    protected override string GetClassNameCore()
        => "Token";

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.ListItem;

    protected override bool IsControlElementCore()
        => true;

    protected override bool IsContentElementCore()
        => true;

    protected override string GetNameCore()
        => (Owner as FrameworkElement)?.DataContext is MultiSelectChipItem item
            ? item.DisplayValue
            : (Owner as ContentPresenter)?.Content?.ToString() ?? base.GetNameCore();

    protected override List<AutomationPeer>? GetChildrenCore()
    {
        if (Owner is DataDisplay.Chip chip)
        {
            if (chip.CanRemove &&
                chip.Template?.FindName("PART_RemoveButton", chip) is Button { Visibility: Visibility.Visible } removeButton &&
                CreatePeerForElement(removeButton) is { } removePeer)
            {
                return [removePeer];
            }

            return null;
        }

        return base.GetChildrenCore();
    }
}