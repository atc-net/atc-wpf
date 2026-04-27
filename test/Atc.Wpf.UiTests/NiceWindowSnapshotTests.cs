namespace Atc.Wpf.UiTests;

/// <summary>
/// Visual snapshot pilot for <c>NiceWindow</c> — the custom themed chrome
/// shipped from <c>Atc.Wpf.Theming</c>. The sample app's <c>MainWindow</c>
/// already inherits from <c>NiceWindow</c>, so launching the sample exercises
/// the full chrome (caption bar, system buttons, theming colors, resize grip).
/// </summary>
/// <remarks>
/// <para>
/// What this test guards against:
/// </para>
/// <list type="bullet">
///   <item><description>The custom chrome silently falling back to a plain
///   <c>Window</c> (asserted via the FlaUI <c>AutomationElement.ClassName</c>
///   property == <c>"NiceWindow"</c>, which the <c>NiceWindowAutomationPeer</c>
///   pins).</description></item>
///   <item><description>Theming regressions in the title bar — captured as a
///   separate sub-rectangle PNG so a future image diff harness can compare
///   just the high-risk caption strip without noise from the content area.</description></item>
///   <item><description>Catastrophic chrome regressions (zero-size window,
///   crash on launch).</description></item>
/// </list>
/// <para>
/// Like the rest of the FlaUI pilot, this test is opt-in
/// (<c>[Fact(Skip = ...)]</c>) so it never runs on headless CI by accident.
/// Snapshots land under <c>bin/&lt;Config&gt;/net10.0-windows/Snapshots/</c>.
/// </para>
/// </remarks>
public sealed class NiceWindowSnapshotTests
{
    private const int LaunchTimeoutMilliseconds = 20_000;

    /// <summary>
    /// Height (in DIPs) of the strip captured as the "title bar" snapshot.
    /// NiceWindow's caption is 30–34px depending on system DPI; 40 gives a
    /// little headroom without bleeding into the content area.
    /// </summary>
    private const int TitleBarStripHeight = 40;

    [Trait("Category", "UI")]
    [Fact(Skip = "Manual UI snapshot test — opt in by removing the Skip attribute or running with --filter Category=UI.")]
    public void NiceWindow_chrome_is_active_and_titlebar_strip_can_be_snapshotted()
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

            // The custom NiceWindowAutomationPeer pins ClassName to "NiceWindow".
            // If this regresses to "Window" the themed chrome is no longer in play.
            Assert.Equal("NiceWindow", mainWindow.ClassName);

            // Bring the sample window to the foreground before capturing. FlaUI's
            // capture path clips to the element's bounding rectangle but still uses
            // the screen as the backing store, so any window above the sample (the
            // test runner's terminal, an IDE, …) would otherwise bleed into the PNG.
            //
            // SetForeground() alone isn't reliable: Windows blocks SetForegroundWindow
            // from non-foreground processes (the "stealing focus" rule). Maximizing via
            // the Window pattern force-activates the window and guarantees it covers the
            // whole work area, which also gives us a deterministic snapshot size across
            // machines / DPIs.
            mainWindow.Patterns.Window.Pattern.SetWindowVisualState(FlaUI.Core.Definitions.WindowVisualState.Maximized);
            mainWindow.SetForeground();

            // Tiny wait so the maximize animation + foreground transition + WPF render
            // pass settle before we screenshot. Synchronous wait by design — this is
            // a UI test, not a unit test, so blocking the test thread briefly is correct.
#pragma warning disable S2925
            System.Threading.Thread.Sleep(500);
#pragma warning restore S2925

            var bounds = mainWindow.BoundingRectangle;
            Assert.True(bounds.Width > 0 && bounds.Height > 0, "Window bounds should be non-zero.");

            var snapshotDir = Path.Combine(
                Path.GetDirectoryName(typeof(NiceWindowSnapshotTests).Assembly.Location)!,
                "Snapshots");
            Directory.CreateDirectory(snapshotDir);

            var fullSnapshot = Path.Combine(snapshotDir, "nice-window-full.png");
            var titleBarSnapshot = Path.Combine(snapshotDir, "nice-window-titlebar.png");

            // Use element-based capture instead of Capture.Rectangle(absoluteScreenRect):
            // the latter screenshots the screen at those coords, which depends on Z-order,
            // so anything floating above the sample app would end up in the snapshot. The
            // element-based capture pulls pixels for just this window regardless of who's
            // on top, then we crop in pixel coordinates to isolate the caption strip.
            using (var fullBitmap = mainWindow.Capture())
            {
                fullBitmap.Save(fullSnapshot, System.Drawing.Imaging.ImageFormat.Png);

                var stripHeight = System.Math.Min(TitleBarStripHeight, fullBitmap.Height);
                var cropRect = new System.Drawing.Rectangle(0, 0, fullBitmap.Width, stripHeight);
                using var titleBarBitmap = fullBitmap.Clone(cropRect, fullBitmap.PixelFormat);
                titleBarBitmap.Save(titleBarSnapshot, System.Drawing.Imaging.ImageFormat.Png);
            }

            Assert.True(new FileInfo(fullSnapshot).Length > 0, "Full window snapshot should be non-empty.");
            Assert.True(new FileInfo(titleBarSnapshot).Length > 0, "Title bar snapshot should be non-empty.");
        }
        finally
        {
            // Best-effort teardown. WaitWhileMainHandleIsMissing throws if the process has
            // already exited (which can win the race after Close()), so swallow it — the
            // `using` on application.Dispose() will finish the cleanup either way.
            // Best-effort test cleanup. FlaUI's Retry wraps the underlying
            // ArgumentException ("Process with id X is not running") in a generic
            // System.Exception, so a narrow catch isn't possible. The `using` on
            // application also calls Dispose, so we're not leaking anything.
#pragma warning disable CA1031
            try
            {
                application.Close();
                application.WaitWhileMainHandleIsMissing(TimeSpan.FromSeconds(5));
            }
            catch (Exception)
            {
                // Process already gone, or FlaUI lost track of it — fine.
            }
#pragma warning restore CA1031
        }
    }
}