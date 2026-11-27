# Atc.Wpf.Controls.Sample

Building blocks for a WPF "control explorer" application - the same pieces the `Atc.Wpf.Sample` demo app is built from.

## 🔍 Overview

`Atc.Wpf.Controls.Sample` lets you build a sample browser where each sample is shown with tabs for the live control, its XAML, its code-behind, its view model and its readme.

| Type | Purpose |
|------|---------|
| `SampleApplication` | Application base for a sample app: merges the Atc style dictionaries, sets culture and theme, traces binding errors and shows unhandled exceptions in the `ExceptionDialog`. |
| `SampleMainWindow` | `NiceWindow` that hosts a `SampleExplorer`; the app's `MainWindow.xaml` only lists its `SampleCategory` items. |
| `SampleExplorer` / `SampleCategory` | The whole shell: filter (text or PascalCase initials), category tabs with match counts, sample trees, expand/collapse, right-to-left, language and theme selectors, viewer and status bar. |
| `SampleTreeFilter` / `SampleExplorerIds` | The filter rules for sample trees, and the automation IDs UI tests can rely on. |
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

## 🚀 Build a sample gallery

```xml
<!-- App.xaml -->
<atcSample:SampleApplication
    x:Class="MyGallery.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:atcSample="clr-namespace:Atc.Wpf.Controls.Sample;assembly=Atc.Wpf.Controls.Sample"
    Culture="en-US"
    StartupUri="MainWindow.xaml"
    Theme="Dark.Blue" />

<!-- MainWindow.xaml -->
<atcSample:SampleMainWindow
    x:Class="MyGallery.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:atcSample="clr-namespace:Atc.Wpf.Controls.Sample;assembly=Atc.Wpf.Controls.Sample"
    xmlns:local="clr-namespace:MyGallery"
    Title="My control explorer">
    <atcSample:SampleCategory Key="Wpf" Header="Wpf" TreeType="{x:Type local:SamplesWpfTreeView}" />
</atcSample:SampleMainWindow>
```

See [SampleExplorer_Readme.md](SampleExplorer_Readme.md) for the sample trees and pages, the filter rules, the
automation IDs and all properties.

## 📝 Notes

- Readme lookup follows the conventions described in the repository `CLAUDE.md` / `README.md`: `docs/{section}/{ClassName}@Readme.md`, then `{ClassName}_Readme.md`, then the namespace folder's `@Readme.md`.
- The `Atc.Wpf.Sample` project in this repository (`sample/Atc.Wpf.Sample`) is the reference application built with these controls.

## 🎮 Sample Application

`sample/Atc.Wpf.Sample` - the whole application is a usage example.