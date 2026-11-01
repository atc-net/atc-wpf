namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Exposes a <see cref="Segmented"/> to UI Automation as a list. In the single-select modes it also has the
/// selection pattern, so its segments (radio buttons) report it as their selection container.
/// </summary>
public sealed class SegmentedAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentedAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The segmented control.</param>
    public SegmentedAutomationPeer(Segmented owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    public bool CanSelectMultiple => false;

    /// <inheritdoc />
    public bool IsSelectionRequired
        => OwnerSegmented.SelectionMode == SegmentedSelectionMode.Single;

    private Segmented OwnerSegmented => (Segmented)Owner;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Selection && OwnerSegmented.SelectionMode != SegmentedSelectionMode.Multiple
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public IRawElementProviderSimple[] GetSelection()
        => OwnerSegmented.Items
            .Where(x => x.IsSelected)
            .Select(CreatePeerForElement)
            .Where(x => x is not null)
            .Select(ProviderFromPeer)
            .ToArray();

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(Segmented);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.List;
}