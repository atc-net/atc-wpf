namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the ToastNotificationService sample page, a toast appears in the top-right corner of the page's
/// notification area, closes by itself after its 5 second duration, and closes at once from its close button.
/// </summary>
/// <remarks>
/// The toast controls have no automation peer of their own, so the test finds a toast by its title text and its
/// <c>PART_CloseButton</c>, and measures the area by the page's scroll viewer, which the area fills.
/// </remarks>
[Collection("SampleApp")]
public sealed class ToastNotificationTests : IDisposable
{
    private const string CloseButtonAutomationId = "PART_CloseButton";
    private const int CornerTolerance = 50;

    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public ToastNotificationTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Toast_appears_in_the_top_right_corner_and_closes_after_its_duration()
    {
        var view = NavigateToToastSample();
        var usageArea = FindUsageArea(view);

        var stopwatch = Stopwatch.StartNew();
        FindButton(view, "Information").Patterns.Invoke.Pattern.Invoke();

        var title = FindToastTitle(view, "Information - ");
        var closeButton = WaitUntilToastSettles(view);
        AssertInTopRightCorner(usageArea, title, closeButton);

        var closed = Retry.WhileFalse(
            () => FindToastTitleOrDefault(view, "Information - ") is null,
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(100)).Result;
        stopwatch.Stop();

        Assert.True(closed, "The toast should close by itself.");
        Assert.InRange(stopwatch.Elapsed, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(8));
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Toast_closes_at_once_from_its_close_button()
    {
        var view = NavigateToToastSample();

        // This toast stays for 10 seconds, so closing within 2 seconds can only come from the close button.
        FindButton(view, "10s Duration").Patterns.Invoke.Pattern.Invoke();
        Assert.NotNull(FindToastTitle(view, "Custom Duration - "));

        var closeButton = WaitUntilToastSettles(view);

        var stopwatch = Stopwatch.StartNew();
        closeButton.Patterns.Invoke.Pattern.Invoke();
        var closed = Retry.WhileFalse(
            () => FindToastTitleOrDefault(view, "Custom Duration - ") is null,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(100)).Result;
        stopwatch.Stop();

        Assert.True(closed, "The toast should close from its close button.");
        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(2),
            $"The toast took {stopwatch.Elapsed} to close from its close button.");
    }

    private AutomationElement NavigateToToastSample()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            tabAutomationId: "TabWpfComponents",
            parentTreeNodeName: "Notifications",
            leafTreeNodeName: "ToastNotificationService");

        var view = Retry.WhileNull(
            () => mainWindow.FindFirstDescendant(cf => cf.ByClassName("ToastNotificationServiceView")),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(view);
        Assert.Null(FindToastTitleOrDefault(view, string.Empty));
        return view;
    }

    /// <summary>
    /// The sample's <c>ServiceArea</c> fills the usage scroll viewer, which is the first scroll viewer in the page.
    /// </summary>
    private static AutomationElement FindUsageArea(AutomationElement view)
    {
        var scrollViewer = view.FindFirstChild(cf => cf.ByClassName("ScrollViewer"));
        Assert.NotNull(scrollViewer);
        return scrollViewer;
    }

    private static void AssertInTopRightCorner(
        AutomationElement usageArea,
        AutomationElement title,
        AutomationElement closeButton)
    {
        var area = usageArea.BoundingRectangle;
        var scrollBar = usageArea.FindFirstChild(cf => cf.ByControlType(ControlType.ScrollBar));
        var areaRight = scrollBar is null || scrollBar.IsOffscreen
            ? area.Right
            : scrollBar.BoundingRectangle.Left;

        var toastRight = closeButton.BoundingRectangle.Right;
        var toastTop = title.BoundingRectangle.Top;
        Assert.True(
            areaRight - toastRight is >= 0 and <= CornerTolerance,
            $"The toast's right edge {toastRight} should be within {CornerTolerance}px of the area's right edge {areaRight}.");
        Assert.True(
            toastTop - area.Top is >= 0 and <= CornerTolerance,
            $"The toast's top {toastTop} should be within {CornerTolerance}px of the area's top {area.Top}.");
    }

    /// <summary>
    /// A new toast scales in from zero size, so wait until its close button has a size that stops changing.
    /// </summary>
    private static AutomationElement WaitUntilToastSettles(
        AutomationElement view)
    {
        var closeButton = FindByAutomationId(view, CloseButtonAutomationId);
        var previous = System.Drawing.Rectangle.Empty;
        var settled = Retry.WhileFalse(
            () =>
            {
                var current = closeButton.BoundingRectangle;
                var isStable = current.Width > 0 && current == previous;
                previous = current;
                return isStable;
            },
            timeout: TimeSpan.FromSeconds(3),
            interval: TimeSpan.FromMilliseconds(150)).Result;
        Assert.True(settled, "The toast should finish its entry animation.");
        return closeButton;
    }

    private static AutomationElement FindToastTitle(
        AutomationElement view,
        string titlePrefix)
    {
        var title = Retry.WhileNull(
            () => FindToastTitleOrDefault(view, titlePrefix),
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(100)).Result;
        Assert.NotNull(title);
        return title;
    }

    /// <summary>
    /// Returns the title of the open toast, found as the text next to its close button.
    /// </summary>
    private static AutomationElement? FindToastTitleOrDefault(
        AutomationElement view,
        string titlePrefix)
    {
        var usageArea = view.FindFirstChild(cf => cf.ByClassName("ScrollViewer"));
        if (usageArea?.FindFirstChild(cf => cf.ByAutomationId(CloseButtonAutomationId)) is null)
        {
            return null;
        }

        return usageArea
            .FindAllChildren(cf => cf.ByControlType(ControlType.Text))
            .FirstOrDefault(x => x.Name.StartsWith(titlePrefix, StringComparison.Ordinal));
    }

    private static AutomationElement FindButton(
        AutomationElement parent,
        string name)
    {
        var element = Retry.WhileNull(
            () => parent.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button).And(cf.ByName(name))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(element);
        return element;
    }

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
}