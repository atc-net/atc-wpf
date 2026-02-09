# ATC.Net WPF

A comprehensive set of modern, enterprise-ready WPF libraries for building professional desktop applications with the MVVM design pattern.
This framework provides a rich collection of reusable controls, theming support, font icons, and MVVM infrastructure to accelerate WPF application development.

## ✨ Key Features

- 🎨 **Rich Control Library** - 160+ controls including labeled form controls, flyouts, color pickers, selectors, and specialized input controls
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

                <!-- Component styles (dialogs, notifications, viewers) -->
                <ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Components;component/Styles/Controls.xaml" />

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
| **1. Base** | `Atc.Wpf` | MVVM, layouts, converters - no UI controls | ViewModelBase, GridEx, FlexPanel |
| **2. Controls** | `Atc.Wpf.Controls` | Atomic/primitive controls | IntegerBox, ToggleSwitch, Flyout, Carousel |
| **3. Forms** | `Atc.Wpf.Forms` | Labeled form fields with validation | LabelTextBox, LabelComboBox, LabelDatePicker |
| **4. Components** | `Atc.Wpf.Components` | Composite high-level components | InfoDialogBox, JsonViewer, ToastNotification |

**Quick Guidelines:**

- Use **Form Controls** (`Atc.Wpf.Forms`) for standard forms - they include labels, validation, and mandatory indicators
- Use **Base Controls** (`Atc.Wpf.Controls`) when you need custom layouts or are building composite controls
- Use **Components** (`Atc.Wpf.Components`) for dialogs, viewers, and settings panels

## 📋 Control Catalog at a Glance

A quick reference of all controls organized by category:

| Category | Controls | Package |
|----------|----------|---------|
| **Layout Panels** | GridEx, AutoGrid, FlexPanel, StaggeredPanel, UniformSpacingPanel, ResponsivePanel, DockPanelPro | Atc.Wpf / Atc.Wpf.Controls |
| **Data Display** | Card, Badge, Chip, Avatar, AvatarGroup, Divider, Carousel, Breadcrumb, Stepper, Segmented, Timeline | Atc.Wpf.Controls |
| **Flyouts** | Flyout, FlyoutHost, FlyoutService | Atc.Wpf.Controls |
| **Input Controls** | NumericBox, IntegerBox, DecimalBox, CurrencyBox, ToggleSwitch, RangeSlider, Rating, FilePicker, DirectoryPicker | Atc.Wpf.Controls |
| **Color Controls** | HueSlider, SaturationBrightnessPicker, TransparencySlider, WellKnownColorPicker | Atc.Wpf.Controls |
| **Buttons** | ImageButton, SplitButton, AuthenticationButton, ConnectivityButton | Atc.Wpf.Controls |
| **Progress** | BusyOverlay, LoadingIndicator, Overlay, Skeleton | Atc.Wpf.Controls |
| **Drag & Drop** | DragDropAttach | Atc.Wpf.Controls |
| **Selectors** | CountrySelector, LanguageSelector, FontFamilySelector, DualListSelector | Atc.Wpf.Controls |
| **Labeled Form Controls** | LabelTextBox, LabelIntegerBox, LabelComboBox, LabelDatePicker, LabelColorPicker, + 20 more | Atc.Wpf.Forms |
| **Dialogs** | InfoDialogBox, QuestionDialogBox, InputDialogBox, InputFormDialogBox, ColorPickerDialogBox | Atc.Wpf.Forms / Components |
| **Viewers** | JsonViewer, TerminalViewer | Atc.Wpf.Components |
| **Notifications** | ToastNotification, ToastNotificationManager, IToastNotificationService | Atc.Wpf.Components |
| **Printing** | IPrintService, PrintService, PrintPreviewWindow | Atc.Wpf / Atc.Wpf.Components |
| **Undo/Redo** | IUndoRedoService, UndoRedoService, UndoRedoHistoryView | Atc.Wpf / Atc.Wpf.Components |
| **Clipboard** | IClipboardService, ClipboardService | Atc.Wpf |
| **Busy Indicator** | IBusyIndicatorService, BusyIndicatorService | Atc.Wpf.Components |
| **Theming** | NiceWindow, ThemeSelector, AccentColorSelector, TransitioningContentControl | Atc.Wpf.Theming |
| **Font Icons** | FontAwesome (3 variants), Bootstrap, MaterialDesign, Weather, IcoFont | Atc.Wpf.FontIcons |
| **Network** | NetworkScannerView | Atc.Wpf.Network |

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

