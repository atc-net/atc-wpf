namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: the TrayIcon sample page adds its icon to the Windows notification area,
/// removes it when hidden, and adds it again when shown.
/// </summary>
/// <remarks>
/// The test reads the page's own status instead of driving Explorer's notification area,
/// whose overflow and hidden-icon settings vary between machines.
/// </remarks>
[Collection("SampleApp")]
public sealed class TrayIconTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public TrayIconTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Show_icon_adds_and_removes_the_notification_area_icon()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            tabAutomationId: "TabWpfComponents",
            parentTreeNodeName: "Notifications",
            leafTreeNodeName: "TrayIcon");

        WaitUntil(() => Created().EndsWith("True", StringComparison.Ordinal));
        Assert.EndsWith("True", Created(), StringComparison.Ordinal);

        var showIcon = FindByAutomationId(mainWindow, "CbShowIcon").AsCheckBox();
        showIcon.Toggle();
        WaitUntil(() => Created().EndsWith("False", StringComparison.Ordinal));
        Assert.EndsWith("False", Created(), StringComparison.Ordinal);

        showIcon.Toggle();
        WaitUntil(() => Created().EndsWith("True", StringComparison.Ordinal));
        Assert.EndsWith("True", Created(), StringComparison.Ordinal);
    }

    private string Created()
        => FindByAutomationId(mainWindow, "TbTrayIconCreated").Name;

    private static AutomationElement FindByAutomationId(
        AutomationElement parent,
        string automationId)
    {
        var element = Retry.WhileNull(
            () => parent.FindFirstDescendant(cf => cf.ByAutomationId(automationId)),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(element);
        return element;
    }

    private static void WaitUntil(Func<bool> condition)
        => Retry.WhileFalse(
            condition,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
}