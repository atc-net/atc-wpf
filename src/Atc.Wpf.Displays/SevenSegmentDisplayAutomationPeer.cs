namespace Atc.Wpf.Displays;

/// <summary>
/// Exposes a <see cref="SevenSegmentDisplay"/> to UI Automation: its name is the digit count, for example <c>Seven-segment display, 8 digits</c>.
/// An <see cref="AutomationProperties.NameProperty"/> set on the control wins.
/// </summary>
public sealed class SevenSegmentDisplayAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SevenSegmentDisplayAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The display.</param>
    public SevenSegmentDisplayAutomationPeer(SevenSegmentDisplay owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(SevenSegmentDisplay);

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

        var display = (SevenSegmentDisplay)Owner;
        return string.Create(
            CultureInfo.InvariantCulture,
            $"Seven-segment display, {display.DigitCount} digits");
    }

    /// <inheritdoc />
    protected override List<AutomationPeer>? GetChildrenCore()
        => null;
}