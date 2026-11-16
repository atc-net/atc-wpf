# 📰 EPaperDisplay

An SSD1680 e-paper module (the 2.13" 122 x 250 and 2.9" 128 x 296 panels, black/white or black/white/red) drawn pixel by pixel, so a desktop app can mirror the e-paper of an ESP32 or Raspberry Pi project exactly, including its refresh flash.

## 🔍 Overview

`EPaperDisplay` draws the paper-grey panel, black ink and (on a black/white/red panel) red ink, inside an optional bezel. As on the real panel, writing the controller RAM changes nothing on the screen: only a refresh shows it. A full refresh plays the panel's flash (the picture inverted, then white, then the picture); a partial refresh is shown at once.

The control shows an `EPaperFrameBuffer`: the two RAM planes and, separately, the shown image. You can drive it in three ways:

- **Text** - set `Text`; the RAM is cleared to white, the text is drawn in black upright on the screen (as turned by `Rotation`) and the panel gets a full refresh.
- **Whole frames** - send the planes of the device's frame buffer and call `EPaperFrameBuffer.LoadRam` then `Refresh(EPaperRefreshKind.Full)` (or `Partial`), or send an EPaperFrame packet to a `DisplayMirrorReceiver`.
- **Emulator** - feed the exact command and data bytes your firmware sends (GxEPD2, Adafruit_EPD) to an `Ssd1680Controller`.

## 📍 Namespace

```csharp
using Atc.Wpf.Displays;
```

In XAML the control is in the `atc` namespace: `xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"`.

## 🚀 Usage

### Modules in XAML

```xml
<!-- The default: a black/white 2.9" 128 x 296 panel in landscape (Rotation 90) -->
<atc:EPaperDisplay Text="Hello, ESP32!" />

<!-- A 2.13" black/white/red panel: 128-pixel-wide rows (as GxEPD2 and Adafruit_EPD send them), 122 visible -->
<atc:EPaperDisplay PixelColumns="128" VisibleColumns="122" PixelRows="250" HasRed="True" Text="2.13 inch" />

<!-- Portrait, no flash -->
<atc:EPaperDisplay Rotation="0" ShowRefreshFlash="False" Text="Portrait" />
```

### Whole frames

```csharp
var buffer = new EPaperFrameBuffer(128, 296, hasRed: true);
display.Buffer = buffer;

using (buffer.BeginBatch())
{
    buffer.LoadRam(blackPlane, redPlane);   // 16 bytes per row, 296 rows each
    buffer.Refresh(EPaperRefreshKind.Full);
}
```

The planes are in the native (portrait) orientation and the SSD1680 RAM polarity:

| Plane | RAM | Bit 1 | Bit 0 |
|---|---|---|---|
| black | `0x24` | white | black |
| red | `0x26` | red | no red |

Rows are padded to whole bytes (122 pixels = 16 bytes), bit 7 is the leftmost pixel. A red bit wins over the black plane. GxEPD2's `_black_buffer` and Adafruit_EPD's buffers are already in this polarity. ⚠️ A **GxEPD2_3C colour buffer is 0 = red**: the device must invert it (`~byte`) before it sends it, as GxEPD2 itself does on its way to RAM `0x26`.

### Emulator

```csharp
var buffer = new EPaperFrameBuffer(128, 296);
var controller = new Ssd1680Controller(buffer);   // the RAM X offset: 0 for GxEPD2

controller.WriteCommands([0x24]);                  // D/C low
controller.WriteData(blackBytes);                  // D/C high
controller.WriteCommands([0x22]);
controller.WriteData([0xF7]);
controller.WriteCommands([0x20]);                  // master activation: a full refresh
```

The emulator keeps the chip's 176 x 296 RAM per plane and maps the panel's part of it into the buffer. `0x20` refreshes as the last `0x22` value says: `F7`, `F4`, `C7` full; `FC`, `CC`, `0C`, `CF` partial; the power, clock and LUT values refresh nothing. Adafruit_EPD starts some panels at RAM byte 1 (`ThinkInk_290_Mono_BN`, `ThinkInk_213_Mono_BN`, the `_RH` and `_RW` tri-colour panels): set `RamXOffsetBytes = 1` for those, or the picture is 8 pixels off.

### Rotation: GxEPD2 and Adafruit_EPD

`Rotation` is GxEPD2's `setRotation(Rotation / 90)`: the default 90 shows a sketch that calls `setRotation(1)` (the usual landscape mounting) upright. Adafruit_EPD constructs the panel in landscape and its rotation 0 is GxEPD2's `setRotation(3)`: use **270** for an Adafruit_EPD sketch at its default rotation.

### The 2.13" panels: 128-pixel rows, 122 visible

