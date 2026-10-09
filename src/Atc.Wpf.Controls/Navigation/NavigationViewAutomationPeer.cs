namespace Atc.Wpf.Controls.Navigation;

/// <summary>
/// Exposes a <see cref="NavigationView"/> to UI Automation as a pane.
/// </summary>
public class NavigationViewAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>Initializes a new instance of the <see cref="NavigationViewAutomationPeer"/> class.</summary>
    public NavigationViewAutomationPeer(NavigationView owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(NavigationView);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Pane;
}