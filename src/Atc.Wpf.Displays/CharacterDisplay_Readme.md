# 📟 CharacterDisplay

A character LCD/VFD module (HD44780 and compatibles) drawn dot by dot, so a desktop app can mirror the display of an ESP32 or Raspberry Pi project exactly.

## 🔍 Overview

`CharacterDisplay` draws a 1 to 40 column by 1 to 4 row character module with 5 x 8 dot cells: the panel, the faint unlit ("ghost") dots, the lit dots, an optional bezel and, for a VFD, a soft glow. It renders the real HD44780 character ROMs (A00 and A02), up to 8 custom glyphs and the underline/blinking cursor.

The control shows a `CharacterDisplayBuffer`, the model of the controller chip (DDRAM, CGRAM, display shift, cursor, display on/off). You can drive it in two ways:

- **Properties** - set `Text`, `CursorMode`, `CustomCharacters` and so on, or call `Write`, `DefineCharacter` and `Clear`.
- **Emulator** - feed the exact bytes your firmware sends to an `Hd44780Controller` (logical instructions and data), or the raw bytes a PCF8574 I2C backpack receives to a `Pcf8574Hd44780Decoder`.

## 📍 Namespace

```csharp
using Atc.Wpf.Displays;
```

In XAML the control is in the `atc` namespace: `xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"`.

## 🚀 Usage

### Presets in XAML

```xml
<!-- The default: a 16 x 2 yellow-green LCD -->
<atc:CharacterDisplay Text="Hello, ESP32!&#10;Line two" />

<!-- A 20 x 4 blue LCD with a blinking cursor -->
<atc:CharacterDisplay
    Columns="20"
    Rows="4"
    Color="Blue"
    CursorMode="Blink"
    CursorRow="3"
    CursorColumn="0"
    Text="Line 1&#10;Line 2&#10;Line 3&#10;&gt;" />

<!-- A 16 x 2 amber VFD, dimmed, without bezel -->
<atc:CharacterDisplay
    Kind="Vfd"
    Color="Amber"
    Brightness="0.5"
    ShowBezel="False"
    Text="VFD 16x2" />

<!-- The reflective panel of an LCD with the backlight off -->
<atc:CharacterDisplay IsBacklightOn="False" Text="Backlight off" />
```

### Text from a view model

```xml
<atc:CharacterDisplay
    Columns="20"
    Rows="4"
    Text="{Binding DisplayText}" />
```

```csharp
[ObservableProperty]
private string displayText = string.Empty;

private void OnReading(double temperature, DateTime time)
    => DisplayText = string.Create(
        CultureInfo.InvariantCulture,
        $"{time:HH:mm:ss}\nTemp {temperature,5:0.0}°C");
```

`Text` is split on `\n` (`\r` is ignored); each line is written from column 0 of its row. There is no wrapping: characters past the last column and lines past the last row are dropped. Characters are mapped through `CharacterRom` (the `°` above is code 0xDF in A00 and 0xB0 in A02); a character the ROM does not have shows as a space.

### Custom glyphs

The codes `\u0000` to `\u0007` show the 8 custom glyphs. A glyph is 8 rows, row 0 at the top, using the low 5 bits of each row (bit 4 is the leftmost dot).

```csharp
// In code: define glyph 0 as a degree sign and use it.
display.DefineCharacter(0, [0x0C, 0x12, 0x12, 0x0C, 0x00, 0x00, 0x00, 0x00]);
display.Text = "Temperature\n21.5\u0000C";
```

```xml
<!-- From a view model: CustomCharacters is an IList<byte[]>, slot 0 first -->
<atc:CharacterDisplay CustomCharacters="{Binding Glyphs}" Text="{Binding Text}" />
```

`CustomCharacters` is written into the glyph RAM when the property is set; later changes to the same list are not followed, so assign a new list to change the glyphs.

### Emulator: mirror a device from a serial or MQTT thread

Bind the control to a buffer owned by the view model and feed an `Hd44780Controller` with the bytes the device sends. The buffer is thread-safe, so the controller can run on the thread that receives the bytes; the control redraws on the UI thread, at most once per frame.

