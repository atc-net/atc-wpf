# 🕰️ AnalogClock

An analog clock face with hour, minute and second hands, hour and minute indicators, the numbers 1-12 and an AM/PM indicator.

## 🔍 Overview

`AnalogClock` draws a scalable clock face (it fills the space it gets and keeps its round shape). By default it follows the current local time: a timer updates it just after each whole second while the clock is loaded, and stops when the clock is unloaded. Turn `FollowCurrentTime` off to show a fixed or bound `Time` instead, for example a time in another time zone or a countdown.

Every part has its own brush. Unset brushes follow the current theme (`AtcApps.Brushes.ThemeBackground` for the face, `AtcApps.Brushes.ThemeForeground` for the ring, indicators, numbers and hands, `AtcApps.Brushes.Accent` for the second hand), so the clock switches with the light and dark themes.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Clocks;
```

## 🚀 Usage

### Basic Example

```xml
<atc:AnalogClock Width="200" Height="200" />
```

### 🕑 Showing a Given Time

```xml
<atc:AnalogClock
    Width="200"
    Height="200"
    FollowCurrentTime="False"
    Time="{Binding DepartureTime}" />
```

```csharp
clock.FollowCurrentTime = false;
clock.SetTime(new TimeSpan(15, 30, 0));
```

### 🎨 Custom Colors

```xml
<atc:AnalogClock
    Width="200"
    Height="200"
    ClockBackgroundColor="Beige"
    ClockBorderColor="SaddleBrown"
    NumberColor="SaddleBrown"
    SecondHandColor="Red"
    ShowAmPm="False" />
```

### 💾 Saving the Look

```csharp
AnalogClockConfigData data = clock.GetConfigData();   // colours as #AARRGGBB text, plus the Show* options
otherClock.SetByConfigData(data);                      // an empty colour restores the theme brush
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Time` | `TimeSpan` | `00:00:00` | The time of day shown (wrapped into 0:00:00 - 23:59:59) |
| `FollowCurrentTime` | `bool` | `true` | Show the current local time, updated every second while loaded |
| `ShowNumbers` | `bool` | `true` | Show the numbers 1-12 |
| `ShowSeconds` | `bool` | `true` | Show the second hand |
| `ShowAmPm` | `bool` | `true` | Show the AM/PM indicator |
| `IsInAm` | `bool` | `true` | Whether the shown time is before noon (set by the clock) |
| `ClockBackgroundColor` | `Brush?` | Theme background | The clock face |
| `ClockBorderColor` | `Brush?` | Theme foreground | The ring around the face |
| `ClockCenterDotColor` | `Brush?` | Theme foreground | The dot in the centre |
| `HourIndicatorColor` | `Brush?` | Theme foreground | The twelve hour indicators |
| `MinuteIndicatorColor` | `Brush?` | Theme foreground | The minute indicators |
| `NumberColor` | `Brush?` | Theme foreground | The numbers |
| `HourHandColor` | `Brush?` | Theme foreground | The hour hand |
| `MinuteHandColor` | `Brush?` | Theme foreground | The minute hand |
| `SecondHandColor` | `Brush?` | Theme accent | The second hand |
| `AmPmColor` | `Brush?` | Theme foreground | The AM/PM indicator |

## 🧰 Methods

| Method | Description |
|--------|-------------|
| `SetTime(TimeSpan)` | Sets `Time` |
| `GetConfigData()` | Returns the colours and options as `AnalogClockConfigData` |
| `SetByConfigData(AnalogClockConfigData)` | Applies colours and options; an empty colour restores the theme brush |

## 📝 Notes

- The timer only runs while the clock is loaded and `FollowCurrentTime` is on; it stops on `Unloaded`, so a clock in a closed view does not keep ticking
- While `FollowCurrentTime` is on, the next tick replaces a `Time` set from code or a binding
- The hour hand moves on with the minutes and the minute hand with the seconds; the second hand jumps once a second
- The AM/PM text is the AM/PM designator of the current culture (falling back to "AM"/"PM" when the culture has none)
- The numbers use the control's `FontFamily`

## 🔗 Related Controls

- **DigitalClock** - Seven-segment digital clock
- **SevenSegment** - A single seven-segment digit
- **ClockPanelPicker** / **LabelTimePicker** - Picking a time (Atc.Wpf.Forms)

## 🎮 Sample Application

See the AnalogClock sample in the Atc.Wpf.Sample application under **Wpf.Controls > Clocks > AnalogClock** for interactive examples.
