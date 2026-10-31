namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E tests on the DataGridFilter sample page (5,000 orders). The filter button of a column is an Invoke button
/// named "Filter {column}"; its pop-up's elements are children of the button in the UI Automation tree. The page's
/// row count and FilterChanged log show the result. The sample app is launched with binding-error tracing, so every
/// test also asserts that no "Binding Error" dialog appeared.
/// </summary>
[Collection("SampleApp")]
public sealed class DataGridFilterTests : IDisposable
{
    private const int AllRows = 5_000;
    private static readonly TimeSpan PopupAnimation = TimeSpan.FromSeconds(1);

    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public DataGridFilterTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow, traceBindingErrors: true);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Unticking_a_status_and_ok_filters_the_rows_and_clear_filter_shows_them_again()
    {
        var page = OpenPage();

        var filterButton = OpenFilter(page.Grid, "Status");
        var cancelled = FindDescendant(filterButton, cf => cf.ByControlType(ControlType.CheckBox).And(cf.ByName("Cancelled")));
        Assert.Equal(ToggleState.On, cancelled.Patterns.Toggle.Pattern.ToggleState.Value);
        cancelled.Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();
        Assert.Equal(ToggleState.Off, cancelled.Patterns.Toggle.Pattern.ToggleState.Value);

        Invoke(filterButton, "Ok");

        var filtered = WaitForLogLine(page, 1);
        Assert.Contains("filters=[Status in (Closed|Open|Pending|Shipped)]", filtered, StringComparison.Ordinal);
        var rows = RowCount(page);
        Assert.InRange(rows, 1, AllRows - 1);
        Assert.Contains($"rows={rows} ", filtered, StringComparison.Ordinal);

        ClearFilter(page.Grid, "Status");

        Assert.Contains("rows=5000 filters=[]", WaitForLogLine(page, 2), StringComparison.Ordinal);
        Assert.Equal(AllRows, RowCount(page));
        BindingErrorWindows.AssertNone(application, automation);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Text_contains_filter_typed_in_the_pop_up_filters_the_rows_without_binding_errors()
    {
        var page = OpenPage();

        // Scrolled rows are the ones the filter's refresh recycles.
        page.Grid.Patterns.Scroll.Pattern.SetScrollPercent(-1, 50);
        Wait.UntilInputIsProcessed();

        var filterButton = OpenFilter(page.Grid, "Customer");
        var textMode = FindDescendant(filterButton, cf => cf.ByAutomationId("FilterTextMode"));
        textMode.Patterns.SelectionItem.Pattern.Select();
        Wait.UntilInputIsProcessed();

        var textOperator = FindDescendant(filterButton, cf => cf.ByAutomationId("FilterTextOperator"));
        Assert.Contains("Contains", textOperator.Patterns.Selection.Pattern.Selection.Value.Single().Name, StringComparison.Ordinal);

        var text = FindDescendant(filterButton, cf => cf.ByAutomationId("FilterText"));
        text.Patterns.Value.Pattern.SetValue("wood");
        Wait.UntilInputIsProcessed();
        BindingErrorWindows.AssertNone(application, automation);

        Invoke(filterButton, "Ok");

        var filtered = WaitForLogLine(page, 1);
        Assert.Contains("filters=[Customer Contains 'wood']", filtered, StringComparison.Ordinal);
        var rows = RowCount(page);
        Assert.InRange(rows, 1, AllRows - 1);
        Assert.Contains($"rows={rows} ", filtered, StringComparison.Ordinal);
        WaitForPopupAnimation();
        BindingErrorWindows.AssertNone(application, automation);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Values_filters_on_two_columns_and_together_and_the_second_lists_only_remaining_values()
    {
        var page = OpenPage();

        // Before any filter: how many dates the Date pop-up offers.
        var dateButton = OpenFilter(page.Grid, "Date");
        var allDates = SelectionTotal(dateButton);
        Invoke(dateButton, "Cancel");

        // Customer: only Contoso.
        var customerButton = OpenFilter(page.Grid, "Customer");
        var selectAll = FindDescendant(customerButton, cf => cf.ByAutomationId("SelectAll")).Patterns.Toggle.Pattern;
        selectAll.Toggle();
        Wait.UntilInputIsProcessed();
        Assert.Equal(ToggleState.Off, selectAll.ToggleState.Value);
        FindDescendant(customerButton, cf => cf.ByControlType(ControlType.CheckBox).And(cf.ByName("Contoso"))).Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();
        Invoke(customerButton, "Ok");
        Assert.Contains("filters=[Customer in (Contoso)]", WaitForLogLine(page, 1), StringComparison.Ordinal);
        var contosoRows = RowCount(page);
        Assert.InRange(contosoRows, 1, AllRows - 1);

        // Status: everything but Cancelled, on top of the Customer filter.
        var statusButton = OpenFilter(page.Grid, "Status");
        FindDescendant(statusButton, cf => cf.ByControlType(ControlType.CheckBox).And(cf.ByName("Cancelled"))).Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();
        Invoke(statusButton, "Ok");
        var both = WaitForLogLine(page, 2);
        Assert.Contains("filters=[Customer in (Contoso), Status in (Closed|Open|Pending|Shipped)]", both, StringComparison.Ordinal);
        var bothRows = RowCount(page);
        Assert.InRange(bothRows, 1, contosoRows - 1);

        // The Date pop-up now only offers the dates of the remaining rows.
        dateButton = OpenFilter(page.Grid, "Date");
        var remainingDates = SelectionTotal(dateButton);
        Assert.InRange(remainingDates, 1, System.Math.Min(bothRows, allDates - 1));
        Invoke(dateButton, "Cancel");

        ClearFilter(page.Grid, "Customer");
        Assert.Contains("filters=[Status in (Closed|Open|Pending|Shipped)]", WaitForLogLine(page, 3), StringComparison.Ordinal);
        ClearFilter(page.Grid, "Status");
        Assert.Contains("rows=5000 filters=[]", WaitForLogLine(page, 4), StringComparison.Ordinal);
        Assert.Equal(AllRows, RowCount(page));
        BindingErrorWindows.AssertNone(application, automation);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Search_box_filters_the_rows_and_clearing_it_shows_them_again()
    {
        var page = OpenPage();
        var search = FindByAutomationId(mainWindow, "SearchBox");

        search.Patterns.Value.Pattern.SetValue("Contoso");
        Assert.Contains("search='Contoso'", WaitForLogLine(page, 1), StringComparison.Ordinal);
        Assert.InRange(RowCount(page), 1, AllRows - 1);

        search.Patterns.Value.Pattern.SetValue(string.Empty);
        Assert.Contains("rows=5000 filters=[] search=''", WaitForLogLine(page, 2), StringComparison.Ordinal);
        Assert.Equal(AllRows, RowCount(page));
        BindingErrorWindows.AssertNone(application, automation);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Column_chooser_hides_a_column_and_shows_it_again()
    {
        var page = OpenPage();
        Assert.True(HasHeader(page.Grid, "Country"));

        ToggleColumnInChooser("Country", expectedAfter: ToggleState.Off);
        WaitUntil(() => !HasHeader(page.Grid, "Country"));
        Assert.True(HasHeader(page.Grid, "Status"));

        ToggleColumnInChooser("Country", expectedAfter: ToggleState.On);
        WaitUntil(() => HasHeader(page.Grid, "Country"));
        BindingErrorWindows.AssertNone(application, automation);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Alt_down_on_a_cell_opens_the_filter_and_escape_closes_it_without_changing_the_rows()
    {
        var page = OpenPage();

        // Row 0, column 1 is a Customer cell (the Id column has no filter).
        var cell = page.Grid.Patterns.Grid.Pattern.GetItem(0, 1);
        Assert.NotNull(cell);

        cell.Focus();
        Wait.UntilInputIsProcessed();

        Keyboard.TypeSimultaneously(VirtualKeyShort.ALT, VirtualKeyShort.DOWN);
        WaitForPopupAnimation();
        Assert.NotNull(FindDescendant(page.Grid, cf => cf.ByAutomationId("Ok")));

        Keyboard.Type(VirtualKeyShort.ESCAPE);
        WaitUntil(() => page.Grid.FindFirstDescendant(cf => cf.ByAutomationId("Ok")) is null);

        Assert.Equal(AllRows, RowCount(page));
        Assert.Empty(Lines(page.Log.Name));
        BindingErrorWindows.AssertNone(application, automation);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Restore_layout_brings_back_the_saved_filters_and_columns()
    {
        var page = OpenPage();

        FindByAutomationId(mainWindow, "SaveLayoutButton").Patterns.Invoke.Pattern.Invoke();
        Assert.Equal("Layout saved", WaitForLogLine(page, 1));

        var statusButton = OpenFilter(page.Grid, "Status");
        FindDescendant(statusButton, cf => cf.ByControlType(ControlType.CheckBox).And(cf.ByName("Cancelled"))).Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();
        Invoke(statusButton, "Ok");
        WaitForLogLine(page, 2);
        Assert.InRange(RowCount(page), 1, AllRows - 1);
        ToggleColumnInChooser("Country", expectedAfter: ToggleState.Off);
        WaitUntil(() => !HasHeader(page.Grid, "Country"));

        var linesBeforeRestore = Lines(page.Log.Name).Length;
        FindByAutomationId(mainWindow, "RestoreLayoutButton").Patterns.Invoke.Pattern.Invoke();

        WaitUntil(() => Lines(page.Log.Name).Length > linesBeforeRestore);
        Assert.Contains("rows=5000 filters=[]", Lines(page.Log.Name)[^1], StringComparison.Ordinal);
        Assert.Equal(AllRows, RowCount(page));
        WaitUntil(() => HasHeader(page.Grid, "Country"));
        BindingErrorWindows.AssertNone(application, automation);
    }

    private SamplePage OpenPage()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            "TabWpfControls",
            parentTreeNodeName: string.Empty,
            leafTreeNodeName: "DataGridFilter");

        var page = new SamplePage(
            FindByAutomationId(mainWindow, "OrdersGrid"),
            FindByAutomationId(mainWindow, "OrdersRowCount"),
            FindByAutomationId(mainWindow, "FilterLog"));
        Assert.Equal($"{AllRows} rows", page.RowCountText.Name);
        return page;
    }

    private void ToggleColumnInChooser(
        string column,
        ToggleState expectedAfter)
    {
        FindByAutomationId(mainWindow, "ColumnsButton").Patterns.Invoke.Pattern.Invoke();
        WaitForPopupAnimation();

        // The menu is a pop-up: UI Automation lists it under the desktop or under the main window.
        var item = Retry.WhileNull(
            () => automation.GetDesktop()
                .FindAllChildren(cf => cf.ByProcessId(application.ProcessId))
                .Append(mainWindow)
                .Select(x => x.FindFirstDescendant(cf => cf.ByControlType(ControlType.MenuItem).And(cf.ByName(column))))
                .FirstOrDefault(x => x is not null),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.True(item is not null, $"Column chooser item '{column}' not found");

        item.Patterns.Toggle.Pattern.Toggle();
        Wait.UntilInputIsProcessed();
        Assert.Equal(expectedAfter, item.Patterns.Toggle.Pattern.ToggleState.Value);

        Keyboard.Type(VirtualKeyShort.ESCAPE);
        Wait.UntilInputIsProcessed();
    }

    private static AutomationElement OpenFilter(
        AutomationElement grid,
        string column)
    {
        var filterButton = FindFilterButton(grid, column);
        filterButton.Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();
        WaitForPopupAnimation();
        return filterButton;
    }

    private static void ClearFilter(
        AutomationElement grid,
        string column)
        => Invoke(OpenFilter(grid, column), "ClearFilter");

    private static void Invoke(
        AutomationElement parent,
        string automationId)
    {
        FindDescendant(parent, cf => cf.ByAutomationId(automationId)).Patterns.Invoke.Pattern.Invoke();
        Wait.UntilInputIsProcessed();
    }

    private static int SelectionTotal(AutomationElement filterButton)
    {
        // The value list shows "{selected} of {total}".
        var count = FindDescendant(filterButton, cf => cf.ByAutomationId("SelectionCount")).Name;
        return int.Parse(count.Split(' ')[^1], NumberStyles.AllowThousands, CultureInfo.CurrentCulture);
    }

    private static bool HasHeader(
        AutomationElement grid,
        string column)
        => grid
            .FindAllDescendants(cf => cf.ByControlType(ControlType.HeaderItem))
            .Any(x => string.Equals(x.Name, column, StringComparison.Ordinal));

    [SuppressMessage("Major Bug", "S2925:Do not use Thread.Sleep", Justification = "FlaUI E2E tests must yield real wall time for the pop-up animation.")]
    private static void WaitForPopupAnimation()
        => Thread.Sleep(PopupAnimation);

    private static int RowCount(SamplePage page)
        => int.Parse(page.RowCountText.Name.Split(' ')[0], CultureInfo.InvariantCulture);

    private static string WaitForLogLine(
        SamplePage page,
        int count)
    {
        WaitUntil(() => Lines(page.Log.Name).Length >= count);
        var lines = Lines(page.Log.Name);
        Assert.Equal(count, lines.Length);
        return lines[count - 1];
    }

    private static void WaitUntil(Func<bool> condition)
        => Assert.True(
            Retry.WhileFalse(condition, timeout: TimeSpan.FromSeconds(10), interval: TimeSpan.FromMilliseconds(200)).Result,
            "The condition was not met within 10 seconds.");

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
        Func<FlaUI.Core.Conditions.ConditionFactory, FlaUI.Core.Conditions.ConditionBase> condition,
        [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(condition))] string? description = null)
    {
        var element = Retry.WhileNull(
            () => parent.FindFirstDescendant(condition),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.True(element is not null, $"Not found: {description}");
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

    private sealed record SamplePage(
        AutomationElement Grid,
        AutomationElement RowCountText,
        AutomationElement Log);
}