The device sends the instructions as Commands packets and the characters as Data packets of the [display mirroring protocol](../../docs/Displays/DisplayMirrorProtocol.md) (or the raw backpack bytes as I2cWrite packets, see below), and a [`DisplayMirrorReceiver`](DisplayMirror_Readme.md) applies them. The CRC of each packet lets the receiver find the next good packet after line noise.

```csharp
public sealed class LcdMirrorViewModel : ViewModelBase
{
    private readonly DisplayMirrorReceiver receiver = new();

    public LcdMirrorViewModel()
    {
        var controller = new Hd44780Controller(Buffer);

        // Commands = instructions (RS low), Data = characters (RS high); I2cWrite = the bytes a PCF8574 backpack receives.
        receiver.Register(0, DisplayMirrorTargets.Combine(
            DisplayMirrorTargets.For(controller),
            DisplayMirrorTargets.For(new Pcf8574Hd44780Decoder(controller))));
    }

    public CharacterDisplayBuffer Buffer { get; } = new(20, 4);

    public async Task MirrorAsync(
        string portName,
        CancellationToken cancellationToken)
    {
        using var port = new SerialPort(portName, 115200);
        port.Open();
        await receiver.RunAsync(port.BaseStream, cancellationToken);
    }
}
```

```xml
<atc:CharacterDisplay Columns="20" Rows="4" Buffer="{Binding Buffer}" />
```

Over MQTT or UDP send one packet per message and call `receiver.ProcessMessage(payload)`; see the receiver readme. To drive the emulator yourself, `Hd44780Controller.Command(instruction)` and `Data(value)` take one byte, and `Write(bytes, isData)` applies a run of instructions or data bytes and raises `CharacterDisplayBuffer.Changed` once for the run. A 40 x 4 module has two controller chips (enable lines E1 and E2): create two controllers on the same buffer, `new Hd44780Controller(buffer, 0)` for rows 1-2 and `new Hd44780Controller(buffer, 1)` for rows 3-4, and give each its own display id.

### Emulator: PCF8574 I2C backpack

Most ESP32 and Raspberry Pi projects drive the display through a PCF8574 "backpack" (for example with LiquidCrystal_I2C or RPLCD). `Pcf8574Hd44780Decoder` takes the raw bytes written to the expander (from an I2C sniffer or a bus capture), latches the nibbles on the falling edge of EN, handles the 4-bit initialization and applies the result to a controller. Its `IsBacklightOn` and `BacklightChanged` report the backlight bit.

```csharp
var controller = new Hd44780Controller(Buffer);
var decoder = new Pcf8574Hd44780Decoder(controller);
decoder.BacklightChanged += (_, _) =>
    Application.Current.Dispatcher.InvokeAsync(() => IsBacklightOn = decoder.IsBacklightOn);

// On the capture thread:
decoder.Write(i2cBytes);
```

The default pin map is the common LiquidCrystal_I2C wiring (P0 = RS, P1 = RW, P2 = EN, P3 = backlight, P4 to P7 = D4 to D7). A backpack wired differently takes its own map (bit positions 0 to 7 in the expander byte), for example one with the data nibble on P0 to P3:

