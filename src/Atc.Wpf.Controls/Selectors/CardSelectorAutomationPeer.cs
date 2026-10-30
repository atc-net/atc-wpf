namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Exposes a <see cref="CardSelector"/> to UI Automation as a list of cards. In
/// <see cref="CardSelectionMode.Multiple"/> the cards are check boxes; in the single-select modes they are radio
/// buttons and the list has the selection pattern.
/// </summary>
public sealed class CardSelectorAutomationPeer : ListBoxAutomationPeer, ISelectionProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CardSelectorAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The card selector.</param>
    public CardSelectorAutomationPeer(CardSelector owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    public bool CanSelectMultiple => false;

    /// <inheritdoc />
    public bool IsSelectionRequired
        => OwnerSelector.SelectionMode == CardSelectionMode.Single;

    private CardSelector OwnerSelector => (CardSelector)Owner;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
    {
        if (patternInterface == PatternInterface.Selection)
        {
            return OwnerSelector.SelectionMode == CardSelectionMode.Multiple
                ? null
                : this;
        }

        return base.GetPattern(patternInterface);
    }

    /// <inheritdoc />
    public IRawElementProviderSimple[] GetSelection()
        => (GetChildren() ?? [])
            .OfType<CardSelectorItemAutomationPeer>()
            .Where(x => OwnerSelector.SelectedItems.Contains(x.Item))
            .Select(ProviderFromPeer)
            .ToArray();

    /// <summary>
    /// Raises the toggle-state changes of the cards that were selected or unselected.
    /// </summary>
    internal void RaiseToggleStateChanged(
        System.Collections.IList addedItems,
        System.Collections.IList removedItems)
    {
        if (!ListenerExists(AutomationEvents.PropertyChanged))
        {
            return;
        }

        foreach (var peer in (GetChildren() ?? []).OfType<CardSelectorItemAutomationPeer>())
        {
            if (addedItems.Contains(peer.Item))
            {
                peer.RaiseToggleStateChanged(isOn: true);
            }
            else if (removedItems.Contains(peer.Item))
            {
                peer.RaiseToggleStateChanged(isOn: false);
            }
        }
    }

    /// <inheritdoc />
    protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        => new CardSelectorItemAutomationPeer(item, this);

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(CardSelector);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.List;
}