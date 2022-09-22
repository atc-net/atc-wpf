namespace Atc.Wpf.Theming.Automation.Peers;

public class NiceDialogAutomationPeer : FrameworkElementAutomationPeer
{
    public NiceDialogAutomationPeer(
        NiceDialogBase owner)
        : base(owner)
    {
    }

    protected override string GetClassNameCore()
    {
        return Owner.GetType().Name;
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Custom;
    }

    protected override string GetNameCore()
    {
        var nameCore = base.GetNameCore();
        if (string.IsNullOrEmpty(nameCore))
        {
            nameCore = ((NiceDialogBase)Owner).Title;
        }

        if (string.IsNullOrEmpty(nameCore))
        {
            nameCore = ((NiceDialogBase)Owner).Name;
        }

        if (string.IsNullOrEmpty(nameCore))
        {
            nameCore = GetClassNameCore();
        }

        return nameCore!;
    }
}