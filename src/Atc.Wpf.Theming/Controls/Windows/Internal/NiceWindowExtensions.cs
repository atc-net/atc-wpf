// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace Atc.Wpf.Theming.Controls.Windows.Internal;

[SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "OK.")]
internal static class NiceWindowExtensions
{
    public static void SetIsHitTestVisibleInChromeProperty<T>(
        this NiceWindow window,
        string name,
        bool hitTestVisible = true)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(window);

        var inputElement = window.GetPart<T>(name) as IInputElement;
        Debug.Assert(inputElement is not null, $"{name} is not a IInputElement");
        if (inputElement is not null &&
            WindowChrome.GetIsHitTestVisibleInChrome(inputElement) != hitTestVisible)
        {
            WindowChrome.SetIsHitTestVisibleInChrome(inputElement, hitTestVisible);
        }
    }

    public static void SetWindowChromeResizeGripDirection(
        this NiceWindow window,
        string name,
        ResizeGripDirection direction)
    {
        ArgumentNullException.ThrowIfNull(window);

        var inputElement = window.GetPart<IInputElement>(name);
        Debug.Assert(inputElement is not null, $"{name} is not a IInputElement");
        if (inputElement is not null && WindowChrome.GetResizeGripDirection(inputElement) != direction)
        {
            WindowChrome.SetResizeGripDirection(inputElement, direction);
        }
    }

    private static Theme? GetCurrentTheme(
        this NiceWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        var currentTheme = ThemeManager.Current.DetectTheme(window);
        if (currentTheme is null)
        {
            var application = Application.Current;
            if (application is not null)
            {
                currentTheme = application.MainWindow is null
                    ? ThemeManager.Current.DetectTheme(application)
                    : ThemeManager.Current.DetectTheme(application.MainWindow);
            }
        }

        return currentTheme;
    }

    public static void ResetAllWindowCommandsBrush(
        this NiceWindow window)
    {
        var currentTheme = window.GetCurrentTheme();

        window.ChangeAllWindowCommandsBrush(window.OverrideDefaultWindowCommandsBrush, currentTheme);
        window.ChangeAllWindowButtonCommandsBrush(window.OverrideDefaultWindowCommandsBrush, currentTheme);
    }

    private static void ChangeAllWindowCommandsBrush(
        this NiceWindow window,
        Brush? foregroundBrush,
        Theme? currentAppTheme)
    {
        if (foregroundBrush is null)
        {
            window.LeftWindowCommands?.ClearValue(Control.ForegroundProperty);
            window.RightWindowCommands?.ClearValue(Control.ForegroundProperty);
        }

        var theme = currentAppTheme is not null && currentAppTheme.BaseColorScheme == ThemeManager.BaseColorDark
            ? ThemeManager.BaseColorDark
            : ThemeManager.BaseColorLight;

        window.LeftWindowCommands?.SetValue(WindowCommands.ThemeProperty, theme);
        window.RightWindowCommands?.SetValue(WindowCommands.ThemeProperty, theme);

        if (foregroundBrush is not null)
        {
            window.LeftWindowCommands?.SetValue(Control.ForegroundProperty, foregroundBrush);
            window.RightWindowCommands?.SetValue(Control.ForegroundProperty, foregroundBrush);
        }
    }

    private static void ChangeAllWindowButtonCommandsBrush(
        this NiceWindow window,
        Brush? foregroundBrush,
        Theme? currentAppTheme,
        PositionType position = PositionType.Top)
    {
        if (position != PositionType.Right && position != PositionType.Top)
        {
            return;
        }

        if (foregroundBrush is null)
        {
            window.WindowButtonCommands?.ClearValue(Control.ForegroundProperty);
        }

        var theme = currentAppTheme is not null && currentAppTheme.BaseColorScheme == ThemeManager.BaseColorDark
            ? ThemeManager.BaseColorDark
            : ThemeManager.BaseColorLight;

        window.WindowButtonCommands?.SetValue(WindowButtonCommands.ThemeProperty, theme);

        if (foregroundBrush is not null)
        {
            window.WindowButtonCommands?.SetValue(Control.ForegroundProperty, foregroundBrush);
        }
    }
}