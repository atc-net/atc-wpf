# 🧭 SampleExplorer

A ready-made shell for a WPF control-explorer ("sample gallery") app: filter, category tabs, sample trees and
the sample viewer, plus a main window and an application base that set up everything around it.

## 🔍 Overview

A sample gallery needs the same things every time: a list of samples grouped by category, a filter, a viewer
that shows the selected sample with its XAML, code and readme, a theme and language picker, and start-up code
that merges the styles, sets the culture and theme, traces binding errors and shows unhandled exceptions.
`Atc.Wpf.Controls.Sample` ships all of it, so a gallery is an `App.xaml`, a short `MainWindow.xaml`, its sample
trees and its sample pages.

| Type | Purpose |
|------|---------|
| `SampleApplication` | Application base: style dictionaries, culture, theme, binding-error tracing, `ExceptionDialog` for unhandled exceptions |
| `SampleMainWindow` | `NiceWindow` that shows a `SampleExplorer`; list the categories in it |
| `SampleExplorer` | The shell itself, for use in your own window |
| `SampleCategory` | One tab and one sample tree: `Key`, `Header`, `TreeType` |
| `SampleTreeFilter` | The filter rules (text or PascalCase initials) for sample trees |
| `SampleExplorerIds` | The automation IDs UI tests can rely on |

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Sample;
```

```xml
xmlns:atcSample="clr-namespace:Atc.Wpf.Controls.Sample;assembly=Atc.Wpf.Controls.Sample"
```

## 🚀 Usage

### A minimal gallery

`App.xaml` derives from `SampleApplication`. The Atc style dictionaries (Theming `Default.xaml`, Controls and
Components `Controls.xaml`) are merged for you; merge only what the app needs on top (here Fluent.Ribbon).

```xml
<atcSample:SampleApplication
    x:Class="MyGallery.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:atcSample="clr-namespace:Atc.Wpf.Controls.Sample;assembly=Atc.Wpf.Controls.Sample"
    Culture="en-US"
    StartupUri="MainWindow.xaml"
    Theme="Dark.Blue"
    TraceBindingErrorsVariable="MY_GALLERY_TRACE_BINDING_ERRORS">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="/Fluent;Component/Themes/Generic.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</atcSample:SampleApplication>
```

`App.xaml.cs` only holds the app's own start-up work, if any:

```csharp
namespace MyGallery;

public partial class App
{
    protected override void OnSampleStartup(StartupEventArgs e)
    {
        // Runs after the styles, culture and theme are set, before MainWindow opens.
        _ = MyResources.Word.Loaded;
    }
}
```

`MainWindow.xaml` lists the categories. Each `TreeType` is a `TreeView` of the app; the explorer creates it.

```xml
<atcSample:SampleMainWindow
    x:Class="MyGallery.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:atcSample="clr-namespace:Atc.Wpf.Controls.Sample;assembly=Atc.Wpf.Controls.Sample"
    xmlns:local="clr-namespace:MyGallery"
    Title="My control explorer"
    StatusText="Every page runs offline on sample data.">
    <atcSample:SampleCategory Key="Wpf" Header="Wpf" TreeType="{x:Type local:SamplesWpfTreeView}" />
    <atcSample:SampleCategory Key="WpfUI" Header="Wpf.UI" TreeType="{x:Type local:SamplesWpfUITreeView}" />
</atcSample:SampleMainWindow>
```

```csharp
namespace MyGallery;

public partial class MainWindow
{
    public MainWindow()
        => InitializeComponent();
}
```

A sample tree groups the samples. `SamplePath` is the end of the sample page's full type name:

```xml
<TreeView
    x:Class="MyGallery.SamplesWpfUITreeView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:atcSample="clr-namespace:Atc.Wpf.Controls.Sample;assembly=Atc.Wpf.Controls.Sample">
    <TreeViewItem Header="Designer">
        <atcSample:SampleTreeViewItem Header="DesignerCanvas" SamplePath="Designer.DesignerCanvasSampleView" />
    </TreeViewItem>
</TreeView>
```

A sample page is a plain `UserControl`. The viewer shows its XAML, code-behind, view model (`DataContext`
type name + `.cs`) and the nearest `*_Readme.md` next to it.

```xml
<UserControl
    x:Class="MyGallery.SamplesWpfUI.Designer.DesignerCanvasSampleView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <GroupBox Header="DesignerCanvas">
        <!-- the control being shown -->
    </GroupBox>
</UserControl>
```

### The explorer in your own window

Use `SampleExplorer` directly when the window needs more than the explorer:

```xml
<atc:NiceWindow
    xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
    xmlns:atcSample="clr-namespace:Atc.Wpf.Controls.Sample;assembly=Atc.Wpf.Controls.Sample"
    xmlns:local="clr-namespace:MyGallery">
    <atcSample:SampleExplorer StatusText="Ready">
        <atcSample:SampleCategory Key="Wpf" Header="Wpf" TreeType="{x:Type local:SamplesWpfTreeView}" />
    </atcSample:SampleExplorer>
