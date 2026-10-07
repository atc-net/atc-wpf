# 🖍️ WellKnownColorSelector

A drop-down selector for picking one of the well-known (named) WPF colors.

## 🔍 Overview

`WellKnownColorSelector` is a `UserControl` wrapping a `ComboBox` that lists the well-known colors with a color indicator, a localized display name and, optionally, the hex code. The selection is exposed as a color name through the two-way `SelectedKey` dependency property, and a `SelectorChanged` event is raised when the user picks a new color. Unlike `AccentColorSelector`, it does not change the application theme.

## 📍 Namespace

```csharp
using Atc.Wpf.Theming.Controls.Selectors;
```

## 🚀 Usage

### Basic Example

```xml
<atc:WellKnownColorSelector />
```

### With "Please select" First Item

```xml
<atc:WellKnownColorSelector DropDownFirstItemType="PleaseSelect" />
```

### Basic Colors Only

```xml
<atc:WellKnownColorSelector UseOnlyBasicColors="True" />
```

### Pre-selected and Bound

```xml
<atc:WellKnownColorSelector SelectedKey="LightBlue" />

<atc:WellKnownColorSelector SelectedKey="{Binding ColorName}" ShowHexCode="False" />
```

### Handling Changes in Code-Behind

```csharp
colorSelector.SelectorChanged += (sender, e) =>
{
    var colorName = e.NewValue;
};
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedKey` | `string` | `""` | Name of the selected color. Binds two-way by default (`UpdateSourceTrigger.LostFocus`) |
| `DefaultColorName` | `string` | `""` | Color selected on load when `SelectedKey` is empty |
| `DropDownFirstItemType` | `DropDownFirstItemType` | `None` | Optional first entry: `None`, `Blank`, `PleaseSelect` or `IncludeAll` |
| `RenderColorIndicatorType` | `RenderColorIndicatorType` | `Square` | Shape of the color indicator (`None`, `Circle`, `Square`) |
| `ShowHexCode` | `bool` | `true` | Show the hex code next to the color name |
| `UseOnlyBasicColors` | `bool` | `false` | Restrict the list to the basic colors |
| `Items` | `ObservableCollectionEx<ColorItem>` | - | Read-only collection of the listed colors |

## ⚡ Events

| Event | Args | Description |
|-------|------|-------------|
| `SelectorChanged` | `ValueChangedEventArgs<string?>` | Raised when the user selects a different color; `NewValue` is the color key |

## 📝 Notes

- Items are populated on `Loaded`; if `SelectedKey` is empty, `DefaultColorName` (or the first item) is selected
- `SelectorChanged` is not raised for the initial selection, for the first-item placeholder entries, or when the selection equals the previous one
- The list is re-translated when `CultureManager.UiCultureChanged` fires, keeping the current selection
- The control sets its own `DataContext` to itself

## 🔗 Related Controls

- **LabelWellKnownColorSelector** - Labeled form version with validation
- **WellKnownColorPicker** - Grid/palette-style picker for well-known colors
- **AccentColorSelector** - Changes the application's accent color

## 🎮 Sample Application

See the WellKnownColorSelector sample in the Atc.Wpf.Sample application under **Wpf.Theming > Input - Selector > WellKnownColorSelector** for interactive examples.
