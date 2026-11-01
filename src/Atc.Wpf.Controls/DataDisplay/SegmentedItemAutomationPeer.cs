namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Exposes a <see cref="SegmentedItem"/> to UI Automation. In a <see cref="SegmentedSelectionMode.Multiple"/>
/// <see cref="Segmented"/> the segment is a toggle button (a button with the toggle pattern); in the single-select
/// modes it is a radio button with the selection-item pattern. The name falls back to the segment's text, also when
/// only its icon is shown.
/// </summary>
public sealed class SegmentedItemAutomationPeer : FrameworkElementAutomationPeer, IToggleProvider, ISelectionItemProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentedItemAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The segment.</param>
    public SegmentedItemAutomationPeer(SegmentedItem owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    public ToggleState ToggleState
        => OwnerItem.IsSelected
            ? ToggleState.On
            : ToggleState.Off;

    /// <inheritdoc />
    public bool IsSelected => OwnerItem.IsSelected;

    /// <inheritdoc />
    public IRawElementProviderSimple? SelectionContainer
        => OwnerItem.Owner is { } segmented
            ? ProviderFromPeer(CreatePeerForElement(segmented))
            : null;

    private SegmentedItem OwnerItem => (SegmentedItem)Owner;

    private bool IsToggleButton
        => OwnerItem.Owner?.SelectionMode == SegmentedSelectionMode.Multiple;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
    {
        if (IsToggleButton)
        {
            return patternInterface == PatternInterface.Toggle
                ? this
                : base.GetPattern(patternInterface);
        }

        return patternInterface == PatternInterface.SelectionItem
            ? this
            : base.GetPattern(patternInterface);
    }

    /// <inheritdoc />
    public void Toggle()
    {
        EnsureEnabled();
        OwnerItem.PerformClick();
    }

    /// <inheritdoc />
    public void Select()
    {
        EnsureEnabled();
        if (!OwnerItem.IsSelected)
        {
            OwnerItem.PerformClick();
        }
    }

    /// <inheritdoc />
    public void AddToSelection()
    {
        if (OwnerItem.IsSelected)
        {
            return;
        }

        throw new InvalidOperationException("Only one segment of a single-select segmented control can be selected; use Select.");
    }

    /// <inheritdoc />
    public void RemoveFromSelection()
    {
        EnsureEnabled();
        if (OwnerItem.IsSelected && OwnerItem.Owner?.SelectionMode == SegmentedSelectionMode.SingleOrNone)
        {
            OwnerItem.PerformClick();
        }
    }

    /// <summary>
    /// Raises the toggle-state (or selection-item) change for a new <see cref="SegmentedItem.IsSelected"/> value.
    /// </summary>
    internal void RaiseSelectionChanged(
        bool oldValue,
        bool newValue)
    {
        if (IsToggleButton)
        {
            RaisePropertyChangedEvent(
                TogglePatternIdentifiers.ToggleStateProperty,
                oldValue ? ToggleState.On : ToggleState.Off,
                newValue ? ToggleState.On : ToggleState.Off);
            return;
        }

        RaisePropertyChangedEvent(SelectionItemPatternIdentifiers.IsSelectedProperty, oldValue, newValue);
        if (newValue)
        {
            RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
        }
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(SegmentedItem);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => IsToggleButton
            ? AutomationControlType.Button
            : AutomationControlType.RadioButton;

    /// <inheritdoc />
    protected override string GetNameCore()
    {
        var name = base.GetNameCore();
        return string.IsNullOrEmpty(name)
            ? OwnerItem.DisplayText ?? string.Empty
            : name;
    }

    private void EnsureEnabled()
    {
        if (!IsEnabled())
        {
            throw new ElementNotEnabledException();
        }
    }
}