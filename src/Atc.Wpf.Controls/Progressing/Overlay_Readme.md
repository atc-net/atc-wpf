# 🌫️ Overlay

A content dimming control that overlays child content with a semi-transparent layer and optional overlay content.

## 🔍 Overview

`Overlay` wraps content and displays a semi-transparent dimming layer when activated, with optional overlay content (e.g., a message, form, or loading indicator). It supports fade-in/fade-out animations with configurable durations and an auto-close feature that dismisses the overlay when the dimming area is clicked.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Progressing;
```

## 🚀 Usage

### Basic Overlay

```xml
<progressing:Overlay IsActive="{Binding IsOverlayVisible}">
    <progressing:Overlay.OverlayContent>
        <TextBlock Text="Processing..." FontSize="18" Foreground="White" />
    </progressing:Overlay.OverlayContent>

    <DataGrid ItemsSource="{Binding Items}" />
</progressing:Overlay>
```

### Auto-Close on Click

```xml
<progressing:Overlay IsActive="{Binding ShowOverlay}" AutoClose="True">
    <progressing:Overlay.OverlayContent>
        <Border Background="White" CornerRadius="8" Padding="24">
            <TextBlock Text="Click outside to dismiss" />
        </Border>
    </progressing:Overlay.OverlayContent>

    <ContentControl Content="{Binding MainContent}" />
</progressing:Overlay>
```

### 🎨 Custom Styling

```xml
<progressing:Overlay
    IsActive="{Binding IsActive}"
    OverlayBrush="Navy"
    OverlayOpacity="0.7"
    FadeInDuration="0:0:0.3"
    FadeOutDuration="0:0:0.2">
    <progressing:Overlay.OverlayContent>
        <progressing:LoadingIndicator Mode="Ring" CustomColorBrush="White" />
    </progressing:Overlay.OverlayContent>

    <ListView ItemsSource="{Binding Data}" />
</progressing:Overlay>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsActive` | `bool` | `false` | Whether the overlay is displayed |
| `OverlayBrush` | `Brush?` | `null` | Color of the dimming layer |
| `OverlayOpacity` | `double` | `0.5` | Opacity of the dimming layer (0.0-1.0) |
| `OverlayContent` | `object?` | `null` | Content displayed on top of the dimming layer |
| `OverlayContentTemplate` | `DataTemplate?` | `null` | Template for overlay content |
| `AutoClose` | `bool` | `false` | Close overlay when dimming area is clicked |
| `FadeInDuration` | `TimeSpan` | `200ms` | Duration of the fade-in animation |
| `FadeOutDuration` | `TimeSpan` | `150ms` | Duration of the fade-out animation |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `Activated` | `RoutedEventHandler` | Raised when the overlay becomes active |
| `Deactivated` | `RoutedEventHandler` | Raised when the overlay becomes inactive |

## 📝 Notes

- Animations use `CubicEase` easing function for smooth transitions
- `AutoClose` sets `IsActive` to false when the dimming rectangle is clicked
- Overlay content is centered on top of the dimming layer
- The child content remains in the visual tree but is covered by the dimming layer

## 🔗 Related Controls

- **BusyOverlay** - Overlay with built-in progress and localized messages
- **LoadingIndicator** - Animated loading spinner (can be used as OverlayContent)
- **Skeleton** - Placeholder content during loading

## 🎮 Sample Application

See the Overlay sample in the Atc.Wpf.Sample application under **Wpf.Controls > Progressing > Overlay** for interactive examples.