| Control | Description | Key Features | Documentation |
|---------|-------------|--------------|---------------|
| **GridEx** | Enhanced Grid | String-based row/column definitions | [Readme](src/Atc.Wpf/Controls/Layouts/GridEx_Readme.md) |
| **AutoGrid** | Auto-indexed Grid | Automatic child positioning | [Readme](src/Atc.Wpf.Controls/Layouts/AutoGrid_Readme.md) |
| **FlexPanel** | CSS Flexbox panel | Grow/shrink, justify, align | [Readme](src/Atc.Wpf.Controls/Layouts/FlexPanel_Readme.md) |
| **StaggeredPanel** | Masonry layout | Pinterest-style waterfall | [Readme](src/Atc.Wpf.Controls/Layouts/StaggeredPanel_Readme.md) |
| **UniformSpacingPanel** | Uniform spacing | Consistent gaps + wrapping | [Readme](src/Atc.Wpf.Controls/Layouts/UniformSpacingPanel_Readme.md) |
| **ResponsivePanel** | Responsive layout | Breakpoint-based responsive design | [Readme](src/Atc.Wpf.Controls/Layouts/ResponsivePanel_Readme.md) |
| **DockPanelPro** | IDE-style docking | Resizable regions | [Readme](src/Atc.Wpf.Controls/Layouts/DockPanelPro_Readme.md) |

### Data Display Controls

| Control | Description | Key Features | Documentation |
|---------|-------------|--------------|---------------|
| **Card** | Content container | Elevation, header/footer, expand | [Readme](src/Atc.Wpf.Controls/DataDisplay/Card_Readme.md) |
| **Badge** | Status indicator | Notification counts, dots | [Readme](src/Atc.Wpf.Controls/DataDisplay/Badge_Readme.md) |
| **Chip** | Tag/filter control | Selectable, removable | [Readme](src/Atc.Wpf.Controls/DataDisplay/Chip_Readme.md) |
| **Avatar** | User profile picture | Initials fallback, status indicator | [Readme](src/Atc.Wpf.Controls/DataDisplay/Avatar_Readme.md) |
| **Divider** | Visual separator | Horizontal/vertical | [Readme](src/Atc.Wpf.Controls/DataDisplay/Divider_Readme.md) |
| **Carousel** | Image carousel | Navigation, auto-play, swipe | [Readme](src/Atc.Wpf.Controls/DataDisplay/Carousel_Readme.md) |
| **Breadcrumb** | Navigation path | Overflow, custom separators | [Readme](src/Atc.Wpf.Controls/DataDisplay/Breadcrumb_Readme.md) |
| **Stepper** | Step-by-step progress | Cancelable transitions | [Readme](src/Atc.Wpf.Controls/DataDisplay/Stepper_Readme.md) |
| **Segmented** | Segment selector | Mutually exclusive selection | [Readme](src/Atc.Wpf.Controls/DataDisplay/Segmented_Readme.md) |
| **Timeline** | Timeline display | Vertical/horizontal, alternate mode | [Readme](src/Atc.Wpf.Controls/DataDisplay/Timeline_Readme.md) |

### Media Controls

- **[SvgImage](src/Atc.Wpf/Controls/Media/SvgImage_Readme.md)** - SVG image rendering control

### Helpers & Utilities

- **[PanelHelper](src/Atc.Wpf/Helpers/PanelHelper_Readme.md)** - Helper methods for panel layout calculations

### Services

| Service | Description | Documentation |
|---------|-------------|---------------|
| **IClipboardService** | MVVM-friendly clipboard operations with history | [Readme](src/Atc.Wpf/Clipboard/ClipboardService_Readme.md) |
| **IUndoRedoService** | Undo/redo with command grouping and history limits | [Readme](src/Atc.Wpf/UndoRedo/UndoRedoService_Readme.md) |
| **IPrintService** | Print and print-preview service interface | [Readme](src/Atc.Wpf.Components/Printing/PrintService_Readme.md) |

### Additional Features

- **[ShaderEffects](src/Atc.Wpf/Media/ShaderEffects/@Readme.md)** - HLSL-based visual effects
  - [HLSL Shader Compiler Guide](src/Atc.Wpf/Media/ShaderEffects/Shaders/@Readme.md)
