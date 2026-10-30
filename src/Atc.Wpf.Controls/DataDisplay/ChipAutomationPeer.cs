namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Exposes a <see cref="Chip"/> to UI Automation. A selectable chip is a check box with the toggle pattern; an option
/// of a single-select <see cref="ChipGroup"/> is a radio button with the selection-item pattern; any other chip keeps
/// the plain element behaviour. The name falls back to the chip's text content.
/// </summary>
public sealed class ChipAutomationPeer : FrameworkElementAutomationPeer, IToggleProvider, ISelectionItemProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChipAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The chip.</param>
    public ChipAutomationPeer(Chip owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    public ToggleState ToggleState
        => OwnerChip.IsSelected
            ? ToggleState.On
            : ToggleState.Off;

    /// <inheritdoc />
    public bool IsSelected => OwnerChip.IsSelected;

    /// <inheritdoc />
    public IRawElementProviderSimple? SelectionContainer
    {
        get
        {
            var group = FindGroup(OwnerChip);
            return group is null
                ? null
                : ProviderFromPeer(CreatePeerForElement(group));
        }
    }

    private Chip OwnerChip => (Chip)Owner;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
    {
        if (OwnerChip.IsRadioItem)
        {
            return patternInterface == PatternInterface.SelectionItem
                ? this
                : base.GetPattern(patternInterface);
        }

        return patternInterface == PatternInterface.Toggle && OwnerChip.IsToggleable
            ? this
            : base.GetPattern(patternInterface);
    }

    /// <inheritdoc />
    public void Toggle()
    {
        EnsureEnabled();
        OwnerChip.RequestToggle();
    }

    /// <inheritdoc />
    public void Select()
    {
        EnsureEnabled();
        if (!OwnerChip.IsSelected)
        {
            OwnerChip.RequestToggle();
        }
    }

    /// <inheritdoc />
    public void AddToSelection()
    {
        if (OwnerChip.IsSelected)
        {
            return;
        }

        throw new InvalidOperationException("Only one option of a single-select chip group can be selected; use Select.");
    }

    /// <inheritdoc />
    public void RemoveFromSelection()
    {
        EnsureEnabled();
        if (OwnerChip.IsSelected)
        {
            OwnerChip.RequestToggle();
        }
    }

    /// <summary>
    /// Raises the toggle-state (or selection-item) change for a new <see cref="Chip.IsSelected"/> value.
    /// </summary>
    internal void RaiseSelectionChanged(
        bool oldValue,
        bool newValue)
    {
        if (OwnerChip.IsRadioItem)
        {
            RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, oldValue, newValue);
            if (newValue)
            {
                RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
            }

            return;
        }

        if (OwnerChip.IsToggleable)
        {
            RaisePropertyChangedEvent(
                TogglePatternIdentifiers.ToggleStateProperty,
                oldValue ? ToggleState.On : ToggleState.Off,
                newValue ? ToggleState.On : ToggleState.Off);
        }
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(Chip);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        if (OwnerChip.IsRadioItem)
        {
            return AutomationControlType.RadioButton;
        }

        return OwnerChip.IsToggleable
            ? AutomationControlType.CheckBox
            : base.GetAutomationControlTypeCore();
    }

    /// <inheritdoc />
    protected override string GetNameCore()
    {
        var name = base.GetNameCore();
        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        return OwnerChip.Content as string ?? string.Empty;
    }

    private static ChipGroup? FindGroup(DependencyObject? element)
    {
        while (element is not null)
        {
            if (element is ChipGroup group)
            {
                return group;
            }

            element = element is Visual
                ? VisualTreeHelper.GetParent(element) ?? LogicalTreeHelper.GetParent(element)
                : LogicalTreeHelper.GetParent(element);
        }

        return null;
    }

    private void EnsureEnabled()
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }
    }
}