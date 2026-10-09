namespace Atc.Wpf.Controls.Inputs.Internal;

/// <summary>Exposes <see cref="ToggleSwitch"/> to UI Automation with the toggle pattern.</summary>
public class ToggleSwitchAutomationPeer : FrameworkElementAutomationPeer, IToggleProvider
{
    /// <summary>Initializes a new instance of the <see cref="ToggleSwitchAutomationPeer"/> class.</summary>
    public ToggleSwitchAutomationPeer(ToggleSwitch owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => "ToggleSwitch";

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Button;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
        => patternInterface == PatternInterface.Toggle
            ? this
            : base.GetPattern(patternInterface);

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    internal virtual void RaiseToggleStatePropertyChangedEvent(
        bool oldValue,
        bool newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        RaisePropertyChangedEvent(
            TogglePatternIdentifiers.ToggleStateProperty,
            ConvertToToggleState(oldValue),
            ConvertToToggleState(newValue));
    }

    private static ToggleState ConvertToToggleState(bool value)
        => value
            ? ToggleState.On
            : ToggleState.Off;

    /// <inheritdoc />
    public ToggleState ToggleState
        => ConvertToToggleState(((ToggleSwitch)Owner).IsOn);

    /// <inheritdoc />
    public void Toggle()
    {
        if (IsEnabled())
        {
            ((ToggleSwitch)Owner).AutomationPeerToggle();
        }
    }
}