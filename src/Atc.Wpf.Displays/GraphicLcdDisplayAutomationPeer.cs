namespace Atc.Wpf.Displays;

/// <summary>
/// Exposes a <see cref="GraphicLcdDisplay"/> to UI Automation: its name is the resolution, <c>LCD 128x64</c>.
/// An <see cref="AutomationProperties.NameProperty"/> set on the control wins.
/// </summary>
public sealed class GraphicLcdDisplayAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicLcdDisplayAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The display.</param>
    public GraphicLcdDisplayAutomationPeer(GraphicLcdDisplay owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(GraphicLcdDisplay);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Image;

    /// <inheritdoc />
    protected override string GetNameCore()
    {
        var name = base.GetNameCore();
        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        return string.Create(
            CultureInfo.InvariantCulture,
            $"LCD {GraphicLcdDisplay.PixelColumns}x{GraphicLcdDisplay.PixelRows}");
    }
}