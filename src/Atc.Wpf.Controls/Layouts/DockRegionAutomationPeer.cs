namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Exposes a <see cref="DockRegion"/> to UI Automation as a group named after its header (or its active pane),
/// with the <see cref="DockRegion.RegionId"/> as automation id. A collapsible region also has the expand/collapse pattern.
/// </summary>
public sealed class DockRegionAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockRegionAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The region.</param>
    public DockRegionAutomationPeer(DockRegion owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    public ExpandCollapseState ExpandCollapseState
        => OwnerRegion.IsExpanded
            ? ExpandCollapseState.Expanded
            : ExpandCollapseState.Collapsed;

    private DockRegion OwnerRegion => (DockRegion)Owner;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.ExpandCollapse && OwnerRegion.IsCollapsible
            ? this
            : base.GetPattern(patternInterface);

    /// <inheritdoc />
    public void Expand()
        => OwnerRegion.SetCurrentValue(DockRegion.IsExpandedProperty, true);

    /// <inheritdoc />
    public void Collapse()
        => OwnerRegion.SetCurrentValue(DockRegion.IsExpandedProperty, false);

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(DockRegion);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Group;

    /// <inheritdoc />
    protected override string GetNameCore()
    {
        var name = base.GetNameCore();
        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        return OwnerRegion.Header switch
        {
            string text => text,
            null => OwnerRegion.ActivePane?.HeaderText ?? string.Empty,
            _ => OwnerRegion.Header.ToString() ?? string.Empty,
        };
    }

    /// <inheritdoc />
    protected override string GetAutomationIdCore()
    {
        var id = base.GetAutomationIdCore();
        return string.IsNullOrEmpty(id)
            ? OwnerRegion.RegionId ?? string.Empty
            : id;
    }
}