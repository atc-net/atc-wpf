namespace Atc.Wpf.Theming.Automation.Peers;

public sealed class NiceWindowAutomationPeer : WindowAutomationPeer
{
    public NiceWindowAutomationPeer(Window owner)
        : base(owner)
    {
    }

    protected override string GetClassNameCore()
        => "NiceWindow";
}