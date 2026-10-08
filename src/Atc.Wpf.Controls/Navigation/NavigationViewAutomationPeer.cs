namespace Atc.Wpf.Controls.Navigation;

/// <summary>
/// Exposes a <see cref="NavigationView"/> to UI Automation as a pane.
/// </summary>
public class NavigationViewAutomationPeer : FrameworkElementAutomationPeer
{
    public NavigationViewAutomationPeer(NavigationView owner)
        : base(owner)
    {
    }

    protected override string GetClassNameCore()
        => nameof(NavigationView);

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Pane;
}