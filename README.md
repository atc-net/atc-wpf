[![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.svg?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/atc.wpf)

# ATC.Net WPF

This is a base libraries for building WPF application with the MVVM design pattern.

# Table of contents

- [ATC.Net WPF](#atcnet-wpf)
- [Table of contents](#table-of-contents)
  - [Requirements](#requirements)
  - [NuGet Packages Provided in this Repository](#nuget-packages-provided-in-this-repository)
  - [Demonstration Application](#demonstration-application)
    - [Playground and Viewer for a Given Control or Functionality](#playground-and-viewer-for-a-given-control-or-functionality)
    - [Initial glimpse at the demonstration application](#initial-glimpse-at-the-demonstration-application)
  - [How to get started with atc-wpf](#how-to-get-started-with-atc-wpf)
  - [Readme's for each NuGet Package area](#readmes-for-each-nuget-package-area)
    - [Atc.Wpf](#atcwpf)
      - [Controls](#controls)
      - [Misc](#misc)
    - [Atc.Wpf.Controls](#atcwpfcontrols)
      - [Controls](#controls-1)
      - [Misc](#misc-1)
    - [Atc.Wpf.FontIcons](#atcwpffonticons)
      - [Misc](#misc-2)
    - [Atc.Wpf.Theming](#atcwpftheming)
  - [How to contribute](#how-to-contribute)

## Requirements

[.NET 9 - Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## NuGet Packages Provided in this Repository

| Nuget package           | Description                                         | Dependencies                   |
|-------------------------|-----------------------------------------------------|--------------------------------|
| Atc.Wpf                 | Base Controls, ValueConverters, Extensions etc.     | Atc & Atc.Wpf.SourceGenerators |
| Atc.Wpf.Controls        | Miscellaneous UI Controls                           | Atc.Wpf & Atc.Wpf.Theming      |
| Atc.Wpf.Controls.Sample | Controls for creating WPF sample apps               | Atc.Wpf & Atc.Wpf.Theming      |
| Atc.Wpf.FontIcons       | Render Svg and Img resources based on fonts         | Atc.Wpf                        |
| Atc.Wpf.Theming         | Theming for Light & Dark mode for WPF base controls | Atc.Wpf                        |

## Demonstration Application

The demonstration application, `Atc.Wpf.Sample`, functions as a control explorer.
It provides quick visualization of a given control, along with options for
copying and pasting the XAML markup and/or the C# code for how to use it.

### Playground and Viewer for a Given Control or Functionality

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

### Initial glimpse at the demonstration application

| Light-Mode                                                                   | Dark-Mode                                                                    |
|------------------------------------------------------------------------------|------------------------------------------------------------------------------|
| Wpf - AutoGrid ![Img](docs/images/lm-wpf-autogrid.png)                       | Wpf - AutoGrid ![Img](docs/images/dm-wpf-autogrid.png)                       |
| Wpf.Controls - Label MIX ![Img](docs/images/lm-wpf-controls-label-mix.png)   | Wpf.Controls - Label MIX ![Img](docs/images/dm-wpf-controls-label-mix.png)   |
| Wpf.Theming - ImageButton ![Img](docs/images/lm-wpf-theming-imagebutton.png) | Wpf.Theming - ImageButton ![Img](docs/images/dm-wpf-theming-imagebutton.png) |
| Wpf.FontIcons - Viewer ![Img](docs/images/lm-wpf-fonicons-viewer.png)        | Wpf.FontIcons - Viewer ![Img](docs/images/dm-wpf-fonicons-viewer.png)        |

## How to get started with atc-wpf

First of all, include Nuget packages in the `.csproj` file like this:

```xml
  <ItemGroup>
    <PackageReference Include="Atc.Wpf" Version="2.0.178" />
    <PackageReference Include="Atc.Wpf.Controls" Version="2.0.178" />
    <PackageReference Include="Atc.Wpf.FontIcons" Version="2.0.178" />
    <PackageReference Include="Atc.Wpf.Theming" Version="2.0.178" />
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

## Readme's for each NuGet Package area

***Note: Right now, it is a limit amount of controls and components there is documented with a `Readme.md` file.
Therefore run the `Atc.Wpf.Sample` application to explore all the controls and components.*** :-)

### Atc.Wpf

#### Controls

- [GridEx](src/Atc.Wpf/Controls/Layouts/GridEx_Readme.md)
- [StaggeredPanel](src/Atc.Wpf/Controls/Layouts/StaggeredPanel_Readme.md)
- [UniformSpacingPanel](src/Atc.Wpf/Controls/Layouts/UniformSpacingPanel_Readme.md)
- [SvgImage](src/Atc.Wpf/Controls/Media/SvgImage_Readme.md)
- Control Helpers
  - [PanelHelper](src/Atc.Wpf/Helpers/PanelHelper_Readme.md)

#### Misc

- [MVVM framework](src/Atc.Wpf/Mvvm/@Readme.md)
  - [RelayCommand's](src/Atc.Wpf/Command/@Readme.md)
- [ShaderEffects](src/Atc.Wpf/Media/ShaderEffects/@Readme.md)
  - [How to use HLSL Shader Compiler](src/Atc.Wpf/Media/ShaderEffects/Shaders/@Readme.md)
- [Tranlation & localizaion](src/Atc.Wpf/Translation/@Readme.md)
- [ValueConverters](src/Atc.Wpf/ValueConverters/@Readme.md)

### Atc.Wpf.Controls

#### Controls

- [WellKnownColorPicker](src/Atc.Wpf.Controls/ColorControls/WellKnownColorPicker_Readme.md)

#### Misc

- [ValueConverters](src/Atc.Wpf.Controls/ValueConverters/@Readme.md)

### Atc.Wpf.FontIcons

#### Misc

- [ValueConverters](src/Atc.Wpf.FontIcons/ValueConverters/@Readme.md)

### Atc.Wpf.Theming

- [ValueConverters](src/Atc.Wpf.Theming/ValueConverters/@Readme.md)

### Source Generators

In MVVM, certain attributes help automate boilerplate code using source generators.

Example for ViewModel classes  

![MVVM Source Generation](docs/images/mvvm-source-generated.png)

For more details, see the [MVVM](src/Atc.Wpf/Mvvm/@Readme.md) section.

### DependencyProperty

### Human made Code for complex property

```csharp
[DependencyProperty<bool>("IsRunning")]
public partial class MyControl : UserControl
{
}
```

### Human made Code for simple property

```csharp
[DependencyProperty<bool>("IsRunning"]
public partial class MyControl : UserControl
{
}
```

### Generated Code for simple property

```csharp
public partial class MyControl
{
    public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(
        nameof(IsRunning),
        typeof(bool),
        typeof(MyControl),
        new FrameworkPropertyMetadata(defaultValue: BooleanBoxes.TrueBox);
            
    public bool IsRunning
    {
        get => (bool)GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }
}
```

### Generated Code for complex property

```csharp
public partial class MyControl
{
    public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(
        nameof(IsRunning),
        typeof(bool),
        typeof(MyControl),
        new FrameworkPropertyMetadata(
            defaultValue: BooleanBoxes.TrueBox,
            propertyChangedCallback: PropertyChangedCallback,
            coerceValueCallback: CoerceValueCallback,
            flags: FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
            defaultUpdateSourceTrigger: UpdateSourceTrigger.Default,
            isAnimationProhibited: true));
            
    public bool IsRunning
    {
        get => (bool)GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }
}
```

## How to contribute

[Contribution Guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)

[Coding Guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)
