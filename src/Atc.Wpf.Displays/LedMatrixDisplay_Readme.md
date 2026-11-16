# 🔴 LedMatrixDisplay

A chain of 1 to 8 MAX7219 8 x 8 LED matrix modules (the FC-16 style 32 x 8 displays and their relatives) drawn LED by LED, so a desktop app can mirror the display of an ESP32 or Raspberry Pi project exactly.

## 🔍 Overview

`LedMatrixDisplay` draws round LEDs with a soft glow on a dark board, one 8 x 8 module per MAX7219 chip in a row. The control shows a `Max7219ChainBuffer`, the registers of the chips (digits, decode mode, intensity, scan limit, shutdown, display test). You can drive it in three ways:

- **Text** - set `Text`: the registers are written as MD_MAX72XX would for the module type, and every chip is set up as its `begin()` does.
- **Registers** - write the digit registers straight into the buffer (`SetDigit`, `SetShutdown`, ...).
- **Emulator** - feed every chip-select (latch) frame your firmware sends to a `Max7219Controller`.

## 📍 Namespace

```csharp
using Atc.Wpf.Displays;
```

In XAML the control is in the `atc` namespace: `xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"`.

## 🚀 Usage

### Modules in XAML

```xml
<!-- The default: four red FC-16 modules -->
<atc:LedMatrixDisplay Text="Hi ESP" />

<!-- Eight green Generic modules -->
<atc:LedMatrixDisplay ModuleCount="8" Wiring="Generic" LedColor="Green" Text="Hello ESP32" />
```

### Emulator: latch frames

```csharp
var buffer = new Max7219ChainBuffer(4);
var controller = new Max7219Controller(buffer);
display.Buffer = buffer;

// One frame = what is shifted in while CS is low: a (register, data) pair per device, the farthest device first.
controller.WriteData([0x0C, 0x01, 0x0C, 0x01, 0x0C, 0x01, 0x0C, 0x01]);   // every chip on
controller.WriteData([0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x5A]);   // device 0: digit 2 = 0x5A
```

The chain is one long shift register: a frame shorter than the chain makes the far devices execute the words they held before ⚠️ (derived from the datasheet's shift-register description).

### Wirings

Device 0, the first in the chain (nearest the microcontroller's DIN), is the rightmost module. For an LED in row `r` (0 = top) and module column `x` (0 = leftmost):

| `Wiring` | MD_MAX72XX type | Register |
|---|---|---|
| `Fc16` | `FC16_HW` | digit `r`, bit `7 - x` (also LedControl and U8g2) |
| `Generic` | `GENERIC_HW` | digit `7 - x`, bit `7 - r` |
| `Parola` | `PAROLA_HW` | digit `r`, bit `x` |
| `ICStation` | `ICSTATION_HW` | digit `7 - r`, bit `x` |

### Mirror it from a device

Send each chip-select frame as a Latch packet and register `DisplayMirrorTargets.For(controller)` on a [`DisplayMirrorReceiver`](DisplayMirror_Readme.md): Latch packets go to `WriteData`, a Reset packet is the power-on reset. With U8g2 the Arduino tee sends the latches by itself; with MD_MAX72XX or LedControl, send your frames with `sendLatch` (example `Max7219Latch` in [firmware/arduino/AtcDisplayMirror/README.md](../../firmware/arduino/AtcDisplayMirror/README.md)), or `send_latch` from Python ([firmware/python/README.md](../../firmware/python/README.md)).

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ModuleCount` | `int` | `4` | Modules (MAX7219 devices) in the chain, 1 to 8 |
| `Wiring` | `Max7219MatrixWiring` | `Fc16` | How a module's LEDs are wired to the digit registers |
| `LedColor` | `LedColor` | `Red` | `Red`, `Green`, `Blue`, `Yellow` or `White` |
| `Text` | `string` | `""` | Clears the digits and draws the text with 5 x 8 glyphs through `Wiring`; a non-empty text also sets every chip as `begin()` does |
| `CharacterRom` | `CharacterRom` | `A00` | The HD44780 ROM whose glyphs draw `Text` |
| `Buffer` | `Max7219ChainBuffer` | own buffer | The registers to show; resized to `ModuleCount` devices. `null` returns to the control's own buffer (power-on: shut down, dark) |
| `Brightness` | `double` | from intensity | Set from the intensity of device 0: `0.25 + 0.75 * intensity / 15` |
| `GhostOpacity`, `GlowRadius`, `DotCornerRadius` | `double` | from preset | The unlit LEDs, the glow and the dot shape |
| `ShowBezel` | `bool` | `true` | Draws a dark frame around the board |
| `Stretch` | `Stretch` | `Uniform` | How the module fills the size of the control |

## 📝 Notes

- **What is lit**, per device: display test lights every LED; otherwise a shut-down device is dark; otherwise a digit above the scan limit is dark; otherwise the bits of the digit register light the LEDs.
- **Code B decode is applied** as the chip would: a digit whose decode-mode bit is set shows its Code B segment pattern (with D7 kept), not the raw register. Matrix libraries keep decoding off.
- **Brightness comes from device 0 only.** The control has one brightness, so a chain with different intensities per chip is shown at device 0's intensity. The libraries send the same intensity to every chip.
- **`Text` sets the control registers** like MD_MAX72XX `begin()`: display test off, scan limit 7, intensity 7, decode off, shutdown off. An empty text leaves the registers alone, so it never blanks an emulator.
- Multi-row matrices (more than one chain row) are not supported.
- **Threading.** The buffer and the controller are thread-safe; the control redraws on the UI thread, at most once per frame.

## 🔗 Related Controls

- **SevenSegmentDisplay** - a MAX7219 seven-segment module
- **CharacterDisplay** - HD44780 character LCD/VFD
- **DisplayMirrorReceiver** - receives the packets a device sends

## 🎮 Sample Application

See the LedMatrixDisplay sample in the Atc.Wpf.Sample application under **Wpf.Displays > Displays > LedMatrixDisplay**: presets for 1, 4 and 8 modules in every wiring and colour, every setting in the side panel and three feed modes: Text, Frame (scrolling text written into the registers) and Replay (the latch frames of MD_MAX72XX, LedControl or U8g2, with start, stop, step and speed).