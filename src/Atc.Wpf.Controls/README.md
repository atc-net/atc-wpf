# Atc.Wpf.Controls

Atomic / primitive WPF controls that act as the building blocks for the higher-level `Atc.Wpf.Forms` and `Atc.Wpf.Components` libraries. Includes 14+ base controls, button styles, color controls, data-display controls, layouts, selectors, progressing controls (`BusyOverlay`, `LoadingIndicator`, `Skeleton`), and the `ZoomBox` family.

## What lives here

| Area | Path | Docs |
|---|---|---|
| Base controls (`NumericBox`, `IntegerBox`, `DecimalBox`, `EndpointBox`, `DateTimePicker`, `TimePicker`, `ColorPicker`, `ToggleSwitch`, …) | `BaseControls/` | per-control `_Readme.md` |
| Buttons (variants: standard, chromeless, icon, badge, …) | `Buttons/` | per-control `_Readme.md` |
| Color controls (`HueSlider`, `SaturationBrightnessPicker`, `TransparencySlider`, `WellKnownColorPicker`, `ColorBox`) | `Colors/` | per-control `_Readme.md` |
| Data-display controls (`Avatar`, `AvatarGroup`, `Badge`, `Breadcrumb`, `Card`, `Carousel`, `Chip`, `Divider`, `Popover`, `Segmented`, `Timeline`) | `DataDisplay/` | [`docs/DataDisplay/@Readme.md`](../../docs/DataDisplay/@Readme.md) |
| Layouts (`VirtualizingStaggeredPanel`, `MasonryPanel`, …) | `Layouts/` | [`docs/Layouts/@Readme.md`](../../docs/Layouts/@Readme.md) |
| Progressing (`BusyOverlay`, `LoadingIndicator`, `Skeleton`, `Overlay`) | `Progressing/` | per-control `_Readme.md` |
| Selectors (`AccentColorSelector`, `CountrySelector`, `LanguageSelector`, `ThemeSelector`, `WellKnownColorSelector`, …) | `Selectors/` | per-control `_Readme.md` |
| Pickers (`DirectoryPicker`, `FilePicker`, `FontPicker`, `AdvancedFontPicker`) | `Pickers/` | per-control `_Readme.md` |
| Zoom (`ZoomBox`, `ZoomMiniMap`, mouse/touch handlers, undo/redo glue) | `Zoom/` | per-control `_Readme.md` |
| Localization resources (en / da / de) | `Resources/` | — |

## Install

```xml
<PackageReference Include="Atc.Wpf.Controls" Version="2.*" />
```

Add the merged dictionary to your `App.xaml` so the styles load:

```xml
<ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Controls;component/Styles/Controls.xaml" />
```

## Dependencies

- [`Atc.Wpf`](../Atc.Wpf/README.md) — core
- [`Atc.Wpf.Theming`](../Atc.Wpf.Theming/README.md) — theme resources
- [`Atc.Wpf.FontIcons`](../Atc.Wpf.FontIcons/README.md) — icons
