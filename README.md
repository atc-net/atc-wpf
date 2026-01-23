# ATC.Net WPF

A comprehensive set of modern, enterprise-ready WPF libraries for building professional desktop applications with the MVVM design pattern.
This framework provides a rich collection of reusable controls, theming support, font icons, and MVVM infrastructure to accelerate WPF application development.

## ✨ Key Features

- 🎨 **Rich Control Library** - 70+ controls including labeled form controls, color pickers, selectors, and specialized input controls
- 🏛️ **Four-Tier Architecture** - Clear separation: Base → Controls → Forms → Components
- 🌓 **Light/Dark Theming** - Built-in theme support for all controls with easy customization
- 🎯 **MVVM Ready** - Complete MVVM infrastructure with observable properties and relay commands
- 🔤 **Font Icon Support** - Render SVG and image resources based on fonts
- ✅ **Smart Validation** - Deferred validation pattern for better user experience
- 📐 **Advanced Layouts** - FlexPanel, AutoGrid, StaggeredPanel, Card, Badge and more
- 🌍 **Localization** - Built-in translation and localization support
- 🎭 **Value Converters** - Extensive collection of XAML value converters

## Requirements

- [.NET 10 - Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- Windows 10 or later

## NuGet Packages Provided in this Repository

| Nuget package                                                                                                                                                                                       | Description                                         | Dependencies                                 |
|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------|----------------------------------------------|
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.svg?label=Atc.Wpf&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf)                                                 | Core library: MVVM, layouts, value converters       | <ul><li>Atc</li></ul>                                          |
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.Controls.svg?label=Atc.Wpf.Controls&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf.Controls)                      | Atomic controls: base inputs, buttons, colors       | <ul><li>Atc.Wpf</li><li>Atc.Wpf.FontIcons</li><li>Atc.Wpf.Theming</li></ul>                    |
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.Forms.svg?label=Atc.Wpf.Forms&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf.Forms)                               | Form field controls: 25+ labeled controls with validation | <ul><li>Atc.Wpf.Controls</li><li>Atc.Wpf.Theming</li></ul>                    |
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.Components.svg?label=Atc.Wpf.Components&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf.Components)                | Composite components: dialogs, viewers, settings    | <ul><li>Atc.Wpf.Forms</li><li>Atc.Wpf.FontIcons</li></ul>                    |
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.FontIcons.svg?label=Atc.Wpf.FontIcons&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf.FontIcons)                   | Font-based icon rendering                           | <ul><li>Atc.Wpf</li></ul>                                      |
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.Theming.svg?label=Atc.Wpf.Theming&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf.Theming)                         | Light & Dark mode theming infrastructure            | <ul><li>Atc.Wpf</li><li>ControlzEx</li><li>Microsoft.Windows.CsWin32</li></ul>                                      |
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.Network.svg?label=Atc.Wpf.Network&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf.Network)                         | Network scanning and discovery controls             | <ul><li>Atc.Network</li><li>Atc.Wpf.Controls</li><li>Atc.Wpf.Forms</li></ul>                                             |
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.Controls.Sample.svg?label=Atc.Wpf.Controls.Sample&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf.Controls.Sample) | Controls for building sample applications           | <ul><li>Atc.Wpf.Components</li><li>MdXaml</li></ul> |

## 🔎 Demonstration Application

The demonstration application, `Atc.Wpf.Sample`, functions as a control explorer.
It provides quick visualization of a given control, along with options for
copying and pasting the XAML markup and/or the C# code for how to use it.

## 🎈 Playground and Viewer for a Given Control or Functionality

The following example is taken from the ReplayCommandAsync which illustrates its usage:

- The `Sample` tab shows how to use the control or feature.
- The `XAML` tab displays the corresponding XAML markup.
- The `CodeBehind` tab reveals the underlying code-behind.
- The `ViewModel` tab displays the associated ViewModel, if used.
- The `Readme` tab displays the associated [control]_Readme.md, if exist.

