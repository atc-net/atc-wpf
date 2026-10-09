namespace Atc.Wpf.Theming.Automation.Peers;

/// <summary>
/// Exposes a <see cref="NiceWindow"/> to UI Automation.
/// </summary>
public sealed class NiceWindowAutomationPeer : WindowAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NiceWindowAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The window associated with this automation peer.</param>
    public NiceWindowAutomationPeer(Window owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => "NiceWindow";
}