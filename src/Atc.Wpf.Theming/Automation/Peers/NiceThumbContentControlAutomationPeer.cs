namespace Atc.Wpf.Theming.Automation.Peers;

public sealed class NiceThumbContentControlAutomationPeer : FrameworkElementAutomationPeer
{
    public NiceThumbContentControlAutomationPeer(FrameworkElement owner)
        : base(owner)
    {
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    protected override string GetClassNameCore()
        => "NiceThumbContentControl";
}