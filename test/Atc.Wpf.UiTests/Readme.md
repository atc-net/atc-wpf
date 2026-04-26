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

        // 1. Drive the UI: click a TreeView item, fill a field, etc.
        // 2. Capture: var img = window.Capture();
        // 3. Persist + (optionally) diff against a baseline under Snapshots/baseline/.
    }
    finally
    {
        app.Close();
    }
}
```

## Why FlaUI and not Verify.Wpf or PercyIO

- **FlaUI** drives the real automation tree (UIA3), so it can interact with controls — not just snapshot static visuals. That makes it a better fit for an enterprise control library where many regressions are interaction-related.
- **Verify.Wpf** is text/markup snapshot oriented; complementary, can be added later if useful.
- **PercyIO** is cloud-hosted and adds a third-party dependency; deferred.

## Status

This is a **pilot** — one smoke test on the sample app's main window, gated behind a manual opt-in. Future expansion: per-view screenshots driven by the existing TreeView item paths, image-diff baselines under `Snapshots/baseline/`, optional Verify.Wpf integration for non-visual XAML structure assertions.