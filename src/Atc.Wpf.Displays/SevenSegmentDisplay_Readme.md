# 🔢 SevenSegmentDisplay

A row of 1 to 8 seven-segment digits driven by a MAX7219 chip (the common 8-digit modules), drawn with `SevenSegment` digits, so a desktop app can mirror the display of an ESP32 or Raspberry Pi project.

## 🔍 Overview

`SevenSegmentDisplay` shows device 0 of a `Max7219ChainBuffer`: digit 0 is the rightmost digit, as on common modules. A digit shows its Code B character when its decode-mode bit is set (0-9, -, E, H, L, P or blank) and its raw segment bits otherwise; bit 7 is the decimal point in both modes. The intensity sets the opacity of the digits.

Drive it by writing the registers into the buffer, or by feeding the latch frames your firmware sends to a `Max7219Controller`.

## 📍 Namespace

```csharp
using Atc.Wpf.Displays;
```

In XAML the control is in the `atc` namespace: `xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"`.

## 🚀 Usage

```xml
<atc:SevenSegmentDisplay DigitCount="8" SegmentBrush="Red" DotBrush="Red" Buffer="{Binding Registers}" />
```

```csharp
var buffer = new Max7219ChainBuffer(1);
var controller = new Max7219Controller(buffer);

controller.WriteData([0x0C, 0x01]);   // shutdown off
controller.WriteData([0x0B, 0x07]);   // scan all eight digits
controller.WriteData([0x09, 0xFF]);   // Code B decode for every digit
controller.WriteData([0x01, 0x85]);   // digit 0 = "5" with its decimal point
```

Raw segments (decode off) use bit 7 = DP, bit 6 = A, bit 5 = B ... bit 0 = G, LedControl's `setChar` bytes.

### Mirror it from a device

Send each chip-select frame as a Latch packet and register `DisplayMirrorTargets.For(controller)` on a [`DisplayMirrorReceiver`](DisplayMirror_Readme.md). With LedControl or your own SPI code, send the two bytes of each `spiTransfer` with `sendLatch` ([firmware/arduino/AtcDisplayMirror/README.md](../../firmware/arduino/AtcDisplayMirror/README.md)) or `send_latch` from Python ([firmware/python/README.md](../../firmware/python/README.md)). A Describe gives the digit count as its width and 1 as its height.

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DigitCount` | `int` | `8` | Digits shown, 1 to 8: MAX7219 digits 0 (rightmost) to `DigitCount - 1` |
| `Buffer` | `Max7219ChainBuffer` | own buffer | The registers to show (device 0 only). `null` returns to the control's own buffer (power-on: shut down, dark) |
| `SegmentBrush` | `Brush?` | accent | Brush of the segments; `null` uses the theme's accent brush |
| `DotBrush` | `Brush?` | accent | Brush of the decimal points; `null` uses the theme's accent brush |

## 📝 Notes

- **What is lit**: display test lights every segment and decimal point at full brightness; otherwise a shut-down chip is dark; otherwise a digit above the scan limit is dark.
- ⚠️ Which physical digit is digit 0 depends on the module; common 8-digit modules have it on the right, which is what the control shows.
- LedControl's `setDigit` hex letters A-F are its own font sent as raw segments, not Code B.
- Other devices of a chained buffer are not shown.
- **Threading.** Buffer changes may come from any thread; they are shown on the UI thread.
- **Accessibility.** The control is an image to UI Automation, named after its digit count (for example `Seven-segment display, 8 digits`); set `AutomationProperties.Name` to describe the content.

## 🔗 Related Controls

- **LedMatrixDisplay** - MAX7219 8 x 8 LED matrix chains
- **SevenSegment** / **DigitalClock** - the digit this control is built from, and a clock
- **DisplayMirrorReceiver** - receives the packets a device sends

## 🎮 Sample Application

See the SevenSegmentDisplay sample in the Atc.Wpf.Sample application under **Wpf.Displays > Displays > SevenSegmentDisplay**: presets for 4 and 8 digits, the digit count, colour and intensity in the side panel and three feed modes: Text (Code B, with `.` for a decimal point), Frame (a stopwatch written as raw segments) and Replay (LedControl's latch frames: init, "HELP 1.23", then Code B "-12.34567", with start, stop, step and speed).