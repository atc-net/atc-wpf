#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Exposes the tokens of a <see cref="TokenBox"/> as a list: one list item per shown token (named by its display value,
/// with the token's "Remove {name}" button as its child) and the "+N" button when tokens are hidden.
/// </summary>
internal sealed class TokenListAutomationPeer : FrameworkElementAutomationPeer
{
    private readonly TokenBox tokenBox;
    private readonly Dictionary<FrameworkElement, TokenItemAutomationPeer> itemPeers = [];

    public TokenListAutomationPeer(
        MultiSelectChipPanel owner,
        TokenBox tokenBox)
        : base(owner)
    {
        this.tokenBox = tokenBox;
    }

    private MultiSelectChipPanel Panel => (MultiSelectChipPanel)Owner;

    protected override string GetClassNameCore()
        => "TokenList";

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.List;

    protected override bool IsControlElementCore()
        => true;

    protected override bool IsContentElementCore()
        => true;

    protected override List<AutomationPeer>? GetChildrenCore()
    {
        var elements = tokenBox.GetShownTokenElements();
        var stale = itemPeers.Keys.Where(x => !elements.Contains(x)).ToList();
        foreach (var element in stale)
        {
            itemPeers.Remove(element);
        }

        var children = new List<AutomationPeer>();
        foreach (var element in elements)
        {
            if (element is TextBox editBox)
            {
                if (CreatePeerForElement(editBox) is { } editPeer)
                {
                    children.Add(editPeer);
                }

                continue;
            }

            if (!itemPeers.TryGetValue(element, out var peer))
            {
                peer = new TokenItemAutomationPeer(element);
                itemPeers[element] = peer;
            }

            children.Add(peer);
        }

        if (Panel.HiddenCount > 0 &&
            CreatePeerForElement(tokenBox.OverflowButton) is { } overflowPeer)
        {
            children.Add(overflowPeer);
        }

        return children.Count == 0 ? null : children;
    }
}