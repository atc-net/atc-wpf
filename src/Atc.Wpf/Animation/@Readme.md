# Animation Library

## Overview

The Animation Library provides a centralized set of reusable animation utilities for WPF elements, including fade, slide, and scale effects. It supports both code-behind (async extension methods) and XAML-only (attached properties) usage patterns.

## Namespace

- Extension methods: `System.Windows` (for discoverability)
- Classes and enums: `Atc.Wpf.Animation`

## Usage

### Code-Behind (Async Extension Methods)

```csharp
// Fade animations
await myElement.FadeInAsync();
await myElement.FadeOutAsync();

// Slide animations
await myElement.SlideInFromAsync(SlideDirection.Left);
await myElement.SlideOutToAsync(SlideDirection.Bottom);

// Scale animations
await myElement.ScaleInAsync();
await myElement.ScaleOutAsync();

// With custom parameters
await myElement.FadeInAsync(new AnimationParameters
{
    Duration = TimeSpan.FromMilliseconds(500),
    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
    Delay = TimeSpan.FromMilliseconds(200),
});

// Using presets
await myElement.FadeInAsync(AnimationParameters.Fast);    // 150ms
await myElement.FadeInAsync(AnimationParameters.Default);  // 300ms
await myElement.FadeInAsync(AnimationParameters.Slow);     // 500ms

// Generic method
await myElement.AnimateAsync(AnimationKind.SlideInFromLeft);
```

### XAML Attached Properties

```xml
<Window xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas">

    <!-- Animate on load -->
    <Border atc:AnimateAttach.OnLoaded="FadeIn" />

    <!-- Animate when visibility changes to Visible -->
    <Border atc:AnimateAttach.OnVisible="SlideInFromBottom" />

    <!-- Custom duration and slide distance -->
    <Border
        atc:AnimateAttach.OnLoaded="SlideInFromLeft"
        atc:AnimateAttach.Duration="500"
        atc:AnimateAttach.SlideDistance="100" />
</Window>
```

## AnimationParameters Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Duration | TimeSpan | 300ms | Duration of the animation |
| EasingFunction | IEasingFunction? | CubicEase (EaseOut) | Easing function for the animation curve |
| Delay | TimeSpan | 0ms | Delay before the animation starts |
| SlideDistance | double | 50 | Distance in pixels for slide animations |

## AnimationParameters Presets

| Preset | Duration | Description |
|--------|----------|-------------|
| Default | 300ms | Standard animation speed |
| Fast | 150ms | Quick animations for subtle feedback |
| Slow | 500ms | Deliberate animations for emphasis |

## AnimationKind Enum

| Value | Description |
|-------|-------------|
| None | No animation |
| FadeIn | Fade from transparent to opaque |
| FadeOut | Fade from opaque to transparent |
| SlideInFromLeft | Slide in from the left edge |
| SlideInFromRight | Slide in from the right edge |
| SlideInFromTop | Slide in from the top edge |
| SlideInFromBottom | Slide in from the bottom edge |
| SlideOutToLeft | Slide out to the left edge |
| SlideOutToRight | Slide out to the right edge |
| SlideOutToTop | Slide out to the top edge |
| SlideOutToBottom | Slide out to the bottom edge |
| ScaleIn | Scale from zero to full size |
| ScaleOut | Scale from full size to zero |

## SlideDirection Enum

| Value | Description |
|-------|-------------|
| Left | Slide from or to the left |
| Right | Slide from or to the right |
| Top | Slide from or to the top |
| Bottom | Slide from or to the bottom |

## AnimateAttach Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| OnLoaded | AnimationKind | None | Animation to run when the element is loaded |
| OnVisible | AnimationKind | None | Animation to run each time the element becomes visible |
| Duration | double | 300 | Animation duration in milliseconds |
| SlideDistance | double | 50 | Slide distance in pixels |

## Notes

- All extension methods are async and return `Task`, allowing sequential animation composition.
- Fade-out and slide-out animations automatically set `Visibility = Collapsed` when complete.
- Fade-in, slide-in, and scale-in animations automatically set `Visibility = Visible` before starting.
- Transform helpers safely add transforms to existing TransformGroups without overwriting other transforms.
- The `AnimateAttach.OnLoaded` handler automatically unsubscribes after the first animation.

## Sample Application

Navigate to **Wpf > Animation > AnimationLibrary** in the sample application to see interactive demos of all animation types.
