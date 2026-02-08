# 📏 RangeSlider

A dual-thumb slider for selecting a value range within a minimum and maximum.

## 🔍 Overview

`RangeSlider` provides two draggable thumbs on a track for selecting a range of values (e.g., a price range or date range). It supports step snapping, value tooltips, customizable track and thumb styling, and full keyboard navigation with Shift for accelerated movement. The highlighted range between the two thumbs is visually distinct.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Inputs;
```

## 🚀 Usage

### Basic Range Slider

```xml
<inputs:RangeSlider
    Minimum="0"
    Maximum="100"
    RangeStart="{Binding MinPrice}"
    RangeEnd="{Binding MaxPrice}" />
```

### With Step Snapping

```xml
<inputs:RangeSlider
    Minimum="0"
    Maximum="1000"
    Step="50"
    RangeStart="200"
    RangeEnd="800" />
```

### Custom Formatting

```xml
<!-- Currency format in tooltips -->
<inputs:RangeSlider
    Minimum="0"
    Maximum="500"
    RangeStart="100"
    RangeEnd="400"
    ToolTipFormat="C0"
    ShowValueToolTips="True" />
```

### 🎨 Custom Styling

```xml
<inputs:RangeSlider
    Minimum="0"
    Maximum="100"
    RangeStart="25"
    RangeEnd="75"
    TrackBackground="LightGray"
    RangeHighlightBrush="DodgerBlue"
    ThumbBrush="White"
    ThumbHoverBrush="AliceBlue"
    ThumbPressedBrush="LightBlue"
    ThumbSize="24"
    TrackHeight="6" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Minimum` | `double` | `0.0` | Minimum allowed value |
| `Maximum` | `double` | `100.0` | Maximum allowed value |
| `RangeStart` | `double` | `0.0` | Lower bound of selected range (two-way bindable) |
| `RangeEnd` | `double` | `100.0` | Upper bound of selected range (two-way bindable) |
| `Step` | `double` | `1.0` | Snap increment for values |
| `ShowValueToolTips` | `bool` | `true` | Show tooltips with current values on thumbs |
| `ToolTipFormat` | `string` | `"N0"` | .NET format string for tooltip values |
| `TrackBackground` | `Brush?` | `null` | Background brush for the track |
| `RangeHighlightBrush` | `Brush?` | `null` | Brush for the highlighted range area |
| `ThumbBrush` | `Brush?` | `null` | Default thumb color |
| `ThumbHoverBrush` | `Brush?` | `null` | Thumb color on hover |
| `ThumbPressedBrush` | `Brush?` | `null` | Thumb color when pressed |
| `ThumbSize` | `double` | `20.0` | Diameter of thumb controls |
| `TrackHeight` | `double` | `4.0` | Height of the track bar |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `RangeChanged` | `RoutedEventHandler` | Raised when either RangeStart or RangeEnd changes |

## ⌨️ Keyboard Navigation

| Key | Action |
|-----|--------|
| `Left` / `Down` | Decrease focused thumb by one step |
| `Right` / `Up` | Increase focused thumb by one step |
| `Shift + Arrow` | Move by 10x step increment |
| `Home` | Set focused thumb to minimum/start boundary |
| `End` | Set focused thumb to maximum/end boundary |

## 📝 Notes

- `RangeStart` is always coerced to be less than or equal to `RangeEnd`
- Both range values are clamped within `Minimum` and `Maximum`
- Values snap to the nearest `Step` increment during drag
- Click a thumb to give it keyboard focus, then use arrow keys
- Both `RangeStart` and `RangeEnd` support two-way binding by default

## 🔗 Related Controls

- **Rating** - Interactive value selection
- **ToggleSwitch** - Binary on/off selection

## 🎮 Sample Application

See the RangeSlider sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > RangeSlider** for interactive examples.
