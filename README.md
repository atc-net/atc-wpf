# ATC.Net WPF

This is a base libraries for building WPF application with the MVVM design pattern.

## Requirements

[.NET 9 - Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## NuGet Packages Provided in this Repository

| Nuget package                                                                                                                                                                                       | Description                                         | Dependencies                                 |
|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------|----------------------------------------------|
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.svg?label=Atc.Wpf&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf)                                                 | Base Controls, ValueConverters, Extensions etc.     | <ul><li>Atc</li><li>Atc.XamlToolkit</li><li>Atc.XamlToolkit.Wpf</li></ul>                                          |
| [![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.Controls.svg?label=Atc.Wpf.Controls&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/Atc.Wpf.Controls)                      | Miscellaneous UI Controls                           | <ul><li>Atc.Wpf</li><li>Atc.Wpf.Theming</li><li>Atc.XamlToolkit</li><li>Atc.XamlToolkit.Wpf</li></ul>                    |
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

# 🚀 How to get started with Atc's WPF

First of all, include Nuget packages in the `.csproj` file like this:

```xml
  <ItemGroup>
    <PackageReference Include="Atc.Wpf" Version="latest" />
    <PackageReference Include="Atc.Wpf.Controls" Version="latest" />
    <PackageReference Include="Atc.Wpf.FontIcons" Version="latest" />
    <PackageReference Include="Atc.Wpf.Theming" Version="latest" />
  </ItemGroup>
```

Then update `App.xaml` like this:

```xml
<Application
    x:Class="Atc.Wpf.Sample.App"
    xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
    [other namespaces]>
    <Application.Resources>
        <ResourceDictionary>

            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Theming;component/Styles/Default.xaml" />
                <ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Controls;component/Styles/Controls.xaml" />
            </ResourceDictionary.MergedDictionaries>

        </ResourceDictionary>
    </Application.Resources>
</Application>
```

Now it is possible to use controls with theming and default WPF controls like TextBox, Button etc. with theme style.

## 💝 WPF with MVVM Easily Separate UI and Business Logic

With the `Atc.Wpf`, package, it is very easy to get startet with the nice `MVVM pattern`

Please read more here:

- [MVVM framework](docs/Mvvm/@Readme.md)
  - [Observerble properties](docs/Mvvm/@Readme.md)
  - [RelayCommands](docs/Mvvm/@Readme.md)

# 📝 Readme's for each NuGet Package area

***Note: Right now, it is a limit amount of controls and components there is documented with a `Readme.md` file.
Therefore run the `Atc.Wpf.Sample` application to explore all the controls and components.*** 😊

## 💟 Atc.Wpf

### Controls

- Layouts
  - [GridEx](src/Atc.Wpf/Controls/Layouts/GridEx_Readme.md)
  - [StaggeredPanel](src/Atc.Wpf/Controls/Layouts/StaggeredPanel_Readme.md)
  - [UniformSpacingPanel](src/Atc.Wpf/Controls/Layouts/UniformSpacingPanel_Readme.md)
- Media
  - [SvgImage](src/Atc.Wpf/Controls/Media/SvgImage_Readme.md)
- Helpers
  - [PanelHelper](src/Atc.Wpf/Helpers/PanelHelper_Readme.md)

### Misc

- [ShaderEffects](src/Atc.Wpf/Media/ShaderEffects/@Readme.md)
  - [How to use HLSL Shader Compiler](src/Atc.Wpf/Media/ShaderEffects/Shaders/@Readme.md)
- [Tranlation & localizaion](src/Atc.Wpf/Translation/@Readme.md)
- [ValueConverters](src/Atc.Wpf/ValueConverters/@Readme.md)

## 💟 Atc.Wpf.Controls

### Controls

- [WellKnownColorPicker](src/Atc.Wpf.Controls/ColorControls/WellKnownColorPicker_Readme.md)

### Misc

- [ValueConverters](src/Atc.Wpf.Controls/ValueConverters/@Readme.md)

## 💟 Atc.Wpf.FontIcons

### Misc

- [ValueConverters](src/Atc.Wpf.FontIcons/ValueConverters/@Readme.md)

## 💟 Atc.Wpf.Theming

- [ValueConverters](src/Atc.Wpf.Theming/ValueConverters/@Readme.md)

# How to contribute

[Contribution Guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)

[Coding Guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)
