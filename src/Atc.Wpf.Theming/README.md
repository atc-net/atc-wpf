# Atc.Wpf.Theming

Light / Dark theme infrastructure for the `Atc.Wpf.*` stack: theme manager, accent-color palette, themed default styles for the standard WPF controls, and the `NiceWindow` chrome. Built on `ControlzEx`.

## What lives here

| Area | Path | Highlights |
|---|---|---|
| Theme manager | `ThemeManager/` | `ThemeManager`, accent + base color application, runtime switching |
| Themed standard controls | `Themes/`, `Controls/` | re-styled `Button`, `TextBox`, `CheckBox`, `RadioButton`, `ComboBox`, `Slider`, `ScrollBar`, etc. |
| `NiceWindow` chrome | `Controls/Windows/NiceWindow.cs` | drop-in replacement for `Window` with custom chrome and theme awareness |
| Decorators | `Decorators/` | `ClipBorder` and other clipping/border decorators |
| Theme resource dictionaries | `Themes/Light/`, `Themes/Dark/`, `Themes/Generic.xaml` | per-theme palette definitions |

## Install

```xml
<PackageReference Include="Atc.Wpf.Theming" Version="2.*" />
```

Add the merged dictionary to your `App.xaml`:

```xml
<ResourceDictionary>
  <ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Theming;component/Styles/Default.xaml" />
  </ResourceDictionary.MergedDictionaries>
</ResourceDictionary>
```

Or pick the theme + accent at runtime:

```csharp
ThemeManager.ChangeTheme("Dark.Blue");
```

## Docs

- Theming overview & accent-color palette: [`docs/Theming/@Readme.md`](../../docs/Theming/@Readme.md)

## Dependencies

- [`Atc.Wpf`](../Atc.Wpf/README.md) — core
- `ControlzEx` (window chrome primitives)
- `Microsoft.Windows.CsWin32` (Win32 P/Invoke for window chrome)
