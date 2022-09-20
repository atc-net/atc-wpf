namespace Atc.Wpf.Theming.Automation.Peers;

public class NiceWindowAutomationPeer : WindowAutomationPeer
{
    public NiceWindowAutomationPeer(
        Window owner)
        : base(owner)
    {
    }

    protected override string GetClassNameCore()
    {
        return "NiceWindow";
    }
}