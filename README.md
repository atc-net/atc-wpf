# ATC.Net WPF

A comprehensive set of modern, enterprise-ready WPF libraries for building professional desktop applications with the MVVM design pattern.
This framework provides a rich collection of reusable controls, theming support, font icons, and MVVM infrastructure to accelerate WPF application development.

## ✨ Key Features

- 🎨 **Rich Control Library** - 25+ labeled controls, color pickers, selectors, and specialized input controls
- 🌓 **Light/Dark Theming** - Built-in theme support for all controls with easy customization
- 🎯 **MVVM Ready** - Complete MVVM infrastructure with observable properties and relay commands
- 🔤 **Font Icon Support** - Render SVG and image resources based on fonts
- ✅ **Smart Validation** - Deferred validation pattern for better user experience
- 📐 **Advanced Layouts** - GridEx, StaggeredPanel, UniformSpacingPanel and more
- 🌍 **Localization** - Built-in translation and localization support
- 🎭 **Value Converters** - Extensive collection of XAML value converters

## Requirements

- [.NET 9 - Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Windows 10 or later

## NuGet Packages Provided in this Repository

| Nuget package                                                                                                                                                                                       | Description                                         | Dependencies                                 |
|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------|----------------------------------------------|
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.svg?label=Atc.Wpf&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf)                                                 | Base Controls, ValueConverters, Extensions etc.     | <ul><li>Atc</li><li>Atc.XamlToolkit</li><li>Atc.XamlToolkit.Wpf</li></ul>                                          |
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.Controls.svg?label=Atc.Wpf.Controls&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf.Controls)                      | Miscellaneous UI Controls                           | <ul><li>Atc.Wpf</li><li>Atc.Wpf.FontIcons</li><li>Atc.Wpf.Theming</li><li>Atc.XamlToolkit</li><li>Atc.XamlToolkit.Wpf</li></ul>                    |
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.Controls.Sample.svg?label=Atc.Wpf.Controls.Sample&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf.Controls.Sample) | Controls for creating WPF sample apps               | <ul><li>Atc.Wpf</li><li>Atc.Wpf.Theming</li><li>Atc.Wpf.Controls</li></ul> |
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.FontIcons.svg?label=Atc.Wpf.FontIcons&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf.FontIcons)                   | Render Svg and Img resources based on fonts         | <ul><li>Atc.Wpf</li></ul>                                      |
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.Theming.svg?label=Atc.Wpf.Theming&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf.Theming)                         | Theming for Light & Dark mode for WPF base controls | <ul><li>Atc.Wpf</li><li>Atc.XamlToolkit</li><li>Atc.XamlToolkit.Wpf</li></ul>                                      |

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
  <PackageReference Include="Atc.Wpf" Version="2.*" />
  <PackageReference Include="Atc.Wpf.Controls" Version="2.*" />
  <PackageReference Include="Atc.Wpf.FontIcons" Version="2.*" />
  <PackageReference Include="Atc.Wpf.Theming" Version="2.*" />
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

## 🎯 Label Controls vs Base Controls

Understanding when to use each type of control:

| Aspect | Label Controls | Base Controls |
|--------|----------------|---------------|
| **Label** | Integrated label with text | No label (you provide your own) |
| **Validation** | Built-in validation message display | Validation logic only |
| **Mandatory** | Asterisk indicator support | Not applicable |
| **Use Case** | Standard forms and dialogs | Custom layouts and composite controls |
| **Example** | `<atc:LabelTextBox LabelText="Name" Text="{Binding Name}" />` | `<atc:IntegerBox Value="{Binding Count}" />` |

**Quick Guideline:**

- Use **Label Controls** for standard forms where you want consistency and less markup
- Use **Base Controls** when you need custom layouts or are building your own controls

## 💝 MVVM Made Easy

The `Atc.Wpf` package provides a complete MVVM infrastructure that makes it easy to separate UI and business logic using the MVVM pattern.

### Key MVVM Features

- **Observable Properties** - Automatic `INotifyPropertyChanged` implementation
- **Relay Commands** - Simple command implementation with `CanExecute` support
- **Async Commands** - Built-in support for async/await patterns
- **Source Generators** - Automatic code generation for ViewModels and properties

### Learn More

