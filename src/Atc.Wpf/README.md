# Atc.Wpf

Core WPF library for the ATC.Net WPF stack: MVVM helpers, layouts, value converters, helpers, attached behaviors, navigation, hotkeys, and translation infrastructure. All other `Atc.Wpf.*` packages depend on it.

## What lives here

| Area | Path | Docs |
|---|---|---|
| MVVM (ViewModels, commands, messaging) | `Mvvm/`, re-exported from [`Atc.XamlToolkit`](https://github.com/atc-net/atc-xaml-toolkit) | [`docs/Mvvm/@Readme.md`](../../docs/Mvvm/@Readme.md) |
| Source generators (`[ObservableProperty]`, `[RelayCommand]`, `[DependencyProperty]`, `[AttachedProperty]`) | provided transitively via `Atc.XamlToolkit.Wpf` | [`docs/SourceGenerators/`](../../docs/SourceGenerators) |
| Layouts (`GridEx`, `AutoGrid`, `FlexPanel`, `StaggeredPanel`, `UniformSpacingPanel`, …) | `Controls/Layouts/` | [`docs/Layouts/@Readme.md`](../../docs/Layouts/@Readme.md) |
| Hotkeys (`HotkeyManager`, `KeyboardHook`) | `Hotkeys/` | [`docs/Hotkeys/@Readme.md`](../../docs/Hotkeys/@Readme.md) |
| Navigation (frame/page navigation primitives) | `Navigation/` | [`docs/Navigation/@Readme.md`](../../docs/Navigation/@Readme.md) |
| Translation (`CultureManager`, `ResxExtension`) | `Translation/` | — |
| Value converters (boolean, visibility, color, string, …) | `ValueConverters/` | — |
| Helpers (color, brush, file, image, geometry, …) | `Helpers/` | — |
| Factories (`BitmapImageFactory`, …) | `Factories/` | — |
| Collections (`ObservableDictionary`, `ObservableCollectionEx`) | `Collections/` | — |
| Threading (`DebounceDispatcher`) | `System/Windows/Threading/` | — |
| Internal SVG renderer (`W3cSvg`) | `Controls/Media/W3cSvg/` | — |

## Install

```xml
<PackageReference Include="Atc.Wpf" Version="2.*" />
```

## Notes

- `CultureManager.UiCultureChanged` is a **weak event**: subscribers are held via `WeakReference`, so long-lived static subscriptions do not root WPF controls. Lambda subscribers with closures must keep the delegate referenced themselves.
- This package targets `.NET 10.0-windows` and depends on `Atc` and `Atc.XamlToolkit.Wpf`.
