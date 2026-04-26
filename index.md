# Atc.Wpf

> A comprehensive set of modern, enterprise-ready WPF libraries for building professional desktop applications with the MVVM design pattern.

`Atc.Wpf` is a four-tier WPF control library: **Atc.Wpf** (core) → **Atc.Wpf.Controls** (atomic controls) → **Atc.Wpf.Forms** (labelled form fields) → **Atc.Wpf.Components** (composite components). Plus dedicated packages for theming, font icons, network scanning, and undo/redo.

## Get started

- 📦 [Install from NuGet](#installation)
- 🚀 [Quick start guide](https://github.com/atc-net/atc-wpf#-quick-start-guide)
- 🎮 Run the sample app: `dotnet run --project sample/Atc.Wpf.Sample`

## Documentation

| Section | What's inside |
|---|---|
| [📐 Layouts](docs/Layouts/@Readme.md) | `GridEx`, `AutoGrid`, `FlexPanel`, `StaggeredPanel`, `UniformSpacingPanel`, `VirtualizingStaggeredPanel` |
| [🧱 MVVM](docs/Mvvm/@Readme.md) | `ViewModelBase`, `RelayCommand`, source generators via `Atc.XamlToolkit` |
| [🪄 Source generators](docs/SourceGenerators/ViewModel.md) | `[ObservableProperty]`, `[RelayCommand]`, `[DependencyProperty]`, `[AttachedProperty]` |
| [🎨 Theming](docs/Theming/@Readme.md) | Light / Dark + accent color palette, `NiceWindow`, themed standard controls |
| [🔤 Font icons](docs/FontIcons/@Readme.md) | FontAwesome 5/7, Material, Octicons, Weather, IcoFont, Bootstrap |
| [📝 Forms](docs/Forms/@Readme.md) | 25+ labelled form field controls with deferred validation |
| [🧩 Components](docs/Components/@Readme.md) | Dialogs, viewers, monitoring, notifications, settings panels, zoom browser |
| [📊 Data display](docs/DataDisplay/@Readme.md) | `Avatar`, `Badge`, `Breadcrumb`, `Card`, `Carousel`, `Chip`, `Divider`, `Popover`, `Segmented`, `Timeline` |
| [⌨️ Hotkeys](docs/Hotkeys/@Readme.md) | `HotkeyManager`, `KeyboardHook` |
| [🧭 Navigation](docs/Navigation/@Readme.md) | Frame / page navigation primitives |
| [🎈 Sample app](docs/sample-app.md) | Search / TreeView / TabControl interaction model |

The full **API reference** (auto-generated from XML docs) is in the **API Reference** tab in the top nav.

## Installation

```xml
<ItemGroup>
  <PackageReference Include="Atc.Wpf" Version="2.*" />
  <PackageReference Include="Atc.Wpf.Theming" Version="2.*" />
  <PackageReference Include="Atc.Wpf.Forms" Version="2.*" />
  <PackageReference Include="Atc.Wpf.Components" Version="2.*" />
  <PackageReference Include="Atc.Wpf.FontIcons" Version="2.*" />
</ItemGroup>
```

## Building this site locally

This site is generated with [DocFX](https://dotnet.github.io/docfx/).

```bash
dotnet tool install -g docfx
docfx docfx.json --serve
```

Then open <http://localhost:8080>.

> **If you see `IOException: ... _site\toc.html ... being used by another process`** — that's a previous `docfx --serve` still holding files. Stop it (close the previous terminal or `Ctrl+C`), delete the `_site/` directory, and re-run.
>
> **If you see `FailedToLoadAnalyzer: Atc.XamlToolkit.SourceGenerators ... ReferencesNewerCompiler`** — that's expected. DocFX's bundled Roslyn is older than what the source generators target, so generated members (`[ObservableProperty]`, `[DependencyProperty]`, etc.) are missing from API metadata. The site still builds because `docfx.json` sets `allowCompilationErrors: true`. Will resolve when DocFX upgrades its bundled Roslyn.

## Contributing

See [CONTRIBUTING.md](https://github.com/atc-net/atc-wpf/blob/main/CONTRIBUTING.md), [CODE_OF_CONDUCT.md](https://github.com/atc-net/atc-wpf/blob/main/CODE_OF_CONDUCT.md), [SECURITY.md](https://github.com/atc-net/atc-wpf/blob/main/SECURITY.md).
