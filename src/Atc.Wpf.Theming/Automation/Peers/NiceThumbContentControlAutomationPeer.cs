namespace Atc.Wpf.Theming.Automation.Peers;

/// <summary>
/// Exposes a <see cref="NiceThumbContentControl"/> to UI Automation.
/// </summary>
public sealed class NiceThumbContentControlAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NiceThumbContentControlAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The element associated with this automation peer.</param>
    public NiceThumbContentControlAutomationPeer(FrameworkElement owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Custom;

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => "NiceThumbContentControl";
}