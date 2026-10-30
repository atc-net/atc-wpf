# Atc.Wpf.FontIcons

Font-based icon controls for WPF: Font Awesome 5 and 7, Bootstrap Glyphicons, Material Design, Weather Icons and IcoFont, as `TextBlock`-based (`Font*`) and `Image`-based (`Image*`) controls that stay crisp at any DPI.

Part of [ATC.Net WPF](https://github.com/atc-net/atc-wpf), a set of WPF libraries for building MVVM desktop applications on .NET 10 (Windows).

## Installation

```shell
dotnet add package Atc.Wpf.FontIcons
```

## Usage

```xml
<Window xmlns:fontIcons="clr-namespace:Atc.Wpf.FontIcons;assembly=Atc.Wpf.FontIcons">
    <StackPanel Orientation="Horizontal">
        <fontIcons:FontAwesomeSolid7 Icon="Home" FontSize="24" />
        <fontIcons:ImageMaterialDesign Icon="ContentCopy" Width="20" Height="20" />
    </StackPanel>
</Window>
```

## Documentation

- [Font icons guide](https://github.com/atc-net/atc-wpf/blob/main/docs/FontIcons/%40Readme.md)
- [Documentation site](https://atc-net.github.io/atc-wpf/)
- [Repository README](https://github.com/atc-net/atc-wpf#readme)
- [Sample application](https://github.com/atc-net/atc-wpf/tree/main/sample/Atc.Wpf.Sample) - a control explorer showing every control with its XAML and code