```csharp
var pinMap = new Pcf8574PinMap
{
    Data4 = 0, // D4 to D7 on bits 0 to 3
    Enable = 4,
    Rw = 5,
    Rs = 6,
    Backlight = 7,
};

var decoder = new Pcf8574Hd44780Decoder(controller, pinMap);
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Columns` | `int` | `16` | Character columns of the module, 1 to 40 |
| `Rows` | `int` | `2` | Character rows of the module, 1 to 4 |
| `Kind` | `CharacterDisplayKind` | `Lcd` | `Lcd` or `Vfd` (glowing round dots on dark glass) |
| `Color` | `CharacterDisplayColor` | `YellowGreen` | Colour preset (see below) |
| `IsBacklightOn` | `bool` | `true` | LCD backlight; off shows the reflective grey-green panel. A VFD ignores it |
| `Text` | `string` | `""` | Lines separated by `\n`, each written from column 0 |
| `CharacterRom` | `CharacterRom` | `A00` | `A00` (ASCII + Japanese katakana) or `A02` (ASCII + Western European, Cyrillic, Greek) |
| `CursorMode` | `CharacterDisplayCursor` | `None` | `None`, `Underline`, `Blink` or `UnderlineAndBlink` |
| `CursorRow` | `int` | `0` | Row of the cursor, clamped to the module |
| `CursorColumn` | `int` | `0` | Column of the cursor, clamped to the module |
| `IsDisplayOn` | `bool` | `true` | Off blanks the dots but keeps the panel and the backlight |
| `CustomCharacters` | `IList<byte[]>?` | `null` | Up to 8 glyphs for the codes `\u0000` to `\u0007` |
| `Buffer` | `CharacterDisplayBuffer` | own buffer | The display state to show; it takes over the content properties set on the control. `null` returns to the control's own buffer |
| `Brightness` | `double` | `1.0` | Intensity of the lit dots, 0.25 to 1.0 |
| `GhostOpacity` | `double` | from preset | Opacity of the unlit dots, 0 to 1 |
| `ShowBezel` | `bool` | `true` | Draws a dark frame around the panel |
| `BezelBrush` | `Brush?` | `Black` | Colour of the bezel |
| `PanelBrush` | `Brush?` | from preset | Colour of the panel |
| `LitDotBrush` | `Brush?` | from preset | Colour of a lit dot |
| `UnlitDotBrush` | `Brush?` | from preset | Colour of an unlit dot; `null` uses `LitDotBrush` |
| `DotCornerRadius` | `double` | from preset | 0 (square) to 0.5 (round) |
| `GlowRadius` | `double` | from preset | Glow around lit dots as a fraction of a dot; 0 draws none |
| `Stretch` | `Stretch` | `Uniform` | How the module fills the size of the control |

Methods: `Write(row, column, text)`, `DefineCharacter(index, glyphRows)` and `Clear()` write into the buffer.

## 🎨 Colour Presets

| `Color` | LCD (backlight on) | VFD |
|---------|--------------------|-----|
| `YellowGreen` | Dark dots on yellow-green | Blue-green |
| `Blue` | Light dots on blue | Blue-green |
| `WhiteOnBlack` | White dots on black | Blue-green |
| `Amber` | Dark dots on amber | Amber |
| `Red` | Light dots on red | Red |
| `Green` | Dark dots on green | Green |
| `BlueGreen` | Dark dots on yellow-green | Blue-green |

With the backlight off, every LCD preset shows the reflective grey-green panel. A VFD shows an LCD-only preset as blue-green, with round dots and a glow.

## 📐 Sizes

| Size | Controllers | DDRAM address of rows 1 / 2 / 3 / 4 |
|------|-------------|-------------------------------------|
| 8 x 1, 16 x 1 | 1 | 0x00 |
| 8 x 2, 10 x 2, 16 x 2, 20 x 2, 24 x 2, 40 x 2 | 1 | 0x00 / 0x40 |
| 16 x 4 | 1 | 0x00 / 0x40 / 0x10 / 0x50 |
| 20 x 4 | 1 | 0x00 / 0x40 / 0x14 / 0x54 |
| 40 x 4 | 2 | 0x00 / 0x40 on controller 0, 0x00 / 0x40 on controller 1 |

Rows 3 and 4 of a one-controller 4-row module continue rows 1 and 2 in DDRAM, as on the real module, so `lcd.setCursor(0, 2)` on a 20 x 4 sends `0x80 | 0x14`. `CharacterDisplayBuffer.AddressOf(row, column, columns, rows, out controller)` gives the address of any cell (it throws for a cell outside the module).

DDRAM addresses passed to `CharacterDisplayBuffer.SetDdram` and `CursorAddress` are 0x00 to 0x7F, like the chip's 7-bit address counter; anything else throws `ArgumentOutOfRangeException`. An address wraps at 0x28 within its line, so 0x28 to 0x3F are the same cells as 0x00 to 0x17, and 0x68 to 0x7F the same as 0x40 to 0x57.