- [MVVM Framework Documentation](docs/Mvvm/@Readme.md)
  - [Observable Properties](docs/Mvvm/@Readme.md#observable-properties)
  - [RelayCommands](docs/Mvvm/@Readme.md#relay-commands)
  - [Source Generators](docs/SourceGenerators/ViewModel.md)

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

Modern layout panels for advanced UI composition:

- **[GridEx](src/Atc.Wpf/Controls/Layouts/GridEx_Readme.md)** - Enhanced Grid with auto-row/column generation
- **[StaggeredPanel](src/Atc.Wpf/Controls/Layouts/StaggeredPanel_Readme.md)** - Masonry-style layout panel
- **[UniformSpacingPanel](src/Atc.Wpf/Controls/Layouts/UniformSpacingPanel_Readme.md)** - Panel with consistent item spacing

### Media Controls

- **[SvgImage](src/Atc.Wpf/Controls/Media/SvgImage_Readme.md)** - SVG image rendering control

### Helpers & Utilities

- **[PanelHelper](src/Atc.Wpf/Helpers/PanelHelper_Readme.md)** - Helper methods for panel layout calculations

### Additional Features

- **[ShaderEffects](src/Atc.Wpf/Media/ShaderEffects/@Readme.md)** - HLSL-based visual effects
  - [HLSL Shader Compiler Guide](src/Atc.Wpf/Media/ShaderEffects/Shaders/@Readme.md)
- **[Translation & Localization](src/Atc.Wpf/Translation/@Readme.md)** - Multi-language support
- **[ValueConverters](src/Atc.Wpf/ValueConverters/@Readme.md)** - Collection of XAML value converters

## 💟 Atc.Wpf.Controls - Rich Control Library

A comprehensive collection of specialized WPF controls for enterprise applications.

### ⭐ Label Controls (25+ Controls)

**[📖 Complete Label Controls Documentation](src/Atc.Wpf.Controls/LabelControls/Readme.md)**

Labeled input controls with built-in validation, mandatory indicators, and consistent styling:

- **Text Input**: LabelTextBox, LabelPasswordBox
- **Number Input**: LabelIntegerBox, LabelDecimalBox, LabelIntegerXyBox, LabelDecimalXyBox, LabelPixelSizeBox
- **Date/Time**: LabelDatePicker, LabelTimePicker, LabelDateTimePicker
- **Selection**: LabelCheckBox, LabelComboBox, LabelToggleSwitch
- **Selectors**: LabelAccentColorSelector, LabelCountrySelector, LabelFontFamilySelector, LabelLanguageSelector, LabelThemeSelector, LabelWellKnownColorSelector
- **Pickers**: LabelColorPicker, LabelDirectoryPicker, LabelFilePicker
- **Network**: LabelEndpointBox (protocol + host + port with validation)
- **Display**: LabelContent, LabelTextInfo

All label controls support:

- ✅ Deferred validation (shows errors only after user interaction)
- ✅ Mandatory field indicators
- ✅ Consistent styling and theming
- ✅ MVVM-friendly data binding

### ⭐ Base Controls (14 Controls)

**[📖 Complete Base Controls Documentation](src/Atc.Wpf.Controls/BaseControls/Readme.md)**

Unlabeled input controls that provide core functionality without the label wrapper:

- **Text Input**: RichTextBoxEx
- **Number Input**: IntegerBox, DecimalBox, IntegerXyBox, DecimalXyBox, PixelSizeBox
- **Time Selection**: ClockPanelPicker
- **Toggle**: ToggleSwitch
- **Pickers**: ColorPicker, DirectoryPicker, FilePicker
- **Network**: EndpointBox (protocol + host + port with validation)

Base controls are perfect for:

- 🎯 Custom layouts where you position labels yourself
- 🧩 Building composite controls
- 📐 Situations where labels are not needed
- 🔨 Creating your own labeled control wrappers

### Color Controls

- **[WellKnownColorPicker](src/Atc.Wpf.Controls/ColorControls/WellKnownColorPicker_Readme.md)** - Predefined color palette picker

### Additional Resources

- **[ValueConverters](src/Atc.Wpf.Controls/ValueConverters/@Readme.md)** - Control-specific value converters

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

---

## 🎯 Source Generators

Atc.Wpf includes powerful source generators to reduce boilerplate code:

- **[ViewModel Generator](docs/SourceGenerators/ViewModel.md)** - Auto-generate ViewModels with observable properties
- **[DependencyProperty Generator](docs/SourceGenerators/DependencyProperty.md)** - Auto-generate dependency properties
- **[AttachedProperty Generator](docs/SourceGenerators/AttachedProperty.md)** - Auto-generate attached properties

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
