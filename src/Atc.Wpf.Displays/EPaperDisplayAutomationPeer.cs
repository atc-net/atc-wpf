namespace Atc.Wpf.Displays;

/// <summary>
/// Exposes an <see cref="EPaperDisplay"/> to UI Automation: its name is the native resolution, for example <c>Display 128x296</c>.
/// An <see cref="AutomationProperties.NameProperty"/> set on the control wins.
/// </summary>
public sealed class EPaperDisplayAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EPaperDisplayAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The display.</param>
    public EPaperDisplayAutomationPeer(EPaperDisplay owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(EPaperDisplay);

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

        var display = (EPaperDisplay)Owner;
        return string.Create(
            CultureInfo.InvariantCulture,
            $"Display {display.VisibleColumns ?? display.PixelColumns}x{display.PixelRows}");
    }
}