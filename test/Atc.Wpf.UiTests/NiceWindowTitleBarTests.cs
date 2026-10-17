namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E tests for the <c>NiceWindow</c> title bar: the centred title sits in the middle of the space between the
/// icon and the window buttons, also when the window has right-hand window commands, and the minimise,
/// maximise/restore and close buttons do what they say.
/// </summary>
/// <remarks>
/// The title is centred in the title bar's middle column, which runs from the icon column (the icon plus its
/// symmetric margin) to the minimise button. It is not centred on the whole window, because the icon column is
/// narrower than the window buttons.
/// </remarks>
[Collection("SampleApp")]
public sealed class NiceWindowTitleBarTests : IDisposable
{
    private const double CentreTolerance = 3;
    private const string RightWindowCommandsWindowTitle = "NiceWindow - RightWindowCommands";

    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public NiceWindowTitleBarTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Title_is_centred_between_the_icon_and_the_window_buttons_when_maximized_and_normal()
    {
        var windowPattern = mainWindow.Patterns.Window.Pattern;
        Assert.Equal(WindowVisualState.Maximized, windowPattern.WindowVisualState.Value);
        AssertTitleIsCentred(mainWindow);

        windowPattern.SetWindowVisualState(WindowVisualState.Normal);
        WaitUntil(() => windowPattern.WindowVisualState.Value == WindowVisualState.Normal);
        Assert.Equal(WindowVisualState.Normal, windowPattern.WindowVisualState.Value);
        WaitUntilTitleBarSettles(mainWindow);
        AssertTitleIsCentred(mainWindow);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Title_stays_centred_between_the_icon_and_the_window_buttons_with_right_window_commands()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            tabAutomationId: "TabWpfTheming",
            parentTreeNodeName: "Window",
            leafTreeNodeName: "NiceWindow");