- **[Translation & Localization](src/Atc.Wpf/Translation/@Readme.md)** - Multi-language support
- **[ValueConverters](src/Atc.Wpf/ValueConverters/@Readme.md)** - Collection of XAML value converters

## 💟 Atc.Wpf.Controls - Atomic Control Library

A collection of primitive/atomic WPF controls - single-purpose building blocks.

### ⭐ Input Controls

Unlabeled input controls that provide core functionality:

| Category | Controls |
|----------|----------|
| **Number Input** | NumericBox, IntegerBox, DecimalBox, CurrencyBox, IntegerXyBox, DecimalXyBox, PixelSizeBox |
| **Toggle & Slider** | ToggleSwitch, RangeSlider, [Rating](src/Atc.Wpf.Controls/Inputs/Readme.md) |
| **Pickers** | [DirectoryPicker, FilePicker](src/Atc.Wpf.Controls/Pickers/Readme.md) |
| **Text Input** | RichTextBoxEx |

### Button Controls

| Control | Description | Documentation |
|---------|-------------|---------------|
| **ImageButton** | Button with bitmap/SVG image | [Readme](src/Atc.Wpf.Controls/Buttons/ImageButton_Readme.md) |
| **ImageToggledButton** | Two-state toggle button | [Readme](src/Atc.Wpf.Controls/Buttons/ImageToggledButton_Readme.md) |
| **SplitButton** | Primary action + dropdown menu | [Readme](src/Atc.Wpf.Controls/Buttons/SplitButton_Readme.md) |
| **AuthenticationButton** | Login/logout toggle | [Readme](src/Atc.Wpf.Controls/Buttons/AuthenticationButton_Readme.md) |
| **ConnectivityButton** | Connect/disconnect toggle | [Readme](src/Atc.Wpf.Controls/Buttons/ConnectivityButton_Readme.md) |

### Color Controls

| Control | Description | Documentation |
|---------|-------------|---------------|
| **WellKnownColorPicker** | Named color palette picker | [Readme](src/Atc.Wpf.Controls/ColorEditing/WellKnownColorPicker_Readme.md) |
| **HueSlider** | Hue selection slider | [Readme](src/Atc.Wpf.Controls/ColorEditing/HueSlider_Readme.md) |
| **SaturationBrightnessPicker** | 2D saturation/brightness picker | [Readme](src/Atc.Wpf.Controls/ColorEditing/SaturationBrightnessPicker_Readme.md) |
| **TransparencySlider** | Alpha channel slider | [Readme](src/Atc.Wpf.Controls/ColorEditing/TransparencySlider_Readme.md) |

### Layout Controls

| Control | Description | Documentation |
|---------|-------------|---------------|
| **DockPanelPro** | IDE-style docking panel | [Readme](src/Atc.Wpf.Controls/Layouts/DockPanelPro_Readme.md) |
| **GridLines** | Grid overlay for debugging | [Readme](src/Atc.Wpf.Controls/Layouts/GridLines_Readme.md) |
| **GroupBoxExpander** | Collapsible group box | [Readme](src/Atc.Wpf.Controls/Layouts/GroupBoxExpander_Readme.md) |

### Data Display Controls

| Control | Description | Documentation |
|---------|-------------|---------------|
| **Avatar** | User profile pictures with initials fallback | [Readme](src/Atc.Wpf.Controls/DataDisplay/Avatar_Readme.md) |
| **Carousel** | Image carousel/slideshow with navigation | [Readme](src/Atc.Wpf.Controls/DataDisplay/Carousel_Readme.md) |
| **Badge** | Status indicator overlay | [Readme](src/Atc.Wpf.Controls/DataDisplay/Badge_Readme.md) |
| **Card** | Elevated container with header/footer | [Readme](src/Atc.Wpf.Controls/DataDisplay/Card_Readme.md) |
| **Chip** | Tag/filter interactive elements | [Readme](src/Atc.Wpf.Controls/DataDisplay/Chip_Readme.md) |
| **Divider** | Visual separator (horizontal/vertical) | [Readme](src/Atc.Wpf.Controls/DataDisplay/Divider_Readme.md) |
| **Breadcrumb** | Navigation path with overflow | [Readme](src/Atc.Wpf.Controls/DataDisplay/Breadcrumb_Readme.md) |
| **Segmented** | Mutually exclusive segment selector | [Readme](src/Atc.Wpf.Controls/DataDisplay/Segmented_Readme.md) |
| **Stepper** | Step-by-step progress indicator | [Readme](src/Atc.Wpf.Controls/DataDisplay/Stepper_Readme.md) |
| **Timeline** | Vertical/horizontal timeline display | [Readme](src/Atc.Wpf.Controls/DataDisplay/Timeline_Readme.md) |

