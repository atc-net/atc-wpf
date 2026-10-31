namespace Atc.Wpf.UiTests;

/// <summary>
/// Smoke test: opens every sample in the sample app (found by <see cref="SampleCatalog"/>) and checks that it
/// loads without a data binding error, a sample load error or a crash. It does not exercise the samples.
/// </summary>
[Collection("SampleApp")]
public sealed class SampleSmokeTests : IClassFixture<SampleAppSession>
{
    internal static readonly string[] ProblemWindowTitles =
    [
        "Binding Error",
        "Error",
        "Dispatcher Unhandled Exception",
        "CurrentDomain Unhandled Exception",
    ];

    private readonly SampleAppSession session;

    public SampleSmokeTests(SampleAppSession session)
        => this.session = session;

    public static TheoryData<string, string, string> Samples()
    {
        var data = new TheoryData<string, string, string>();
        foreach (var sample in SampleCatalog.Load())
        {
            data.Add(sample.SamplePath, sample.Header, sample.TreeName);
        }

        return data;
    }

    [Trait("Category", "UI")]
    [Theory]
    [MemberData(nameof(Samples))]
    [SuppressMessage("Major Bug", "S2925:Do not use Thread.Sleep", Justification = "FlaUI E2E tests must yield real wall time for the sample to load.")]
    public void Sample_opens_without_binding_errors(
        string samplePath,
        string header,
        string treeName)
    {
        var mainWindow = session.EnsureRunning();

        var filterBox = Retry.WhileNull(
            () => mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("TbSampleFilter")),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(filterBox);
        filterBox.AsTextBox().Text = samplePath;
        Wait.UntilInputIsProcessed();

        var leaf = Retry.WhileNull(
            () => mainWindow
                .FindFirstDescendant(cf => cf.ByAutomationId(treeName))
                ?.FindFirstDescendant(cf => cf.ByControlType(ControlType.TreeItem).And(cf.ByName(header))),
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.True(leaf is not null, $"The tree item '{header}' ({samplePath}) was not found in {treeName}.");

        leaf.Patterns.ScrollItem.PatternOrDefault?.ScrollIntoView();
        Wait.UntilInputIsProcessed();

        // SampleTreeViewItem loads its sample on a real mouse click (OnMouseLeftButtonDown).
        leaf.Click();
        Wait.UntilInputIsProcessed();
        Thread.Sleep(1500);

        var problems = session.TakeProblemWindows(ProblemWindowTitles);
        Assert.True(
            problems.Count == 0,
            $"Opening the sample '{header}' ({samplePath}) showed:{Environment.NewLine}{string.Join(Environment.NewLine, problems)}");
        Assert.False(session.HasExited, $"The sample app exited after opening '{header}' ({samplePath}).");
    }
}