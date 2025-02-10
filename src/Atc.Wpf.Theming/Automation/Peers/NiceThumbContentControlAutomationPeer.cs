namespace Atc.Wpf.Theming.Automation.Peers;

public sealed class NiceThumbContentControlAutomationPeer : FrameworkElementAutomationPeer
{
    public NiceThumbContentControlAutomationPeer(FrameworkElement owner)
        : base(owner)
    {
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Custom;
    }

    protected override string GetClassNameCore()
    {
        return "NiceThumbContentControl";
    }
}