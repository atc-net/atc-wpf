# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ATC.Net WPF is an enterprise-ready WPF control library framework providing MVVM infrastructure, theming, 40+ controls, and source generators. The project consists of six NuGet packages:

- **Atc.Wpf** - Core library (base controls, MVVM, layouts, value converters)
- **Atc.Wpf.Controls** - Rich control library (25+ labeled controls, 14 base controls)
- **Atc.Wpf.FontIcons** - Font-based icon rendering
- **Atc.Wpf.Theming** - Light/Dark theme infrastructure
- **Atc.Wpf.NetworkControls** - Network scanning and discovery controls (NetworkScannerView)
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

### Source Generators (Key Pattern)

The project uses C# source generators extensively for MVVM boilerplate reduction:

- `[ObservableProperty]` - Generates properties with INotifyPropertyChanged from private fields
- `[RelayCommand]` - Generates ICommand properties from methods (supports async and CanExecute)
- `[DependencyProperty]` - Generates WPF dependency properties
- `[AttachedProperty]` - Generates WPF attached properties

Source generator implementation: `src/Atc.Wpf.SourceGenerators/`

### Control Architecture

**Label Controls** (`src/Atc.Wpf.Controls/LabelControls/`): Composite controls with integrated label, validation display, and mandatory indicators. Use for standard forms.

**Base Controls** (`src/Atc.Wpf.Controls/BaseControls/`): Core functionality controls without label wrapper. Use for custom layouts or building composite controls.

### MVVM Base Classes

Located in `src/Atc.Wpf/Mvvm/`:
- `ViewModelBase` - Standard ViewModel base
- `MainWindowViewModelBase` - For main window ViewModels
- `ViewModelDialogBase` - For dialog ViewModels
- `RelayCommand<T>` / `RelayCommandAsync<T>` - Command implementations

### Project Structure

```
src/
├── Atc.Wpf/                      # Core: MVVM, layouts, converters
├── Atc.Wpf.Controls/             # Rich control library
├── Atc.Wpf.Controls.Sample/      # Sample app controls
├── Atc.Wpf.FontIcons/            # Font icon support
├── Atc.Wpf.Theming/              # Theme infrastructure
├── Atc.Wpf.NetworkControls/      # Network scanning controls
└── Atc.Wpf.SourceGenerators/     # Code generators
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
- Test utilities: AutoFixture, FluentAssertions, NSubstitute
- Global usings for test projects include: AutoFixture, FluentAssertions, NSubstitute, Xunit

## Key Documentation

- MVVM guide: `docs/Mvvm/@Readme.md`
- Source generators: `docs/SourceGenerators/ViewModel.md`
- Label controls: `src/Atc.Wpf.Controls/LabelControls/Readme.md`
- Base controls: `src/Atc.Wpf.Controls/BaseControls/Readme.md`