|                                                                         |                                                                       |
|-------------------------------------------------------------------------|-----------------------------------------------------------------------|
| Sample ![Img](docs/images/lm-wpf-replaycommandasync-sample.png)         | XAML ![Img](docs/images/lm-wpf-replaycommandasync-xaml.png)           |
| CodeBehind ![Img](docs/images/lm-wpf-replaycommandasync-codebehind.png) | ViewModel ![Img](docs/images/lm-wpf-replaycommandasync-viewmodel.png) |

### 🔦 Initial glimpse at the demonstration application

| Light-Mode                                                                   | Dark-Mode                                                                    |
|------------------------------------------------------------------------------|------------------------------------------------------------------------------|
| Wpf - AutoGrid ![Img](docs/images/lm-wpf-autogrid.png)                       | Wpf - AutoGrid ![Img](docs/images/dm-wpf-autogrid.png)                       |
| Wpf.Controls - Label MIX ![Img](docs/images/lm-wpf-controls-label-mix.png)   | Wpf.Controls - Label MIX ![Img](docs/images/dm-wpf-controls-label-mix.png)   |
| Wpf.Theming - ImageButton ![Img](docs/images/lm-wpf-theming-imagebutton.png) | Wpf.Theming - ImageButton ![Img](docs/images/dm-wpf-theming-imagebutton.png) |
| Wpf.FontIcons - Viewer ![Img](docs/images/lm-wpf-fonicons-viewer.png)        | Wpf.FontIcons - Viewer ![Img](docs/images/dm-wpf-fonicons-viewer.png)        |

# 🚀 Quick Start Guide

## Installation

Add the NuGet packages to your `.csproj` file:

```xml
<ItemGroup>
  <!-- Core packages -->
  <PackageReference Include="Atc.Wpf" Version="4.*" />
  <PackageReference Include="Atc.Wpf.Theming" Version="4.*" />

  <!-- For form controls (LabelTextBox, LabelComboBox, etc.) -->
  <PackageReference Include="Atc.Wpf.Forms" Version="4.*" />

  <!-- For composite components (dialogs, viewers) -->
  <PackageReference Include="Atc.Wpf.Components" Version="4.*" />

  <!-- Optional: Font icons -->
  <PackageReference Include="Atc.Wpf.FontIcons" Version="4.*" />
</ItemGroup>
```

## Configuration

### 1. Update App.xaml

Add the required resource dictionaries to enable theming and control styles:

```xml
<Application
    x:Class="YourApp.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- Base theming styles for Light/Dark mode -->
                <ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Theming;component/Styles/Default.xaml" />

                <!-- Control library styles -->
                <ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Controls;component/Styles/Controls.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### 2. Use Controls in XAML

Now you can use all controls with full theming support:

```xml
<Window
    xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
    ...>

    <!-- Labeled controls with validation -->
    <atc:LabelTextBox
        LabelText="User Name"
        Text="{Binding UserName}"
        IsMandatory="True" />

    <!-- Network endpoint input -->
    <atc:LabelEndpointBox
        LabelText="API Endpoint"
        NetworkProtocol="Https"
        Value="{Binding EndpointUri}" />

    <!-- Standard WPF controls automatically themed -->
    <Button Content="Save" Command="{Binding SaveCommand}" />
    <TextBox Text="{Binding Notes}" />
