# ⏳ LoadingIndicator

An animated indicator control with multiple visual styles for signaling ongoing operations.

## 🔍 Overview

`LoadingIndicator` provides animated loading visuals with three built-in styles: arcs ring, ring, and three dots. The animation speed is configurable, and the indicator can be activated or deactivated dynamically. A custom color brush allows matching the indicator to your application's theme.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Progressing;
```

## 🚀 Usage

### Basic Loading Indicator

```xml
<progressing:LoadingIndicator IsActive="True" Mode="ArcsRing" />
```

### Different Styles

```xml
<!-- Arcs ring (default) -->
<progressing:LoadingIndicator Mode="ArcsRing" />

<!-- Ring spinner -->
<progressing:LoadingIndicator Mode="Ring" />

<!-- Three bouncing dots -->
<progressing:LoadingIndicator Mode="ThreeDots" />
```

### 🎨 Custom Color and Speed

```xml
<progressing:LoadingIndicator
    IsActive="{Binding IsLoading}"
    Mode="Ring"
    CustomColorBrush="DodgerBlue"
    SpeedRatio="1.5" />
```

### Conditional Display

```xml
<progressing:LoadingIndicator
    IsActive="{Binding IsBusy}"
    Mode="ThreeDots" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsActive` | `bool` | `true` | Whether the animation is running |
| `Mode` | `LoadingIndicatorType` | `ArcsRing` | Visual style of the indicator |
| `CustomColorBrush` | `Brush?` | `null` | Override color for the indicator |
| `SpeedRatio` | `double` | `1.0` | Animation speed multiplier (higher = faster) |

## 📊 Enumerations

### LoadingIndicatorType

| Value | Description |
|-------|-------------|
| `ArcsRing` | Rotating arcs in a ring pattern (default) |
| `Ring` | Spinning ring indicator |
| `ThreeDots` | Three bouncing dots |

## 📝 Notes

- The indicator uses visual state manager for active/inactive states
- Animation storyboards are built into the control templates
- `SpeedRatio` adjusts the speed of all template storyboards
- Set `IsActive="False"` to pause the animation

## 🔗 Related Controls

- **BusyOverlay** - Full content overlay with loading indicator
- **Skeleton** - Placeholder content during loading
- **Overlay** - Content dimming overlay

## 🎮 Sample Application

See the LoadingIndicator sample in the Atc.Wpf.Sample application under **Wpf.Controls > Progressing > LoadingIndicator** for interactive examples.
