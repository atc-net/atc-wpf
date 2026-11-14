# 🔢 SevenSegment

A single seven-segment display digit (0-9) with an optional colon and dot, a glow and an animated change of digit.

## 🔍 Overview

`SevenSegment` draws one digit the way an LED or LCD display does: seven segments, lit or dimmed. Dimmed segments stay faintly visible (`NumberMinimumOpacity`), a blurred copy behind the segments gives a glow (`BlurRadius`), and a change of digit can fade the segments (`PlayAnimation`). It scales to the space it gets with a 3:5 width:height ratio. It is the building block of `DigitalClock`, and can be used on its own for counters, scores or other numeric read-outs.

Unset brushes use the theme accent brush (`AtcApps.Brushes.Accent`).

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Clocks;
```

## 🚀 Usage

### Basic Example

```xml
<atc:SevenSegment Width="30" Height="50" Digit="7" />
```

### 🔢 A Row of Digits

```xml
<StackPanel Orientation="Horizontal">
    <atc:SevenSegment Width="30" Height="50" Margin="0,0,10,0" Digit="{Binding Tens}" />
    <atc:SevenSegment Width="30" Height="50" Digit="{Binding Ones}" ShowDot="True" />
</StackPanel>
```

### 🎨 Custom Look

```xml
<atc:SevenSegment
    Width="60"
    Height="100"
    BlurRadius="10"
    Digit="3"
    NumberColor="LimeGreen"
    NumberMinimumOpacity="0"
    PlayAnimation="False" />
```

### 🔎 Segment Mapping

```csharp
SevenSegmentParts parts = SevenSegment.GetSegments(4);
// TopLeft | TopRight | Middle | BottomRight
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Digit` | `int?` | `0` | The digit (0-9); `null` lights no segment |
| `LitSegments` | `SevenSegmentParts` | - | The segments lit for `Digit` (read-only) |
| `NumberColor` | `Brush?` | Theme accent | The segments |
| `ColonColor` | `Brush?` | Theme accent | The colon and the dot |
| `NumberMinimumOpacity` | `double` | `0.05` | Opacity of an unlit segment (0 hides it) |
| `BlurRadius` | `int` | `5` | Blur radius of the glow (0 = no glow) |
| `PlayAnimation` | `bool` | `false` | Fade the segments when the digit changes |
| `AnimationDuration` | `TimeSpan` | `00:00:00.05` | Duration of the fade |
| `ShowColon` | `bool` | `false` | Show a colon to the right of the digit |
| `ShowDot` | `bool` | `false` | Show a dot to the lower right of the digit |
| `AnimateColon` | `bool` | `false` | Let `PulseColon()` fade the colon in and out |

## 🧰 Methods

| Method | Description |
|--------|-------------|
| `SetNumber(int)` | Sets `Digit` (0-9) |
| `PulseColon()` | Fades the colon in and out once over a second (when `AnimateColon` is on) |
| `GetSegments(int?)` | (static) The segments lit for a digit |

## 📝 Notes

- `Digit` only accepts 0-9 or `null`; other values throw
- The colon and the dot are drawn just right of the digit cell (outside the control's bounds), so leave a margin to the right when you show them
- With `PlayAnimation` off, the segments switch at once

## 🔗 Related Controls

- **DigitalClock** - A clock built from six seven-segment digits
- **AnalogClock** - Analog clock face

## 🎮 Sample Application

See the SevenSegment sample in the Atc.Wpf.Sample application under **Wpf.Controls > Clocks > SevenSegment** for interactive examples.
