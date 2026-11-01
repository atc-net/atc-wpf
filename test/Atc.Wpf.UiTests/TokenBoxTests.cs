namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test on the TokenBox sample page. The field is a UI Automation combo box with an edit child, a token list and
/// (while open) a suggestion list. Typing "or" opens the suggestions, Enter picks "Orders", a value of
/// "payments, billing" set through the Value pattern is handled as a paste, and invoking "Remove Payments" removes that
/// token. The page writes each <c>SelectedKeysChanged</c> event to its <c>SelectedKeysLog</c> text.
/// </summary>
[Collection("SampleApp")]
public sealed class TokenBoxTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public TokenBoxTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Typing_picking_pasting_and_removing_leaves_the_expected_tokens()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "TokenBox");

        var box = FindByAutomationId(mainWindow, "InteractiveBox");
        var log = FindByAutomationId(mainWindow, "SelectedKeysLog");
        Assert.Equal(ControlType.ComboBox, box.ControlType);
        Assert.Empty(TokenNames(box));

        var edit = box.FindFirstChild(cf => cf.ByControlType(ControlType.Edit));
        Assert.NotNull(edit);
        edit.Focus();
        Keyboard.Type("or");
        Wait.UntilInputIsProcessed();

        var expandCollapse = box.Patterns.ExpandCollapse.Pattern;
        Retry.WhileFalse(
            () => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Expanded,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        var suggestions = FindByAutomationId(box, "Suggestions");
        Assert.Contains(
            "Orders",
            suggestions.FindAllChildren(cf => cf.ByControlType(ControlType.ListItem)).Select(x => x.Name),
            StringComparer.Ordinal);

        Keyboard.Type(VirtualKeyShort.ENTER);
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 1,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(["Orders"], TokenNames(box));
        Assert.Equal(string.Empty, box.Patterns.Value.Pattern.Value.Value);
        Assert.Equal(ExpandCollapseState.Collapsed, expandCollapse.ExpandCollapseState.Value);

        box.Patterns.Value.Pattern.SetValue("payments, billing");
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 2,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(["Orders", "Payments", "Billing"], TokenNames(box));
        Assert.EndsWith("[orders] -> [orders, payments, billing]", Lines(log.Name)[1], StringComparison.Ordinal);

        var removePayments = Retry.WhileNull(
            () => box.FindFirstDescendant(cf => cf
                .ByControlType(ControlType.Button)
                .And(cf.ByName("Remove Payments"))),
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(removePayments);
        removePayments.Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 3,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(["Orders", "Billing"], TokenNames(box));
        var lines = Lines(log.Name);
        Assert.Equal(3, lines.Length);
        Assert.EndsWith("[orders, payments, billing] -> [orders, billing]", lines[2], StringComparison.Ordinal);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void A_click_on_empty_space_outside_the_box_closes_the_suggestions()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "TokenBox");

        var box = FindByAutomationId(mainWindow, "InteractiveBox");
        var edit = box.FindFirstChild(cf => cf.ByControlType(ControlType.Edit));
        Assert.NotNull(edit);
        edit.Focus();
        Keyboard.Type("or");
        Wait.UntilInputIsProcessed();

        var expandCollapse = box.Patterns.ExpandCollapse.Pattern;
        Retry.WhileFalse(
            () => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Expanded,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));

        // To the right of the box (it is left-aligned and 320 wide): empty, non-focusable panel space.
        var bounds = box.BoundingRectangle;
        Mouse.Click(new System.Drawing.Point(bounds.Right + 150, bounds.Top + (bounds.Height / 2)));
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => expandCollapse.ExpandCollapseState.Value == ExpandCollapseState.Collapsed,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Equal(ExpandCollapseState.Collapsed, expandCollapse.ExpandCollapseState.Value);
    }

    private static List<string> TokenNames(AutomationElement box)
    {
        var list = box.FindFirstChild(cf => cf.ByAutomationId("Tokens"));
        Assert.NotNull(list);
        return list
            .FindAllChildren(cf => cf.ByControlType(ControlType.ListItem))
            .Select(x => x.Name)
            .ToList();
    }

    private static string[] Lines(string text)
        => text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

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