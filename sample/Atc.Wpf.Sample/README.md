# Atc.Wpf.Sample

The `Atc.Wpf.Sample` application is the **control explorer** for the ATC.Net WPF stack. It hosts every demo page across the eight library categories, with a searchable tree, per-control readme tab, and a live preview tab.

## ▶️ Run

From the repo root:

```bash
dotnet run --project sample/Atc.Wpf.Sample
```

Or open `Atc.Wpf.sln` in Visual Studio / Rider and start `Atc.Wpf.Sample` (it is set as the default startup project).

> Requires **.NET 10 Desktop Runtime** on Windows 10 or later.

## 🗺️ What's in the explorer

The left panel hosts a `TabControl` (used as styled category buttons) plus stacked TreeViews. Selecting a category filters down to that section; typing in the search box filters across all categories at once.

| Category | TreeView | What you'll find |
|---|---|---|
| **Wpf** | `SamplesWpfTreeView.xaml` | Commands, Layouts, Media, Markup, Dialogs, Navigation |
| **Wpf.Controls** | `SamplesWpfControlsTreeView.xaml` | Base controls, Buttons, Color controls, Layouts, Progress |
| **Wpf.Forms** | `SamplesWpfFormsTreeView.xaml` | `Label*` controls, Selectors, Pickers |
| **Wpf.Components** | `SamplesWpfComponentsTreeView.xaml` | Dialogs, Viewers, Monitoring, Notifications |
| **Wpf.Network** | `SamplesWpfNetworkTreeView.xaml` | `NetworkScannerView` + settings |
| **Wpf.Theming** | `SamplesWpfThemingTreeView.xaml` | Themed standard WPF controls |
| **Wpf.SourceGenerators** | `SamplesWpfSourceGeneratorsTreeView.xaml` | Generator demos (`[ObservableProperty]`, `[RelayCommand]`, `[DependencyProperty]`, `[AttachedProperty]`) |
| **Wpf.FontIcons** | `SamplesWpfFontIconsTreeView.xaml` | Icon rendering (FontAwesome 7/5, Material, Octicons, …) |

The right pane shows the selected sample with three tabs:

- **Sample** — the live, themed control demo
- **Readme** — auto-discovered markdown next to the control source (`[Control]_Readme.md`, category-level `@Readme.md`, or folder-level `Readme.md`)
- **Code** — the XAML/C# source of the demo page itself

## 🔎 Search tips

- **Plain text** — case-insensitive substring match (e.g. `range` finds `RangeSlider`).
- **All-caps abbreviation (≥ 2 chars)** — matches against the uppercase letters in the item name. So `RS` matches both `RangeSlider` and `ReversibleStackPanel`.
- **Badges** — once you start typing, each tab header shows the per-category match count.

The full UX state-machine for search / tab / TreeView interaction is documented in [`docs/sample-app.md`](../../docs/sample-app.md).

## 🎨 Theme switching

The top-right toolbar exposes light/dark theme + accent color selectors. The selection is wired through `Atc.Wpf.Theming.ThemeManager` and applies live to every demo page.

## 🛠️ Contributing a new sample

1. Add a new view next to similar samples (e.g. `Samples/Wpf/Controls/MyControlSampleView.xaml`).
2. Add a TreeView item entry in the matching `Samples*TreeView.xaml`.
3. Optionally drop a `[MyControl]_Readme.md` next to the control's source so the **Readme** tab picks it up automatically.

Sample readme discovery is implemented in [`SampleViewerViewModel.LoadAndRenderMarkdownDocumentIfPossible`](../../src/Atc.Wpf.Controls.Sample/SampleViewerViewModel.cs); see the **Control Readme Conventions** section in [`CLAUDE.md`](../../CLAUDE.md) for the fallback chain.
