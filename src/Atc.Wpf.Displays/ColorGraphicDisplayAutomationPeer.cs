namespace Atc.Wpf.Displays;

/// <summary>
/// Exposes a <see cref="ColorGraphicDisplay"/> to UI Automation: its name is the resolution, for example <c>Display 128x160</c>.
/// An <see cref="AutomationProperties.NameProperty"/> set on the control wins.
/// </summary>
public sealed class ColorGraphicDisplayAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorGraphicDisplayAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The display.</param>
    public ColorGraphicDisplayAutomationPeer(ColorGraphicDisplay owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(ColorGraphicDisplay);

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

        var display = (ColorGraphicDisplay)Owner;
        return string.Create(
            CultureInfo.InvariantCulture,
            $"Display {display.PixelColumns}x{display.PixelRows}");
    }
}