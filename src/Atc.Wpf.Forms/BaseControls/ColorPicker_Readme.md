# 🎨 ColorPicker

A compact color field that shows the current color and hex code, and opens a color dialog on demand.

## 🔍 Overview

`ColorPicker` is a small inline control made of a color indicator (square or circle), the color's hex code and a palette button. Clicking the button opens the `ColorPickerDialogBox` (built on `AdvancedColorPicker`). When the dialog is confirmed, `ColorValue` and `BrushValue` are updated and the `ColorChanged` event is raised.

`ColorValue`, `BrushValue` and `DisplayHexCode` are kept in sync: setting either the color or the brush updates the other two.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms.BaseControls;
```

## 🚀 Usage

### Basic Example

```xml
<atc:ColorPicker ColorValue="Green" />
```

### Bind to a Brush

```xml
<atc:ColorPicker BrushValue="Blue" />
```

### Circle Indicator

```xml
<atc:ColorPicker ColorValue="DarkMagenta" RenderColorIndicatorType="Circle" />
```

### Two-Way Binding to a ViewModel

```xml
<atc:ColorPicker x:Name="MyColorPicker" ColorValue="{Binding ColorValue}" />
<Rectangle Fill="{Binding ElementName=MyColorPicker, Path=BrushValue}" />
```

### Handle Color Changes in Code

```csharp
myColorPicker.ColorChanged += (sender, e) =>
{
    var newColor = e.NewValue;
};
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ColorValue` | `Color?` | `Black` | The selected color. Updates `BrushValue` and `DisplayHexCode` when changed |
| `BrushValue` | `SolidColorBrush?` | `Black` | The selected color as a brush. Updates `ColorValue` and `DisplayHexCode` when changed |
| `DisplayHexCode` | `string?` | `#FF000000` | Hex code shown next to the indicator. Binds two-way by default |
| `RenderColorIndicatorType` | `RenderColorIndicatorType` | `Square` | Shape of the color indicator: `None`, `Circle` or `Square` |

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `ColorChanged` | `EventHandler<ValueChangedEventArgs<Color>>` | Raised after a color is confirmed in the color dialog |

## 📝 Notes

- The color dialog is opened with `Application.Current.MainWindow` as owner
- `ColorChanged` is only raised from the dialog flow, not when `ColorValue` or `BrushValue` is set from code or a binding
- The `OldValue` of the `ColorChanged` event args is the color before the dialog was confirmed (`Colors.Transparent` when `ColorValue` was `null`)
- `RenderColorIndicatorType="None"` hides the indicator and shows only the hex code and button
- The control sets its own `DataContext` to itself; bind its properties from the outside as shown above

## 🔗 Related Controls

- **LabelColorPicker** - Labeled form-field version with validation and mandatory indicator
- **AdvancedColorPicker** - Full color editor (hue, saturation/brightness, transparency, well-known colors) used inside the dialog
- **WellKnownColorPicker** - Pick from a list of named colors
- **WellKnownColorSelector** - Drop-down selector for named colors

## 🎮 Sample Application

See the ColorPicker sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > ColorPicker** for interactive examples.
