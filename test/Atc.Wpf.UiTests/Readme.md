# Atc.Wpf.UiTests

Pilot UI / visual-regression test project, built on **[FlaUI](https://github.com/FlaUI/FlaUI)** (`FlaUI.Core` + `FlaUI.UIA3`). FlaUI drives the sample app via Windows UI Automation, captures screenshots, and lets later tests diff them against baselines.

## Why the tests are skipped by default

This project targets Windows-only and launches a real WPF process. Headless / Linux CI cannot execute it. The tests carry `[Trait("Category", "UI")]` and the smoke test is currently `[Fact(Skip = "...")]` — opt in by removing the skip locally, or wire CI to opt in via filter:

```bash
dotnet run --project test/Atc.Wpf.UiTests -c Release \
    -- --filter-trait "Category=UI"
```

## Running locally

1. Build the sample app first so the executable exists:
   ```bash
   dotnet build sample/Atc.Wpf.Sample -c Release
   ```
2. Open `SampleAppSmokeTests.cs`, remove the `Skip = "..."` from the `[Fact]`, and run:
   ```bash
   dotnet run --project test/Atc.Wpf.UiTests -c Release --no-build
   ```
3. Captured screenshots are written to `bin/<Config>/net10.0-windows/Snapshots/`.

## Tests

| Test | What it pins |
|---|---|
| `SampleAppSmokeTests.Sample_app_launches_and_shows_main_window` | App launches, main window has a non-empty title, full window screenshot can be captured. |
| `NiceWindowSnapshotTests.NiceWindow_chrome_is_active_and_titlebar_strip_can_be_snapshotted` | Custom `NiceWindow` chrome is in play (asserts `ClassName == "NiceWindow"` via `NiceWindowAutomationPeer`). Captures full window + cropped title bar strip — the latter isolates the highest-risk theming surface (caption colors, system buttons, title text) for low-noise image diffs. |

## Pattern for new tests

```csharp
[Trait("Category", "UI")]
[Fact(Skip = "Manual UI test")]
public void Some_view_renders_without_errors()
{
    var exePath = SampleAppPath.Resolve();
    using var app = Application.Launch(new ProcessStartInfo(exePath));
    try
    {
        using var automation = new UIA3Automation();
        var window = app.GetMainWindow(automation, TimeSpan.FromSeconds(20));

        // Bring window forward and let it settle BEFORE capturing — see Gotchas.
        window.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
        window.SetForeground();
        Thread.Sleep(500);

        // 1. Drive the UI: click a TreeView item, fill a field, etc.
        // 2. Capture: var bitmap = window.Capture();
        // 3. Persist + (optionally) diff against a baseline under Snapshots/baseline/.
    }
    finally
    {
        try { app.Close(); app.WaitWhileMainHandleIsMissing(TimeSpan.FromSeconds(5)); }
        catch (Exception) { /* process already gone */ }
    }
}
```

## Gotchas (learned the hard way)

- **`mainWindow.Capture()` uses the screen as backing store.** The doc says "Captures the object as screenshot" but it actually clips to the element's bounding rectangle while pulling pixels from the screen. If another window is on top of the sample, you'll capture *that*. Always force the sample to foreground first; maximizing via the Window pattern is the most reliable way (`SetForegroundWindow` alone is blocked by Windows for non-foreground processes).
- **`Capture.Rectangle(absoluteScreenRect)` has the same problem, worse.** Use `mainWindow.Capture()` then crop the resulting `Bitmap` in pixel coordinates.
- **`application.WaitWhileMainHandleIsMissing(...)` races with `application.Close()`.** On fast machines the process exits before WaitWhile can find it by pid; FlaUI then throws a wrapped `System.Exception`. Wrap teardown in a broad try/catch — it's already best-effort and the `using` on `application` calls Dispose anyway.
- **Don't call `Bitmap.Clone(rect, format)` without an explicit cast** — overload resolution inside this project picks an unrelated generic `Clone<T>` extension first; the build error is misleading. (As of this commit the cast isn't needed because the right overload wins, but if the build breaks again on a `Clone` line, that's your suspect.)

## Why FlaUI and not Verify.Wpf or PercyIO

- **FlaUI** drives the real automation tree (UIA3), so it can interact with controls — not just snapshot static visuals. That makes it a better fit for an enterprise control library where many regressions are interaction-related.
- **Verify.Wpf** is text/markup snapshot oriented; complementary, can be added later if useful.
- **PercyIO** is cloud-hosted and adds a third-party dependency; deferred.

## Status

This is a **pilot** — two opt-in tests covering app launch and `NiceWindow` chrome capture. Future expansion: per-view screenshots driven by the existing TreeView item paths, image-diff baselines under `Snapshots/baseline/`, theme-toggle snapshots (Light + Dark), optional Verify.Wpf integration for non-visual XAML structure assertions.