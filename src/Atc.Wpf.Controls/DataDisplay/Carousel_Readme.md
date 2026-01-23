# 🎠 Carousel

A rotating display control for showcasing images or content with navigation, indicators, and animations.

## 🔍 Overview

`Carousel` provides a modern, animated rotating display for images, cards, or any content. It supports navigation arrows, dot indicators, auto-play functionality, multiple transition animations, infinite loop behavior, drag/swipe navigation, and full keyboard accessibility.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## 🚀 Usage

### Basic Carousel

```xml
<dataDisplay:Carousel Width="500" Height="300" CornerRadius="8">
    <Border Background="#4A90D9">
        <TextBlock Text="Slide 1" HorizontalAlignment="Center" VerticalAlignment="Center" />
    </Border>
    <Border Background="#7B68EE">
        <TextBlock Text="Slide 2" HorizontalAlignment="Center" VerticalAlignment="Center" />
    </Border>
    <Border Background="#20B2AA">
        <TextBlock Text="Slide 3" HorizontalAlignment="Center" VerticalAlignment="Center" />
    </Border>
</dataDisplay:Carousel>
```

### 🔄 Auto-Play Carousel

```xml
<dataDisplay:Carousel
    AutoPlay="True"
    AutoPlayInterval="3000"
    PauseOnHover="True"
    TransitionType="Fade">
    <!-- Slides -->
</dataDisplay:Carousel>
```

### 🃏 Carousel with Custom Cards

```xml
<dataDisplay:Carousel TransitionType="SlideAndFade">
    <dataDisplay:Card Header="Product A" Elevation="3" Margin="20">
        <StackPanel Margin="10">
            <TextBlock Text="Premium Widget" FontSize="18" />
            <TextBlock Text="$99.99" FontSize="24" FontWeight="Bold" />
        </StackPanel>
    </dataDisplay:Card>
    <dataDisplay:Card Header="Product B" Elevation="3" Margin="20">
        <StackPanel Margin="10">
            <TextBlock Text="Standard Widget" FontSize="18" />
            <TextBlock Text="$49.99" FontSize="24" FontWeight="Bold" />
        </StackPanel>
    </dataDisplay:Card>
</dataDisplay:Carousel>
```

### 🎨 Custom Styling

```xml
<!-- Custom indicator colors, no navigation arrows -->
<dataDisplay:Carousel
    ShowNavigationArrows="False"
    IndicatorActiveBrush="#FF6347"
    IndicatorInactiveBrush="#FFB6C1"
    IndicatorSize="12"
    IndicatorSpacing="12"
    IndicatorPosition="Top">
    <!-- Slides -->
</dataDisplay:Carousel>
```

### 🔚 Bounded Mode (Non-Infinite Loop)

```xml
<dataDisplay:Carousel IsInfiniteLoop="False">
    <!-- Previous button disabled at first slide, Next disabled at last -->
</dataDisplay:Carousel>
```

### 📋 Data Binding with ItemTemplate

```xml
<dataDisplay:Carousel ItemsSource="{Binding Images}">
    <dataDisplay:Carousel.ItemTemplate>
        <DataTemplate>
            <Image Source="{Binding Url}" Stretch="UniformToFill" />
        </DataTemplate>
    </dataDisplay:Carousel.ItemTemplate>
</dataDisplay:Carousel>
```

## ⚙️ Properties

### Navigation Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowNavigationArrows` | `bool` | `true` | Show/hide the previous and next navigation buttons |
| `ShowIndicators` | `bool` | `true` | Show/hide the dot indicators |
| `IndicatorPosition` | `IndicatorPosition` | `Bottom` | Position of indicators (Bottom, Top, Left, Right) |

### Auto-Play Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AutoPlay` | `bool` | `false` | Enable automatic slide advancement |
| `AutoPlayInterval` | `double` | `5000` | Interval between slides in milliseconds |
| `PauseOnHover` | `bool` | `true` | Pause auto-play when mouse hovers over the carousel |

### Behavior Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsInfiniteLoop` | `bool` | `true` | Enable infinite looping (wrap from last to first and vice versa) |
| `TransitionType` | `CarouselTransitionType` | `Slide` | Animation type (None, Slide, Fade, SlideAndFade) |
| `TransitionDuration` | `double` | `300` | Animation duration in milliseconds |
| `IsDragEnabled` | `bool` | `true` | Enable drag/swipe navigation |
| `DragThreshold` | `double` | `0.2` | Minimum drag distance (percentage of width) to trigger navigation |

### Styling Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IndicatorActiveBrush` | `Brush` | Theme accent | Brush for active (selected) indicator |
| `IndicatorInactiveBrush` | `Brush` | Gray | Brush for inactive indicators |
| `IndicatorSize` | `double` | `10` | Diameter of indicator dots |
| `IndicatorSpacing` | `double` | `8` | Spacing between indicators |
| `CornerRadius` | `CornerRadius` | `0` | Corner radius for the carousel container |

## 📊 Enumerations

### CarouselTransitionType

| Value | Description |
|-------|-------------|
| `None` | Instant change with no animation |
| `Slide` | Horizontal slide animation |
| `Fade` | Crossfade animation |
| `SlideAndFade` | Combined slide and fade animation |

### IndicatorPosition

| Value | Description |
|-------|-------------|
| `Bottom` | Indicators at the bottom center |
| `Top` | Indicators at the top center |
| `Left` | Indicators on the left side (vertical) |
| `Right` | Indicators on the right side (vertical) |

## 📡 Events

| Event | Args | Description |
|-------|------|-------------|
| `SlideChanged` | `CarouselSlideChangedEventArgs` | Fired after a slide change completes |
| `SlideChanging` | `CarouselSlideChangingEventArgs` | Fired before a slide change begins (cancelable) |

### Event Args

**CarouselSlideChangedEventArgs:**
- `OldIndex` - The previous slide index
- `NewIndex` - The new slide index

**CarouselSlideChangingEventArgs:**
- `CurrentIndex` - The current slide index
- `TargetIndex` - The target slide index
- `Cancel` - Set to `true` to cancel the navigation

## 🔧 Methods

| Method | Description |
|--------|-------------|
| `Previous()` | Navigate to the previous slide |
| `Next()` | Navigate to the next slide |
| `GoToSlide(int index)` | Navigate to a specific slide by index |

## ⌨️ Keyboard Navigation

| Key | Action |
|-----|--------|
| Left Arrow | Navigate to previous slide |
| Right Arrow | Navigate to next slide |
| Home | Navigate to first slide |
| End | Navigate to last slide |

## 👆 Drag/Swipe Navigation

When `IsDragEnabled` is `true`, users can drag left or right to navigate between slides. The navigation is triggered when the drag distance exceeds `DragThreshold` (default 20% of carousel width).

## 📝 Notes

- The carousel inherits from `Selector`, providing `SelectedIndex`, `SelectedItem`, and `Items` properties
- When `IsInfiniteLoop` is `false`, navigation buttons are disabled at the boundaries
- Auto-play pauses when dragging regardless of `PauseOnHover` setting
- Transitions are queued - rapid clicks won't cause animation overlap
- Use `TransitionType="None"` for instant switching (useful for testing)

## 🔗 Related Controls

- **Card** - Can be used as carousel slide content
- **Badge** - Can overlay carousel for notification counts
- **ResponsivePanel** - Alternative for responsive grid layouts

## 🎮 Sample Application

See the Carousel sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Carousel** for interactive examples.