### Flyout Controls

Sliding panel overlays that slide in from window edges - inspired by Azure Portal blade pattern.

| Control | Description | Documentation |
|---------|-------------|---------------|
| **Flyout** | Sliding panel overlay from edges (Right, Left, Top, Bottom) | [Readme](src/Atc.Wpf.Controls/Flyouts/Flyout_Readme.md) |
| **FlyoutHost** | Container that manages multiple flyouts with nesting support | [Readme](src/Atc.Wpf.Controls/Flyouts/Flyout_Readme.md#-flyouthost) |
| **FlyoutService** | MVVM-friendly service for programmatic flyout management | [Readme](src/Atc.Wpf.Controls/Flyouts/Flyout_Readme.md#-iflyoutservice-mvvm) |

**Key Features:**
- 🎯 Four positions: Right (default), Left, Top, Bottom
- 🌓 Light dismiss (click outside or Escape key)
- 📚 Nested flyouts (Azure Portal drill-down style)
- 🎬 Smooth slide animations
- 🎨 Full theming support

```xml
<!-- Basic Flyout -->
<flyouts:Flyout x:Name="SettingsFlyout" Header="Settings" FlyoutWidth="400">
    <StackPanel Margin="16">
        <CheckBox Content="Enable notifications" />
        <CheckBox Content="Dark mode" />
    </StackPanel>
</flyouts:Flyout>
```

### Drag & Drop

| Control | Description | Documentation |
|---------|-------------|---------------|
| **DragDropAttach** | XAML-driven drag-and-drop framework | [Readme](src/Atc.Wpf.Controls/DragDrop/DragDropAttach_Readme.md) |

### Selector Controls

| Control | Description | Documentation |
|---------|-------------|---------------|
| **CountrySelector** | Country dropdown with flags | [Readme](src/Atc.Wpf.Controls/Selectors/CountrySelector_Readme.md) |
| **LanguageSelector** | Language dropdown with flags | [Readme](src/Atc.Wpf.Controls/Selectors/LanguageSelector_Readme.md) |
| **FontFamilySelector** | Font family dropdown with previews | [Readme](src/Atc.Wpf.Controls/Selectors/FontFamilySelector_Readme.md) |
| **DualListSelector** | Dual-list transfer control | [Readme](src/Atc.Wpf.Controls/Selectors/DualListSelector_Readme.md) |

### Progress Controls

| Control | Description | Documentation |
|---------|-------------|---------------|
| **BusyOverlay** | Dimming overlay with loading indicator | [Readme](src/Atc.Wpf.Controls/Progressing/BusyOverlay_Readme.md) |
| **LoadingIndicator** | Animated loading indicator | [Readme](src/Atc.Wpf.Controls/Progressing/LoadingIndicator_Readme.md) |
| **Overlay** | Content dimming with fade animations | [Readme](src/Atc.Wpf.Controls/Progressing/Overlay_Readme.md) |
| **Skeleton** | Loading placeholder with shimmer effect | [Readme](src/Atc.Wpf.Controls/Progressing/Skeleton_Readme.md) |

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

| Control | Description | Documentation |
|---------|-------------|---------------|
| **InfoDialogBox** | Information-only dialog | [Readme](src/Atc.Wpf.Components/Dialogs/InfoDialogBox_Readme.md) |
| **QuestionDialogBox** | Yes/No confirmation dialog | [Readme](src/Atc.Wpf.Components/Dialogs/QuestionDialogBox_Readme.md) |
| **InputDialogBox** | Single input dialog | [Readme](src/Atc.Wpf.Components/Dialogs/InputDialogBox_Readme.md) |
| **InputFormDialogBox** | Multi-field form dialog | [Readme](src/Atc.Wpf.Components/Dialogs/InputFormDialogBox_Readme.md) |
| **BasicApplicationSettingsDialogBox** | Settings dialog | [Readme](src/Atc.Wpf.Components/Dialogs/BasicApplicationSettingsDialogBox_Readme.md) |
| **DialogService** | MVVM-friendly dialog management | - |

*Note: ColorPickerDialogBox is located in `Atc.Wpf.Forms.Dialogs`*

### Viewers

| Control | Description | Documentation |
|---------|-------------|---------------|
| **JsonViewer** | JSON document viewer with syntax highlighting | [Readme](src/Atc.Wpf.Components/Viewers/JsonViewer_Readme.md) |
| **TerminalViewer** | Terminal/console output viewer | [Readme](src/Atc.Wpf.Components/Viewers/TerminalViewer_Readme.md) |

### Monitoring

| Control | Description | Documentation |
|---------|-------------|---------------|
| **ApplicationMonitorView** | Application event monitoring component | [Readme](src/Atc.Wpf.Components/Monitoring/ApplicationMonitorView_Readme.md) |

### Notifications

| Control | Description | Documentation |
|---------|-------------|---------------|
| **ToastNotification** | Toast notification system | [Readme](src/Atc.Wpf.Components/Notifications/ToastNotification_Readme.md) |
| **IToastNotificationService** | MVVM-friendly notification service | [Readme](src/Atc.Wpf.Components/Notifications/ToastNotification_Readme.md) |

### Printing

| Control | Description | Documentation |
|---------|-------------|---------------|
| **IPrintService / PrintService** | MVVM-friendly print and preview service | [Readme](src/Atc.Wpf.Components/Printing/PrintService_Readme.md) |

### Undo/Redo

| Control | Description | Documentation |
|---------|-------------|---------------|
| **UndoRedoHistoryView** | Unified history view for navigating undo/redo stacks | [Readme](src/Atc.Wpf.Components/UndoRedo/UndoRedoHistoryView_Readme.md) |

### Busy Indicator

| Control | Description | Documentation |
|---------|-------------|---------------|
| **IBusyIndicatorService** | MVVM-friendly busy overlay management | [Readme](src/Atc.Wpf.Components/Progressing/BusyIndicatorService_Readme.md) |

### Settings

| Control | Description | Documentation |
|---------|-------------|---------------|
| **BasicApplicationSettingsView** | Embeddable settings panel | [Readme](src/Atc.Wpf.Components/Settings/BasicApplicationSettingsView_Readme.md) |

## 💟 Atc.Wpf.FontIcons - Font-Based Icons

Render SVG and image resources using font-based icon systems for crisp, scalable icons. See the **[complete Font Icons documentation](docs/FontIcons/@Readme.md)** for detailed usage.

### Features

- 🎨 Scalable vector icons from 7 font families
- 🎯 Font-based (`Font*`) and image-based (`Image*`) rendering
- 🔄 Spin animation, rotation, and flip transformations
- 🌈 Color and size customization

### Supported Icon Families

Font Awesome (Solid, Regular, Brand), Bootstrap Glyphicons, Material Design, Weather Icons, IcoFont

### Resources

- **[Font Icons Guide](docs/FontIcons/@Readme.md)** - Complete documentation for all 14 icon controls
- **[ValueConverters](src/Atc.Wpf.FontIcons/ValueConverters/@Readme.md)** - Icon-related value converters

## 💟 Atc.Wpf.Theming - Light & Dark Mode

Complete theming infrastructure with Light and Dark mode support for all WPF controls. See the **[complete Theming documentation](docs/Theming/@Readme.md)** for detailed usage.

### Features

- 🌓 Light/Dark theme switching
- 🎨 Accent color customization
- 🎯 Automatic control styling
- 🔄 Runtime theme changes
- 🪟 NiceWindow with customizable title bar, window commands, and dialog hosting

### Key Controls

| Control | Description | Documentation |
|---------|-------------|---------------|
| **NiceWindow** | Enhanced window with title bar, commands, overlays | [Readme](docs/Theming/@Readme.md#-nicewindow) |
| **ThemeSelector** | Light/Dark theme dropdown | [Readme](docs/Theming/@Readme.md#-themeselector) |
| **AccentColorSelector** | Accent color dropdown | [Readme](docs/Theming/@Readme.md#-accentcolorselector) |
| **TransitioningContentControl** | Animated content transitions | [Readme](docs/Theming/@Readme.md#-transitioningcontentcontrol) |

### Resources

- **[Theming Guide](docs/Theming/@Readme.md)** - Complete documentation for all theming controls
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
