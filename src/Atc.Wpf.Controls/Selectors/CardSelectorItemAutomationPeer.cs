namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Exposes one card of a <see cref="CardSelector"/> to UI Automation. In <see cref="CardSelectionMode.Multiple"/> the
/// card is a check box with the toggle pattern; in the single-select modes it is a radio button with the
/// selection-item pattern. The name is the <see cref="AutomationProperties.NameProperty"/> set in the card template,
/// else the item's key.
/// </summary>
public sealed class CardSelectorItemAutomationPeer : ListBoxItemAutomationPeer, IToggleProvider, ISelectionItemProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CardSelectorItemAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The data item of the card.</param>
    /// <param name="selectorAutomationPeer">The peer of the card selector.</param>
    public CardSelectorItemAutomationPeer(
        object owner,
        CardSelectorAutomationPeer selectorAutomationPeer)
        : base(owner, selectorAutomationPeer)
    {
    }

    /// <inheritdoc />
    public ToggleState ToggleState
        => IsSelected
            ? ToggleState.On
            : ToggleState.Off;

    /// <inheritdoc />
    public bool IsSelected
        => Selector?.SelectedItems.Contains(Item) == true;

    /// <inheritdoc />
    public IRawElementProviderSimple? SelectionContainer
        => ProviderFromPeer(ItemsControlAutomationPeer);

    private CardSelector? Selector
        => ItemsControlAutomationPeer?.Owner as CardSelector;

    private bool IsCheckBox
        => Selector?.SelectionMode == CardSelectionMode.Multiple;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
    {
        if (patternInterface == PatternInterface.Toggle)
        {
            return IsCheckBox ? this : null;
        }

        if (patternInterface == PatternInterface.SelectionItem)
        {
            return IsCheckBox ? null : this;
        }

        return base.GetPattern(patternInterface);
    }

    /// <inheritdoc />
    public void Toggle()
    {
        EnsureEnabled();
        Selector?.RequestToggle(Item);
    }

    /// <inheritdoc />
    public void Select()
    {
        EnsureEnabled();
        if (!IsSelected)
        {
            Selector?.RequestToggle(Item);
        }
    }

    /// <inheritdoc />
    public void AddToSelection()
    {
        if (IsSelected)
        {
            return;
        }

        throw new InvalidOperationException("Only one card of a single-select card selector can be selected; use Select.");
    }

    /// <inheritdoc />
    public void RemoveFromSelection()
    {
        EnsureEnabled();
        if (IsSelected && Selector?.SelectionMode == CardSelectionMode.SingleOrNone)
        {
            Selector.RequestToggle(Item);
        }
    }

    /// <summary>
    /// Raises the toggle-state change of a card that was selected or unselected.
    /// </summary>
    internal void RaiseToggleStateChanged(bool isOn)
        => RaisePropertyChangedEvent(
            TogglePatternIdentifiers.ToggleStateProperty,
            isOn ? ToggleState.Off : ToggleState.On,
            isOn ? ToggleState.On : ToggleState.Off);

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(CardSelectorItem);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => IsCheckBox
            ? AutomationControlType.CheckBox
            : AutomationControlType.RadioButton;

    /// <inheritdoc />
    protected override string GetNameCore()
    {
        var container = Selector?.ItemContainerGenerator.ContainerFromItem(Item) as CardSelectorItem;
        if (container is not null)
        {
            var explicitName = AutomationProperties.GetName(container);
            if (!string.IsNullOrEmpty(explicitName))
            {
                return explicitName;
            }

            var templateName = container.GetTemplateAutomationName();
            if (!string.IsNullOrEmpty(templateName))
            {
                return templateName;
            }
        }

        var key = Selector?.GetKey(Item);
        return string.IsNullOrEmpty(key)
            ? base.GetNameCore()
            : key;
    }

    private void EnsureEnabled()
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }
    }
}