namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the "Windows theme sync" sample page, following the Windows app mode applies the
/// Windows light/dark setting, and picking a theme by hand stops following it.
/// </summary>
/// <remarks>
/// The test reads the Windows app mode from the page instead of changing it, so it works on any machine.
/// </remarks>
[Collection("SampleApp")]
public sealed class WindowsThemeSyncTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public WindowsThemeSyncTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Following_app_mode_applies_windows_mode_and_a_manual_pick_stops_it()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            tabAutomationId: "TabWpfTheming",
            parentTreeNodeName: "Theme",
            leafTreeNodeName: "Windows theme sync");

        var windowsMode = FindByAutomationId("TbWindowsAppMode").Name;
        var otherMode = windowsMode == "Light" ? "Dark" : "Light";
        var followAppMode = FindByAutomationId("CbFollowAppMode").AsCheckBox();
        Assert.False(followAppMode.IsChecked);

        PickTheme(otherMode);
        Assert.StartsWith(otherMode, CurrentTheme(), StringComparison.Ordinal);

        followAppMode.Click();
        WaitUntil(() => CurrentTheme().StartsWith(windowsMode, StringComparison.Ordinal));
        Assert.True(followAppMode.IsChecked);
        Assert.StartsWith(windowsMode, CurrentTheme(), StringComparison.Ordinal);

        PickTheme(otherMode);
        WaitUntil(() => followAppMode.IsChecked == false);
        Assert.False(followAppMode.IsChecked);
        Assert.StartsWith(otherMode, CurrentTheme(), StringComparison.Ordinal);
    }

    private AutomationElement FindByAutomationId(string automationId)
    {
        var element = Retry.WhileNull(
            () => mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId)),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(element);
        return element;
    }

    private string CurrentTheme()
        => FindByAutomationId("TbCurrentTheme").Name;

    /// <summary>
    /// ThemeSelector lists the base themes ordered by display name: Dark, then Light.
    /// </summary>
    private void PickTheme(string mode)
    {
        var comboBox = FindByAutomationId("PageThemeSelector")
            .FindFirstDescendant(cf => cf.ByControlType(ControlType.ComboBox))
            .AsComboBox();
        comboBox.Select(mode == "Dark" ? 0 : 1);
        comboBox.Collapse();
        WaitUntil(() => CurrentTheme().StartsWith(mode, StringComparison.Ordinal));
    }

    private static void WaitUntil(Func<bool> condition)
        => Retry.WhileFalse(
            condition,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
}