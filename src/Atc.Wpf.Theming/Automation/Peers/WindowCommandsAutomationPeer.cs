// ReSharper disable SuggestBaseTypeForParameterInConstructor
namespace Atc.Wpf.Theming.Automation.Peers;

/// <summary>
/// Exposes a <see cref="WindowCommands"/> control to UI Automation as a toolbar.
/// </summary>
public sealed class WindowCommandsAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowCommandsAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">The window commands control associated with this automation peer.</param>
    public WindowCommandsAutomationPeer(WindowCommands owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
        => "WindowCommands";

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.ToolBar;

    /// <inheritdoc />
    protected override string GetNameCore()
    {
        var nameCore = base.GetNameCore();

        if (string.IsNullOrEmpty(nameCore))
        {
            nameCore = ((WindowCommands)Owner).Name;
        }

        if (string.IsNullOrEmpty(nameCore))
        {
            nameCore = GetClassNameCore();
        }

        return nameCore!;
    }

    /// <inheritdoc />
    protected override bool IsOffscreenCore()
        => !((WindowCommands)Owner).HasItems || base.IsOffscreenCore();

    /// <inheritdoc />
    protected override Point GetClickablePointCore()
    {
        if (!((WindowCommands)Owner).HasItems)
        {
            return new Point(double.NaN, double.NaN);
        }

        return base.GetClickablePointCore();
    }
}