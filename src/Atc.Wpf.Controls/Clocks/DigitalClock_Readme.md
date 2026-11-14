# ⏰ DigitalClock

A digital clock built from seven-segment digits, showing hours, minutes and (optionally) seconds with blinking colons and a glow.

## 🔍 Overview

`DigitalClock` lines up six `SevenSegment` digits (`HH:MM:SS`) in a scalable layout: it fills the space it gets and keeps its proportions. By default it follows the current local time: a timer updates it just after each whole second while the clock is loaded, and stops when the clock is unloaded. Turn `FollowCurrentTime` off to show a fixed or bound `Time` instead, for example an elapsed time or a countdown.

Unset brushes use the theme accent brush (`AtcApps.Brushes.Accent`), so the clock follows the current theme and accent colour.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Clocks;
```

## 🚀 Usage

### Basic Example

```xml
<atc:DigitalClock Height="60" />
```

### ⏱️ Showing a Given Time

```xml
<atc:DigitalClock
    Height="60"
    FollowCurrentTime="False"
    ShowSeconds="True"
    Time="{Binding Elapsed}" />
```

```csharp
clock.FollowCurrentTime = false;
clock.SetTime(TimeSpan.FromMinutes(90)); // shows 01:30:00
```

### 🎨 Custom Look

```xml
<atc:DigitalClock
    Height="60"
    AnimateColon="False"
    ColonColor="Brown"
    NumberBlurRadius="10"
    NumberColor="Chocolate"
    ShowSeconds="False" />
```

### 💾 Saving the Look

```csharp
DigitalClockConfigData data = clock.GetConfigData();   // colours as #AARRGGBB text, plus the options
otherClock.SetByConfigData(data);                       // an empty colour restores the theme brush
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Time` | `TimeSpan` | `00:00:00` | The time of day shown (wrapped into 0:00:00 - 23:59:59) |
| `FollowCurrentTime` | `bool` | `true` | Show the current local time, updated every second while loaded |
| `ShowSeconds` | `bool` | `true` | Show the seconds |
| `ShowColon` | `bool` | `true` | Show the colons between hours, minutes and seconds |
| `AnimateColon` | `bool` | `true` | Fade the colons in and out on each change of `Time` |
| `PlayAnimation` | `bool` | `true` | Fade the segments when a digit changes |
| `NumberBlurRadius` | `int` | `5` | Blur radius of the glow behind the digits (0 = no glow) |
| `NumberColor` | `Brush?` | Theme accent | The digits |
| `ColonColor` | `Brush?` | Theme accent | The colons |

## 🧰 Methods

| Method | Description |
|--------|-------------|
| `SetTime(TimeSpan)` | Sets `Time` |
| `GetConfigData()` | Returns the colours and options as `DigitalClockConfigData` |
| `SetByConfigData(DigitalClockConfigData)` | Applies colours and options; an empty colour restores the theme brush |

## 📝 Notes

- The timer only runs while the clock is loaded and `FollowCurrentTime` is on; it stops on `Unloaded`, so a clock in a closed view does not keep ticking
- While `FollowCurrentTime` is on, the next tick replaces a `Time` set from code or a binding
- The clock always shows 24-hour time
- The colon after the minutes is only shown when `ShowSeconds` is on

## 🔗 Related Controls

- **SevenSegment** - The single digit the clock is built from
- **AnalogClock** - Analog clock face

## 🎮 Sample Application

See the DigitalClock sample in the Atc.Wpf.Sample application under **Wpf.Controls > Clocks > DigitalClock** for interactive examples.
