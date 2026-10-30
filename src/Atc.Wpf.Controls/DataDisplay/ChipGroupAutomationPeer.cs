namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Exposes a <see cref="ChipGroup"/> to UI Automation as a list. In the single-select modes it also has the
/// selection pattern, so its chips (radio buttons) report it as their selection container.
/// </summary>
public sealed class ChipGroupAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChipGroupAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The chip group.</param>
    public ChipGroupAutomationPeer(ChipGroup owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    public bool CanSelectMultiple => false;

    /// <inheritdoc />
    public bool IsSelectionRequired
        => OwnerGroup.SelectionMode == ChipSelectionMode.Single;

    private ChipGroup OwnerGroup => (ChipGroup)Owner;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Selection && OwnerGroup.SelectionMode != ChipSelectionMode.Multiple
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public IRawElementProviderSimple[] GetSelection()
        => OwnerGroup.SelectedChips
            .Select(CreatePeerForElement)
            .Where(x => x is not null)
            .Select(ProviderFromPeer)
            .ToArray();

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(ChipGroup);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.List;
}