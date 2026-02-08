# ⭐ Rating

An interactive star-rating control with configurable icons, half-star support, and hover preview.

## 🔍 Overview

`Rating` displays a row of icons (stars by default) that users can click to set a rating value. It supports half-star ratings, hover preview highlighting, custom icon types (text, SVG, FontAwesome, Material Design, and more), and keyboard navigation. The control includes an automation peer for accessibility.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Inputs;
```

## 🚀 Usage

### Basic Star Rating

```xml
<inputs:Rating Value="{Binding UserRating}" />
```

### Half-Star Support

```xml
<inputs:Rating Value="{Binding Rating}" AllowHalfStars="True" Maximum="5" />
```

### Custom Scale

```xml
<!-- 10-star rating -->
<inputs:Rating Value="{Binding Score}" Maximum="10" ItemSize="20" ItemSpacing="2" />
```

### 🎨 Custom Colors

```xml
<inputs:Rating
    Value="3.5"
    AllowHalfStars="True"
    FilledBrush="Gold"
    EmptyBrush="LightGray"
    PreviewBrush="Orange" />
```

### Read-Only Display

```xml
<inputs:Rating Value="{Binding AverageRating}" IsReadOnly="True" />
```

### Custom Icons

```xml
<!-- Custom text icons -->
<inputs:Rating FilledIcon="♥" EmptyIcon="♡" HalfFilledIcon="♥" FilledBrush="Red" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `double` | `0.0` | Current rating value (two-way bindable) |
| `Maximum` | `int` | `5` | Maximum number of rating items |
| `AllowHalfStars` | `bool` | `false` | Enable half-star (0.5 increment) ratings |
| `IsReadOnly` | `bool` | `false` | Disable user interaction |
| `ShowPreviewOnHover` | `bool` | `true` | Show preview highlighting on mouse hover |
| `FilledIcon` | `object` | `"★"` | Icon for filled items |
| `EmptyIcon` | `object` | `"☆"` | Icon for empty items |
| `HalfFilledIcon` | `object` | `"★"` | Icon for half-filled items |
| `ItemSize` | `double` | `24.0` | Width and height of each item |
| `ItemSpacing` | `double` | `4.0` | Spacing between items |
| `FilledBrush` | `Brush?` | `null` | Color for filled items |
| `EmptyBrush` | `Brush?` | `null` | Color for empty items |
| `PreviewBrush` | `Brush?` | `null` | Color for hover preview items |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `ValueChanged` | `RoutedPropertyChangedEventHandler<double>` | Raised when the rating value changes |

## ⌨️ Keyboard Navigation

| Key | Action |
|-----|--------|
| `Right` / `Up` | Increase rating by 1 (or 0.5 with half-stars) |
| `Left` / `Down` | Decrease rating by 1 (or 0.5 with half-stars) |
| `Home` | Set to minimum (0) |
| `End` | Set to maximum |

## 📝 Notes

- Icons support multiple types: plain text, SVG paths, ImageSource, and all FontIcon types (FontAwesome, Material Design, Bootstrap, IcoFont, Weather)
- `Value` supports two-way binding by default
- Half-star calculation is based on click position within each item
- Hover preview only appears when `ShowPreviewOnHover` is true and `IsReadOnly` is false

## 🔗 Related Controls

- **RangeSlider** - Slider-based value selection
- **ToggleSwitch** - Binary on/off selection

## 🎮 Sample Application

See the Rating sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > Rating** for interactive examples.