</Window>
```

All standard WPF controls (Button, TextBox, ComboBox, etc.) are automatically styled with Light/Dark theme support.

## 🎯 Four-Tier Architecture

Understanding the package hierarchy and when to use each:

| Tier | Package | Purpose | Example Controls |
|------|---------|---------|------------------|
| **1. Base** | `Atc.Wpf` | MVVM, layouts, converters - no UI controls | ViewModelBase, GridEx, AutoGrid |
| **2. Controls** | `Atc.Wpf.Controls` | Atomic/primitive controls | IntegerBox, ToggleSwitch, ColorPicker |
| **3. Forms** | `Atc.Wpf.Forms` | Labeled form fields with validation | LabelTextBox, LabelComboBox, LabelDatePicker |
| **4. Components** | `Atc.Wpf.Components` | Composite high-level components | InfoDialogBox, JsonViewer, SettingsView |

**Quick Guidelines:**

- Use **Form Controls** (`Atc.Wpf.Forms`) for standard forms - they include labels, validation, and mandatory indicators
- Use **Base Controls** (`Atc.Wpf.Controls`) when you need custom layouts or are building composite controls
- Use **Components** (`Atc.Wpf.Components`) for dialogs, viewers, and settings panels

## 💝 MVVM Made Easy

For MVVM infrastructure, add the [Atc.XamlToolkit](https://github.com/atc-net/atc-xaml-toolkit) package to your project. This provides powerful source generators and base classes for clean MVVM architecture.

> **Note:** As of version 4.x, `Atc.Wpf` no longer directly depends on `Atc.XamlToolkit` to optimize build performance. Add `Atc.XamlToolkit` directly to your project to use source generators and MVVM base classes.

### Key MVVM Features (from Atc.XamlToolkit)

- **Observable Properties** - Automatic `INotifyPropertyChanged` implementation via `[ObservableProperty]` attribute
- **Relay Commands** - Simple command implementation with `CanExecute` support via `[RelayCommand]` attribute
- **Async Commands** - Built-in support for async/await patterns
- **Source Generators** - Automatic code generation for ViewModels, DependencyProperties, and AttachedProperties

### MVVM Components

| Component | Source | Description |
|-----------|--------|-------------|
| `ViewModelBase` | Atc.XamlToolkit.Mvvm | Base class for ViewModels |
| `ObservableObject` | Atc.XamlToolkit.Mvvm | Base class implementing `INotifyPropertyChanged` |
| `RelayCommand<T>` | Atc.XamlToolkit.Command | Command with `CanExecute` support |
| `RelayCommandAsync<T>` | Atc.XamlToolkit.Command | Async command with `CanExecute` support |
| `[ObservableProperty]` | Atc.XamlToolkit | Source generator for properties |
| `[RelayCommand]` | Atc.XamlToolkit | Source generator for commands |
| `[DependencyProperty]` | Atc.XamlToolkit.Wpf | Source generator for WPF dependency properties |
| `[AttachedProperty]` | Atc.XamlToolkit.Wpf | Source generator for WPF attached properties |

### Learn More

- [MVVM Framework Documentation](docs/Mvvm/@Readme.md)
- [Source Generators Documentation](docs/SourceGenerators/ViewModel.md)
- [Atc.XamlToolkit GitHub Repository](https://github.com/atc-net/atc-xaml-toolkit)

### Quick MVVM Example

```csharp
public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private bool isEnabled = true;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        await SaveUserDataAsync(UserName);
    }

    private bool CanSave() => !string.IsNullOrEmpty(UserName);
}
```

---

# 📚 Comprehensive Documentation

> **Note:** While this README provides an overview, the best way to explore all controls and components is to run the **`Atc.Wpf.Sample`** application,
> which includes interactive examples with XAML and code-behind for every control! �

## 💟 Atc.Wpf - Core WPF Library

The foundation library providing essential WPF controls, layouts, and utilities.

### Layout Controls

Modern layout panels and containers for advanced UI composition. See the **[complete Layout Controls documentation](docs/Layouts/@Readme.md)** for detailed usage.

| Control | Description | Key Features |
|---------|-------------|--------------|
| **[GridEx](src/Atc.Wpf/Controls/Layouts/GridEx_Readme.md)** | Enhanced Grid | String-based row/column definitions |
| **[AutoGrid](docs/Layouts/@Readme.md#-autogrid)** | Auto-indexed Grid | Automatic child positioning |
| **[FlexPanel](docs/Layouts/@Readme.md#-flexpanel)** | CSS Flexbox panel | Grow/shrink, justify, align |
| **[StaggeredPanel](src/Atc.Wpf.Controls/Layouts/StaggeredPanel_Readme.md)** | Masonry layout | Pinterest-style waterfall |
| **[UniformSpacingPanel](src/Atc.Wpf.Controls/Layouts/UniformSpacingPanel_Readme.md)** | Uniform spacing | Consistent gaps + wrapping |
| **[Card](docs/DataDisplay/@Readme.md#-card)** | Content container | Elevation, header/footer, expand |
| **[Badge](docs/DataDisplay/@Readme.md#-badge)** | Status indicator | Notification counts, dots |
| **[Chip](docs/DataDisplay/@Readme.md#-chip)** | Tag/filter control | Selectable, removable |
| **[Avatar](docs/DataDisplay/@Readme.md#-avatar)** | User profile picture | Initials fallback, status indicator |

### Media Controls

- **[SvgImage](src/Atc.Wpf/Controls/Media/SvgImage_Readme.md)** - SVG image rendering control

### Helpers & Utilities

- **[PanelHelper](src/Atc.Wpf/Helpers/PanelHelper_Readme.md)** - Helper methods for panel layout calculations

### Additional Features

- **[ShaderEffects](src/Atc.Wpf/Media/ShaderEffects/@Readme.md)** - HLSL-based visual effects
  - [HLSL Shader Compiler Guide](src/Atc.Wpf/Media/ShaderEffects/Shaders/@Readme.md)
- **[Translation & Localization](src/Atc.Wpf/Translation/@Readme.md)** - Multi-language support
- **[ValueConverters](src/Atc.Wpf/ValueConverters/@Readme.md)** - Collection of XAML value converters

## 💟 Atc.Wpf.Controls - Atomic Control Library

A collection of primitive/atomic WPF controls - single-purpose building blocks.

### ⭐ Base Controls (14 Controls)

Unlabeled input controls that provide core functionality:

- **Number Input**: NumericBox, IntegerBox, DecimalBox, IntegerXyBox, DecimalXyBox, PixelSizeBox
- **Toggle**: ToggleSwitch, RangeSlider
- **Pickers**: DirectoryPicker, FilePicker
- **Text Input**: RichTextBoxEx

### Button Controls

- AuthenticationButton, ConnectivityButton, ImageButton, ImageToggledButton

### Color Controls

- HueSlider, SaturationBrightnessPicker, TransparencySlider, WellKnownColorPicker

### Layout Controls

- DockPanelPro, GridLines, GroupBoxExpander

### Data Display Controls

- Avatar, AvatarGroup, Badge, Card, Chip, Divider

### Progress Controls

- BusyOverlay, LoadingIndicator

## 💟 Atc.Wpf.Forms - Form Field Library

Form field components with labels, validation, and mandatory indicators for building data entry forms.

### ⭐ Label Controls (25+ Controls)

Labeled input controls with built-in validation and consistent styling:

- **Text Input**: LabelTextBox, LabelPasswordBox
- **Number Input**: LabelIntegerBox, LabelDecimalBox, LabelIntegerXyBox, LabelDecimalXyBox, LabelPixelSizeBox
- **Date/Time**: LabelDatePicker, LabelTimePicker, LabelDateTimePicker
- **Selection**: LabelCheckBox, LabelComboBox, LabelToggleSwitch, LabelSlider
- **Selectors**: LabelAccentColorSelector, LabelCountrySelector, LabelFontFamilySelector, LabelLanguageSelector, LabelThemeSelector, LabelWellKnownColorSelector
- **Pickers**: LabelColorPicker, LabelDirectoryPicker, LabelFilePicker
- **Network**: LabelEndpointBox (protocol + host + port with validation)
- **Display**: LabelContent, LabelTextInfo

All label controls support:

- ✅ Deferred validation (shows errors only after user interaction)
- ✅ Mandatory field indicators
- ✅ Consistent styling and theming
- ✅ MVVM-friendly data binding

### Form Infrastructure

- **Abstractions**: ILabelControl, ILabelControlsForm
- **Extractors**: Form data extraction utilities
- **Factories**: LabelControlFactory, LabelControlsFormFactory
- **Writers**: Form rendering utilities

## 💟 Atc.Wpf.Components - Composite Components

Higher-level composite components combining multiple controls for business-ready UI.

### Dialogs

- InfoDialogBox, QuestionDialogBox, InputDialogBox, InputFormDialogBox, BasicApplicationSettingsDialogBox
- DialogService - MVVM-friendly dialog management

*Note: ColorPickerDialogBox is located in `Atc.Wpf.Forms.Dialogs`*

### Viewers

- JsonViewer - JSON document viewer with syntax highlighting
- TerminalViewer - Terminal/console output viewer

### Monitoring

- ApplicationMonitorView - Application event monitoring component

### Notifications

- ToastNotificationManager, ToastNotificationArea, ToastNotification - Toast notification system

### Settings

- BasicApplicationSettingsView, BasicApplicationSettingsViewModel

## 💟 Atc.Wpf.FontIcons - Font-Based Icons

Render SVG and image resources using font-based icon systems for crisp, scalable icons.

### Features

- 🎨 Scalable vector icons
- 🎯 Font-based rendering
- 📦 Multiple icon font support
- 🌈 Color and size customization

### Resources

- **[ValueConverters](src/Atc.Wpf.FontIcons/ValueConverters/@Readme.md)** - Icon-related value converters

## 💟 Atc.Wpf.Theming - Light & Dark Mode

Complete theming infrastructure with Light and Dark mode support for all WPF controls.

### Features

- 🌓 Light/Dark theme switching
- 🎨 Accent color customization
- 🎯 Automatic control styling
- 🔄 Runtime theme changes
- 📱 Consistent visual design

### Resources

- **[ValueConverters](src/Atc.Wpf.Theming/ValueConverters/@Readme.md)** - Theme-aware value converters

## 💟 Atc.Wpf.Network - Network Discovery Controls

Specialized controls for network scanning and host discovery, built on the [Atc.Network](https://github.com/atc-net/atc-network) library.

### NetworkScannerView Control

A comprehensive network scanner control that displays scan results in a sortable, filterable ListView.

**Features:**

- 🔍 IP range scanning with configurable start/end addresses
- 📊 Real-time progress reporting with cancellation support
- 🌐 Displays IP address, hostname, MAC address, and vendor information
- 🚪 Port scanning with configurable port lists
- 📶 Network quality indicators (ping status visualization)
- 🔄 Sortable columns with click-to-sort headers
- 🎛️ Configurable column visibility
- 🔎 Built-in filtering (show only successful pings, show only hosts with open ports)
- 🌍 Localization support (English, Danish, German)

**Usage:**

```xml
<Window xmlns:atcNetwork="clr-namespace:Atc.Wpf.Network;assembly=Atc.Wpf.Network">
    <atcNetwork:NetworkScannerView DataContext="{Binding NetworkScannerVm}" />
