# Atc.Wpf.Controls.Sample

Building blocks for a WPF "control explorer" application - the same pieces the `Atc.Wpf.Sample` demo app is built from.

## 🔍 Overview

`Atc.Wpf.Controls.Sample` lets you build a sample browser where each sample is shown with tabs for the live control, its XAML, its code-behind, its view model and its readme.

| Type | Purpose |
|------|---------|
| `SampleViewerView` / `SampleViewerViewModel` | The tabbed viewer (Sample / XAML / CodeBehind / ViewModel / Readme). Loads the source files of the selected sample and renders the matching `*_Readme.md` / `@Readme.md` as markdown. |
| `SampleTreeViewItem` | `TreeViewItem` with a `SamplePath` property that identifies the sample view to show. |
| `SampleItemMessage` | Messenger message (`Header`, `SampleItemPath`) sent when a sample is selected. |
| `SampleSidePanel` | Side panel hosting `ActionsContent` and editors for a `SourceObject`. |
| `SamplePropertyController` | Generates editors for the public properties of its `SourceObject`. |
| `SampleDataController` | Add / remove / reset / populate buttons driven by an `ISampleDataGenerator`. |
| `ISampleDataGenerator` / `SampleDataGeneratorAttribute` | Contract for sample-data generators, and the attribute that links a demo view model to its generator. |

## 📦 Installation

```xml
<PackageReference Include="Atc.Wpf.Controls.Sample" Version="4.*" />
```

## 📝 Notes

- Readme lookup follows the conventions described in the repository `CLAUDE.md` / `README.md`: `docs/{section}/{ClassName}@Readme.md`, then `{ClassName}_Readme.md`, then the namespace folder's `@Readme.md`.
- The `Atc.Wpf.Sample` project in this repository (`sample/Atc.Wpf.Sample`) is the reference application built with these controls.

## 🎮 Sample Application

`sample/Atc.Wpf.Sample` - the whole application is a usage example.