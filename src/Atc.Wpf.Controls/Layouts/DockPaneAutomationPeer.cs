namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Exposes a <see cref="DockPane"/> to UI Automation as a pane named after its header,
/// with the <see cref="DockPane.PaneId"/> as automation id.
/// </summary>
public sealed class DockPaneAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockPaneAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The pane.</param>
    public DockPaneAutomationPeer(DockPane owner)
        : base(owner)
    {
    }

    private DockPane OwnerPane => (DockPane)Owner;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(DockPane);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Pane;

    /// <inheritdoc />
    protected override string GetNameCore()
    {
        var name = base.GetNameCore();
        return string.IsNullOrEmpty(name)
            ? OwnerPane.HeaderText
            : name;
    }

    /// <inheritdoc />
    protected override string GetAutomationIdCore()
    {
        var id = base.GetAutomationIdCore();
        return string.IsNullOrEmpty(id)
            ? OwnerPane.PaneId ?? string.Empty
            : id;
    }
}