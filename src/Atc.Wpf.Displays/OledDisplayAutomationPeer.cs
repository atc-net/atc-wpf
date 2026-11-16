namespace Atc.Wpf.Displays;

/// <summary>
/// Exposes an <see cref="OledDisplay"/> to UI Automation: its name is the resolution, for example <c>OLED 128x64</c>.
/// An <see cref="AutomationProperties.NameProperty"/> set on the control wins.
/// </summary>
public sealed class OledDisplayAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OledDisplayAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The display.</param>
    public OledDisplayAutomationPeer(OledDisplay owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(OledDisplay);

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

        var display = (OledDisplay)Owner;
        return string.Create(
            CultureInfo.InvariantCulture,
            $"OLED {display.PixelColumns}x{display.PixelRows}");
    }
}