The 2.13" glass shows 122 columns, but its RAM rows are 16 bytes (128 bits, bits 122 to 127 are padding), and both libraries keep a
128-pixel-wide buffer: GxEPD2 (`WIDTH = 128`, `WIDTH_VISIBLE = 122`) and Adafruit_EPD (the 122 short side rounded up to 128). Their
frames are 128 x 250, so set `PixelColumns = 128` (the buffer and the frames) and `VisibleColumns = 122` (what the glass shows). The
control crops the columns past `VisibleColumns` before it turns the picture, and `Rotation` and `Text` work on the 122 visible
columns, as GxEPD2's `drawPixel` does (it rotates against the visible width its graphics base class was constructed with): a
`setRotation(1)` pixel (x, y), stored at native (121 - y, x), shows at screen (x, y).

### Mirror it from a device

Send EPaperFrame packets (or the SSD1680 byte stream as Commands and Data) with the [display mirroring](DisplayMirror_Readme.md) helpers and register `DisplayMirrorTargets.For(buffer)` (and `DisplayMirrorTargets.For(controller)` for the byte stream) on a `DisplayMirrorReceiver`. The Arduino library's `EPaperFrame` example draws with GxEPD2 and calls `sendEPaperFrame` ([firmware/arduino/AtcDisplayMirror/README.md](../../firmware/arduino/AtcDisplayMirror/README.md)); the Python helper has `send_epaper_frame` and `send_image` ([firmware/python/README.md](../../firmware/python/README.md)). Send a full refresh only when the device does one: each plays the flash.

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `PixelColumns` | `int` | `128` | Native (portrait) columns of the buffer and the frames, 1 to 400: 128 for the 2.9" and the 2.13" |
| `VisibleColumns` | `int?` | `null` | Native columns the glass shows, from column 0 (`null` = all; coerced to 1 to `PixelColumns`): 122 for the 2.13" |
| `PixelRows` | `int` | `296` | Native rows, 1 to 400: 296 for the 2.9", 250 for the 2.13" |
| `HasRed` | `bool` | `false` | Black/white/red panel; a black/white panel never shows the red plane |
| `Rotation` | `int` | `90` | 0, 90, 180 or 270: GxEPD2 `setRotation(Rotation / 90)` |
| `ShowRefreshFlash` | `bool` | `true` | A full refresh plays the flash; otherwise it is shown at once |
| `RefreshFlashDuration` | `TimeSpan` | `600 ms` | The whole flash, three equal phases |
| `Text` | `string` | `""` | Clears the RAM, draws the text upright and refreshes fully |
| `Buffer` | `EPaperFrameBuffer` | own buffer | The panel to show; resized to the pixel size and given `HasRed`. `null` returns to the control's own buffer |
| `PaperBrush` | `Brush` | paper grey | Colour of white |
| `InkBrush` | `Brush` | near black | Colour of black |
| `RedInkBrush` | `Brush` | dark red | Colour of red |
| `ShowBezel` | `bool` | `true` | Draws a dark frame around the panel |
| `BezelBrush` | `Brush?` | `Black` | Colour of the bezel |
| `Stretch` | `Stretch` | `Uniform` | How the module fills the size of the control |

## 📝 Notes

- **Black/white panels ignore the red RAM.** GxEPD2 keeps the previous picture in RAM `0x26` and Adafruit_EPD mono panels write an unused buffer there, so it is not an image on a black/white panel.
- **Size and offset come from the panel**, never from the `0x01` or `0x21` commands: GxEPD2_213_BN sends 296 MUX lines to its 250-row panel.
- **Deep sleep.** `0x10 0x01` puts the chip to sleep; a real chip wakes only on its reset pin, which a mirror rarely sees. The emulator wakes leniently on `0x12`, `0x20`, `0x22`, `0x24`, `0x26` or a Reset ⚠️ (a design choice, not chip behaviour).
- ⚠️ Derived, not confirmed on hardware: the bit meaning of `0x22` (an unknown value is taken as a full refresh), the counter wrap at the window edge, the pixel order for X decrement, no automatic copy of `0x24` to `0x26`, the RAM content at power-on (white, no red) and the RAM X offset mapping.
- **Grey levels** (4-grey LUTs) are not emulated: the black/white bit decides.
- **Threading.** The buffer and the controller are thread-safe; the control redraws on the UI thread, at most once per frame.
- **Accessibility.** The control is an image to UI Automation, named after its resolution.

## 🔗 Related Controls

- **OledDisplay** - monochrome OLED with an SSD1306 emulator
- **GraphicLcdDisplay** - ST7920 128 x 64 graphic LCD
- **DisplayMirrorReceiver** - receives the packets a device sends

## 🎮 Sample Application

See the EPaperDisplay sample in the Atc.Wpf.Sample application under **Wpf.Displays > Displays > EPaperDisplay**: presets for the 2.13" and 2.9" panels in black/white and black/white/red, every setting in the side panel (rotation, bezel, refresh flash and its length, text) and three feed modes: Text, Frame (planes loaded with `LoadRam` once a second, a full refresh every fifth frame) and Replay (GxEPD2's init, clear screen, a picture, a partial refresh on black/white panels and hibernate, with start, stop, step and speed).