        FindByName(mainWindow, ControlType.Button, "Open with RightWindowCommands").Patterns.Invoke.Pattern.Invoke();
        var window = Retry.WhileNull(
            () => application
                .GetAllTopLevelWindows(automation)
                .FirstOrDefault(x => x.Title == RightWindowCommandsWindowTitle),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(window);

        try
        {
            var rightWindowCommands = FindByName(window, ControlType.ToolBar, "RightWindowCommands");
            WaitUntil(() => rightWindowCommands.BoundingRectangle.Width > 0);
            Assert.True(rightWindowCommands.BoundingRectangle.Width > 0, "The right window commands should take up space.");
            Assert.Equal(0, FindByName(window, ControlType.ToolBar, "LeftWindowCommands").BoundingRectangle.Width);

            WaitUntilTitleBarSettles(window);
            AssertTitleIsCentred(window);
            Assert.True(
                FindTitleBar(window).BoundingRectangle.Right <= rightWindowCommands.BoundingRectangle.Left,
                "The title should not overlap the right window commands.");
        }
        finally
        {
            window.Close();
        }
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Window_buttons_maximize_restore_minimize_and_close_the_window()
    {
        var windowPattern = mainWindow.Patterns.Window.Pattern;
        var minimize = FindWindowButton("Minimize");
        var maximizeRestore = FindWindowButton("MaximizeRestore");
        var close = FindWindowButton("Close");

        Assert.Equal(WindowVisualState.Maximized, windowPattern.WindowVisualState.Value);
        Assert.Equal("Restore", maximizeRestore.Name);

        maximizeRestore.Patterns.Invoke.Pattern.Invoke();
        WaitForVisualState(windowPattern, WindowVisualState.Normal);
        WaitUntil(() => maximizeRestore.Name == "Maximize");
        Assert.Equal("Maximize", maximizeRestore.Name);

        maximizeRestore.Patterns.Invoke.Pattern.Invoke();
        WaitForVisualState(windowPattern, WindowVisualState.Maximized);
        WaitUntil(() => maximizeRestore.Name == "Restore");
        Assert.Equal("Restore", maximizeRestore.Name);

        // Minimise from the normal state, so restoring it afterwards returns to Normal rather than Maximized.
        maximizeRestore.Patterns.Invoke.Pattern.Invoke();
        WaitForVisualState(windowPattern, WindowVisualState.Normal);

        minimize.Patterns.Invoke.Pattern.Invoke();
        WaitForVisualState(windowPattern, WindowVisualState.Minimized);

        windowPattern.SetWindowVisualState(WindowVisualState.Normal);
        WaitForVisualState(windowPattern, WindowVisualState.Normal);

        close.Patterns.Invoke.Pattern.Invoke();
        var exited = Retry.WhileFalse(
            () => application.HasExited,
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.True(exited, "Closing the main window should end the sample app.");
    }

    private AutomationElement FindWindowButton(string automationId)
    {
        var button = mainWindow.FindFirstChild(cf => cf.ByControlType(ControlType.Button).And(cf.ByAutomationId(automationId)));
        Assert.NotNull(button);
        return button;
    }

    private static void AssertTitleIsCentred(Window window)
    {
        var thumb = FindByAutomationId(window, "PART_WindowTitleThumb").BoundingRectangle;
        var icon = window.FindFirstChild(cf => cf.ByControlType(ControlType.Image));
        Assert.NotNull(icon);
        var minimize = window.FindFirstChild(cf => cf.ByControlType(ControlType.Button).And(cf.ByAutomationId("Minimize")));
        Assert.NotNull(minimize);

        // The icon's margin is the same on both sides, so the icon column ends one left margin after the icon.
        var iconBounds = icon.BoundingRectangle;
        var middleColumnLeft = iconBounds.Right + (iconBounds.Left - thumb.Left);
        var middleColumnRight = minimize.BoundingRectangle.Left;
        var expectedCentre = (middleColumnLeft + middleColumnRight) / 2.0;

        var title = FindTitleBar(window).BoundingRectangle;
        var titleCentre = title.Left + (title.Width / 2.0);
        var message = $"Title centre {titleCentre} should be within {CentreTolerance}px of {expectedCentre}, " +
                      $"the middle of {middleColumnLeft}..{middleColumnRight} (title {title}).";
        Assert.True(
            System.Math.Abs(titleCentre - expectedCentre) <= CentreTolerance,
            message);
    }

    /// <summary>
    /// The title margin is updated on size changes, so wait until the title stops moving after a resize.
    /// </summary>
    private static void WaitUntilTitleBarSettles(Window window)
    {
        var titleBar = FindTitleBar(window);
        var previous = titleBar.BoundingRectangle;
        Retry.WhileFalse(
            () =>
            {
                var current = titleBar.BoundingRectangle;
                var settled = current == previous;
                previous = current;
                return settled;
            },
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(250));
    }

    private static AutomationElement FindTitleBar(Window window)
        => FindByAutomationId(window, "PART_TitleBar");

    private static AutomationElement FindByAutomationId(
        AutomationElement parent,
        string automationId)
    {
        var element = Retry.WhileNull(
            () => parent.FindFirstChild(cf => cf.ByAutomationId(automationId)),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(element);
        return element;
    }

    private static AutomationElement FindByName(
        AutomationElement parent,
        ControlType controlType,
        string name)
    {
        var element = Retry.WhileNull(
            () => parent.FindFirstDescendant(cf => cf.ByControlType(controlType).And(cf.ByName(name))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(element);
        return element;
    }

    private static void WaitForVisualState(
        FlaUI.Core.Patterns.IWindowPattern windowPattern,
        WindowVisualState expected)
    {
        WaitUntil(() => windowPattern.WindowVisualState.Value == expected);
        Assert.Equal(expected, windowPattern.WindowVisualState.Value);
    }

    private static void WaitUntil(Func<bool> condition)
        => Retry.WhileFalse(
            condition,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
}