</Window>
```

```csharp
// ViewModel setup
var viewModel = new NetworkScannerViewModel
{
    StartIpAddress = "192.168.1.1",
    EndIpAddress = "192.168.1.254",
    PortsNumbers = [80, 443, 22, 3389]
};

// Start scanning
await viewModel.ScanCommand.ExecuteAsync(null);

// Handle selection changes
viewModel.EntrySelected += (sender, args) =>
{
    var selectedHost = args.NetworkHost;
    // Handle selected host
};
```

**ViewModels:**

- `NetworkScannerViewModel` - Main ViewModel for the scanner control
- `NetworkHostViewModel` - Represents a discovered network host
- `NetworkScannerColumnsViewModel` - Controls column visibility
- `NetworkScannerFilterViewModel` - Controls result filtering

---

## 🎯 Source Generators

Atc.Wpf includes powerful source generators from [Atc.XamlToolkit](https://github.com/atc-net/atc-xaml-toolkit) to reduce boilerplate code:

- **[ViewModel Generator](docs/SourceGenerators/ViewModel.md)** - Auto-generate ViewModels with `[ObservableProperty]` and `[RelayCommand]`
- **[DependencyProperty Generator](docs/SourceGenerators/DependencyProperty.md)** - Auto-generate WPF dependency properties with `[DependencyProperty]`
- **[AttachedProperty Generator](docs/SourceGenerators/AttachedProperty.md)** - Auto-generate WPF attached properties with `[AttachedProperty]`

---

## 🤝 Contributing

We welcome contributions! Please read our guidelines:

- **[Contribution Guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)** - How to contribute to ATC projects
- **[Coding Guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)** - Code style and best practices

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

Built with ❤️ by the ATC.Net team and contributors.

- Visit [atc-net.github.io](https://atc-net.github.io) for more ATC projects
- Report issues on [GitHub Issues](https://github.com/atc-net/atc-wpf/issues)
- Contribute on [GitHub](https://github.com/atc-net/atc-wpf)
