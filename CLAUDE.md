# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ATC.Net WPF is an enterprise-ready WPF control library framework providing MVVM infrastructure, theming, 150+ controls, and source generators. The project follows a four-tier architecture:

- **Atc.Wpf** - Core library (MVVM, layouts, value converters, helpers)
- **Atc.Wpf.Controls** - Atomic controls library (14 base controls, buttons, color controls)
- **Atc.Wpf.Forms** - Form field controls (25+ labeled controls with validation)
- **Atc.Wpf.Components** - Composite components (dialogs, viewers, settings)
- **Atc.Wpf.FontIcons** - Font-based icon rendering
- **Atc.Wpf.Theming** - Light/Dark theme infrastructure
- **Atc.Wpf.Network** - Network scanning and discovery controls
- **Atc.Wpf.Controls.Sample** - Controls for building sample applications

## Build Commands

```bash
# Build solution
dotnet build

# Build release (treats warnings as errors)
dotnet build -c Release

# Run all tests
dotnet test

# Run specific test project
dotnet test test/Atc.Wpf.Tests
dotnet test test/Atc.Wpf.Controls.Tests

# Run single test by name
dotnet test --filter "FullyQualifiedName~TestMethodName"

# Run sample application
dotnet run --project sample/Atc.Wpf.Sample
```

## Architecture

### Source Generators and MVVM (from Atc.XamlToolkit)

The project uses C# source generators and MVVM infrastructure from the [Atc.XamlToolkit](https://github.com/atc-net/atc-xaml-toolkit) packages (included as dependencies of Atc.Wpf):

**Source Generator Attributes:**
- `[ObservableProperty]` - Generates properties with INotifyPropertyChanged from private fields
- `[RelayCommand]` - Generates ICommand properties from methods (supports async and CanExecute)
- `[DependencyProperty]` - Generates WPF dependency properties
- `[AttachedProperty]` - Generates WPF attached properties

**MVVM Base Classes:**
- `ViewModelBase`, `MainWindowViewModelBase`, `ViewModelDialogBase` - from `Atc.XamlToolkit.Mvvm`
- `RelayCommand<T>`, `RelayCommandAsync<T>` - from `Atc.XamlToolkit.Command`
- `ObservableObject` - from `Atc.XamlToolkit.Mvvm`

Source generators are provided by the `Atc.XamlToolkit` and `Atc.XamlToolkit.Wpf` NuGet packages.

### Control Architecture (Four-Tier)

**Base Controls** (`src/Atc.Wpf.Controls/BaseControls/`): Atomic/primitive controls - single-purpose building blocks like NumericBox, IntegerBox, ToggleSwitch.

**Form Controls** (`src/Atc.Wpf.Forms/`): Labeled form field controls with validation display and mandatory indicators. Use for building data entry forms. Examples: LabelTextBox, LabelComboBox, LabelDatePicker.

**Composite Components** (`src/Atc.Wpf.Components/`): Higher-level components combining multiple controls - dialogs (InfoDialogBox, QuestionDialogBox, InputDialogBox, InputFormDialogBox, BasicApplicationSettingsDialogBox), viewers (JsonViewer, TerminalViewer), monitoring (ApplicationMonitorView), notifications (ToastNotification system), and settings panels.

### MVVM Base Classes

The MVVM base classes come from the `Atc.XamlToolkit` package (included as a dependency):
- `ViewModelBase` - Standard ViewModel base (from `Atc.XamlToolkit.Mvvm`)
- `MainWindowViewModelBase` - For main window ViewModels (from `Atc.XamlToolkit.Mvvm`)
- `ViewModelDialogBase` - For dialog ViewModels (from `Atc.XamlToolkit.Mvvm`)
- `RelayCommand<T>` / `RelayCommandAsync<T>` - Command implementations (from `Atc.XamlToolkit.Command`)

### Project Structure

```
src/
├── Atc.Wpf/                      # Core: MVVM, layouts, converters
├── Atc.Wpf.Controls/             # Atomic controls (base, buttons, colors)
├── Atc.Wpf.Forms/                # Form field controls (Label*)
├── Atc.Wpf.Components/           # Composite components (dialogs, viewers)
├── Atc.Wpf.Controls.Sample/      # Sample app controls
├── Atc.Wpf.FontIcons/            # Font icon support
├── Atc.Wpf.Theming/              # Theme infrastructure
└── Atc.Wpf.Network/              # Network scanning controls
test/                             # XUnit test projects
sample/Atc.Wpf.Sample/            # Demo application
```

## Code Style

- Target: .NET 10.0, C# 14.0
- Nullable reference types enabled
- StyleCop, Meziantou, SonarAnalyzer enforced
- Release builds treat warnings as errors

## Testing