## 📝 Notes

- **One way into the buffer.** `Text`, `CursorMode`, `CursorRow`, `CursorColumn`, `IsDisplayOn` and `CustomCharacters` write into the buffer when they change. An emulator's changes are shown, but they are not reflected back into these properties.
- **Resizing re-applies the text.** Changing `Columns` or `Rows` keeps the DDRAM content at its addresses and writes a non-empty `Text` again, so growing the module shows all of it. Set `Text` to empty before an emulator takes over, or a resize writes the old text over the device's screen. Changing `CharacterRom` also re-applies a non-empty `Text`.
- **No wrapping.** `Text` splits on `\n`, ignores `\r` and drops what does not fit.
- **A supplied buffer takes over the content properties set on the control**, whatever the order in which they (or their bindings) arrive: a non-empty `Text`, non-null `CustomCharacters`, and `CursorMode`, `IsDisplayOn`, `CursorRow` and `CursorColumn` when they are set locally, by a style or by a binding (even to their default value). Properties left on their default do not overwrite an emulator buffer's own cursor and display state. A supplied buffer is also resized to the control's `Columns` x `Rows`.
- **Threading.** `CharacterDisplayBuffer` and `Hd44780Controller` are safe to use from any thread; `Changed` and `BacklightChanged` are raised on the thread that wrote. The members of `Pcf8574Hd44780Decoder` are locked too, but pairing nibbles depends on the byte order, so feed it from one thread. The decoder applies the decoded bytes while it holds its lock, so `CharacterDisplayBuffer.Changed` may be raised under that lock: a `Changed` handler must not block on the UI thread (for example with `Dispatcher.Invoke`); use `InvokeAsync`. `IsBacklightOn` is read without the lock. The control marshals redraws to the UI thread and coalesces them to at most one per frame.
- **PCF8574 decoder.** It is meant to be fed from one thread, in order. A capture that starts in the middle of a stream is out of step until the next initialization sequence (0x3, 0x3, 0x3, 0x2), where the decoder resyncs. Read cycles (RW high) are ignored, and no byte value throws.
- **40 x 4 uses two controllers.** Use two `Hd44780Controller` instances on one buffer, index 0 and 1. Display on/off and the cursor style are shared, so the last display control command wins.
- **1-line mode is not blanked on multi-row modules.** A function set with N = 0 makes the emulator address the DDRAM as one line of 80 characters, but rows that a 1-line chip would not drive are not blanked: characters past the 40th show on the row at 0x40. A display shift in 1-line mode also wraps every 40 characters, not 80. LiquidCrystal and RPLCD only pass through 1-line mode during initialization, so this rarely shows.
- **A02 glyphs use the bottom (cursor) row**, as on the real chip, so an underline cursor overlaps them.
- **Colours.** A brush, dot shape or ghost opacity set on the control (locally, by a style or a binding) wins over the preset. Clearing a locally set brush falls back to the base default until `Kind`, `Color` or `IsBacklightOn` changes and the preset is applied again.
- **Accessibility.** The automation name is the visible text (custom glyphs read as spaces). It is not re-announced when the text changes, and it is still reported when the display is off.
- **Idle CPU.** The control draws in `OnRender` from frozen geometries that are rebuilt only when the dots change. The blinking cursor runs a 400 ms timer only while the control is loaded and visible; otherwise nothing runs.

## 🔗 Related Controls

- **SevenSegment** / **DigitalClock** - seven-segment digits and clock
- **AnalogClock** - analog clock face
- **Card** - a container to put the display in

## 🎮 Sample Application

See the CharacterDisplay sample in the Atc.Wpf.Sample application under **Wpf.Displays > Displays > CharacterDisplay**: every property in the side panel, custom glyph demos (thermometer, bar graph) and an emulator that plays or steps a captured 20 x 4 clock + temperature screen, as logical HD44780 bytes or as raw PCF8574 I2C bytes (the backpack's backlight bit drives `IsBacklightOn`).