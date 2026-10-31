namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test on the DataGridFilter sample page. Invoking the filter button of the Status column (an Invoke button
/// named "Filter Status") opens the filter pop-up, whose rows are children of the button in the UI Automation tree;
/// unticking "Cancelled" and invoking OK filters the 5,000 orders, which the page's row count and FilterChanged log
/// show; Clear filter in the reopened pop-up brings all rows back.
/// </summary>
[Collection("SampleApp")]
public sealed class DataGridFilterTests : IDisposable
{
    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public DataGridFilterTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Unticking_a_status_and_ok_filters_the_rows_and_clear_filter_shows_them_again()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "DataGridFilter");

        var grid = FindByAutomationId(mainWindow, "OrdersGrid");
        var rowCount = FindByAutomationId(mainWindow, "OrdersRowCount");
        var log = FindByAutomationId(mainWindow, "FilterLog");
        Assert.Equal("5000 rows", rowCount.Name);

        var filterButton = FindFilterButton(grid, "Status");
        filterButton.Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();

        var cancelled = FindDescendant(filterButton, cf => cf.ByControlType(ControlType.CheckBox).And(cf.ByName("Cancelled")));
        Assert.Equal(ToggleState.On, cancelled.Patterns.Toggle.Pattern.ToggleState.Value);
        cancelled.Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();
        Assert.Equal(ToggleState.Off, cancelled.Patterns.Toggle.Pattern.ToggleState.Value);

        FindDescendant(filterButton, cf => cf.ByAutomationId("Ok")).Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 1,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        var filtered = Lines(log.Name)[0];
        Assert.Contains("filters=[Status in (Closed|Open|Pending|Shipped)]", filtered, StringComparison.Ordinal);
        var rows = int.Parse(rowCount.Name.Split(' ')[0], CultureInfo.InvariantCulture);
        Assert.InRange(rows, 1, 4_999);
        Assert.Contains($"rows={rows} ", filtered, StringComparison.Ordinal);

        filterButton = FindFilterButton(grid, "Status");
        filterButton.Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();

        FindDescendant(filterButton, cf => cf.ByAutomationId("ClearFilter")).Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();

        Retry.WhileFalse(
            () => Lines(log.Name).Length == 2,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
        Assert.Contains("rows=5000 filters=[]", Lines(log.Name)[1], StringComparison.Ordinal);
        Assert.Equal("5000 rows", rowCount.Name);
    }

    private static string[] Lines(string text)
        => text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

    private static AutomationElement FindFilterButton(
        AutomationElement grid,
        string column)
    {
        var button = Retry.WhileNull(
            () => grid
                .FindAllDescendants(cf => cf.ByControlType(ControlType.Button).And(cf.ByClassName("DataGridFilterButton")))
                .FirstOrDefault(x => x.Name.EndsWith(column, StringComparison.Ordinal)),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(button);
        return button;
    }

    private static AutomationElement FindDescendant(
        AutomationElement parent,
        Func<FlaUI.Core.Conditions.ConditionFactory, FlaUI.Core.Conditions.ConditionBase> condition)
    {
        var element = Retry.WhileNull(
            () => parent.FindFirstDescendant(condition),
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