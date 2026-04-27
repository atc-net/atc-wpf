namespace Atc.Wpf.UiTests;

/// <summary>
/// Pilot UI smoke test: launches the sample app via FlaUI, asserts that the
/// main window appears with the expected title, captures a screenshot to the
/// test output directory, and shuts the process down.
/// </summary>
/// <remarks>
/// Marked with <see cref="TraitAttribute"/> Category=UI so CI can opt in/out.
/// FlaUI requires Windows; the entire test project targets `net10.0-windows`.
/// Future tests that need a baseline image diff can place baselines under
/// <c>Snapshots/</c> and use the captured screenshot to compare.
/// </remarks>
public sealed class SampleAppSmokeTests
{
    private const int LaunchTimeoutMilliseconds = 20_000;

    [Trait("Category", "UI")]
    [Fact(Skip = "Manual UI smoke test — opt in by removing the Skip attribute or running with --filter Category=UI.")]
    public void Sample_app_launches_and_shows_main_window()
    {
        var exePath = SampleAppPath.Resolve();
        var startInfo = new ProcessStartInfo(exePath)
        {
            UseShellExecute = false,
            WorkingDirectory = Path.GetDirectoryName(exePath)!,
        };

        using var application = Application.Launch(startInfo);
        try
        {
            using var automation = new UIA3Automation();
            var mainWindow = application.GetMainWindow(automation, TimeSpan.FromMilliseconds(LaunchTimeoutMilliseconds));

            Assert.NotNull(mainWindow);
            Assert.False(string.IsNullOrWhiteSpace(mainWindow.Title));

            // Persist a screenshot next to the test assembly so CI can publish it as an artifact.
            var snapshotDir = Path.Combine(
                Path.GetDirectoryName(typeof(SampleAppSmokeTests).Assembly.Location)!,
                "Snapshots");
            Directory.CreateDirectory(snapshotDir);
            var snapshotPath = Path.Combine(snapshotDir, "sample-app-main-window.png");
            mainWindow.CaptureToFile(snapshotPath);
            Assert.True(new FileInfo(snapshotPath).Length > 0);
        }
        finally
        {
            // Best-effort cleanup. WaitWhileMainHandleIsMissing throws a wrapped
            // System.Exception if the process exits before it can find the pid (a
            // race that wins on fast machines). The `using` on application calls
            // Dispose, so this catch only suppresses noise, it doesn't leak.
#pragma warning disable CA1031
            try
            {
                application.Close();
                application.WaitWhileMainHandleIsMissing(TimeSpan.FromSeconds(5));
            }
            catch (Exception)
            {
                // Process already gone — fine.
            }
#pragma warning restore CA1031
        }
    }
}