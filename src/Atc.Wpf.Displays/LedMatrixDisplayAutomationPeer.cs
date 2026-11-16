namespace Atc.Wpf.Displays;

/// <summary>
/// Exposes an <see cref="LedMatrixDisplay"/> to UI Automation: its name is the resolution, for example <c>LED matrix 32x8</c>.
/// An <see cref="AutomationProperties.NameProperty"/> set on the control wins.
/// </summary>
public sealed class LedMatrixDisplayAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LedMatrixDisplayAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The display.</param>
    public LedMatrixDisplayAutomationPeer(LedMatrixDisplay owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(LedMatrixDisplay);

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

        var display = (LedMatrixDisplay)Owner;
        return string.Create(
            CultureInfo.InvariantCulture,
            $"LED matrix {display.ModuleCount * 8}x8");
    }
}