</atc:NiceWindow>
```

### Your own start-up (dependency injection, settings)

`SampleApplication` has no host of its own. An app with one keeps it and leaves `Culture`, `Theme` and
`StartupUri` unset:

```csharp
public partial class App
{
    private readonly IHost host = Host.CreateDefaultBuilder()
        .ConfigureServices(services => services.AddSingleton<MainWindow>())
        .Build();

    private async void ApplicationStartup(object sender, StartupEventArgs e)
    {
        await host.StartAsync();
        CultureManager.SetCultures("da-DK");
        ThemeManagerHelper.SetThemeAndAccent(Current, "Light.Steel");
        host.Services.GetRequiredService<MainWindow>().Show();
    }
}
```

## 🔎 Filter rules

| Filter | Matches | Example |
|--------|---------|---------|
| Text | Header, `Tag` or `SamplePath` containing it, ignoring case | `button` → `ImageButton`, `AuthenticationButton` |
| 2+ capitals only | The capitals of the text | `NW` → `NiceWindow`; `TS` → `ToggleSwitch`, `LabelToggleSwitch` |

Typing in the filter shows every category again, hides what doesn't match, expands the parents of what does and
shows the number of matching samples in each tab's badge. Clicking a tab shows only its tree; selecting a sample
selects its tab. The same rules are available as `SampleTreeFilter.Matches`, `ApplyFilter` and `CountMatches`.

## 🤖 Automation IDs

UI tests (e.g. FlaUI) can rely on these; `SampleExplorerIds` has them as constants.

| Element | Automation ID |
|---------|---------------|
| Filter text box | `TbSampleFilter` |
| Tab control | `SamplesTabControl` |
| Category tab | `Tab{Key}`, e.g. `TabWpfUI` |
| Sample tree | `StvSamples{Key}`, e.g. `StvSamplesWpfUI` |
| Right-to-left check box | `SampleRightToLeft` |

Unhandled exceptions are shown in windows titled `Dispatcher Unhandled Exception` (an `ExceptionDialog`) and
`CurrentDomain Unhandled Exception` (a native message box).

## ⚙️ Properties

### SampleApplication

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Culture` | `string?` | `null` | Culture set at start-up (`CultureManager.SetCultures`); `null` leaves it |
| `Theme` | `string?` | `null` | Theme and accent set at start-up, e.g. `Dark.Blue`; `null` leaves it |
| `TraceBindingErrorsVariable` | `string?` | `null` | Environment variable that, set to `1`, traces binding errors without a debugger |
| `ShutdownOnUnhandledException` | `bool` | `false` | Close the app after showing an unhandled UI-thread exception |

Override `OnSampleStartup(StartupEventArgs)` for the app's own start-up work.

### SampleMainWindow

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Categories` | `ObservableCollection<SampleCategory>` | empty | The categories (content property) |
| `StatusText` | `string?` | `null` | Text in the status bar |
| `Explorer` | `SampleExplorer` | | The hosted explorer |

Size 1600 x 1000 (at least 1024 x 768), centred; the DataContext is a `MainWindowViewModelBase`.

### SampleExplorer

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Categories` | `ObservableCollection<SampleCategory>` | empty | The categories (content property), in tab order |
| `StatusText` | `string?` | `null` | Text in the status bar |
| `SampleTreeViews` | `IReadOnlyList<TreeView>` | | The created trees, in category order (after load) |
| `Viewer` | `SampleViewerView` | | The sample viewer |

### SampleCategory

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Key` | `string` | `""` | Unique key; names the automation IDs |
| `Header` | `string` | `""` | Tab text |
| `TreeType` | `Type?` | `null` | A `TreeView` subclass with a public parameterless constructor |

## 📝 Notes

- A missing or duplicate `Key`, or a `TreeType` that is not a `TreeView` with a public parameterless
  constructor, throws an `InvalidOperationException` when the explorer loads.
- The bold category style of the trees is scoped to the tree panel; it doesn't reach the sample pages.
- Trees get the Atc `TreeView` style in code, so the app needs no `Style` per tree class.

## 🔗 Related Controls

- `SampleViewerView` - the tabbed viewer (Sample / XAML / CodeBehind / ViewModel / Readme)
- `SampleTreeViewItem` - a tree item that points to a sample by `SamplePath`
- `NiceWindow` - the window `SampleMainWindow` derives from
- `ExceptionDialog` - the dialog for unhandled exceptions

## 🎮 Sample Application

`sample/Atc.Wpf.Sample` is built with it: `App.xaml` (a `SampleApplication` with its own host) and
`MainWindow.xaml` (a `SampleMainWindow` with eleven categories).