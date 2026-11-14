namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// Exposes a <see cref="CharacterDisplay"/> to UI Automation as text: its name is the visible text
/// (the rows joined by a new line, custom glyphs shown as spaces), so screen readers and UI tests can read the display.
/// An <see cref="AutomationProperties.NameProperty"/> set on the control wins.
/// </summary>
public sealed class CharacterDisplayAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterDisplayAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The display.</param>
    public CharacterDisplayAutomationPeer(CharacterDisplay owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => nameof(CharacterDisplay);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Text;

    /// <inheritdoc />
    protected override string GetNameCore()
    {
        var name = base.GetNameCore();
        return string.IsNullOrEmpty(name)
            ? ((CharacterDisplay)Owner).GetDisplayedText()
            : name;
    }
}