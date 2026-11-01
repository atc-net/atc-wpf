namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Exposes a <see cref="DockPanelPro"/> to UI Automation as a pane, with the <see cref="DockPanelPro.LayoutId"/>
/// as automation id when no other id is set.
/// </summary>
public sealed class DockPanelProAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockPanelProAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The dock panel.</param>
    public DockPanelProAutomationPeer(DockPanelPro owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(DockPanelPro);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Pane;

    /// <inheritdoc />
    protected override string GetAutomationIdCore()
    {
        var id = base.GetAutomationIdCore();
        return string.IsNullOrEmpty(id)
            ? ((DockPanelPro)Owner).LayoutId ?? string.Empty
            : id;
    }
}