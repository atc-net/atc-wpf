// ReSharper disable SuggestBaseTypeForParameterInConstructor

namespace Atc.Wpf.Theming.Automation.Peers;

public class WindowCommandsAutomationPeer : FrameworkElementAutomationPeer
{
    public WindowCommandsAutomationPeer(
        WindowCommands owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
    {
        return "WindowCommands";
    }

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.ToolBar;
    }

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
    {
        return !((WindowCommands)Owner).HasItems || base.IsOffscreenCore();
    }

    protected override Point GetClickablePointCore()
    {
        if (!((WindowCommands)Owner).HasItems)
        {
            return new Point(double.NaN, double.NaN);
        }

        return base.GetClickablePointCore();
    }
}