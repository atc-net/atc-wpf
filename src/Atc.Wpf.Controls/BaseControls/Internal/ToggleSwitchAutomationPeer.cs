namespace Atc.Wpf.Controls.BaseControls.Internal;

public class ToggleSwitchAutomationPeer : FrameworkElementAutomationPeer, IToggleProvider
{
    public ToggleSwitchAutomationPeer([NotNull] ToggleSwitch owner)
        : base(owner)
    {
    }

    protected override string GetClassNameCore()
        => "ToggleSwitch";

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Button;

    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Toggle
            ? this
            : base.GetPattern(patternInterface);

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    internal virtual void RaiseToggleStatePropertyChangedEvent(bool oldValue, bool newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, ConvertToToggleState(oldValue), ConvertToToggleState(newValue));
    }

    private static ToggleState ConvertToToggleState(bool value)
        => value
            ? ToggleState.On
            : ToggleState.Off;

    public ToggleState ToggleState => ConvertToToggleState(((ToggleSwitch)Owner).IsOn);

    public void Toggle()
    {
        if (IsEnabled())
        {
            ((ToggleSwitch)Owner).AutomationPeerToggle();
        }
    }
}