- Framework: XUnit 3 with Microsoft Testing Platform
- WPF-specific testing: Xunit.StaFact (for STA thread tests)
- Test utilities: Atc.XUnit, AutoFixture, FluentAssertions, NSubstitute
- Global usings for test projects include: AutoFixture, FluentAssertions, NSubstitute, Xunit

### Test Projects Structure

| Test Project | Focus Area | Coverage |
|--------------|------------|----------|
| `Atc.Wpf.Tests` | Core library (Helpers, ValueConverters, Serialization) | 42 test files |
| `Atc.Wpf.Controls.Tests` | Controls library (code compliance) | 1 test file |
| `Atc.Wpf.Forms.Tests` | Form controls (Extractors, Factories, Helpers) | 8 test files |
| `Atc.Wpf.Network.Tests` | Network ViewModels | 5 test files |
| `Atc.Wpf.Theming.Tests` | Theme infrastructure (code compliance) | 1 test file |

### Running Tests

```bash
# Run all tests (719 tests)
dotnet test

# Run specific test project
dotnet test test/Atc.Wpf.Tests
dotnet test test/Atc.Wpf.Forms.Tests

# Run single test by name
dotnet test --filter "FullyQualifiedName~TestMethodName"
```

## Sample Application

The sample app (`sample/Atc.Wpf.Sample/`) serves as a control explorer with 8 main categories:

| Category | TreeView | Samples |
|----------|----------|---------|
| Wpf | `SamplesWpfTreeView.xaml` | Commands, Layouts, Media, Markup, etc. |
| Wpf.Controls | `SamplesWpfControlsTreeView.xaml` | Base controls, Buttons, Colors, Layouts |
| Wpf.Forms | `SamplesWpfFormsTreeView.xaml` | Label controls, Selectors, Pickers |
| Wpf.Components | `SamplesWpfComponentsTreeView.xaml` | Dialogs, Viewers (to be populated) |
| Wpf.Network | `SamplesWpfNetworkTreeView.xaml` | NetworkScanner |
| Wpf.Theming | `SamplesWpfThemingTreeView.xaml` | Themed standard WPF controls |
| Wpf.SourceGenerators | `SamplesWpfSourceGeneratorsTreeView.xaml` | Generator demos |
| Wpf.FontIcons | `SamplesWpfFontIconsTreeView.xaml` | Icon rendering |

## Control Readme Conventions

Each control can have a `_Readme.md` file that is displayed in the sample app's **Readme** tab. Three naming conventions exist:

| Convention | Placement | Scope | Example |
|---|---|---|---|
| `[Control]_Readme.md` | Next to the control `.cs` file | Single control | `Badge_Readme.md` |
| `@Readme.md` | In a category folder under `docs/` or `src/` | All controls in that namespace | `docs/Theming/@Readme.md` |
| `Readme.md` | In a library/folder root | Multiple controls in that folder | `src/Atc.Wpf.Forms/Readme.md` |

### Sample App Readme Discovery

The sample app auto-discovers readme files via `SampleViewerViewModel.LoadAndRenderMarkdownDocumentIfPossible()` in `src/Atc.Wpf.Controls.Sample/SampleViewerViewModel.cs`. It pre-caches all `*.md` files from the solution root and uses case-insensitive `EndsWith()` matching with this fallback chain:

1. `docs/{section}/{className}@Readme.md` — section-based lookup
2. `{className}_Readme.md` — control-specific readme next to source
3. `docs/{namespace_folder}/@Readme.md` — namespace-based category fallback (also tries singular form)
4. `{classViewName}_Readme.md` — view class name fallback

### When Creating a New Control

Place a `[ControlName]_Readme.md` file next to the control's `.cs` file. Follow the template pattern from `GridEx_Readme.md`: emoji title, Overview, Namespace, Usage examples, Properties table (Property/Type/Default/Description), Notes, Related Controls, Sample Application path.

### Important

- The `@Readme.md` and `_Readme.md` naming patterns are **intentional** for sample-app readme pickup — do NOT rename them
- `@` prefix files serve as category-level documentation shared across a namespace
- Controls without their own `_Readme.md` fall back to category or folder-level readmes

## Key Documentation

- MVVM guide: `docs/Mvvm/@Readme.md`
- Source generators: `docs/SourceGenerators/ViewModel.md`
- Restructuring plan: `docs/RESTRUCTURING_PLAN.md`
- Theming controls: `docs/Theming/@Readme.md`
- Font icons: `docs/FontIcons/@Readme.md`
- Layout controls: `docs/Layouts/@Readme.md`
- Data display controls: `docs/DataDisplay/@Readme.md`
- Form controls: `src/Atc.Wpf.Forms/` (LabelTextBox, LabelComboBox, etc.)
- Base controls: `src/Atc.Wpf.Controls/BaseControls/` (NumericBox, IntegerBox, etc.)
- Composite components: `src/Atc.Wpf.Components/` (Dialogs, Viewers, Monitoring, Notifications)
